// Main Contributors: Everyone (fair share)

using System;
using System.Windows;
using MonkeyQuest.MonkeyQuest.UI;

namespace MonkeyQuestMain
{
    public static class MonkeyQuestMain
    {
        // Load XML map
        // you need to 'Rebuild Solution' so that the xml will be output the the debug directory
        private static string[] mapPathArray = { "Resources/Maps/Level1.xml", "Resources/Maps/Level2.xml", "Resources/Maps/Level3.xml", "Resources/Maps/Level4.xml", "Resources/Maps/Level5.xml", "Resources/Maps/Level6.xml", "Resources/Maps/Level7.xml", "Resources/Maps/Level8.xml", "Resources/Maps/Level9.xml", "Resources/Maps/Level10.xml" };

        private static MonkeyBoardUIWindow window;
        private static MonkeyBoardPackage boardPackage;

        private static void GameoverWithNoLivesBinding(object sender, MonkeyBoardGameoverEventArgs e)
        {
            GameoverWindow gameoverWindow = new GameoverWindow();

            // pause the current map first
            boardPackage.Pause();

            gameoverWindow.GameoverDecision += (gameoverSender, gameoverArgs) =>
            {
                if (gameoverArgs.GameoverState == GameoverState.TryAgain)
                {
                    // restarts everything
                    boardPackage.GameoverWithNoLives -= GameoverWithNoLivesBinding;
                    window.ClearBoardPackage();

                    boardPackage = new MonkeyBoardPackage(mapPathArray, new MonkeyBoardUI(null), new MonkeyMainBarUI(null), true);
                    boardPackage.GameCompleted += (gameCompletedSender, gameCompletedArgs) => new GameCompletedWindow().ShowDialog();
                    window.SetBoardPackage(boardPackage);

                    boardPackage.GameoverWithNoLives += GameoverWithNoLivesBinding;
                    boardPackage.Start();
                }
                else if (gameoverArgs.GameoverState == GameoverState.Quit)
                {
                    // quits game
                    window.Close();
                }
            };

            gameoverWindow.ShowDialog();
        }

        [STAThread]
        public static void Main()
        {
            boardPackage = new MonkeyBoardPackage(mapPathArray, new MonkeyBoardUI(null), new MonkeyMainBarUI(null), true);
            window = new MonkeyBoardUIWindow(boardPackage);

            // the first binding and this binding continues for every other board package reloaded
            boardPackage.GameCompleted += (gameCompletedSender, gameCompletedArgs) => new GameCompletedWindow().ShowDialog();
            boardPackage.GameoverWithNoLives += GameoverWithNoLivesBinding;

            Application app = new Application();

            // v0.15 here
            //MessageBox.Show("Welcome to MonkeyQuest v0.15! =)");
            //MessageBox.Show("You will be controlling a monkey and your objective is to collect all the bananas on the map and then proceed to the door that will mysteriously appear.");
            //MessageBox.Show("You can move left right by pressing the left and right arrows.");
            //MessageBox.Show("You can also jump by pressing either spacebar, or left shift.");
            //MessageBox.Show("Another cool feature is that you can wall jump; you need to be exactly beside a tile and press jump in order to do so.");
            //MessageBox.Show("Pressing up key while wall jumping will allow you to jump higher, but with less propulsion. This is useful in certain cases.");
            //MessageBox.Show("If you don't press the up key, then the monkey will perform a normal wall jump, with less height and more propulsion.");
            //MessageBox.Show("Both wall jumps can allow the monkey to reach places that can't be accessed if using only normal jumps.");
            //MessageBox.Show("If you ever get stuck in the game, you can press 'R' to restart.");
            //MessageBox.Show("There are other game features that have not been stated here, so feel free to explore!");
            //MessageBox.Show("There are 4 maps in all and enjoy your game!");

            // start the timer
            boardPackage.Start();
            app.Run(window);
        }
    }
}