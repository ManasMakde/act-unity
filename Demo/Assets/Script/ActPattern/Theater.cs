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
        // Copy over all acts
        _performActs.UnionWith(_allActs);


        // Perform all acts
        while (_performActs.Count != 0)
        {
            var act = GetFirst(_performActs);
            _performActs.Remove(act);
            act.Perform();
        }


        // Clear acts
        _performActs.Clear();
    }
    public void RetryAll()
    {
        // Copy over all acts
        _retryActs.UnionWith(_allActs);


        // Retry all acts
        while (_retryActs.Count != 0)
        {
            var act = GetFirst(_retryActs);
            _retryActs.Remove(act);
            act.Retry();
        }


        // Clear acts
        _retryActs.Clear();
    }
    public void AbortAll()
    {
        // Copy over all acts
        _abortActs.UnionWith(_allActs);


        // Abort all acts
        while (_abortActs.Count != 0)
        {
            var act = GetFirst(_abortActs);
            _abortActs.Remove(act);
            act.Abort();
        }


        // Clear acts
        _abortActs.Clear();
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
    public bool HasAct(Act act)
    {
        return _allActs.Contains(act);
    }
    public bool IsEnabled()
    {
        return _isEnabled;
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

    private HashSet<Act> _performActs = new();
    private HashSet<Act> _retryActs = new();
    private HashSet<Act> _abortActs = new();

    private Dictionary<Act, Act.TickFlags> _performDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _retryDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _abortDeferActs = new();
    private HashSet<Act> _tickActs = new();

    private Dictionary<Act, Act.TickFlags> _performingDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _retryingDeferActs = new();
    private Dictionary<Act, Act.TickFlags> _abortingDeferActs = new();
    private HashSet<Act> _tickedActs = new();

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
        _performingDeferActs.Remove(act);
    }
    private void RetryDeferred(Act act, Act.TickFlags flag)
    {
        // Return if invalid act or Invalid flag or Reassigning
        if (act == null || flag == Act.TickFlags.None || (_retryDeferActs.ContainsKey(act) && _retryDeferActs[act] == flag))
        {
            return;
        }

        _retryDeferActs[act] = flag;
        _retryingDeferActs.Remove(act);
    }
    private void AbortDeferred(Act act, Act.TickFlags flag)
    {
        // Return if invalid act or Invalid flag or Reassigning
        if (act == null || flag == Act.TickFlags.None || (_abortDeferActs.ContainsKey(act) && _abortDeferActs[act] == flag))
        {
            return;
        }

        _abortDeferActs[act] = flag;
        _abortingDeferActs.Remove(act);
    }
    private void CancelPerformDeferred(Act act)
    {
        _performDeferActs.Remove(act);
        _performingDeferActs.Remove(act);
    }
    private void CancelRetryDeferred(Act act)
    {
        _retryDeferActs.Remove(act);
        _retryingDeferActs.Remove(act);
    }
    private void CancelAbortDeferred(Act act)
    {
        _abortDeferActs.Remove(act);
        _abortingDeferActs.Remove(act);
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
        _tickedActs.Remove(act);
    }


    // Private Static Methods
    private static Act GetFirstKey(Dictionary<Act, Act.TickFlags> collection)
    {
        foreach (var kvp in collection)
        {
            return kvp.Key;
        }

        return null;
    }
    private static Act GetFirst(HashSet<Act> collection)
    {
        foreach (var data in collection)
        {
            return data;
        }

        return null;
    }
    private static void TickActs(ref HashSet<Act> acts, ref HashSet<Act> tickedActs, Act.TickFlags flag)
    {
        // Return if no acts to tick
        if (acts.Count == 0)
        {
            return;
        }


        // Iterate and invoke operations
        while (acts.Count != 0)
        {
            // Get act
            var act = GetFirst(acts);
            acts.Remove(act);
            tickedActs.Add(act);  // Shift to completed


            // Continue if cannot tick
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


        // Reference swap
        var temp = acts;
        acts = tickedActs;
        tickedActs = temp;
    }
    private static void InvokeOnActs(ref Dictionary<Act, Act.TickFlags> acts, ref Dictionary<Act, Act.TickFlags> invokingActs, Operation operation, Act.TickFlags flag)
    {
        // Return if no acts to operate
        if (acts.Count == 0)
        {
            return;
        }


        // Reference swap so no act can be readded
        var temp = acts;
        acts = invokingActs;
        invokingActs = temp;


        // Iterate and invoke operations
        while (invokingActs.Count != 0)
        {
            // Get act
            var act = GetFirstKey(invokingActs);
            var actFlag = invokingActs[act];
            invokingActs.Remove(act);


            // Continue if not to invoke
            if (!((actFlag & flag) != 0))
            {
                acts[act] = actFlag;
                continue;
            }


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
        TickActs(ref _tickActs, ref _tickedActs, Act.TickFlags.Tick);
        InvokeOnActs(ref _performDeferActs, ref _performingDeferActs, Operation.Perform, Act.TickFlags.Tick);
        InvokeOnActs(ref _retryDeferActs, ref _retryingDeferActs, Operation.Retry, Act.TickFlags.Tick);
        InvokeOnActs(ref _abortDeferActs, ref _abortingDeferActs, Operation.Abort, Act.TickFlags.Tick);
    }
    private void FixedUpdate()
    {
        TickActs(ref _tickActs, ref _tickedActs, Act.TickFlags.PhysicsTick);
        InvokeOnActs(ref _performDeferActs, ref _performingDeferActs, Operation.Perform, Act.TickFlags.PhysicsTick);
        InvokeOnActs(ref _retryDeferActs, ref _retryingDeferActs, Operation.Retry, Act.TickFlags.PhysicsTick);
        InvokeOnActs(ref _abortDeferActs, ref _abortingDeferActs, Operation.Abort, Act.TickFlags.PhysicsTick);
    }
    private void LateUpdate()
    {
        TickActs(ref _tickActs, ref _tickedActs, Act.TickFlags.LateTick);
        InvokeOnActs(ref _performDeferActs, ref _performingDeferActs, Operation.Perform, Act.TickFlags.LateTick);
        InvokeOnActs(ref _retryDeferActs, ref _retryingDeferActs, Operation.Retry, Act.TickFlags.LateTick);
        InvokeOnActs(ref _abortDeferActs, ref _abortingDeferActs, Operation.Abort, Act.TickFlags.LateTick);
    }
    private void OnDestroy()
    {
        AbortAll();
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
    }
}
