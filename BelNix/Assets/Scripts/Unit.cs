using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CharacterInfo;
using CombatSystem;

public enum MovementType {Move, BackStep, Recover, Cancel, None}
public enum StandardType {Attack, Reload, Intimidate, Inventory, Throw, Place_Turret, Lay_Trap, Cancel, None}
public enum MinorType {Loot, Cancel, None}
public enum Affliction {Prone = 1 << 0, Immobilized = 1 << 1, Addled = 1 << 2, Confused = 1 << 3, Poisoned = 1 << 4, None}
public enum InventorySlot {Head, Shoulder, Back, Chest, Glove, RightHand, LeftHand, Pants, Boots, Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, None}

public class Unit : MonoBehaviour {
	public bool playerControlled = true;
	public AStarEnemyMap aiMap = null;
	public Character characterSheet;
	int initiative;
	public string characterName;
	public string characterId;
	public int team = 0;

	public static float cPerc = 0.0f;
	public static float kPerc = 0.0f; 
	
	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;
	static Vector2 turnOrderScrollPos = new Vector2(0.0f, 0.0f);
	Vector2 classFeaturesScrollPos = new Vector2(0.0f, 0.0f);
	Vector2 groundScrollPosition = new Vector2(0.0f, 0.0f);

	public Unit attackedByCharacter = null;

	public Transform trail;
	public MapGenerator mapGenerator;
	public int moveDistLeft = 5;
	public int currentMoveDist = 5;
	public int attackRange = 1;
	public int viewDist = 11;
	public int maxMoveDist = 5;
	public ArrayList currentPath = new ArrayList();
	public int currentMaxPath = 0;
	public bool beingAttacked = false;
	public bool wasBeingAttacked = false;
	public int shouldMove = 0;
	public bool shouldDoAthleticsCheck = false;
	public bool shouldCancelMovement = false;
	public bool moving = false;
	public bool rotating = false;
	public bool attacking = false;
	public bool attackAnimating = false;
	public Unit attackEnemy = null;
	public Vector2 rotateFrom;
	public Vector2 rotateTo;
	Animator anim;
	public bool usedMovement;
	public bool usedStandard;
	public int minorsLeft = 2;
//	public bool usedMinor1;
//	public bool usedMinor2;

	public bool isCurrent;
	public bool isSelected;
	public bool isTarget;
	SpriteRenderer targetSprite;
	SpriteRenderer hairSprite;
	public bool doAttOpp = true;
	public GameGUI gui;

	public List<Affliction> afflictions;
	public List<TurretUnit> turrets;

	public GameObject damagePrefab;


	public void addHair() {
		GameObject go = Instantiate(characterSheet.personalInfo.getCharacterHairStyle().getHairPrefab()) as GameObject;
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(0, 0, 0);
		go.transform.localEulerAngles = new Vector3(0, 0, 0);
		hairSprite = go.GetComponent<SpriteRenderer>();
		hairSprite.color = characterSheet.characterSheet.characterColors.headColor;
	}

	public List<SpriteOrder> getSprites() {
		List<SpriteOrder> sprites = characterSheet.getSprites();
		sprites.Add(new SpriteOrder(hairSprite.gameObject, 7));
		return sprites;
	}

	public void setAllSpritesToRenderingOrder(int renderingOrder) {
		List<SpriteOrder> sprites = getSprites();
		foreach (SpriteOrder sprite in sprites) {
			sprite.sprite.renderer.sortingOrder = renderingOrder + sprite.order;
		}
	}

	public bool isProne() {
		return isAfflictedWith(Affliction.Prone);
//		return affliction == Affliction.Prone;
	}

	public bool isAfflictedWith(Affliction a) {
		return afflictions.Contains(a);
//		Debug.Log(a + "  " + affliction + "  " + (a & affliction));
//		return (a & affliction) != Affliction.None;
	}

	public static string getNameOfStandardType(StandardType standard) {
		switch (standard) {
		case StandardType.Place_Turret:
			return "Place Turret";
		case StandardType.Lay_Trap:
			return "Lay Trap";
		default:
			return standard.ToString();
		}
	}

	public static string getNameOfMovementType(MovementType movement) {
		switch (movement) {
		default:
			return movement.ToString();
		}
	}

	public static string getNameOfMinorType(MinorType minor) {
		switch (minor) {
		default:
			return minor.ToString();
		}
	}


	public void selectMovementType(MovementType t) {
		switch(t) {
		case MovementType.BackStep:
			currentMoveDist = 1;
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		case MovementType.Move:
			currentMoveDist = moveDistLeft;
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		default:
			currentMoveDist = 0;
			mapGenerator.removePlayerPath();
			break;
		}
	}

	public StandardType getStandardType(ClassFeature feature) {
		switch (feature) {
		case ClassFeature.Throw:
			return StandardType.Throw;
		case ClassFeature.Intimidate:
			return StandardType.Intimidate;
		default:
			return StandardType.None;
		}
	}


	public MovementType[] getMovementTypes() {
		List<MovementType> movementTypes = new List<MovementType>();
		if (isProne()) {
			movementTypes.Add(MovementType.Recover);
		}
		else {
			movementTypes.Add(MovementType.Move);
			movementTypes.Add(MovementType.BackStep);
		}
	//	movementTypes.Add(MovementType.Cancel);
		return movementTypes.ToArray();
		return new MovementType[] {MovementType.Move, MovementType.BackStep, MovementType.Cancel};
	}

	public StandardType[] getStandardTypes() {
		List<StandardType> standardTypes = new List<StandardType>();
		standardTypes.Add(StandardType.Attack);
		ClassFeature[] features = characterSheet.characterProgress.getClassFeatures();
		foreach (ClassFeature feature in features) {
			StandardType st = getStandardType(feature);
			if (st != StandardType.None)
				standardTypes.Add(st);
		}
		if (characterSheet.characterSheet.inventory.hasTurret()) standardTypes.Add(StandardType.Place_Turret);
		if (characterSheet.characterSheet.inventory.hasTrap()) standardTypes.Add(StandardType.Lay_Trap);
		standardTypes.Add(StandardType.Inventory);
	//	standardTypes.Add (StandardType.Cancel);
		return standardTypes.ToArray();
		return new StandardType[] {StandardType.Attack, StandardType.Inventory, StandardType.Cancel};
	}
	public MinorType[] getMinorTypes() {
		return new MinorType[] {MinorType.Loot};
	}

	public int numberMinors() {
		return getMinorTypes().Length;
	}

	public int numberMovements() {
		return getMovementTypes().Length;
	}

	public int numberStandards() {
		return getStandardTypes().Length;
	}

	public int minReachableDistance() {
		for (int n=1;n<10;n++) {
			if (canGetWithin(n,n)) return n;
		}
		return 1;
	}

	public int getAttackRange() {
		Weapon w = characterSheet.characterSheet.characterLoadout.rightHand;
		if (w==null) return 0;
		return w.range;
	}

	public bool canGetWithin(int dist, int minDist = 1) {
		for (int n=-dist;n<=dist;n++) {
			for (int m=-dist;m<=dist;m++) {
				int d = Mathf.Abs(n) + Mathf.Abs(m);
				if (d > dist || d < minDist) continue;
				int x = (int)position.x + n;
				int y = (int)-position.y + m;
				if (x >= 0 && y>=0 && x < mapGenerator.actualWidth && y < mapGenerator.actualHeight) {
					Tile t = mapGenerator.tiles[x,y];
					if (t.canStand()) return true;
				}
			}
		}
		return false;
	}

	public void setSelected() {
		isSelected = true;
		isTarget = false;
		getTargetSprite().enabled = true;
		getTargetSprite().color = Color.white;
		setTargetObjectScale();
	}

	public void setTarget() {
		isSelected = false;
		isTarget = true;
		getTargetSprite().enabled = true;
		getTargetSprite().color = Color.red;
		setTargetObjectScale();
	}

	public void deselect() {
		isSelected = false;
		isTarget = false;
		getTargetSprite().enabled = false;
	}

	public void setCurrent() {
		isCurrent = true;
		addTrail();
	}

	public void removeCurrent() {
		isCurrent = false;
		removeTrail();
	}

	public void removeTrail() {
		if (trail) {
			TrailRenderer tr = trail.GetComponent<TrailRenderer>();
			tr.enabled = false;
			tr.time = 0.0f;
		}
	}

	public void addTrail() {
		if (trail) {
			setTrailRendererPosition();
			TrailRenderer tr = trail.GetComponent<TrailRenderer>();
			tr.enabled = true;
			StartCoroutine(resetTrailDist());
//			tr.time = 2.2f;
		}
	}

	
	IEnumerator resetTrailDist() {
		yield return new WaitForSeconds(.1f);
		trail.GetComponent<TrailRenderer>().time=1.0f;//2.2f;
		
	}


	public void setTrailRendererPosition() {
		if (trail || (isCurrent && trail)) {
			float factor = 0.5f - trail.GetComponent<TrailRenderer>().startWidth/2.0f;
			float speed = 3.0f;
			float x = Mathf.Sin(Time.time * speed) * factor;
			float y = Mathf.Cos(Time.time * speed) * factor;
			trail.localPosition = new Vector3(x, y, trail.localPosition.z);
		}
	}

	public void setTargetObjectScale() {
		if (isSelected || isTarget) {
			float factor = 1.0f/10.0f;
			float speed = 3.0f;
			float addedScale = Mathf.Sin(Time.time * speed) * factor;
			float scale = 1.0f + factor + addedScale;
			getTargetSprite().transform.localScale = new Vector3(scale, scale, 1.0f);
		}
	}

