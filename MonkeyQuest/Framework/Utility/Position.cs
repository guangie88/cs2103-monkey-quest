// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public struct Position
    {
        #region Data Fields

        // a very important constant that defines how much fineness is there within a square
        // a movable object is able to move between -FINENESS_LIMIT (inc) to FINENESS_LIMIT (exc) in a square
        // before hitting into the next square.
        public readonly int FinenessLimit;

        private int x;
        private int y;
        private int finenessX;
        private int finenessY;

        #endregion

        #region Properties

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int FinenessX
        {
            get { return finenessX; }
            set
            {
                if (!CheckFinenessWithinLimit(value))
                    throw new PositionFinenessExceedLimitException(value, FinenessLimit);

                finenessX = value;
            }
        }

        public int FinenessY
        {
            get { return finenessY; }
            set
            {
                if (!CheckFinenessWithinLimit(value))
                    throw new PositionFinenessExceedLimitException(value, FinenessLimit);

                finenessY = value;
            }
        }

        public int OverallFinenessX
        {
            get { return (X * FinenessLimit * 2) + FinenessX; }
        }

        public int OverallFinenessY
        {
            get { return (Y * FinenessLimit * 2) + FinenessY; }
        }

        #endregion

        #region Methods

        private bool CheckFinenessWithinLimit(int fineness)
        {
            if (fineness < -FinenessLimit || fineness >= FinenessLimit)
                return false;

            return true;
        }

        private static int FinenessShift(int initial, int delta, int basis, int range)
        {
            // this process basically checks and see if the fineness value has crossed the true grid border
            // if it does, it returns the fineness value with respect to next grid
            // if it doesn't, it just return the fineness value after the addition
            int next = initial - basis + delta;

            next %= range;

            // % may still give you negative values, so bring it back to range
            if (next < 0)
                next += range;

            next += basis;

            return next;
        }

        private static int GridShift(int initial, int delta, int basis, int range)
        {
            int bigDelta = 0;
            int next = initial - basis + delta;

            bigDelta = (int)Math.Floor((double)next / (double)range);
            return bigDelta;
        }

        public Position AddFineness(int finenessX, int finenessY)
        {
            // X part
            
            int nextX = X;

            int divisor = FinenessLimit * 2;
            int baseValue = -FinenessLimit;

            int nextFinenessX = FinenessShift(FinenessX, finenessX, baseValue, divisor);
            int exceedX = GridShift(FinenessX, finenessX, baseValue, divisor);

            // if crossed the left square, proceed to the left square (-ve)
            // else if crossed the right square, proceed to the right square (+ve)
            nextX += exceedX;

            // Y part

            int nextY = Y;

            int nextFinenessY = FinenessShift(FinenessY, finenessY, baseValue, divisor);
            int exceedY = GridShift(FinenessY, finenessY, baseValue, divisor);

            // if crossed the top square, proceed to the top square (-ve)
            // else if crossed the bottom square, proceed to the bottom square (+ve)
            nextY += exceedY;

            return new Position(nextX, nextY, nextFinenessX, nextFinenessY, FinenessLimit);
        }

        #endregion

        #region Constructors

        public Position(int x, int y, int finenessX = 0, int finenessY = 0, int finenessLimit = Values.FinenessLimit)
        {
            // assigns fineness limit from the board
            // once set, cannot be undone
            // default is 15 => fineness can accept integer values between [-15 to 14]
            FinenessLimit = finenessLimit;

            this.x = x;
            this.y = y;

            this.finenessX = finenessX;
            this.finenessY = finenessY;

            // have to assign everything before I can do check
            // limitation of struct
            if (!CheckFinenessWithinLimit(finenessX))
                throw new PositionFinenessExceedLimitException(finenessX, FinenessLimit);

            if (!CheckFinenessWithinLimit(finenessY))
                throw new PositionFinenessExceedLimitException(finenessX, FinenessLimit);
        }

        #endregion
    }
}