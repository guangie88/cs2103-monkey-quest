using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

using Multimedia;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.MonkeyQuest.UI;

namespace MonkeyQuest.Test
{
    public class TestPackage : ContentElement
    {
        #region Data Fields

        public const int InitialLives = 99;
        private int time;
        private List<string> mapPathList = new List<string>();
        private MonkeyBoard currentBoard;
        private MonkeyBoardUI boardUI;
        private MonkeyMainBarUI mainBarUI;

        private int currentLives = InitialLives;

        private int currentBoardIndex = 0;
        private bool isRepeating = false;
        private bool trig = false;

        private StreamWriter log;

        #endregion

        private void TimeKeep(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(time);

            if (time == 180)
            {
                bool AIfail = false;
                int bananaCount = 0;
                int crateCount = 0;
                foreach (Collidable<MonkeyBoard> c in boardUI.Board.CollisionManager.CollidableList)
                {
                    if (c is AI<MonkeyBoard>)
                    {
                        AIfail = true;
                    }
                    if (c.Position.Y < 5)
                    {
                        if (c is Banana)
                            bananaCount++;
                        else if (c is Crate)
                            crateCount++;
                    }
                }
                if ((AIfail == false) && ((bananaCount == 4) && (crateCount == 4)))
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    //    MessageBox.Show("First map tests passed.");
                    //}));
                    log.WriteLine("First map tests passed.");

                else if (AIfail)
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //    {
                    //        MessageBox.Show("Test Fails-There should be no AI elements on screen now.");
                    //    }));
                    log.WriteLine("Test Fails-There should be no AI elements on screen now.");
                else 
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //    {
                    //        MessageBox.Show("Test Fails-There should be 4 bananas & 4 crates in the upper half of the screen now.");
                    //    }));
                    log.WriteLine("Test Fails-There should be 4 bananas & 4 crates in the upper half of the screen now.");
            }
            else if (time == 380)
            {
                int counter = 0;
                bool monkey = false;
                foreach (Collidable<MonkeyBoard> c in boardUI.Board.CollisionManager.CollidableList)
                {
                    if (c is Banana)
                        counter++;
                    else if (c is Monkey)
                        monkey = true;
                }
                if ((counter <= 63) && (monkey == true))
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    //    MessageBox.Show("Second map tests passed.");
                    //}));
                    log.WriteLine("Second map tests passed.");
                else if (monkey == true)
                //    Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    MessageBox.Show("Second map tests failed - too many bananas on screen.");
                //}));
                    log.WriteLine("Second map tests failed - " + counter + " bananas on screen. There should be not more than 63 bananas.");
                else 
                //    Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    MessageBox.Show("Second map tests failed - No monkey on screen.
                    //How do you get yourself killed with only bananas on the map lol.");
                //}));
                    log.WriteLine("Second map tests failed - No monkey on screen. How do you " +
                "get yourself killed with only bananas on the map lol.");

            }
            else if (time == 580)
            {
                bool crate = false;
                bool ai = true;
                bool monkey = true;
                foreach (Collidable<MonkeyBoard> c in currentBoard.CollisionManager.CollidableList)
                {
                    if ((c is Crate) && (c.X < 8))
                    {
                        crate = true;
                        //Dispatcher.BeginInvoke((Action)(() =>
                        //        {
                        //        }));
                        log.WriteLine("Third Map test failed - Controllables" + 
                            "should be able to push the crates towards the right.");
                    }
                    else if (c is Monkey) {
                        monkey = false;
                    }                        
                    else if (c is AI<MonkeyBoard>) {
                        ai = false;
                    }
                }
                if ((!ai) && (!monkey) && (!crate))
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //    {
                    //        MessageBox.Show("Third Map tests passed.");
                    //    }));
                    log.WriteLine("Third Map tests passed.");
                else if (ai)
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    //    MessageBox.Show("Third Map test failed - AI should be alive & present on board.");
                    //}));
                    log.WriteLine("Third Map test failed - AI should be alive & present on board.");
                else
                    //Dispatcher.BeginInvoke((Action)(() =>
                    //{
                    //    MessageBox.Show("Third Map test failed - Monkey should be alive & present on board.");
                    //}));
                    log.WriteLine("Third Map test failed - Monkey should be alive & present on board.");
            }
            else if (time == 605)
            {
                Rocket r = new RocketRight();
                for (int i = 0; i < currentBoard.CollisionManager.CollidableList.Count; i++) {
                    Collidable<MonkeyBoard> c = currentBoard.CollisionManager.CollidableList[i];
                    if (c is Monkey)
                    {
                        r.SetPosition(new Position(c.X + 1, c.Y));
                        r.Board = currentBoard;
                    }
                }
            }
            else if ((time < 580) && (time > 400))
            {
                foreach (Collidable<MonkeyBoard> c in currentBoard.CollisionManager.CollidableList)
                    if (c is Monkey)
                        (c as Monkey).AddMovement(new MoveLeftRight(1000));
            }
            else if ((time < 780) && (time > 600))
            {
                foreach (Collidable<MonkeyBoard> c in currentBoard.CollisionManager.CollidableList)
                {
                    if (c is Monkey)
                    {
                        if (!trig)
                            (c as Monkey).AddMovement(new MoveLeftRight(600));
                        else
                            (c as Monkey).AddMovement(new MoveLeftRight(-600));
                    }
                }
            }
            else if (time == 790)
            {
                foreach (Collidable<MonkeyBoard> c in currentBoard.CollisionManager.CollidableList)
                    if (c is AI<MonkeyBoard>)
                        log.WriteLine("Fourth map test failed - AI should no longer be alive. Weapons systems offline :(");
                //Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        MessageBox.Show("Fourth map tests failed. Should've exit using the door by this time.");
                //    }));
                log.WriteLine("Fourth map test failed. Should've exit using the door by this time.");
                log.Flush();
                log.Close();
            }
            else if (time == 600)
                currentBoard.BananaCollect += bananaCollected;


            if ((time++ % 200) > 198)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    Pause();
                    NextBoard();
                    Start();
                }));

                // infinite bindings here
                InitializeBindings();
                boardUI.Board.BoardTick += TimeKeep;
            }
        }

        public void bananaCollected(object sender, BananasEventArgs e) {
            if (e.BananasCollected == e.BananasTotal)
                trig = true;
        }

        #region Properties

        public IList<string> MapPathList
        {
            get { return mapPathList; }
            protected set { mapPathList = new List<string>(value); }
        }

        public MonkeyBoardUI BoardUI
        {
            get { return boardUI; }
            protected set { boardUI = value; }
        }

        public MonkeyMainBarUI MainBarUI
        {
            get { return mainBarUI; }
            protected set { mainBarUI = value; }
        }

        public bool IsRepeating
        {
            get { return isRepeating; }
            set { isRepeating = value; }
        }

        public int CurrentBoardIndex
        {
            get { return currentBoardIndex; }
            private set { currentBoardIndex = value; }
        }

        public MonkeyBoard CurrentBoard
        {
            get { return currentBoard; }
            private set { currentBoard = value; }
        }
        #endregion

        #region Methods

        internal void RestartKeyBinding(object sender, KeyEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (e.Key == Key.R)
                    RestartBoard();
            }));
        }

        private void InitializeBoard()
        {
            DisplayBoard(JumpToBoard(0));
        }

        private void InitializeBindings()
        {
            // culprit of needing dispatcher
            CurrentBoard.BoardCleared += (object sender, EventArgs e) =>
            {
                // culprit
                //Dispatcher.Invoke((Action)(() =>
                //{
                //    MessageBox.Show("Fourth Map Tests passed.");
                //    Environment.Exit(0);
                //}));
                foreach (Collidable<MonkeyBoard> c in currentBoard.CollisionManager.CollidableList)
                    if (c is AI<MonkeyBoard>)
                        log.WriteLine("Fourth map test failed - AI should no longer be alive. Weapons systems offline :(");
                log.WriteLine("Fourth Map tests passed.");
                log.Flush();
                log.Close();
                Environment.Exit(0);

                // infinite bindings here
                InitializeBindings();
            };
        }

        private void DisplayBoard(MonkeyBoard board)
        {
            BoardUI.Board = board;
            MainBarUI.Board = board;
        }

        // returns a clone of the board so that the player does not actually change
        // the elements on the initial board itself
        public MonkeyBoard JumpToBoard(int index)
        {
            // this is to transit the number of lives across all boards
            int currentLives = InitialLives;

            if (CurrentBoard != null)
                currentLives = CurrentBoard.NumberOfLives;

            if (index >= MapPathList.Count)
                throw new Exception("Attempting to jump to a board index that is more than current board count!");

            CurrentBoardIndex = index;
            CurrentBoard = MonkeyBoard.LoadXmlMap(MapPathList[CurrentBoardIndex]);

            // need to plus one since board level starts with 1 while index starts with 0
            CurrentBoard.BoardLevel = CurrentBoardIndex + 1;
            CurrentBoard.SetNumberOfLives(currentLives);

            return CurrentBoard;
        }

        public void RestartBoard()
        {
            // pauses, clears and then re-display the same board
            Pause();
            CurrentBoard.RestartMap();
            Start();

            //CurrentBoard.Clear();
            //DisplayBoard(JumpToBoard(CurrentBoardIndex));
            //InitializeBindings();
        
        
        
        }

        public virtual void NextBoard()
        {
            // remove current board first
            CurrentBoard.Clear();

            int nextBoardIndex = CurrentBoardIndex + 1;

            if (IsRepeating)
                DisplayBoard(JumpToBoard(nextBoardIndex % MapPathList.Count));
            else
                DisplayBoard(JumpToBoard(nextBoardIndex));
        }

        public void Pause()
        {
            if (CurrentBoard != null)
                CurrentBoard.Pause();
        }

        public void Start()
        {
            if (CurrentBoard != null)
                CurrentBoard.Start();
        }

        #endregion

        #region Constructors

        // set to 0, 0 because the initialization is done over at SetUpCurrentBoard
        public TestPackage(IEnumerable<string> mapPathEnumerable, MonkeyBoardUI boardUI, MonkeyMainBarUI mainBarUI, bool isRepeating = false)
        {
            MapPathList = new List<string>(mapPathEnumerable);
            BoardUI = boardUI;
            MainBarUI = mainBarUI;
            IsRepeating = isRepeating;

            InitializeBoard();
            InitializeBindings();
            time = 0;
            boardUI.Board.BoardTick += TimeKeep;
            String tmp = DateTime.Now.ToString();
            foreach (char c in Path.GetInvalidFileNameChars())
                tmp = tmp.Replace(c.ToString(), String.Empty);
            log = new StreamWriter(@"TestSuite on " + System.Environment.MachineName + " @ " + tmp +".log", true);
        }

        #endregion
    }
}
