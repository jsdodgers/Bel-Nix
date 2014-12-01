using UnityEngine;
using System.Collections;

public class AStarAlgorithm {
	public static AStarReturnObject findPath(AStarMap map) {
		return findPath(map,int.MaxValue);
	}
	
	public static AStarReturnObject findPath(AStarMap map, int max) {
		if (map.startNode == null) return null;
		BinaryHeap heap = new BinaryHeap(map.startNode);
		ArrayList closed = new ArrayList();
		AStarNode node = null;
		AStarNode closest = null;
		while (!heap.isEmpty()) {
			node = heap.remove();
			closed.Add(node.parameters);
			if (map.nodeIsCloseEnough(node)) {
				return new AStarReturnObject(node,closed,heap,map);
			}
			if (max==0) {
				return new AStarReturnObject(closest,closed,heap,map);
			}
			float d = node.distance();
			ArrayList next = map.nextNodesFrom(node);
			foreach (AStarNode nextNode in next) {
				if (!closed.Contains(nextNode.parameters)) {
					AStarNode nextNode2 = heap.remove(nextNode.parameters);
					float newD = d + map.distanceBetweenNodes(node,nextNode);
					if (nextNode2 != null) {
						if (newD < nextNode2.distance()) {
							nextNode2.setDistance(newD);
							nextNode2.prev = node;
						}
					}
					else {
						nextNode2 = nextNode;
						nextNode2.setDistance(newD);
						nextNode2.prev = node;
					} 
					if (!nextNode2.isValidNode(map)) {
						closed.Add(nextNode2.parameters);
					}
					else {
						heap.add(nextNode2);
					}
				}
			}
			if (closest == null) closest = node;
			else if (node.heuristic() < closest.heuristic()) closest = node;
			max--;
		}
		return new AStarReturnObject(closest,closed,heap,map);
	}
	
	public static AStarNode reversePath(AStarNode path) {
		AStarNode curr = path;
		AStarNode prev = curr.prev;
		curr.prev = null;
		while (prev != null) {
			AStarNode prevPrev = prev.prev;
			prev.prev = curr;
			curr = prev;
			prev = prevPrev;
		}
		return curr;
	}
}


public class AStarReturnObject {
	public AStarNode finalNode;
	public ArrayList closedList;
	public BinaryHeap heap;
	public AStarMap map;
	
	public AStarReturnObject(AStarNode finalNode,ArrayList closedList,BinaryHeap heap,AStarMap map) {
		this.finalNode = finalNode;
		this.closedList = closedList;
		this.heap = heap;
		this.map = map;
	}
}
