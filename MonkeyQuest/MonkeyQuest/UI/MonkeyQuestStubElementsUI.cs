using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using MonkeyQuest.Framework.Images;
using MonkeyQuest.Framework.UI;

using MonkeyQuest.Resources;
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