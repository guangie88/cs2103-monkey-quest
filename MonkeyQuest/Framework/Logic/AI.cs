using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class AI<TBoard> : Movable<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private IMovement previousMovement;
        private SoundPlayer monkeyKilledSound;
        private SoundPlayer aiKilledSound;

        #endregion

        #region Properties

        public IMovement PreviousMovement
        {
            get { return previousMovement; }
            protected set { previousMovement = value; }
        }

        public SoundPlayer MonkeyKilledSound
        {
            get { return monkeyKilledSound; }
            protected set { monkeyKilledSound = value; }
        }

        public SoundPlayer AIKilledSound
        {
            get { return aiKilledSound; }
            protected set { aiKilledSound = value; }
        }

        #endregion

        #region Events

        protected internal override event CollidingEventHandler<TBoard> Colliding;

        #endregion

        #region Methods

        // must be overriden, provides basic initial movements to the AI
        protected abstract void InitializeMovement();

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            // automatically initialize movements
            InitializeMovement();
        }

        protected virtual void OnColliding(CollisionEventArgs<TBoard> e)
        {
            if (Colliding != null)
                Colliding(this, e);
        }

        protected override void CollisionAfter(Collidable<TBoard> otherCollidable, CollisionStatus direction)
        {
            OnColliding(new CollisionEventArgs<TBoard>(otherCollidable, direction));
            base.CollisionAfter(otherCollidable, direction);
        }

        #endregion

        #region Constructors

        public AI()
            : this(default(RectangularMask))
        {
        }

        public AI(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
        }

        #endregion
    }

    public delegate void CollidingEventHandler<TBoard>(object sender, CollisionEventArgs<TBoard> e) where TBoard : Board<TBoard>;

    public class CollisionEventArgs<TBoard> : EventArgs
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private CollisionStatus direction;
        private Collidable<TBoard> otherCollidable;

        #endregion

        #region Properties

        public CollisionStatus Direction
        {
            get { return direction; }
            protected set { direction = value; }
        }

        public Collidable<TBoard> OtherCollidable
        {
            get { return otherCollidable; }
            protected set { otherCollidable = value; }
        }

        #endregion

        #region Constructors

        public CollisionEventArgs(Collidable<TBoard> otherCollidable, CollisionStatus direction)
        {
            Direction = direction;
            OtherCollidable = otherCollidable;
        }

        #endregion
    }
}
