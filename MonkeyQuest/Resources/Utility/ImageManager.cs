using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Reflection;

namespace MonkeyQuest.Resources.Utility
{
    public static class ImageManager
    {
        #region Data Fields

        private const string TILE_ID = "tile";

        private static SortedDictionary<string, BitmapImage> elementImageDictionary = new SortedDictionary<string,BitmapImage>();
        private static SortedDictionary<string, BitmapImage> tileImageDictionary = new SortedDictionary<string, BitmapImage>();

        #endregion

        #region Properties

        public static SortedDictionary<string, BitmapImage> ElementImageDictionary
        {
            get { return elementImageDictionary; }
            private set { elementImageDictionary = value; }
        }

        public static SortedDictionary<string, BitmapImage> TileImageDictionary
        {
            get { return tileImageDictionary; }
            private set { tileImageDictionary = value; }
        }

        #endregion

        #region Methods

        private static void InitializeImages()
        {
            // finding all the properties (name + types) using reflections
            // categorizes the images into elements and tiles

            Type resourcesType = typeof(Properties.Resources);
            PropertyInfo[] resourcesProperties = resourcesType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic);

            foreach (PropertyInfo resourcesProperty in resourcesProperties)
                if (resourcesProperty.PropertyType.Equals(typeof(Bitmap)))
                {
                    BitmapImage bitmapImage = BitmapToBitmapImage(resourcesProperty.GetValue(typeof(Properties.Resources), null) as Bitmap);

                    if (resourcesProperty.Name.ToLower().Contains(TILE_ID))
                    {
                        TileImageDictionary.Add(resourcesProperty.Name, bitmapImage);
                        System.Diagnostics.Debug.WriteLine(resourcesProperty.Name + " image put into tile dictionary.");
                    }
                    else
                    {
                        ElementImageDictionary.Add(resourcesProperty.Name, bitmapImage);
                        System.Diagnostics.Debug.WriteLine(resourcesProperty.Name + " image put into element dictionary.");
                    }
                }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            Stream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        #endregion

        #region Constructors

        static ImageManager()
        {
            InitializeImages();
        }

        #endregion
    }
}
