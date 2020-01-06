using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.Resources;
using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.Resources.Utility;

namespace MonkeyQuest.MonkeyQuest.UI
{
    public class TileUI : PositionalUI<MonkeyBoard, Tile, Image>
    {
        #region Data Fields

        #endregion

        #region Properties

        public string LogicIdentifier
        {
            get { return Logic.TileIdentifier; }
        }

        #endregion

        #region Events

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public TileUI(Tile tile, Image imageToDisplay, MonkeyBoardUI boardUI = null)
            : base(tile, imageToDisplay, boardUI)
        {
        }

        #endregion
    }
}