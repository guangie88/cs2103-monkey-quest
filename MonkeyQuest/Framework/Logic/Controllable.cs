// Main Contributors: Weiguang

using System;
using System.Collections.Generic;
using System.Windows.Input;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Controllable<TBoard> : Movable<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void InitializeBindings()
        {
            base.InitializeBindings();
            Board.BoardKeyDown += BoardKeyDownMapperBind;
        }

        protected override void RemoveBindings()
        {
            Board.BoardKeyDown -= BoardKeyDownMapperBind;
            base.RemoveBindings();
        }

        protected internal abstract void BoardKeyDownMapperBind(object sender, BoardKeyEventArgs<TBoard> e);

        #endregion

        #region Constructors

        public Controllable(RectangularMask rectangularMask = default(RectangularMask), IEnumerable<IMovement> movements = null, Position position = default(Position))
            : base(rectangularMask, movements, position)
        {
        }

        #endregion
    }
}
