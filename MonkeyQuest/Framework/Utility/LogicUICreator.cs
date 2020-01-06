// Main Contributors: Weiguang (not used yet)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.UI;
using MonkeyQuest.Framework.Images;

namespace MonkeyQuest.Framework.Utility
{
    public delegate PositionalUI<TBoard> LogicUICreator<TBoard, in TPositional>(TPositional logic)
        where TBoard : Board<TBoard>
        where TPositional : Positional<TBoard>;

    public sealed class LogicUICreatorManager<TBoard>
        where TBoard : Board<TBoard>
    {
        #region Data Fields

        private SortedDictionary<string, object> logicNameToConverter;

        #endregion

        #region Properties

        public SortedDictionary<string, object> LogicNameToConverter
        {
            get { return logicNameToConverter; }
            private set { logicNameToConverter = value; }
        }

        #endregion

        #region Methods

        public bool Add(string logicType, object converter)
        {
            LogicNameToConverter.Add(logicType, converter);
            return true;
        }

        public PositionalUI<TBoard> Convert<TPositional>(TPositional logic)
            where TPositional : Positional<TBoard>
        {
            // using dynamic keyword to perform the casting, a new feature in C# 4.0
            string logicName = logic.LogicIdentifier;
            dynamic converterDynamic = logicNameToConverter[logicName];
            dynamic logicDynamic = logic;

            return converterDynamic(logicDynamic);
        }

        #endregion

        #region Constructors

        public LogicUICreatorManager()
        {
            logicNameToConverter = new SortedDictionary<string, object>();
        }

        #endregion
    }
}
