using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.SoloStates
{
    static class Adversarial<State> where State : class, IState<State>
    {
        /* Adverserial search (aka games) have 2x2 cases: deterministic/chance and perfect/imperfect information.
         * Gonna start with deterministic perfect info games. (Methods of analyzing psychology/adaption patterns in games for later)
         * 
         * Zero-sum games: pure competition; single objective function, one agent maxing, other minimizing. Each move in game = a "ply".
         * Agents can disagree on what they think the value truly is (various chess bots).
         * Embedded thinking/backwards reasoning: how does the opponent think + many steps of yourself/opponent in advance.
         * 
         * Formalization: S_0 = initial state, Player(s) = whose move it is, Action(s) = set of legal moves (for that player)
         * Transition function: T(s, a) -> s; Terminal states when game ends/someone won; Utility(s, p) = objective func in term state s for player p
         * 
         * Minimax algorithm: single objective function, max starts; depth first search, finding utility of terminal states.
         * Then, propogate the those values up from terminal states (max node = highest of all children, min = lowest of all children).
         * By alternating grabs for mins/maxs at each depth, you make it seem like each guy's playing optimally.
         */

        //Returns next optimal move for next guy (p1sTurn == true -> it's the first player's turn)
        public static State MinimaxDecision(State initial, bool p1sTurn)
        {
            return MinimaxNext(initial, p1sTurn).Item1;
        }

        private static Tuple<State, double> MinimaxNext(State s, bool findMax)
        {
            List<State> children = new List<State>(s.GetChildren());
            if (children.Count > 0)
            {
                List<Tuple<State, double>> temp = new List<Tuple<State, double>>();
                foreach (State st in children)
                    temp.Add(new Tuple<State, double>(st, MinimaxNext(st, !findMax).Item2));
                Comparison<Tuple<State, double>> comp;
                if (findMax)
                    comp = delegate (Tuple<State, double> a, Tuple<State, double> b) { return b.Item2.CompareTo(a.Item2); };
                else
                    comp = delegate (Tuple<State, double> a, Tuple<State, double> b) { return a.Item2.CompareTo(b.Item2); };
                temp.Sort(comp);
                return temp[0];
            }
            else
                return new Tuple<State, double>(s, s.Value());
        }

        /* DFS time: b^m, DFS space: bm -> really bad for high branching factor (like chess/go)
         * Problem: real world limitations -> can't search the whole thing;
         * Solution(s): replace terminal utility func with evaluation func for non-terminal position
         *   + iterative deepening + pruning (removing large parts of the tree)
         * 
         * Alpha-beta pruning, a fix to minimax: in a min step to be passed to a max step, if we find a value that is less
         * than the min of another branch, we don't need to continue searching in that branch: can be applied both ways.
         * 
         * Meaning: we start at alpha = -inf and beta = inf; dfs to the terminal nodes and as you're searching for a result,
         * remembering to copy the newest alpha/beta of the parent; for either max/min, change alpha/beta to be the best 
         * candidate of the max/min from their children. These updates might percolate multiple levels.
         * 
         * Clear ex:
         *             A                Max
         *     /       |     \
         *    B        C      D         Min
         *  / |  \    /|\   / |\
         * 3  12  8  2 P P 14 5 2       Max
         * 
         * We start with A's alpha = -inf and beta = inf. A sends this down to B, so this is B's values too. B, looks at 3,
         * updating beta cuz we found a smaller value. 12 and 8 are looked at but discarded as they aren't smaller than 3
         * for the min. B's beta is its min result. This is sent back up to A, which updates alpha to the its current max
         * result contender, which is now 3. A sends its values down to C, so C is initialized with alpha = 3 and beta = inf.
         * C looks at the first min contender, 2, and updates beta accordingly. Now, as alpha >= beta, we do not have to see
         * any of C's children. C returns, its min result or beta = 2 back to A. A rejects 2 because its alpha is already a
         * higher max. The process repeats for D, but D can't prune anything because 3 only >= 2 in its children, and its
         * children are in decreasing order.
         * 
         * Rationale: C pruned because any result of C after seeing 2 would be <= 2, by def of min. But A already has a higher
         * max candidate than 2 (in 3), so there's no need to look at those when they can never be max candidates. D couldn't be
         * pruned because beta/D's min candidates were higher than A's other max candidates until you got to 2 (which would've
         * pruned the rest of D's children if there were any left).
         */
         
        //Faster version of the minimax one (just following algorithm to the dot instead of trying to use c# syntatic sugar)
        public static State AlphaBetaDecision(State current, bool p1sTurn)
        {
            return AlphaBetaNext(current, p1sTurn, double.NegativeInfinity, double.PositiveInfinity).Item1;
        }

        private static Tuple<State, double> AlphaBetaNext(State parent, bool findMax, double parentAlpha, double parentBeta)
        {
            IReadOnlyCollection<State> children = parent.GetChildren();
            Tuple<State, double> output = new Tuple<State, double>(null, findMax ? double.NegativeInfinity : double.PositiveInfinity);
            if (children.Count > 0)
            {
                foreach (State child in children)
                {
                    double utility = AlphaBetaNext(child, !findMax, parentAlpha, parentBeta).Item2;
                    if (findMax)
                    {
                        if (utility > output.Item2)
                            output = new Tuple<State, double>(child, utility);
                        if (output.Item2 >= parentBeta)
                            break;
                        if (output.Item2 > parentAlpha)
                            parentAlpha = output.Item2;
                    }
                    else
                    {
                        if (utility < output.Item2)
                            output = new Tuple<State, double>(child, utility);
                        if (output.Item2 <= parentAlpha)
                            break;
                        if (output.Item2 < parentBeta)
                            parentBeta = output.Item2;
                    }
                }
                return output;
            }
            else
                return new Tuple<State, double>(null, parent.Value());
        }

        /* Move ordering: if the best moves are on the left, tons of pruning + very fast vs best moves on the right;
         * Worst time complexity = b^m vs best time complexity = b^(m/2).
         * Easy fix: improve state.GetChildren() algorithm -> remember best moves for shallow depths (chess openings),
         * order nodes after finding, domain knowledge (prioritize finding 'higher value' moves), bookkeep/intern states.
         * Harder fix: try to find best states when the children object is produced.
         * 
         * Minimax generates the entire game search space, alpha-beta prunes chunks but still goes to terminal leaves.
         * Impractical in real-time/impossible for some games.
         * Solution: update value func to estimate the value of the current board configuration (non-terminal states).
         * Should rank terminal states normally, yet be a heuristic for any state. Ex: in chess, white - black piece values.
         * 
         * Typical heuristic type: weighted linear sum of the features -> work of art to find best features (chess engines).
         * These weights can be determined via machine learning (linear regression? but this would require a training set)!
         * 
         * Beyond alpha-beta pruning: Stochastic Games (chance/non-deterministic)
         * Now, there's layers of chance nodes/children between max/min layers. Upgraded minimax: for chance nodes, it's the
         * average of all children nodes.
         */

        //On to machine learning (K nearest neighbors to start)!
    }
}
