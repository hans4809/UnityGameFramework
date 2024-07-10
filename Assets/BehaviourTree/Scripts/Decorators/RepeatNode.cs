using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecoratorNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }


    // 특정 조건에 Loop 도는 거는 커스터마이징해라
    protected override State OnUpdate()
    {
        ChildNode.Update();
        return State.Running;
    }

}
