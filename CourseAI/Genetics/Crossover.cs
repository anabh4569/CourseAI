using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Genetics
{
    //Other genetic classes include single parent w/ higher mutation, etc
    static class Crossover
    {
        /* Genetic algorithms: variant of stochastic beam (hill climbing), states made by combining two parents.
         * k randomly generated states, called population. Individual states represented as binary
         * strings. Requires some objective/value/fitness function: healthier states are better.
         * 
         * Ex: string "32748552" can represent a state in the 8 queens problem, each column's queen's row.
         * The objective function could be non-attacking pairs; solution state = 28 = (7 + 6 + ... + 1).
         * 
         * Pairs of individuals randomly selected to reproduce at some probabilities. Crossover point
         * chosen at random in the string -> offspring created by crossing parents at that point.
         * Each element in the string subject to some small mutation rate.
         */
        
        public delegate int Fitness(string state);
        //String states -> generates results, returns first to meet fitness goal.
        public static string FindSolution(List<string> population, Fitness test, int fitnessGoal, double mutationRate)
        {
            int sLength;
            //Crossover is really dumb without at least 1 pair, so just no.
            if (population.Count > 1)
            {
                sLength = population[0].Length;
                foreach (string indiv in population)
                    if (sLength != indiv.Length)
                        return null;
            }
            else
                return null;

            List<string> current = new List<string>(population);
            while (true)
            {
                List<string> randoms = new List<string>();
                foreach (string s in current) //delegates need an adapter :(
                    randoms.Add(Helper.WeightedRandom<string>.PickRandom(current, delegate (string a) { return test(a); }));

                Random r = new Random();
                current.Clear();
                for (int i = 0; i < randoms.Count - 1; i += 2)
                {
                    //Cut can be between any letters (even after/before em all)
                    string fusion1 = "";
                    string fusion2 = "";
                    float cutoff = 0.5f + r.Next(sLength + 1);
                    for (int j = 0; j < sLength; j++)
                    {
                        if (j < cutoff)
                        {
                            fusion1 += randoms[i][j];
                            fusion2 += randoms[i + 1][j];
                        }
                        else
                        {
                            fusion1 += randoms[i + 1][j];
                            fusion2 += randoms[i][j];
                        }
                    }
                    current.Add(fusion1);
                    current.Add(fusion2);
                }

                //Now that current has the crossovers, check if any meet the goal!
                for (int i = 0; i < current.Count; i++)
                {
                    if (r.NextDouble() < mutationRate)
                    {
                        char[] letters = current[i].ToArray();
                        letters[r.Next(sLength)] = Char.Parse(r.Next(10).ToString());
                    }
                    string s = current[i];
                    if (test(s) == fitnessGoal)
                        return s;
                }
            }
        }
    }
}
