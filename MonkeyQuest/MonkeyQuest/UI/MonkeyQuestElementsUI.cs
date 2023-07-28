// Main Contributors: Frank, Silin
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.Resources;
using MonkeyQuest.Resources.Utility;
using MonkeyQuest.MonkeyQuest.Logic;

namespace MonkeyQuest.MonkeyQuest.UI
{
    #region Crate

    public class CrateUI : PositionalUI<MonkeyBoard, Crate, Image>
    {
        public CrateUI(Crate crate, BoardUI<MonkeyBoard> boardUI = null)
            : base(crate, new Image() { Source = Images.Crate }, boardUI)
        {
        }
    }


    #endregion

    #region Monkey

    public class MonkeyUI : ControllableAnimatedUI<MonkeyBoard, Monkey, ControllableAnimatedImage>
    {
        public MonkeyUI(Monkey monkey, BoardUI<MonkeyBoard> boardUI = null)
            : base(monkey, new ControllableAnimatedImage(
                new List<ControllableImageFrames> { 
                    new ControllableImageFrames(new Image() { Source = Images.MonkeyMoveLeft }, Key.Left),
                    new ControllableImageFrames(new Image() { Source = Images.MonkeyMoveRight }, Key.Right)
                }, new Image() { Source = Images.Monkey })
            , boardUI)
        {
        }
    }

    /*public class MonkeyUI : PositionalUI<Monkey, Image>
    {
        public MonkeyUI(Monkey monkey, BoardUI boardUI = null)
            : base(monkey, new Image() { Source = Images.Monkey }, boardUI)
        {
        }
    }*/

    #endregion

    #region MonkeyBanana

    public class MonkeyBananaUI : PositionalAnimatedUI<MonkeyBoard, Banana, AnimatedImage>
    {
        static Random rnd = new Random();

        public MonkeyBananaUI(Banana block, BoardUI<MonkeyBoard> boardUI = null)
            : base(block, new AnimatedImage(
                new List<ImageFrames> { 
                    new ImageFrames(new Image() { Source = Images.EyeBanana1 }, rnd.Next(50, 121)), 
                    new ImageFrames(new Image() { Source = Images.EyeBanana2 }, 7),
                    new ImageFrames(new Image() { Source = Images.EyeBanana3 }, 7),
                    new ImageFrames(new Image() { Source = Images.EyeBanana4 }, 7),
                    new ImageFrames(new Image() { Source = Images.EyeBanana3 }, 7),
                    new ImageFrames(new Image() { Source = Images.EyeBanana2 }, 7)
                })
            , boardUI)
        {
        }
    }

    #endregion

    #region AIUI

    public class SpikeCeilingUI : PositionalUI<MonkeyBoard, SpikeCeiling, Image>
    {
        public SpikeCeilingUI(SpikeCeiling spikeCeiling, BoardUI<MonkeyBoard> boardUI = null)
            : base(spikeCeiling, new Image() { Source = ImageManager.ElementImageDictionary["SpikeCeiling"] }, boardUI)
        {
        }
    }

    public class SpikeUI : PositionalUI<MonkeyBoard, Spike, Image>
    {
        public SpikeUI(Spike spike, BoardUI<MonkeyBoard> boardUI = null)
            : base(spike, new Image() { Source = ImageManager.ElementImageDictionary["Spike"] }, boardUI)
        {
        }
    }

    public class SpiderUI : PositionalUI<MonkeyBoard, Spider, Image>
    {
        public SpiderUI(Spider spider, BoardUI<MonkeyBoard> boardUI = null)
            : base(spider, new Image() { Source = ImageManager.ElementImageDictionary["Spider"] }, boardUI)
        {
        }
    }

    public class CrabUI : PositionalUI<MonkeyBoard, Crab, Image>
    {
        public CrabUI(Crab crab, BoardUI<MonkeyBoard> boardUI = null)
            : base(crab, new Image() { Source = ImageManager.ElementImageDictionary["Crab"] }, boardUI)
        {
        }
    }

    public class OctopusUI : PositionalUI<MonkeyBoard, Octopus, Image>
    {
        public OctopusUI(Octopus octopus, BoardUI<MonkeyBoard> boardUI = null)
            : base(octopus, new Image() { Source = ImageManager.ElementImageDictionary["Octopus"] }, boardUI)
        {
        }
    }

    public class TortoiseUI : PositionalUI<MonkeyBoard, Tortoise, Image>
    {
        public TortoiseUI(Tortoise tortoise, BoardUI<MonkeyBoard> boardUI = null)
            : base(tortoise, new Image() { Source = ImageManager.ElementImageDictionary["Tortoise"] }, boardUI)
        {
        }
    }

    public class TortoiseLeftUI : PositionalUI<MonkeyBoard, Tortoise, Image>
    {
        public TortoiseLeftUI(TortoiseLeft tortoiseLeft, BoardUI<MonkeyBoard> boardUI = null)
            : base(tortoiseLeft, new Image() { Source = ImageManager.ElementImageDictionary["TortoiseLeft"] }, boardUI)
        {
        }
    }

    public class GagaUI : MovableAnimatedUI<MonkeyBoard, Gaga, MovableAnimatedImage>
    {
        public GagaUI(Gaga gaga, BoardUI<MonkeyBoard> boardUI = null)
            : base(gaga, new MovableAnimatedImage(
                new List<ImageFrames> { 
                    new ImageFrames(new Image() { Source = Images.Monster }, 500)
                }), boardUI)
        {

        }
    }

    public class GhostUI : MovableAnimatedUI<MonkeyBoard, Ghost, MovableAnimatedImage>
    {
        static Random rnd = new Random();
        public GhostUI(Ghost Ghost, BoardUI<MonkeyBoard> boardUI = null)
            : base(Ghost, new MovableAnimatedImage(
                new List<ImageFrames> { 
                    new ImageFrames(new Image() { Source = Images.Ghost1 }, 1), 
                    new ImageFrames(new Image() { Source = Images.Ghost2 }, 1)
                })
            , boardUI)
        {
        }
    }

    public class GoombaUI : MovableAnimatedUI<MonkeyBoard, Goomba, MovableAnimatedImage>
    {
        static Random rnd = new Random();
        public GoombaUI(Goomba Goomba, BoardUI<MonkeyBoard> boardUI = null)
            : base(Goomba, new MovableAnimatedImage(
                new List<ImageFrames> { 
                    new ImageFrames(new Image() { Source = Images.Goomba1 }, 1), 
                    new ImageFrames(new Image() { Source = Images.Goomba2 }, 1)
                })
            , boardUI)
        {

        }
    }

    #endregion
    
    #region WeaponUI

    public class RocketUI : PositionalUI<MonkeyBoard, Rocket, Image>
    {
        public RocketUI(Rocket rocket, Image image, BoardUI<MonkeyBoard> boardUI = null)
            : base(rocket, image, boardUI)
        {
        }
    }

    #endregion
}
