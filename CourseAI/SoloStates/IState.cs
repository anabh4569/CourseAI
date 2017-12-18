using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.SoloStates
{
    //Represents some state that can point to/create children and has a heuristic/value/objective/utility.
    interface IState<T> where T : IState<T> //Type parameters can enforce T = typeof(this)
    {
        //Termainal state if no children
        IReadOnlyCollection<T> GetChildren();
        //To be maxed by p1 and min'd by p2; 
        double Value();
    }
}
