// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.UI;

namespace MonkeyQuest.Framework.Utility
{
    #region Delegates

    #region Positional delegates

    public delegate void PositionChangedEventHandler<TBoard>(object sender, PositionsEventArgs e) where TBoard : Board<TBoard>;

    #endregion

    #region Movable delegates

    public delegate void AfterMotionEventHandler<TBoard>(object sender, MovableEventArgs<TBoard> e) where TBoard : Board<TBoard>;

    #endregion

    #region AI delegates

    public delegate void CollisionEventHandler<TBoard>(object sender, CollisionEventArgs<TBoard> e) where TBoard : Board<TBoard>;

    #endregion 

    #region Board delegates

    public delegate void BoardResizeEventHandler(object sender, BoardResizeEventArgs e);
    public delegate void BoardTickEventHandler(object sender, EventArgs e);
    public delegate void BoardKeyEventHandler<TBoard>(object sender, BoardKeyEventArgs<TBoard> e) where TBoard : Board<TBoard>;
    public delegate void AddingPositionalEventHandler<TBoard>(object sender, PositionalEventArgs<TBoard> e) where TBoard : Board<TBoard>;
    public delegate void RemovingPositionalEventHandler<TBoard>(object sender, PositionalEventArgs<TBoard> e) where TBoard : Board<TBoard>;
    public delegate void TempRemovingPositionalEventHandler<TBoard>(object sender, PositionalEventArgs<TBoard> e) where TBoard : Board<TBoard>;
    public delegate void RestoringPositionalEventHandler<TBoard>(object sender, PositionalEventArgs<TBoard> e) where TBoard : Board<TBoard>;
    public delegate void BoardFadeEventHandler<TBoard>(object sender, PositionalEventArgs<TBoard> e) where TBoard : Board<TBoard>;

    #endregion

    #region BoardUI delegates

    public delegate void BoardUIResizeEventHandler(object sender, BoardUIResizeEventArgs e);

    #endregion

    #endregion
}