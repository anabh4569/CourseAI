using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAI.Graphs
{
    sealed class Graph<N, L> where L : IComparable<L>
    {
        private Dictionary<N, List<Edge<N, L>>> edges;

        private Graph() { }
        private Graph(Dictionary<N, List<Edge<N, L>>> edges)
        {
            this.edges = edges ?? throw new ArgumentNullException();
        }

        public IReadOnlyCollection<N> ViewNodes()
        {
            return edges.Keys;
        }

        public IReadOnlyCollection<Edge<N, L>> ViewChildren(N parent)
        {
            if (parent == null)
                throw new ArgumentNullException();
            return edges[parent];
        }

        sealed public class Builder
        {
            private Dictionary<N, List<Edge<N, L>>> edges; //List -> mutltigraph

            public Builder()
            {
                edges = new Dictionary<N, List<Edge<N, L>>>();
            }

            public Graph<N, L> ToGraph()
            {
                return new Graph<N, L>(edges);
            }

            public void AddNode(N node)
            {
                if (node == null)
                    throw new ArgumentNullException();
                if (!edges.Keys.Contains(node)) //otherwise error thrown by dictionary!
                    edges.Add(node, new List<Edge<N, L>>());
            }

            public void AddEdge(Edge<N, L> edge)
            {
                if (edge == null)
                    throw new ArgumentNullException();
                if (edges.Keys.Contains(edge.parent) && edges.Keys.Contains(edge.child))
                    edges[edge.parent].Add(edge);
            }
        }
    }

    sealed class Edge<N, L> : IComparable<Edge<N, L>> where L : IComparable<L>
    {
        public Edge(N parent, L label, N child)
        {
            this.parent = parent;
            this.label = label;
            this.child = child;
        }

        public readonly N parent;
        public readonly L label;
        public readonly N child;

        public int CompareTo(Edge<N, L> other)
        {
            return label.CompareTo(other.label);
        }
    }
}