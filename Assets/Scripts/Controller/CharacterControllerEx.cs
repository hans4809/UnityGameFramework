using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControllerEx : BaseController
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

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
        switch (State)
        {
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Walk:
                UpdateWalk();
                break;
            case Define.State.Run:
                UpdateRun();
                break;
        }

    }
    private void FixedUpdate()
    {
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
    }
}
