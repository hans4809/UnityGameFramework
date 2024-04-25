using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayableController : BaseController
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.NonPlayer;
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
