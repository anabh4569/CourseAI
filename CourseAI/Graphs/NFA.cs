using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Graphs
{
    sealed class NFA
    {
        //Very INCOMPLETE class; finish: epsilon closure, concat/union/star/dot/single

        //Remember, graphs are immutable (builders); FiniteStates are too (empty)
        public NFA(Graph<FiniteState, char> states, FiniteState startState, HashSet<FiniteState> finalStates)
        {
            this.states = states ?? throw new ArgumentNullException(nameof(states));
            this.startState = startState ?? throw new ArgumentNullException(nameof(startState));
            if (finalStates == null)
                throw new ArgumentNullException(nameof(finalStates));
            this.finalStates = new HashSet<FiniteState>(finalStates);
        }

        //No epsilon transitions allowed
        public bool Read(string input)
        {
            HashSet<FiniteState> current = new HashSet<FiniteState> { startState };
            //For each input char
            foreach (char c in input)
            {
                HashSet<FiniteState> temp = new HashSet<FiniteState>();
                //For each state we're in atm
                foreach (FiniteState state in current)
                {
                    //For each of their children
                    foreach (Edge<FiniteState, char> transition in states.ViewChildren(state))
                    {
                        if (c == transition.label && transition.parent == state)
                        {
                            temp.Add(transition.child);
                        }
                    }
                }
                current = temp;
            }
            //If any of our states are in an accept state
            foreach (FiniteState state in current)
                if (finalStates.Contains(state))
                    return true;
            //Otherwise we have no states in accept states, so fail
            return false;
        }

        public const char EPSILON = '-';

        private Graph<FiniteState, char> states; //embedded states and transitions into this
        private FiniteState startState;
        private HashSet<FiniteState> finalStates;

    }

    sealed class FiniteState { } //Need something to point to
}
