using System.Windows.Controls;
using MonkeyQuest.Framework.UI;
using MonkeyQuest.MonkeyQuest.Logic;

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