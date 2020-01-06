// Main Contributors: Everyone (fair share)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;

using MonkeyQuest.MonkeyQuest.Logic;
using MonkeyQuest.MonkeyQuest.UI;
using MonkeyQuest.Test;

namespace Tester
{
    public static class TestMain
    {
        [STAThread]
        public static void Main()
        {
            // Load XML map
            // you need to 'Rebuild Solution' so that the xml will be output the the debug directory


            //Tests:
                //1st map
                    //-Objects are being added succesfully.
                    //-Collidables are colliding.
                    //-Organics a la AI (& by extension monkeys etc) are killed & removed when impacted from above by other collidables.
                    //-Gravitational fields are functional.
                    //-Anti-Gravitional fields work :P (if bananas can hold the crates up) a.k.a. objects on board that are not bound to gravity don't have all motions thrust upon them blindly.

                //2nd map
                    //In addition to most from above
                    //-Monkey collects bananas, which are removed from board.

                //3rd map
                    //In addition to most tests from above
                    //-Controllables [monkey] can push crates left & right unless obstructed by another collidable
                    //-Controllables [monkey] dont die unless in contact with AI
                    //-AI doesn't die unless impacted from above by crate or controllable

                //4th map
                    //In addition to (1st & 2nd map tests)
                    //Monkey collecting all bananas leads to emergence of door
                    //Collecting all bananas (i.e. controllable fulfilling specified objectives) reveals door a.k.a means to progress to next level.
                    //Collision with door results in signal for next level
                    //Weapons kill the monnnstaaaaar! AI is removed when weapons collide into them.

            string[] mapPathArray = { "Resources/Maps/Test.xml", "Resources/Maps/CollisionTestMap.xml", "Resources/Maps/CrateTestMap.xml", "Resources/Maps/DoorTestMap.xml" };
            TestPackage boardPackage = new TestPackage(mapPathArray, new MonkeyBoardUI(null), new MonkeyMainBarUI(null), true);

            Application app = new Application();
            TestUIWindow window;

            // creates the UI for window
            window = new TestUIWindow(boardPackage);

            // start the timer
            boardPackage.Start();

            app.Run(window);
            System.Diagnostics.Debug.WriteLine("Board Debug");

        }
    }
}
