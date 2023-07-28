// Main Contributors: Weiguang

using System;
using System.Windows;
using System.Windows.Media;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public class MonkeyBoard : Board<MonkeyBoard>
    {
        #region Dependency Properties

        public static readonly DependencyProperty BananasCollectedProperty
            = DependencyProperty.Register("BananasCollected", typeof(int), typeof(MonkeyBoard));

        public static readonly DependencyProperty BananasTotalProperty
            = DependencyProperty.Register("BananasTotal", typeof(int), typeof(MonkeyBoard));

        public static readonly DependencyProperty NumberofLivesProperty
           = DependencyProperty.Register("NumberofLives", typeof(int), typeof(MonkeyBoard));

        public static readonly DependencyProperty BoardLevelProperty
            = DependencyProperty.Register("BoardLevel", typeof(int), typeof(MonkeyBoard));

        public static readonly DependencyProperty BoardTimeProperty
            = DependencyProperty.Register("BoardTime", typeof(int), typeof(MonkeyBoard));

        #endregion

        #region Data Fields

        private static MediaPlayer mediaPlayer;
        private static int instances = 0;

        private XMLMap xmlMap;

        private int secondInterval = 0;

        #endregion

        #region Properties

        protected void SetBananasCollectedDirectly(int bananaCount)
        {
            SetValue(BananasCollectedProperty, bananaCount);
        }

        protected void SetBananasTotalDirectly(int bananaTotal)
        {
            SetValue(BananasTotalProperty, bananaTotal);
        }

        protected void SetBoardTimeDirectly(int boardTime)
        {
            SetValue(BoardTimeProperty, boardTime);
        }

        protected void SetNumberOfLivesDirectly(int numberOfLives)
        {
            SetValue(NumberofLivesProperty, numberOfLives);
        }

        public int BananasCollected
        {
            get { return (int)GetValue(BananasCollectedProperty); }
            protected set
            {
                SetValue(BananasCollectedProperty, value);

                BananasEventArgs e = new BananasEventArgs(BananasCollected, BananasTotal);

                if (BananaCollect != null)
                    BananaCollect(this, e);

                if (BananasCollected == BananasTotal && BananaTotalCollect != null)
                    BananaTotalCollect(this, e);
            }
        }

        public int BananasTotal
        {
            get { return (int)GetValue(BananasTotalProperty); }
            protected set
            {
                SetValue(BananasTotalProperty, value);

                BananasEventArgs e = new BananasEventArgs(BananasCollected, BananasTotal);

                if (BananasTotalChanged != null)
                    BananasTotalChanged(this, e);

                if (BananasCollected == BananasTotal && BananaTotalCollect != null)
                    BananaTotalCollect(this, e);
            }
        }

        public int NumberOfLives
        {
            get { return (int)GetValue(NumberofLivesProperty); }
            protected set
            {
                SetValue(NumberofLivesProperty, value);

                MonkeyLivesEventArgs e = new MonkeyLivesEventArgs(NumberOfLives);

                if (LivesLeft != null)
                    LivesLeft(this, e);

                if (NumberOfLives == 0 && AllLivesUsed != null)
                    AllLivesUsed(this, e);
            }
        }

        public int BoardTime
        {
            get { return (int)GetValue(BoardTimeProperty); }
            protected set
            {
                SetValue(BoardTimeProperty, value);
                TimerEventArgs e = new TimerEventArgs(BoardTime);

                OnBoardTimerFullTick(e);

                if (BoardTime == 0)
                    DecreaseNumberOfLives();
            }
        }

        public XMLMap XmlMap
        {
            get { return xmlMap; }
            set { xmlMap = value; }
        }

        public int BoardLevel
        {
            get { return (int)GetValue(BoardLevelProperty); }
            set { SetValue(BoardLevelProperty, value); }
        }

        public int SecondInterval
        {
            get { return secondInterval; }
            set { secondInterval = value; }
        }

        #endregion

        #region Events

        public event BananasEventHandler BananaCollect;
        public event BananasEventHandler BananasTotalChanged;
        public event BananasEventHandler BananaTotalCollect;
        public event MonkeyLivesEventHandler LivesLeft;
        public event MonkeyLivesEventHandler AllLivesUsed;
        public event BoardTimerEventHandler BoardTimerFullTick;
        public event BoardTimerEventHandler Restart;

        public event EventHandler BoardCleared;

        #endregion

        #region Methods

        private void LoopMusicPlayback()
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(@"Resources/Music/Monkey Quest.mp3", UriKind.Relative));
            mediaPlayer.Play();
            mediaPlayer.MediaEnded += (object sender, EventArgs e) => { mediaPlayer.Stop(); mediaPlayer.Play(); };
        }

        private void InitializeBindings()
        {
            Restart += (sender, e) => RestartMap();
            BoardTick += BoardTimerTick;
        }

        private void InitializeBoardBorder()
        {
            // top & bottom border
            for (int x = 0; x < this.Columns; x++)
            {
                new BorderTile() { Position = new Position(x, -1) }.SetBoard(this);
                new BorderTile() { Position = new Position(x, this.Rows) }.SetBoard(this);
            }

            // left & right border
            for (int y = 0; y < this.Rows; y++)
            {
                new BorderTile() { Position = new Position(-1, y) }.SetBoard(this);
                new BorderTile() { Position = new Position(this.Columns, y) }.SetBoard(this);
            }
        }

        private void ResetAllCounters()
        {
            const int UnassignedValue = 0;

            SetBananasCollectedDirectly(UnassignedValue);
            SetBananasTotalDirectly(UnassignedValue);
            SetBoardTimeDirectly(UnassignedValue);
        }

        protected virtual void OnBoardCleared(EventArgs e)
        {
            if (BoardCleared != null)
                BoardCleared(this, e);
        }

        protected virtual void OnBoardTimerFullTick(TimerEventArgs e)
        {
            if (BoardTimerFullTick != null)
                BoardTimerFullTick(this, e);
        }

        protected virtual void OnRestart(TimerEventArgs e)
        {
            if (Restart != null)
                Restart(this, e);
        }

        internal protected void MonkeyReachedDoorBinding(object sender, EventArgs e)
        {
            OnBoardCleared(new EventArgs());
        }

        private void BoardTimerTick(object sender, EventArgs e)
        {
            if (++SecondInterval == 50 && BoardTime != 0)
            {
                BoardTime--;
                SecondInterval = 0;
            }
        }


        internal protected void IncrementBananaCollected()
        {
            BananasCollected++;
        }

        internal protected void IncrementBananaTotal()
        {
            BananasTotal++;
        }

        internal protected void DecreaseNumberOfLives()
        {
            NumberOfLives--;
            OnRestart(new TimerEventArgs(BoardTime));
        }

        internal protected void SetNumberOfLives(int value)
        {
            NumberOfLives = value;
        }

        public override void Clear()
        {
            base.Clear();

            // also flush away all the counters
            ResetAllCounters();

            // but add back the board border
            InitializeBoardBorder();
        }

        public void RestartMap()
        {
            int currentLives = NumberOfLives;

            // clear the board first before adding the stuff
            Clear();
            LoadXmlMap();

            NumberOfLives = currentLives;
        }

        private bool LoadXmlMap()
        {
            // load the correct rows, columns and timer first
            Rows = XmlMap.MapObject.Rows;
            Columns = XmlMap.MapObject.Columns;
            SetBoardTimeDirectly(XmlMap.MapObject.Time);

            // starting parsing the data
            foreach (MapObject mapObject in XmlMap.MapTileObjects)
            {
                Type type = mapObject.GetType();
                String typeString = type.Name;

                Position position = new Position(mapObject.X, mapObject.Y);

                switch (typeString)
                {
                    case "StaticMapObject":
                        StaticMapObject staticMapObject = (StaticMapObject)mapObject;

                        switch (staticMapObject.Identifier)
                        {
                            case "monkey":
                                new Monkey().SetBoard(this).SetPosition(position);
                                break;
                            case "door":
                                new Door().SetBoard(this).SetPosition(position);
                                break;
                            case "spike":
                                new Spike().SetBoard(this).SetPosition(position);
                                break;
                            case "spikeceiling":
                                new SpikeCeiling().SetBoard(this).SetPosition(position);
                                break;
                            case "spider":
                                new Spider().SetBoard(this).SetPosition(position);
                                break;
                            case "crab":
                                new Crab().SetBoard(this).SetPosition(position);
                                break;
                            case "octopus":
                                new Octopus().SetBoard(this).SetPosition(position);
                                break;
                            case "tortoise":
                                new Tortoise().SetBoard(this).SetPosition(position);
                                break;
                            case "tortoiseleft":
                                new TortoiseLeft().SetBoard(this).SetPosition(position);
                                break;
                            case "gaga":
                                new Gaga().SetBoard(this).SetPosition(position);
                                break;
                            case "goomba":
                                new Goomba().SetBoard(this).SetPosition(position);
                                break;
                            case "ghost":
                                new Ghost().SetBoard(this).SetPosition(position);
                                break;
                            default:
                                return false;
                        }

                        break;


                    case "TileMapObject":

                        TileMapObject tileMapObject = (TileMapObject)mapObject;

                        switch (tileMapObject.Type)
                        {
                            case "crate":
                                new Crate().SetBoard(this).SetPosition(position);
                                break;
                            case "banana":
                                new Banana().SetBoard(this).SetPosition(position);
                                break;
                            case "mud":
                                new Tile("MudTile" + tileMapObject.Property).SetBoard(this).SetPosition(position);
                                break;
                            case "grass":
                                new Tile("GrassTile" + tileMapObject.Property).SetBoard(this).SetPosition(position);
                                break;
                            default:
                                return false;
                        }

                        break;
                }
            }

            return true;
        }

        private static MonkeyBoard LoadXmlMap(XMLMap xmlMap)
        {
            int rows = xmlMap.MapObject.Rows;
            int columns = xmlMap.MapObject.Columns;
            int time = xmlMap.MapObject.Time;

            // Initialize the size of board (along with fineness limit and frames rate)
            MonkeyBoard board = new MonkeyBoard(columns, rows, time) { XmlMap = xmlMap };
            board.LoadXmlMap();

            return board;
        }

        public static MonkeyBoard LoadXmlMap(string filename)
        {
            // Load XML map
            return LoadXmlMap(new XMLMap(filename));
        }

        #endregion

        #region Constructors

        public MonkeyBoard(int columns, int rows, int time, int finenessLimit = Values.FinenessLimit, int framesPerSecond = Values.FramesPerSecond)
            : base(columns, rows, finenessLimit, framesPerSecond)
        {
            // loads the background music and plays it
            if (instances == 0)
                LoopMusicPlayback();

            // adds to number of instances
            instances++;

            // set board level to 1 by default
            BoardLevel = 1;

            // initialize the board border
            InitializeBoardBorder();

            //set the time for the board level
            BoardTime = time;

            // initialize bindings
            InitializeBindings();

            //// initialize the tile manager
            //InitializeTileManager();
        }

        // removes static number of instances
        // this is to prevent two boards from playing 2 soundtracks at the same time
        ~MonkeyBoard()
        {
            instances--;
        }

        #endregion
    }

    public class BananasEventArgs : EventArgs
    {
        #region Data Fields

        private int bananasCollected;
        private int bananasTotal;

        #endregion

        #region Properties

        public int BananasCollected
        {
            get { return bananasCollected; }
            protected set { bananasCollected = value; }
        }

        public int BananasTotal
        {
            get { return bananasTotal; }
            protected set { bananasTotal = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public BananasEventArgs(int bananasCollected, int bananasTotal)
        {
            BananasCollected = bananasCollected;
            BananasTotal = bananasTotal;
        }

        #endregion
    }

    public class MonkeyLivesEventArgs : EventArgs
    {
        #region Data Fields

        private int numberofLives;

        #endregion

        #region Properties

        public int NumberofLives
        {
            get { return numberofLives; }
            protected set { numberofLives = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public MonkeyLivesEventArgs(int numberofLives)
        {
            NumberofLives = numberofLives;
        }

        #endregion
    }

    public class TimerEventArgs : EventArgs
    {
        #region Data Fields

        private int remainingTime;
        
        #endregion

        #region Properties

        public int RemainingTime
        {
            get { return remainingTime; }
            protected set { remainingTime = value; }
        }

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public TimerEventArgs(int remainingTime)
        {
            RemainingTime = remainingTime;
        }

        #endregion
    }

    public delegate void BananasEventHandler(object sender, BananasEventArgs e);
    public delegate void MonkeyLivesEventHandler(object sender, MonkeyLivesEventArgs e);
    public delegate void BoardTimerEventHandler(object sender, TimerEventArgs e);
}