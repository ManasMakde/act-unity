// v0.4.0-alpha
// 
// MIT License
//
// Copyright (c) 2025-present Manas Ravindra Makde
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System;
using System.Collections.Generic;
using UnityEngine;


public class Theater : MonoBehaviour
{
    // Public Signals
    public event Action<Theater, bool> OnEnableChanged;
    public event Action<Theater, Act> OnActStatusChange;


    // Public Methods
    public void PerformAll()
    {
        // Copy all acts
        CopyHashSetToList(_allActs, _performActs);


        // Perform all acts
        while (_performActs.Count != 0)
        {
            int lastIndex = _performActs.Count - 1;
            var act = _performActs[lastIndex];
            _performActs.RemoveAt(lastIndex);
            act.Perform();
        }


        // Clear acts
        _performActs.Clear();
    }
    public void RetryAll()
    {
        // Copy all acts
        CopyHashSetToList(_allActs, _retryActs);


        // Retry all acts
        while (_retryActs.Count != 0)
        {
            int lastIndex = _retryActs.Count - 1;
            var act = _retryActs[lastIndex];
            _retryActs.RemoveAt(lastIndex);
            act.Retry();
        }


        // Clear acts
        _retryActs.Clear();
    }
    public void AbortAll()
    {
        // Copy all acts
        CopyHashSetToList(_allActs, _abortActs);


        // Abort all acts
        while (_abortActs.Count != 0)
        {
            int lastIndex = _abortActs.Count - 1;
            var act = _abortActs[lastIndex];
            _abortActs.RemoveAt(lastIndex);
            act.Abort();
        }


        // Clear acts
        _abortActs.Clear();
    }
    public void SetEnabled(bool newEnabled)
    {
        // Return if reassigning same value
        if (newEnabled == _isEnabled)
        {
            return;
        }


        // Assign new value
        var oldIsEnabled = IsEnabled();
        _isEnabled = newEnabled;


        // Return if final value not changed
        var newIsEnabled = IsEnabled();
        if (oldIsEnabled == newIsEnabled)
        {
            return;
        }


        // Abort all if disabled
        if (!newIsEnabled)
        {
            AbortAll();
            CancelAllPerformDeferred();
            CancelAllRetryDeferred();
            CancelAllAbortDeferred();
        }


        // Broadcast
        OnEnableChanged?.Invoke(this, newIsEnabled);
    }
    public bool HasAct(Act act)
    {
        return _allActs.Contains(act);
    }
    public bool IsEnabled()
    {
        return _isEnabled && enabled && gameObject.activeInHierarchy;
    }
    public bool AreAnyPerforming()
    {
        return _performingActs.Count != 0;
    }
    public IReadOnlyCollection<Act> GetAllPerformingActs()
    {
        return _performingActs;
    }
    public IReadOnlyCollection<Act> GetAllActs()
    {
        return _allActs;
    }


    // Private Enums
    private enum Operation
    {
        Perform,
        Retry,
        Abort
    }


    // Private Properties
    private HashSet<Act> _allActs = new();
    private HashSet<Act> _performingActs = new();

    private List<Act> _performActs = new();
    private List<Act> _retryActs = new();
    private List<Act> _abortActs = new();

    private Dictionary<Act, Act.TickFlags> _performDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _retryDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _abortDeferActs = new();
    private HashSet<Act> _tickActs = new();

    private List<Act> _performingDeferActs = new();
    private List<Act> _retryingDeferActs = new();
    private List<Act> _abortingDeferActs = new();
    private List<Act> _tickingActs = new();

    private bool _isEnabled = true;


