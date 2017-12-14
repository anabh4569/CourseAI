using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Helper
{
    static class WeightedRandom<Item>
    {
        public delegate int Weight(Item item);
        public static Item PickRandom(List<Item> items, Weight weight)
        {
            int sum = 0;
            foreach (Item i in items)
                sum += weight(i);

            int rnd = new Random().Next(sum);
            foreach (Item i in items)
            {
                int w = weight(i);
                if (rnd < w)
                    return i;
                rnd -= w;
            }
            //Should never come here.
            return default(Item);
        }
    }
}
