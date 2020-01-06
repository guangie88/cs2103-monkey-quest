using System;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public class TypeComparer : IComparer<Type>
    {
        private const int SAME = 0;
        private const int DIFFERENT = -1;

        public int Compare(Type x, Type y)
        {
            if (x.Equals(y))
                return SAME;

            return DIFFERENT;
        }
    }
}
