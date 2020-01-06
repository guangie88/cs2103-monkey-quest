// Main Contributors: Weiguang, Div

using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Input;

using Multimedia;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Board<TBoard> : DependencyObject
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private int columns;
        private int rows;

        private int finenessLimit;
        private int framesPerSecond;

        private Timer timer;

        private IList<Positional<TBoard>> positionals;
        private CollisionManager<TBoard> collisionManager;

        private ISet<Key> keyPressedSet = new SortedSet<Key>();

        private static readonly ISet<Key> DefaultKeySet = new SortedSet<Key>(new Key[] { Key.LeftShift, Key.LeftCtrl, Key.LeftAlt, Key.Left, Key.Right, Key.Up, Key.Down, Key.Space });

        // const values
        public const int MinColumns = 4;
        public const int MinRows = 3;

        public const int MaxColumns = 32;
        public const int MaxRows = 18;

        #endregion

        #region Properties

        protected ISet<Key> KeyPressedSet
        {
            get { return keyPressedSet; }
            set { keyPressedSet = new SortedSet<Key>(value); }
        }
        
        internal protected IList<Positional<TBoard>> Positionals
        {
            get { return positionals; }
            protected set { positionals = value; }
        }

        public CollisionManager<TBoard> CollisionManager
        {
            get { return collisionManager; }
            protected set { collisionManager = value; }
        }

        public int Columns
        {
            get { return columns; }
            set
            {
                SetUpColumns(value);
                OnBoardResize(new BoardResizeEventArgs(Columns, Rows));
            }
        }

        public int Rows
        {
            get { return rows; }
            set
            {
                SetUpRows(value);
                OnBoardResize(new BoardResizeEventArgs(Columns, Rows));
            }
        }

        public int FinenessLimit
        {
            get { return finenessLimit; }
            protected set { finenessLimit = value; }
        }

        internal protected Timer Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            protected set
            {
                framesPerSecond = value;
                timer.Period = 1000 / framesPerSecond;
            }
        }

        #endregion

        #region Events

        public event BoardResizeEventHandler BoardResize;
        public event BoardTickEventHandler BoardTick;
        public event BoardKeyEventHandler<TBoard> BoardKeyDown;
        public event AddingPositionalEventHandler<TBoard> AddingPositional;
        public event RemovingPositionalEventHandler<TBoard> RemovingPositional;
        public event TempRemovingPositionalEventHandler<TBoard> TempRemovingPositional;
        public event RestoringPositionalEventHandler<TBoard> RestoringPositional;
        public event BoardFadeEventHandler<TBoard> BoardFading;

        #endregion

        #region Methods

        private void CaptureKeyTrigger(object sender, EventArgs e)
        {
            // a need to be so complicated because of threading issues
            // timer is not sync with everything else basically

            Dispatcher.Invoke((Action)(() =>
            {
                OnBoardKeyDown(new BoardKeyEventArgs<TBoard>(this, KeyPressedSet));
                KeyPressedSet.Clear();

                foreach (Key key in DefaultKeySet) 
                    if (Keyboard.IsKeyDown(key))
                        KeyPressedSet.Add(key);
            }));
        }

        private void CaptureTick(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                OnBoardTick(new EventArgs());
            }));
        }

        private void SetUpColumns(int columns)
        {
            if (columns < MinColumns || columns > MaxColumns)
                throw new BoardInvalidColumnsRowsException(columns, Rows, MaxColumns, MaxRows);

            this.columns = columns;
        }

        private void SetUpRows(int rows)
        {
            if (rows < MinRows || rows > MaxRows)
                throw new BoardInvalidColumnsRowsException(Columns, rows, MaxColumns, MaxRows);

            this.rows = rows;
        }

        protected virtual void OnBoardResize(BoardResizeEventArgs e)
        {
            if (BoardResize != null)
                BoardResize(this, e);
        }

        protected virtual void OnBoardTick(EventArgs e)
        {
            if (BoardTick != null)
                BoardTick(this, e);
        }

        protected virtual void OnBoardKeyDown(BoardKeyEventArgs<TBoard> e)
        {
            if (BoardKeyDown != null)
                BoardKeyDown(this, e);
        }

        protected virtual void OnAddingPositional(PositionalEventArgs<TBoard> e)
        {
            if (AddingPositional != null)
                AddingPositional(this, e);
        }

        protected virtual void OnRemovingPositional(PositionalEventArgs<TBoard> e)
        {
            if (RemovingPositional != null)
                RemovingPositional(this, e);
        }

        public void AddPositional(Positional<TBoard> positional)
        {
            // disallow repeated adding
            if (Positionals.Contains(positional))
                return;

            // if it is from another board, we remove it from there first,
            // before adding to the current one.
            if (positional.Board != null)
                positional.Board.RemovePositional(positional);

            positional.SetBoardDirectly(this as TBoard);
            Positionals.Add(positional);

            Position position = positional.Position;
            // very important to reset the position with the new finenessLimit value
            positional.Position = new Position(position.X, position.Y, position.FinenessX, position.FinenessY, this.FinenessLimit);

            OnAddingPositional(new PositionalEventArgs<TBoard>(positional));
        }

        public bool RemovePositional(Positional<TBoard> positional)
        {
            // if it is not supposed to be from this board,
            // just return false and do nothing.
            if (!Positionals.Contains(positional))
                return false;

            // by setting the positional.Board to null
            // all the bindings from logic to board will be removed
            // this is because positional.Board is made to be virtual
            positional.Board = null;

            Positionals.Remove(positional);
            OnRemovingPositional(new PositionalEventArgs<TBoard>(positional));

            return true;
        }

        public void TempRemovePositional(Positional<TBoard> positional)
        {
            TempRemovingPositional(this, new PositionalEventArgs<TBoard>(positional));
        }

        public void RestorePositional(Positional<TBoard> positional)
        {
            RestoringPositional(this, new PositionalEventArgs<TBoard>(positional));
        }

        public void OnBoardFadeOut(Positional<TBoard> positional)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (BoardFading != null)
                    BoardFading(this, new PositionalEventArgs<TBoard>(positional));
            }));
        }

        public int CalculateFineness(double proportion)
        {
            return Values.CalculateFineness(proportion, FinenessLimit);
        }

        // starts the timer
        public void Start()
        {
            Timer.Start();
        }

        // pauses the timer
        public void Pause()
        {
            Timer.Stop();
        }

        // clears the entire board and removes all logic bindings
        public virtual void Clear()
        {
            while (Positionals.Count > 0)
                RemovePositional(Positionals[0]);
        }

        #endregion

        #region Constructors

        // default finenessLimit is 5000
        public Board(int columns, int rows, int finenessLimit = Values.FinenessLimit, int framesPerSecond = Values.FramesPerSecond)
        {
            FinenessLimit = finenessLimit;

            SetUpColumns(columns);
            SetUpRows(rows);

            Timer = new Timer();
            Timer.Tick += CaptureTick;
            // capturing and retransmitting of keyboard key capturing event
            Timer.Tick += CaptureKeyTrigger;

            // setting the fps will auto set the period of timer
            FramesPerSecond = framesPerSecond;

            Positionals = new List<Positional<TBoard>>();
            CollisionManager = new CollisionManager<TBoard>();
        }

        #endregion
    }

    public class BoardResizeEventArgs : EventArgs
    {
        #region Properties

        public int Width
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        #endregion

        #region Constructors

        public BoardResizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion
    }

    public class PositionalEventArgs<TBoard> : EventArgs
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        Positional<TBoard> positional;

        #endregion

        #region Properties

        public Positional<TBoard> Positional
        {
            get { return positional; }
            protected set { positional = value; }
        }

        #endregion

        #region Constructors

        public PositionalEventArgs(Positional<TBoard> positional)
        {
            Positional = positional;
        }

        #endregion
    }

    public class BoardKeyEventArgs<TBoard> : EventArgs
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Board<TBoard> board;
        private ISet<Key> alreadyDownKeys;

        #endregion

        #region Properties

        public ISet<Key> AlreadyDownKeys
        {
            get { return alreadyDownKeys; }
            protected set { alreadyDownKeys = value; }
        }

        public Board<TBoard> Board
        {
            get { return board; }
            protected set { board = value; }
        }

        #endregion

        #region Methods

        public bool IsKeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }

        public bool IsKeyPressedOnce(Key key)
        {
            return !AlreadyDownKeys.Contains(key) && Keyboard.IsKeyDown(key);
        }

        #endregion

        #region Constructors

        public BoardKeyEventArgs(Board<TBoard> board, ISet<Key> alreadyDownKeys)
        {
            Board = board;
            AlreadyDownKeys = alreadyDownKeys;
        }

        #endregion
    }
}