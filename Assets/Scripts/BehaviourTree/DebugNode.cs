using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNode : ActionNode
{
    [SerializeField] string _message;
    public string Message { get => _message; set => _message = value; }
    protected override void OnStart()
    {
        Debug.Log($"OnStart{Message}");
    }

    protected override void OnStop()
    {
        Debug.Log($"OnStop{Message}");
    }
    

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate{Message}");
        return State.Success;
    }
}
