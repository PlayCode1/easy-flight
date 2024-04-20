using System;
using Godot;
using static App.Audio.Audio.BusEnum;

namespace App.Audio;

public partial class Audio : Node
{
    public enum BusEnum : int
    {
        Master,
        Music,
        Sound,
        Ambient
    }
}