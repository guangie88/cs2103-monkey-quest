using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using MonkeyQuest.Resources;

using MonkeyQuest.Framework.UI;

using MonkeyQuest.MonkeyQuest.Logic;

namespace MonkeyQuest.MonkeyQuest.UI
{
    public class MonkeyMainBarUI : MainBarUI<MonkeyBoard>
    {
        #region Data Fields

        private TextBlock level = new TextBlock();
        private TextBlock counterOnBoard = new TextBlock();
        private TextBlock livesCounter = new TextBlock();
        private TextBlock timerOnBoard = new TextBlock();

        private const string LevelStringFormat = "Level {0}";
        private const string BananaCounterStringFormat = "Bananas: {0}/{1}";
        private const string LivesCounterStringFormat = "Lives: {0}";
        private const string TimerStringFormat = "Time Remaining: {0}";

        #endregion

        #region Properties

        public override MonkeyBoard Board
        {
            get { return base.Board; }
            set
            {
                base.Board = value;
                InitializeDisplayText();
            }
        }

        public TextBlock Level
        {
            get { return level; }
            protected set { level = value; }
        }

        public TextBlock CounterOnBoard
        {
            get { return counterOnBoard; }
            protected set { counterOnBoard = value; }
        }

        public TextBlock LivesCounter
        {
            get { return livesCounter; }
            protected set { livesCounter = value; }
        }

        public TextBlock TimerOnBoard
        {
            get { return timerOnBoard; }
            protected set { timerOnBoard = value; }
        }

        #endregion

        #region Methods

        private void InitializeSetContent()
        {
            Grid elementsHolder = new Grid();
            elementsHolder.Height = 50;
            elementsHolder.Width = 1120;

            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            ColumnDefinition column3 = new ColumnDefinition();
            ColumnDefinition column4 = new ColumnDefinition();
            ColumnDefinition column5 = new ColumnDefinition();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            column1.Width = new GridLength(elementsHolder.Width / 5);
            column2.Width = new GridLength(elementsHolder.Width / 10 * 3);
            column3.Width = new GridLength(elementsHolder.Width / 10);
            column4.Width = new GridLength(elementsHolder.Width / 5);
            column5.Width = new GridLength(elementsHolder.Width / 4);
            row2.Height = new GridLength(5);

            elementsHolder.ColumnDefinitions.Add(column1);
            elementsHolder.ColumnDefinitions.Add(column2);
            elementsHolder.ColumnDefinitions.Add(column3);
            elementsHolder.ColumnDefinitions.Add(column4);
            elementsHolder.ColumnDefinitions.Add(column5);
            elementsHolder.RowDefinitions.Add(row1);
            elementsHolder.RowDefinitions.Add(row2);

            const string FontUri = "pack://application:,,,";
            FontFamily fontFamily = new FontFamily(new Uri(FontUri, UriKind.Absolute), "MonkeyQuest;Component/Resources/Fonts/#Hurry Up");

            Level.Text = "Level 1";
            Level.FontSize = 30;
            Level.FontFamily = fontFamily;
            Level.Foreground = Brushes.BurlyWood;
            Level.TextAlignment = TextAlignment.Center;
            Level.VerticalAlignment = VerticalAlignment.Center;

            Grid bananaCounterHolder = new Grid();
            ColumnDefinition bcounterCol1 = new ColumnDefinition();
            ColumnDefinition bcounterCol2 = new ColumnDefinition();
            bcounterCol1.Width = GridLength.Auto;
            bananaCounterHolder.ColumnDefinitions.Add(bcounterCol1);
            bananaCounterHolder.ColumnDefinitions.Add(bcounterCol2);

            CounterOnBoard.FontSize = 25;
            CounterOnBoard.FontFamily = fontFamily;
            CounterOnBoard.Foreground = Brushes.BurlyWood;
            CounterOnBoard.TextAlignment = TextAlignment.Center;
            CounterOnBoard.VerticalAlignment = VerticalAlignment.Center;
            CounterOnBoard.HorizontalAlignment = HorizontalAlignment.Right;

            Image banana = new Image();
            banana.Source = Images.Banana1;
            banana.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            banana.VerticalAlignment = VerticalAlignment.Center;
            banana.HorizontalAlignment = HorizontalAlignment.Left;

            Grid.SetColumn(CounterOnBoard, 0);
            Grid.SetColumn(banana, 1);
            bananaCounterHolder.Children.Add(CounterOnBoard);
            bananaCounterHolder.Children.Add(banana);
            bananaCounterHolder.VerticalAlignment = VerticalAlignment.Top;

            Grid livesCounterHolder = new Grid();
            ColumnDefinition lcounterCol1 = new ColumnDefinition();
            ColumnDefinition lcounterCol2 = new ColumnDefinition();
            lcounterCol1.Width = GridLength.Auto;
            livesCounterHolder.ColumnDefinitions.Add(lcounterCol1);
            livesCounterHolder.ColumnDefinitions.Add(lcounterCol2);

            LivesCounter.FontSize = 25;
            LivesCounter.FontFamily = fontFamily;
            LivesCounter.Foreground = Brushes.BurlyWood;
            LivesCounter.TextAlignment = TextAlignment.Center;
            LivesCounter.VerticalAlignment = VerticalAlignment.Center;
            LivesCounter.HorizontalAlignment = HorizontalAlignment.Right;

            Image heart = new Image();
            heart.Source = Images.Heart;
            heart.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            heart.VerticalAlignment = VerticalAlignment.Center;
            heart.HorizontalAlignment = HorizontalAlignment.Left;

            Grid.SetColumn(LivesCounter, 0);
            Grid.SetColumn(heart, 1);
            livesCounterHolder.Children.Add(LivesCounter);
            livesCounterHolder.Children.Add(heart);
            livesCounterHolder.VerticalAlignment = VerticalAlignment.Top;

            Grid timerHolder = new Grid();
            ColumnDefinition tcounterCol1 = new ColumnDefinition();
            tcounterCol1.Width = GridLength.Auto;
            timerHolder.ColumnDefinitions.Add(tcounterCol1);
            
            TimerOnBoard.FontSize = 25;
            TimerOnBoard.FontFamily = fontFamily;
            TimerOnBoard.Foreground = Brushes.BurlyWood;
            TimerOnBoard.TextAlignment = TextAlignment.Center;
            TimerOnBoard.VerticalAlignment = VerticalAlignment.Center;
            TimerOnBoard.HorizontalAlignment = HorizontalAlignment.Right;

            Grid.SetColumn(TimerOnBoard, 0);
            timerHolder.Children.Add(TimerOnBoard);
            timerHolder.VerticalAlignment = VerticalAlignment.Center;

            Rectangle borderLine = new Rectangle();
            borderLine.Fill = Brushes.DarkGoldenrod;

            Grid.SetColumn(level, 0);
            Grid.SetColumn(timerHolder, 1);
            Grid.SetColumn(livesCounterHolder, 3);
            Grid.SetColumn(bananaCounterHolder, 4);
            Grid.SetRow(borderLine, 1);
            Grid.SetColumnSpan(borderLine, 5);
            elementsHolder.Children.Add(level);
            elementsHolder.Children.Add(bananaCounterHolder);
            elementsHolder.Children.Add(livesCounterHolder);
            elementsHolder.Children.Add(timerHolder);
            elementsHolder.Children.Add(borderLine);

            Children.Add(elementsHolder);
        }

