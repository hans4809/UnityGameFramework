using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    GameObject _player;
    public GameObject GetPlayer() { return _player; }
    HashSet<GameObject> _monsters = new HashSet<GameObject>();
    HashSet<GameObject> _items = new HashSet<GameObject>();
    public Action<int> OnSpawnEvent;
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Enemy:
                _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
        }

        return go;
    }
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
        {
            BaseItem Item = go.GetComponent<BaseItem>();
            if (Item == null)
                return Define.WorldObject.Unknown;
            return Item.WorldObjectType;
        }

        return bc.WorldObjectType;
    }
    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Enemy:
                {
                    if (_monsters.Contains(go))
                    {
                        _monsters.Remove(go);
                        if (OnSpawnEvent != null)
                            OnSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
            case Define.WorldObject.Item:
                {
                    if(_items.Contains(go))
                    {
                        _items.Remove(go);
                    }
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
