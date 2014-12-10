using UnityEngine;
using System.Collections;


public class AStarMap {
	public AStarNode startNode;
	public ArrayList goalNodes;
	public bool hasMultipleGoals;

	public AStarMap() {
		goalNodes = new ArrayList();
	}

	public virtual ArrayList nextNodesFrom(AStarNode node) {
		return new ArrayList();
	}
	
	public virtual ArrayList nextNodesFrom(AStarNode node, ArrayList closedList) {
		return new ArrayList();
	}
	
	public virtual bool nodeIsCloseEnough(AStarNode node) {
		return false;
	}
	
	public virtual bool nodeCanBeReachedFrom(AStarNode node,AStarNode fromNode) {
		return false;
	}
	
	public virtual float distanceBetweenNodes(AStarNode node,AStarNode node2) {
		return 0.0f;
	}
	
	public virtual float distanceBetweenParams(AStarParameters param, AStarParameters param2) {
		return 0.0f;
	}
	
	public virtual float heuristicForParameters(AStarParameters parameters) {
		return 0.0f;
	}
	
	public virtual AStarNode createNodeWithParameters(AStarParameters parameters) {
		return new AStarNode(parameters,heuristicForParameters(parameters));
	}
	
	public virtual void addGoalNode(AStarNode node) {
		goalNodes.Add(node);
		if (goalNodes.Count>1) hasMultipleGoals = true;
	}
	
	public virtual void setGoalNodes(ArrayList goals) {
		goalNodes = goals;
		if (goalNodes.Count>1) hasMultipleGoals = true;
	}

}
