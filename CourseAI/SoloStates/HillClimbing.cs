using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.SoloStates
{
    static class HillClimbing
    {
        //Redoing the hill climbing algorithm from Graphs.Searcher
        public static State LocalMax<State>(State start) where State : IState<State>
        {
            State current = start;
            List<State> children = new List<State>(current.GetChildren());
            while (children.Count > 0)
            {
                children.Sort(delegate (State a, State b) { return b.Value().CompareTo(a.Value()); });
                if (current.Value() >= children[0].Value())
                    break;
                current = children[0];
                children = new List<State>(current.GetChildren());
            }
            return current;
        }
    }
}
