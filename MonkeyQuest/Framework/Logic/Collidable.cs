// Main Contributors: Weiguang

using System;
using System.Collections.Generic;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Collidable<TBoard> : Positional<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private RectangularMask rectangularMask = new RectangularMask();

        private SortedSet<Type> collidingInclusionSet = new SortedSet<Type>(new TypeComparer());
        private SortedSet<Type> collidingExclusionSet = new SortedSet<Type>(new TypeComparer());
        private CollidingMode collidingMode = CollidingMode.NoException;

        #endregion

        #region Properties

        public RectangularMask RectangularMask
        {
            get { return rectangularMask; }
            protected set { rectangularMask = value; }
        }

        // defines what it should ONLY collide with
        // basically empty unless you want to put in exceptional stuff
        // only contains the type to have the exception, not the object
        public ISet<Type> CollidingInclusionSet
        {
            get { return collidingInclusionSet; }
            protected set { collidingInclusionSet = new SortedSet<Type>(value, new TypeComparer()); }
        }

        // defines what it should NOT collide with
        // basically empty unless you want to put in exceptional stuff
        // only contains the type to have the exception, not the object
        public ISet<Type> CollidingExclusionSet
        {
            get { return collidingExclusionSet; }
            protected set { collidingExclusionSet = new SortedSet<Type>(value, new TypeComparer()); }
        }

        // allows user to switch between colliding mode
        // generally it's on NoException unless special cases such as Door
        // need it to allow the Monkey to collide with it only (inclusion)
        public CollidingMode CollidingMode
        {
            get { return collidingMode; }
            protected set { collidingMode = value; }
        }

        #endregion

        #region Methods

        protected override void InitializeBindings()
        {
            base.InitializeBindings();
            Board.CollisionManager.CollidableList.Add(this);
            SetPosition(Position);
        }

        protected override void RemoveBindings()
        {
            Board.CollisionManager.CollidableList.Remove(this);
            base.RemoveBindings();
        }

        protected CollisionStatus IsCollidingInto(Position nextPosition, RectangularMask mask, Collidable<TBoard> otherCollidable)
        {
            // creates a dummy variable just to store the out value
            int dummyCollisionDistance;
            return IsCollidingInto(new Position(), nextPosition, mask, otherCollidable, out dummyCollisionDistance);
        }

        protected CollisionStatus IsCollidingInto(Position previousPosition, Position nextPosition, RectangularMask mask, Collidable<TBoard> otherCollidable, out int collisionDistance)
        {
            // assume no collision first, and therefore collision distance is 0
            collisionDistance = 0;

            // check whether if the other collidable object is itself
            // if it is, ignore
            if (this == otherCollidable)
                return CollisionStatus.NoCollision;

            // sometimes removal of board is done before this (because of the async nature of event handling)
            // if so, we ignore the collision
            if (Board == null)
                return CollisionStatus.NoCollision;

            // we do checks according to the colliding mode
            // we also check whether the other collidable object's type
            // is found within the inclusion or exclusion set
            if (CollidingMode == Logic.CollidingMode.Exclusion && CollidingExclusionSet.Contains(otherCollidable.GetType()) ||
                otherCollidable.CollidingMode == Logic.CollidingMode.Exclusion && otherCollidable.CollidingExclusionSet.Contains(this.GetType()))
            {
                return CollisionStatus.NoCollision;
            }

            else if (CollidingMode == Logic.CollidingMode.Inclusion||
                     otherCollidable.CollidingMode == Logic.CollidingMode.Inclusion)
            {
                bool tmp = false;

                foreach (Type c in CollidingInclusionSet) {
                    if ((otherCollidable.GetType().IsSubclassOf(c) || this.GetType().IsSubclassOf(c))
                        || (otherCollidable.GetType().Equals(c)) || (this.GetType().Equals(c)))
                    {
                        tmp = true;
                        break;
                    }
                }

                if (!tmp)
                {
                    foreach (Type c in otherCollidable.CollidingInclusionSet)
                    {
                        if (otherCollidable.GetType().IsSubclassOf(c) || this.GetType().IsSubclassOf(c)
                            || (otherCollidable.GetType().Equals(c)) || (this.GetType().Equals(c)))
                        {
                            tmp = true;
                            break;
                        }
                    }
                }

                if (!tmp)
                    return CollisionStatus.NoCollision;
            }

            // IMPORTANT
            // check for mask collision here
            // refer to CollidableMaskCalculation.png for pictorial descriptions

            // note that a mask can at most take up a grid size
            // which is (FinenessLimit * 2) by (FinenessLimit * 2)

            // we will be converting all X and Y values into the absolute fineness values
            // for convenience in comparison

            // TODO:
            // should use subprocedure to calculate all these because most calculation parts
            // are the same

            // check for x-axis first
            RectangularMask otherMask = otherCollidable.RectangularMask;
            int otherMaskFinenessX = otherCollidable.X * Board.FinenessLimit * 2 + otherCollidable.FinenessX;
            int otherMaskLeftFinenessX = otherMaskFinenessX + otherMask.LeftMargin;
            int otherMaskRightFinenessX = otherMaskFinenessX + Board.FinenessLimit * 2 - 1 - otherMask.RightMargin;

            int maskFinenessX = nextPosition.X * Board.FinenessLimit * 2 + nextPosition.FinenessX;
            int maskLeftFinenessX = maskFinenessX + mask.LeftMargin;
            // because the upper limit is exclusive unlike the lower limit, and since the fineness unit is discrete
            // there is a need to -1
            int maskRightFinenessX = maskFinenessX + Board.FinenessLimit * 2 - 1 - mask.RightMargin;

            // check collision now by comparing the mask
            // if x doesn't collide, that means it won't collide anyway, so return false
            if (!((maskLeftFinenessX <= otherMaskLeftFinenessX && otherMaskLeftFinenessX <= maskRightFinenessX) ||
                  (maskLeftFinenessX <= otherMaskRightFinenessX && otherMaskRightFinenessX <= maskRightFinenessX) ||
                  (otherMaskLeftFinenessX <= maskLeftFinenessX && maskLeftFinenessX <= otherMaskRightFinenessX) ||
                  (otherMaskLeftFinenessX <= maskRightFinenessX && maskRightFinenessX <= otherMaskRightFinenessX)))
                return CollisionStatus.NoCollision;

            // now check for y-axis
            int otherMaskFinenessY = otherCollidable.Y * Board.FinenessLimit * 2 + otherCollidable.FinenessY;
            int otherMaskTopFinenessY = otherMaskFinenessY + otherMask.TopMargin;
            int otherMaskBottomFinenessY = otherMaskFinenessY + Board.FinenessLimit * 2 - 1 - otherMask.BottomMargin;

            int maskFinenessY = nextPosition.Y * Board.FinenessLimit * 2 + nextPosition.FinenessY;
            int maskTopFinenessY = maskFinenessY + mask.TopMargin;
            int maskBottomFinenessY = maskFinenessY + Board.FinenessLimit * 2 - 1 - mask.BottomMargin;

            // check collision now by comparing the mask
            // if y doesn't collide, return false
            if (!((maskTopFinenessY <= otherMaskTopFinenessY && otherMaskTopFinenessY <= maskBottomFinenessY) ||
                 (maskTopFinenessY <= otherMaskBottomFinenessY && otherMaskBottomFinenessY <= maskBottomFinenessY) ||
                 (otherMaskTopFinenessY <= maskTopFinenessY && maskTopFinenessY <= otherMaskBottomFinenessY) ||
                 (otherMaskTopFinenessY <= maskBottomFinenessY && maskBottomFinenessY <= otherMaskBottomFinenessY)))
                return CollisionStatus.NoCollision;

            // this part onwards reveals where the collision is headed from
            CollisionStatus statusForOther;

            // first check for direction of collision for Y to solve a serious bug
            int previousFinenessY = previousPosition.Y * Board.FinenessLimit * 2 + previousPosition.FinenessY;
            int previousTopFinenessY = previousFinenessY + mask.TopMargin;
            int previousBottomFinenessY = previousFinenessY + Board.FinenessLimit * 2 - 1 - mask.BottomMargin;

            if (previousBottomFinenessY < otherMaskTopFinenessY)
            {
                collisionDistance = otherMaskTopFinenessY - previousBottomFinenessY - 1;
                statusForOther = CollisionStatus.CollisionFromTop;
            }

            else if (previousTopFinenessY > otherMaskBottomFinenessY)
            {
                collisionDistance = previousTopFinenessY - otherMaskBottomFinenessY - 1;
                statusForOther = CollisionStatus.CollisionFromBottom;
            }

            else
            {
                // next check for direction of collision for X first
                int previousFinenessX = previousPosition.X * Board.FinenessLimit * 2 + previousPosition.FinenessX;
                int previousLeftFinenessX = previousFinenessX + mask.LeftMargin;
                int previousRightFinenessX = previousFinenessX + Board.FinenessLimit * 2 - 1 - mask.RightMargin;

                if (previousRightFinenessX < otherMaskLeftFinenessX)
                {
                    // there is a need to minus 1 because without minusing, the two masks will exactly clip onto each other
                    // at the boundary. By minusing 1, the two masks will be exactly at side by side.
                    collisionDistance = otherMaskLeftFinenessX - previousRightFinenessX - 1;
                    statusForOther = CollisionStatus.CollisionFromLeft;
                }

                else
                {
                    collisionDistance = previousLeftFinenessX - otherMaskRightFinenessX - 1;
                    statusForOther = CollisionStatus.CollisionFromRight;
                }
            }

            return statusForOther;
        }

        protected CollisionStatus PerformCollidingInto(Position nextPosition, RectangularMask mask, Collidable<TBoard> otherCollidable)
        {
            // creates a dummy variable just to store the out value
            int dummyCollisionDistance;
            return PerformCollidingInto(new Position(), nextPosition, mask, otherCollidable, out dummyCollisionDistance);
        }

        protected CollisionStatus PerformCollidingInto(Position previousPosition, Position nextPosition, RectangularMask mask, Collidable<TBoard> otherCollidable, out int collisionDistance)
        {
            // instead of just checking for collision, this method will also trigger the collision after events
            CollisionStatus statusForOther = IsCollidingInto(previousPosition, nextPosition, mask, otherCollidable, out collisionDistance);
            CollisionStatus statusForItself = CollisionStatus.NoCollision;

            switch (statusForOther)
            {
                case CollisionStatus.CollisionFromTop:
                    statusForItself = CollisionStatus.CollisionFromBottom;
                    break;
                case CollisionStatus.CollisionFromBottom:
                    statusForItself = CollisionStatus.CollisionFromTop;
                    break;
                case CollisionStatus.CollisionFromLeft:
                    statusForItself = CollisionStatus.CollisionFromRight;
                    break;
                case CollisionStatus.CollisionFromRight:
                    statusForItself = CollisionStatus.CollisionFromLeft;
                    break;
            }

            // calls class-specific behavioural methods. For instance, these methods will trigger removal of bananas from board when they collide with the monkey.
            if (statusForOther != CollisionStatus.NoCollision)
            {
                CollisionStatus newStatusForItself = BeforeCollision(otherCollidable, statusForItself);
                CollisionStatus newStatusForOther;

                if (newStatusForItself != CollisionStatus.NoCollision)
                    newStatusForOther = otherCollidable.BeforeCollision(this, statusForOther);
                else
                    newStatusForOther = CollisionStatus.NoCollision;

                CollisionAfter(otherCollidable, newStatusForItself);
                otherCollidable.CollisionAfter(this, newStatusForOther);

                return newStatusForOther;
            }

            return statusForOther;
        }

        // finds other collidables upto 1 X/Y away from itself, so that checks for collisions dont need to go up against all the board's collidables, which would be inefficient
        protected IEnumerable<Collidable<TBoard>> FindCollidablesWithinRegion()
        {
            List<Collidable<TBoard>> regionCollidableList = new List<Collidable<TBoard>>();

            for (int i = 0; i < Board.CollisionManager.CollidableList.Count; i++)
            {
                Collidable<TBoard> element = Board.CollisionManager.CollidableList[i];

                float xDiff = (element.X - this.X) / 2;
                float yDiff = (element.Y - this.Y) / 2;

                if (xDiff < 0.5 && yDiff < 0.5)
                    regionCollidableList.Add(element);
            }

            return regionCollidableList;
        }

        protected bool CheckHasCollision(Position nextPosition)
        {
            // IMPORTANT
            // code to check for collision with all other objects here
            // return false if the move is legal (or no collision occurred)
            // return true if collision occurred -> move becomes illegal
            // when returning false, the positional object will not move
            bool isColliding = false;

            // TODO:
            // now brute force checking with every collidable objects which is very bad
            // make use of algo to efficiently search for things that are in vicinity and is likely to collide
            // avoid doing checking of collision with those that will surely not be in collision
            // likely to use lists that stores left most x and right most x, top most y and bottom most y
            // and also binary search for fast retrieval
            foreach (Collidable<TBoard> otherCollidable in FindCollidablesWithinRegion())
            {
                // don't check against itself
                // otherwise it's a sure collision
                if (this == otherCollidable)
                    continue;

                if (PerformCollidingInto(nextPosition, RectangularMask, otherCollidable) != CollisionStatus.NoCollision)
                    isColliding = true;
            }

            return isColliding;
        }

        // this method exists so that derived classes can decide what changes can be made
        // to itself just before a collision happen, like for example,
        // removing bananas as the monkey collects them
        protected virtual CollisionStatus BeforeCollision(Collidable<TBoard> otherCollidable, CollisionStatus direction)
        {
            // this is meant for overriding
            // but by default, there should be no effect

            // if we do not want CollisionAfter to be invoked after this method, simply set direction to
            // no collision

            return direction;
        }

        // this method exists so that derived classes can decide what changes can be made
        // to itself after a collision with another collidable object
        protected virtual void CollisionAfter(Collidable<TBoard> otherCollidable, CollisionStatus direction)
        {
            // this is meant for overriding
            // but by default, there should be no effect
        }

        // overriding from Positional since setting position for collidable
        // requires checking of any collision first
        // if there are any collision, do not allow the collidable object to move to the spot
        public override bool SetPosition(Position nextPosition)
        {
            if (Board != null)
            {
                // check for collision with CollisionManager
                // if there is collision, return false
                // if not, continue to pass to Positional for further checking

                // *** this part has been commented out for the other maps to work first ***

                //if (CheckHasCollision(nextPosition))
                //    //return false;
                //    throw new Exception(string.Format("Game will crash because {0} is trying to set at X: {1}, Y: {2}", GetType(), nextPosition.X, nextPosition.Y));
            }

            // okay here so pass to Positional to check
            return base.SetPosition(nextPosition);
        }

        #endregion

        #region Constructors

        public Collidable(RectangularMask rectangularMask = default(RectangularMask), Position position = default(Position)) : base(position)
        {
            RectangularMask = rectangularMask;
        }

        #endregion
    }

    // enumeration to check the status of collision when two collidable objects collide
    // used for adjusting of velocity of the moving object
    public enum CollisionStatus
    {
        NoCollision, CollisionFromLeft, CollisionFromRight, CollisionFromTop, CollisionFromBottom
    }

    // enumeration to check for special collision modes
    // basically only used for a few types of Logic types like Door
    public enum CollidingMode
    {
        NoException, Inclusion, Exclusion
    }
}
