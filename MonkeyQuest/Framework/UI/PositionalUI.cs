// Main Contributors: Weiguang, Frank

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.UI
{
    public abstract class PositionalUI<TBoard> : Image
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Positional<TBoard> logic;
        private BoardUI<TBoard> boardUI;
        private Image imageToDisplay;

        #endregion

        #region Properties

        public int ZIndex
        {
            get { return (int)GetValue(Panel.ZIndexProperty); }
            set { SetValue(Panel.ZIndexProperty, value); }
        }

        public Positional<TBoard> Logic
        {
            get { return logic; }
            protected set { logic = value; }
        }

        public BoardUI<TBoard> BoardUI
        {
            get { return boardUI; }
            internal protected set { AddToBoardUI(value); }
        }

        public Image ImageToDisplay
        {
            get { return imageToDisplay; }
            protected set { imageToDisplay = value; }
        }

        #endregion

        #region Methods

        // perform the initial binding to bind this.Source to ImageToDisplay.Source
        private void InitializeBindings()
        {
            Binding sourceBinding = new Binding("Source");
            sourceBinding.Source = ImageToDisplay;
            this.SetBinding(SourceProperty, sourceBinding);
        }

        // perform only relocation of the UI object
        private void Relocate()
        {
            // RELOCATION
            Position position = Logic.Position;

            // get the big squares first
            double displayX = position.X * BoardUI.ColumnDisplayUnit;
            double displayY = position.Y * BoardUI.RowDisplayUnit;

            // assume n is the fineness limit
            // hence we have -n to n and x, where x is a value in between the range
            // by taking x over 2n, we have the proportion on where the fineness position is
            // so multiply that by the display unit to get the actual screen displacement
            // no need to cast double everywhere, so as long one portion is double, the rest
            // will be implicitly converted to double
            double finenessDisplayX = ((double)position.FinenessX) / (position.FinenessLimit * 2) * BoardUI.ColumnDisplayUnit;
            double finenessDisplayY = ((double)position.FinenessY) / (position.FinenessLimit * 2) * BoardUI.RowDisplayUnit;

            // sets the image in position
            // needs to check for NaN due to some issue with the initial bindings
            // it attempts to bind and set the margin even before PositionalUI was given a Position
            // resulting in NaN
            if (!displayX.Equals(double.NaN) && !displayY.Equals(double.NaN))
                this.Margin = new Thickness(displayX + finenessDisplayX, displayY + finenessDisplayY, 0, 0);
        }

        // adds to boardUI and then perform the required bindings
        protected internal virtual void AddToBoardUI(BoardUI<TBoard> boardUI)
        {
            // automatically adds to the boardUI and do the necessary event bindings
            // defensive programming required since I used default parameters

            if (BoardUI != null)
                RemoveFromBoardUI();

            SetBoardUIDirectly(boardUI);

            if (BoardUI != null)
            {
                BoardUI.Children.Add(this);

                // set the position and size of the UI object right at the start
                ResizingRelocationBind(this, new BoardUIResizeEventArgs(new Size(BoardUI.ActualWidth, BoardUI.ActualHeight), BoardUI.Board.Columns, BoardUI.Board.Rows));

                // binding of display size change
                BoardUI.BoardUIResize += ResizingRelocationBind;

                // IMPORTANT
                // binding of movement
                Logic.PositionChanged += PositionBind;
            }
        }

        // removes UI object from the board
        protected internal virtual void RemoveFromBoardUI()
        {
            if (BoardUI != null)
                BoardUI.Children.Remove(this);

            BoardUI.BoardUIResize -= ResizingRelocationBind;
            Logic.PositionChanged -= PositionBind;
        }

        // do not attempt to use this if possible
        protected internal void SetBoardUIDirectly(BoardUI<TBoard> boardUI)
        {
            this.boardUI = boardUI;
        }

        // perform necessary shifting of image when positional object moves
        protected internal void PositionBind(object sender, PositionsEventArgs e)
        {
            // relocates the UI object when it moves
            Dispatcher.Invoke((Action)Relocate);
        }

        // perform necessary resize and relocation of this particular instance
        protected internal void ResizingRelocationBind(object sender, BoardUIResizeEventArgs e)
        {
            // IMPORTANT

            // RESIZING
            this.Width = BoardUI.ColumnDisplayUnit;
            this.Height = BoardUI.RowDisplayUnit;
            
            // RELOCATION
            Relocate();
        }

        #endregion

        #region Constructors

        protected internal PositionalUI(Positional<TBoard> positional, Image imageToDisplay, BoardUI<TBoard> boardUI = null)
        {
            // set the resizing mode to high quality so that the UI looks nicer
            SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
            // set stretch mode to fill
            Stretch = System.Windows.Media.Stretch.Fill;

            Logic = positional;
            ImageToDisplay = imageToDisplay;

            BoardUI = boardUI;

            // set this.Source to bind to ImageToDisplay.Source
            InitializeBindings();
        }

        #endregion
    }

    public abstract class PositionalUI<TBoard, TPositional, TImage> : PositionalUI<TBoard>
        where TBoard : Board<TBoard>
        where TPositional : Positional<TBoard>
        where TImage : Image
    {
        #region Data Fields

        #endregion

        #region Properties

        public new TPositional Logic
        {
            get { return base.Logic as TPositional; }
            protected set { base.Logic = value; }
        }

        public new TImage ImageToDisplay
        {
            get { return base.ImageToDisplay as TImage; }
            protected set { base.ImageToDisplay = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        protected internal PositionalUI(TPositional positional, TImage imageToDisplay, BoardUI<TBoard> boardUI = null) : base(positional, imageToDisplay, boardUI)
        {
        }

        #endregion
    }
}
