using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Helper
{
    static class Randomness<Item>
    {
        public delegate int Weight(Item item);
        public static List<Item> WeightedPicks(List<Item> items, Weight weight, int count)
        {
            int sum = 0;
            foreach (Item i in items)
                sum += weight(i);

            Random r = new Random();
            List<Item> output = new List<Item>();

            for (int i = 0; i < count; i++)
            {
                int rnd = r.Next(sum);
                foreach (Item it in items)
                {
                    int w = weight(it);
                    if (rnd < w)
                    {
                        output.Add(it);
                        break;
                    }
                    rnd -= w;
                }
            }

            return output;
        }
    }
}
