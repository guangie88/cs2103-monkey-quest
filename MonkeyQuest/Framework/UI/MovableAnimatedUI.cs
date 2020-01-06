// Main Contributors: Div

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.UI
{
    public abstract class MovableAnimatedUI<TBoard, TMovable, TMovableAnimatedImage> : PositionalAnimatedUI<TBoard, TMovable, TMovableAnimatedImage>
        where TBoard : Board<TBoard>
        where TMovable : Movable<TBoard>
        where TMovableAnimatedImage : MovableAnimatedImage
    {
        #region Data Fields

        #endregion

        #region Methods

        // adds to boardUI and then perform the required bindings
        protected internal override void AddToBoardUI(BoardUI<TBoard> boardUI)
        {
            // make sure base AddToBoardUI is called first to set up the properties
            base.AddToBoardUI(boardUI);
            // performs other bindings
        }

        protected void HandleCollision(object sender, CollisionEventArgs<TBoard> e)
        {
            if (e.Direction == CollisionStatus.CollisionFromTop)
                BoardUI.Board.RemovePositional(sender as Positional<TBoard>);
            else if (e.Direction == CollisionStatus.CollisionFromLeft)
            {
                PerformTween(sender, new EventArgs());
                ImageToDisplay.Right();
            }
            else if (e.Direction == CollisionStatus.CollisionFromRight)
            {
                PerformTween(sender, new EventArgs());
                ImageToDisplay.Left();
            }
            else
                PerformTween(sender, new EventArgs());                //TODO: Switch images
        }

        #endregion

        #region Constructor

        internal protected MovableAnimatedUI(TMovable movable, TMovableAnimatedImage imageToDisplay, BoardUI<TBoard> boardUI) :
            base(movable, imageToDisplay, boardUI)
        {
            Debug.WriteLine("Constructor called MovableAnimated");

            if (boardUI != null)
            {
                //boardUI.RemovePosUI += RemoveSelf;
            }

            Logic.Colliding += HandleCollision;
        }

        #endregion
    }
}
