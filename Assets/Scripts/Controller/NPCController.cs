using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : NonPlayableController
{
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        WorldObjectType = Define.WorldObject.NPC;
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

