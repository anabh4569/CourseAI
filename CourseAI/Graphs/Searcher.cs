using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Graphs
{
    class Searcher<Node, Cost> where Cost : IComparable<Cost>
    {
        //Need edges that have costs (not meaningless labels or something)
        private Graph<Node, Cost> graph;

        public Searcher(Graph<Node, Cost> graph)
        {
            this.graph = graph ?? throw new ArgumentNullException();
        }

        public IReadOnlyList<Edge<Node, Cost>> BreadthFirst(Node start, Node destination)
        {
            if (start == null || destination == null)
                throw new ArgumentNullException();

            Queue<Node> frontier = new Queue<Node>();
            frontier.Enqueue(start);
            HashSet<Node> explored = new HashSet<Node>();
            Dictionary<Node, List<Edge<Node, Cost>>> paths = new Dictionary<Node, List<Edge<Node, Cost>>>();

            while (frontier.Count != 0)
            {
                Node state = frontier.Dequeue();
                explored.Add(state);
                
                if (state.Equals(destination))
                    return paths[state]; //path found (but s = d -> empty list)

                foreach (Edge<Node, Cost> outgoing in graph.ViewChildren(state))
                {
                    Node neighbor = outgoing.child;
                    if (!(frontier.Contains(neighbor) || explored.Contains(neighbor)))
                    {
                        frontier.Enqueue(neighbor);
                        List<Edge<Node, Cost>> pathCopy = new List<Edge<Node, Cost>>(paths[state]) { outgoing };
                        paths.Add(neighbor, pathCopy);
                    }
                }
            }
            
            return null; //path not found
        }

        public IReadOnlyList<Edge<Node, Cost>> DepthFirst(Node start, Node destination)
        {
            if (start == null || destination == null)
                throw new ArgumentNullException();

            Stack<Node> frontier = new Stack<Node>();
            frontier.Push(start);
            HashSet<Node> explored = new HashSet<Node>();
            Dictionary<Node, List<Edge<Node, Cost>>> paths = new Dictionary<Node, List<Edge<Node, Cost>>>();

            while (frontier.Count != 0)
            {
                Node state = frontier.Pop();
                explored.Add(state);

                if (state.Equals(destination))
                    return paths[state]; //path found (but s = d -> empty list)

                foreach (Edge<Node, Cost> outgoing in graph.ViewChildren(state))
                {
                    Node neighbor = outgoing.child;
                    if (!(frontier.Contains(neighbor) || explored.Contains(neighbor)))
                    {
                        frontier.Push(neighbor);
                        List<Edge<Node, Cost>> pathCopy = new List<Edge<Node, Cost>>(paths[state]) { outgoing };
                        paths.Add(neighbor, pathCopy);
                    }
                }
            }

            return null;
        }

        public IReadOnlyList<Edge<Node, Cost>> DepthLimited(Node start, Node destination, int maxDepth)
        {
            ISet<Node> explored = new HashSet<Node>();
            Graph<Node, Cost>.Builder builder = new Graph<Node, Cost>.Builder();
            explored.Add(start);
            List<Node> temp = new List<Node>();
            //Adds all nodes to builder; Note: very inefficient -> gotta rework!
            for (int i = 0; i < maxDepth; i++)
            {
                foreach (Node state in explored)
                {
                    foreach (Edge<Node, Cost> outgoing in graph.ViewChildren(state))
                    {
                        if (!explored.Contains(outgoing.child))
                            temp.Add(outgoing.child);
                    }
                }
                foreach (Node state in temp)
                {
                    explored.Add(state);
                }
                temp.Clear();
            }
            foreach (Node state in explored)
            {
                builder.AddNode(state);
            }
            //Add edges to builder
            foreach (Node state in explored)
            {
                foreach (Edge<Node, Cost> outgoing in graph.ViewChildren(state))
                {
                    builder.AddEdge(outgoing);
                }
            }
            //Search over new smaller depth graph
            Searcher<Node, Cost> searcher = new Searcher<Node, Cost>(builder.ToGraph());
            return searcher.DepthFirst(start, destination);
        }

        //Uniform cost search
        public IReadOnlyList<Edge<Node, Cost>> LeastCost(Node start, Node destination)
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
         * LS: useful for pure optimization, circuit design, when final configuration > steps to reach (like 8 queens).
         * Hold a single current state and move around to neighbors -> good solutions in large/continuous spaces + little memory.
         * Ascent/descent, simulated annealing, genetic algorithms are all examples.
         * Objective function (cost/objective function to min/max) -> can get stuck at local maximums.
         * 
         * Nodes are now a pairing of state and value (or regular graph w/ some function that maps states to values).
         * Hill climbing and genetic algorithms:
         */
         
        /* Allows cost/value functions to be passed in;
         * delegate = new 'class' that can be instantiated/passed around
         *    ex: private Cost c1 = CoolFunc; c1 += OtherFunc; c1();
         * event = keyword to a multicast delegate to allow adding from other classes but only private calls
         *    ex: private event Cost c2 = CoolFunc; etc.
         */
        public delegate Cost Value(Node state);

        /* Hill climbing: gradient ascent (states instead of continuous independent variable)
         * Variants:
         * Sideways moves: to escape plateaus where neighbors are the same value
         * Random-restart: Multiple random initial starts, pick the best one to guess global max
         * Stochastic: random upwards movements
         * Local beam search: top k valued states instead of top #1
         * Stochastic beam: k successors are picked at random
         */
        public Node HillClimbing(Node start, Value vOfNode)
        {
            Node current = start;
            while (true)
            {
                List<Node> neighbors = new List<Node>((IEnumerable<Node>)this.graph.ViewChildren(current));
                if (neighbors.Count > 0)
                {
                    neighbors.Sort(delegate(Node x, Node y)
                    {
                        return vOfNode(x).CompareTo(vOfNode(y));
                    });
                    Node maxN = neighbors[neighbors.Count - 1];
                    Cost max = vOfNode(maxN);

                    if (vOfNode(current).CompareTo(max) > 0)
                        return current;

                    current = maxN;
                }
                else
                    return current;
            }
        }

        //Genetic algorithm/notes in another file
    }
}
