// Main Contributors: Frank

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.Framework.Images
{
    public class ControllableAnimatedImage : AnimatedImage
    {
        #region Data Fields

        private readonly List<ControllableImageFrames> imageFramesList;
        private Image staticImage;
        
        #endregion

        #region Properties

        Image StaticImage
        {
            get { return staticImage; }
            set { staticImage = value; }
        }

        #endregion

        #region Methods

        public void KeyCheck<TBoard>(BoardKeyEventArgs<TBoard> e)
            where TBoard : Board<TBoard>
        {
            foreach (ControllableImageFrames frame in imageFramesList)
            {
                if (e.IsKeyDown(frame.Keyy))
                {
                    Dispatcher.Invoke((Action)delegate()
                    { Source = frame.Image.Source; });

                    break;
                }
                else
                {
                    Dispatcher.Invoke((Action)delegate()
                    { Source = StaticImage.Source; });
                }
            }
        }

        #endregion

        #region Constructors
        public ControllableAnimatedImage(IEnumerable<ControllableImageFrames> imageFramesEnumerable, Image staticImage)
        {
            imageFramesList = new List<ControllableImageFrames>(imageFramesEnumerable);

            StaticImage = staticImage;

            Source = staticImage.Source;
        }
        #endregion
    }

    public class ControllableImageFrames
    {
        #region Data Fields

        private Image image;
        private Key keyy;
        private AnimatedImage parent;

        #endregion

        #region Properties

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        internal AnimatedImage Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Key Keyy
        {
            get { return keyy; }
            set { keyy = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public ControllableImageFrames(Image image, Key key)
        {
            Image = image;
            Keyy = key;
        }

        #endregion
    }
}
