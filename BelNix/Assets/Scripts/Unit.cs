using UnityEngine;
using System.Collections;
using CharacterInfo;


public enum MovementType {Move, BackStep, Recover, Cancel, None}
public enum StandardType {Attack, Reload, Inventory, Cancel, None}

public class Unit : MonoBehaviour {
	Character characterSheet;
	int priority;
	public string characterName;
	public int team = 0;

	
	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;

	
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
	public bool usedMinor1;
	public bool usedMinor2;

	public bool isCurrent;
	public bool isSelected;
	public bool isTarget;
	SpriteRenderer targetSprite;
	public bool doAttOpp = true;

	public GameObject damagePrefab;

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

	public MovementType[] getMovementTypes() {
		return new MovementType[] {MovementType.Move, MovementType.BackStep, MovementType.Cancel};
	}

	public StandardType[] getStandardTypes() {
		return new StandardType[] {StandardType.Attack, StandardType.Inventory, StandardType.Cancel};
	}

	public int numberMovements() {
		return getMovementTypes().Length;
	}

	public int numberStandards() {
		return getStandardTypes().Length;
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
		Debug.Log("Add Trail: " + characterName);
		if (trail) {
			Debug.Log("Has Trail");
			setTrailRendererPosition();
			TrailRenderer tr = trail.GetComponent<TrailRenderer>();
			tr.enabled = true;
			StartCoroutine(resetTrailDist());
//			tr.time = 2.2f;
		}
	}

	
	IEnumerator resetTrailDist() {
		yield return new WaitForSeconds(.1f);
		trail.GetComponent<TrailRenderer>().time=2.2f;
		
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


	public virtual void setPosition(Vector3 position) {

	}

	public void resetVars() {
		usedMovement = false;
		usedStandard = false;
		usedMinor1 = false;
		usedMinor2 = false;
		currentMoveDist = 0;
		moveDistLeft = maxMoveDist;
	}

	public void setPriority() {
		priority = Random.Range(1,21);
	}

	public int getPriority() {
		return priority;
	}

	public bool isEnemyOf(Unit cs) {
		return team != cs.team;
	}

	public bool isAllyOf(Unit cs) {
		return team != cs.team;
	}



	
	
	public void setNewTilePosition(Vector3 pos) {
		if (mapGenerator && position.x > 0 && -position.y > 0) {
			if (mapGenerator.tiles[(int)position.x,(int)-position.y].getCharacter()==this) {
				mapGenerator.tiles[(int)position.x,(int)-position.y].removeCharacter();
			}
		}
		if (mapGenerator && !mapGenerator.tiles[(int)pos.x,(int)-pos.y].hasCharacter() && pos.x > 0 && -pos.y > 0) {
			mapGenerator.tiles[(int)pos.x,(int)-pos.y].setCharacter(this);
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

	bool canPass(Direction dir, Vector2 pos) {
		return mapGenerator.canPass(dir, (int)pos.x, (int)pos.y, this);
	}
	
	ArrayList calculatePath(ArrayList currentPathFake, Vector2 posFrom,Vector2 posTo, ArrayList curr, int maxDist, bool first, int num = 0) {
		//	Debug.Log(posFrom + "  " + posTo + "  " + maxDist);
		
		if (!first) {
			//	if (!mapGenerator.canStandOn(posFrom.x, posFrom.y)) return curr;
			if ((exists(currentPathFake, posFrom) || exists(curr, posFrom))) return curr;
			curr.Add(posFrom);
		}
		//	if (maxDist == 0) Debug.Log("Last: " + curr.Count);
		if (maxDist <= 0) return curr;
		ArrayList a = new ArrayList();
		if (canPass(Direction.Left, posFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x - 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		if (canPass(Direction.Right, posFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x + 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		if (canPass(Direction.Up, posFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y - 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
		if (canPass(Direction.Down, posFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y + 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1));
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

	public void crushingSwingSFX() {
		if (mapGenerator && mapGenerator.audioBank) {
			mapGenerator.audioBank.playClipAtPoint(ClipName.CrushingSwing, transform.position);
		}
	}

	
	void attackAnimation() {
	//	crushingSwingSFX();
		Debug.Log("Attack!");
		anim.SetTrigger("Attack");
		//	attackEnemy = null;
	}
	
	void dealDamage() {
		int hit = Random.Range(1,21);
		int wapoon = characterSheet.mainHand.rollDamage();
		bool didHit = hit >= gameObject.GetComponent<CharacterLoadout>().getAC();
		DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
		damageDisplay.begin(wapoon, didHit, attackEnemy.transform.position);
		if (didHit)
			attackEnemy.damage(wapoon);
		if (!attackEnemy.moving) {
			attackEnemy.attackedByCharacter = this;
			attackEnemy.setRotationToAttackedByCharacter();
			attackEnemy.beingAttacked = true;
		}
		else {
			attackEnemy.shouldMove--;
			if (attackEnemy.shouldMove<0) attackEnemy.shouldMove = 0;
		}
		Debug.Log((hit > 4 ? "wapoon: " + wapoon : "miss!") + " hit: " + hit);
//		damageDisplay.begin(
	}



	float redStyleWidth = 0.0f;
	float greenStyleWidth = 0.0f;
	float healthHeight = 15.0f;
	GUIStyle redStyle = null;
	GUIStyle greenStyle = null;
	Texture2D[] greenTextures;
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
	
	void OnGUI() {
//		return;
	//	if (attackEnemy && mapGenerator.getCurrentUnit() == this) {
		if (mapGenerator.selectedUnit == this) {
			float totalWidth = Screen.width * 0.7f;
			float x = (Screen.width - totalWidth)/2.0f;
			float y = 10.0f;
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

	public int attackOfOpp(Vector2 one) {
		int move = 0;
		for (int n=-1;n<=1;n++) {
			for (int m=-1;m<=1;m++) {
				if ((n==0 && m==0) || !(n==0 || m==0)) continue;
				int x = (int)one.x + n;
				int y = (int)one.y + m;
				if (x >=0 && y >= 0 && x < mapGenerator.actualWidth && y < mapGenerator.actualHeight) {
					Tile t = mapGenerator.tiles[x,y];
					if (t.hasEnemy(this)) {
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


	public void startMoving(bool backStepping) {
		if (currentPath.Count <= 1) return;
		//					p.rotating = true;
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


	void moveBy(float moveDist) {
		Vector2 one = (Vector2)currentPath[1];
		Vector2 zero = (Vector2)currentPath[0];
		zero = new Vector2(transform.localPosition.x - 0.5f, -transform.localPosition.y - 0.5f);
		float directionX = sign(one.x - zero.x);
		float directionY = -sign(one.y - zero.y);
		//				directionX = Mathf.s
		float dist = Mathf.Max(Mathf.Abs(one.x - zero.x),Mathf.Abs(one.y - zero.y));
		if (dist <= 0.5f && doAttOpp && currentPath.Count >= 3) {
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

	public SpriteRenderer getTargetSprite() {
		if (targetSprite == null) {
			targetSprite = transform.FindChild("Target").GetComponent<SpriteRenderer>();
		}
		return targetSprite;
	}

	public virtual void initializeVariables() {
		characterSheet = gameObject.GetComponent<Character>();
		hitPoints = maxHitPoints;
		moving = false;
		attacking = false;
		rotating = false;
		isCurrent = false;
		currentMoveDist = 0;
		attackRange = 1;
		viewDist = 11;
		maxMoveDist = 5;
		moveDistLeft = 5;
		anim = gameObject.GetComponent<Animator>();
		currentMaxPath = 0;
	//	Debug.Log("Children: " + transform.childCount + "  Team: " + team);
	//	trail = transform.FindChild("Trail");
		if (trail) {
			TrailRenderer tr = trail.GetComponent<TrailRenderer>();
			tr.sortingOrder = 7;
		}
		if (isCurrent) {
			addTrail();
		}
//		if (isSelected) {
//			setSelected();
//		}
//		else if (isTarget) {
//			setTarget();
//		}
//		deselect();
//		resetPath();
	}
	
	// Update is called once per frame
	void Update () {
		doMovement();
		doRotation();
		doAttack();
		doDeath();
		setLayer();
		setTargetObjectScale();
		setTrailRendererPosition();
	}

	void setLayer() {
		renderer.sortingOrder = (moving || attacking || attackAnimating ? 11 : 10);
		transform.FindChild("Circle").renderer.sortingOrder = (renderer.sortingOrder == 11 ? 5 : 4);
	}

	void doMovement() {
		if (moving && shouldMove == 0) {// && !beingAttacked) {
		//	if (wasBeingAttacked) {
		//		setRotatingPath();
		//	}
			if (currentPath.Count >= 2) {
				float speed = 2.0f;
				float time = Time.deltaTime;
				float moveDist = time * speed;
				moveBy(moveDist);
			}
			else {
				moving = false;
				addTrail();
				currentPath = new ArrayList();
				currentPath.Add(new Vector2(position.x, -position.y));
				if (currentMoveDist == 0) usedMovement = true;
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
		}
	}

	void doAttack() {
		if (attacking && !moving && !rotating) {
			attackAnimation();
			attackAnimating = true;
			attacking = false;
			usedStandard = true;
			if (moveDistLeft < maxMoveDist) {
				usedMovement = true;
				currentMoveDist = 0;
			}
		}
	}

	void attackFinished() {
		attackAnimating = false;
		if (attackEnemy) {
			attackEnemy.wasBeingAttacked = attackEnemy.beingAttacked;
			attackEnemy.beingAttacked = false;
			attackEnemy.attackedByCharacter = null;
		}
		mapGenerator.resetAttack(this);
		if (this == mapGenerator.getCurrentUnit())
			mapGenerator.resetRanges();
	}

	public void crushingHitSFX() {
		mapGenerator.audioBank.playClipAtPoint(ClipName.CrushingHit, transform.position);
	}
	
	public void damage(int damage) {
		//	Debug.Log("Damage");
		if (damage > 0) {
			crushingHitSFX();
			hitPoints -= damage;
			if (hitPoints <= 0) died = true;
		}
		//	Debug.Log("EndDamage");
	}
	
	void doDeath() {
		//	Debug.Log("Do Death");
		//	mapGenerator.
	//	if (died) dieTime += Time.deltaTime;
		//	if (dieTime >= 1) Destroy(gameObject);
		//	if (dieTime >= 0.5f) {
		if (died) {
			if (!mapGenerator.selectedUnit || !mapGenerator.selectedUnit.attacking) {
				if (mapGenerator.selectedUnit) {
				//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
					Unit p = mapGenerator.selectedUnit;
					if (p.attackEnemy==this) p.attackEnemy = null;
				}
//				mapGenerator.enemies.Remove(gameObject);
				mapGenerator.removeCharacter(this);
				mapGenerator.tiles[(int)position.x, (int)-position.y].removeCharacter();
				Destroy(gameObject);
				mapGenerator.resetCharacterRange();
			}
		}
		//	Debug.Log("End Death");
	}

}
