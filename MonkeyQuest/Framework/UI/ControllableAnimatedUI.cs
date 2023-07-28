// Main Contributors: Frank

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.UI
{
    public abstract class ControllableAnimatedUI<TBoard, TControllable, TControllableAnimatedImage> : PositionalAnimatedUI<TBoard, TControllable, TControllableAnimatedImage>
        where TBoard : Board<TBoard>
        where TControllable : Controllable<TBoard>
        where TControllableAnimatedImage : ControllableAnimatedImage
    {
        #region Methods
        // adds to boardUI and then perform the required bindings
        protected internal override void AddToBoardUI(BoardUI<TBoard> boardUI)
        {
            // make sure base AddToBoardUI is called first to set up the properties
            base.AddToBoardUI(boardUI);
            // performs other bindings
            if (BoardUI != null)
                boardUI.Board.BoardKeyDown += PerformKey;
        }

        protected internal virtual void PerformKey(object sender, BoardKeyEventArgs<TBoard> e)
        {
            ImageToDisplay.KeyCheck(e);
            //Debug.WriteLine("Key pressed");
        }
        #endregion

        #region Constructor

        internal protected ControllableAnimatedUI(TControllable controllable, TControllableAnimatedImage imageToDisplay, BoardUI<TBoard> boardUI) :
            base(controllable, imageToDisplay, boardUI)
        {
            Debug.WriteLine("Constructor called");
            if (boardUI != null)
            {
                boardUI.Board.BoardKeyDown += PerformKey;
            }            
        }

        #endregion
    }
}
