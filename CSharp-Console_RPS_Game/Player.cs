namespace CSharp_Console_RPS_Game
{
    internal class Player
    {
        // Fields -----------------------------------------------------------------------
        private ConsoleKey[] _control = new ConsoleKey[3];
        private string _name = String.Empty;
        private int _score = 0;
        private bool _pressedKey;
        private bool _pressedFirst;
        // Properties -------------------------------------------------------------------
        internal ConsoleKey[] Control
        {
            get { return _control; }
            set { _control = value; }
        }
        internal string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        internal int Score
        {
            get { return _score; }
            set { _score = value; }
        }
        internal bool HasPressed
        {
            get { return _pressedKey; }
            set { _pressedKey = value; }
        }
        internal bool PressedFirst
        {
            get { return _pressedFirst; }
            set { _pressedFirst = value; }
        }
        // Methods ----------------------------------------------------------------------
        internal Player(string Name, int control, bool isCPU = false) // Constructor 🧿
        {
            // Assigning UserName:s (Not Blocking Bad Words Because NO)
            this.Name = Name;
            // Setting Up Controls For The Player:s
            switch (control)
            {
                case 0:
                    Control[0] = ConsoleKey.Q;
                    Control[1] = ConsoleKey.W;
                    Control[2] = ConsoleKey.E;
                    break;
                case 1:
                    Control[0] = ConsoleKey.J;
                    Control[1] = ConsoleKey.K;
                    Control[2] = ConsoleKey.L;
                    break;
                case 2:
                    Control[0] = ConsoleKey.LeftArrow;
                    Control[1] = ConsoleKey.DownArrow;
                    Control[2] = ConsoleKey.RightArrow;
                    break;
                case 3:
                    Control[0] = ConsoleKey.NumPad4;
                    Control[1] = ConsoleKey.NumPad5;
                    Control[2] = ConsoleKey.NumPad6;
                    break;
                default:
                    Control[0] = ConsoleKey.V;
                    Control[1] = ConsoleKey.B;
                    Control[2] = ConsoleKey.N;
                    break;
            }
            if (isCPU)
            {
                this._pressedKey = true;
            }
        }
    }
}
