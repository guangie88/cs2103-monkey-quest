// Main Contributors: Frank

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.MonkeyQuest.Logic;

using MonkeyQuest.Resources;
using MonkeyQuest.Resources.Utility;

namespace MonkeyQuest.MonkeyQuest.UI
{
    public class DoorUI : PositionalUI<MonkeyBoard, Door, AnimatedImage>
    {
        #region Methods

        protected internal virtual void OpenDoor(object sender, EventArgs e)
        {
            ImageToDisplay.NextImage();
        }

        // adds to boardUI and then perform the required bindings
        protected internal override void AddToBoardUI(BoardUI<MonkeyBoard> boardUI)
        {
            // make sure base AddToBoardUI is called first to set up the properties
            base.AddToBoardUI(boardUI);
        }

        #endregion

        #region Constructors

        public DoorUI(Door positional, BoardUI<MonkeyBoard> boardUI) :
            base(positional, new AnimatedImage(
                new List<ImageFrames> { 
                    new ImageFrames(new Image() { Source = ImageManager.ElementImageDictionary["DoorClose"] }, 0), 
                    new ImageFrames(new Image() { Source = ImageManager.ElementImageDictionary["DoorOpen"] }, 0)
                }), boardUI)
        {
            if (boardUI != null)
            {
                positional.DoorOpen += this.OpenDoor;
            }
        }

        #endregion
    }
}
