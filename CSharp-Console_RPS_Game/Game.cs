using System.Runtime.InteropServices;

namespace CSharp_Console_RPS_Game
{
    internal class Game
    {
        // Stuff To Cancel Input --------------------------------------------------------
        private const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
        // Fields -----------------------------------------------------------------------
        private static Player[] _players = new Player[2];
        private static bool _cpu = false;
        private static int[,] _result = new int[3, 3]; // Doing This As An Array Instead Of A List
        private static string[] _signs = new string[3]; // Because I'm Not Planning To Expand To More Signs ATM
        private static bool _signAnswer = false;
        private static int _maxScore = -1;
        private static bool _inputInterrupted;
        // Properties -------------------------------------------------------------------
        private static Player[] Players
        {
            get { return _players; }
            set { _players = value; }
        }
        private static bool CPU
        {
            get { return _cpu; }
            set { _cpu = value; }
        }
        private static int[,] Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private static string[] Signs
        {
            get { return _signs; }
            set { _signs = value; }
        }
        private static bool SignAnswer
        {
            get { return _signAnswer; }
            set { _signAnswer = value; }
        }
        private static int MaxScore
        {
            get { return _maxScore; }
            set { _maxScore = value; }
        }
        private static bool InputInterrupted
        {
            get { return _inputInterrupted; }
            set { _inputInterrupted = value; }
        }

        // Methods ----------------------------------------------------------------------
        internal static void Init()
        {
            // Sign Setup ---------------------------------------------------------------

            if (!SignAnswer) // So You Only Have To Answer That Once
            {
                Console.Clear();
                Console.WriteLine(AppResource.Setup);

                SetSigns(Utility.Decision($"Can You See These Emotes {{🗿📄✂️}}"));
                SignAnswer = true;
            }

            // Single Or Multilayer -----------------------------------------------------

            Console.Clear();
            Console.WriteLine(AppResource.Setup); // Declaring That THIS Is The Setup Phase 😄

            CPU = Utility.Decision("Play VS Computer Or Humanoid?", ConsoleKey.C, ConsoleKey.H);

            // Player Setup -------------------------------------------------------------

            List<int> controlOptions = new(new[] { 0, 1, 2, 3 }); // Init ArrayList With Values

            for (int i = 0; i < Players.Length; i++) // It's Two | Running Twice | For Each Player
            {
                SetPlayers(i, controlOptions);
            }

            Console.Clear();

            // Deciding On The Max Goal
            SetScoreGoal("How Many Points To Win ( 1 - 9 )");

            // Getting The Result Array R2G
            SetResults();
        }

        internal static void Start()
        {
            for (int i = 0; i < MaxScore;)
            {
                if (CPU)
                {
                    SinglePlayer(ref i);
                }
                else
                {
                    MultiPlayer(ref i);
                }
            }
            GetWinner();
        }

        private static void SinglePlayer(ref int rounds)
        {
            ConsoleKey[] key = new ConsoleKey[2];

            do
            {
                GetCountdown();

                Task.Run(() => InputManageSinglePlayer(ref key));

                Task.Delay(2100).ContinueWith(_ =>
                {
                    InterruptInput();
                });

                Thread.Sleep(2400); // How Much Time The Player:s Have For Input

                if (!Players[0].HasPressed)
                {
                    InputFailed();
                }
            } while (!Players[0].HasPressed);

            Random rnd = new(); // Randomizer For CPU Selection
            key[1] = Players[1].Control[rnd.Next(3)]; // CPU Input

            SetScore(key);

            GetScore(key);
            rounds = Math.Max(Players[0].Score, Players[1].Score); // Dunno How I Missed This 😂

        }

