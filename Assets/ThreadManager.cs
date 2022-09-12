using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> _ExecuteOnMainThread = new List<Action>();
    private static readonly List<Action> _ExecuteCopiedOnMainThread = new List<Action>();
    private static bool _ActionToExecuteOnMainThread = false;

    private void Update()
    {
        UpdateMain();
    }

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="_action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (_ExecuteOnMainThread)
        {
            _ExecuteOnMainThread.Add(_action);
            _ActionToExecuteOnMainThread = true;
        }
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    public static void UpdateMain()
    {
        if (_ActionToExecuteOnMainThread)
        {
            _ExecuteCopiedOnMainThread.Clear();
            lock (_ExecuteOnMainThread)
            {
                _ExecuteCopiedOnMainThread.AddRange(_ExecuteOnMainThread);
                _ExecuteOnMainThread.Clear();
                _ActionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < _ExecuteCopiedOnMainThread.Count; i++)
            {
                _ExecuteCopiedOnMainThread[i]();
            }
        }
    }
}