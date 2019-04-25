using System;

namespace Seron.Extensions
{
    public static class RandomExtensions
    {
        private static Random Randomizer = new Random();

        //First included, Second excluded
        public static int GetRandom(this (int, int) me)
        {
            return Randomizer.Next(me.Item1, me.Item2);
        }
    }
}
