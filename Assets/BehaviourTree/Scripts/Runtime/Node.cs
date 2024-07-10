using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private Context _context;
    public Context Context { get => _context; set => _context = value; }

    private BlackBoard _blackBoard;
    public BlackBoard BlackBoard { get => _blackBoard; set => _blackBoard = value; }
    [TextArea] public string description;

    private bool _drawGizmos = false;   
    public bool DrawGizmos { get => _drawGizmos; set => _drawGizmos = value;}

    //private AiAgent _aiAgent;
    //public AiAgent 

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

    public void Abort()
    {
        BehaviourTree.Traverse(this, (node) => {
            node.bIsStarted = false;
            node.CurrentState = State.Running;
            node.OnStop();
        });
    }
    public virtual void OnDrawGizmos() { }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();

}
