using UnityEngine;
using System.Collections.Generic;

public class SkillTree {

	struct Node {
		public int id;
		public List<int> children;
		public List<int> parents;
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

	public void AddChild(int cid, int pid) {
		Node parent = FindNode(pid);
	}

	private Node FindNode(int id) {

	}

}
