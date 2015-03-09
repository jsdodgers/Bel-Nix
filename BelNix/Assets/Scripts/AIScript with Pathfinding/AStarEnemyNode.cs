using UnityEngine;
using System.Collections;

public class AStarEnemyNode : AStarNode  {


	public AStarEnemyNode(AStarParameters parameters, float heuristic) : base(parameters, heuristic)  {

	}
	
	public override void setPrev(AStarNode node)  {
		base.setPrev(node);
		if (node is AStarEnemyNode)  {
			AStarEnemyParameters fromN = node.parameters as AStarEnemyParameters;
			AStarEnemyParameters toN = parameters as AStarEnemyParameters;
			Direction dir = Direction.Down;
			if (toN.x < fromN.x) dir = Direction.Left;
			else if (toN.x > fromN.x) dir = Direction.Right;
			else if (toN.y < fromN.y) dir = Direction.Up;
			(parameters as AStarEnemyParameters).fromDir = dir;
		}
	}
	/*
	public bool isValidNode(AStarEnemyMap map, Direction dir)  {
		Tile t = MapGenerator.tiles[parameters.x, parameters.y];
		return t.canStand();
	}
	public bool canGoInDirection(AStarEnemyMap map, Direction dir, Direction prevDir)  {
		Tile t = MapGenerator.tiles[parameters.x, parameters.y];
		return t.canPass(dir, map.unit, prevDir);
	}*/
}



public class AStarEnemyParameters : AStarParameters  {
	public int x;
	public int y;
	public Direction fromDir = Direction.None;
	public Tile t = null;

	public AStarEnemyParameters(int x, int y, Tile t)  {
		this.x = x;
		this.y = y;
		this.t = t;
	}

	public Vector2 getPos()  {
		return new Vector2(x,y);
	}

	public override bool Equals (object obj)  {
		if (obj==null) return false;
		AStarEnemyParameters p = (AStarEnemyParameters)obj;
		if ((System.Object)p==null) return false;
		return x==p.x && y==p.y && (t == null || t.standable || fromDir == p.fromDir);
		
	}
	
	public override string toString()  {
		return "(x:" + x + ",y:" + y + ", dir: " + fromDir + ")";		
	}
}
