﻿namespace ChatStream;

public static class Utils
{
    public static ConsoleColor GetRandomColor()
    {
        var random = new Random();
        var color = (ConsoleColor)random.Next(0, 15);
        if (color == ConsoleColor.Green)
        {
            GetRandomColor();
        }
        return color;
    }}