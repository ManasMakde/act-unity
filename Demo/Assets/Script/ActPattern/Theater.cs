using System;
using System.Collections.Generic;
using UnityEngine;


public class Theater : MonoBehaviour
{
    // Public Signals
    public event Action<Theater, bool> OnEnableChanged;
    public event Action<Theater, Act> OnPerformStart;
    public event Action<Theater, Act> OnPerformEnd;
    public event Action<Theater> OnAllPerformEnd;


    // Public Methods
    public bool IsEnabled()
    {
        return _isEnabled;
    }
    public void SetEnabled(bool newEnabled)
    {
        if (newEnabled == _isEnabled)
        {
            return;
        }

        _isEnabled = newEnabled;

        if (!_isEnabled)
        {
            AbortAll();
        }

        OnEnableChanged?.Invoke(this, _isEnabled);
    }
    public void AbortAll()
    {
        // Return if already in between aborting all
        if (_isAbortingAll)
        {
            return;
        }


        // Guard to avoid mutation
        _isAbortingAll = true;


        // Abort all acts
        foreach (Act act in _allActs)
        {
            act.Abort();
        }


        // Reset guard
        _isAbortingAll = false;


        // Apply pending adds & removes after loop
        foreach (Act act in _pendingModActs.Keys)
        {
            if (_pendingModActs[act])
            {
                _allActs.Add(act);
            }
            else
            {
                _allActs.Remove(act);
            }
        }
        _pendingModActs.Clear();
    }
    public bool AreAnyOngoing()
    {
        return _ongoingActs.Count != 0;
    }
    public HashSet<Act> GetAllActs()
    {
        return new HashSet<Act>(_allActs);
    }


    // Private Properties
    private HashSet<Act> _allActs = new();
    private HashSet<Act> _ongoingActs = new();
    private Dictionary<Act, Act.TickFlags> _deferredActs = new();
    private Dictionary<Act, bool> _pendingModActs = new();
    private Dictionary<Act, bool> _stagedTickActs = new();
    private Dictionary<Act, bool> _stagedPhysicsTickActs = new();
    private Dictionary<Act, bool> _stagedLateTickActs = new();
    private Dictionary<Act, bool> _actsToTick = new();
    private Dictionary<Act, bool> _actsToPhysicsTick = new();
    private Dictionary<Act, bool> _actsToLateTick = new();
    private bool _isAbortingAll = false;
    private bool _isEnabled = true;


    // Private Staging Methods
    public void AddAct(Act newAct)
    {
        // Return if null act
        if (newAct == null)
        {
            return;
        }


        // Mark as pending if abort all is ongoing
        if (_isAbortingAll)
        {
            _pendingModActs[newAct] = true;
            return;
        }

        _allActs.Add(newAct);
    }
    public void RemoveAct(Act oldAct)
    {
        // Return if null act
        if (oldAct == null)
        {
            return;
        }


        // Mark as pending if abort all is ongoing
        if (_isAbortingAll)
        {
            _pendingModActs[oldAct] = false;
            return;
        }

        _allActs.Remove(oldAct);
    }
    public void StageOngoing(Act act)
    {
        // Return if invalid act or already ongoing
        if (act == null || _ongoingActs.Contains(act))
        {
            return;
        }


        // Mark as ongoing act
        _ongoingActs.Add(act);


        // Clear defer
        UnstageDeferred(act);


        // Broadcast act started
        OnPerformStart?.Invoke(this, act);
    }
    public void UnstageOngoing(Act act)
    {
        // Return if act is null or was never staged ongoing
        if (act == null || !_ongoingActs.Contains(act))
        {
            return;
        }


        // Remove as ongoing act
        _ongoingActs.Remove(act);


        // Broadcast act ended
        OnPerformEnd?.Invoke(this, act);


        // Broadcast all ended if none ongoing
        if (!AreAnyOngoing())
        {
            OnAllPerformEnd?.Invoke(this);
        }
    }
    public void StageDeferred(Act act, Act.TickFlags flag)
    {
        if (act == null)
        {
            return;
        }

        _deferredActs[act] = _deferredActs.ContainsKey(act) ? (_deferredActs[act] | flag) : flag;
    }
    public void UnstageDeferred(Act act)
    {
        if (act == null)
        {
            return;
        }

        _deferredActs.Remove(act);
    }
    public void StageTick(Act act)
    {
        if (act == null)
        {
            return;
        }

        _stagedTickActs[act] = true;
    }
    public void UnstageTick(Act act)
    {
        if (act == null)
        {
            return;
        }


        // Remove if not reference swapped yet else mark as pending removal
        if (_stagedTickActs.ContainsKey(act))
        {
            _stagedTickActs.Remove(act);
        }
        else if (_actsToTick.ContainsKey(act))
        {
            _stagedTickActs[act] = false;
        }
    }
    public void StagePhysicsTick(Act act)
    {
        if (act == null)
        {
            return;
        }

        _stagedPhysicsTickActs[act] = true;
    }
    public void UnstagePhysicsTick(Act act)
    {
        if (act == null)
        {
            return;
        }


        // Remove if not reference swapped yet else mark as pending removal
        if (_stagedPhysicsTickActs.ContainsKey(act))
        {
            _stagedPhysicsTickActs.Remove(act);
        }
        else if (_actsToPhysicsTick.ContainsKey(act))
        {
            _stagedPhysicsTickActs[act] = false;
        }
    }
    public void StageLateTick(Act act)
    {

        if (act == null)
        {
            return;
        }

        _stagedLateTickActs[act] = true;
    }
    public void UnstageLateTick(Act act)
    {
        if (act == null)
        {
            return;
        }


        // Remove if not reference swapped yet else mark as pending removal
        if (_stagedLateTickActs.ContainsKey(act))
        {
            _stagedLateTickActs.Remove(act);
        }
        else if (_actsToLateTick.ContainsKey(act))
        {
            _stagedLateTickActs[act] = false;
        }
    }


