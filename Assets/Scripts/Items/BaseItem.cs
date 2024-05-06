using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField]
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Item;

    [SerializeField]
    public Define.Item ItemType { get; protected set; } = Define.Item.Unknown;
    // Start is called before the first frame update
    void Start()
    {
    }

    public abstract void Init();
    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Used();
}
