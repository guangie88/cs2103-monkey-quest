// Main Contributors: Weiguang
using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public struct Acceleration
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

        public Acceleration(int finenessX, int finenessY)
        {
            this.finenessX = finenessX;
            this.finenessY = finenessY;
        }
    }
}