    // Private Staging Methods
    private void AddAct(Act newAct)
    {
        // Return if null act
        if (newAct == null)
        {
            return;
        }

        _allActs.Add(newAct);
    }
    private void RemoveAct(Act oldAct)
    {
        // Return if null act
        if (oldAct == null)
        {
            return;
        }


        // Unstage everything
        UnstageTick(oldAct);
        CancelPerformDeferred(oldAct);
        CancelRetryDeferred(oldAct);
        CancelAbortDeferred(oldAct);
        _performingActs.Remove(oldAct);
        _performActs.Remove(oldAct);
        _retryActs.Remove(oldAct);
        _abortActs.Remove(oldAct);
        _allActs.Remove(oldAct);
    }
    private void ActStatusChange(Act act)
    {
        if (act.IsPerforming())
        {
            _performingActs.Add(act);
        }
        else
        {
            _performingActs.Remove(act);
        }

        OnActStatusChange?.Invoke(this, act);
    }
    private void PerformDeferred(Act act, Act.TickFlags flag)
    {
        // Return if invalid act or Invalid flag or Reassigning
        if (act == null || flag == Act.TickFlags.None || (_performDeferActs.ContainsKey(act) && _performDeferActs[act] == flag))
        {
            return;
        }

        _performDeferActs[act] = flag;
    }
    private void RetryDeferred(Act act, Act.TickFlags flag)
    {
        // Return if invalid act or Invalid flag or Reassigning
        if (act == null || flag == Act.TickFlags.None || (_retryDeferActs.ContainsKey(act) && _retryDeferActs[act] == flag))
        {
            return;
        }

        _retryDeferActs[act] = flag;
    }
    private void AbortDeferred(Act act, Act.TickFlags flag)
    {
        // Return if invalid act or Invalid flag or Reassigning
        if (act == null || flag == Act.TickFlags.None || (_abortDeferActs.ContainsKey(act) && _abortDeferActs[act] == flag))
        {
            return;
        }

        _abortDeferActs[act] = flag;
    }
    private void CancelPerformDeferred(Act act)
    {
        if (act == null)
        {
            return;
        }

        _performDeferActs.Remove(act);
    }
    private void CancelRetryDeferred(Act act)
    {
        if (act == null)
        {
            return;
        }

        _retryDeferActs.Remove(act);
    }
    private void CancelAbortDeferred(Act act)
    {
        if (act == null)
        {
            return;
        }

        _abortDeferActs.Remove(act);
    }
    private void CancelAllPerformDeferred()
    {
        _performDeferActs.Clear();
        _performingDeferActs.Clear();
    }
    private void CancelAllRetryDeferred()
    {
        _retryDeferActs.Clear();
        _retryingDeferActs.Clear();
    }
    private void CancelAllAbortDeferred()
    {
        _abortDeferActs.Clear();
        _abortingDeferActs.Clear();
    }
    private void StageTick(Act act)
    {
        if (act == null || act.GetStatus() != Act.Status.Ticking)
        {
            return;
        }

        _tickActs.Add(act);
    }
    private void UnstageTick(Act act)
    {
        if (act == null)
        {
            return;
        }

        _tickActs.Remove(act);
    }


    // Private Static Methods
    private static void WriteLog(string message)
    {
        Debug.LogWarning(message);
    }
    private static void CopyHashSetToList(HashSet<Act> from, List<Act> to)
    {
        to.Clear();

        foreach (var act in from)
        {
            to.Add(act);
        }
    }
    private static void CopyDictToList(Dictionary<Act, Act.TickFlags> from, List<Act> to)
    {
        to.Clear();

        foreach (var kvp in from)
        {
            to.Add(kvp.Key);
        }
    }
    private static void TickStagedActs(HashSet<Act> acts, List<Act> tickingActs, Act.TickFlags flag)
    {
        // Copy all acts
        CopyHashSetToList(acts, tickingActs);


        // Return if no acts to tick
        if (tickingActs.Count == 0)
        {
            return;
        }


        // Iterate and invoke operations
        while (tickingActs.Count != 0)
        {
            // Get act
            int lastIndex = tickingActs.Count - 1;
            var act = tickingActs[lastIndex];
            tickingActs.RemoveAt(lastIndex);


            // Skip if no longer staged
            if (!acts.Contains(act))
            {
                continue;
            }


            // Skip if cannot tick
            if (!act.CanTick(flag))
            {
                continue;
            }


            // Invoke tick
            if ((flag & Act.TickFlags.Tick) != 0)
            {
                Act.Friend.TickImpl(act);
            }
            else if ((flag & Act.TickFlags.PhysicsTick) != 0)
            {
                Act.Friend.PhysicsTickImpl(act);
            }
            else if ((flag & Act.TickFlags.LateTick) != 0)
            {
                Act.Friend.LateTickImpl(act);
            }
        }
    }
    private static void InvokeOnActs(Dictionary<Act, Act.TickFlags> acts, List<Act> invokingActs, Operation operation, Act.TickFlags flag)
    {
        // Copy all acts
        CopyDictToList(acts, invokingActs);


        // Return if no acts to operate
        if (invokingActs.Count == 0)
        {
            return;
        }


        // Iterate and invoke operations
        while (invokingActs.Count != 0)
        {
            // Get act
            int lastIndex = invokingActs.Count - 1;
            var act = invokingActs[lastIndex];
            invokingActs.RemoveAt(lastIndex);


            // Skip if no longer staged
            if (!acts.ContainsKey(act))
            {
                continue;
            }


            // Skip if not to invoke
            var actFlag = acts[act];
            if (!((actFlag & flag) != 0))
            {
                continue;
            }


            // Remove from acts
            acts.Remove(act);  // Intentionally before invoke to avoid tick type re-defer being lost


            // Invoke operation
            if (operation == Operation.Perform)
            {
                act.Perform();
            }
            else if (operation == Operation.Retry)
            {
                act.Retry();
            }
            else if (operation == Operation.Abort)
            {
                act.Abort();
            }
        }
    }


