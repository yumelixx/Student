using System;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public string Name;

    public event Action OnEnter;
    public event Action OnUpdate;
    public event Action OnExit;

    public State(string name)
    {
        Name = name;
    }

    public void Enter()
    {
        OnEnter?.Invoke();
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }
}

public class StateMachine
{
    Dictionary<string, State> states;
    string currentStateName;

    public string CurrentState => currentStateName;

    public StateMachine()
    {
        states = new Dictionary<string, State>();
        currentStateName = string.Empty;
    }

    public void AddState(State state)
    {
        bool success = states.TryAdd(state.Name, state);
        if (!success)
        {
            Debug.LogError($"State '{state.Name}' already exists.");
        }
    }

    public void RemoveState(string stateName)
    {
        states.Remove(stateName);
    }

    public void ChangeState(string newStateName)
    {
        if (currentStateName == newStateName)
        {
            return;
        }

        if (!states.ContainsKey(newStateName))
        {
            return;
        }

        bool success = states.TryGetValue(currentStateName, out State state);
        if (success)
        {
            state.Exit();
        }

        currentStateName = newStateName;
        State newState = states[newStateName];
        newState.Enter();
    }

    public void UpdateState()
    {
        bool success = states.TryGetValue(currentStateName, out State state);
        if (!success)
        {
            return;
        }

        state.Update();
    }

    public void Stop()
    {
        bool success = states.TryGetValue(currentStateName, out State state);
        if (success)
        {
            state.Exit();
        }

        currentStateName = string.Empty;
    }
}
