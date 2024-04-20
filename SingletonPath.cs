using Godot;
using System;
using System.Collections.Generic;

namespace App;

public static class SingletonPath
{
    public const string World = "/root/World";
    public const string Player = "/root/Player";
    public const string MessageBus = "/root/MessageBus";
    public const string Audio = "/root/Audio/Audio";
    public const string GameSave = "/root/GameSave";

    public static IList<T> Shuffle<T>(IList<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }
}