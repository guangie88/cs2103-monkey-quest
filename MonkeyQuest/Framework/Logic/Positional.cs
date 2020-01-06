// Main Contributors: Weiguang

using System;
using System.Reflection;
using System.Windows.Threading;

using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Logic
{
    public abstract class Positional<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Position position;
        private TBoard board;

        private bool isActiveOnBoard = false;

        #endregion

        #region Properties

        // determines if the positional is considered to be active on board
        // this is to prevent initial collision due to late setting of position
        public bool IsActiveOnBoard
        {
            get { return isActiveOnBoard; }
            protected set { isActiveOnBoard = value; }
        }

        // returns an identifier for every instance of the class for conversion from logic to UI.
        public virtual string LogicIdentifier
        {
            get { return GetType().ToString(); }
        }

        // No worries about user attempting to change X, Y or Fineness through the accessor method of Position
        // in order to bypass the change of Position event.
        // This is because Position is a struct and when returned, a duplicated value is returned on behalf,
        // and attempting to change X, Y or Fineness in it will cause an error.
        public Position Position
        {
            get { return position; }
            set { SetPosition(value); }
        }

        public int X
        {
            get { return Position.X; }
            set { SetPosition(value, Y, FinenessX, FinenessY); }
        }

        public int Y
        {
            get { return Position.Y; }
            set { SetPosition(X, value, FinenessX, FinenessY); }
        }

        public int FinenessX
        {
            get { return Position.FinenessX; }
            set { SetPosition(X, Y, value, FinenessY); }
        }

        public int FinenessY
        {
            get { return Position.FinenessY; }
            set { SetPosition(X, Y, FinenessX, value); }
        }

        public virtual TBoard Board
        {
            get { return board; }
            protected internal set
            {
                // removes the positional directly
                if (Board != null)
                {
                    RemoveBindings();
                    Board.Positionals.Remove(this);
                }

                board = value;

                if (value != null)
                {
                    Board.AddPositional(this);
                    InitializeBindings();
                }
            }
        }

        #endregion

        #region Events

        public event PositionChangedEventHandler<TBoard> PositionChanged;

        #endregion

        #region Methods

        protected virtual void InitializeBindings()
        {
            // nothing to bind here
            // meant for overriding purposes
        }

        protected virtual void RemoveBindings()
        {
            // nothing to bind here
            // meant for overriding purposes
        }

        protected internal void SetBoardDirectly(TBoard board)
        {
            this.board = board;
        }

        protected virtual void OnPositionChanged(PositionsEventArgs e)
        {
            // only activates when a move is legal and the position really changed
            if (PositionChanged != null)
                PositionChanged(this, e);
        }

        // do not attempt to use this unless absolutely necessary
        protected void SetPositionDirectly(Position nextPosition)
        {
            position = nextPosition;
        }

        public Positional<TBoard> SetBoard(TBoard board)
        {
            Board = board;
            return this;
        }

        public bool SetPosition(int x, int y, int finenessX = 0, int finenessY = 0)
        {
            if (Board == null)
                return SetPosition(new Position(x, y, finenessX, finenessY));

            return SetPosition(new Position(x, y, finenessX, finenessY, Board.FinenessLimit));
        }

        // this method will be overriden over at Collidable
        // at this point, Positional allows the position to be set at anywhere
        // except position that is out of the board
        public virtual bool SetPosition(Position nextPosition)
        {
            if (Board != null)
            {
                // check for conflicting fineness limit
                if (nextPosition.FinenessLimit != Board.FinenessLimit)
                    throw new PositionFinenessConflictException(Board.FinenessLimit, nextPosition.FinenessLimit);

                // checking for board boundary
                // disallow things to fly out of screen
                if (nextPosition.X < 0 || nextPosition.X >= Board.Columns || nextPosition.Y < 0 || nextPosition.Y >= Board.Rows)
                    return false;
            }

            Position currentPosition = Position;

            // using position instead of Position because setter of Position is reliant on this SetPosition method itself
            // trying to use Position will cause infinite loop
            position = nextPosition;
            IsActiveOnBoard = true;
            OnPositionChanged(new PositionsEventArgs(currentPosition, nextPosition));

            // position is accepted
            return true;
        }

        #endregion

        #region Constructors

        // set finenessLimit to default (Constants.FinenessLimit) first because board may be null
        // add the finenessLimit only when board.AddPositional is called
        public Positional(Position position = default(Position))
        {
            SetPositionDirectly(position);
        }

        #endregion
    }

    public class PositionsEventArgs : EventArgs
    {
        #region Data Fields

        private Position currentPosition;
        private Position nextPosition;

        #endregion

        #region Properties

        public Position CurrentPosition
        {
            get { return currentPosition; }
            protected set { currentPosition = value; }
        }

        public Position NextPosition
        {
            get { return nextPosition; }
            protected set { nextPosition = value; }
        }

        public int CurrentX
        {
            get { return CurrentPosition.X; }
        }

        public int CurrentY
        {
            get { return CurrentPosition.Y; }
        }

        public int CurrentFinenessX
        {
            get { return CurrentPosition.FinenessX; }
        }

        public int CurrentFinenessY
        {
            get { return CurrentPosition.FinenessY; }
        }

        public int NextX
        {
            get { return NextPosition.X; }
        }

        public int NextY
        {
            get { return NextPosition.Y; }
        }

        public int NextFinenessX
        {
            get { return NextPosition.FinenessX; }
        }

        public int NextFinenessY
        {
            get { return NextPosition.FinenessY; }
        }

        #endregion

        #region Constructors

        public PositionsEventArgs(int currentX, int currentY, int nextX, int nextY)
            : this(currentX, currentY, 0, 0, nextX, nextY, 0, 0)
        {
        }

        public PositionsEventArgs(int currentX, int currentY, int currentFinenessX, int currentFinenessY, int nextX, int nextY, int nextFinenessX, int nextFinenessY)
            : this(new Position(currentX, currentY, currentFinenessX, currentFinenessY), new Position(nextX, nextY, nextFinenessX, nextFinenessY))
        {
        }

        public PositionsEventArgs(Position currentPosition, Position nextPosition)
        {
            CurrentPosition = currentPosition;
            NextPosition = nextPosition;
        }

        #endregion
    }
}