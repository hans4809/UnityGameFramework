using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        Unknown,
        Player,
        NonPlayer,
        NPC,
        Enemy,
        Item
    }
    public enum State
    {
        Idle,
        Walk,
        Run,
        Damaged,
        Die
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
        MainScene,
    }
    public enum Sound
    {
        Master,
        BGM,
        SFX,
        MaxCount
    }

    public enum Item
    {
        Unknown,
        Light,
        Battery,
        MaxCount
    }
}