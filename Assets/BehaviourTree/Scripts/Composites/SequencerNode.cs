using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _currentIndex;
    public int CurrentIndex { get => _currentIndex; private set => _currentIndex = value; }
    protected override void OnStart()
    {
        CurrentIndex = 0;
    }

    protected override void OnStop()
    {
        throw new System.NotImplementedException();
    }

    protected override State OnUpdate()
    {
        var childNode = ChildrenNodes[CurrentIndex];
        switch(childNode.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                CurrentIndex++;
                break;
        }

        return CurrentIndex == ChildrenNodes.Count ? State.Success : State.Running;
    }
}
