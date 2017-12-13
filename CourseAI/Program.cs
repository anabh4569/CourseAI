using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseAI.Graphs;

namespace CourseAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, string> test = new Dictionary<int, string>
            {
                { 0, "dog" },
                { 0, "cat" } //throws error if multiple are added!
            };
            foreach (int i in test.Keys)
                Console.WriteLine(test[i]);
            Console.ReadLine();
        }
    }
}
