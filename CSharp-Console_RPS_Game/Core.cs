using System.Text;

namespace CSharp_Console_RPS_Game
{
    internal class Core
    {
        static void Main(string[] args)
        {
            // App Start ----------------------------------------------------------------

            Console.Title = "Rock Paper Scissors - Console Application"; // Just Realize That This Command Exists
            Console.OutputEncoding = Encoding.UTF8; // Because Emotes
            Console.CursorVisible = false; // It's Ugly And Only Useful For Non-Direct Input
            Utility.Intro();

            // Want Some Music ❓ -------------------------------------------------------

            if (Utility.Decision("Do You Want To Enable Music?"))
            {
                Utility.Music(); // Looping Background Music #Royalty Free 😎
            }
            // Actual Application -------------------------------------------------------
            do
            {
                // Setup Phase ❕ --------------------------------------------------------
                Game.Init();

                // Spicy Phase 😋 -------------------------------------------------------
                Game.Start();
            } while (Utility.Decision("Do You Want To Play Again?"));

            Utility.Exit();

            Environment.Exit(0); // Clean Exit
        }
    }
}