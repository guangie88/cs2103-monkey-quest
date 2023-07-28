// Main Contributors: Silin

using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.Resources;

namespace MonkeyQuest.MonkeyQuest.UI
{
    public class MonkeyBoardUIWindow : Window
    {
        #region Data Fields

        #endregion

        #region Properties

        public MonkeyBoardPackage BoardPackage
        {
            get;
            protected set;
        }

        #endregion

        #region Events

        #endregion

        #region Methods

        private void InitializeSettings()
        {
            // sets the title
            Title = "Monkey Quest";

            // sets the icon
            Uri iconUri = new Uri(@"Resources\Images\MonkeyQuestIcon.ico", UriKind.Relative);
            Icon = BitmapFrame.Create(iconUri);

            // sets the default height and width when window first appears
            Height = 680;
            Width = 1076;

            // sets background outside of game area
            Background = Brushes.Black;
        }

        private void ClearDisplay()
        {
            Content = null;
        }

        private void SetupDisplay()
        {
            BoardUI<MonkeyBoard> boardUI = BoardPackage.BoardUI;
            MainBarUI<MonkeyBoard> mainbarUI = BoardPackage.MainBarUI;

            // maintains aspect ratio
            Viewbox displayedWindow = new Viewbox();
            displayedWindow.StretchDirection = StretchDirection.Both;
            displayedWindow.Stretch = Stretch.Uniform;

            Grid displayedGrid = new Grid();
            displayedGrid.Height = 680;
            displayedGrid.Width = 1120;

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            row1.Height = new GridLength(50);
            displayedGrid.RowDefinitions.Add(row1);
            displayedGrid.RowDefinitions.Add(row2);

            Viewbox background = new Viewbox();
            Image backgroundImage = new Image();
            backgroundImage.Source = Images.NatureForest;
            background.Child = backgroundImage;
            background.Stretch = Stretch.UniformToFill;
            background.ClipToBounds = true;
            background.SetValue(Canvas.ZIndexProperty, -1);

            // displayed game area
            Viewbox displayedBoardUI = new Viewbox();
            displayedBoardUI.Child = boardUI;
            displayedBoardUI.Stretch = Stretch.UniformToFill;
            displayedBoardUI.ClipToBounds = true;

            Grid.SetRow(background, 0);
            Grid.SetRowSpan(background, 2);
            Grid.SetRow(mainbarUI, 0);
            Grid.SetRow(displayedBoardUI, 1);

            displayedGrid.Children.Add(background);
            displayedGrid.Children.Add(mainbarUI);
            displayedGrid.Children.Add(displayedBoardUI);

            displayedWindow.Child = displayedGrid;
            Content = displayedWindow;
        }

        public void ClearBoardPackage()
        {
            if (BoardPackage != null)
            {
                ClearDisplay();
                this.PreviewKeyDown -= BoardPackage.RestartKeyBinding;
                this.PreviewKeyDown -= BoardPackage.AddLifeBinding;
                BoardPackage = null;
            }
        }

        public void SetBoardPackage(MonkeyBoardPackage boardPackage)
        {
            if (boardPackage != null)
            {
                BoardPackage = boardPackage;
                this.PreviewKeyDown += BoardPackage.RestartKeyBinding;
                this.PreviewKeyDown += BoardPackage.AddLifeBinding;
                SetupDisplay();
            }
        }

        #endregion

        #region Constructors

        public MonkeyBoardUIWindow(MonkeyBoardPackage boardPackage)
        {
            InitializeSettings();
            SetBoardPackage(boardPackage);
        }

        #endregion
    }
}
