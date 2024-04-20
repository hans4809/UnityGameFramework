using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        Unknown,
        Player,
        Enemy,
    }
    public enum State
    {
        Idle,
        Walk,
        Run
    }
    public enum UIEvent
    {
        Click,
        Slider,
        BeginDrag,
        Drag,
        DragEnd,
        PointerDown,
        PointerUP
    }
    public enum MouseEvent
    {
        Press,
        Click
    }

    public enum Scene
    {
        Unknown,
        MainScene
    }
    public enum Sound
    {
        Master,
        BGM,
        SFX,
        MaxCount
    }
}