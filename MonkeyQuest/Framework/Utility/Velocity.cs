// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public struct Velocity
    {
        private int finenessX;
        private int finenessY;

        public int FinenessX
        {
            get { return finenessX; }
            internal set { finenessX = value; }
        }

        public int FinenessY
        {
            get { return finenessY; }
            internal set { finenessY = value; }
        }

        public Velocity(int finenessX, int finenessY)
        {
            this.finenessX = finenessX;
            this.finenessY = finenessY;
        }
    }
}
