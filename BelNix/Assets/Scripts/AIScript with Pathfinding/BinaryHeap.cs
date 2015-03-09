using System;
using System.Collections;
using UnityEngine;


public class BinaryHeap {
	private ArrayList tree;
	
	public BinaryHeap ()  {
		tree = new ArrayList();
	}
	
	public BinaryHeap(AStarNode head)  {
		tree = new ArrayList();
		tree.Add(head);
	}
	
	public int getLength()  {
		return tree.Count;
	}
	
	public AStarNode getRoot()  {
		return (AStarNode) tree[0];
	}
	
	public Boolean isEmpty()  {
		return tree.Count==0;
	}
	
	public int size()  {
		return tree.Count;
	}
	
	public Boolean isRoot(int x)  {
		return x == 0;
	}
	
	public Boolean hasLeft(int x)  {
		return x*2 + 2 <= tree.Count;
	}
	
	public Boolean hasRight(int x)  {
		return x * 2 + 3 <= tree.Count;
	}
	
	public AStarNode leftChild(int x)  {
		return (AStarNode) tree[x * 2 + 1];
	}
	
	public AStarNode rightChild(int x)  {
		return (AStarNode) tree[x * 2 + 2];
	}
	
	public AStarNode getParent(int x)  {
		return (AStarNode) tree[(x - 1)/2];
	}
	
	public void add(AStarNode node)  {
		
		tree.Add(node);
		int x = tree.Count - 1;
		while (!isRoot(x))  {
			AStarNode parent = getParent(x);
			AStarNode xEntry = (AStarNode) tree[x];
			if (parent.f() > xEntry.f())  {
				tree[x] = parent;
				tree[(x-1)/2] = xEntry;
			}
			else  {
				break;
			}
			x = (x-1)/2;
		}
	}
	
	public AStarNode remove()  {
		return remove(0);
	}
	
	public AStarNode remove(AStarParameters parameters)  {
		for (int n=0;n<tree.Count;n++)  {
			if (((AStarNode)tree[n]).parameters.Equals(parameters))  {
				return remove(n);
			}
		}
		return null;
	}
	
	
	public AStarNode remove(int x)  {

		if (isEmpty() || x>=size() || x<0) return null;
		AStarNode e = (AStarNode) tree[x];
		tree.RemoveAt(x);
		if (!isEmpty())  {
			tree.Insert(x,tree[tree.Count-1]);
			tree.RemoveAt(tree.Count-1);
		}
		while (hasLeft(x))  {
			AStarNode l = leftChild(x);
			AStarNode min = l;
			AStarNode parent = (AStarNode)tree[x];
			int next = x * 2 + 1;
			if (hasRight(x))  {
				AStarNode r = rightChild(x);
				if (r.f() < l.f())  {
					min = r;
					next = x * 2 + 2;
				}
			}
			if (min.f() < parent.f())  {
				tree[next] = tree[x];
				tree[x] = min;
			}
			else  {
				break;
			}
			x = next;
		}
		return e;
	}
	
	public String toString()  {
		String s = "[";
		for (int n=0;n<tree.Count;n++)  {
			if (n!=0)  {
				s+=",";
			}
			else  {
			}
//			AStarNode
			s+= ((AStarNode) tree[n]).toString();
		}
		s+="]";
		return s;
	}
	
}

