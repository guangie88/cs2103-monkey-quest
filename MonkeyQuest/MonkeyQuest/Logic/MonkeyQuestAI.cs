using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Media;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    // a non killable AI class
    public abstract class NonKillableNonMovingAI : AI<MonkeyBoard>
    {
        #region Methods

        protected override void InitializeMovement()
        {
            // stand still there and do nothing
        }

        protected override CollisionStatus BeforeCollision(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Monkey)
            {
                Board.RemovePositional(otherCollidable);

                if (MonkeyKilledSound != null)
                    MonkeyKilledSound.Play();

                Board.Dispatcher.Invoke((Action)(() => Board.DecreaseNumberOfLives()));

                return CollisionStatus.NoCollision;
            }

            return base.BeforeCollision(otherCollidable, direction);
        }

        #endregion

        #region Constructors

        public NonKillableNonMovingAI(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
        }

        #endregion
    }

    // a basic framework AI class to implement from for a non-moving AI
    public abstract class NonMovingAI : AI<MonkeyBoard>
    {
        #region Events

        #endregion

        #region Methods

        protected override void InitializeMovement()
        {
            // stand still there and do nothing
        }

        protected override CollisionStatus BeforeCollision(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Monkey)
            {
                if (direction == CollisionStatus.CollisionFromTop)
                    return CollisionStatus.CollisionFromTop;
                else
                {
                    Board.RemovePositional(otherCollidable);

                    if (MonkeyKilledSound != null)
                        MonkeyKilledSound.Play();

                    Board.Dispatcher.Invoke((Action)(() => Board.DecreaseNumberOfLives()));
                    return CollisionStatus.NoCollision;
                }
            }

            return base.BeforeCollision(otherCollidable, direction);
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Monkey || otherCollidable is Crate)
            {
                if (direction == CollisionStatus.CollisionFromTop)
                {
                    Board.RemovePositional(this);

                    if (AIKilledSound != null)
                        AIKilledSound.Play();
                }
            }

            base.CollisionAfter(otherCollidable, direction);
        }

        #endregion

        #region Constructors

        public NonMovingAI(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
            // set AI killed sound to be standard
            AIKilledSound = new SoundPlayer("Resources/Sfx/Enemy Killed.wav");
        }

        #endregion
    }

    // a basic framework AI class that is affected by gravity
    public abstract class NonMovingGravityAffectedAI : NonMovingAI
    {
        #region Events

        #endregion

        #region Methods

        protected override void InitializeMovement()
        {
            base.InitializeMovement();

            // adds gravity
            AddMovement(new Gravity(Board.CalculateFineness(0.02)));
        }

        #endregion

        #region Constructors

        public NonMovingGravityAffectedAI(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
        }

        #endregion
    }

    // a basic framework AI class to implement from for a collision detected AI
    public abstract class BasicMovingAI : NonMovingGravityAffectedAI
    {
        #region Data Fields

        private double movementSpeedProportion = 0.02;

        #endregion

        #region Properties

        public virtual double MovementSpeedProportion
        {
            get { return movementSpeedProportion; }
        }

        #endregion

        #region Events

        #endregion

        #region Methods

        protected override void InitializeMovement()
        {
            base.InitializeMovement();
            PreviousMovement = new SlideLeftRight(Board.CalculateFineness(MovementSpeedProportion));
            AddMovement(PreviousMovement);
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (!(otherCollidable is Monkey) && !(otherCollidable is BasicWeapon) && otherCollidable is Collidable<MonkeyBoard>)
            {
                RemoveMovement(PreviousMovement);

                if (direction == CollisionStatus.CollisionFromRight)
                    PreviousMovement = new SlideLeftRight(Board.CalculateFineness(-MovementSpeedProportion));
                else if (direction == CollisionStatus.CollisionFromLeft)
                    PreviousMovement = new SlideLeftRight(Board.CalculateFineness(MovementSpeedProportion));

                // changes the direction to reverse
                AddMovement(PreviousMovement);
            }

            base.CollisionAfter(otherCollidable, direction);
        }

        #endregion

        #region Constructors

        public BasicMovingAI(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
        }

        #endregion
    }
}