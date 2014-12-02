using UnityEngine;
using System.Collections;

public class AStarEnemyNode : AStarNode {


	public AStarEnemyNode(AStarParameters parameters, float heuristic) : base(parameters, heuristic) {

	}

	/*
	public bool isValidNode(AStarEnemyMap map, Direction dir) {
		Tile t = MapGenerator.tiles[parameters.x, parameters.y];
		return t.canStand();
	}
	public bool canGoInDirection(AStarEnemyMap map, Direction dir, Direction prevDir) {
		Tile t = MapGenerator.tiles[parameters.x, parameters.y];
		return t.canPass(dir, map.unit, prevDir);
	}*/
}



public class AStarEnemyParameters : AStarParameters {
	public int x;
	public int y;


	public AStarEnemyParameters(int x, int y) {
		this.x = x;
		this.y = y;
	}
	
	public override bool Equals (object obj)
	{
		if (obj==null) return false;
		AStarEnemyParameters p = (AStarEnemyParameters)obj;
		if ((System.Object)p==null) return false;
		return x==p.x && y==p.y;
		
	}
	
	public override string toString() {
		return "(x:" + x + ",y:" + y + ")";		
	}
}
