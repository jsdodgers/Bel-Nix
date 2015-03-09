using UnityEngine;
using System.Collections;
using System;


public class AStarParameters  {
	public override bool Equals(System.Object obj)  {
		if (obj == null) return false;
		AStarParameters p = obj as AStarParameters;
		if ((System.Object)p == null) return false;
		if (p == this) return true;
		return false;
	}
	
	public virtual string toString()  {
		return "()";
	}
	
	public virtual string ToString()  {
		return toString();
	}
}

public class AStarNode  {
	
	private float h = int.MaxValue;
	private float d = int.MaxValue;
	public AStarParameters parameters;
	public AStarNode prev;
	
	
	public AStarNode()  {
		prev = null;
	}
	
	public AStarNode(AStarParameters parameters)  {
//		this(parameters,0.0f);
		this.parameters = parameters;
	}
	
	public AStarNode(AStarParameters parameters, float heuristic) : this(parameters)  {
		this.h = heuristic;
	//	return this;
	}

	public virtual void setPrev(AStarNode node)  {
		prev = node;
	}
	
	public virtual void setHeuristic(float heuristic)  {
		this.h = heuristic;
	}
	
	public virtual void setDistance(float distance)  {
		this.d = distance;
	}
	
	public virtual void setHeuristicMax()  {
		this.h = int.MaxValue;
	}
	
	public virtual void setDistanceMax()  {
		this.d = int.MaxValue;
	}
	
	public virtual int depth()  {
		if (prev == null) return 0;
		return prev.depth() + 1;
	}
	
	public virtual float heuristic()  {	
		return this.h;
	}
	
	public virtual float distance()  {
		return this.d;
	}
	
	public virtual float f()  {
		if (this.heuristic() == int.MaxValue || this.distance() == int.MaxValue || (this.heuristic() >= 0 && int.MaxValue - this.heuristic() < this.distance())) return int.MaxValue;
		return this.heuristic() + this.distance();
	}
	
	public virtual bool isValidNode(AStarMap map)  {
		return true;
	}
	
	
	public virtual String toString()  {
		return "(" + this.parameters + ", (H: " + h + ",D: " + d + ",F: " + f() + "))";
	}
	
	public override String ToString()  {
		return toString();
	}
}