    // Private Methods
    private static void TickActs(ref Dictionary<Act, bool> stagedActs, ref Dictionary<Act, bool> actsToTick, Act.TickFlags flag)
    {
        // Return if no act to process
        if (stagedActs.Count == 0)
        {
            return;
        }


        // Reference swap to avoid mutation
        actsToTick = stagedActs;
        stagedActs = new();


        // Tick all acts based on flag
        foreach (Act act in actsToTick.Keys)
        {
            if (flag == Act.TickFlags.Tick)
            {
                act.TickImpl();
            }
            else if (flag == Act.TickFlags.PhysicsTick)
            {
                act.PhysicsTickImpl();
            }
            else if (flag == Act.TickFlags.LateTick)
            {
                act.LateTickImpl();
            }
        }


        // Merge back & clear
        MergeDict(stagedActs, actsToTick, false);
        actsToTick.Clear();


        // Filter
        var filter = new List<Act>();
        foreach (Act act in stagedActs.Keys)
        {
            if (!stagedActs[act])
            {
                filter.Add(act);
            }
        }
        foreach (Act act in filter)
        {
            stagedActs.Remove(act);
        }
    }
    private static void MergeDict<TKey, TValue>(Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other, bool overwrite)
    {
        foreach (var pair in other)
        {
            if (overwrite || !dict.ContainsKey(pair.Key))
            {
                dict[pair.Key] = pair.Value;
            }
        }
    }
    private void DeferActs(Act.TickFlags flag)
    {
        // Return if no acts to defer
        if (_deferredActs.Count == 0)
        {
            return;
        }


        // Reference swap to avoid mutation
        var actsToDefer = _deferredActs;
        _deferredActs = new();


        // Defer perform acts
        var filter = new List<Act>();
        foreach (Act act in actsToDefer.Keys)
        {
            if ((actsToDefer[act] & flag) != 0)
            {
                act.Perform();
                filter.Add(act);
            }
        }


        // Filter out
        foreach (Act act in filter)
        {
            actsToDefer.Remove(act);
        }


        // Merge back unperformed
        MergeDict(_deferredActs, actsToDefer, false);
    }


    // Private Override Methods
    private void Update()
    {
        TickActs(ref _stagedTickActs, ref _actsToTick, Act.TickFlags.Tick);
        DeferActs(Act.TickFlags.Tick);
    }
    private void FixedUpdate()
    {
        TickActs(ref _stagedPhysicsTickActs, ref _actsToPhysicsTick, Act.TickFlags.PhysicsTick);
        DeferActs(Act.TickFlags.PhysicsTick);
    }
    private void LateUpdate()
    {
        TickActs(ref _stagedLateTickActs, ref _actsToLateTick, Act.TickFlags.LateTick);
        DeferActs(Act.TickFlags.LateTick);
    }
}
