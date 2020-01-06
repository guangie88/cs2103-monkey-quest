// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public struct RectangularMask
    {
        #region Data Fields

        private double leftMarginProportion;
        private double rightMarginProportion;
        private double topMarginProportion;
        private double bottomMarginProportion;
        private int finenessLimit;

        #endregion

        #region Properties

        public double LeftMarginProportion
        {
            get { return leftMarginProportion; }
            set { leftMarginProportion = value; }
        }

        public double RightMarginProportion
        {
            get { return rightMarginProportion; }
            set { rightMarginProportion = value; }
        }

        public double TopMarginProportion
        {
            get { return topMarginProportion; }
            set { topMarginProportion = value; }
        }

        public double BottomMarginProportion
        {
            get { return bottomMarginProportion; }
            set { bottomMarginProportion = value; }
        }
        
        public int FinenessLimit
        {
            get { return finenessLimit; }
            set { finenessLimit = value; }
        }

        public int LeftMargin
        {
            get { return Values.CalculateFineness(LeftMarginProportion, FinenessLimit); }
        }

        public int RightMargin
        {
            get { return Values.CalculateFineness(RightMarginProportion, FinenessLimit); }
        }

        public int TopMargin
        {
            get { return Values.CalculateFineness(TopMarginProportion, FinenessLimit); }
        }

        public int BottomMargin
        {
            get { return Values.CalculateFineness(BottomMarginProportion, FinenessLimit); }
        }

        #endregion

        #region Constructors

        // note that all the margins uses proportion, not discrete fineness
        // conversion that be done via using Board.CalculateFineness(double proportion)
        public RectangularMask(double leftMarginProportion, double rightMarginProportion, double topMarginProportion, double bottomMarginProportion, int finenessLimit = Values.FinenessLimit)
        {
            this.leftMarginProportion = leftMarginProportion;
            this.rightMarginProportion = rightMarginProportion;
            this.topMarginProportion = topMarginProportion;
            this.bottomMarginProportion = bottomMarginProportion;
            this.finenessLimit = finenessLimit;
        }

        #endregion
    }
}
