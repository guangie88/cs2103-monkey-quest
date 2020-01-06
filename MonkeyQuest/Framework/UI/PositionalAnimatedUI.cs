// Main Contributors: Frank

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.UI
{
    public abstract class PositionalAnimatedUI<TBoard, TPositional, TAnimatedImage> : PositionalUI<TBoard, TPositional, TAnimatedImage>
        where TBoard : Board<TBoard>
        where TPositional : Positional<TBoard>
        where TAnimatedImage : AnimatedImage
    {
        #region Methods

        //protected override void RemoveSelf(Object o, PositionalEventArgs e) {
        //    if (e.Positional.Equals(this.Positional))
        //    {
        //        BoardUI.Board.BoardTick -= PerformTween;
        //    }
        //}

        protected internal virtual void PerformTween(object sender, EventArgs e)
        {
            ImageToDisplay.IncrementFrame();
        }

        // adds to boardUI and then perform the required bindings
        protected internal override void AddToBoardUI(BoardUI<TBoard> boardUI)
        {
            // make sure base AddToBoardUI is called first to set up the properties
            base.AddToBoardUI(boardUI);
            // performs tweening

            if (BoardUI != null)
                BoardUI.Board.BoardTick += PerformTween;
        }

        #endregion

        #region Constructors

        internal protected PositionalAnimatedUI(TPositional positional, TAnimatedImage imageToDisplay, BoardUI<TBoard> boardUI) :
            base(positional, imageToDisplay, boardUI)
        {
            if (boardUI != null)
            {
                boardUI.Board.BoardTick += PerformTween;
                //boardUI.RemovePosUI += RemoveSelf;
            }            
        }

        #endregion
    }
}
