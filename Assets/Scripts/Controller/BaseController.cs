using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]
    protected GameObject _lockTarget;
    [SerializeField]
    protected Define.State _state = Define.State.Idle;
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;
    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }
    public abstract void Init();
    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Idle:
                    anim.SetBool("isWalk", false);
                    anim.SetBool("isRun", false);
                    break;
                case Define.State.Walk:
                    anim.SetBool("isWalk", true);
                    anim.SetBool("isRun", false);
                    break;
                case Define.State.Run:
                    anim.SetBool("isRun", true);
                    break;
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
    }
    private void FixedUpdate()
    {
        
    }
    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateWalk()
    {
    }

    protected virtual void UpdateRun()
    {
    }
}
