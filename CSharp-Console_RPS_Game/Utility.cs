using System.Media; // Sound Player Stuff

namespace CSharp_Console_RPS_Game
{
    internal static class Utility
    {
        // Fields -----------------------------------------------------------------------
        // Properties -------------------------------------------------------------------
        // Methods ----------------------------------------------------------------------
        internal static void Intro()
        {
            Thread.Sleep(369);
            Console.Write(AppResource.Rock);
            Thread.Sleep(369);
            Console.Write(AppResource.Paper);
            Thread.Sleep(369);
            Console.WriteLine(AppResource.Scissors);
            Thread.Sleep(999);
        }

        internal static void Music() // Starts The Music Or At Least That's The Assumption 😉
        {
            SoundPlayer music = new() // Creating An Object And Setting Sound Path
            {
                Stream = AppResource.bg_music
            };
            music.PlayLooping();
        }

        internal static bool Decision(string msg, ConsoleKey option1 = ConsoleKey.Y, ConsoleKey option2 = ConsoleKey.N) // I Should've Used Default Values For Y | N 🙈
        {
            ConsoleKeyInfo ckey;
            bool fail = false;

            do
            {
                if (fail) // Which Will Always Be True Except For The First Time
                {
                    Console.WriteLine(Environment.NewLine +
                        $"Only Use {option1} Or {option2} Key" +
                        Environment.NewLine);
                }

                Console.Write(msg + $" ( {option1} | {option2} )" + Environment.NewLine); // Printing Message
                ckey = Console.ReadKey(true); // Reading SetUserName

                fail = true; // Won't Matter If The Condition Has Been Met

            } while (ckey.Key != option1 && ckey.Key != option2); // Same Check Less Writing
            /* If You Scout The Previous Version Of This File You'll See That I Figured <IT> Out
             * What Do I Mean By <THAT>, Well, You'll Have To Figure <That> One Out Yourself 😉
             * And Yes I Did Know It Was Possible But I Wasn't Sure How 🙈 Until Now
             */
            return ckey.Key == option1;
        }

        internal static void InvalidInput(int min = 0, int max = 0, int dur = 1800)
        {
            Console.Clear();
            Console.WriteLine(AppResource.Invalid);

            if (min == 0 || max == 0)
            {
                Console.WriteLine($"Make Sure That Your Input Correct");
            }
            else
            {
                Console.WriteLine($"Make Sure That Your Input Is Between {min} And {max} Chars");
            }

            Thread.Sleep(dur);
        }

        internal static void Exit()
        {
            Console.Clear();
            Console.WriteLine(AppResource.Bye);
            Thread.Sleep(1500);
        }
    }
}
