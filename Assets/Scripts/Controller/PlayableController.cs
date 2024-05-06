using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableController : BaseController
{
    [SerializeField]
    public int Hp { get; protected set; } = 3;
    // Start is called before the first frame update
    void Start()
    {
        
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
    protected override void Damaged()
    {
        base.Damaged();
        Hp--;
        if (Hp <= 0)
        {
            State = Define.State.Die;
        }
    }
    protected override void Died()
    {
        base.Died();
        Managers.Game.Despawn(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
