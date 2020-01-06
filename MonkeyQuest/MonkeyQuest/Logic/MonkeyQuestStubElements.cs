using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Media;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public class Stub : Collidable<MonkeyBoard>
    {
        public Stub()
            : base()
        {
            // CollidingMode = CollidingMode.Exclusion;
            CollidingMode = CollidingMode.Inclusion;
            // CollidingExclusionSet.Add(typeof(Monkey));
            CollidingInclusionSet.Add(typeof(Monkey));
        }
    }
}