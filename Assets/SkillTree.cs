using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SkillTree {

	public class Node {
		public int id;
		public List<Node> adjacentNodes;
        public bool active;

        // Next 3 attributes are volatile, and for
        // use only in the search algorithms
        public int dist;
        public bool known;
        public Node path;

        public bool Landlocked()
        {
            foreach (Node a in adjacentNodes)
            {
                if (a.active)
                {
                    return true;
                }
            }
            return false;
        }
	}
	
	List<Node> inactiveNodes;
    List<Node> activeNodes;
    List<Node> nodes;
	
    /**
     * This method finds the shortest path(s) from an inactive node
     * on the skill tree to one that is active.
     */
	public List<LinkedList<Node>> bfSearch(Node s) {

        // "Zero out" all the nodes in the graph
        foreach (Node n in nodes)
        {
            n.dist = int.MaxValue;
            n.known = false;
        }

        // Create node queue and add the start node
        Queue<Node> q = new Queue<Node>();
        s.dist = 0;
        s.path = null;
        q.Enqueue(s);

        // List of viable ending nodes
        List<Node> destinations = new List<Node>();

        // Go through each depth, attempting to find an active node
        // Stop when we find some viable destination points, or when
        // we run out of nodes in the queue
        while (q.Count != 0 && destinations.Count != 0)
        {
            Node v = q.Dequeue();

            // Loop through each of v's adjacent nodes, searching
            // for an active node
            foreach (Node w in v.adjacentNodes)
            {
                if (w.dist == int.MaxValue)
                {
                    w.dist = v.dist + 1;
                    w.path = v;
                    // Enqueue all non-active nodes
                    if (w.active) {
                        destinations.Add(w);
                    } else {
                        q.Enqueue(w);
                    }
                }
            }
        }

        // Create paths from destination points
        List<LinkedList<Node>> paths = new List<LinkedList<Node>>();
        foreach (Node d in destinations)
        {
            LinkedList<Node> path = new LinkedList<Node>();
            Node c = d;
            while (c.path != null)
            {
                path.AddLast(c);
                c = c.path;
            }
        }

        return paths;
	}

}
