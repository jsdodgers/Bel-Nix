using UnityEngine;
using System.Collections;
using CharacterInfo;

public enum MovementType {Move, BackStep, Recover, Cancel, None}
public enum StandardType {Attack, Reload, Inventory, Cancel, None}

public class Unit : MonoBehaviour {
	public Character characterSheet;
	int initiative;
	public string characterName;
	public int team = 0;

	
	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;
	static Vector2 turnOrderScrollPos = new Vector2(0.0f, 0.0f);
	Vector2 classFeaturesScrollPos = new Vector2(0.0f, 0.0f);

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
	public GameGUI gui;

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
		usedMinor1 = false;
		usedMinor2 = false;
		currentMoveDist = 0;
		moveDistLeft = maxMoveDist;
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

	bool canPass(Direction dir, Vector2 pos, Direction dirFrom) {
		if (!mapGenerator.tiles[(int)pos.x,(int)pos.y].canTurn)
			Debug.Log(pos + ": " + dir + "   --  " + dirFrom);
		return mapGenerator.canPass(dir, (int)pos.x, (int)pos.y, this, dirFrom);//dirFrom);
	}
	
	ArrayList calculatePath(ArrayList currentPathFake, Vector2 posFrom,Vector2 posTo, ArrayList curr, int maxDist, bool first, int num = 0, Direction dirFrom = Direction.None) {
		//	Debug.Log(posFrom + "  " + posTo + "  " + maxDist);
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
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x - 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Left));
		if (canPass(Direction.Right, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x + 1, posFrom.y), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Right));
		if (canPass(Direction.Up, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y - 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Up));
		if (canPass(Direction.Down, posFrom, dirFrom))
			a.Add(calculatePath(currentPathFake, new Vector2(posFrom.x, posFrom.y + 1), posTo, (ArrayList)curr.Clone(), maxDist-1, false, num+1, Direction.Down));
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
			Direction dir = Direction.None;
			if (currList.Count > 1) {
				Vector2 curr = (Vector2)currList[currList.Count-1];
				Vector2 last = (Vector2)currList[currList.Count-2];
				if (curr.x < last.x) dir = Direction.Left;
				else if (curr.x > last.x) dir = Direction.Right;
				else if (curr.y < last.y) dir = Direction.Up;
				else if (curr.y > last.y) dir = Direction.Down;
			}
			ArrayList added = calculatePath(currList, last1, posTo, new ArrayList(), maxDist, true, 0, dir);
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
	//	int hit = characterSheet.rollHit();//Random.Range(1,21);
		Hit hit = characterSheet.rollHit();
		int enemyAC = attackEnemy.GetComponent<CharacterLoadout>().getAC();
		Hit critHit = characterSheet.rollHit();
		bool crit = hit.crit && critHit.hit  >= enemyAC;
		int wapoon = characterSheet.rollDamage(crit);//.characterLoadout.rightHand.rollDamage();
		bool didHit = hit.hit >= enemyAC || hit.crit;
		DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
		damageDisplay.begin(wapoon, didHit, crit, attackEnemy);
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
			paperDollTexturesHead = new Texture[]{Resources.Load<Texture>("Characters/Jackie/JackiePaperdollHead")};
		}
		return paperDollTexturesHead;
	}

	private Texture[] paperDollTexturesFull;
	public Texture[] getPaperDollTexturesFull() {
		if (paperDollTexturesFull == null) {
			paperDollTexturesFull = new Texture[]{Resources.Load<Texture>("Characters/Jackie/JackiePaperdoll")};
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
	static Texture2D paperDollHealthBannerTexture = null;
	static int lastWidth = 0;

	Texture2D getPaperDollHealthBannerTexture(int width, int height) {
		if (paperDollHealthBannerTexture == null || width != lastWidth) {
			paperDollHealthBannerTexture = makeTexBanner(width, height, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			lastWidth = width;
		}
		return paperDollHealthBannerTexture;
	}

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

	
	Texture2D makeTexBorder(int width, int height, Color col )
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
			composureTextStyle.normal.textColor = new Color(.316f, 0.0f, .316f);
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
	
	const float paperDollHeadSize = 100.0f;
	const float bannerWidth = 300.0f;
	const float paperDollFullWidth = 201.0f;
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
	public bool guiContainsMouse(Vector2 mousePos) {
		if (gui.openTab == Tab.None) {
			if (mousePos.y <= paperDollHeadSize && paperDollHeadSize + bannerWidth - mousePos.x >= (mousePos.y)/2) return true;
			if (mousePos.y <= paperDollHeadSize + tabButtonsWidth && mousePos.x <= (tabButtonsWidth - 1.0f) * 5 + 1) return true;
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

	public void drawGUI() {
	/*	float healthX = -1.0f;
		float healthY = 0.0f;
		float healthCenter = -1.0f;
		float healthCompY = 0.0f;
		float compY = 25.0f;*/
		float tabButtonsY = 0.0f;
	//	int healthFont = 0;
		string healthText = "Health: " + characterSheet.combatScores.getCurrentHealth() + "/" + characterSheet.combatScores.getMaxHealth();
		GUIContent healthContent = new GUIContent(healthText);
		string composureText = "Composure: " + characterSheet.combatScores.getCurrentComposure() + "/" + characterSheet.combatScores.getMaxComposure();
		GUIContent composureContent = new GUIContent(composureText);
		if (gui.openTab == Tab.None) {
			Texture[] textures = getPaperDollTexturesHead();
//			GUIStyle headStyle = getPaperDollHeadStyle();
			foreach (Texture2D texture in textures) {
//			headStyle.normal.background = texture;
				GUI.DrawTexture(new Rect(0.0f, 0.0f, paperDollHeadSize, paperDollHeadSize), texture);
	//			GUI.Label(new Rect(0.0f, 0.0f, paperDollHeadSize, paperDollHeadSize), "", headStyle);
			}
			GUI.DrawTexture(new Rect(paperDollHeadSize, 0.0f, bannerWidth, paperDollHeadSize + 1), getPaperDollHealthBannerTexture((int)bannerWidth, (int)paperDollHeadSize + 1));
			float healthX = 35.0f + paperDollHeadSize;
			float healthCompY = paperDollHeadSize/2.0f;
			tabButtonsY = paperDollHeadSize;
			GUIStyle healthStyle = getHealthTextStyle(25);
			GUIStyle compStyle = getComposureTextStyle(25);
			Vector2 healthSize = healthStyle.CalcSize(healthContent);
			Vector2 composureSize = compStyle.CalcSize(composureContent);
			//	float totalHeight = composureSize.y + healthSize.y;
			float healthY = healthCompY - healthSize.y;
			float compY = healthCompY;
			GUI.Label(new Rect(healthX, healthY, healthSize.x, healthSize.y), healthContent, healthStyle);
			GUI.Label(new Rect(healthX, compY, composureSize.x, composureSize.y), composureContent, compStyle);
		}
		else {
			tabButtonsY = paperDollFullHeight - 1;
			GUI.DrawTexture(new Rect(0.0f, 0.0f, paperDollFullWidth, paperDollFullHeight), getPaperDollFullBackgroundTexture((int)paperDollFullWidth, (int)paperDollFullHeight));
			float y = 0.0f;
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
			float healthCenter = paperDollFullWidth/2.0f;
//			healthCompY = y + 20.0f;
			float healthY = y;
			GUIStyle healthStyle = getHealthTextStyle(12);
			GUIStyle compStyle = getComposureTextStyle(12);
			Vector2 healthSize = healthStyle.CalcSize(healthContent);
			Vector2 composureSize = compStyle.CalcSize(composureContent);
		//	float totalHeight = composureSize.y + healthSize.y;
			//healthY = healthCompY - healthSize.y;
			float mid = 8.0f;
			float compY = healthY + healthSize.y - mid;
			GUI.Label(new Rect(healthCenter - healthSize.x/2.0f, healthY, healthSize.x, healthSize.y), healthContent, healthStyle);
			GUI.Label(new Rect(healthCenter - composureSize.x/2.0f, compY, composureSize.x, composureSize.y), composureContent, compStyle);
			y = compY + composureSize.y - mid;
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
			}

			GUI.EndScrollView();
		}
		else if (gui.openTab == Tab.C) {
			GUI.DrawTexture(fullCRect(), getCharacterStatsBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent characterStats = new GUIContent("Character Stats");
			Vector2 characterStatsSize = titleStyle.CalcSize(characterStats);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth - 1.0f - characterStatsSize.x)/2.0f, 0.0f, characterStatsSize.x, characterStatsSize.y), characterStats, titleStyle);
		//	float y = turnOrderSize.y;
			float statX = paperDollFullWidth + 10.0f;
			float statWidth = 40.0f;
			float baseX = statX + statWidth;
			float baseWidth = 40.0f;
			float modX = baseX + baseWidth;
			float modWidth = baseWidth;
			float y = characterStatsSize.y + 5.0f;
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
			GUIContent armorClass = new GUIContent("Armor Class: " + characterSheet.characterLoadout.getAC());
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
		else if (gui.openTab == Tab.K) {
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
			float y = skillSize.y + 5.0f;
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
			GUIContent features = new GUIContent("Class Features");
			Vector2 featuresSize = titleStyle.CalcSize(features);
			GUI.Label(new Rect(paperDollFullWidth + (characterStatsWidth - 1.0f - skillSize.x)/2.0f, y, featuresSize.x, featuresSize.y), features, titleStyle);
			y += featuresSize.y;
			float featureX = paperDollFullWidth + 10.0f;
			GUIStyle featuresStyle = st;
			GUIContent c = new GUIContent("Feature");
			float featuresHeight = featuresStyle.CalcSize(c).y;
			ClassFeature[] classFeatures = characterSheet.characterSheet.characterProgress.getCharacterClass().getClassFeatures();
			float scrollHeight = featuresHeight * classFeatures.Length;
			float remainingHeight = skillsHeight - y;
			classFeaturesScrollPos = GUI.BeginScrollView(new Rect(paperDollFullWidth - 1.0f, y, skillsWidth, remainingHeight), classFeaturesScrollPos, new Rect(paperDollFullWidth - 1.0f, y, skillsWidth - (scrollHeight > remainingHeight ? 16.0f : 0.0f), scrollHeight));
			foreach (ClassFeature classFeature in classFeatures) {
				GUIContent feat = new GUIContent(classFeature.ToString());
				Vector2 featSize = featuresStyle.CalcSize(feat);
				GUI.Label(new Rect(featureX, y, featSize.x, featSize.y), feat, featuresStyle);
				y += featuresHeight;
			}
			GUI.EndScrollView();
		}
		else if (gui.openTab == Tab.I) {
			GUI.DrawTexture(fullIRect(), getInventoryBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent armour = new GUIContent("Armour");
			Vector2 armourSize = titleStyle.CalcSize(armour);
			GUI.Label(new Rect(paperDollFullWidth - 1.0f + inventoryWidth/3.0f - armourSize.x/2.0f, 0.0f, armourSize.x, armourSize.y), armour, titleStyle);
			GUIContent inventory = new GUIContent("Inventroy");
			Vector2 inventorySize = titleStyle.CalcSize(inventory);
			GUI.Label(new Rect(paperDollFullWidth - 1.0f + inventoryWidth*2.0f/3.0f - inventorySize.x/2.0f, 0.0f, inventorySize.x, inventorySize.y), inventory, titleStyle);

		}
		if (GUI.Button(new Rect((tabButtonsWidth-1)*0, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "M",(gui.openTab == Tab.M ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			if (gui.openTab == Tab.M) gui.openTab = Tab.None;
			else gui.openTab = Tab.M;
		}
		if (GUI.Button(new Rect((tabButtonsWidth-1)*1, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "C",(gui.openTab == Tab.C ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			if (gui.openTab == Tab.C) gui.openTab = Tab.None;
			else gui.openTab = Tab.C;
		}
		if (GUI.Button(new Rect((tabButtonsWidth-1)*2, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "K",(gui.openTab == Tab.K ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			if (gui.openTab == Tab.K) gui.openTab = Tab.None;
			else gui.openTab = Tab.K;
		}
		if (GUI.Button(new Rect((tabButtonsWidth-1)*3, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "I",(gui.openTab == Tab.I ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			if (gui.openTab == Tab.I) gui.openTab = Tab.None;
			else gui.openTab = Tab.I;
		}
		if (GUI.Button(new Rect((tabButtonsWidth-1)*4, tabButtonsY, tabButtonsWidth, tabButtonsWidth), "T",(gui.openTab == Tab.T ? getSelectedButtonStyle(tabButtonsWidth) : getNonSelectedButtonStyle(tabButtonsWidth)))) {
			if (gui.openTab == Tab.T) gui.openTab = Tab.None;
			else gui.openTab = Tab.T;
		}
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

	public void startAttacking() {
		if (attackEnemy!=null && !moving) {
			attacking = true;
		}
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

	SpriteRenderer circleSprite;
	public SpriteRenderer getCircleSprite() {
		if (circleSprite == null) {
			circleSprite = transform.FindChild("Circle").GetComponent<SpriteRenderer>();
		}
		return circleSprite;
	}

	bool characterSheetLoaded = false;
	public void loadCharacterSheet() {
		if (characterSheetLoaded) return;
		characterSheetLoaded = true;
		characterSheet.loadData();
		characterSheet.unit = this;
	}

	public virtual void initializeVariables() {
//		characterSheet = gameObject.GetComponent<Character>();
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
		setCircleScale();
	}

	void setLayer() {
		renderer.sortingOrder = (mapGenerator.isInCharacterPlacement() ? 90000 : (moving || attacking || attackAnimating ? 11 : 10));
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
//			hitPoints -= damage;
//			if (hitPoints <= 0) died = true;
			characterSheet.combatScores.loseHealth(damage);
		}
		//	Debug.Log("EndDamage");
	}
	
	void doDeath() {
		//	Debug.Log("Do Death");
		//	mapGenerator.
	//	if (died) dieTime += Time.deltaTime;
		//	if (dieTime >= 1) Destroy(gameObject);
		//	if (dieTime >= 0.5f) {
		if (characterSheet.combatScores.isDead()) {
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
