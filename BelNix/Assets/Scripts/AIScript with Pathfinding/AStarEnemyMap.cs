using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarEnemyMap : AStarMap {

	public Unit unit;
	public MapGenerator mapGenerator;
	
	public AStarEnemyMap(Unit u, MapGenerator mg) {
		unit = u;
		mapGenerator = mg;
		hasMultipleGoals = false;
		setStartNode();	
	}
	
	public void setStartNode() {
		AStarEnemyParameters parameters = new AStarEnemyParameters((int)unit.position.x,(int)-unit.position.y, mapGenerator.tiles[(int)unit.position.x,(int)-unit.position.y]);
		float heuristic = heuristicForParameters(parameters);
		startNode = new AStarEnemyNode(parameters,heuristic);
		startNode.setDistance(heuristic);
	}
	
	public void setGoalsAndHeuristics(List<Unit> goalUnits) {
		ArrayList arr = new ArrayList();
		foreach (Unit u in goalUnits) {
			AStarEnemyParameters parameters = new AStarEnemyParameters((int)u.position.x,(int)-u.position.y, mapGenerator.tiles[(int)u.position.x,(int)-u.position.y]);
			arr.Add(new AStarEnemyNode(parameters,0.0f));
		}
		setGoalNodes(arr);
		setStartNode();
	}
	
	public override float heuristicForParameters(AStarParameters parameters) {
		float min = -1251.0f;
		foreach (AStarEnemyNode node in goalNodes) {
			float current = distanceBetweenParams(node.parameters,parameters);
			if (current==0.0f) return 0.0f;
			if (min<0.0f || current < min) {
				min = current;
			}
		}
		return (min>=0.0f?min:0.0f);
	}
	
	
	public override ArrayList nextNodesFrom(AStarNode node) {
		return nextNodesFrom(node,null);
	}
	
	public override ArrayList nextNodesFrom(AStarNode node, ArrayList closedList) {
		ArrayList arr = new ArrayList();
		AStarEnemyParameters param = (AStarEnemyParameters)node.parameters;
		for (int n=-1;n<=1;n++) {
			for (int m=-1;m<=1;m++) {
				if ((n==0 && m==0) || (n!=0 && m!=0)) continue;
				int x = param.x + n;
				int y = param.y + m;
				AStarEnemyParameters param1 = new AStarEnemyParameters(x,y, mapGenerator.tiles[x,y]);
				if (closedList != null && closedList.Contains(param1)) continue;
				float heur = heuristicForParameters(param1);
				AStarEnemyNode node1 = new AStarEnemyNode(param1,heur);
				if (!nodeCanBeReachedFrom(node1,node)) continue;
				arr.Add(node1);
			}
		}
		return arr;
	}
	
	public override bool nodeCanBeReachedFrom(AStarNode node,AStarNode fromNode) {
		AStarEnemyParameters toN = (AStarEnemyParameters)node.parameters;
		AStarEnemyParameters fromN = (AStarEnemyParameters)fromNode.parameters;
		Direction dir = Direction.Down;
		if (toN.x < fromN.x) dir = Direction.Left;
		else if (toN.x > fromN.x) dir = Direction.Right;
		else if (toN.y < fromN.y) dir = Direction.Up;
		Tile t = mapGenerator.tiles[fromN.x, fromN.y];
		return t.canPass(dir, unit, fromN.fromDir);
	}
	
	
	public override bool nodeIsCloseEnough(AStarNode node) {
		AStarEnemyParameters nodeParams = (AStarEnemyParameters)node.parameters;
		Tile t = mapGenerator.tiles[nodeParams.x,nodeParams.y];
		foreach (AStarEnemyNode goal in goalNodes) {
			AStarEnemyParameters goalParams = (AStarEnemyParameters)goal.parameters;
			Tile g = mapGenerator.tiles[goalParams.x,goalParams.y];
			//if (Mathf.Abs(goalParams.x-nodeParams.x) + Mathf.Abs(goalParams.y-nodeParams.y)<=(g.hasCharacter()?g.getCharacter().minReachableDistance():1.0f)) {
			if (t.canStand() || t.getCharacter()==unit) {
				if (mapGenerator.hasLineOfSight(t, g, (g.hasCharacter()?g.getCharacter().minReachableDistance(unit):1.0f), true)) {
					return true;
				}
			}
		}
		return false;
	}
	
	public override float distanceBetweenNodes(AStarNode node,AStarNode node2) {
		return distanceBetweenParams(node.parameters,node2.parameters);
	}
	
	public override float distanceBetweenParams(AStarParameters param,AStarParameters param2) {
		AStarEnemyParameters enemyParam = (AStarEnemyParameters)param;
		AStarEnemyParameters enemyParam2 = (AStarEnemyParameters)param2;
//		float diag = Mathf.Min(Mathf.Abs(enemyParam.x-enemyParam2.x),Mathf.Abs(enemyParam.y-enemyParam2.y));
		float straight = Mathf.Abs(enemyParam.x-enemyParam2.x) + Mathf.Abs(enemyParam.y-enemyParam2.y);
//		return diag*1.4f + (straight - 2*diag) * 1.0f;
		Tile t = mapGenerator.tiles[enemyParam.x,enemyParam.y];
		if (t.shouldTakeAttOppLeaving(unit)) {
		//	Debug.Log("Take Attack Of Opportunity: " + enemyParam.x + ", " + enemyParam.y + "   " + straight);
			straight += 3;
		}
		Vector2 from = enemyParam.getPos();
		Vector2 to = enemyParam2.getPos();
		Direction dir = Tile.directionBetweenTiles(from, to);
		int pass = t.passabilityInDirection(dir);
		if (pass > 1) {
			straight += 1 + (pass-1)/5;
		}
		if (t.hasAlly(unit)) {
			straight++;
		}
		return straight;
	}
}
