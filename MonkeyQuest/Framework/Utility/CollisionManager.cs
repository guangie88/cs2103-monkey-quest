// Main Contributors: Weiguang

using System;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public class CollisionManager<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields
        
        private List<Collidable<TBoard>> collidableList;

        #endregion

        #region Properties

        public List<Collidable<TBoard>> CollidableList
        {
            get { return collidableList; }
            protected set { collidableList = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        internal CollisionManager() : this(new Collidable<TBoard>[] {})
        {
        }

        internal CollisionManager(IEnumerable<Collidable<TBoard>> collidables)
        {
            CollidableList = new List<Collidable<TBoard>>(collidables);
        }

        #endregion
    }
}
