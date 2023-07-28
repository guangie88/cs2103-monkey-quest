// Main Contributors: Silin

using System.Windows.Controls;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Framework.UI;
using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.Resources.Utility;

namespace MonkeyQuest.MonkeyQuest.UI
{
    public class MonkeyBoardUI : BoardUI<MonkeyBoard>
    {
        #region Data Fields

        #endregion

        #region Properties

        public override MonkeyBoard Board
        {
            get { return base.Board; }
            set
            {
                // removing done in base class
                base.Board = value;

                if (Board != null)
                {
                    // convert and wrap all the relevant logic parts with their respective UI
                    foreach (Positional<MonkeyBoard> positional in Board.Positionals)
                        LogicUICreatorManager.Convert(positional);
                }
            }
        }

        #endregion

        #region Methods

        protected override void InitializeLogicUISetup()
        {
            const int TileZIndex = 5;
            const int PositionalZIndex = 10;
            const int AIZIndex = 13;
            const int CrateZIndex = 15;
            const int MonkeyZIndex = 20; 

            //LogicUICreatorManager.Add(typeof(MonkeyBlock).ToString(), new LogicUICreator<MonkeyBlock>((MonkeyBlock logic) => new MonkeyBlockUI(logic, this)));

            // stub added here for testing purposes
            LogicUICreatorManager.Add(typeof(Stub).ToString(), new LogicUICreator<MonkeyBoard, Stub>((Stub logic) => new StubUI(logic, this) { ZIndex = PositionalZIndex }));
            
            // monkey and its bananas created here
            LogicUICreatorManager.Add(typeof(Monkey).ToString(), new LogicUICreator<MonkeyBoard, Monkey>((Monkey logic) => new MonkeyUI(logic, this) { ZIndex = MonkeyZIndex }));
            LogicUICreatorManager.Add(typeof(Banana).ToString(), new LogicUICreator<MonkeyBoard, Banana>((Banana logic) => new MonkeyBananaUI(logic, this) { ZIndex = PositionalZIndex }));

            //weapons
            LogicUICreatorManager.Add(typeof(RocketLeft).ToString(), new LogicUICreator<MonkeyBoard, RocketLeft>((RocketLeft logic) => new RocketUI(logic, new Image() { Source = ImageManager.ElementImageDictionary["Rocket2"] }, this) { ZIndex = MonkeyZIndex }));
            LogicUICreatorManager.Add(typeof(RocketRight).ToString(), new LogicUICreator<MonkeyBoard, RocketRight>((RocketRight logic) => new RocketUI(logic, new Image() { Source = ImageManager.ElementImageDictionary["Rocket1"] }, this) { ZIndex = MonkeyZIndex }));

            // door over here
            LogicUICreatorManager.Add(typeof(Door).ToString(), new LogicUICreator<MonkeyBoard, Door>((Door logic) => new DoorUI(logic, this) { ZIndex = PositionalZIndex }));

            // followed up by the enemies
            LogicUICreatorManager.Add(typeof(Spike).ToString(), new LogicUICreator<MonkeyBoard, Spike>((Spike logic) => new SpikeUI(logic, this) { ZIndex = PositionalZIndex }));
            LogicUICreatorManager.Add(typeof(SpikeCeiling).ToString(), new LogicUICreator<MonkeyBoard, SpikeCeiling>((SpikeCeiling logic) => new SpikeCeilingUI(logic, this) { ZIndex = PositionalZIndex }));
            LogicUICreatorManager.Add(typeof(Spider).ToString(), new LogicUICreator<MonkeyBoard, Spider>((Spider logic) => new SpiderUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Crab).ToString(), new LogicUICreator<MonkeyBoard, Crab>((Crab logic) => new CrabUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Octopus).ToString(), new LogicUICreator<MonkeyBoard, Octopus>((Octopus logic) => new OctopusUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Tortoise).ToString(), new LogicUICreator<MonkeyBoard, Tortoise>((Tortoise logic) => new TortoiseUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(TortoiseLeft).ToString(), new LogicUICreator<MonkeyBoard, TortoiseLeft>((TortoiseLeft logic) => new TortoiseLeftUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Gaga).ToString(), new LogicUICreator<MonkeyBoard, Gaga>((Gaga logic) => new GagaUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Ghost).ToString(), new LogicUICreator<MonkeyBoard, Ghost>((Ghost logic) => new GhostUI(logic, this) { ZIndex = AIZIndex }));
            LogicUICreatorManager.Add(typeof(Goomba).ToString(), new LogicUICreator<MonkeyBoard, Goomba>((Goomba logic) => new GoombaUI(logic, this) { ZIndex = AIZIndex }));

            // crates over here
            LogicUICreatorManager.Add(typeof(Crate).ToString(), new LogicUICreator<MonkeyBoard, Crate>((Crate logic) => new CrateUI(logic, this) { ZIndex = CrateZIndex }));

            // border tile is unique because it's invisible and it disallows wall jumping
            LogicUICreatorManager.Add(typeof(BorderTile).ToString(),
                new LogicUICreator<MonkeyBoard, BorderTile>((BorderTile logic) => new TileUI(logic, null, this)));

            // all other tiles here are normal, including grass and mud tiles
            foreach (string identifier in ImageManager.TileImageDictionary.Keys)
                LogicUICreatorManager.Add(typeof(Tile).ToString() + identifier,
                    new LogicUICreator<MonkeyBoard, Tile>((Tile logic) => new TileUI(logic, new Image() { Source = ImageManager.TileImageDictionary[logic.TileIdentifier] }, this) { ZIndex = TileZIndex }));
        }

        #endregion

        #region Constructors

        public MonkeyBoardUI(MonkeyBoard board, double displayUnit = 80.0) : base(board, displayUnit)
        {
            // all work done by setting Board = board (virtual property)
        }

        #endregion
    }
}