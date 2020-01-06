// Main Contributors: Weiguang, Silin

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;
using System.Windows.Media.Animation;

namespace MonkeyQuest.Framework.UI
{
    public class BoardUI<TBoard> : Canvas
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Board<TBoard> board;
        private Image backgroundImage;

        private LogicUICreatorManager<TBoard> logicUICreatorManager = new LogicUICreatorManager<TBoard>();

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty RowDisplayUnitProperty
            = DependencyProperty.Register("RowDisplayUnit", typeof(double), typeof(BoardUI<TBoard>));

        public static readonly DependencyProperty ColumnDisplayUnitProperty
            = DependencyProperty.Register("ColumnDisplayUnit", typeof(double), typeof(BoardUI<TBoard>));

        #endregion

        #region Properties

        public double ColumnDisplayUnit
        {
            get { return (double)GetValue(ColumnDisplayUnitProperty); }
            set { SetValue(ColumnDisplayUnitProperty, value); }
        }

        public double RowDisplayUnit
        {
            get { return (double)GetValue(RowDisplayUnitProperty); }
            set { SetValue(RowDisplayUnitProperty, value); }
        }

        public virtual TBoard Board
        {
            get { return board as TBoard; }
            set
            {
                if (Board != null)
                {
                    RemoveBindings();
                    RemoveBoard();
                }

                board = value;

                if (Board != null)
                {
                    InitializeBoard();
                    InitializeBindings();            
                    BoardFadeIn(this, null);
                }
            }
        }

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        public LogicUICreatorManager<TBoard> LogicUICreatorManager
        {
            get { return logicUICreatorManager; }
            protected set { logicUICreatorManager = value; }
        }

        #endregion

        #region Events

        public event BoardUIResizeEventHandler BoardUIResize;
        public event RemovingPositionalEventHandler<TBoard> RemovePosUI;

        #endregion

        #region Methods

        private void GotFocusBinding(object sender, RoutedEventArgs e)
        {
            this.Board.Start();
        }

        private void LostFocusBinding(object sender, RoutedEventArgs e)
        {
            this.Board.Pause();
        }

        private void InitializeBackgroundImage()
        {
            const int BACKGROUND_ZINDEX = -1;

            BackgroundImage = new Image();

            // this is to set the background behind everything else
            SetValue(Canvas.ZIndexProperty, BACKGROUND_ZINDEX);

            // there is no need to bind the width and height of the image to the canvas
            // because it is automatically done

            // displays the image
            this.Children.Add(BackgroundImage);
        }

        private void RemoveBindings()
        {
            // removes all current board bindings
            Board.AddingPositional -= AddingPositionalBinding;
            Board.RemovingPositional -= RemovingPositionalBinding;
            Board.TempRemovingPositional -= TempRemovingPositionalBinding;
            Board.RestoringPositional -= RestoringPositionalBinding;
            Board.BoardFading -= BoardFadeOut;
        }

        private void SizeChangedBinding(object sender, SizeChangedEventArgs e)
        {
            if (BoardUIResize != null)
                BoardUIResize(this, new BoardUIResizeEventArgs(e.NewSize, Board.Columns, Board.Rows));
        }

        private void RemoveBoard()
        {
            Board.BoardResize -= BoardResizeBind;
            SizeChanged -= SizeChangedBinding;

            // culprit
            Dispatcher.Invoke((Action)(() => this.Children.Clear()));
        }

        private void InitializeBindings()
        {
            // display portion
            
            Binding columnDisplayUnitBinding = new Binding("Width");
            Binding rowDisplayUnitBinding = new Binding("Height");

            columnDisplayUnitBinding.Source = this;
            rowDisplayUnitBinding.Source = this;

            columnDisplayUnitBinding.Converter = new BoardColumnRowValueConverter(Board.Columns);
            rowDisplayUnitBinding.Converter = new BoardColumnRowValueConverter(Board.Rows);

            SetBinding(ColumnDisplayUnitProperty, columnDisplayUnitBinding);
            SetBinding(RowDisplayUnitProperty, rowDisplayUnitBinding);

            // management of logic portion

            // need to capture RemovingPositional event emitted from Board (since BoardUI is aware of Board but not the other way around)
            Board.AddingPositional += AddingPositionalBinding;
            Board.RemovingPositional += RemovingPositionalBinding;
            Board.TempRemovingPositional += TempRemovingPositionalBinding;
            Board.RestoringPositional += RestoringPositionalBinding;
            Board.BoardFading += BoardFadeOut;

            // focus
            this.LostFocus += LostFocusBinding;
            this.GotFocus += GotFocusBinding;
        }

        private void InitializeBoard()
        {
            Board.BoardResize += BoardResizeBind;

            this.Width = Board.Columns * ColumnDisplayUnit;
            this.Height = Board.Rows * RowDisplayUnit;

            // invoke the more specified boardUI resize event when its own size changes
            SizeChanged += SizeChangedBinding;
        }

        private void AttachAllLogic()
        {
            // IMPORTANT
            // matches logic and ui type pair
            // once found the match, get the create the ui, which will wrap upon the logic

            foreach (Positional<TBoard> positional in Board.Positionals)
                AddFromLogic(positional);
        }

