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
        foreach (Act act in _allActs)
        {
            act.Abort();
        }
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
    public HashSet<Act> _allActs = new();
    public HashSet<Act> _ongoingActs = new();
    public Dictionary<Act, Act.TickFlags> _deferredActs = new();
    public Dictionary<Act, bool> _stagedTickActs = new();
    public Dictionary<Act, bool> _stagedPhysicsTickActs = new();
    public Dictionary<Act, bool> _stagedLateTickActs = new();
    public Dictionary<Act, bool> _actsToTick = new();
    public Dictionary<Act, bool> _actsToPhysicsTick = new();
    public Dictionary<Act, bool> _actsToLateTick = new();
    public bool _isEnabled = true;


    // Private Staging Methods
    public void StageDeferred(Act act, Act.TickFlags flag)
    {
        if (act == null)
        {
            return;
        }

        _deferredActs[act] = flag;
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
            Debug.LogWarning("Theater UnstagePhysicsTick received null act");
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


    // Private Tick Methods
    private void TickActs()
    {
        // Return if no act to tick
        if (_stagedTickActs.Count == 0)
        {
            return;
        }


        // Reference swap to avoid mutation
        _actsToTick = _stagedTickActs;
        _stagedTickActs = new();


        // Tick all acts
        foreach (Act act in _actsToTick.Keys)
        {
            act.TickImpl();
        }


        // Merge back
        MergeDict(_stagedTickActs, _actsToTick, false);


        // Clear
        _actsToTick.Clear();


        // Filter
        var filter = new List<Act>();
        foreach (Act act in _stagedTickActs.Keys)
        {
            if (!_stagedTickActs[act])
            {
                filter.Add(act);
            }
        }

        foreach (Act act in filter)
        {
            _stagedTickActs.Remove(act);
        }
    }
    private void PhysicsTickActs()
    {
        // Return if no act to physics tick
        if (_stagedPhysicsTickActs.Count == 0)
        {
            return;
        }


        // Reference swap to avoid mutation
        _actsToPhysicsTick = _stagedPhysicsTickActs;
        _stagedPhysicsTickActs = new Dictionary<Act, bool>();


        // Physics tick all acts
        foreach (Act act in _actsToPhysicsTick.Keys)
        {
            act.PhysicsTickImpl();
        }


        // Merge back
        MergeDict(_stagedPhysicsTickActs, _actsToPhysicsTick, false);


        // Clear
        _actsToPhysicsTick.Clear();


        // Filter
        var filter = new List<Act>();
        foreach (Act act in _stagedPhysicsTickActs.Keys)
        {
            if (!_stagedPhysicsTickActs[act])
            {
                filter.Add(act);
            }
        }

        foreach (Act act in filter)
        {
            _stagedPhysicsTickActs.Remove(act);
        }
    }
    private void LateTickActs()
    {
        // Return if no act to late tick
        if (_stagedLateTickActs.Count == 0)
        {
            return;
        }


        // Reference swap to avoid mutation
        _actsToLateTick = _stagedLateTickActs;
        _stagedLateTickActs = new Dictionary<Act, bool>();


        // Late tick all acts
        foreach (Act act in _actsToLateTick.Keys)
        {
            act.LateTickImpl();
        }


        // Merge back
        MergeDict(_stagedLateTickActs, _actsToLateTick, false);


        // Clear
        _actsToLateTick.Clear();


        // Filter
        var filter = new List<Act>();
        foreach (Act act in _stagedLateTickActs.Keys)
        {
            if (!_stagedLateTickActs[act])
            {
                filter.Add(act);
            }
        }

        foreach (Act act in filter)
        {
            _stagedLateTickActs.Remove(act);
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


    // Private Override Methods
    private void Update()
    {
        TickActs();
        DeferActs(Act.TickFlags.Tick);
    }
    private void FixedUpdate()
    {
        PhysicsTickActs();
        DeferActs(Act.TickFlags.PhysicsTick);
    }
    private void LateUpdate()
    {
        LateTickActs();
        DeferActs(Act.TickFlags.LateTick);
    }
}