        private static void MultiPlayer(ref int rounds)
        {
            ConsoleKey[] key = new ConsoleKey[2];

            do
            {
                GetCountdown();

                Task.Run(() => InputManageMultiPlayer(ref key));

                Task.Delay(2100).ContinueWith(_ =>
                {
                    InterruptInput();
                });

                Thread.Sleep(2400); // How Much Time The Player:s Have For Input

                if (!Players[0].HasPressed || !Players[1].HasPressed)
                {
                    InputFailed();
                }
            } while (!Players[0].HasPressed || !Players[1].HasPressed);

            /*
             * Because We Have Different And Also Optional Input Options
             * I Have To Figure Out Who Pressed First So I Can Compare The Right Keys
             * Otherwise We'll Land Outta Bounds
             */

            SetScore(key);

            GetScore(key);
            rounds = Math.Max(Players[0].Score, Players[1].Score); // Dunno How I Missed This 😂
        }

        private static void InputManageSinglePlayer(ref ConsoleKey[] key)
        {
            Players[0].HasPressed = false;
            try
            {
                key[0] = Console.ReadKey(true).Key;
            }
            catch (InvalidOperationException)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }

            for (int c = 0; c < Players[0].Control.Length; c++) // Should Always Be 3 Times
            {
                if (key[0] == Players[0].Control[c])
                {
                    Players[0].HasPressed = true;
                }
            }
        }

        private static void InputManageMultiPlayer(ref ConsoleKey[] key)
        {
            for (int i = 0; i < Players.Length; i++) // Resetting Who Pressed
            {
                Players[i].HasPressed = false;
            }
            Players[0].PressedFirst = false;

            for (int p = 0; p < Players.Length; p++) // For Each Player | Twice
            {
                try
                {
                    key[p] = Console.ReadKey(true).Key;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                for (int c = 0; c < Players[p].Control.Length; c++) // Should Always Be 3 Times
                {
                    if (key[p] == Players[0].Control[c])
                    {
                        Players[0].HasPressed = true;
                        if (p == 0)
                        {
                            Players[0].PressedFirst = true;
                        }
                    }
                    if (key[p] == Players[1].Control[c])
                    {
                        Players[1].HasPressed = true;
                    }
                }
            }
        }

        private static void InterruptInput()
        {
            InputInterrupted = false; // Reset

            var handle = GetStdHandle(STD_INPUT_HANDLE);
            bool success = CancelIoEx(handle, IntPtr.Zero);
            if (success)
            {
                InputInterrupted = true;
            }
        }

        private static void InputFailed()
        {
            Console.Clear();

            Console.WriteLine(AppResource.Hint);

            if (!Players[0].HasPressed && !Players[1].HasPressed)
            {
                Console.WriteLine($"{{{Players[0].Name}}} And {{{Players[1].Name}}} Failed To Select In Time");
            }
            else if (!Players[0].HasPressed)
            {
                Console.WriteLine($"{{{Players[0].Name}}} Has Not Selected In Time");
            }
            else
            {
                Console.WriteLine($"{{{Players[1].Name}}} Has Not Selected In Time");
            }

            if (InputInterrupted)
            {
                Console.Write(Environment.NewLine + "Press Enter Twice To Reset Input Buffer");
            }
            else
            {
                Console.Write(Environment.NewLine + "Press Enter To Reset Input Buffer");
            }
            ConsoleKey buffer;
            do
            {
                buffer = Console.ReadKey(true).Key;
            } while (buffer != ConsoleKey.Enter);
        }

        private static void SetPlayers(int i, List<int> controlOptions)
        {
            if (i == 1 && CPU)
            {
                Players[i] = new("CPU", -1, true); // True == IsCPU
            }
            else
            {
                int control; // Fixed My Previous Confusion 😂
                do
                {
                    Players[i] = new(SetUserName($"Enter UserName For Player {i + 1}: "),
                        SetControl($"Press Corresponding Key For Player {i + 1} Settings", controlOptions, out control));
                    GetPlayerSettings(i);
                } while (!Utility.Decision("Confirm Settings?"));
                controlOptions.Remove(control);
            }
        }

        private static string SetUserName(string msg)
        {
            Console.CursorVisible = true;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(AppResource.Setup);

                Console.Write(msg);

                string? input = Console.ReadLine();

                if (!String.IsNullOrEmpty(input))
                {
                    if (input.Length >= 3 && input.Length <= 18)
                    {
                        Console.CursorVisible = false;
                        Console.WriteLine();
                        return _ = char.ToUpper(input[0]) + input[1..]; // IntelliSense Was Here
                    }
                }
                Utility.InvalidInput(3, 18, 2700);
            }
        }

