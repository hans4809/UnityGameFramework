using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryItem : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Item;
        ItemType = Define.Item.Battery;
    }

    public override void Used()
    {
        Debug.Log("BatteryItem Used");
    }
}
