// Main Contributors: Weiguang, Div

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public class Gravity : IMovement
    {
        #region Data Fields

        private bool isActive = true;
        private int gravityAcceleration;

        #endregion

        #region Properties

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        public int GravityAcceleration
        {
            get { return gravityAcceleration; }
            protected set { gravityAcceleration = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            movable.Velocity = new Velocity(movable.Velocity.FinenessX, movable.Velocity.FinenessY + GravityAcceleration);
        }

        #endregion

        #region Constructors

        public Gravity(int gravityAcceleration)
        {
            GravityAcceleration = gravityAcceleration;
        }

        #endregion
    }

    public class MoveLeftRight : IMovement
    {
        #region Data Fields

        private bool isActive = true;
        private int moveFinenessX;

        #endregion

        #region Properties

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        public int MoveFinenessX
        {
            get { return moveFinenessX; }
            protected set { moveFinenessX = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            Velocity velocity = movable.Velocity;

            IsActive = false;
            // can't do this because we don't have something called friction to resist fixed velocity motion
            // movable.Velocity = new Velocity(velocity.FinenessX + MoveFinenessX, velocity.FinenessY);
            movable.Velocity = new Velocity(MoveFinenessX, velocity.FinenessY);
        }

        #endregion

        #region Constructors

        public MoveLeftRight(int moveFinenessX)
        {
            MoveFinenessX = moveFinenessX;
        }

        #endregion
    }

    public class SlideLeftRight : IMovement
    {
        #region Data Fields

        private int decayTicks = 1;
        private int ticks;
        private bool isActive = true;
        private int moveFinenessX;

        #endregion

        #region Properties

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        public int MoveFinenessX
        {
            get { return moveFinenessX; }
            protected set { moveFinenessX = value; }
        }

        public int DecayTicks
        {
            get { return decayTicks; }
            protected set { decayTicks = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            Velocity velocity = movable.Velocity;
            if (ticks == DecayTicks)
                IsActive = false;

            movable.Velocity = new Velocity(MoveFinenessX, velocity.FinenessY);
            ticks++;
        }

        #endregion

        #region Constructors

        public SlideLeftRight(int moveFinenessX)
        {
            MoveFinenessX = moveFinenessX;
        }

        #endregion
    }

    public class CrateMove : IMovement
    {
        #region Data Fields

        private bool isMovingLeft;

        private int displacementAmount;
        private int displacementLeft;
        private int moveSpeed;

        private bool isActive = true;
        
        #endregion

        #region Properties

        public bool IsMovingLeft
        {
            get { return isMovingLeft; }
            protected set { isMovingLeft = value; }
        }

        public int DisplacementAmount
        {
            get { return displacementAmount; }
            protected set { displacementAmount = value; }
        }

        public int DisplacementLeft
        {
            get { return displacementLeft; }
            protected set { displacementLeft = value; }
        }

        public int MoveSpeed
        {
            get { return moveSpeed; }
            protected set { moveSpeed = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            DisplacementLeft -= Math.Abs(movable.Velocity.FinenessX);

            if (DisplacementLeft <= 0)
            {
                // only reaches here when the pushing is overshot
                // forces the crate to move back a little

                if (IsMovingLeft)
                    movable.Position = movable.Position.AddFineness(Math.Abs(DisplacementLeft), 0);
                else
                    movable.Position = movable.Position.AddFineness(-Math.Abs(DisplacementLeft), 0);

                movable.Velocity = new Velocity(0, movable.Velocity.FinenessY);
                IsActive = false;
                return;
            }

            Velocity velocity = movable.Velocity;

            if (IsMovingLeft)
                velocity = new Velocity(-MoveSpeed, velocity.FinenessY);
            else
                velocity = new Velocity(MoveSpeed, velocity.FinenessY);

            movable.Velocity = velocity;
        }

        #endregion

        #region Constructors

        public CrateMove(bool isMovingLeft, int displacementAmount, int moveSpeed)
        {
            IsMovingLeft = isMovingLeft;
            DisplacementAmount = displacementAmount;
            DisplacementLeft = DisplacementAmount;
            MoveSpeed = moveSpeed;
        }

        #endregion
    }

    public class Jump : IMovement
    {
        #region Data Fields

        private bool isActive = true;
        private int moveFinenessY;

        #endregion

        #region Properties

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        public int MoveFinenessY
        {
            get { return moveFinenessY; }
            protected set { moveFinenessY = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            Velocity velocity = movable.Velocity;

            IsActive = false;
            // can't do this because we don't have something called friction to resist fixed velocity motion
            // movable.Velocity = new Velocity(velocity.FinenessX, velocity.FinenessY + MoveFinenessY);
            movable.Velocity = new Velocity(velocity.FinenessX, MoveFinenessY);
        }

        #endregion

        #region Constructors

        public Jump(int moveFinenessY)
        {
            MoveFinenessY = moveFinenessY;
        }

        #endregion
    }

    public class ReboundOffWall : IMovement
    {
        #region Data Fields

        public const int StopLimit = 20;
        public const int DecayTime = StopLimit / 2;

        public double decayFactor;

        private int counter = 0;
        private bool isActive = true;
        private int velocityFinenessX;

        #endregion

        #region Properties

        public int Counter
        {
            get { return counter; }
            protected set { counter = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            protected set { isActive = value; }
        }

        public int VelocityFinenessX
        {
            get { return velocityFinenessX; }
            protected set { velocityFinenessX = value; }
        }

        public double DecayFactor
        {
            get { return decayFactor; }
            set { decayFactor = value; }
        }

        #endregion

        #region Methods

        public void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>
        {
            Velocity velocity = movable.Velocity;

            if (Counter == StopLimit)
                IsActive = false;
            else
            {
                // slowly decay the velocity away
                if (StopLimit - Counter < DecayTime)
                     velocityFinenessX = (int)(velocityFinenessX * DecayFactor);

                Counter++;
            }

            movable.Velocity = new Velocity(velocity.FinenessX + VelocityFinenessX, velocity.FinenessY);
        }

        #endregion

        #region Constructors

        public ReboundOffWall(int velocityFinenessX, double decayFactor)
        {
            VelocityFinenessX = velocityFinenessX;
            DecayFactor = decayFactor;
        }

        #endregion
    }
}
