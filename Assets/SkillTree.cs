using UnityEngine;
using System.Collections.Generic;

public class SkillTree {

	private struct Node {
		public int id;
		public List<Node> children;
		public List<Node> parents;
		public bool IsRoot
		{
			get
			{
				return parents.Count == 0;
			}
		}
		public bool IsLeaf
		{
			get
			{
				return children.Count == 0;
			}
		}
	}
	
	List<Node> rootNodes;

	public void AddChild(int childId, int parentId) {
		Node n = new Node();
		n.id = childId;
		if (rootNodes.Count == 0) {
			rootNodes.Add(n);
		}
		Node parent = FindNode(parentId, rootNodes);
	}

	private Node FindNode(int id, List<Node> nodes) {
		Node match;
		foreach (Node n in nodes) {
			if (n.id == id) {
				match = n;
			} else {
				match = FindNode(id, n.children);
			}
			if (match != null) {
				break;
			}
		}
		return match;
	}

}
