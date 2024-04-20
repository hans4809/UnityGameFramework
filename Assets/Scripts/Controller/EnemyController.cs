using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using UnityEngine;
using static EnemyController;

public class EnemyController : BaseController
{
 
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Enemy;
    }
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    protected override void UpdateRun()
    {
        base.UpdateRun();
    }
    protected override void UpdateWalk()
    {
        base.UpdateWalk();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
