using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Dictionary<KeyCode, Action> KeyActions = null;
    public Action<Define.MouseEvent> MouseAction = null;
    bool _pressed = false;
    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        foreach (var keyAction in KeyActions)
        {
            if (Input.GetKeyDown(keyAction.Key))
            {
                keyAction.Value?.Invoke();
            }
        }

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.Click);
                }
                _pressed = false;
            }
        }
    }
    public void Clear()
    {
        MouseAction = null;
        if(KeyActions != null)
            KeyActions.Clear();
    }
}
