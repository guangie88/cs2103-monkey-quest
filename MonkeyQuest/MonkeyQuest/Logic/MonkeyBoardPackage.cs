using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;

using Multimedia;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.MonkeyQuest.UI;

public class MonkeyBoardPackage : ContentElement
{
    #region Data Fields

    public const int InitialLives = 3;

    private List<string> mapPathList = new List<string>();
    private MonkeyBoard currentBoard;
    private MonkeyBoardUI boardUI;
    private MonkeyMainBarUI mainBarUI;

    private int currentLives = InitialLives;

    private int currentBoardIndex = 0;
    private bool isRepeating = false;

    #endregion

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

    #region Events

    public event MonkeyBoardGameoverEventHandler GameCompleted;
    public event MonkeyBoardGameoverEventHandler GameoverWithNoLives;

    #endregion

    #region Methods

    private void InitializeBoard()
    {
        DisplayBoard(GoToBoardAndBind(0));
    }

    private void BindingsToNextBoard()
    {
        CurrentBoard.BoardCleared += (object sender, EventArgs e) =>
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Pause();
                NextBoard();
                Start();
            }));

            // infinite bindings to link one map to another
            BindingsToNextBoard();
        };
    }

    private void AllLivesUsedBinding(object sender, MonkeyLivesEventArgs e)
    {
        OnGameoverWithNoLives(new MonkeyBoardGameoverEventArgs(CurrentBoard.BoardLevel, MapPathList.Count, e.NumberofLives));
    }

    private void DisplayBoard(MonkeyBoard board)
    {
        // defensive programming needed in case the board package doesn't go into cyclic mode
        if (board != null)
        {
            BoardUI.Board = board;
            MainBarUI.Board = board;
        }
    }

    protected void OnGameCompleted(MonkeyBoardGameoverEventArgs e)
    {
        if (GameCompleted != null)
            GameCompleted(this, e);
    }

    protected void OnGameoverWithNoLives(MonkeyBoardGameoverEventArgs e)
    {
        if (GameoverWithNoLives != null)
            GameoverWithNoLives(this, e);
    }

    internal void RestartKeyBinding(object sender, KeyEventArgs e)
    {
        Dispatcher.Invoke((Action)(() =>
        {
            if (e.Key == Key.R)
                RestartBoard();
        }));
    }

    // returns a clone of the board so that the player does not actually change
    // the elements on the initial board itself
    public MonkeyBoard GoToBoardAndBind(int index)
    {
        // this is to transit the number of lives across all boards
        int currentLives = InitialLives;

        if (CurrentBoard != null)
        {
            currentLives = CurrentBoard.NumberOfLives;

            // disengage the gameover event for current board
            CurrentBoard.AllLivesUsed -= AllLivesUsedBinding;
        }

        if (index >= MapPathList.Count)
            throw new Exception("Attempting to jump to a board index that is more than current board count!");

        CurrentBoardIndex = index;
        CurrentBoard = MonkeyBoard.LoadXmlMap(MapPathList[CurrentBoardIndex]);

        // need to plus one since board level starts with 1 while index starts with 0
        CurrentBoard.BoardLevel = CurrentBoardIndex + 1;
        CurrentBoard.SetNumberOfLives(currentLives);

        // do some initialization here for gameover
        CurrentBoard.AllLivesUsed += AllLivesUsedBinding;

        return CurrentBoard;
    }

    public void RestartBoard()
    {
        // pauses, clears and then re-display the same board
        Pause();
        CurrentBoard.RestartMap();
        DisplayBoard(CurrentBoard);
        Start();
    }

    public virtual void NextBoard()
    {
        int nextBoardIndex = CurrentBoardIndex + 1;

        // check whether we are currently at the last map
        if (nextBoardIndex == MapPathList.Count)
            OnGameCompleted(new MonkeyBoardGameoverEventArgs(CurrentBoard.BoardLevel, MapPathList.Count, CurrentBoard.NumberOfLives));

        // remove current board first
        CurrentBoard.Clear();

        if (IsRepeating)
            DisplayBoard(GoToBoardAndBind(nextBoardIndex % MapPathList.Count));
        else if (nextBoardIndex < MapPathList.Count)
            DisplayBoard(GoToBoardAndBind(nextBoardIndex));
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
    public MonkeyBoardPackage(IEnumerable<string> mapPathEnumerable, MonkeyBoardUI boardUI, MonkeyMainBarUI mainBarUI, bool isRepeating = false)
    {
        MapPathList = new List<string>(mapPathEnumerable);
        BoardUI = boardUI;
        MainBarUI = mainBarUI;
        IsRepeating = isRepeating;

        InitializeBoard();
        BindingsToNextBoard();
    }

    #endregion
}

public delegate void MonkeyBoardGameoverEventHandler(object sender, MonkeyBoardGameoverEventArgs e);

public class MonkeyBoardGameoverEventArgs : EventArgs
{
    #region Data Fields

    private int levelEndedAt;
    private int levelTotalCount;
    private int livesLeft;

    #endregion

    #region Properties

    public int LevelEndedAt
    {
        get { return levelEndedAt; }
        protected set { levelEndedAt = value; }
    }

    public int LevelTotalCount
    {
        get { return levelTotalCount; }
        protected set { levelTotalCount = value; }
    }

    public int LivesLeft
    {
        get { return livesLeft; }
        protected set { livesLeft = value; }
    }

    #endregion

    #region Constructors

    public MonkeyBoardGameoverEventArgs(int levelEndedAt, int levelTotalCount, int livesLeft)
    {
        LevelEndedAt = levelEndedAt;
        LevelTotalCount = levelTotalCount;
        LivesLeft = livesLeft;
    }

    #endregion
}

