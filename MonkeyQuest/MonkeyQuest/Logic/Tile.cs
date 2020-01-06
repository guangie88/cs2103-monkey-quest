using System;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Resources.Utility;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public class Tile : Collidable<MonkeyBoard>
    {
        #region Data Fields

        private bool canGroundJumpOn = true;
        private bool canWallJumpOn = true;

        private string tileIdentifier = "";

        #endregion

        #region Properties

        public override string LogicIdentifier
        {
            get { return GetType().ToString() + TileIdentifier; }
        }

        public bool CanGroundJumpOn
        {
            get { return canGroundJumpOn; }
            protected set { canGroundJumpOn = value; }
        }

        public bool CanWallJumpOn
        {
            get { return canWallJumpOn; }
            protected set { canWallJumpOn = value; }
        }

        public string TileIdentifier
        {
            get { return tileIdentifier; }
            protected set { tileIdentifier = value; }
        }

        #endregion

        #region Events

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public Tile(string tileIdentifier)
            : this(tileIdentifier, new RectangularMask())
        {
        }

        public Tile(string tileIdentifier, RectangularMask rectangularMask) : base(rectangularMask)
        {
            // the identifier is meant for creating the correct TileUI
            TileIdentifier = tileIdentifier;
        }

        #endregion
    }
}