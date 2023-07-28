using MonkeyQuest.Framework.Logic;

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