using System.Windows.Controls;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.UI;
using MonkeyQuest.Resources.Utility;
using MonkeyQuest.MonkeyQuest.Logic;


namespace MonkeyQuest.MonkeyQuest.UI
{
    public class StubUI : PositionalUI<MonkeyBoard>
    {
        public StubUI(Positional<MonkeyBoard> logic, BoardUI<MonkeyBoard> boardUI)
            : base(logic, new Image() { Source = ImageManager.ElementImageDictionary["Stub"] }, boardUI)
        {

        }
    }
}