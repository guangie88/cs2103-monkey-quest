// Main Contributors: Weiguang, Frank

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

using MonkeyQuest.Resources.Utility;

namespace MonkeyQuest.Resources
{
    public static class Images
    {
        #region Data Fields

        // non-tiles (elements)
        /*public MudTile(int code) {
            ResourceManager rm = new ResourceManager("rmc", this.GetType().Assembly);

            Bitmap bitmap = new Bitmap((System.Drawing.Image) rm.GetObject(identifier));
            MudTile = new BitmapImage();
            MudTile.BeginInit();
            MudTile.StreamSource = stream;
            MudTile.EndInit();

            //Bitmap bmp = (Bitmap)Image.FromFile("Images/GrassTile.png");*/

        public static readonly BitmapImage Monster = ImageManager.ElementImageDictionary["Monster"];
        public static readonly BitmapImage Goomba1 = ImageManager.ElementImageDictionary["Goomba1"];
        public static readonly BitmapImage Goomba2 = ImageManager.ElementImageDictionary["Goomba2"];
        public static readonly BitmapImage Ghost1 = ImageManager.ElementImageDictionary["Ghost1"];
        public static readonly BitmapImage Ghost2 = ImageManager.ElementImageDictionary["Ghost2"];


        public static readonly BitmapImage NatureForest = ImageManager.ElementImageDictionary["NatureForest"];
        public static readonly BitmapImage Star = ImageManager.ElementImageDictionary["Star"];
        public static readonly BitmapImage BlueMoon = ImageManager.ElementImageDictionary["BlueMoon"];
        public static readonly BitmapImage MarioFlying = ImageManager.ElementImageDictionary["MarioFlying"];
        public static readonly BitmapImage MarioStar = ImageManager.ElementImageDictionary["MarioStar"];
        public static readonly BitmapImage MarioBlock = ImageManager.ElementImageDictionary["MarioBlock"];

        public static readonly BitmapImage Heart = ImageManager.ElementImageDictionary["Heart"];
        public static readonly BitmapImage Banana1 = ImageManager.ElementImageDictionary["Banana1"];

        public static readonly BitmapImage Crate = ImageManager.ElementImageDictionary["Crate"];

        public static readonly BitmapImage Monkey = ImageManager.ElementImageDictionary["Monkey"];
        public static readonly BitmapImage MonkeyMoveLeft = ImageManager.ElementImageDictionary["MonkeyMoveLeft"];
        public static readonly BitmapImage MonkeyMoveRight = ImageManager.ElementImageDictionary["MonkeyMoveRight"];

        public static readonly BitmapImage Banana = ImageManager.ElementImageDictionary["Banana"];

        public static readonly BitmapImage EyeBanana1 = ImageManager.ElementImageDictionary["EyeBanana1"];
        public static readonly BitmapImage EyeBanana2 = ImageManager.ElementImageDictionary["EyeBanana2"];
        public static readonly BitmapImage EyeBanana3 = ImageManager.ElementImageDictionary["EyeBanana3"];
        public static readonly BitmapImage EyeBanana4 = ImageManager.ElementImageDictionary["EyeBanana4"];

        // tiles

        public static readonly BitmapImage GrassTile = ImageManager.TileImageDictionary["GrassTile"];
        public static readonly BitmapImage BlackTile = ImageManager.TileImageDictionary["BlackTile"];

        //public static readonly BitmapImage GrassTile00000000 = ImageManager.TileImageDictionary["GrassTile00000000"];
        //public static readonly BitmapImage GrassTile00100000 = ImageManager.TileImageDictionary["GrassTile00100000"];
        //public static readonly BitmapImage GrassTile01100000 = ImageManager.TileImageDictionary["GrassTile01100000"];
        //public static readonly BitmapImage GrassTile00110000 = ImageManager.TileImageDictionary["GrassTile00110000"];
        //public static readonly BitmapImage GrassTile01010000 = ImageManager.TileImageDictionary["GrassTile01010000"];
        //public static readonly BitmapImage GrassTile00010010 = ImageManager.TileImageDictionary["GrassTile00010010"];
        //public static readonly BitmapImage GrassTile00010000 = ImageManager.TileImageDictionary["GrassTile00010000"];
        //public static readonly BitmapImage MudTile00000000 = ImageManager.TileImageDictionary["MudTile00000000"];
        //public static readonly BitmapImage MudTile01000000 = ImageManager.TileImageDictionary["MudTile01000000"];
        //public static readonly BitmapImage MudTile01100000 = ImageManager.TileImageDictionary["MudTile01100000"];
        //public static readonly BitmapImage MudTile01101000 = ImageManager.TileImageDictionary["MudTile01101000"];
        //public static readonly BitmapImage MudTile01010000 = ImageManager.TileImageDictionary["MudTile01010000"];
        //public static readonly BitmapImage MudTile00010110 = ImageManager.TileImageDictionary["MudTile00010110"];
        //public static readonly BitmapImage MudTile00101100 = ImageManager.TileImageDictionary["MudTile00101100"];
        //public static readonly BitmapImage MudTile01001001 = ImageManager.TileImageDictionary["MudTile01001001"];
        //public static readonly BitmapImage MudTile00001111 = ImageManager.TileImageDictionary["MudTile00001111"];

        #endregion
    }
}