using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Graphs
{
    class Searcher<N>
    {
        private Graph<N, double> internalState; //double costs to travel from one node to another

        public Searcher(Graph<N, double> graph)
        {
            internalState = graph ?? throw new ArgumentNullException();
        }

        public IReadOnlyList<Edge<N, double>> BreadthFirst(N start, N destination)
        {
            if (start == null || destination == null)
                throw new ArgumentNullException();

            Queue<N> frontier = new Queue<N>();
            frontier.Enqueue(start);
            HashSet<N> explored = new HashSet<N>();
            Dictionary<N, List<Edge<N, double>>> paths = new Dictionary<N, List<Edge<N, double>>>();

            while (frontier.Count != 0)
            {
                N state = frontier.Dequeue();
                explored.Add(state);
                
                if (state.Equals(destination))
                    return paths[state]; //path found (but s = d -> empty list)

                foreach (Edge<N, double> outgoing in internalState.ViewChildren(state))
                {
                    N neighbor = outgoing.child;
                    if (!(frontier.Contains(neighbor) || explored.Contains(neighbor)))
                    {
                        frontier.Enqueue(neighbor);
                        List<Edge<N, double>> pathCopy = new List<Edge<N, double>>(paths[state]) { outgoing };
                        paths.Add(neighbor, pathCopy);
                    }
                }
            }
            
            return null; //path not found
        }

        public IReadOnlyList<Edge<N, double>> DepthFirst(N start, N destination)
        {
            if (start == null || destination == null)
                throw new ArgumentNullException();

            Stack<N> frontier = new Stack<N>();
            frontier.Push(start);
            HashSet<N> explored = new HashSet<N>();
            Dictionary<N, List<Edge<N, double>>> paths = new Dictionary<N, List<Edge<N, double>>>();

            while (frontier.Count != 0)
            {
                N state = frontier.Pop();
                explored.Add(state);

                if (state.Equals(destination))
                    return paths[state]; //path found (but s = d -> empty list)

                foreach (Edge<N, double> outgoing in internalState.ViewChildren(state))
                {
                    N neighbor = outgoing.child;
                    if (!(frontier.Contains(neighbor) || explored.Contains(neighbor)))
                    {
                        frontier.Push(neighbor);
                        List<Edge<N, double>> pathCopy = new List<Edge<N, double>>(paths[state]) { outgoing };
                        paths.Add(neighbor, pathCopy);
                    }
                }
            }

            return null;
        }

        public IReadOnlyList<Edge<N, double>> DepthLimited(N start, N destination, int maxDepth)
        {
            ISet<N> explored = new HashSet<N>();
            Graph<N, double>.Builder builder = new Graph<N, double>.Builder();
            explored.Add(start);
            List<N> temp = new List<N>();
            //Adds all nodes to builder; Note: very inefficient -> gotta rework!
            for (int i = 0; i < maxDepth; i++)
            {
                foreach (N state in explored)
                {
                    foreach (Edge<N, double> outgoing in internalState.ViewChildren(state))
                    {
                        if (!explored.Contains(outgoing.child))
                            temp.Add(outgoing.child);
                    }
                }
                foreach (N state in temp)
                {
                    explored.Add(state);
                }
                temp.Clear();
            }
            foreach (N state in explored)
            {
                builder.AddNode(state);
            }
            //Add edges to builder
            foreach (N state in explored)
            {
                foreach (Edge<N, double> outgoing in internalState.ViewChildren(state))
                {
                    builder.AddEdge(outgoing);
                }
            }
            //Search over new smaller depth graph
            Searcher<N> searcher = new Searcher<N>(builder.ToGraph());
            return searcher.DepthFirst(start, destination);
        }

        //Uniform cost search
        public IReadOnlyList<Edge<N, double>> LeastCost(N start, N destination)
        {
            if (start == null || destination == null)
                throw new ArgumentNullException();

            return null;
        }

        /* Uniform cost search: Least cost/Djikstra's; cost function for p-queue based on distance from start
         * g(n) = cost to reach a node; used in UCS
         * Informed search: are we getting closer to the goal (steps or cost reducing)? only need an estimate
         * h_sld(n) = how close are we to the goal from this node; a possible heuristic function
         * Greedy search: expands nodes based on what is closest to goal, based on h(n) (same p-queue as UCS)
         * A*: combines the two -> cost/f(n) = g(n) + h(n); h can be any heuristic; same p-queue to expand nodes; IDA* = iterative deepening of A*.
         * Multiple other heuristics (can almost eliminate tedious search if really good)
         * Admissiable heuristic: never overestimates cost to reach goal (either accurate or underestimates cost; optimistic, like direct distance)
         * Heuristic examples: # of misplaced tiles/manhattan distance for 8 puzzle;
         * If h(n) is admissable, A* is optimal
         * 
         * If the path does not matter/systematically searching the space is not possible, turn to Local Search!
         * 
         */
    }
}
