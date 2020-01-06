// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    #region Board exceptions

    public class BoardProportionException : Exception
    {
        #region Data Fields

        private double proportion;

        #endregion

        #region Properties

        public double Proportion
        {
            get { return proportion; }
            protected set { proportion = value; }
        }

        #endregion

        #region Constructors

        public BoardProportionException(double proportion)
        {
            Proportion = proportion;
        }

        #endregion
    }

    public class BoardInvalidColumnsRowsException : Exception
    {
        #region Data Fields

        private int columns;
        private int rows;

        public readonly int MaxColumns;
        public readonly int MaxRows;

        #endregion

        #region Properties

        public int Columns
        {
            get { return columns; }
            protected set { columns = value; }
        }

        public int Rows
        {
            get { return rows; }
            protected set { rows = value; }
        }

        #endregion

        #region Constructors

        public BoardInvalidColumnsRowsException(int columns, int rows, int maxColumns, int maxRows)
        {
            Columns = columns;
            Rows = rows;
            MaxColumns = maxColumns;
            MaxRows = maxRows;
        }

        #endregion
    }

    #endregion

    #region Position exceptions

    public class PositionFinenessExceedLimitException : Exception
    {
        #region Data Fields

        private int fineness;
        private int finenessLimit;

        #endregion

        #region Properties

        public int Fineness
        {
            get { return fineness; }
            protected set { fineness = value; }
        }

        public int FinenessLimit
        {
            get { return finenessLimit; }
            protected set { finenessLimit = value; }
        }

        #endregion

        #region Constructors

        public PositionFinenessExceedLimitException(int fineness, int finenessLimit)
        {
            Fineness = fineness;
            FinenessLimit = finenessLimit;
        }

        #endregion
    }

    public class PositionFinenessConflictException : Exception
    {
        #region Data Fields

        private int boardFinenessLimit;
        private int positionFinenessLimit;

        #endregion

        #region Properties

        public int BoardFinenessLimit
        {
            get { return boardFinenessLimit; }
            protected set { boardFinenessLimit = value; }
        }

        public int PositionFinenessLimit
        {
            get { return positionFinenessLimit; }
            protected set { positionFinenessLimit = value; }
        }

        #endregion

        #region Constructors

        public PositionFinenessConflictException(int boardFinenessLimit, int positionFinenessLimit)
        {
            BoardFinenessLimit = boardFinenessLimit;
            PositionFinenessLimit = positionFinenessLimit;
        }

        #endregion
    }

    #endregion
}