        private void AddingPositionalBinding(object sender, PositionalEventArgs<TBoard> e)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                AddFromLogic(e.Positional);
            });
        }

        private void RemovingPositionalBinding(object sender, PositionalEventArgs<TBoard> e)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                foreach (UIElement child in this.Children)
                {
                    // we do not bother with things that are not of type PositionalUI onwards
                    if (!(child is PositionalUI<TBoard>))
                        continue;

                    PositionalUI<TBoard> childUI = child as PositionalUI<TBoard>;

                    if (childUI.Logic.Equals(e.Positional))
                    {
                        Children.Remove(childUI);
                        break;
                    }
                }
            });

            // wow the image is removed from the BoardUI.
            // but the collidable still exists in the Board, so monkey will seem to collide with empty air. Let's remove that, shall we?
            foreach (Positional<TBoard> p in Board.CollisionManager.CollidableList)
            {
                if (p.Equals(e.Positional))
                {
                    Board.CollisionManager.CollidableList.Remove(p as Collidable<TBoard>);
                    break;
                }
            }
        }

        private void TempRemovingPositionalBinding(object sender, PositionalEventArgs<TBoard> e)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                foreach (UIElement child in this.Children)
                {
                    if (!(child is PositionalUI<TBoard>))
                        continue;

                    PositionalUI<TBoard> childUI = child as PositionalUI<TBoard>;

                    if (childUI.Logic.Equals(e.Positional))
                    {
                        childUI.Opacity = 0;
                        break;
                    }
                }
            });
        }

        private void RestoringPositionalBinding(object sender, PositionalEventArgs<TBoard> e)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                foreach (UIElement child in this.Children)
                {
                    if (!(child is PositionalUI<TBoard>))
                        continue;

                    PositionalUI<TBoard> childUI = child as PositionalUI<TBoard>;

                    if (childUI.Logic.Equals(e.Positional))
                    {
                        DoubleAnimation animateOpacity = new DoubleAnimation();
                        animateOpacity.From = 0;
                        animateOpacity.To = 1;
                        animateOpacity.Duration = TimeSpan.Parse("0:0:2");
                        animateOpacity.BeginTime = TimeSpan.Parse("0:0:0.75");
                        childUI.BeginAnimation(OpacityProperty, animateOpacity);

                        break;
                    }
                }
            });
        }

        protected void BoardFadeOut(object sender, EventArgs e)
        {
            DoubleAnimation animateOpacity = new DoubleAnimation();
            TimeSpan timeSpan = TimeSpan.Parse("0:0:0.5");

            animateOpacity.From = 1;
            animateOpacity.To = 0;
            animateOpacity.Duration = timeSpan;

            //bool animationLock = true;

            //animateOpacity.Completed += (object animationSender, EventArgs animationArgs) => animationLock = false;
            this.BeginAnimation(OpacityProperty, animateOpacity, HandoffBehavior.Compose);
            Board.Pause();

            //while (animationLock) ;
        }

        protected void BoardFadeIn(object sender, EventArgs e)
        {
            DoubleAnimation animateOpacity = new DoubleAnimation();
            TimeSpan timeSpan = TimeSpan.Parse("0:0:0.5");

            animateOpacity.From = 0;
            animateOpacity.To = 1;
            animateOpacity.Duration = timeSpan;

            this.BeginAnimation(OpacityProperty, animateOpacity, HandoffBehavior.Compose);
        }

        protected virtual void OnBoardUIResize(BoardUIResizeEventArgs e)
        {
            if (BoardUIResize != null)
                BoardUIResize(this, e);
        }

        protected virtual void InitializeLogicUISetup()
        {
            // do nothing, meant for overriding
            // alternative way of using this is to pass the initialization method into the #ctor
        }
        
        internal virtual void BoardResizeBind(object sender, BoardResizeEventArgs e)
        {
            Width = e.Width * ColumnDisplayUnit;
            Height = e.Height * RowDisplayUnit;
        }

        internal protected virtual void AddFromLogic(Positional<TBoard> positional)
        {
            // no need to call add
            // the convert method automatically adds the UI to the boardUI
            LogicUICreatorManager.Convert(positional);
        }

        #endregion

        #region Constructors

        public BoardUI(TBoard board, double columnDisplayUnit, double rowDisplayUnit, Action initialization = null)
        {
            if (initialization != null)
                initialization();

            InitializeLogicUISetup();
            InitializeBackgroundImage();
            
            ColumnDisplayUnit = columnDisplayUnit;
            RowDisplayUnit = rowDisplayUnit;

            Board = board;
        }

        public BoardUI(TBoard board, double displayUnit, Action initialization = null)
            : this(board, displayUnit, displayUnit, initialization)
        {
        }

        #endregion
    }

    public class BoardColumnRowValueConverter : IValueConverter
    {
        #region Data Fields

        private int divisor;

        #endregion

        #region Properties

        public int Divisor
        {
            get { return divisor; }
            protected set { divisor = value; }
        }

        #endregion

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value) / Divisor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value * Divisor);
        }

        #endregion

        #region Constructors

        public BoardColumnRowValueConverter(int divisor)
        {
            Divisor = divisor;
        }

        #endregion
    }

    public class BoardUIResizeEventArgs : EventArgs
    {
        #region Data Fields

        private Size displaySize;
        private int boardColumns;
        private int boardRows;

        #endregion

        #region Properties

        public Size DisplaySize
        {
            get { return displaySize; }
            protected set { displaySize = value; }
        }

        public int BoardColumns
        {
            get { return boardColumns; }
            protected set { boardColumns = value; }
        }

        public int BoardRows
        {
            get { return boardRows; }
            protected set { boardRows = value; }
        }

        #endregion

        #region Constructors

        public BoardUIResizeEventArgs(Size displaySize, int boardColumns, int boardRows)
        {
            DisplaySize = displaySize;
            BoardColumns = boardColumns;
            BoardRows = boardRows;
        }

        #endregion
    }
}