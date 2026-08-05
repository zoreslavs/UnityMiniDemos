using System;

namespace UnityMiniDemos.Features.MagicWords
{
    public static class AvatarPosition
    {
        private const string Left = "left";
        private const string Right = "right";

        public static bool IsKnown(string position)
        {
            return IsLeft(position) || IsRight(position);
        }

        public static bool IsLeft(string position)
        {
            return string.Equals(position, Left, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRight(string position)
        {
            return string.Equals(position, Right, StringComparison.OrdinalIgnoreCase);
        }
    }
}