using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success,
    }

    private State _currentState = State.Running;
    public State CurrentState { get => _currentState; set => _currentState = value; }

    private bool _bIsStarted = false;
    public bool bIsStarted { get => _bIsStarted; set => _bIsStarted = value; }

    private string _guid;
    public string GUID { get => _guid; set => _guid = value; }

    public Vector2 _position;
    public Vector2 Position { get => _position; set => _position = value; }

    public State Update()
    {
        if (!bIsStarted)
        {
            OnStart();
            bIsStarted = true;
        }

        CurrentState = OnUpdate();

        if (CurrentState == State.Failure || CurrentState == State.Success)
        {
            OnStop();
            bIsStarted = false;
        }

        return CurrentState;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();

}
