using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAI.SoloStates;

namespace CourseAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(default(float));
            Console.ReadKey();
        }

        static int Queens(string state)
        {
            /*
             * Main: usage code
            Random r = new Random();
            List<string> pop = new List<string>
            {
                "13572684",
                "24748552",
                "24415124",
                "32752411",
            };
            for (int i = 0; i < 1000; i++)
            {
                String state = "";
                for (int j = 0; j < 8; j++)
                {
                    state += r.Next(8) + 1;
                }
                pop.Add(state);
            }
            foreach (string state in pop)
                Console.WriteLine(Queens(state));
            Console.WriteLine("Solution: " + Genetics.Crossover.FindSolution(pop, Queens, 28, 0.2));
            */


            int freePairs = 0;
            if (state.Length != 8)
                return freePairs;
            int[] vals = new int[8];
            for (int i = 0; i < vals.Length; i++)
            {
                if (!Int32.TryParse(state[i].ToString(), out vals[i]))
                    return freePairs;
            }

            for (int i = 0; i < vals.Length; i++)
            {
                for (int j = i + 1; j < vals.Length; j++)
                {
                    int a = vals[i];
                    int b = vals[j];
                    if (a != b && Math.Abs(a - b) != j - i)
                        freePairs++;
                }
            }

            return freePairs;
        }
    }

    sealed class TicTacState : IState<TicTacState>
    {
        /* Usage example (not complete)
            TicTacState current = new TicTacState();
            TicTacState next;
            while (true)
            {
                Console.WriteLine(current);
                next = Adversarial<TicTacState>.Minimax(current, true);
                if (current.Equals(next))
                    break;
                current = next;

            }
         */


        /* We'll see if interning is necessary to make this not slow!
         * Make changes in the 2 interface methods.
        //Privately uses interning to save computational time
        private static TicTacState Intern(TicTacState yourCopy)
        {
            return null;
        }

        private static Dictionary<TicTacState, TicTacState> intern;
        if we already have its value, should not give children (terminal state -> already calced!)
        private static Dictionary<TicTacState, double> value; -> change up value method to intern this too */

        //Publicly creates multiple copies of this starter board (check if interning)
        public TicTacState() : this(new int[3,3]) { }

        private TicTacState(int[,] board)
        {
            if (board == null || board.GetLength(0) != 3 || board.GetLength(1) != 3)
                throw new ArgumentException();
            this.board = new int[3, 3];
            Array.Copy(board, this.board, 9);
            //intern = new Dictionary<TicTacState, TicTacState>();
            //+ make this a copy somehow
        }

        private int[,] board; //[rows (0 at top), cols]; markings: (0 empty, 1 p1, 2 p2)



        public IReadOnlyCollection<TicTacState> GetChildren()
        {
            List<TicTacState> states = new List<TicTacState>();

            try
            {
                Value();
                return states;
            }
            catch (InvalidOperationException)
            {
                
            }

            int ones = 0;
            int twos = 0;
            for (int i = 0; i < board.Length; i++)
            {
                int val = board[i / 3, i % 3];
                if (val == 1)
                    ones++;
                else if (val == 2)
                    twos++;
            }
            int toPlace = (ones <= twos) ? 1 : 2;

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i / 3, i % 3] == 0)
                {
                    board[i / 3, i % 3] = toPlace;
                    states.Add(new TicTacState(board));
                    board[i / 3, i % 3] = 0;
                }
            }
            return states;
        }

        public double Value()
        {
            //throw exception if not terminal or in interning system
            for (int i = 1; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (CheckVals(i, board[j, 0], board[j, 1], board[j, 2]) || CheckVals(j, board[0, j], board[1, j], board[2, j]))
                    {
                        return (i == 1) ? 1 : -1;
                    }
                }
                if (CheckVals(i, board[0, 0], board[1, 1], board[2, 2]) || CheckVals(i, board[2, 0], board[1, 1], board[0, 2]))
                {
                    return (i == 1) ? 1 : -1;
                }
            }
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i / 3, i % 3] == 0)
                    throw new InvalidOperationException();
            }
            return 0;
        }

        private bool CheckVals(int standard, params int[] vals)
        {
            foreach (int i in vals)
                if (i != standard)
                    return false;
            return true;
        }

        public override string ToString()
        {
            string output = "[";
            output += "[" + board[0, 0] + ", " + board[0, 1] + ", " + board[0, 2] + "]\n";
            output += " [" + board[1, 0] + ", " + board[1, 1] + ", " + board[1, 2] + "]\n";
            output += " [" + board[2, 0] + ", " + board[2, 1] + ", " + board[2, 2] + "]]";
            return output;
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(GetType()))
                return false;
            int[,] oBoard = ((TicTacState)obj).board;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != oBoard[i, j])
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;
            for (int i = 0; i < board.Length; i++)
            {
                sum += (int)(Math.Pow(i, i)) * board[i / 3, i % 3];
            }
            return sum;
        }
    }
}