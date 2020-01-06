// Main Contributors: Weiguang, Div

using System;
using System.Collections.Generic;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Movable<TBoard> : Collidable<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private List<IMovement> movementList = new List<IMovement>();
        private Velocity velocity;
        private Acceleration acceleration;

        #endregion

        #region Properties

        public IList<IMovement> MovementList
        {
            get { return movementList; }
            protected set
            {
                if (value is List<IMovement>)
                    movementList = value as List<IMovement>;
            }
        }

        public Velocity Velocity
        {
            get { return velocity; }
            internal protected set { velocity = value; }
        }

        public virtual int MaxVelocityFinenessX
        {
            get { return int.MaxValue; }
        }

        public virtual int MinVelocityFinenessX
        {
            get { return int.MinValue; }
        }

        public virtual int MaxVelocityFinenessY
        {
            get { return int.MaxValue; }
        }

        public virtual int MinVelocityFinenessY
        {
            get { return int.MinValue; }
        }

        public Acceleration Acceleration
        {
            get { return acceleration; }
            internal protected set { acceleration = value; }
        }

        #endregion

        #region Events

        public virtual event CollidingEventHandler<TBoard> Colliding;
        public event AfterMotionEventHandler<TBoard> AfterMotion;

        #endregion

        #region Methods

        private void UpdateVelocity()
        {
            int velocityNewFinenessX = Velocity.FinenessX + Acceleration.FinenessX;
            int velocityNewFinenessY = Velocity.FinenessY + Acceleration.FinenessY;

            Velocity = new Velocity(velocityNewFinenessX, velocityNewFinenessY);
        }

        private void AccumulateMotion()
        {
            // to hasten the process when there is no movement
            if (MovementList.Count == 0)
                return;

            // removes movements that are already non-active
            for (int i = 0; i < MovementList.Count; )
            {
                if (!MovementList[i].IsActive)
                    MovementList.RemoveAt(i);
                else
                    i++;
            }

            foreach (IMovement movement in MovementList)
                movement.ChangeMotion(this);
        }

        private void LimitVelocity()
        {
            // limit the velocity for X according to the max min limits
            if (Velocity.FinenessX < MinVelocityFinenessX)
                velocity.FinenessX = MinVelocityFinenessX;
            else if (Velocity.FinenessX > MaxVelocityFinenessX)
                velocity.FinenessX = MaxVelocityFinenessX;

            // limit the velocity for Y according to the max min limits
            if (Velocity.FinenessY < MinVelocityFinenessY)
                velocity.FinenessY = MinVelocityFinenessY;
            else if (Velocity.FinenessY > MaxVelocityFinenessY)
                velocity.FinenessY = MaxVelocityFinenessY;
        }

        private bool TraverseMotion()
        {
            // defensive programming
            // defend from async nature of event handling
            if (Board == null)
                return true;

            Position nextPosition = Position.AddFineness(Velocity.FinenessX, Velocity.FinenessY);

            int velocityFinenessX = Velocity.FinenessX;
            int velocityFinenessY = Velocity.FinenessY;
            int collisionDistance;

            // collidables are first sorted according to their proximity to current position
            // this will make the checking of collision done first on collidable
            // that is nearest to the current position
            foreach (Collidable<TBoard> otherCollidable in FindCollidablesWithinRegion().SortProximityReturn(Position))
            {
                CollisionStatus collisionStatus = PerformCollidingInto(Position, nextPosition, this.RectangularMask, otherCollidable, out collisionDistance);

                // correct the velocity before trying everything again
                if (collisionStatus == CollisionStatus.NoCollision)
                    continue;

                if (collisionStatus == CollisionStatus.CollisionFromLeft)
                    velocityFinenessX = collisionDistance;
                else if (collisionStatus == CollisionStatus.CollisionFromRight)
                    velocityFinenessX = -collisionDistance;
                else if (collisionStatus == CollisionStatus.CollisionFromTop)
                    velocityFinenessY = collisionDistance;
                else // collision from bottom
                    velocityFinenessY = -collisionDistance;

                Velocity = new Velocity(velocityFinenessX, velocityFinenessY);

                // false to indicate the motion hasn't been completed
                // will be re-invoked in PerformMotionBind
                return false;
            }

            // attempt to set the position
            bool positionCanBeSet = SetPosition(nextPosition);
            OnAfterMotion(new MovableEventArgs<TBoard>(this));

            return true;
        }

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            // adds itself for the BoardTick event of the new Board
            Board.BoardTick += PerformMotionBind;
        }

        protected override void RemoveBindings()
        {
            // removes itself from the BoardTick event of the current Board
            Board.BoardTick -= PerformMotionBind;

            base.RemoveBindings();
        }

        protected virtual void OnAfterMotion(MovableEventArgs<TBoard> e)
        {
            if (AfterMotion != null)
                AfterMotion(this, e);
        }

        protected internal virtual void PerformMotionBind(object sender, EventArgs e)
        {
            // defensive programming
            // defend from async nature of event handling
            if (Board == null)
                return;

            // first update the velocity according to its acceleration
            UpdateVelocity();

            // then let the Movable object calculates its next motion by allowing
            // its acceleration and velocity values to be tweaked by all the IMovement objects
            AccumulateMotion();

            // limit the velocity of the movable object
            // in accordance to the limits set
            LimitVelocity();

            // traverse towards its motion path at this instant
            // if it's colliding with something at the next position
            // it will attempt to trim down the velocity
            // so that the object will just stop beside the other Collision object
            while (!TraverseMotion()) ;
        }

        public void AddMovement(IMovement movement)
        {
            if (movement == null)
                return;

            MovementList.Add(movement);
        }

        public bool RemoveMovement(IMovement movement)
        {
            if (movement == null)
                return false;

            return MovementList.Remove(movement);
        }

        #endregion

        #region Constructors

        public Movable(RectangularMask rectangularMask = default(RectangularMask), IEnumerable<IMovement> movements = null, Position position = default(Position))
            : base(rectangularMask, position)
        {
            if (movements == null)
                MovementList = new List<IMovement>();
            else
                MovementList = new List<IMovement>(movements);
        }
        
        #endregion
    }

    internal class CollidableProximity<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Comparer

        internal class CollidableProximityComparer : IComparer<CollidableProximity<TBoard>>
        {
            public int Compare(CollidableProximity<TBoard> x, CollidableProximity<TBoard> y)
            {
                return x.SquaredProximity - y.SquaredProximity;
            }
        }

        #endregion

        #region Data Fields

        private Collidable<TBoard> collidable;
        private int squaredProximity;

        #endregion

        #region Properties

        public Collidable<TBoard> Collidable
        {
            get { return collidable; }
            internal protected set { collidable = value; }
        }

        public int SquaredProximity
        {
            get { return squaredProximity; }
            internal protected set { squaredProximity = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public CollidableProximity(Collidable<TBoard> collidable, int squaredProximity)
        {
            Collidable = collidable;
            SquaredProximity = squaredProximity;
        }

        #endregion
    }

    internal static class CollidableProximitySorter
    {
        public static IEnumerable<Collidable<TBoard>> RemoveProximity<TBoard>(this IEnumerable<CollidableProximity<TBoard>> collidableProximityEnumerable)
            where TBoard : Board<TBoard>
        {
            List<Collidable<TBoard>> collidableList = new List<Collidable<TBoard>>();

            foreach (CollidableProximity<TBoard> collidableProximity in collidableProximityEnumerable)
                collidableList.Add(collidableProximity.Collidable);

            return collidableList;
        }

        public static IEnumerable<Collidable<TBoard>> SortProximityReturn<TBoard>(this IEnumerable<Collidable<TBoard>> collidables, Position selfPosition)
            where TBoard : Board<TBoard>
        {
            // this method performs an out-of-place sort to collidables
            // with first collidable being nearest to selfPosition

            List<int> squaredProximityList = new List<int>();
            List<CollidableProximity<TBoard>> collidableProximityList = new List<CollidableProximity<TBoard>>();

            int selfX = selfPosition.OverallFinenessX;
            int selfY = selfPosition.OverallFinenessY;

            foreach (Collidable<TBoard> collidable in collidables)
            {
                // calculates all the proximity between other collidables and its own position
                int otherX = collidable.Position.OverallFinenessX;
                int otherY = collidable.Position.OverallFinenessY;

                // by Pythagoras' Theorem
                int squaredProximity = (selfX - otherX) * (selfX - otherX) + (selfY - otherY) * (selfY - otherY);

                collidableProximityList.Add(new CollidableProximity<TBoard>(collidable, squaredProximity));
            }

            collidableProximityList.Sort(new CollidableProximity<TBoard>.CollidableProximityComparer());
            return collidableProximityList.RemoveProximity();
        }
    }

    public class MovableEventArgs<TBoard> : EventArgs
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Movable<TBoard> movable;

        #endregion

        #region Properties

        public Movable<TBoard> Movable
        {
            get { return movable; }
            protected set { movable = value; }
        }

        #endregion

        #region Constructors

        public MovableEventArgs(Movable<TBoard> movable)
        {
            Movable = movable;
        }

        #endregion
    }
}
