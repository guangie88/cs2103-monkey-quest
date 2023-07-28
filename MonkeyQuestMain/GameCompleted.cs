using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MonkeyQuestMain
{
    public class GameCompletedWindow : Window
    {
        #region Data Fields

        private const string FontUri = "pack://application:,,,";
        private static FontFamily fontFamily = new FontFamily(new Uri(FontUri, UriKind.Absolute), "MonkeyQuest;Component/Resources/Fonts/#Hurry Up");
        private const double FontSizeValue = 30;

        private const int HeightValue = 300;
        private const int WidthValue = 800;

        private Grid mainGrid = new Grid();

        private TextBlock displayText = new TextBlock();
        private TextBlock displayText2 = new TextBlock();
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
            displayText2.FontFamily = fontFamily;
            decisionText.FontFamily = fontFamily;

            displayText.FontSize = FontSizeValue;
            displayText2.FontSize = FontSizeValue;
            decisionText.FontSize = FontSizeValue;

            displayText.TextAlignment = TextAlignment.Center;
            displayText.Margin = new Thickness(0, 92, 0, 0);
            displayText.Foreground = Brushes.WhiteSmoke;

            displayText2.TextAlignment = TextAlignment.Center;
            displayText2.Margin = new Thickness(0, 130, 0, 0);
            displayText2.Foreground = Brushes.WhiteSmoke;

            decisionText.TextAlignment = TextAlignment.Center;
            decisionText.Margin = new Thickness(0, 168, 0, 0);
            decisionText.Foreground = Brushes.WhiteSmoke;

            displayText.Text = "Congratulations! You have completed all the maps!";
            displayText2.Text = "Try pressing 'V' and 'B' in-game for more fun!";
            decisionText.Text = "(Press any key to continue)";

            Content = mainGrid;
            mainGrid.Children.Add(displayText);
            mainGrid.Children.Add(displayText2);
            mainGrid.Children.Add(decisionText);
        }

        private void InitializeKeyBindings()
        {
            PreviewKeyDown += (sender, e) => this.Close();
        }

        #endregion

        #region Constructors

        public GameCompletedWindow()
        {
            InitializeWindowSettings();
            InitializeDisplay();
            InitializeKeyBindings();
        }

        #endregion
    }
}