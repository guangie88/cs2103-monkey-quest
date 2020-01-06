using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public abstract class BasicWeapon : Weapon<MonkeyBoard>
    {
        #region Events

        #endregion

        #region Methods

        protected override void InitializeMovement()
        {
            // adds gravity
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is AI<MonkeyBoard>) {
                AIKilledSound.Play();
                Board.RemovePositional(otherCollidable);
                Board.RemovePositional(this);
            }
            if (otherCollidable is BorderTile)
            {
                Board.RemovePositional(this);
            }

        }

        #endregion

        protected override void RemoveBindings()
        {
            base.RemoveBindings();
            HomingSound.Stop();
        }

        #region Constructors

        public BasicWeapon(RectangularMask rectangularMask)
            : base(rectangularMask)
        {
            base.CollidingMode = CollidingMode.Inclusion;
            CollidingInclusionSet.Add(typeof(AI<MonkeyBoard>));
            CollidingInclusionSet.Add(typeof(BorderTile));
        }

        #endregion
    }
}
