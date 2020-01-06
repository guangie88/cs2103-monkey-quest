// Main Contributors: Frank

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace MonkeyQuest.Framework.Images
{
    public class AnimatedImage : Image
    {
        #region Data Fields

        private int currentFrame;
        private int framesPerSecond;
        private readonly List<ImageFrames> imageFramesList;
        private int currentSlide = 0;
        private int framesLeftInCurrentSlide = 0;

        private int framesCount;

        #endregion

        #region Properties

        public int CurrentFrame
        {
            get { return currentFrame; }
            protected set { 
                currentFrame = value;
                Tween();
            }
        }

        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            protected set { framesPerSecond = value; }
        }

        public IList<ImageFrames> ImageFramesList
        {
            get { return imageFramesList; }
        }

        public int FramesCount
        {
            get { return framesCount; }
            protected set { framesCount = value; }
        }

        #endregion

        #region Methods

        protected void Tween()
        {
            if (framesLeftInCurrentSlide-- == 0)
            {
                currentSlide = currentFrame % imageFramesList.Count;

                // a need to be so complicated because of threading issues
                Dispatcher.Invoke((Action)delegate()
                { Source = imageFramesList[currentSlide].Image.Source; });

                framesLeftInCurrentSlide = imageFramesList[currentSlide].FramesDelay;
            }
        }

        public void NextImage()
        {
            Dispatcher.Invoke((Action)delegate()
            { Source = imageFramesList[++currentSlide].Image.Source; });
        }

        private void AddToTotalFrames(ImageFrames imageFrames)
        {
            FramesCount += imageFrames.FramesDelay;
        }

        private void RemoveFromTotalFrames(ImageFrames imageFrames)
        {
            FramesCount -= imageFrames.FramesDelay;
        }

        public void IncrementFrame()
        {
            // loops through the frames without exceeding the maximum frames count
            if(FramesCount!=0)
                CurrentFrame = (CurrentFrame + 1) % FramesCount;
        }

        public void Add(ImageFrames imageFrames)
        {
            ImageFramesList.Add(imageFrames);
            AddToTotalFrames(imageFrames);
        }

        public void Remove(ImageFrames imageFrames)
        {
            if (ImageFramesList.Remove(imageFrames))
                RemoveFromTotalFrames(imageFrames);
        }

        #endregion

        #region Constructor

        public AnimatedImage()
        {
            imageFramesList = new List<ImageFrames>();
        }

        public AnimatedImage(IEnumerable<ImageFrames> imageFramesEnumerable)
        {
            imageFramesList = new List<ImageFrames>(imageFramesEnumerable);

            foreach (ImageFrames imageFrames in imageFramesList)
                AddToTotalFrames(imageFrames);

            CurrentFrame = 0;
        }

        #endregion
    }

    public class ImageFrames
    {
        #region Data Fields

        private Image image;
        private int framesDelay;

        private AnimatedImage parent;

        #endregion

        #region Properties

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        // disabled set to prevent user from directly changing the framesDelay
        // when the ImageFrame object is in AnimatedImage.
        // Will pose a problem to the calculation of number of frames in AnimatedImage.
        public int FramesDelay
        {
            get { return framesDelay; }
            protected set { framesDelay = value; }
        }

        internal AnimatedImage Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public ImageFrames(Image image, int framesDelay)
        {
            Image = image;
            FramesDelay = framesDelay;
        }

        #endregion
    }

    public class MovableAnimatedImage : AnimatedImage
    {
        #region Data Fields

        private readonly List<ImageFrames> imageFramesList;


        #endregion

        #region Properties

        new public IList<ImageFrames> ImageFramesList
        {
            get { return imageFramesList; }
        }

        #endregion

        #region Methods

        public void Left()
        {
            if (imageFramesList.Count > 1)
                Dispatcher.Invoke((Action)(() => Source = imageFramesList[1].Image.Source));
        }

        public void Right()
        {
            Dispatcher.Invoke((Action)(() => Source = imageFramesList[0].Image.Source));
        }

        #endregion

        #region Constructor

        public MovableAnimatedImage()
        {
            imageFramesList = new List<ImageFrames>();
        }

        public MovableAnimatedImage(IEnumerable<ImageFrames> imageFramesEnumerable)
        {
            imageFramesList = new List<ImageFrames>(imageFramesEnumerable);
            Right();
        }

        #endregion
    }

}
