using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public MapGenerator mapGenerator;
	public int currentMoveDist = 5;
	public int attackRange = 2;
	public int viewDist = 11;
	public int maxMoveDist = 5;
	public ArrayList currentPath = new ArrayList();
	int currentMaxPath = 0;

	public Vector3 position;


	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
		currentMaxPath = 0;
		currentPath = new ArrayList();
		currentPath.Add(new Vector2(position.x, -position.y));
	}

	public void setMoveDist(int newMoveDist) {
		currentMoveDist = newMoveDist;
//		currentPath = new Vector2[currentMoveDist];
	}

	public ArrayList addPathTo(Vector2 pos) {
		int diff;
	//	if (currentPath.Count > 0) {
			Vector2 lastObj = (Vector2)currentPath[currentPath.Count-1];
			diff = (int)(Mathf.Abs(lastObj.x - pos.x) + Mathf.Abs(lastObj.y - pos.y));
	//	}
	//	else {
	//		diff = (int)(Mathf.Abs(pos.x - position.x) + Mathf.Abs(-pos.y - position.y));
	//	}
	//	Debug.Log("diff : " + diff);
		if (diff == 0) return currentPath;
	//	if (diff + currentMaxPath <= currentMoveDist) {
	//		Debug.Log("Add!");
//			currentPath.Add(pos);

//			ArrayList newObjs = calculatePath(lastObj, pos, new ArrayList(), currentMoveDist - currentMaxPath, true);
//			currentMaxPath += newObjs.Count;
//			foreach (Vector2 v in newObjs) {
//				currentPath.Add(v);
//			}
		Debug.Log("AddPathTo: " + currentMoveDist + "  " + currentMaxPath);
			currentPath = calculatePathSubtractive((ArrayList)currentPath.Clone(), pos, currentMoveDist - currentMaxPath);
			currentMaxPath = currentPath.Count - 1;
//		}
		return currentPath;
	}

	ArrayList calculatePath(ArrayList currentPathFake, Vector2 posFrom,Vector2 posTo, ArrayList curr, int maxDist, bool first, int num = 0) {
	//	Debug.Log(posFrom + "  " + posTo + "  " + maxDist);
		
		if (!first) {
			if ((exists(currentPathFake, posFrom) || exists(curr, posFrom))) return curr;
			curr.Add(posFrom);
		}
	//	if (maxDist == 0) Debug.Log("Last: " + curr.Count);
		if (maxDist == 0) return curr;
		ArrayList a = new ArrayList();
		a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x - 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x + 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y - 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y + 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		int dist = maxDist + 10000;
		int minLength = maxDist + 10000;
		ArrayList minArray = new ArrayList();
//		Debug.Log("dist: " + dist);
		foreach (ArrayList b in a) {
//			Debug.Log("From: " + posFrom + " To: " + posTo + " maxDist: " + maxDist + " num: " + num + " count: " + b.Count + " currCount: " + curr.Count);
//			if (num == 1) Debug.Log("First: " + b.Count + "  " + maxDist);
			if (b.Count == 0) continue;
			Vector2 last = (Vector2)b[b.Count-1];
			int d = (int)(Mathf.Abs(last.x - posTo.x) + Mathf.Abs(last.y - posTo.y));
//			if (num == 1) Debug.Log("First Two: " + d);
			if (d < dist || (d == dist && b.Count < minLength)) {// && b.Count > 1)) {
				dist = d;
				minArray = b;
				minLength = b.Count;
			}

//			if (d == 0) break;
		}
		return minArray;
	}

	ArrayList calculatePathSubtractive(ArrayList currList, Vector2 posTo, int maxDist) {
	//	Debug.Log(maxDist + "!!!!!!");
		int closestDist = maxDist + 10000;
		int minLength = maxDist + 10000;
		ArrayList closestArray = new ArrayList();
		closestArray.Add(currList[0]);
	//	Debug.Log("Subtractive:   " + currList.Count);
		while (currList.Count >= 1) {// && maxDist < currentMoveDist) {
	//		Debug.Log("currList: " + currList.Count);
			Vector2 last1;
			if (currList.Count > 0) {
				last1 = (Vector2)currList[currList.Count-1];
			}
			else {
				last1 = new Vector2(position.x, position.y);
			}
			ArrayList added = calculatePath(currList, last1, posTo, new ArrayList(), maxDist, true);
			ArrayList withAdded = new ArrayList();
			foreach (Vector2 v in currList) {
				withAdded.Add(v);
			}
			foreach (Vector2 v in added) {
				withAdded.Add(v);
			}
			if (withAdded.Count != 0) {
				Vector2 last = (Vector2)withAdded[withAdded.Count-1];
				int d = (int)(Mathf.Abs(last.x - posTo.x) + Mathf.Abs(last.y - posTo.y));
				if (d == 0) return withAdded;
				if (d < closestDist || (d == closestDist && withAdded.Count < minLength)) {
			//		Debug.Log("Is Closer!!  " + d);
					closestDist = d;
					closestArray = withAdded;
					minLength = closestArray.Count;
				}
			}
			maxDist++;
			currList.RemoveAt(currList.Count - 1);
		}
	//	if (closestArray.Count == 0)
	//		closestArray.Add(new Vector2(position.x, position.y));
		return closestArray;
	}

	public static bool exists(ArrayList a, Vector2 v) {
		if (a == null) return false;
		foreach (Vector2 v2 in a) {
//			if (v2.x == v.x && v2.y == v.y) return true;
			if (vectorsEqual(v2, v)) return true;
		}
		return false;
	}

	public static bool vectorsEqual(Vector2 one, Vector2 two) {
//		return one.x == two.x && one.y == two.y;
		return Mathf.Abs(one.x - two.x) < 0.01 && Mathf.Abs(one.y - two.y) < 0.01;
	}

	public ArrayList removeFromPathTo(Vector2 pos) {
		for (int n = currentPath.Count-1; n>=0; n--) {
			if (currentPath[n].Equals(pos)) {
				break;
			}
			Vector2 curr = (Vector2)currentPath[n];
			if (n > 0) {
				Vector2 curr1 = (Vector2)currentPath[n-1];
				currentMaxPath -= (int)(Mathf.Abs(curr.x - curr1.x) + Mathf.Abs(curr.y - curr1.y));
			}
			currentPath.RemoveAt(n);
		}
		return currentPath;
	}

	// Use this for initialization
	void Start () {
		
		currentMoveDist = 5;
		attackRange = 2;
		viewDist = 11;
		maxMoveDist = 5;
//		currentPath = new ArrayList();
	//	currentMaxPath = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
