using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.MonkeyQuest.UI;
using System.Windows.Input;

namespace MonkeyQuestMain
{
    public class GameoverWindow : Window
    {
        #region Data Fields

        private const string FontUri = "pack://application:,,,";
        private static FontFamily fontFamily = new FontFamily(new Uri(FontUri, UriKind.Absolute), "MonkeyQuest;Component/Resources/Fonts/#Hurry Up");
        private const double FontSizeValue = 30;

        private const int HeightValue = 300;
        private const int WidthValue = 750;

        private Grid mainGrid = new Grid();

        private TextBlock displayText = new TextBlock();
        private TextBlock decisionText = new TextBlock();

        #endregion

        #region Properties

        #endregion

        #region Events

        public event GameoverKeysEventHandler GameoverDecision;

        #endregion

        #region Methods

        private void InitializeWindowSettings()
        {
            this.Width = WidthValue;
            this.Height = HeightValue;

            this.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            this.Title = "MonkeyQuest Gameover...";
            this.WindowStyle = WindowStyle.None;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void InitializeDisplay()
        {
            displayText.FontFamily = fontFamily;
            decisionText.FontFamily = fontFamily;

            displayText.FontSize = FontSizeValue;
            decisionText.FontSize = FontSizeValue;

            displayText.TextAlignment = TextAlignment.Center;
            displayText.Margin = new Thickness(0, 118, 0, 0);
            displayText.Foreground = Brushes.WhiteSmoke;

            decisionText.TextAlignment = TextAlignment.Center;
            decisionText.Margin = new Thickness(0, 156, 0, 0);
            decisionText.Foreground = Brushes.WhiteSmoke;

            displayText.Text = "Game Over! Do you wish to restart the game?";
            decisionText.Text = "('Y' to restart, 'N' to quit game)";

            Content = mainGrid;
            mainGrid.Children.Add(displayText);
            mainGrid.Children.Add(decisionText);
        }

        private void InitializeKeyBindings()
        {
            PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Y)
                {
                    OnGameoverDecision(new GameoverKeyEventArgs(GameoverState.TryAgain));
                    this.Close();
                }
                else if (e.Key == Key.N)
                {
                    OnGameoverDecision(new GameoverKeyEventArgs(GameoverState.Quit));
                    this.Close();
                }
            };
        }

        protected virtual void OnGameoverDecision(GameoverKeyEventArgs e)
        {
            if (GameoverDecision != null)
                GameoverDecision(this, e);
        }

        #endregion

        #region Constructors

        public GameoverWindow()
        {
            InitializeWindowSettings();
            InitializeDisplay();
            InitializeKeyBindings();
        }

        #endregion
    }

    public delegate void GameoverKeysEventHandler(object sender, GameoverKeyEventArgs e);

    public class GameoverKeyEventArgs
    {
        #region Data Fields

        private GameoverState gameoverState;

        #endregion

        #region Properties

        public GameoverState GameoverState
        {
            get { return gameoverState; }
            protected set { gameoverState = value; }
        }

        #endregion

        #region Constructors

        public GameoverKeyEventArgs(GameoverState gameoverState)
        {
            GameoverState = gameoverState;
        }

        #endregion
    }

    public enum GameoverState { TryAgain, Quit };
}