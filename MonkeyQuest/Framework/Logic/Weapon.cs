using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Weapon<TBoard> : Movable<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private IMovement previousMovement;
        private SoundPlayer aiKilledSound;
        private SoundPlayer homingSound;
        #endregion

        #region Properties

        public IMovement PreviousMovement
        {
            get { return previousMovement; }
            protected set { previousMovement = value; }
        }

        public SoundPlayer HomingSound
        {
            get { return homingSound;  }
            protected set { homingSound = value; }
        }

        public SoundPlayer AIKilledSound
        {
            get { return aiKilledSound; }
            protected set { aiKilledSound = value; }
        }

        #endregion

        #region Events

        public override event CollidingEventHandler<TBoard> Colliding;

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

        public Weapon()
            : this(default(RectangularMask))
        {
        }

        public Weapon(RectangularMask rectangularMask)
            : base(rectangularMask)
        {

        }

        #endregion
    }
}