    // Private Override Methods
    private void Update()
    {
        TickStagedActs(_tickActs, _tickingActs, Act.TickFlags.Tick);
        InvokeOnActs(_performDeferActs, _performingDeferActs, Operation.Perform, Act.TickFlags.Tick);
        InvokeOnActs(_retryDeferActs, _retryingDeferActs, Operation.Retry, Act.TickFlags.Tick);
        InvokeOnActs(_abortDeferActs, _abortingDeferActs, Operation.Abort, Act.TickFlags.Tick);
    }
    private void FixedUpdate()
    {
        TickStagedActs(_tickActs, _tickingActs, Act.TickFlags.PhysicsTick);
        InvokeOnActs(_performDeferActs, _performingDeferActs, Operation.Perform, Act.TickFlags.PhysicsTick);
        InvokeOnActs(_retryDeferActs, _retryingDeferActs, Operation.Retry, Act.TickFlags.PhysicsTick);
        InvokeOnActs(_abortDeferActs, _abortingDeferActs, Operation.Abort, Act.TickFlags.PhysicsTick);
    }
    private void LateUpdate()
    {
        TickStagedActs(_tickActs, _tickingActs, Act.TickFlags.LateTick);
        InvokeOnActs(_performDeferActs, _performingDeferActs, Operation.Perform, Act.TickFlags.LateTick);
        InvokeOnActs(_retryDeferActs, _retryingDeferActs, Operation.Retry, Act.TickFlags.LateTick);
        InvokeOnActs(_abortDeferActs, _abortingDeferActs, Operation.Abort, Act.TickFlags.LateTick);
    }
    private void OnEnable()
    {
        if (_isEnabled)
        {
            OnEnableChanged?.Invoke(this, IsEnabled());
        }
    }
    private void OnDisable()
    {
        AbortAll();
        CancelAllPerformDeferred();
        CancelAllRetryDeferred();
        CancelAllAbortDeferred();
        if (_isEnabled)
        {
            OnEnableChanged?.Invoke(this, false);
        }
    }
    private void OnDestroy()
    {
        AbortAll();
        CancelAllPerformDeferred();
        CancelAllRetryDeferred();
        CancelAllAbortDeferred();
    }


    // Friend Class
    public class Friend
    {
        static public void AddAct(Theater theater, Act newAct)
        {
            theater.AddAct(newAct);
        }
        static public void RemoveAct(Theater theater, Act oldAct)
        {
            theater.RemoveAct(oldAct);
        }
        static public void ActStatusChange(Theater theater, Act act)
        {
            theater.ActStatusChange(act);
        }
        static public void PerformDeferred(Theater theater, Act act, Act.TickFlags flag)
        {
            theater.PerformDeferred(act, flag);
        }
        static public void RetryDeferred(Theater theater, Act act, Act.TickFlags flag)
        {
            theater.RetryDeferred(act, flag);
        }
        static public void AbortDeferred(Theater theater, Act act, Act.TickFlags flag)
        {
            theater.AbortDeferred(act, flag);
        }
        static public void CancelPerformDeferred(Theater theater, Act act)
        {
            theater.CancelPerformDeferred(act);
        }
        static public void CancelRetryDeferred(Theater theater, Act act)
        {
            theater.CancelRetryDeferred(act);
        }
        static public void CancelAbortDeferred(Theater theater, Act act)
        {
            theater.CancelAbortDeferred(act);
        }
        static public void StageTick(Theater theater, Act act)
        {
            theater.StageTick(act);
        }
        static public void UnstageTick(Theater theater, Act act)
        {
            theater.UnstageTick(act);
        }
        static public void WriteLog(Theater theater, string message)
        {
            Theater.WriteLog(message);
        }
    }
}
