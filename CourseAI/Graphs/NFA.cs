using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Graphs
{
    sealed class NFA
    {
        //Very INCOMPLETE class; finish: epsilon closure, concat/union/star/dot/single, read

        public const char EPSILON = '-';

        private Graph<FiniteState, char> states; //embedded states and transitions into this
        private FiniteState startState;
        private HashSet<FiniteState> finalStates;

        public bool Read(string input)
        {
            return false;
        }
    }

    sealed class FiniteState { } //Need something to point to
}
