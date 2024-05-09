using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ShopBuffType
    {
        None,
        Block,
        Attack,
        Speed,
        Sight,
    }
    public enum UIAnimation
    {
        Idle,
        Up,
        Down,
    }
    public enum PPType
    {
        Idle,
        Update,
        Stop
    }
    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }

    public enum CreatureState
    {
        Idle,
        Moving,
        Skill,
        Dead,
        Fire
    }

    public enum State
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Block = 10,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
        StartMenu,
        SingleGame
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
