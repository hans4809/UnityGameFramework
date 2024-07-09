using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    [SerializeField] float _duration = 1f;
    public float Duration { get => _duration; set => _duration = value;}

    float _startTime;
    public float StartTime { get => _startTime; private set => _startTime = value; }
    protected override void OnStart()
    {
        StartTime = Time.time;
    }

    protected override void OnStop()
    {
        throw new System.NotImplementedException();
    }

    protected override State OnUpdate()
    {
        if (Time.time - StartTime > Duration)
            return State.Success;

        return State.Running;
    }
}
