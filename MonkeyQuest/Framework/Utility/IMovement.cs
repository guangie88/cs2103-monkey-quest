// Main Contributors: Weiguang

using System;

using MonkeyQuest.Framework.Logic;

namespace MonkeyQuest.Framework.Utility
{
    public interface IMovement
    {
        bool IsActive { get; }
        void ChangeMotion<TBoard>(Movable<TBoard> movable) where TBoard : Board<TBoard>;
    }
}
