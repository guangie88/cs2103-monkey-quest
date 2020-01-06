using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Media;
using System.Windows.Media.Imaging;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.Resources;
using System.Windows.Input;
using MonkeyQuest.MonkeyQuest.UI;

namespace MonkeyQuest.Test
{
    public class TestUIWindow : Window
    {
        public TestUIWindow(TestPackage boardPackage) : this(boardPackage.BoardUI, boardPackage.MainBarUI)
        {
            this.PreviewKeyDown += boardPackage.RestartKeyBinding;
        }

        public TestUIWindow(MonkeyBoardUI boardUI, MonkeyMainBarUI mainbarUI)
        {
            // sets the title
            Title = "Monkey Quest Tester";

            // sets the icon
            Uri iconUri = new Uri(@"Resources\Images\MonkeyQuestIcon.ico", UriKind.Relative);
            Icon = BitmapFrame.Create(iconUri);

            // sets the default height and width when window first appears
            Height = 680;
            Width = 1076;

            // sets background outside of game area
            Background = Brushes.Black;

            // maintains aspect ratio
            Viewbox displayedWindow = new Viewbox();
            displayedWindow.StretchDirection = StretchDirection.Both;
            displayedWindow.Stretch = Stretch.Uniform;

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

            displayedWindow.Child = displayedBoardUI;
            Content = displayedWindow;
        }
    }
}
