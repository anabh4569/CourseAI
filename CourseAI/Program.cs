using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAI;

namespace CourseAI
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Console.WriteLine("Solution: " +Genetics.Crossover.FindSolution(pop, Queens, 28, 0.2));
            Console.ReadKey();
        }

        static int Queens(string state)
        {
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
}
