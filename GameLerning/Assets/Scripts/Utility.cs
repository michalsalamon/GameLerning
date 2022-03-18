using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] f_array, int f_seed)
    {
        System.Random prng = new System.Random(f_seed);

        for (int i = 0; i < f_array.Length - 1; i++)
        {
            int f_randomIndex = prng.Next(i, f_array.Length);
            T f_tempItem = f_array[f_randomIndex];
            f_array[f_randomIndex] = f_array[i];
            f_array[i] = f_tempItem;
        }
        return f_array;

    }
}
