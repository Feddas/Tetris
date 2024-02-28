using System;
using UnityEngine;

public static class Utility
{
    /// <summary> Applies the same function to every component of a vector. </summary>
    public static Vector3Int Scale(this Vector3 input, Func<float, int> scalingFunc)
    {
        return new Vector3Int(scalingFunc(input.x), scalingFunc(input.y), scalingFunc(input.z));
    }

    /// <summary>
    /// https://stackoverflow.com/questions/14415753/wrap-value-into-range-min-max-without-division
    /// </summary>
    /// <returns> the value of <paramref name="input"/> wrapped into the range of <paramref name="x_min"/> and <paramref name="x_max"/>. </returns>
    public static int Wrap(int input, int x_min, int x_max)
    {
        if (input < x_min)
            return x_max - (x_min - input) % (x_max - x_min);
        else
            return x_min + (input - x_min) % (x_max - x_min);
    }
}