	public void setCircleScale() {
		if (isHovering) {
			float factor = 1.0f/10.0f;
			float speed = 3.0f;
			float addedScale = Mathf.Sin(Time.time * speed) * factor;
			float scale = 1.0f + factor + addedScale;
			getCircleSprite().transform.localScale = new Vector3(scale, scale, 1.0f);
		}
		else {
			getCircleSprite().transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
	}

	public virtual void setPosition(Vector3 position) {

	}

	public void resetVars() {
		usedMovement = false;
		usedStandard = false;
//		usedMinor1 = false;
//		usedMinor2 = false;
		minorsLeft = 2;
		currentMoveDist = 0;
		moveDistLeft = maxMoveDist;
		usedDecisiveStrike = false;
	}

	public void rollInitiative() {
		initiative = Random.Range(1,21) + characterSheet.combatScores.getInitiative();
	}

	public int getInitiative() {
		return initiative;
	}

	public bool isEnemyOf(Unit cs) {
		return team != cs.team;
	}

	public bool isAllyOf(Unit cs) {
		return team == cs.team;
	}



	
	
	public void setNewTilePosition(Vector3 pos) {
		if (mapGenerator==null) return;
		if (mapGenerator && position.x > 0 && -position.y > 0) {
			if (mapGenerator.tiles[(int)position.x,(int)-position.y].getCharacter()==this) {
				mapGenerator.tiles[(int)position.x,(int)-position.y].removeCharacter();
			}
		}
		if (pos.x > 0 && -pos.y > 0) {
			Tile t = mapGenerator.tiles[(int)pos.x,(int)-pos.y];
			if (mapGenerator && !t.hasCharacter()) {
				mapGenerator.tiles[(int)pos.x,(int)-pos.y].setCharacter(this);
			}
			t.doTrapDamage(this);
		}
	}
	
	public void followPath() {
		moving = true;
	}

	public void resetPath() {
//		Debug.Log("reset path");
		currentMaxPath = 0;
		currentPath = new ArrayList();
		currentPath.Add(new Vector2(position.x, -position.y));
	}
	
	public void setMoveDist(int newMoveDist) {
		currentMoveDist = newMoveDist;
		moveDistLeft = newMoveDist;
		//		currentPath = new Vector2[currentMoveDist];
	}
	
	public void setPathCount() {
		currentMaxPath = currentPath.Count - 1;
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
//		Debug.Log("AddPathTo: " + currentMoveDist + "  " + currentMaxPath);
		currentPath = calculatePathSubtractive((ArrayList)currentPath.Clone(), pos, currentMoveDist - currentMaxPath);
		setPathCount();
		//		}
		return currentPath;
	}

	bool canPass(Direction dir, Vector2 pos, Direction dirFrom) {
//		if (!mapGenerator.tiles[(int)pos.x,(int)pos.y].canTurn)
//			Debug.Log(pos + ": " + dir + "   --  " + dirFrom);
		return mapGenerator.canPass(dir, (int)pos.x, (int)pos.y, this, dirFrom);//dirFrom);
	}

	int passibility(Direction dir, Vector2 pos) {
		return mapGenerator.passibility(dir, (int)pos.x, (int)pos.y);
	}

	ArrayList calculatePath(ArrayList currentPathFake, Vector2 posFrom,Vector2 posTo, ArrayList curr, int maxDist, bool first, int num = 0, Direction dirFrom = Direction.None, int minorsUsed = 0) {
		//	Debug.Log(posFrom + "  " + posTo + "  " + maxDist);
		if (minorsUsed > minorsLeft) return curr;
		if (!first) {
			//	if (!mapGenerator.canStandOn(posFrom.x, posFrom.y)) return curr;
			if ((exists(currentPathFake, posFrom) || exists(curr, posFrom))) return curr;
			curr.Add(posFrom);
		}
		if (vectorsEqual(posFrom, posTo)) return curr;
		//	if (maxDist == 0) Debug.Log("Last: " + curr.Count);
		if (maxDist <= 0) return curr;
		ArrayList a = new ArrayList();
		if (canPass(Direction.Left, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x - 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Left, minorsUsed + (passibility(Direction.Left, posFrom)>1?1:0)));
		if (canPass(Direction.Right, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x + 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Right, minorsUsed + (passibility(Direction.Right, posFrom)>1?1:0)));
		if (canPass(Direction.Up, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y - 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Up, minorsUsed + (passibility(Direction.Up, posFrom)>1?1:0)));
		if (canPass(Direction.Down, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y + 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Down, minorsUsed + (passibility(Direction.Down, posFrom)>1?1:0)));
		int dist = maxDist + 10000;
		int minLength = maxDist + 10000;
		ArrayList minArray = curr;//new ArrayList();
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

		int closestDist = maxDist + 10000;
		int minLength = maxDist + 10000;

		ArrayList closestArray = (ArrayList)currList.Clone();
		Vector2 las = (Vector2)currList[currList.Count-1];
		int dis = (int)(Mathf.Abs(las.x - posTo.x) + Mathf.Abs(las.y - posTo.y));
		closestDist = dis;
		minLength = currList.Count;
		//	Debug.Log("Subtractive:   " + currList.Count);
		int nnn = 1;
		while (currList.Count >= 1) {// && maxDist < currentMoveDist) {
			//		Debug.Log("currList: " + currList.Count);
			Vector2 last1;
			if (currList.Count > 0) {
				last1 = (Vector2)currList[currList.Count-1];
			}
			else {
				last1 = new Vector2(position.x, position.y);
			}
			Direction dir = Direction.None;
			if (currList.Count > 1) {
				Vector2 curr = (Vector2)currList[currList.Count-1];
				Vector2 last = (Vector2)currList[currList.Count-2];
				if (curr.x < last.x) dir = Direction.Left;
				else if (curr.x > last.x) dir = Direction.Right;
				else if (curr.y < last.y) dir = Direction.Up;
				else if (curr.y > last.y) dir = Direction.Down;
			}
			int minorsUsed = 0;
			for (int n=1;n<currList.Count;n++) {
				Tile t1 = mapGenerator.tiles[(int)((Vector2)currList[n-1]).x,(int)((Vector2)currList[n-1]).y];
				Direction dir1 = Tile.directionBetweenTiles((Vector2)currList[n-1],(Vector2)currList[n]);
				if (passibility(dir1,(Vector2)currList[n-1])>1) minorsUsed++;
			}
			Debug.Log(nnn + ":   " + minorsUsed);
			nnn++;
			ArrayList added = calculatePath(currList, last1, posTo, new ArrayList(), maxDist, true, 0, dir, minorsUsed);
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
				if (d < closestDist) {// || (d == closestDist && withAdded.Count < minLength)) {
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
			if (vectorsEqual(v2, v)) return true;
		}
		return false;
	}
	
	public static bool vectorsEqual(Vector2 one, Vector2 two) {
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

	public Unit closestEnemy() {
		Unit closest = null;
		float dist = float.MaxValue;
		foreach (Unit u in mapGenerator.priorityOrder) {
			if (u.team != team && !u.deadOrDyingOrUnconscious()) {
				float d = distanceFromUnit(u);
				if (d < dist) {
					dist = d;
					closest = u;
				}
			}
		}
		
		for (int n=0;n<mapGenerator.turrets.transform.childCount;n++) {
			Transform tr = mapGenerator.turrets.transform.GetChild(n);
			TurretUnit tur = tr.GetComponent<TurretUnit>();
			if (tur != null && tur.team != team) {
				float d = distanceFromUnit(tur);
				if (d < dist) {
					dist = d;
					closest = tur;
				}
			}
		}
		return closest;
	}

	public float closestEnemyDist() {
		float dist = 0;
		foreach (Unit u in mapGenerator.priorityOrder) {
			if (u.team != team && !u.deadOrDyingOrUnconscious()) {
				float d = distanceFromUnit(u);
				if (d < dist || dist == 0) {
					dist = d;
				}
			}
		}
		for (int n=0;n<mapGenerator.turrets.transform.childCount;n++) {
			Transform tr = mapGenerator.turrets.transform.GetChild(n);
			TurretUnit tur = tr.GetComponent<TurretUnit>();
			if (tur != null && tur.team != team) {
				float d = distanceFromUnit(tur);
				if (d < dist || dist == 0) {
					dist = d;
				}
			}
		}
		return dist;
	}

	public float distanceFromUnit(Unit u) {
		return Mathf.Abs(u.position.x - position.x) + Mathf.Abs(u.position.y - position.y);
	}

	public void performAI() {
		if (isPerformingAnAction() || mapGenerator.movingCamera) return;
		float closestDist = closestEnemyDist();
		Unit enemy = closestEnemy();
		if (!usedMovement) {
			if (closestDist > 1) {
				
				List<Unit> units = new List<Unit>();
				foreach (Unit u in mapGenerator.priorityOrder) {
					if (team != u.team && !u.deadOrDyingOrUnconscious()) {
						units.Add (u);
					}
				}
				for (int n=0;n<mapGenerator.turrets.transform.childCount;n++) {
					Transform tr = mapGenerator.turrets.transform.GetChild(n);
					TurretUnit tur = tr.GetComponent<TurretUnit>();
					if (tur != null && tur.team != team) {
						units.Add (tur);
					}
				}
				aiMap.setGoalsAndHeuristics(units);
				AStarReturnObject ret = AStarAlgorithm.findPath(aiMap);
				AStarNode node = ret.finalNode;
				node = AStarAlgorithm.reversePath(node);
//				currentPath = new ArrayList();
				resetPath();
				int d = 0;
				while (node != null) {
					if (d != 0) {
						AStarEnemyParameters param = (AStarEnemyParameters)node.parameters;
						Vector2 v = new Vector2(param.x, param.y);
						currentPath.Add(v);
					}
					node = node.prev;
					d++;
					if (d>5) break;
				}
				for (int n=currentPath.Count-1;n>=1;n--) {
					Vector2 v = (Vector2)currentPath[n];
					if (!mapGenerator.tiles[(int)v.x,(int)v.y].canStand()) {
						currentPath.RemoveAt(n);
						setPathCount();
					}
					else {
						break;
					}
				}
				mapGenerator.resetPlayerPath();
				mapGenerator.setPlayerPath(currentPath);
				startMoving(false);
				usedMovement = true;
				return;
			}
		}
		if (isPerformingAnAction() || mapGenerator.movingCamera) return;
	//	usedStandard = true;
		if (!usedStandard) {
			if (closestDist <= 1.0f) {
				usedStandard = true;
				attackEnemy = enemy;
				setRotationToAttackEnemy();
				if (enemy != null) enemy.setTarget();
				startAttacking();
				return;
			}
		}
		if (isPerformingAnAction() || mapGenerator.movingCamera) return;
		if ((usedStandard || closestDist > 1.0f) && (usedMovement || closestDist <= 1.0f)) {
			mapGenerator.nextPlayer();
		}
	}

	static float redStyleWidth = 0.0f;
	static float greenStyleWidth = 0.0f;
	static float healthHeight = 15.0f;
	static GUIStyle redStyle = null;
	static GUIStyle greenStyle = null;
	static Texture2D[] greenTextures;
	/*
	void createStyle() {
		if (redStyle == null) {
			redStyle = new GUIStyle(GUI.skin.box);
		}
		if (greenStyle == null) {
			greenStyle = new GUIStyle(GUI.skin.box);
		}
	}*/

	GUIStyle getRedStyle(float width) {
		if (redStyle == null) {
			redStyle = new GUIStyle("box");
		}
		if (width != redStyleWidth) {
			redStyleWidth = width;
			redStyle.normal.background = makeTex((int)width, (int)healthHeight, Color.red);
		}
		return redStyle;
	}

	GUIStyle getGreenStyle(int width) {
		if (greenStyle == null) {
			greenStyle = new GUIStyle("box");
		}
		if (greenTextures == null || greenTextures.Length != (int)Screen.width) {
			Texture2D[] tex = new Texture2D[(int)Screen.width];
			for (int n=0;n<Mathf.Min(tex.Length, (greenTextures != null ? greenTextures.Length : 0)); n++) {
				tex[n] = greenTextures[n];
			}
			greenTextures = tex;
		}
		if (greenTextures[width] == null) {
			greenTextures[width] = makeTex((int)width, (int)healthHeight, Color.green);
		}
		if (greenStyleWidth != width) {
			greenStyle.normal.background = greenTextures[width];
			greenStyleWidth = width;
		}
		return greenStyle;
	}

	private Texture[] paperDollTexturesHead;

	public Texture[] getPaperDollTexturesHead() {
		if (paperDollTexturesHead == null) {
			paperDollTexturesHead = new Texture[]{Resources.Load<Texture>("Units/Jackie/JackiePaperdollHead")};
		}
		return paperDollTexturesHead;
	}

	private Texture[] paperDollTexturesFull;
	public Texture[] getPaperDollTexturesFull() {
		if (paperDollTexturesFull == null) {
			paperDollTexturesFull = new Texture[]{Resources.Load<Texture>("Units/Jackie/JackiePaperdoll")};
		}
		return paperDollTexturesFull;
	}

	/*
	GUIStyle paperDollHealthBannerStyle = null;
	
	GUIStyle getPaperDollHealthBannerStyle() {
		if (paperDollHeadStyle == null) {
			paperDollHeadStyle = new GUIStyle("label");
		}
		return paperDollHeadStyle;
	}*/
//	static Texture2D paperDollHealthBannerTexture = null;
//	static int lastWidth = 0;

//	Texture2D getPaperDollHealthBannerTexture(int width, int height) {
//		if (paperDollHealthBannerTexture == null || width != lastWidth) {
//			paperDollHealthBannerTexture = //makeTexBanner(width, height, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
//			lastWidth = width;
//		}
//		return paperDollHealthBannerTexture;
//	}

	static Texture2D paperDollFullBackgroundTexture = null;
	static int lastWidthFull = 0;
	Texture2D getPaperDollFullBackgroundTexture(int width, int height) {
		if (paperDollFullBackgroundTexture == null || width != lastWidthFull) {
			paperDollFullBackgroundTexture = makeTexBorder(width, height, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			lastWidthFull = width;
		}
		return paperDollFullBackgroundTexture;
	}

	static Texture2D missionObjectivesBackgroundTexture = null;
	Texture2D getMissionObjectivesBackgroundTexture() {
		if (missionObjectivesBackgroundTexture == null) {
			missionObjectivesBackgroundTexture = makeTexBorder((int)missionObjectivesWidth, (int)missionObjectivesHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return missionObjectivesBackgroundTexture;
	}

	static Texture2D missionTitleBackgroundTexture = null;
	Texture2D getMissionTitleBackgroundTexture() {
		if (missionTitleBackgroundTexture == null) {
			missionTitleBackgroundTexture = makeTexBorder((int)missionObjectivesWidth + (int)missionTabWidth - 1, (int)missionTopHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return missionTitleBackgroundTexture;
	}

	static Texture2D turnOrderBackgroundTexture = null;
	Texture2D getTurnOrderBackgroundTexture() {
		if (turnOrderBackgroundTexture == null) {
			turnOrderBackgroundTexture = makeTexBorder((int)turnOrderWidth, (int)paperDollFullHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return turnOrderBackgroundTexture;
	}
	
	static Texture2D turnOrderNameBackgroundTexture = null;
	Texture2D getTurnOrderNameBackgroundTexture() {
		if (turnOrderNameBackgroundTexture == null) {
			turnOrderNameBackgroundTexture = makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderNameBackgroundTexture;
	}
	
	static Texture2D turnOrderSectionBackgroundTexture = null;
	Texture2D getTurnOrderSectionBackgroundTexture() {
		if (turnOrderSectionBackgroundTexture == null) {
					turnOrderSectionBackgroundTexture = makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderSectionBackgroundTexture;
	}
	
	static Texture2D turnOrderNameBackgroundTextureEnemy = null;
	Texture2D getTurnOrderNameBackgroundTextureEnemy() {
		if (turnOrderNameBackgroundTextureEnemy == null) {
			turnOrderNameBackgroundTextureEnemy = makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderNameBackgroundTextureEnemy;
	}
	
	static Texture2D turnOrderSectionBackgroundTextureEnemy = null;
	Texture2D getTurnOrderSectionBackgroundTextureEnemy() {
		if (turnOrderSectionBackgroundTextureEnemy == null) {
			turnOrderSectionBackgroundTextureEnemy = makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderSectionBackgroundTextureEnemy;
	}

	static Texture2D characterStatsBackgroundTexture = null;
	Texture2D getCharacterStatsBackgroundTexture() {
		if (characterStatsBackgroundTexture == null) {
			characterStatsBackgroundTexture = makeTexBorder((int)characterStatsWidth, (int)characterStatsHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return characterStatsBackgroundTexture;
	}

	static Texture2D skillsBackgroundTexture = null;
	Texture2D getSkillsBackgroundTexture() {
		if (skillsBackgroundTexture == null) {
			skillsBackgroundTexture = makeTexBorder((int)skillsWidth, (int)skillsHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return skillsBackgroundTexture;
	}

	static Texture2D skillsMidSectionTexture = null;
	Texture2D getSkillsMidSectionTexture() {
		if (skillsMidSectionTexture == null) {
			skillsMidSectionTexture = makeTex((int)skillsWidth, 2, new Color(0.08f, 0.08f, 0.2f));
		}
		return skillsMidSectionTexture;
	}

	static Texture2D inventoryBackgroundTexture = null;
	Texture2D getInventoryBackgroundTexture() {
		if (inventoryBackgroundTexture == null) {
			inventoryBackgroundTexture = makeTexBorder((int)inventoryWidth, (int)inventoryHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return inventoryBackgroundTexture;
	}
	Color inventoryLineColor = Color.white;
	
	static Texture2D inventoryLineTall = null;
	Texture2D getInventoryLineTall() {
		if (inventoryLineTall == null) {
			inventoryLineTall = makeTex((int)inventoryLineThickness, (int)inventoryCellSize, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineTall;
	}
	
	static Texture2D inventoryLineWide = null;
	Texture2D getInventoryLineWide() {
		if (inventoryLineWide == null) {
			inventoryLineWide = makeTex((int)inventoryCellSize, (int)inventoryLineThickness, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineWide;
	}

	static Texture2D inventoryHoverBackground = null;
	Texture2D getInventoryHoverBackground() {
		if (inventoryHoverBackground == null) {
			inventoryHoverBackground = makeTex((int)inventoryCellSize,(int)inventoryCellSize, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return inventoryHoverBackground;
	}

	static Texture2D armorHoverBackground = null;
	Texture2D getArmorHoverBackground() {
		if (armorHoverBackground==null) {
			armorHoverBackground = makeTex((int)inventoryCellSize*2,(int)inventoryCellSize*2, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return armorHoverBackground;
	}

	Texture2D makeTexBanner( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for(int i = 0; i < pix.Length; i++)
		{
			pix[i] = col;
		}
		for (int n=0;n<width;n++) {
			for (int m=0;m<height;m++) {
				if (n == 0 || m == height-1 || (m == 0 && width - n >= (height - m)/2) || (width - n == (height-m)/2)) pix[n + m * width] = Color.black;
				else if (width - n > (height - m)/2) pix[n + m * width] = col;
				else pix[n + m * width] = Color.clear;
			}
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}


	public static Texture2D makeTexBorder(int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			//	Debug.Log("it is: " + (i/width));
			if (i/width == 0 || i/width == height-1) pix[i] = Color.black;
			else if (i%width == 0 || i % width == width-1) pix[i] = Color.black;
			else pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	static GUIStyle healthTextStyle = null;
	public GUIStyle getHealthTextStyle(int fontSize) {
		if (healthTextStyle == null) {
			healthTextStyle = new GUIStyle("Label");
			healthTextStyle.normal.textColor = Color.red;
		}
		healthTextStyle.fontSize = fontSize;
		return healthTextStyle;
	}
	
	
	static GUIStyle composureTextStyle = null;
	public GUIStyle getComposureTextStyle(int fontSize) {
		if (composureTextStyle == null) {
			composureTextStyle = new GUIStyle("Label");
			composureTextStyle.normal.textColor = Color.green;//new Color(.316f, 0.0f, .316f);
		}
		composureTextStyle.fontSize = fontSize;
		return composureTextStyle;
	}

	static GUIStyle titleTextStyle = null;
	public GUIStyle getTitleTextStyle() {
		if (titleTextStyle == null) {
			titleTextStyle = new GUIStyle("Label");
			titleTextStyle.normal.textColor = Color.white;
			titleTextStyle.fontSize = 20;
		}
		return titleTextStyle;
	}

	static GUIStyle stackStyle = null;
	public GUIStyle getStackStyle() {
		if (stackStyle == null) {
			stackStyle = new GUIStyle("Label");
			stackStyle.normal.textColor = Color.white;
			stackStyle.fontSize = 11;
			stackStyle.alignment = TextAnchor.LowerRight;
			stackStyle.padding = new RectOffset(0, 0, 0, 0);
		}
		return stackStyle;
	}

	static GUIStyle selectedButtonStyle;
	static float styleWidth = 0.0f;
	GUIStyle getSelectedButtonStyle(float width) {
		if (selectedButtonStyle == null || styleWidth != width) {
			selectedButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTexBorder((int)width,(int)width, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedButtonStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
			selectedButtonStyle.hover.background = tex;//selectedButtonStyle.normal.background;
			selectedButtonStyle.active.background = tex;
			selectedButtonStyle.hover.textColor = Color.white;
			selectedButtonStyle.normal.textColor = Color.white;
			selectedButtonStyle.active.textColor = Color.white;
			styleWidth = width;
		}
		return selectedButtonStyle;
	}

	static GUIStyle nonSelectedButtonStyle;
	static float nonStyleWidth = 0.0f;
	GUIStyle getNonSelectedButtonStyle(float width) {
		if (nonSelectedButtonStyle == null || width != nonStyleWidth) {
			nonSelectedButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTexBorder((int)width,(int)width,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedButtonStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(15.0f, 20.0f, 105.0f));
			nonSelectedButtonStyle.hover.background = tex;//nonSelectedButtonStyle.normal.background;
			nonSelectedButtonStyle.active.background = tex;//getSelectedButtonStyle().normal.background;
			nonSelectedButtonStyle.active.textColor = nonSelectedButtonStyle.normal.textColor = nonSelectedButtonStyle.hover.textColor = Color.white;
			nonStyleWidth = width;
		}
		return nonSelectedButtonStyle;
	}
	
	static GUIStyle nonSelectedMissionButtonStyle;
	GUIStyle getNonSelectedMissionButtonStyle() {
		if (nonSelectedMissionButtonStyle == null) {
			nonSelectedMissionButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTexBorder((int)missionTabWidth, (int)missionTabHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedMissionButtonStyle.normal.background = nonSelectedMissionButtonStyle.hover.background = nonSelectedMissionButtonStyle.active.background = tex;
			nonSelectedMissionButtonStyle.active.textColor = nonSelectedMissionButtonStyle.hover.textColor = nonSelectedMissionButtonStyle.active.textColor = Color.white;
		}
		return nonSelectedMissionButtonStyle;
	}

	static GUIStyle selectedMissionButtonStyle;
	GUIStyle getselectedMissionButtonStyle() {
		if (selectedMissionButtonStyle == null) {
			selectedMissionButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTexBorder((int)missionTabWidth, (int)missionTabHeight, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedMissionButtonStyle.normal.background = selectedMissionButtonStyle.hover.background = selectedMissionButtonStyle.active.background = tex;
			selectedMissionButtonStyle.active.textColor = selectedMissionButtonStyle.hover.textColor = selectedMissionButtonStyle.active.textColor = Color.white;
		}
		return selectedMissionButtonStyle;
	}

	static GUIStyle turnOrderNameStyle;
	static GUIStyle turnOrderNameStyleEnemy;
	GUIStyle getTurnOrderNameStyle(Unit u) {
		if (u.team == 0) {
			if (turnOrderNameStyle == null) {
				turnOrderNameStyle = new GUIStyle("button");
				turnOrderNameStyle.normal.background = turnOrderNameStyle.hover.background = turnOrderNameStyle.active.background = getTurnOrderNameBackgroundTexture();
			}
			return turnOrderNameStyle;
		}
		else {
			if (turnOrderNameStyleEnemy == null) {
				turnOrderNameStyleEnemy = new GUIStyle("button");
				turnOrderNameStyleEnemy.normal.background = turnOrderNameStyleEnemy.hover.background = turnOrderNameStyleEnemy.active.background = getTurnOrderNameBackgroundTextureEnemy();
			}
			return turnOrderNameStyleEnemy;
		}
	}

	static GUIStyle turnOrderSectionStyle;
	static GUIStyle turnOrderSectionStyleEnemy;
	GUIStyle getTurnOrderSectionStyle(Unit u) {
		if (u.team == 0) {
			if (turnOrderSectionStyle == null) {
				turnOrderSectionStyle = new GUIStyle("button");
				turnOrderSectionStyle.normal.background = turnOrderSectionStyle.hover.background = turnOrderSectionStyle.active.background = getTurnOrderSectionBackgroundTexture();
			}
			return turnOrderSectionStyle;
		}
		else {
			if (turnOrderSectionStyleEnemy == null) {
				turnOrderSectionStyleEnemy = new GUIStyle("button");
				turnOrderSectionStyleEnemy.normal.background = turnOrderSectionStyleEnemy.hover.background = turnOrderSectionStyleEnemy.active.background = getTurnOrderSectionBackgroundTextureEnemy();
			}
			return turnOrderSectionStyleEnemy;
		}
	}

	static GUIStyle playerInfoStyle;
	GUIStyle getPlayerInfoStyle() {
		if (playerInfoStyle == null) {
			playerInfoStyle = new GUIStyle("Label");
			playerInfoStyle.normal.textColor = Color.white;
			playerInfoStyle.fontSize = 14;
		}
		return playerInfoStyle;
	}
	
	const float paperDollHeadSize = 90.0f;
	const float portraitBorderSize = 107.0f;
	const float bagButtonSize = 60.0f;
	public const float bannerX = -130.0f;
	public const float bannerY = -160.0f;
	public const float bannerHeight = 317.0f;
	public const float bannerWidth = 500.0f;
	public const float bottomSheetWidth = 500.0f;
	public const float bottomSheetHeight = 500.0f;
	const float cLeft = 8.0f;
	const float kLeft = 14.0f;
	static float paperDollFullWidth = 501.0f;
	const float paperDollFullHeight = 400.0f;
	const float tabButtonsWidth = 51.0f;
	const float missionTopHeight = 31.0f;
	const float missionTabHeight = 124.0f;
	const float missionObjectivesHeight = paperDollFullHeight - missionTopHeight + 1;
	const float missionTabWidth = 80.0f;
	const float missionObjectivesWidth = 150.0f;
	const float turnOrderWidth = 200.0f;
	const float turnOrderSectionHeight = 30.0f;
	const float turnOrderTableX = 15.0f;
	const float turnOrderNameWidth = turnOrderWidth - turnOrderTableX * 2 - turnOrderSectionHeight * 2 - 1.0f;
	const float characterStatsWidth = 150.0f;
	const float characterStatsHeight = 250.0f;
	const float skillsWidth = 225.0f;
	const float skillsHeight = paperDollFullHeight;
	const float inventoryWidth = 300.0f;
	const float inventoryHeight = paperDollFullHeight;
	public const float inventoryCellSize = 24.0f;
	const float inventoryLineThickness = 2.0f;
	public bool guiContainsMouse(Vector2 mousePos) {
		Rect kRect = new Rect (kMin.x + (kMax.x - kMin.x) * kPerc, kMin.y + (kMax.y - kMin.y) * kPerc, kMin.width + (kMax.width - kMin.width) * kPerc, kMin.height + (kMax.height - kMin.height) * kPerc - 18.0f);
		Rect cRect = new Rect (cMin.x + (cMax.x - cMin.x) * cPerc, cMin.y + (cMax.y - cMin.y) * cPerc, cMin.width + (cMax.width - cMin.width) * cPerc, cMin.height + (cMax.height - cMin.height) * cPerc - 18.0f);
		if (kRect.Contains(mousePos) || cRect.Contains(mousePos)) return true;
		if (new Rect(bannerX, bannerY, bannerWidth, bannerHeight).Contains(mousePos)) return true;
		if (gui.getTabButtonRect(Tab.C).Contains(mousePos) || gui.getTabButtonRect(Tab.K).Contains(mousePos)) return true;
		if (gui.openTab == Tab.None || true) {
//			if (mousePos.y <= paperDollHeadSize && paperDollHeadSize + bannerWidth - mousePos.x >= (mousePos.y)/2) return true;
//			if (mousePos.y <= paperDollHeadSize + tabButtonsWidth && mousePos.x <= (tabButtonsWidth - 1.0f) * 3 + 1) return true;
		}
		else {
			if (fullCharacterRect().Contains(mousePos) || fullTabsRect().Contains(mousePos)) return true;
			switch (gui.openTab) {
			case Tab.M:
				return fullMRect().Contains(mousePos);
			case Tab.T:
				return fullTRect().Contains(mousePos);
			case Tab.C:
				return fullCRect().Contains(mousePos);
			case Tab.K:
				return fullKRect().Contains(mousePos);
			case Tab.I:
				return fullIRect().Contains(mousePos);
			default:
				return false;
			}
		}
		return false;
	}
	public Rect fullCharacterRect() {
		return new Rect(0.0f, 0.0f, paperDollFullWidth, paperDollFullHeight);
	}
	public Rect fullTabsRect() {
		return new Rect(0.0f, paperDollFullHeight, (tabButtonsWidth - 1.0f) * 5 + 1, tabButtonsWidth);
	}
	public Rect fullMRect() {
		return new Rect(paperDollFullWidth - 1.0f, 0.0f, missionTabWidth + missionObjectivesWidth, paperDollFullHeight);
	}
	public Rect fullTRect() {
		return new Rect(paperDollFullWidth - 1.0f, 0.0f, turnOrderWidth, paperDollFullHeight);
	}
	public Rect fullCRect() {
		return new Rect(paperDollFullWidth - 1.0f, 0.0f, characterStatsWidth, characterStatsHeight);
	}
	public Rect fullKRect() {
		return new Rect(paperDollFullWidth - 1.0f, 0.0f, skillsWidth, skillsHeight);
	}
	public Rect fullIRect() {
		return new Rect(paperDollFullWidth - 1.0f, 0.0f, inventoryWidth, inventoryHeight);
	}
	public static InventorySlot[] armorSlots = new InventorySlot[]{InventorySlot.Head,InventorySlot.Shoulder,InventorySlot.Back,InventorySlot.Chest,InventorySlot.Glove,InventorySlot.RightHand,InventorySlot.LeftHand,InventorySlot.Pants,InventorySlot.Boots};
	public static InventorySlot[] inventorySlots = new InventorySlot[]{InventorySlot.Zero, InventorySlot.One,InventorySlot.Two,InventorySlot.Three,InventorySlot.Four,InventorySlot.Five,InventorySlot.Six,InventorySlot.Seven,InventorySlot.Eight,InventorySlot.Nine,InventorySlot.Ten,InventorySlot.Eleven, InventorySlot.Twelve, InventorySlot.Thirteen, InventorySlot.Fourteen, InventorySlot.Fifteen};
	public InventorySlot  getInventorySlotFromIndex(Vector2 index) {
//		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return InventorySlot.None;
//		int ind = (int)index.x + ((int)index.y)*4;
		int ind = getLinearIndexFromIndex(index);
		if (ind==-1) return InventorySlot.None;
		return inventorySlots[ind];
	}
	public int getLinearIndexFromIndex(Vector2 index) {
		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return -1;
		return (int)index.x + ((int)index.y)*4;
	}
	public Vector2 getIndexFromLinearIndex(int index) {
		if (index <0 || index > 15) return new Vector2(-1, -1);
		return new Vector2(index%4,index/4);
	}
	public Vector2 getIndexOfSlot(InventorySlot slot) {
		switch (slot) {
		case InventorySlot.Zero:
			return new Vector2(0,0);
		case InventorySlot.One:
			return new Vector2(1,0);
		case InventorySlot.Two:
			return new Vector2(2,0);
		case InventorySlot.Three:
			return new Vector2(3,0);
		case InventorySlot.Four:
			return new Vector2(0,1);
		case InventorySlot.Five:
			return new Vector2(1,1);
		case InventorySlot.Six:
			return new Vector2(2,1);
		case InventorySlot.Seven:
			return new Vector2(3,1);
		case InventorySlot.Eight:
			return new Vector2(0,2);
		case InventorySlot.Nine:
			return new Vector2(1,2);
		case InventorySlot.Ten:
			return new Vector2(2,2);
		case InventorySlot.Eleven:
			return new Vector2(3,2);
		case InventorySlot.Twelve:
			return new Vector2(0,3);
		case InventorySlot.Thirteen:
			return new Vector2(1,3);
		case InventorySlot.Fourteen:
			return new Vector2(2,3);
		case InventorySlot.Fifteen:
			return new Vector2(3,3);
		default:
			return new Vector2(-1,-1);
		}
	}
	public Rect getInventorySlotRect(InventorySlot slot) {
		Vector2 v = getInventorySlotPos(slot);
		switch (slot) {
		case InventorySlot.Head:
		case InventorySlot.Shoulder:
		case InventorySlot.Back:
		case InventorySlot.Chest:
		case InventorySlot.Glove:
		case InventorySlot.RightHand:
		case InventorySlot.LeftHand:
		case InventorySlot.Pants:
		case InventorySlot.Boots:
			return new Rect(v.x, v.y, inventoryCellSize*2, inventoryCellSize*2);
		case InventorySlot.Zero:
		case InventorySlot.One:
		case InventorySlot.Two:
		case InventorySlot.Three:
		case InventorySlot.Four:
		case InventorySlot.Five:
		case InventorySlot.Six:
		case InventorySlot.Seven:
		case InventorySlot.Eight:
		case InventorySlot.Nine:
		case InventorySlot.Ten:
		case InventorySlot.Eleven:
		case InventorySlot.Twelve:
		case InventorySlot.Thirteen:
		case InventorySlot.Fourteen:
		case InventorySlot.Fifteen:
			return new Rect(v.x, v.y, inventoryCellSize, inventoryCellSize);
		default:
			return new Rect();
		}
	}
	const float baseY = 50.0f;
	static float baseX = paperDollFullWidth + 25.0f;
	const float armorWidth = inventoryCellSize * 7;
	const float change = inventoryCellSize - inventoryLineThickness;
	const float change2 = inventoryCellSize - inventoryLineThickness/2.0f;
	const float groundY = baseY + change*4 + inventoryCellSize;
	const float groundHeight = inventoryHeight - groundY;
	static float groundX = baseX + armorWidth;
	static float groundWidth = inventoryWidth + paperDollFullWidth - groundX - 3.0f;
	public Vector2 getInventorySlotPos(InventorySlot slot) {
		switch (slot) {
		case InventorySlot.Head:
			return new Vector2(baseX + change2*2, baseY);
		case InventorySlot.Shoulder:
			return new Vector2(baseX, baseY + change2);
		case InventorySlot.Back:
			return new Vector2(baseX + change2*4, baseY + change2);
		case InventorySlot.Chest:
			return new Vector2(baseX + change2*2, baseY + change2*4);
		case InventorySlot.Glove:
			return new Vector2(baseX + change2*4, baseY + change2*5);
		case InventorySlot.RightHand:
			return new Vector2(baseX + change2*.5f, baseY + change2*7);
		case InventorySlot.LeftHand:
			return new Vector2(baseX + change2*3.5f, baseY + change2*7);
		case InventorySlot.Pants:
			return new Vector2(baseX + change2*2, baseY + change2*9);
		case InventorySlot.Boots:
			return new Vector2(baseX + change2*.5f, baseY + change2*11);
		case InventorySlot.Zero:
			return new Vector2(baseX + armorWidth, baseY);
		case InventorySlot.One:
			return new Vector2(baseX + armorWidth + change, baseY);
		case InventorySlot.Two:
			return new Vector2(baseX + armorWidth + change*2, baseY);
		case InventorySlot.Three:
			return new Vector2(baseX + armorWidth + change*3, baseY);
		case InventorySlot.Four:
			return new Vector2(baseX + armorWidth, baseY + change);
		case InventorySlot.Five:
			return new Vector2(baseX + armorWidth + change, baseY + change);
		case InventorySlot.Six:
			return new Vector2(baseX + armorWidth + change*2, baseY + change);
		case InventorySlot.Seven:
			return new Vector2(baseX + armorWidth + change*3, baseY + change);
		case InventorySlot.Eight:
			return new Vector2(baseX + armorWidth, baseY + change*2);
		case InventorySlot.Nine:
			return new Vector2(baseX + armorWidth + change, baseY + change*2);
		case InventorySlot.Ten:
			return new Vector2(baseX + armorWidth + change*2, baseY + change*2);
		case InventorySlot.Eleven:
			return new Vector2(baseX + armorWidth + change*3, baseY + change*2);
		case InventorySlot.Twelve:
			return new Vector2(baseX + armorWidth, baseY + change*3);
		case InventorySlot.Thirteen:
			return new Vector2(baseX + armorWidth + change, baseY + change*3);
		case InventorySlot.Fourteen:
			return new Vector2(baseX + armorWidth + change*2, baseY + change*3);
		case InventorySlot.Fifteen:
			return new Vector2(baseX + armorWidth + change*3, baseY + change*3);
			
		default:
			return new Vector2();
		}
	}
	public Item selectedItem;
	public InventorySlot selectedItemWasInSlot;
	Vector3 selectedMousePos = new Vector3();
	Vector2 selectedItemPos = new Vector2();
	Vector2 selectedCell = new Vector2();
	public void selectItem() {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		foreach (InventorySlot slot in inventorySlots) {
			Rect r = getInventorySlotRect(slot);
			if (r.Contains(mousePos)) {
				Vector2 v = getIndexOfSlot(slot);
//				Debug.Log(v);
				int ind = getLinearIndexFromIndex(v);
				InventoryItemSlot sl = characterSheet.characterSheet.inventory.inventory[ind];
				InventoryItemSlot slR = sl.itemSlot;
				if (slR==null) break;
			//	Item i = slR.item;
				Vector2 itemSlot = characterSheet.characterSheet.inventory.getSlotForIndex(ind);
				ItemReturn ir = characterSheet.characterSheet.inventory.removeItemFromSlot(itemSlot);
				selectedItem = ir.item;
				if (mapGenerator.shiftDown) {
					if (selectedItem.stackSize()>1) {
						characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, itemSlot - ir.slot);
						selectedItem = selectedItem.popStack();
					}
				}
				selectedCell = ir.slot;
				selectedMousePos = mousePos;
//				selectedItemPos = getInventorySlotPos();
				selectedItemPos = getInventorySlotPos(inventorySlots[slR.index]);
				selectedItemWasInSlot = inventorySlots[slR.index];
				break;
			}
		}
		if (!gui.looting || mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight) return;
		Vector2 scrollOff = groundScrollPosition;
		float div = 20.0f;
		float y = div + groundY - scrollOff.y;
		float mid = groundX + groundWidth/2.0f + scrollOff.x;
	//	mousePos.y += groundScrollPosition.y;
		selectedItem = null;
		List<Item> groundItems = mapGenerator.tiles[(int)position.x,(int)-position.y].getReachableItems();
		foreach (Item i in groundItems) {
			if (i.inventoryTexture==null) continue;
		//	Debug.Log(mousePos.x + "  " + mousePos.y + "       " + mid + "  " + y);
			Vector2 size = i.getSize();
			float x = mid - size.x*inventoryCellSize/2.0f;
			Rect r = new Rect(x, y, size.x*inventoryCellSize, size.y*inventoryCellSize);
			if (r.Contains(mousePos)) {
			//	Debug.Log(i);
				selectedCell = new Vector2((int)((mousePos.x - x)/inventoryCellSize), (int)((mousePos.y - y)/inventoryCellSize));
				foreach (Vector2 cell in i.getShape()) {
					if (cell.x == selectedCell.x && cell.y == selectedCell.y) {
						selectedItemPos = new Vector2(x, y);
						selectedMousePos = mousePos;
						selectedItem = i;
						selectedItemWasInSlot = InventorySlot.None;
					}
				}
				Debug.Log(selectedCell);
				if (selectedItem!=null) {
					break;
				}
			}
			y += size.y*inventoryCellSize + div;
		}
	}
	public void deselectItem() {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		Tile t = mapGenerator.tiles[(int)position.x,(int)-position.y];
		foreach (InventorySlot slot in inventorySlots) {
			Rect r = getInventorySlotRect(slot);
			if (r.Contains(mousePos)) {
				Vector2 v2 = getIndexOfSlot(slot);
				Vector2 v = v2 - selectedCell;
				Debug.Log(v);
				if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, v)) {
					if (selectedItemWasInSlot == InventorySlot.None) {
						t.removeItem(selectedItem,1);
						minorsLeft--;
					}
					characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, v);
					selectedItem = null;
					return;
				}
				else {
					InventoryItemSlot invSlot = characterSheet.characterSheet.inventory.inventory[characterSheet.characterSheet.inventory.getIndexForSlot(v2)];
					Item invSlotItem = invSlot.getItem();
					if (invSlotItem != null && characterSheet.characterSheet.inventory.itemCanStackWith(invSlotItem, selectedItem)) {
						if (selectedItemWasInSlot == InventorySlot.None) {
							t.removeItem(selectedItem,1);
							minorsLeft--;
						}
						characterSheet.characterSheet.inventory.stackItemWith(invSlotItem,selectedItem);
						selectedItem = null;
						return;
					}
				}
				break;
			}
		}
		if (gui.looting && !(mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight)) {
			if (selectedItemWasInSlot!=InventorySlot.None && selectedItem!=null) {
				while (selectedItem.stackSize() > 1) t.addItem(selectedItem.popStack());
				t.addItem(selectedItem);
				minorsLeft--;
		//		characterSheet.characterSheet.inventory.removeItemFromSlot(getInventorySlotPos(selectedItemWasInSlot));
			}
		}
		else if (selectedItemWasInSlot!=InventorySlot.None) {
			if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot))) {
				characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot));
			}
			else {
				int slot1 = getLinearIndexFromIndex(getIndexOfSlot(selectedItemWasInSlot));
				if (slot1 > -1 && characterSheet.characterSheet.inventory.itemCanStackWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem)) {
					characterSheet.characterSheet.inventory.stackItemWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem);
				}
			}
		}
		selectedItem = null;
	}
	static GUIStyle courierStyle;
	static GUIStyle getCourierStyle(int size) {
		if (courierStyle == null) {
			courierStyle = new GUIStyle("Label");
			courierStyle.font = Resources.Load<Font>("Fonts/Courier New");
			courierStyle.normal.textColor = Color.black;
		}
		courierStyle.fontSize = size;
		return courierStyle;
	}

	public string getSmallCapsString(string original, int size) {
		string newS = "";
		char[] chars = original.ToCharArray();
		bool inLowerCase = false;
		foreach (char c in chars) {
			if (c >= 'a' && c <= 'z') {
				if (!inLowerCase) {
					newS += "<size=" + size + ">";
					inLowerCase = true;
				}
				int i = (int)c;
				i -= (int)'a';
				i += (int)'A';
				newS += ((char)i);
			}
			else {
				if (c == '\'' || c == '-') {
					if (!inLowerCase) {
						newS += "<size=" + size + ">";
						inLowerCase = true;
					}
				}
				else if (inLowerCase) {
					newS += "</size>";
					inLowerCase = false;
				}
				newS += c;
			}
		}
		if (inLowerCase) newS += "</size>";
		return newS;
	}
	static Rect kMax = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - 8.0f, bannerY + (bannerHeight - bottomSheetHeight) + kLeft + 120.0f, bottomSheetWidth, bottomSheetHeight);
	static Rect kMin = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - kLeft, bannerY + (bannerHeight - bottomSheetHeight) + kLeft + 10.0f, bottomSheetWidth, bottomSheetHeight);
	static Rect cMax = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - 4.0f, bannerY + (bannerHeight - bottomSheetHeight) + cLeft + 225.0f, bottomSheetWidth, bottomSheetHeight);
	static Rect cMin = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - cLeft, bannerY + (bannerHeight - bottomSheetHeight) + cLeft + 10.0f, bottomSheetWidth, bottomSheetHeight);


	static float t = 0;
	static int dir = 1;
	public void drawGUI() {
		float speed = 1.0f/3.0f;
		t += Time.deltaTime * speed * dir;
		Color start = Color.cyan;
		Color end = Color.black;
		float max = 0.9f;
		float min = 0.35f;
		if (t > max) {
			dir = -1;
			t = max;
		}
		if (t < min) {
			dir = 1;
			t = min;
		}
		if (GUI.Button(gui.getTabButtonRect(Tab.C), "C", GameGUI.getTabButtonRightStyle())) {
			gui.clickTab(Tab.C);
		}
		if (GUI.Button(gui.getTabButtonRect(Tab.K), "K", GameGUI.getTabButtonRightStyle())) {
			gui.clickTab(Tab.K);
		}
		string sizeString = "<size=10>";
		string sizeString12 = "<size=12>";
		string sizeString14 = "<size=14>";
		string sizeEnd = "</size>";
		string divString = "<size=6>\n\n</size>";
		string otherDivString = "<size=3>\n\n</size>";
		string divString2 = "<size=5>\n\n</size>";

		float y = 0;
		float x = 0;
		Rect kRect = new Rect (kMin.x + (kMax.x - kMin.x) * kPerc, kMin.y + (kMax.y - kMin.y) * kPerc, kMin.width + (kMax.width - kMin.width) * kPerc, kMin.height + (kMax.height - kMin.height) * kPerc);
		Rect cRect = new Rect (cMin.x + (cMax.x - cMin.x) * cPerc, cMin.y + (cMax.y - cMin.y) * cPerc, cMin.width + (cMax.width - cMin.width) * cPerc, cMin.height + (cMax.height - cMin.height) * cPerc);
		GUI.DrawTexture(kRect, bottomSheetTexture);

		y = kRect.y + 370.0f;
		x = 10.0f;
		GUIStyle statsStyle = getCourierStyle(18);
		string info = "L" + sizeString12 + "EVEL" + sizeEnd + ":" + sizeString14 + characterSheet.characterProgress.getCharacterLevel() + sizeEnd +
			"\n" + "E" + sizeString12 + "XPERIENCE" + sizeEnd + ":" + sizeString14 + characterSheet.characterProgress.getCharacterExperience() + "/" + (characterSheet.characterProgress.getCharacterLevel()*100) + sizeEnd +
				"\n" + getSmallCapsString(characterSheet.characterProgress.getCharacterClass().getClassName().ToString(), 12) +
				"\n" + getSmallCapsString(characterSheet.personalInfo.getCharacterRace().getRaceString(), 12) +
				"\n" + getSmallCapsString(characterSheet.personalInfo.getCharacterBackground().ToString(), 12);
		GUIContent infoContent = new GUIContent(info);
		Vector2 infoSize = statsStyle.CalcSize(infoContent);
		GUI.Label(new Rect(x, y, infoSize.x, infoSize.y), infoContent, statsStyle);
		x += infoSize.x + 15.0f;
		y -= 10.0f;

		GUIStyle statsTitleStyle = getCourierStyle(20);
		string classFeaturesTitleString = "C<size=15>LASS</size> F<size=15>EATURES</size>";
		GUIContent classFeaturesTitleContent = new GUIContent(classFeaturesTitleString);
		Vector2 classFeaturesTitleSize = statsTitleStyle.CalcSize(classFeaturesTitleContent);
		GUI.Label(new Rect(x + (kMax.x + kMax.width - x - classFeaturesTitleSize.x)/2.0f, y, classFeaturesTitleSize.x, classFeaturesTitleSize.y), classFeaturesTitleContent, statsTitleStyle);
		y += classFeaturesTitleSize.y + 5.0f;
		statsStyle = getCourierStyle(18);


		ClassFeature[] classFeatures = characterSheet.characterSheet.characterProgress.getClassFeatures();
		string classFeaturesString = "";
		foreach (ClassFeature classFeature in classFeatures) {
			if (classFeaturesString != "") classFeaturesString += "\n";
			classFeaturesString += getSmallCapsString(ClassFeatures.getName(classFeature), 12);
		}
		GUIContent featuresContent = new GUIContent(classFeaturesString);
		Vector2 featuresSize = statsStyle.CalcSize(featuresContent);
		classFeaturesScrollPos = GUI.BeginScrollView(new Rect(x, y, (kRect.x + kRect.width - x - 11), kRect.y + kRect.height - y - 30), classFeaturesScrollPos, new Rect(x, y, (kRect.x + kRect.width - x - 11) - 16.0f, featuresSize.y));
		GUI.Label(new Rect(x, y, featuresSize.x, featuresSize.y), featuresContent, statsStyle);
		GUI.EndScrollView();
		/*
		float featureX = paperDollFullWidth + 10.0f;
		GUIStyle featuresStyle = st;
		GUIContent c = new GUIContent("Feature");
		float featuresHeight = featuresStyle.CalcSize(c).y;
		float scrollHeight = featuresHeight * classFeatures.Length;
		float remainingHeight = skillsHeight - y;
		classFeaturesScrollPos = GUI.BeginScrollView(new Rect(paperDollFullWidth - 1.0f, y, skillsWidth, remainingHeight), classFeaturesScrollPos, new Rect(paperDollFullWidth - 1.0f, y, skillsWidth - (scrollHeight > remainingHeight ? 16.0f : 0.0f), scrollHeight));
		foreach (ClassFeature classFeature in classFeatures) {
			//	GUIContent feat = new GUIContent(classFeature.ToString());
			GUIContent feat = new GUIContent(ClassFeatures.getName(classFeature));
			Vector2 featSize = featuresStyle.CalcSize(feat);
			GUI.Label(new Rect(featureX, y, featSize.x, featSize.y), feat, featuresStyle);
			y += featuresHeight;
		}
		GUI.EndScrollView();

*/
		GUI.DrawTexture(cRect, bottomSheetTexture);

		y = cRect.y + 265.0f;
		x = 10.0f;
		statsTitleStyle = getCourierStyle(20);
		string characterStatsString = "C<size=15>HARACTER STATS</size>";
		GUIContent characterStatsContent = new GUIContent(characterStatsString);
		Vector2 characterStatsSize = statsTitleStyle.CalcSize(characterStatsContent);
		GUI.Label(new Rect((cRect.x + cRect.width)/2.0f - characterStatsSize.x/2.0f, y, characterStatsSize.x, characterStatsSize.y), characterStatsContent, statsTitleStyle);
		y += characterStatsSize.y + 5.0f;
		statsStyle = getCourierStyle(15);
		string typesString = otherDivString + "P" + sizeString + "HYSIQUE" + sizeEnd + "\n" + otherDivString +
			divString2 + otherDivString + "P" + sizeString + "ROWESS" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "M" + sizeString + "ASTERY" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "K" + sizeString + "NOWLEDGE" + sizeEnd + otherDivString;
		GUIContent typesContent = new GUIContent(typesString);
		Vector2 typesSize = statsStyle.CalcSize(typesContent);
		GUI.Label(new Rect(x, y, typesSize.x, typesSize.y), typesContent, statsStyle);
		x += typesSize.x + 20.0f;

		string statsString = "S" + sizeString + "TURDY" + sizeEnd + "\n" + characterSheet.abilityScores.getSturdy() + " (<size=13>MOD:" + characterSheet.combatScores.getInitiative() + "</size>)" +
			divString + "P" + sizeString + "ERCEPTION" + sizeEnd + "\n" + characterSheet.abilityScores.getPerception() + " (<size=13>MOD:" + characterSheet.combatScores.getCritical() + "</size>)" +
				divString + "T" + sizeString + "ECHNIQUE" + sizeEnd + "\n" + characterSheet.abilityScores.getTechnique() + " (<size=13>MOD:" + characterSheet.combatScores.getHandling() + "</size>)" +
				divString + "W" + sizeString + "ELL-VERSED" + sizeEnd + "\n" + characterSheet.abilityScores.getWellVersed() + " (<size=13>MOD:" + characterSheet.combatScores.getDominion() + "</size>)";
		GUIContent statsContent = new GUIContent(statsString);
		Vector2 statsSize = statsStyle.CalcSize(statsContent);
		GUI.Label(new Rect(x, y, statsSize.x, statsSize.y), statsContent, statsStyle);
		x += statsSize.x + 20.0f;

		string skillNamesString = "A" + sizeString + "THLETICS" + sizeEnd + ":\nM" + sizeString + "ELEE" + sizeEnd + ":" + 
			divString + "R" + sizeString + "ANGED" + sizeEnd + ":\nS" + sizeString + "TEALTH" + sizeEnd + ":" +
				divString + "M" + sizeString + "ECHANICAL" + sizeEnd + ":\nM" + sizeString + "EDICINAL" + sizeEnd + ":" +
				divString + "H" + sizeString + "ISTORICAL" + sizeEnd + ":\nP" + sizeString + "OLITICAL" + sizeEnd + ":";
		GUIContent skillNamesContent = new GUIContent(skillNamesString);
		Vector2 skillNamesSize = statsStyle.CalcSize(skillNamesContent);
		GUI.Label(new Rect(x, y, skillNamesSize.x, skillNamesSize.y), skillNamesContent, statsStyle);
		string skillStatsString = characterSheet.skillScores.getScore(Skill.Athletics) + "<size=13> =(" + characterSheet.combatScores.getInitiative() + "+" + (characterSheet.skillScores.getScore(Skill.Athletics) - characterSheet.combatScores.getInitiative()) + ")</size>\n" + characterSheet.skillScores.getScore(Skill.Melee) + "<size=13> =(" + characterSheet.combatScores.getInitiative() + "+" + (characterSheet.skillScores.getScore(Skill.Melee) - characterSheet.combatScores.getInitiative()) + ")</size>" + divString +
			characterSheet.skillScores.getScore(Skill.Ranged) + "<size=13> =(" + characterSheet.combatScores.getCritical() + "+" + (characterSheet.skillScores.getScore(Skill.Ranged) - characterSheet.combatScores.getCritical()) + ")</size>\n" + characterSheet.skillScores.getScore(Skill.Stealth) + "<size=13> =(" + characterSheet.combatScores.getCritical() + "+" + (characterSheet.skillScores.getScore(Skill.Stealth) - characterSheet.combatScores.getCritical()) + ")</size>" + divString +
				characterSheet.skillScores.getScore(Skill.Mechanical)  + "<size=13> =(" + characterSheet.combatScores.getHandling() + "+" + (characterSheet.skillScores.getScore(Skill.Mechanical) - characterSheet.combatScores.getHandling()) + ")</size>\n" + characterSheet.skillScores.getScore(Skill.Medicinal) + "<size=13> =(" + characterSheet.combatScores.getHandling() + "+" + (characterSheet.skillScores.getScore(Skill.Medicinal) - characterSheet.combatScores.getHandling()) + ")</size>" + divString +
				characterSheet.skillScores.getScore(Skill.Historical)  + "<size=13> =(" + characterSheet.combatScores.getDominion() + "+" + (characterSheet.skillScores.getScore(Skill.Historical) - characterSheet.combatScores.getDominion()) + ")</size>\n" + characterSheet.skillScores.getScore(Skill.Political) + "<size=13> =(" + characterSheet.combatScores.getDominion() + "+" + (characterSheet.skillScores.getScore(Skill.Political) - characterSheet.combatScores.getDominion()) + ")</size>";
		GUIContent skillStatsContent = new GUIContent(skillStatsString);
		Vector2 skillStatsSize = statsStyle.CalcSize(skillStatsContent);
		x += skillNamesSize.x + 10.0f;
		GUI.Label(new Rect(x, y, skillStatsSize.x, skillStatsSize.y), skillStatsContent, statsStyle);
		y += skillStatsSize.y + 5.0f;
		string armorClass = "A" + sizeString + "RMOR" + sizeEnd + " C" + sizeString + "LASS" + sizeEnd + ": " + characterSheet.characterSheet.characterLoadout.getAC();
		GUIContent armorClassContent = new GUIContent(armorClass);
		Vector2 armorClassSize = statsStyle.CalcSize(armorClassContent);
		GUI.Label(new Rect((cMax.x + cMax.width - armorClassSize.x)/2.0f, y, armorClassSize.x, armorClassSize.y), armorClassContent, statsStyle);
		if (gui.openTab == Tab.K) {
		//	GUI.DrawTexture(, bottomSheetTexture);
			
		}
		else {
		//	GUI.DrawTexture(, bottomSheetTexture);
			
		}
		if (gui.openTab == Tab.C) {
		//	GUI.DrawTexture(, bottomSheetTexture);
			
		}
		else {
		//	GUI.DrawTexture(, bottomSheetTexture);
			
		}


	//	while (t > 1) t--;
	/*	float healthX = -1.0f;
		float healthY = 0.0f;
		float healthCenter = -1.0f;
		float healthCompY = 0.0f;
		float compY = 25.0f;*/
		float tabButtonsY = 0.0f;
	//	int healthFont = 0;
		string playerText = "N<size=13>AME</size>/A<size=13>LIAS</size>:\n\"";
		string playerName = characterSheet.personalInfo.getCharacterName().fullName();
		playerText += getSmallCapsString(playerName, 13);

		playerText += "\"\n";
		playerText += "H<size=13>EALTH</size>:\n" + characterSheet.combatScores.getCurrentHealth() + "/" + characterSheet.combatScores.getMaxHealth() + "\n";
	//	GUIContent healthContent = new GUIContent(healthText);
		playerText += "C<size=13>OMPOSURE</size>:\n" + characterSheet.combatScores.getCurrentComposure() + "/" + characterSheet.combatScores.getMaxComposure() + "\n";
	//	GUIContent composureContent = new GUIContent(composureText);
		GUIContent playerContent = new GUIContent(playerText);
		if (gui.openTab == Tab.None || true) {
			Texture[] textures = getPaperDollTexturesHead();
			//			GUIStyle headStyle = getPaperDollHeadStyle();
			GUI.DrawTexture(new Rect(bannerX, bannerY, bannerWidth, bannerHeight), playerBannerTexture);
			foreach (Texture2D texture in textures) {
//			headStyle.normal.background = texture;
				GUI.DrawTexture(new Rect(24.0f, ((bannerHeight + bannerY) - paperDollHeadSize)/2.0f - 1.0f, paperDollHeadSize, paperDollHeadSize), texture);
	//			GUI.Label(new Rect(0.0f, 0.0f, paperDollHeadSize, paperDollHeadSize), "", headStyle);
			}
			GUI.DrawTexture(new Rect(15.0f, ((bannerHeight + bannerY) - portraitBorderSize)/2.0f, portraitBorderSize, portraitBorderSize), portraitBorderTexture);
//			GUI.DrawTexture(new Rect(paperDollHeadSize, 0.0f, bannerWidth, paperDollHeadSize + 1), getPaperDollHealthBannerTexture((int)bannerWidth, (int)paperDollHeadSize + 1));
	//		float healthX = 35.0f + paperDollHeadSize;
	//		float healthCompY = paperDollHeadSize/2.0f;
			tabButtonsY = bannerHeight + bannerY;
	//		GUIStyle healthStyle = getCourierStyle();
	//		GUIStyle compStyle = getCourierStyle();
	//		Vector2 healthSize = healthStyle.CalcSize(healthContent);
	//		Vector2 composureSize = compStyle.CalcSize(composureContent);
			//	float totalHeight = composureSize.y + healthSize.y;
	//		float healthY = healthCompY - healthSize.y;
	//		float compY = healthCompY;
	//		GUI.Label(new Rect(healthX, healthY, healthSize.x, healthSize.y), healthContent, healthStyle);
	//		GUI.Label(new Rect(healthX, compY, composureSize.x, composureSize.y), composureContent, compStyle);
			GUIStyle cStyle = getCourierStyle(20);
			Vector2 playerTextSize = cStyle.CalcSize(playerContent);
			float playerTextY = ((bannerHeight + bannerY) - playerTextSize.y)/2.0f;
			float playerTextX = 45.0f + paperDollHeadSize;
			GUI.Label(new Rect(playerTextX, playerTextY, playerTextSize.x, playerTextSize.y), playerContent, cStyle);
			if (GUI.Button(new Rect(bannerX + bannerWidth - 25.0f - bagButtonSize, bannerY + bannerHeight - bagButtonSize - 15.0f, bagButtonSize, bagButtonSize), "", getBagButtonStyle())) {
				gui.clickTab(Tab.I);
			}
		}
		else {
			tabButtonsY = paperDollFullHeight - 1;
			GUI.DrawTexture(new Rect(0.0f, 0.0f, paperDollFullWidth, paperDollFullHeight), getPaperDollFullBackgroundTexture((int)paperDollFullWidth, (int)paperDollFullHeight));
			y = 0.0f;
			GUIStyle textStyle = getPlayerInfoStyle();
			GUIContent name = new GUIContent(characterSheet.personalInfo.getCharacterName().fullName());
			Vector2 nameSize = textStyle.CalcSize(name);
			GUI.Label(new Rect((paperDollFullWidth - nameSize.x)/2.0f, y, nameSize.x, nameSize.y), name, textStyle);
			y += nameSize.y;
			Texture[] textures = getPaperDollTexturesFull();
			if (textures.Length >= 1) {
				Texture t1 = textures[0];
				float widths = paperDollFullWidth * 0.7f;
				float heights = widths * (t1.height/t1.width);
				float paperDollX = (paperDollFullWidth - widths)/2.0f;
				foreach (Texture2D texture in textures) {
					GUI.DrawTexture(new Rect(paperDollX, y, widths, heights), texture);
				}
				y += heights;
			}
	/*
			float healthCenter = paperDollFullWidth/2.0f;
//			healthCompY = y + 20.0f;
			float healthY = y;
			GUIStyle healthStyle = getHealthTextStyle(12);
			GUIStyle compStyle = getComposureTextStyle(12);
			Vector2 healthSize = healthStyle.CalcSize(healthContent);
			Vector2 composureSize = compStyle.CalcSize(composureContent);
		//	float totalHeight = composureSize.y + healthSize.y;
			//healthY = healthCompY - healthSize.y;
			float compY = healthY + healthSize.y - mid;
			GUI.Label(new Rect(healthCenter - healthSize.x/2.0f, healthY, healthSize.x, healthSize.y), healthContent, healthStyle);
			GUI.Label(new Rect(healthCenter - composureSize.x/2.0f, compY, composureSize.x, composureSize.y), composureContent, compStyle);
			y = compY + composureSize.y - mid;
*/
			float mid = 8.0f;
			GUIContent profession = new GUIContent(characterSheet.characterProgress.getCharacterClass().getClassName().ToString());
			Vector2 professionSize = textStyle.CalcSize(profession);
			GUI.Label(new Rect((paperDollFullWidth - professionSize.x)/2.0f, y, professionSize.x, professionSize.y), profession, textStyle);
			y += professionSize.y - mid;
			GUIContent race = new GUIContent(characterSheet.personalInfo.getCharacterRace().getRaceString());
			Vector2 raceSize = textStyle.CalcSize(race);
			GUI.Label(new Rect((paperDollFullWidth - raceSize.x)/2.0f, y, raceSize.x, raceSize.y), race, textStyle);
			y += raceSize.y - mid;
			GUIContent background = new GUIContent(characterSheet.personalInfo.getCharacterBackground().ToString());
			Vector2 backgroundSize = textStyle.CalcSize(background);
			GUI.Label(new Rect((paperDollFullWidth - backgroundSize.x)/2.0f, y, backgroundSize.x, backgroundSize.y), background, textStyle);
			y += backgroundSize.y;
			GUIContent level = new GUIContent("Level: " + characterSheet.characterProgress.getCharacterLevel());
			Vector2 levelSize = textStyle.CalcSize(level);
			GUI.Label(new Rect(5.0f, y, levelSize.x, levelSize.y), level, textStyle);
			GUIContent experience = new GUIContent(characterSheet.characterProgress.getCharacterExperience() + " exp");
			Vector2 experienceSize = textStyle.CalcSize(experience);
			GUI.Label(new Rect(paperDollFullWidth - experienceSize.x - 5.0f, y, experienceSize.x, experienceSize.y), experience, textStyle);
		}
		/*
		if (gui.openTab == Tab.M) {
			GUI.DrawTexture(new Rect(paperDollFullWidth - 1.0f, 0.0f, missionTabWidth + missionObjectivesWidth - 1.0f, missionTopHeight), getMissionTitleBackgroundTexture());
			GUI.DrawTexture(new Rect(paperDollFullWidth + missionTabWidth - 2.0f, missionTopHeight - 1.0f, missionObjectivesWidth, missionObjectivesHeight), getMissionObjectivesBackgroundTexture());

			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent missions = new GUIContent("Missions");
			GUIContent objectives = new GUIContent("Objectives");
			Vector2 missionsSize = titleStyle.CalcSize(missions);
			Vector2 objectivesSize = titleStyle.CalcSize(objectives);
			GUI.Label(new Rect(paperDollFullWidth + (missionTabWidth + missionObjectivesWidth - 1.0f - missionsSize.x)/2.0f, (missionTopHeight - missionsSize.y)/2.0f, missionsSize.x, missionsSize.y), missions, titleStyle);

		//	GUI.BeginScrollView(new Rect()

			GUI.Label(new Rect(paperDollFullWidth + missionTabWidth + (missionObjectivesWidth - 1.0f - objectivesSize.x)/2.0f, missionTopHeight, objectivesSize.x, objectivesSize.y), objectives, titleStyle);


			float y = missionTopHeight + objectivesSize.y + 20.0f;
			float x = paperDollFullWidth + missionTabWidth + 10.0f;
			float toggleHeight = 20.0f;
			float toggleWidth = 200.0f;
//			GUI.enabled = false;
			GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (gui.openMission == Mission.Optional ? true : false), "Main Objective");
			x += 20.0f;
			y += toggleHeight;
			GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), true, (gui.openMission == Mission.Primary ? "How you do it" : (gui.openMission == Mission.Secondary ? "Destroy Enemy" : "Enjoy the view")));
			y += toggleHeight;
			GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (gui.openMission != Mission.Secondary ? true : false), (gui.openMission == Mission.Primary ? "This too" : (gui.openMission == Mission.Secondary ? "Reinforcements" : "Daydream")));
			y += toggleHeight;
			GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (gui.openMission != Mission.Primary ? true : false), (gui.openMission == Mission.Primary ? "And this as well" : (gui.openMission == Mission.Secondary ? "Conquer" : "Eat Snacks")));
			y += toggleHeight;
			if (gui.openMission == Mission.Optional) {
				GUI.Toggle (new Rect(x, y, toggleWidth, toggleHeight), true, "Nap Time!");
				y += toggleHeight;
			}

			
			
			if (GUI.Button(new Rect(paperDollFullWidth - 1, missionTopHeight - 1.0f + (missionTabHeight - 1.0f) * 0, missionTabWidth, missionTabHeight), "Primary", (gui.openMission==Mission.Primary ? getselectedMissionButtonStyle() : getNonSelectedMissionButtonStyle()))) {
				gui.openMission = Mission.Primary;
			}
			if (GUI.Button(new Rect(paperDollFullWidth - 1, missionTopHeight - 1.0f + (missionTabHeight - 1.0f) * 1, missionTabWidth, missionTabHeight), "Secondary", (gui.openMission==Mission.Secondary ? getselectedMissionButtonStyle() : getNonSelectedMissionButtonStyle()))) {
				gui.openMission = Mission.Secondary;
			}
			if (GUI.Button(new Rect(paperDollFullWidth - 1, missionTopHeight - 1.0f + (missionTabHeight - 1.0f) * 2, missionTabWidth, missionTabHeight), "Optional", (gui.openMission==Mission.Optional ? getselectedMissionButtonStyle() : getNonSelectedMissionButtonStyle()))) {
				gui.openMission = Mission.Optional;
			}

		}
		else if (gui.openTab == Tab.T) {
			GUI.DrawTexture(fullTRect(), getTurnOrderBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent turnOrder = new GUIContent("Turn Order");
			Vector2 turnOrderSize = titleStyle.CalcSize(turnOrder);
			GUI.Label(new Rect(paperDollFullWidth + (turnOrderWidth - 1.0f - turnOrderSize.x)/2.0f, 0.0f, turnOrderSize.x, turnOrderSize.y), turnOrder, titleStyle);
			float y = turnOrderSize.y;
			int numPlayers = mapGenerator.priorityOrder.Count;
			int currentPlayer = mapGenerator.currentUnit;
			if (currentPlayer < 0) currentPlayer = 0;
			turnOrderScrollPos = GUI.BeginScrollView(new Rect(paperDollFullWidth, y, turnOrderWidth - 4.0f, paperDollFullHeight - y - 1.0f), turnOrderScrollPos, new Rect(paperDollFullWidth, y, turnOrderWidth - 16.0f - 4.0f, (numPlayers + 1) * (turnOrderSectionHeight - 1.0f) + 1.0f + 5.0f));

			GUIStyle st = getPlayerInfoStyle();
			st.wordWrap = false;
			float x = paperDollFullWidth + turnOrderTableX - 5.0f;
			GUIContent num = new GUIContent("Pos");
			Vector2 numSize = st.CalcSize(num);
			GUI.Label(new Rect(x + (turnOrderSectionHeight - numSize.x)/2.0f, y + (turnOrderSectionHeight - numSize.y)/2.0f, numSize.x, numSize.y), num, getPlayerInfoStyle());
			x += turnOrderSectionHeight - 1.0f;
			GUIContent name = new GUIContent("Name");
			Vector2 nameSize = st.CalcSize(name);
			GUI.Label(new Rect(x + (turnOrderNameWidth - nameSize.x)/2.0f, y + (turnOrderSectionHeight - nameSize.y)/2.0f, nameSize.x, nameSize.y), name, getPlayerInfoStyle());
			x += turnOrderNameWidth - 1.0f;
			GUIContent initiative = new GUIContent("Roll");
			Vector2 initiativeSize = st.CalcSize(initiative);
			GUI.Label (new Rect(x + (turnOrderSectionHeight - initiativeSize.x)/2.0f, y + (turnOrderSectionHeight - initiativeSize.y)/2.0f, initiativeSize.x, initiativeSize.y), initiative, getPlayerInfoStyle());
			y+=turnOrderSectionHeight;
			for (int n=0;n<numPlayers;n++) {
				int playerNum = (n + currentPlayer) % numPlayers;
				Unit player = mapGenerator.priorityOrder[playerNum];
				if (player == this) {
					st.normal.textColor = Color.Lerp (start, end, t);
					st.fontStyle = FontStyle.Bold;
				}
				x = paperDollFullWidth + turnOrderTableX - 5.0f;
				Rect r = new Rect(x, y, turnOrderSectionHeight, turnOrderSectionHeight);
			//	Rect r2 = new Rect(x + (turnOrderSectionHeight
			//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderSectionBackgroundTexture() : getTurnOrderSectionBackgroundTextureEnemy()));
				if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player))) {
					selectUnit(player);
				}
				num = new GUIContent("" + (playerNum + 1));
				numSize = st.CalcSize(num);
				GUI.Label(new Rect(x + (turnOrderSectionHeight - numSize.x)/2.0f, y + (turnOrderSectionHeight - numSize.y)/2.0f, numSize.x, numSize.y), num, getPlayerInfoStyle());
				x += turnOrderSectionHeight - 1.0f;
				r = new Rect(x, y, turnOrderNameWidth, turnOrderSectionHeight);
			//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderNameBackgroundTexture() : getTurnOrderNameBackgroundTextureEnemy()));
				if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderNameStyle(player))) {
					selectUnit(player);
				}
				name = new GUIContent(player.characterSheet.personalInfo.getCharacterName().fullName());
				nameSize = st.CalcSize(name);
				GUI.Label(new Rect(x + 3.0f, y + (turnOrderSectionHeight - nameSize.y)/2.0f, Mathf.Min(nameSize.x, turnOrderNameWidth - 4.0f), nameSize.y), name, getPlayerInfoStyle());
				x += turnOrderNameWidth - 1.0f;
				r = new Rect(x, y, turnOrderSectionHeight, turnOrderSectionHeight);
			//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderSectionBackgroundTexture() : getTurnOrderSectionBackgroundTextureEnemy()));
				if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player))) {
					selectUnit(player);
				}
				initiative = new GUIContent(player.getInitiative() + "");
				initiativeSize = st.CalcSize(initiative);
				GUI.Label (new Rect(x + (turnOrderSectionHeight - initiativeSize.x)/2.0f, y + (turnOrderSectionHeight - initiativeSize.y)/2.0f, initiativeSize.x, initiativeSize.y), initiative, getPlayerInfoStyle());
				y += turnOrderSectionHeight - 1.0f;
				if (player == this) {
					st.normal.textColor = Color.white;
					st.fontStyle = FontStyle.Normal;
				}
			}

			GUI.EndScrollView();
		}*/
		/*
		if (gui.openTab == Tab.C || true) {
			GUI.DrawTexture(fullCRect(), getCharacterStatsBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent characterStats = new GUIContent("Character Stats");
			characterStatsSize = titleStyle.CalcSize(characterStats);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth - 1.0f - characterStatsSize.x)/2.0f, 0.0f, characterStatsSize.x, characterStatsSize.y), characterStats, titleStyle);
		//	float y = turnOrderSize.y;
			float statX = paperDollFullWidth + 10.0f;
			float statWidth = 40.0f;
			float baseX = statX + statWidth;
			float baseWidth = 40.0f;
			float modX = baseX + baseWidth;
			float modWidth = baseWidth;
			y = characterStatsSize.y + 5.0f;
			GUIStyle st = getPlayerInfoStyle();
			string[] stats = new string[]{"", "STR", "PER", "TEC", "W-VER"};
			string[] bases = new string[]{"Base", "" + characterSheet.abilityScores.getSturdy(), "" + characterSheet.abilityScores.getPerception(), "" + characterSheet.abilityScores.getTechnique(), "" + characterSheet.abilityScores.getWellVersed()};
			string[] mods = new string[]{"Mod", "" + characterSheet.combatScores.getInitiative(), "" + characterSheet.combatScores.getCritical(), "" + characterSheet.combatScores.getHandling(), "" + characterSheet.combatScores.getDominion()};
			for (int n=0;n<stats.Length;n++) {
				GUIContent stat = new GUIContent(stats[n]);
				Vector2 statSize = st.CalcSize(stat);
				GUI.Label(new Rect(statX + (statWidth - statSize.x)/2.0f, y, statSize.x, statSize.y), stat, st);
				GUIContent baseContent = new GUIContent(bases[n]);
				Vector2 baseSize = st.CalcSize(baseContent);
				GUI.Label(new Rect(baseX + (baseWidth - baseSize.x)/2.0f, y, baseSize.x, baseSize.y), baseContent, st);
				GUIContent mod = new GUIContent(mods[n]);
				Vector2 modSize = st.CalcSize(mod);
				GUI.Label(new Rect(modX + (modWidth - modSize.x)/2.0f, y, modSize.x, modSize.y), mod, st);
				y += Mathf.Max(new float[]{statSize.y, modSize.y, baseSize.y});
			}
			y += 10.0f;
			GUIContent armorClass = new GUIContent("Armor Class: " + characterSheet.characterSheet.characterLoadout.getAC());
			Vector2 armorClassSize = st.CalcSize(armorClass);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth - armorClassSize.x)/2.0f, y, armorClassSize.x, armorClassSize.y), armorClass, st);
			y += armorClassSize.y + 10.0f;
			GUIContent healthTitle = new GUIContent("Health");
			GUIContent healthAmount = new GUIContent(characterSheet.combatScores.getMaxHealth() + "");
			GUIContent composureTitle = new GUIContent("Composure");
			GUIContent composureAmount = new GUIContent(characterSheet.combatScores.getMaxComposure() + "");
			Vector2 healthTitleSize = st.CalcSize(healthTitle);
			Vector2 healthSize = st.CalcSize(healthAmount);
			Vector2 composureTitleSize = st.CalcSize(composureTitle);
			Vector2 composureSize = st.CalcSize(composureAmount);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth/4.0f - healthTitleSize.x/2.0f), y, healthTitleSize.x, healthTitleSize.y), healthTitle, st);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth*2.0f/3.0f - composureTitleSize.x/2.0f), y, composureTitleSize.x, composureTitleSize.y), composureTitle, st);
			y += healthTitleSize.y;
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth/4.0f - healthSize.x/2.0f), y, healthSize.x, healthSize.y), healthAmount, st);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth*2.0f/3.0f - composureSize.x/2.0f), y, composureSize.x, composureSize.y), composureAmount, st);
		}
		 if (gui.openTab == Tab.K || true) {
			paperDollFullWidth += characterStatsWidth;
			GUI.DrawTexture(fullKRect(), getSkillsBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent skills = new GUIContent("Skills");
			Vector2 skillSize = titleStyle.CalcSize(skills);
			GUI.Label(new Rect(paperDollFullWidth + (skillsWidth - 1.0f - skillSize.x)/2.0f, 0.0f, skillSize.x, skillSize.y), skills, titleStyle);
			string[] skillCategories = new string[]{"Physique", "Prowess", "Mastery", "Knowledge"};
			string[] skillNames = new string[]{"Athletics","Melee","Ranged","Stealth","Mechanical","Medicinal","Historical","Political"};
			string[] skillScores = new string[]{"" + characterSheet.skillScores.getScore(Skill.Athletics),"" + characterSheet.skillScores.getScore(Skill.Melee),"" + characterSheet.skillScores.getScore(Skill.Ranged),"" + characterSheet.skillScores.getScore(Skill.Stealth),"" + characterSheet.skillScores.getScore(Skill.Mechanical),"" + characterSheet.skillScores.getScore(Skill.Medicinal),"" + characterSheet.skillScores.getScore(Skill.Historical),"" + characterSheet.skillScores.getScore(Skill.Political)};
			float skillCategoryX = paperDollFullWidth + 5.0f;
			float skillCategoryWidth = 80.0f;
			float skillNameX = skillCategoryX + skillCategoryWidth;
			float skillNameWidth = 100.0f;
			float skillScoreX = skillNameX + skillNameWidth;
			float skillScoreWidth = 30.0f;
			GUIStyle st = getPlayerInfoStyle();
			y = skillSize.y + 5.0f;
			for (int n=0;n<skillCategories.Length;n++) {
				GUIContent skillCategory = new GUIContent(skillCategories[n]);
				Vector2 skillCategorySize = st.CalcSize(skillCategory);
				GUIContent skillName1 = new GUIContent(skillNames[n * 2]);
				Vector2 skillNameSize1 = st.CalcSize(skillName1);
				GUIContent skillName2 = new GUIContent(skillNames[n * 2 + 1]);
				Vector2 skillNameSize2 = st.CalcSize(skillName2);
				GUIContent skillScore1 = new GUIContent(skillScores[n * 2]);
				Vector2 skillScoreSize1 = st.CalcSize(skillScore1);
				GUIContent skillScore2 = new GUIContent(skillScores[n * 2 + 1]);
				Vector2 skillScoreSize2 = st.CalcSize(skillScore2);
				float namesHeight = skillNameSize1.y + skillNameSize2.y;
				GUI.Label(new Rect(skillCategoryX, y + (namesHeight - skillCategorySize.y)/2.0f, skillCategorySize.x, skillCategorySize.y), skillCategory, st);
				GUI.Label(new Rect(skillNameX + (skillNameWidth - skillNameSize1.x)/2.0f, y, skillNameSize1.x, skillNameSize1.y), skillName1, st);
				GUI.Label(new Rect(skillScoreX + (skillScoreWidth - skillScoreSize1.x)/2.0f, y, skillScoreSize1.x, skillScoreSize1.y), skillScore1, st);
				y += skillNameSize1.y;
				GUI.Label(new Rect(skillNameX + (skillNameWidth - skillNameSize2.x)/2.0f, y, skillNameSize2.x, skillNameSize2.y), skillName2, st);
				GUI.Label(new Rect(skillScoreX + (skillScoreWidth - skillScoreSize2.x)/2.0f, y, skillScoreSize2.x, skillScoreSize2.y), skillScore2, st);
				y += skillNameSize2.y + 10.0f;
			}
			GUI.DrawTexture(new Rect(paperDollFullWidth - 1.0f, y, skillsWidth, 2.0f), getSkillsMidSectionTexture());
			y += 2.0f;

			paperDollFullWidth -= characterStatsWidth;
		}*/
		if (gui.openTab == Tab.I) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			GUI.DrawTexture(fullIRect(), getInventoryBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent armour = new GUIContent("Armor");
			Vector2 armourSize = titleStyle.CalcSize(armour);
			GUI.Label(new Rect(paperDollFullWidth - 1.0f + inventoryWidth/3.0f - armourSize.x/2.0f, 0.0f, armourSize.x, armourSize.y), armour, titleStyle);
			GUIContent inventory = new GUIContent("Inventory");
			Vector2 inventorySize = titleStyle.CalcSize(inventory);
			GUI.Label(new Rect(paperDollFullWidth - 1.0f + inventoryWidth*2.0f/3.0f - inventorySize.x/2.0f, 0.0f, inventorySize.x, inventorySize.y), inventory, titleStyle);
		
//			foreach (CharacterInfo.InventoryItemSlot slot in characterSheet.characterSheet.inventory.inventory) {

//			}
			
			foreach (InventorySlot slot in inventorySlots) {
				Rect r = getInventorySlotRect(slot);
				if (r.Contains(mousePos)) {
					GUI.DrawTexture(r, getInventoryHoverBackground());
					if (selectedItem!=null) {
						Vector2 startPos = getIndexOfSlot(slot);
						foreach(Vector2 cell in selectedItem.getShape()) {
							Vector2 pos = startPos;
							pos.x += cell.x - selectedCell.x;
							pos.y += cell.y - selectedCell.y;
							if (pos.x == startPos.x && pos.y == startPos.y) continue;
						//	Debug.Log(startPos + "   " + pos);
							InventorySlot newSlot = getInventorySlotFromIndex(pos);
							if (newSlot != InventorySlot.None) {
								Rect r2 = getInventorySlotRect(newSlot);
								GUI.DrawTexture(r2, getInventoryHoverBackground());
							}
						}
					}
					break;
				}
			}
			foreach (InventorySlot slot in armorSlots) {
				Rect r = getInventorySlotRect(slot);
				if (r.Contains(mousePos)) {
					GUI.DrawTexture(r, getArmorHoverBackground());
					break;
				}
			}
			foreach (InventorySlot slot in armorSlots) {
				Rect r = getInventorySlotRect(slot);
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y+ inventoryCellSize,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				Item i = characterSheet.characterSheet.characterLoadout.getItemInSlot(slot);
				if (i != null && i.inventoryTexture != null) {
					float w = i.getSize().x*inventoryCellSize;
					float h = i.getSize().y*inventoryCellSize;
					x = r.x;
					y = r.y;
					if (h > r.height) y = r.y + r.height - h;
					else y = r.y + (r.height - h)/2.0f;
					x = r.x + (r.width - w)/2.0f;
					GUI.DrawTexture(new Rect(x, y, w, h), i.inventoryTexture);
				}
			}
			foreach (InventorySlot slot in inventorySlots) {
				Rect r = getInventorySlotRect(slot);
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
			}
			GUIStyle stackSt = getStackStyle();
			foreach (InventorySlot slot in inventorySlots) {
				Vector2 vec = getIndexOfSlot(slot);
				int ind = getLinearIndexFromIndex(vec);
				InventoryItemSlot isl = characterSheet.characterSheet.inventory.inventory[ind];
				Item i = isl.item;
				if (i == null) continue;
				Vector2 origin = getInventorySlotPos(slot);
				Vector2 size = i.getSize();
				GUI.DrawTexture(new Rect(origin.x,origin.y, size.x*inventoryCellSize,size.y*inventoryCellSize),i.inventoryTexture);
				if (i.stackSize()>1) {
					Vector2 bottomRight = i.getBottomRightCell();
					bottomRight.x *= inventoryCellSize - inventoryLineThickness;
					bottomRight.y *= inventoryCellSize - inventoryLineThickness;
					Vector2 stackPos = origin + bottomRight;
					GUIContent content = new GUIContent("" + i.stackSize());
					GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
				}
			}
			if (gui.looting || true) {
				List<Item> groundItems = mapGenerator.tiles[(int)position.x,(int)-position.y].getReachableItems();
			//	Debug.Log("ground Items: " + groundItems.Count + "   " + groundItems);
				float div = 20.0f;
				float height = div;
				foreach (Item i in groundItems) {
					if (i.inventoryTexture==null) continue;
					height += i.getSize().y*inventoryCellSize + div;
				}
				groundScrollPosition = GUI.BeginScrollView(new Rect(groundX, groundY, groundWidth, groundHeight), groundScrollPosition, new Rect(groundX, groundY, groundWidth-20.0f, height));
				y = div + groundY;
				float mid = groundX + groundWidth/2.0f;
				foreach (Item i in groundItems) {
					if (i.inventoryTexture==null) continue;
					Vector2 size = i.getSize();
					if (i!=selectedItem) {
						GUI.DrawTexture(new Rect(mid - size.x*inventoryCellSize/2.0f, y, size.x*inventoryCellSize, size.y*inventoryCellSize), i.inventoryTexture);
					}
					y += size.y*inventoryCellSize + div;
				}
				GUI.EndScrollView();
			}
			if (selectedItem != null) {
				Vector2 size = selectedItem.getSize();
				Vector2 pos = selectedItemPos;
				pos.y += (mousePos.y - selectedMousePos.y);
				pos.x += (mousePos.x - selectedMousePos.x);
				GUI.DrawTexture(new Rect(pos.x, pos.y,size.x*inventoryCellSize, size.y*inventoryCellSize), selectedItem.inventoryTexture);
				if (selectedItem.stackSize()>1) {
					Vector2 bottomRight = selectedItem.getBottomRightCell();
					bottomRight.x *= inventoryCellSize - inventoryLineThickness;
					bottomRight.y *= inventoryCellSize - inventoryLineThickness;
					Vector2 stackPos = pos + bottomRight;
					GUIContent content = new GUIContent("" + selectedItem.stackSize());
					GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
				}
			}

		}/*
		if (GUI.Button(new Rect((tabButtonsWidth-1)*0, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "M",(gui.openTab == Tab.M ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
		//	if (gui.openTab == Tab.M) gui.openTab = Tab.None;
		//	else gui.openTab = Tab.M;
			gui.clickTab(Tab.M);
		}*/
//		if (GUI.Button(new Rect((tabButtonsWidth-1)*0, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "C",(gui.openTab == Tab.C ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {

/*		if (GUI.Button(new Rect((tabButtonsWidth-1)*2, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "I",(gui.openTab == Tab.I ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			gui.clickTab(Tab.I);
		}*/
		/*
		if (GUI.Button(new Rect((tabButtonsWidth-1)*4, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "T",(gui.openTab == Tab.T ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			gui.clickTab(Tab.T);
		}*/
		string tt = GUI.tooltip;
		if (tt != null && tt!="") {
			int num = int.Parse(tt);
			if (hovering != null) hovering.removeHovering();
			hovering = mapGenerator.priorityOrder[num];
			hovering.setHovering();
		}
		else if (hovering != null) {
			hovering.removeHovering();
		}
	}

	void selectUnit(Unit player) {
		if (player != mapGenerator.selectedUnit) {
			mapGenerator.deselectAllUnits();
			mapGenerator.selectUnit(player, false);
			if (player.transform.parent == mapGenerator.playerTransform || player.transform.parent == mapGenerator.enemyTransform)
				mapGenerator.moveCameraToSelected(false);
		}
	}

	static Unit hovering = null;
	bool isHovering = false;
	void setHovering() {
		isHovering = true;
	}
	void removeHovering() {
		isHovering = false;
	}
	void OnGUI() {
//		return;
	//	if (attackEnemy && mapGenerator.getCurrentUnit() == this) {
		return;
		if (mapGenerator.selectedUnit == this || (mapGenerator.selectedUnit && mapGenerator.selectedUnit.attackEnemy == this)) {
			float totalWidth = Screen.width * 0.7f;
			float x = (Screen.width - totalWidth)/2.0f;
			float y = 10.0f + (mapGenerator.selectedUnit == this ? 0.0f : healthHeight + 10.0f);
			float height = healthHeight;
			//			float healthWidth = Mathf.Min(Mathf.Max(totalWidth * (((float)attackEnemy.hitPoints)/((float)attackEnemy.maxHitPoints)), 0.0f), totalWidth);
			float healthWidth = Mathf.Min(Mathf.Max(totalWidth * (((float)hitPoints)/((float)maxHitPoints)), 0.0f), totalWidth);
			//	GUI.BeginGroup(new Rect(x, y, totalWidth, height));
		//	createStyle();
		//	redStyle.normal.background = makeTex((int)totalWidth, (int)height, Color.red);
			GUIStyle red = getRedStyle(totalWidth);
			GUI.Box(new Rect(x, y, totalWidth, height), "", red);
			//	currentStyle.normal.background = makeTex((int)healthWidth, (int)height, Color.green)
			//			if (heal
			if (healthWidth > 0) {
				//greenStyle.normal.background = makeTex((int)healthWidth, (int)height, Color.green);
				GUI.Box(new Rect(x, y, healthWidth, height), "", getGreenStyle((int)healthWidth));
			}

			//	GUI.EndGroup();
		}
	}
	
	Texture2D makeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	
	public void setRotatingPath() {
		setRotationFrom((Vector2)currentPath[0],(Vector2)currentPath[1]);
	}
	
	public void setRotationToAttackEnemy() {
		if (attackEnemy != null) {
			setRotationToCharacter(attackEnemy);
		}
	}

	public void setRotationToAttackedByCharacter() {
		if (attackedByCharacter != null) {
			setRotationToCharacter(attackedByCharacter);
		}
	}
	
	public void setRotationFrom(Vector2 from, Vector2 to) {
		rotateFrom = from;
		rotateTo = to;
		rotating = true;
	}
	
	public void setRotationToCharacter(Unit enemy) {
		setRotationFrom(new Vector2(position.x + .001f, position.y), new Vector2(enemy.position.x, enemy.position.y));
	}

	public void setRotationToTile(Vector2 tile) {
		setRotationFrom(new Vector2(position.x + .001f, position.y), tile);
	}

	public void setRotationToTile(Tile t) {
		setRotationToTile(new Vector2(t.x, -t.y));
	}

	public void setRotationToMostInterestingTile() {	
		Tile t = mapGenerator.getMostInterestingTile(this);
		if (t != null)
			setRotationToTile(t);
	}
	
	void rotateBy(float rotateDist) {
		float midSlope = (rotateTo.y - rotateFrom.y)/(rotateTo.x - rotateFrom.x);
		float rotation = Mathf.Atan(midSlope) + Mathf.PI/2.0f;
		Vector3 rot1 = transform.eulerAngles;
		if (rotateTo.x > rotateFrom.x) {
			rotation += Mathf.PI;
		}
		rotation *= 180.0f / Mathf.PI;
		//		rot1.z = rotation;
		//		transform.eulerAngles = rot1;
		float rotation2 = rotation - 360.0f;
		float rotation3 = rotation + 360.0f;
		//	if (rotation == 0.0f) rotation2 = 360.0f;
		float difference1 = Mathf.Abs(rotation - rot1.z);
		float difference2 = Mathf.Abs(rotation2 - rot1.z);
		float difference3 = Mathf.Abs(rotation3 - rot1.z);
		float move1 = rotation - rot1.z;
		float move2 = rotation2 - rot1.z;
		float move3 = rotation3 - rot1.z;
		float sign1 = sign(move1);
		float sign2 = sign(move2);
		float sign3 = sign(move3);
		float s = sign1;
		float m = move1;
		float d = difference1;
		if (difference2 < d) {// || difference1 > 180.0f) {
	//		Debug.Log("Use 2!!");
			s = sign2;
			m = move2;
			d = difference2;
		}
		if (difference3 < d) {
			s = sign3;
			m = move3;
			d = difference3;
		}
		if (d <= rotateDist) {
			rot1.z = rotation;
			rotating = false;
		}
		else {
			rot1.z += rotateDist * s;
		}
		if (rot1.z <= 0) rot1.z += 360.0f;
		transform.eulerAngles = rot1;
		//		Debug.Log("Rotate Dist: " + rotateDist + " r1: " + rotation + " r2: " + rotation2 + "  m1: " + move1 + " m2: " + move2);
		//		rotating = false;
	}

	public virtual bool canAttOpp() {
		return !deadOrDyingOrUnconscious();
	}

	public int attackOfOpp(Vector2 one) {
		int move = 0;
		for (int n=-1;n<=1;n++) {
			for (int m=-1;m<=1;m++) {
				if ((n==0 && m==0) || !(n==0 || m==0)) continue;
				int x = (int)one.x + n;
				int y = (int)one.y + m;
				if (x >=0 && y >= 0 && x < mapGenerator.actualWidth && y < mapGenerator.actualHeight) {
					Tile t = mapGenerator.tiles[x,y];
					if (t.hasEnemy(this) && t.getEnemy(this).canAttOpp()) {
					
						move++;
						Unit enemy = t.getEnemy(this);
						enemy.attackEnemy = this;
						//								enemy.setRotationToAttackEnemy();
						
					//	enemy.setRotationToCharacter(this);
						enemy.setRotationToTile(new Vector2(one.x,-one.y));
						enemy.attackAnimation();
						enemy.attackAnimating = true;
						//								attacking = false;
						//								usedStandard = true;
						//		if (currentMoveDist < maxMoveDist) {
						//			usedMovement = true;
						//			currentMoveDist = 0;
						//		}
						
					}
				}
			}
		}
		return move;
	}

	public void recover() {
		if (isProne())
			afflictions.Remove(Affliction.Prone);
	//	affliction ^= Affliction.Prone;
	//	affliction = Affliction.None;
		usedMovement = true;
	}

	public bool isPerformingAnAction() {
		return attackAnimating || attacking || throwing || moving || intimidating;
	}

	public void startAttacking() {
		if (attackEnemy!=null && !moving) {
			attacking = true;
			if (mapGenerator.getCurrentUnit()==this)
				mapGenerator.moveCameraToPosition(transform.position, false, 90.0f);
		}
	}

	public bool throwing = false;
	public void startThrowing() {
		if (attackEnemy != null && !throwing) {
			throwing = true;
			if (mapGenerator.getCurrentUnit()==this)
				mapGenerator.moveCameraToPosition(transform.position, false, 90.0f);
	//		if (this == mapGenerator.getCurrentUnit())
	//			mapGenerator.resetRanges();
		}
	}

	public bool intimidating = false;
	public void startIntimidating() {
		if (attackEnemy != null && !intimidating) {
			intimidating = true;
			if (mapGenerator.getCurrentUnit()==this)
				mapGenerator.moveCameraToPosition(transform.position, false, 90.0f);
		}
	}
	
	public void startMoving(bool backStepping) {
	
		if (currentPath.Count <= 1) return;
		shouldDoAthleticsCheck = true;
		shouldCancelMovement = false;
		if (!backStepping) {
			moveAnimation(true);
		}
		//					p.rotating = true;
		if (mapGenerator.getCurrentUnit()==this)
			mapGenerator.moveCameraToPosition(transform.position, false, 90.0f);
		setRotatingPath();
		shouldMove = 0;
		if (!backStepping) {
			shouldMove = attackOfOpp((Vector2)currentPath[0]);
		}
		startMovingActually();
	//	if (shouldMove == 0) startMovingActually();
	}

	public void startMovingActually() {
		moving = true;
		removeTrail();
	}

	void moveBy(float moveDist, bool attopp) {
		Vector2 one = (Vector2)currentPath[1];
		Vector2 zero = (Vector2)currentPath[0];
		zero = new Vector2(transform.localPosition.x - 0.5f, -transform.localPosition.y - 0.5f);
		float directionX = sign(one.x - zero.x);
		float directionY = -sign(one.y - zero.y);
		//				directionX = Mathf.s
		float dist = Mathf.Max(Mathf.Abs(one.x - zero.x),Mathf.Abs(one.y - zero.y));
		if (dist <= 0.5f && doAttOpp && currentPath.Count >= 3 && attopp) {
			attackOfOpp(one);
			doAttOpp = false;
		}
		//		float distX = one.x - zero.x;
		//		float distY = one.y - zero.y;
		if (Mathf.Abs(dist - moveDist) <= 0.001f || moveDist >= dist) {
			//			moving = false;
			unDrawGrid();
			setNewTilePosition(new Vector3(one.x,-one.y,0.0f));
			position = new Vector3(one.x, -one.y, 0.0f);
			transform.localPosition = new Vector3(one.x + 0.5f, -one.y - 0.5f, 0.0f);
			currentPath.RemoveAt(0);
			moveDist = moveDist - dist;
			currentMoveDist--;
			moveDistLeft--;
			currentMaxPath = currentPath.Count - 1;
			mapGenerator.setCurrentUnitTile();
			shouldDoAthleticsCheck = true;
			doAttOpp = true;
			if (currentPath.Count >= 2) {
				setRotatingPath();
				//		attacking = true;
			}
			else {
				//		Debug.Log("count less than 2");
				if (attacking && attackEnemy) {
					//			Debug.Log("Gonna set rotation");
					//					setRotationFrom(position, attackEnemy.position)
					setRotationToAttackEnemy();
				}
			}
			redrawGrid();
			//	if (currentPath.Count >= 2 && moving) 
			//		moveBy(moveDist);
		}
		else {
			Vector3 pos = transform.localPosition;
			pos.x += directionX*moveDist;
			pos.y += directionY*moveDist;
			transform.localPosition = pos;
			//			transform.Translate(new Vector3(directionX * moveDist, directionY * moveDist, 0.0f));
		}
		//	Vector2 dist = new Vector2(currentPath[1].x - currentPath[0].x, currentPath[1].y - currentPath[0].y);
		//	Vector2 actualDist = dist;
	}
	
	void unDrawGrid() {
//		mapGenerator.resetAroundPlayer(this, viewDist);
//		mapGenerator.resetAroundCharacter(this);
	}
	
	void redrawGrid() {
		if (mapGenerator.getCurrentUnit() != this) return;
		if (currentMoveDist == 0) {
			mapGenerator.resetMoveDistances();
		}
//		mapGenerator.resetPlayerPath();
//		mapGenerator.setPlayerPath(currentPath);
//		mapGenerator.setAroundPlayer(this, currentMoveDist, viewDist, attackRange);
		mapGenerator.resetRanges();
		mapGenerator.resetPlayerPath();
		mapGenerator.setPlayerPath(currentPath);
		mapGenerator.setAroundCharacter(this);
	}
	
	float sign(float num) {
		if (Mathf.Abs(num) < 0.0001f) return 0.0f;
		if (num > 0) return 1.0f;
		return -1.0f;
	}


	// Use this for initialization
	void Start () {
		initializeVariables();
	}

	Transform target = null;
	public Transform getTarget() {
		if (target==null) {
			target = transform.FindChild("Target");
		}
		return target;
	}

	public SpriteRenderer getTargetSprite() {
		if (targetSprite == null) {
			targetSprite = getTarget().GetComponent<SpriteRenderer>();
		}
		return targetSprite;
	}

	SpriteRenderer circleSprite;
	public SpriteRenderer getCircleSprite() {
		if (circleSprite == null) {
			circleSprite = transform.FindChild("Circle").GetComponent<SpriteRenderer>();
		}
		return circleSprite;
	}

	public bool characterSheetLoaded = false;
	public void loadCharacterSheet() {
		if (characterSheetLoaded) return;
		characterSheetLoaded = true;
		characterSheet.unit = this;
		characterSheet.loadData();
	}

	public void loadCharacterSheetFromTextFile(string textFile) {
		if (characterSheetLoaded) return;
		characterSheet.unit = this;
		characterSheet.loadCharacterFromTextFile(textFile);
		characterSheetLoaded = true;
	}

	public void setMapGenerator(MapGenerator mg) {
		mapGenerator = mg;
		if (!playerControlled) {
			aiMap = new AStarEnemyMap(this, mapGenerator);
		}
	}

	public void removeTurret(TurretUnit tu) {
		if (turrets.Contains(tu)) turrets.Remove(tu);
	}

	public void addTurret(TurretUnit tu) {
		turrets.Add(tu);
	}
	
	static GUIStyle bagButtonStyle;
	static GUIStyle getBagButtonStyle() {
		if (bagButtonStyle == null) {
			bagButtonStyle = new GUIStyle("Button");
			bagButtonStyle.active.background = bagButtonStyle.normal.background = bagButtonStyle.hover.background = Resources.Load<Texture>("UI/bag-button") as Texture2D;
		}
		return bagButtonStyle;
	}
	static Texture2D playerBannerTexture;
	static Texture2D bottomSheetTexture;
	static Texture2D portraitBorderTexture;
	public virtual void initializeVariables() {
//		characterSheet = gameObject.GetComponent<Character>();
		playerBannerTexture = Resources.Load<Texture>("UI/bottom-sheet") as Texture2D;
		bottomSheetTexture = Resources.Load<Texture>("UI/bottom-sheet-long") as Texture2D;
		portraitBorderTexture = Resources.Load<Texture>("UI/portrait-border") as Texture2D;
		turrets = new List<TurretUnit>();
		afflictions = new List<Affliction>();
		loadCharacterSheet();
		hitPoints = maxHitPoints;
		moving = false;
		attacking = false;
		rotating = false;
		isCurrent = false;
		currentMoveDist = 0;
		attackRange = 1;
		viewDist = 11;
		maxMoveDist = 5;
		moveDistLeft = maxMoveDist;
		anim = gameObject.GetComponent<Animator>();
		currentMaxPath = 0;
		if (trail) {
			TrailRenderer tr = trail.GetComponent<TrailRenderer>();
			tr.sortingOrder = MapGenerator.trailOrder;
		}
		if (isCurrent) {
			addTrail();
		}
	}
	
	// Update is called once per frame
	void Update () {
		doMovement();
		doRotation();
		doAttack();
		doThrow();
		doGetThrown();
		doIntimidate();
		doDeath();
		setLayer();
		setTargetObjectScale();
		setTrailRendererPosition();
		setCircleScale();
	}


	public void doTurrets() {
		Debug.Log("DoTurrets " + turrets.Count);
		foreach (TurretUnit t in turrets) {
			t.fire();
		}
	}

	void setLayer() {
		if (!mapGenerator.isInCharacterPlacement()) {
			renderer.sortingOrder = ((moving || attacking || attackAnimating ? MapGenerator.playerMovingOrder : MapGenerator.playerNormalOrder));
			setAllSpritesToRenderingOrder((moving || attacking || attackAnimating ? MapGenerator.playerMovingArmorOrder : MapGenerator.playerArmorOrder));
			transform.FindChild("Circle").renderer.sortingOrder = ((moving || attacking || attackAnimating ? MapGenerator.circleMovingOrder : MapGenerator.circleNormalOrder));
//			transform.FindChild("Circle").renderer.sortingOrder = (renderer.sortingOrder == 11 ? 5 : 4);
		}
	}

	void doMovement() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (moving && shouldMove == 0) {// && !beingAttacked) {
		//	if (wasBeingAttacked) {
		//		setRotatingPath();
		//	}
			if (shouldDoAthleticsCheck) {
				if (currentPath.Count >= 2) {
					Vector2 from = (Vector2)currentPath[0];
					Vector2 to = (Vector2)currentPath[1];
					Direction dir = Tile.directionBetweenTiles(from,to);
					Tile t = mapGenerator.tiles[(int)from.x,(int)from.y];
					int passability = t.passabilityInDirection(dir);
					if (passability > 1) {
						int athletics = characterSheet.skillScores.getScore(Skill.Athletics);
						int check = characterSheet.rollForSkill(Skill.Athletics);
						if (check >= passability) {
							gui.log.addMessage(getName() + " passed Athletics check with a roll of " + check + " (" + (check - athletics) + " + " + athletics + ")");
						}
						else {
							gui.log.addMessage(getName() + " failed Athletics check with a roll of " + check + " (" + (check - athletics) + " + " + athletics + ") and became prone.");
							shouldCancelMovement = true;
							becomeProne();
							mapGenerator.resetPlayerPath();
							mapGenerator.resetRanges();
							usedMovement = true;
						}
						minorsLeft--;
					}
				}
				shouldDoAthleticsCheck = false;
			}
			if (currentPath.Count >= 2 && !shouldCancelMovement) {
				float speed = 2.0f;
				speed = 4.0f;
				float time = Time.deltaTime;
				float moveDist = time * speed;
				moveBy(moveDist, true);
				if (this == mapGenerator.getCurrentUnit())
					mapGenerator.moveCameraToSelected(true);
			}
			else {
				moveAnimation(false);
				moving = false;
				shouldCancelMovement = false;
				addTrail();
				currentPath = new ArrayList();
				currentPath.Add(new Vector2(position.x, -position.y));
				if (currentMoveDist == 0) usedMovement = true;
				setRotationToMostInterestingTile();
				if (!usedStandard && closestEnemyDist() <= characterSheet.characterLoadout.rightHand.getWeapon().range) {
					gui.selectAttack();
				}
			}
		}
	}

	void doRotation() {
		if (rotating) {
			float speed = 180.0f*3.0f;// + 20.0f;
			float time = Time.deltaTime;
			float rotateDist = time * speed;
			//			float rotateGoal = (rotateTo.
			rotateBy(rotateDist);
			Transform targ = getTarget();
			if (targ!=null) {
				targ.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			}
		}
	}

	public bool throwAnimating = false;
	void doThrow() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (throwing && !moving && !attacking) {
			throwAnimation();
			throwAnimating = true;
			throwing = false;
			usedStandard = true;
			if (moveDistLeft < maxMoveDist) {
				usedMovement = true;
				currentMoveDist = 0;
				moveDistLeft = 0;
			}
		}
	}

	Direction directionOf(Unit u) {
		Direction dir = Direction.Down;
		if (u.position.x > position.x) dir = Direction.Right;
		else if (u.position.x < position.x) dir = Direction.Left;
		else if (u.position.y > position.y) dir = Direction.Up;
		return dir;
	}

	void throwAnimation() {
		attackEnemy.setRotationToCharacter(this);
		attackEnemy.getThrown(directionOf(attackEnemy), characterSheet.combatScores.getInitiative(), this);
		mapGenerator.resetAttack(this);
		if (this == mapGenerator.getCurrentUnit())
			mapGenerator.resetRanges();
	}

	void setXYFromDirection(Direction dir, ref int x, ref int y) {
		switch (dir) {
		case Direction.Left:
			x--;
			break;
		case Direction.Right:
			x++;
			break;
		case Direction.Down:
			y++;
			break;
		case Direction.Up:
			y--;
			break;
		default:
			break;
		}
	}

	bool gettingThrown = false;
	Vector3 gettingThrownPosition;
	void getThrown(Direction dir, int distance, Unit thrownBy) {
		Debug.Log("getThrown(" + dir + ", " + distance + ")");
		int x = (int)position.x;
		int y = (int)-position.y;
		Tile t = mapGenerator.tiles[x, y];
		int dis = 0;
		for (;distance>0;distance--) {
			Tile nextT = t.getTile(dir);
			if (!nextT.hasCharacter() && t.passabilityInDirection(dir)==1) {//t.canPass(dir, this, dir)) {
				t = nextT;
				setXYFromDirection(dir, ref x, ref y);
				dis++;
			}
			else break;
		}
		bool becameProne = false;
		Unit alsoProne = null;
		if (t.passabilityInDirection(dir)!=1 || (t.getTile(dir)==null || t.getTile(dir).hasCharacter())) {
		//	affliction = Affliction.Prone;
			becomeProne();
			becameProne = true;
			if (t.getTile(dir) != null) {
				Unit u = t.getTile(dir).getCharacter();
				if (u) {
					if (u.characterSheet.rollForSkill(Skill.Athletics) < 15) {
//					u.affliction = Affliction.Prone;
						u.becomeProne();
						alsoProne = u;
					}
				}
			}
		}
		if (team == 0) {
			gui.log.addMessage(getName() + " was thrown " + (dis*5) + " feet by " + thrownBy.getName() + (becameProne ? " and was knocked prone" + (alsoProne!=null?" along with " + alsoProne.getName():"") : "") + "!", Color.red);
		}
		else {
			gui.log.addMessage(thrownBy.getName() + " threw " + getName() + " " + (dis*5) + " feet" + (becameProne ? " and knocked " + (characterSheet.characterSheet.personalInformation.getCharacterSex()==CharacterSex.Female ? "her":"him") + " prone" + (alsoProne!=null?" along with " + alsoProne.getName():"") : "") + "!", Log.greenColor);
		}
		gettingThrown = true;
//		gettingThrownPosition = new Vector3(x, -y, position.z);
		currentPath.Add(new Vector2(x, y));
	//	setPosition(new Vector3(x, -y, position.z));
	}

	void becomeProne() {
		if (!isProne()) {
			afflictions.Add(Affliction.Prone);
		}
	}

	void doGetThrown() {
		if (gettingThrown) {
			if (currentPath.Count>=2) {
				float speed = 5.0f;
				float time = Time.deltaTime;
				float moveDist = time * speed;
				moveBy(moveDist, false);
			}
			else {
				gettingThrown = false;
			}
		}
	}

	public void useMovementIfStarted() {
		if (moveDistLeft < maxMoveDist) {
			usedMovement = true;
			currentMoveDist = 0;
			moveDistLeft = 0;
		}
	}

	public bool intimidateAnimating = false;
	void doIntimidate() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (intimidating && !moving && !rotating) {
			intimidateAnimation();
			intimidateAnimating = true;
			intimidating = false;
			usedStandard = true;
			useMovementIfStarted();
		}
	}

	void intimidateAnimation() {
		dealIntimidationDamage();
		mapGenerator.resetAttack(this);
		if (this == mapGenerator.getCurrentUnit())
			mapGenerator.resetRanges();
	}

	void dealIntimidationDamage() {
		if (attackEnemy != null) {
			int sturdy = characterSheet.rollForSkill(Skill.Melee, 20);
			int wellV = attackEnemy.characterSheet.rollForSkill(Skill.Political, 10);
			bool didHit = sturdy >= wellV;
			int wapoon = Mathf.Max(1, characterSheet.combatScores.getInitiative());
			DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
			damageDisplay.begin(wapoon, didHit, false, attackEnemy, Color.green);
			if (didHit) {
				attackEnemy.damageComposure(wapoon, this);
				attackEnemy.setRotationToCharacter(this);
			}
		}
	}

	
	public void damageComposure(int damage, Unit u) {
		if (damage > 0) {
			crushingHitSFX();
			characterSheet.combatScores.loseComposure(damage);
		}
	}

	void doAttack() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (attacking && !moving && !rotating) {
			attackAnimation();
			attackAnimating = true;
			attacking = false;
		}
	}

	
	
	public void crushingSwingSFX() {
		if (mapGenerator && mapGenerator.audioBank) {
			mapGenerator.audioBank.playClipAtPoint(ClipName.CrushingSwing, transform.position);
		}
	}

	void moveAnimation(bool moving) {
		anim.SetBool("Move",moving);
		movementAnimationAllSprites(moving);
	}

	void deathAnimation() {
		anim.SetTrigger("Death");
		deathAnimationAllSprites();
	}
	
	void attackAnimation() {
		anim.SetTrigger("Attack");
		attackAnimationAllSprites();
		//	attackEnemy = null;
	}

	void attackAnimationAllSprites() {
		setAllSpritesTrigger("Attack");
	}

	void movementAnimationAllSprites(bool move) {
		setAllSpritesBool("Move",move);
	}

	void deathAnimationAllSprites() {
		setAllSpritesTrigger("Death");
	}

	void setAllSpritesBool(string boolName, bool b) {
		Debug.Log(boolName + "  " + b);
		List<SpriteOrder> sprites = getSprites();
		foreach (SpriteOrder sprite in sprites) {
			sprite.sprite.GetComponent<Animator>().SetBool(boolName, b);
		}
	}

	void setAllSpritesTrigger(string trigger) {
		List<SpriteOrder> sprites = getSprites();
		foreach (SpriteOrder sprite in sprites) {
			sprite.sprite.GetComponent<Animator>().SetTrigger(trigger);
		}
	}

	public virtual int getAC() {
		return characterSheet.characterSheet.characterLoadout.getAC();
	}

	public virtual string getName() {
		return characterSheet.personalInfo.getCharacterName().fullName();
	}



	public virtual int getCritChance() {
		return characterSheet.characterSheet.characterLoadout.rightHand.criticalChance;
	}

	public virtual int getMeleeScore() {
		return characterSheet.characterSheet.skillScores.getScore(Skill.Melee);
	}

	public virtual int rollDamage(bool crit) {
		return characterSheet.rollDamage(crit);
	}

	public virtual Weapon getWeapon() {
		return characterSheet.characterSheet.characterLoadout.rightHand;
	}

	public virtual int getUncannyKnowledgeBonus() {
		if (characterSheet.characterProgress.hasFeature(ClassFeature.Uncanny_Knowledge)) return 1;
		return 0;
	}

	public virtual string getGenderString() {
		return (characterSheet.characterSheet.personalInformation.getCharacterSex()==CharacterSex.Female ? " her" : " his");
	}

	public virtual bool hasWeaponFocus() {
		return getWeapon().damageType == characterSheet.characterSheet.characterProgress.getWeaponFocus();
	}

	public void showDamage(int wapoon, bool didHit, bool crit) {
		DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
		damageDisplay.begin(wapoon, didHit, crit, this);

	}

	public void dealDamage() {
		//	int hit = characterSheet.rollHit();//Random.Range(1,21);
        Hit hit = Combat.rollHit(this);
		int enemyAC = attackEnemy.getAC();
		Hit critHit = Combat.rollHit(this);
		bool crit = hit.crit && critHit.hit  >= enemyAC;
		int wapoon = rollDamage(crit);//.characterLoadout.rightHand.rollDamage();
		bool didHit = hit.hit >= enemyAC || hit.crit;
		attackEnemy.showDamage(wapoon, didHit, crit);
		gui.log.addMessage(getName() + (didHit ? (crit ? " critted " : " hit ") : " missed ") + attackEnemy.getName() + (didHit ? " with " + (getWeapon() == null ?  getGenderString() + " fist " : getWeapon().itemName + " ") + "for " + wapoon + " damage!" : "!"), (team==0 ? Log.greenColor : Color.red));
		if (didHit)
			attackEnemy.damage(wapoon, this);
		if (!attackEnemy.moving) {
			attackEnemy.attackedByCharacter = this;
			attackEnemy.setRotationToAttackedByCharacter();
			attackEnemy.beingAttacked = true;
		}
		else {
			attackEnemy.shouldMove--;
			if (attackEnemy.shouldMove<0) attackEnemy.shouldMove = 0;
		}
		Debug.Log((hit.hit > 4 ? "wapoon: " + wapoon : "miss!") + " hit: " + hit.hit + "  " + hit.crit + "  critHit: " + critHit.hit + "   enemyAC: " + enemyAC);
		//		damageDisplay.begin(
	}
	
	public int damageNumber = 0;
	
	public void addDamageDisplay() {
		damageNumber++;
	}
	
	public void removeDamageDisplay() {
		damageNumber--;
	}

	public virtual string deathString() {
		return "killed";
	}


	public void killedEnemy(Unit enemy, bool decisiveStrike) {
		Debug.Log("Killed Enemy!!");
		if (this.team==0) gui.log.addMessage(getName() + " " + enemy.deathString() + " " + enemy.getName() + "!", Log.greenColor);
		else gui.log.addMessage(enemy.getName() + " was " + enemy.deathString() +" by " + getName() + "!",Color.red);
		if (decisiveStrike) handleClassFeature(ClassFeature.Decisive_Strike);
		setRotationToMostInterestingTile();
	}

	void attackFinished() {
		if (attackEnemy) {
			attackEnemy.wasBeingAttacked = attackEnemy.beingAttacked;
			attackEnemy.beingAttacked = false;
			attackEnemy.attackedByCharacter = null;
		}
		mapGenerator.resetAttack(this);
		if (this == mapGenerator.getCurrentUnit())
			mapGenerator.resetRanges();
		attackAnimating = false;
		usedStandard = true;
		useMovementIfStarted();

	}
	
	public void crushingHitSFX() {
		mapGenerator.audioBank.playClipAtPoint(ClipName.CrushingHit, transform.position);
	}

	public virtual bool deadOrDying() {
		return characterSheet.combatScores.isDead() || characterSheet.combatScores.isDying();
	}

	public virtual bool unconscious() {
		return characterSheet.combatScores.isUnconscious();
	}

	public virtual bool deadOrDyingOrUnconscious() {
		return deadOrDying() || unconscious();
	}

	public virtual void loseHealth(int amount) {
		characterSheet.combatScores.loseHealth(amount);
	}

	public virtual bool givesDecisiveStrike() {
		return true;
	}

	public void damage(int damage, Unit u) {
		//	Debug.Log("Damage");
		if (damage > 0) {
			crushingHitSFX();
//			hitPoints -= damage;
//			if (hitPoints <= 0) died = true;
			bool d = deadOrDyingOrUnconscious();
			loseHealth(damage);
			if (!d && deadOrDyingOrUnconscious() && u) {
				u.killedEnemy(this, givesDecisiveStrike());
			}
			if (mapGenerator.getCurrentUnit()==this && deadOrDyingOrUnconscious()) mapGenerator.nextPlayer();
		}
		//	Debug.Log("EndDamage");
	}

	public bool didDeath = false;
	public bool didActualDeath = false;
	public virtual bool isDead() {
		return characterSheet.combatScores.isDead();
	}

	public virtual void doDeath() {
		//	Debug.Log("Do Death");
		//	mapGenerator.
	//	if (died) dieTime += Time.deltaTime;
		//	if (dieTime >= 1) Destroy(gameObject);
		//	if (dieTime >= 0.5f) {
		if (deadOrDyingOrUnconscious() && !didDeath) {
			if (!mapGenerator.selectedUnit || !mapGenerator.selectedUnit.attacking || !mapGenerator.selectedUnit.moving) {
				didDeath = true;
				if (mapGenerator.selectedUnit) {
					Unit p = mapGenerator.selectedUnit;
					if (p.attackEnemy==this) p.attackEnemy = null;
				}
				deselect();
			//	if (isDead())
			//		mapGenerator.removeCharacter(this);
//				Tile t = mapGenerator.tiles[(int)position.x, (int)-position.y];
//				if (t.getCharacter()==this)
//					t.removeCharacter();
//				Destroy(gameObject);
//				mapGenerator.resetCharacterRange();
				deathAnimation();
				mapGenerator.setGameState();
			}
		}
		if (isDead() && !didActualDeath) {
			if (mapGenerator.priorityOrder.Contains(this))
				mapGenerator.removeCharacter(this);
			didActualDeath = true;
		}
		//	Debug.Log("End Death");
	}

	void handleClassFeature(ClassFeature feature) {
		if (characterSheet==null) return;
		if (!characterSheet.characterProgress.hasFeature(feature)) return;
		Debug.Log("Has!!");
		switch(feature) {
		case ClassFeature.Decisive_Strike:
			handleDecisiveStrike();
			break;
		default:
			break;
		}
	}

	public bool usedDecisiveStrike = false;
	void handleDecisiveStrike() {
		Debug.Log("Handle Decisive Strike!");
		if (usedDecisiveStrike) return;
		usedDecisiveStrike = true;
		usedStandard = false;
	}

	const float tabSpeed = 4.0f;
	public static void doTabs(GameGUI gui) {
		if (gui.openTab==Tab.C) {
			cPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else {
			cPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		if (gui.openTab == Tab.K) {
			kPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else {
			kPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		cPerc = Mathf.Clamp01(cPerc);
		kPerc = Mathf.Clamp01(kPerc);
	}




	public void deleteCharacter() {
		Saves.deleteCharacter(characterId);
	}

	public void saveCharacter() {
		Saves.saveCharacter(characterId, getCharacterString());
	}
	
	const string delimiter = ";";
	public string getCharacterString() {
		string characterStr = "";
		//********PERSONAL INFORMATION********\\
		//Adding player first name.
		characterStr += characterSheet.personalInfo.getCharacterName().firstName + delimiter;
		characterStr += characterSheet.personalInfo.getCharacterName().lastName + delimiter;
		CharacterSex sex = characterSheet.personalInfo.getCharacterSex();
		characterStr += (sex==CharacterSex.Male ? 0 : (sex==CharacterSex.Female ? 1 : 2)) + delimiter;
		RaceName race = characterSheet.personalInfo.getCharacterRace().raceName;
		characterStr += (race == RaceName.Berrind ? 0 : (race == RaceName.Ashpian ? 1 : 2)) + delimiter;
		CharacterBackground background = characterSheet.personalInfo.getCharacterBackground();
		characterStr += (background == CharacterBackground.FallenNoble || background == CharacterBackground.Commoner || background == CharacterBackground.Servant ? 0 : 1) + delimiter;
		characterStr += characterSheet.personalInfo.getCharacterAge().age + delimiter;
		characterStr += (characterSheet.personalInfo.getCharacterHeight().feet * 12 + characterSheet.personalInfo.getCharacterHeight().inches) + delimiter;
		characterStr += characterSheet.personalInfo.getCharacterWeight().weight + delimiter;
		ClassName clas = characterSheet.characterProgress.getCharacterClass().getClassName();
		characterStr += (clas == ClassName.ExSoldier ? 0 : (clas == ClassName.Engineer ? 1 : (clas == ClassName.Investigator ? 2 : (clas == ClassName.Researcher ? 3 : 4)))) + delimiter;
		characterStr += characterSheet.abilityScores.getSturdy() + delimiter;
		characterStr += characterSheet.abilityScores.getPerception() + delimiter;
		characterStr += characterSheet.abilityScores.getTechnique() + delimiter;
		characterStr += characterSheet.abilityScores.getWellVersed() + delimiter;
		foreach (int score in characterSheet.skillScores.scores) {
			characterStr += score + delimiter;
		}
		CharacterColors colors = characterSheet.characterSheet.characterColors;
		characterStr += colorString(colors.characterColor);
		characterStr += colorString(colors.headColor);
		characterStr += colorString(colors.primaryColor);
		characterStr += colorString(colors.secondaryColor);
		characterStr += characterSheet.characterSheet.personalInformation.getCharacterHairStyle().hairStyle + delimiter;
		characterStr += characterSheet.characterProgress.getCharacterLevel() + delimiter;
		characterStr += characterSheet.characterProgress.getCharacterExperience() + delimiter;
		//*********Hair*********\\
		characterStr += characterSheet.characterSheet.inventory.purse.money + delimiter;
		characterStr += characterSheet.characterSheet.combatScores.getCurrentHealth() + delimiter;
		characterStr += characterSheet.characterSheet.combatScores.getCurrentComposure() + delimiter;
		characterStr += "0;";
		return characterStr;
	}
	
	static string colorString(Color c) {
		return ((int)(c.r*255)) + delimiter + ((int)(c.g*255)) + delimiter + ((int)(c.b*255)) + delimiter;
	}
}
