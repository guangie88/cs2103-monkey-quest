using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.UI
{
    public class MainBarUI<TBoard> : Canvas
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private Board<TBoard> board;
        private Image backgroundImage;

        #endregion

        #region Properties

        public virtual TBoard Board
        {
            get { return board as TBoard; }
            set
            {
                if (Board != null)
                    RemoveBindings();

                board = value;

                if (Board != null)
                    InitializeBindings();
            }
        }

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        #endregion

        #region Methods

        private void InitializeBackgroundImage()
        {
            BackgroundImage = new Image();
            SetValue(Canvas.ZIndexProperty, -1);
            this.Children.Add(BackgroundImage);
        }

        protected virtual void RemoveBindings()
        {
            // nothing to remove
        }

        protected virtual void InitializeBindings()
        {
            // nothing to bind
        }
        
        #endregion

        #region Constructors

        public MainBarUI(TBoard board)
        {
            InitializeBackgroundImage();
            Board = board;
        }

        #endregion
    }
}