        private static int SetControl(string msg, List<int> controlOptions, out int control) // Selecting Control Map
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine(AppResource.Setup);

                Console.WriteLine("Available Control Key Maps");

                GetCtrlOptions(controlOptions);
                Console.Write(msg);

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (int.TryParse(key.KeyChar.ToString(), out control)) // If It Is A Number
                {
                    if (controlOptions.Contains(control)) // And This Number Is A Valid Option
                    {
                        return control;
                    }
                }
                Utility.InvalidInput();
            }
        }

        private static void SetScoreGoal(string msg) // How Many Points Till Victory
        {
            Console.WriteLine(AppResource.Setup);
            while (true)
            {
                Console.WriteLine(msg);

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (int.TryParse(key.KeyChar.ToString(), out int value) && key.KeyChar != '0')
                {
                    Console.WriteLine();
                    MaxScore = Math.Abs(value);

                    return;
                }
                Console.WriteLine(Environment.NewLine +
                    "Invalid Input. Make Sure To Select A Key Between 1 - 9" +
                    Environment.NewLine);
            }
        }

        private static void SetScore(ConsoleKey[] key)
        {
            if (CPU)
            {
                if (Result[Array.IndexOf(Players[0].Control, key[0]),
                    Array.IndexOf(Players[1].Control, key[1])] > 0)
                {
                    Players[0].Score++;
                }
                else if (Result[Array.IndexOf(Players[0].Control, key[0]),
                    Array.IndexOf(Players[1].Control, key[1])] < 0)
                {
                    Players[1].Score++;
                }
            }
            else
            {
                if (Players[0].PressedFirst)
                {
                    if (Result[Array.IndexOf(Players[0].Control, key[0]),
                        Array.IndexOf(Players[1].Control, key[1])] > 0)
                    {
                        Players[0].Score++;
                    }
                    else if (Result[Array.IndexOf(Players[0].Control, key[0]),
                        Array.IndexOf(Players[1].Control, key[1])] < 0)
                    {
                        Players[1].Score++;
                    }
                }
                else
                {
                    if (Result[Array.IndexOf(Players[1].Control, key[0]),
                        Array.IndexOf(Players[0].Control, key[1])] > 0)
                    {
                        Players[1].Score++;
                    }
                    else if (Result[Array.IndexOf(Players[1].Control, key[0]),
                        Array.IndexOf(Players[0].Control, key[1])] < 0)
                    {
                        Players[0].Score++;
                    }
                }
            }
        }

        private static void SetSigns(bool usingTerminal)
        {
            if (usingTerminal)
            {
                Signs[0] = "🗿";
                Signs[1] = "📄";
                Signs[2] = "✂️";
            }
            else
            {
                Signs[0] = "Rock";
                Signs[1] = "Paper";
                Signs[2] = "Scissors";
            }
        }

        private static void SetResults() // Generating Result Array
        {
            int value = 0;
            for (int take = 0; take < Result.GetLength(0); take++)
            {
                for (int vs = 0; vs < Result.GetLength(1); vs++)
                {
                    // 0 -1 1 => 1 0 -1 => -1 1 0
                    Result[take, vs] = value;
                    if (vs < Result.GetLength(1) - 1) // So The Last Number Will Be The First Next Time Around
                    {
                        value--;
                    }
                    if (value < -1) // Resetting The Number
                    {
                        value = 1;
                    }
                }
            }
        }

        private static void GetCountdown() // Countdown
        {
            Console.Clear();

            Console.Write(AppResource.Ready);
            Thread.Sleep(1500);
            Console.Clear();

            Console.Write(AppResource._3);
            Thread.Sleep(1000);
            Console.Write(AppResource._2);
            Thread.Sleep(1000);
            Console.Write(AppResource._1);
        }

        private static void GetCtrlOptions(List<int> controlOptions)
        {
            for (int i = 0; i < controlOptions.Count; i++)
            {
                switch (controlOptions[i])
                {
                    case 0:
                        Console.WriteLine("{0,-42}{1,-3}", "For {Q, W, E}", "| 0");
                        break;
                    case 1:
                        Console.WriteLine("{0,-42}{1,-3}", "For {J, K, L}", "| 1");
                        break;
                    case 2:
                        Console.WriteLine("{0,-42}{1,-3}", "For {LeftArrow, DownArrow, RightArrow}", "| 2");
                        break;
                    case 3:
                        Console.WriteLine("{0,-42}{1,-3}", "For {Num_4, Num_5, Num_6}", "| 3");
                        break;
                }
            }
            Console.WriteLine();
        }

        private static void GetPlayerSettings(int i)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Player {i + 1} Settings" + Environment.NewLine);
            Console.WriteLine("{0,-18}{1,0}", "UserName: ", Players[i].Name);

            for (int ctrl = 0; ctrl < Players[i].Control.Length; ctrl++)
            {
                switch (ctrl)
                {
                    case 0:
                        Console.WriteLine("{0,-18}{1,0}", $"{Signs[0]}:", Convert.ToString(Players[i].Control[ctrl]));
                        break;
                    case 1:
                        Console.WriteLine("{0,-18}{1,0}", $"{Signs[1]}:", Convert.ToString(Players[i].Control[ctrl]));
                        break;
                    case 2:
                        Console.WriteLine("{0,-18}{1,0}", $"{Signs[2]}:", Convert.ToString(Players[i].Control[ctrl]));
                        break;
                }
            }
            Console.WriteLine();
        }

        private static void GetScore(ConsoleKey[] key) // Displays Score
        {
            Console.Clear();
            Console.WriteLine(AppResource.Result);
            Console.WriteLine(new String('-', 72) + Environment.NewLine);

            string p1Press, p2Press;

            if (Players[0].PressedFirst || CPU)
            {
                p1Press = Signs[Array.IndexOf(Players[0].Control, key[0])];
                p2Press = Signs[Array.IndexOf(Players[1].Control, key[1])];
            }
            else
            {
                p1Press = Signs[Array.IndexOf(Players[0].Control, key[1])];
                p2Press = Signs[Array.IndexOf(Players[1].Control, key[0])];
            }

            Console.WriteLine($"Player: {{{Players[0].Name}}}");
            Console.WriteLine("Attack: " + p1Press);
            Console.WriteLine("Points: " + Players[0].Score);

            Console.WriteLine();

            Console.WriteLine($"Player: {{{Players[1].Name}}}");
            Console.WriteLine("Attack: " + p2Press);
            Console.WriteLine("Points: " + Players[1].Score);

            Console.WriteLine();

            Console.WriteLine($"Score Needed To Win {MaxScore}");

            Thread.Sleep(3000);
        }

        private static void GetWinner() // Displays Winner
        {
            Console.Clear();
            Console.WriteLine(AppResource.Conclusion);

            if (Players[0].Score == MaxScore)
            {
                Console.WriteLine($"Player One {{{Players[0].Name}}} Has Won" +
                    Environment.NewLine);
            }
            else
            {
                Console.WriteLine($"Player Two {{{Players[1].Name}}} Has Won" +
                    Environment.NewLine);
            }
        }
    }
}