        private void InitializeDisplayText()
        {
            // initial display
            if (Board == null)
            {
                Level.Text = string.Format(LevelStringFormat, "-");
                CounterOnBoard.Text = string.Format(BananaCounterStringFormat, 0, 0);
                LivesCounter.Text = string.Format(LivesCounterStringFormat, 0);
                TimerOnBoard.Text = string.Format(TimerStringFormat, 0);
            }
            else
            {
                Level.Text = string.Format(LevelStringFormat, Board.BoardLevel);
                CounterOnBoard.Text = string.Format(BananaCounterStringFormat, Board.BananasCollected, Board.BananasTotal);
                LivesCounter.Text = string.Format(LivesCounterStringFormat, Board.NumberOfLives);
                TimerOnBoard.Text = string.Format(TimerStringFormat, Board.BoardTime);
            }
        }

        private void CounterOnBoardTextBinding(object sender, BananasEventArgs e)
        {
            CounterOnBoard.Text = string.Format(BananaCounterStringFormat, e.BananasCollected.ToString(), e.BananasTotal.ToString());
        }

        private void LivesCounterTextBinding(object sender, MonkeyLivesEventArgs e)
        {
            LivesCounter.Text = string.Format(LivesCounterStringFormat, e.NumberofLives.ToString());
        }

        private void TimerBinding(object sender, TimerEventArgs e)
        {                        
            TimerOnBoard.Text = string.Format(TimerStringFormat, e.RemainingTime.ToString());
        }

        protected override void RemoveBindings()
        {
            Board.BananaCollect -= CounterOnBoardTextBinding;
            Board.BananasTotalChanged -= CounterOnBoardTextBinding;
            Board.LivesLeft -= LivesCounterTextBinding;
            Board.BoardTimerFullTick -= TimerBinding;
        }

        protected override void InitializeBindings()
        {
            Board.BananaCollect += CounterOnBoardTextBinding;
            Board.BananasTotalChanged += CounterOnBoardTextBinding;
            Board.LivesLeft += LivesCounterTextBinding;
            Board.BoardTimerFullTick += TimerBinding;
        }

        #endregion

        #region Constructors

        public MonkeyMainBarUI(MonkeyBoard board)
            : base(board)
        {
            InitializeSetContent();
            // all the rest of the work done though setting Board = board (virtual property)
        }

        #endregion
    }
}
