using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovementType {Move, BackStep, Recover, None}
public enum StandardType {Attack, OverClock, Reload, Intimidate, Inventory, Throw, Place_Turret, Lay_Trap, None}
public enum MinorType {Loot, Stealth, Mark, TemperedHands, Escape, Invoke, OneOfMany, None}
public enum Affliction {Prone = 1 << 0, Immobilized = 1 << 1, Addled = 1 << 2, Confused = 1 << 3, Poisoned = 1 << 4, None}
public enum InventorySlot {Head, Shoulder, Back, Chest, Glove, RightHand, LeftHand, Pants, Boots, Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, Frame, Applicator, Gear, TriggerEnergySource, TrapTurret, None}

public class Unit : MonoBehaviour {
	public bool needsOverlay = false;
	bool doOverlay = false;
	public List<Unit> markedUnits;
	public bool playerControlled = true;
	public AStarEnemyMap aiMap = null;
	public Character characterSheet;
	public CharacterTemplate characterTemplate;
	public bool aiActive = false;
	int initiative;
	public int stealth;
//	public string characterId;
	public int team = 0;
	public bool overClockedAttack = false;

	public int temperedHandsUsesLeft = 2;
	public bool escapeUsed = false;
	public int invokeUsesLeft = 2;

	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;
	static Vector2 turnOrderScrollPos = new Vector2(0.0f, 0.0f);

	public Unit attackedByCharacter = null;

	public bool isBackStepping = false;
	public bool doingTemperedHands = false;
	public int temperedHandsMod = 0;
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

	public bool inPrimal = false;
	public Unit primalInstigator = null;
	public int primalTurnsLeft = 0;

	public bool isMarked;
	public bool isCurrent;
	public bool isSelected;
	public bool isTarget;
	SpriteRenderer markSprite;
	SpriteRenderer targetSprite;
	SpriteRenderer hairSprite;
	public bool doAttOpp = true;

	public List<Affliction> afflictions;
	public List<TurretUnit> turrets;

	public GameObject damagePrefab;

	public void setActive(bool active) {
		aiActive = active;
		BattleGUI.writeToConsole(getName() + " has been " + (active?"":"de") + "activated!");
	}

	public int sneakAttackBonus(Unit u) {
		if (!hasCombatAdvantageOver(u) || !characterSheet.characterSheet.characterProgress.hasFeature(ClassFeature.Sneak_Attack)) return 0;
		int perception = characterSheet.abilityScores.getPerception((hasMarkOn(u) ? 2 : 0));
		if (distanceFromUnit(u) <= 1.5f) return perception;
		return perception/2;
	}

	public int getBasePerception() {
		return characterSheet.combatScores.getPerceptionMod(false);
	}

	public bool hasCombatAdvantageOver(Unit u) {
		return isFlanking(u);
	}

	public bool isFlanking(Unit u) {
		return Combat.flanking(this, u);
	}

	public void beginTurn() {
		foreach (Unit u in markedUnits) {
			u.setMarked(true);
		}
		idleAnimation(true);
		//setGUIToThis();
        BattleGUI.beginTurn(this);
	}

    /*
	public void setGUIToThis() {
		BattleGUI.setAtAGlanceText(getAtAGlanceString());
		BattleGUI.setStatsText(0,getCharacterStatsString1());
		BattleGUI.setStatsText(1,getCharacterStatsString2());
		BattleGUI.setStatsText(2,getCharacterStatsString3());
		BattleGUI.setStatsText(3,getCharacterStatsString4());
		BattleGUI.setCharacterInfoText(getCharacterInfoString());
		BattleGUI.setClassFeatures(getClassFeatureStrings());
		BattleGUI.disableAllButtons();
		BattleGUI.enableButtons(getMinorTypes(), getMovementTypes(), getStandardTypes());
	}*/

	public string[] getClassFeatureStrings() {
		
		
		ClassFeature[] classFeatures = characterSheet.characterSheet.characterProgress.getClassFeatures();
		string[] classFeatureStrings = new string[classFeatures.Length];
		int n = 0;
		foreach (ClassFeature classFeature in classFeatures) {
			classFeatureStrings[n++] = UnitGUI.getSmallCapsString(ClassFeatures.getName(classFeature), 12);
		}
		return classFeatureStrings;
	}

	public string getCharacterInfoString() {
		return UnitGUI.getSmallCapsString("Level",12) + ":" + UnitGUI.getSmallCapsString(characterSheet.characterProgress.getCharacterLevel() + "", 14) +
			"\n" + UnitGUI.getSmallCapsString("Experience", 12) + ":" + UnitGUI.getSmallCapsString(characterSheet.characterProgress.getCharacterExperience() + "/" + (characterSheet.characterProgress.getCharacterLevel()*100), 14) +
				"\n" + UnitGUI.getSmallCapsString(characterSheet.characterProgress.getCharacterClass().getClassName().ToString(), 12) +
				"\n" + UnitGUI.getSmallCapsString(characterSheet.personalInfo.getCharacterRace().getRaceString(), 12) +
				"\n" + UnitGUI.getSmallCapsString(characterSheet.personalInfo.getCharacterBackground().ToString(), 12);

	}

    public string getCharacterStatsString1()
    {
		string sizeString = "<size=10>";
		string sizeEnd = "</size>";
		string otherDivString = "<size=4>\n\n</size>";
		string divString2 = "<size=4>\n\n</size>";

		return otherDivString + "P" + sizeString + "HYSIQUE" + sizeEnd + "\n" + otherDivString +
			divString2 + otherDivString + "P" + sizeString + "ROWESS" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "M" + sizeString + "ASTERY" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "K" + sizeString + "NOWLEDGE" + sizeEnd + otherDivString;
	}

    public string getCharacterStatsString2()
    {
		string sizeString = "<size=10>";
		string sizeEnd = "</size>";
		string divString = "<size=6>\n\n</size>";

		return "S" + sizeString + "TURDY" + sizeEnd + "\n" + characterSheet.abilityScores.getSturdy() + " (<size=13>MOD:" + characterSheet.combatScores.getInitiative() + "</size>)" +
			divString + "P" + sizeString + "ERCEPTION" + sizeEnd + "\n" + characterSheet.abilityScores.getPerception(0) + " (<size=13>MOD:" + characterSheet.combatScores.getCritical(false) + "</size>)" +
				divString + "T" + sizeString + "ECHNIQUE" + sizeEnd + "\n" + characterSheet.abilityScores.getTechnique() + " (<size=13>MOD:" + characterSheet.combatScores.getHandling() + "</size>)" +
				divString + "W" + sizeString + "ELL-VERSED" + sizeEnd + "\n" + characterSheet.abilityScores.getWellVersed() + " (<size=13>MOD:" + characterSheet.combatScores.getDominion() + "</size>)";
	}

    public string getCharacterStatsString3()
    {
		string sizeString = "<size=10>";
		string sizeEnd = "</size>";
		string divString = "<size=6>\n\n</size>";

		return "A" + sizeString + "THLETICS" + sizeEnd + ":\nM" + sizeString + "ELEE" + sizeEnd + ":" + 
			divString + "R" + sizeString + "ANGED" + sizeEnd + ":\nS" + sizeString + "TEALTH" + sizeEnd + ":" +
				divString + "M" + sizeString + "ECHANICAL" + sizeEnd + ":\nM" + sizeString + "EDICINAL" + sizeEnd + ":" +
				divString + "H" + sizeString + "ISTORICAL" + sizeEnd + ":\nP" + sizeString + "OLITICAL" + sizeEnd + ":";
	}

    public string getCharacterStatsString4()
    {
		string divString = "<size=6>\n\n</size>";

		return characterSheet.skillScores.getScore(Skill.Athletics) + "\n" + characterSheet.skillScores.getScore(Skill.Melee) + divString +
			characterSheet.skillScores.getScore(Skill.Ranged) + "\n" + characterSheet.skillScores.getScore(Skill.Stealth) + divString +
				characterSheet.skillScores.getScore(Skill.Mechanical)  + "\n" + characterSheet.skillScores.getScore(Skill.Medicinal) + divString +
				characterSheet.skillScores.getScore(Skill.Historical)  + "\n" + characterSheet.skillScores.getScore(Skill.Political);

	}

	public virtual List<Turret> getTurrets() {
		return characterSheet.characterSheet.inventory.getTurrets();
	}

	public virtual List<Trap> getTraps() {
		return characterSheet.characterSheet.inventory.getTraps();
	}

	public virtual int getCurrentHealth() {
		return characterSheet.combatScores.getCurrentHealth();
	}

	public virtual int getMaxHealth() {
		return characterSheet.combatScores.getMaxHealth();
	}

	public virtual int getCurrentComposure() {
		return characterSheet.combatScores.getCurrentComposure();
	}

	public virtual int getMaxComposure() {
		return characterSheet.combatScores.getMaxComposure();
	}

    public string getAtAGlanceString()  {
//		string playerText = Unit "N<size=13>AME</size>/A<size=13>LIAS</size>:\n\"";
		string playerName = getName();
		string playerText = UnitGUI.getSmallCapsString(playerName, 13);

		playerText += "\n";
		playerText += UnitGUI.getSmallCapsString("Health", 13) + ":\n" + (team == 1 ? "?/?" : getCurrentHealth() + "/" + getMaxHealth()) + "\n";
		playerText += UnitGUI.getSmallCapsString("Composure", 13) + ":\n" + (team == 1 ? "?/?" : getCurrentComposure() + "/" + getMaxComposure());
		return playerText;
	}

	public void useTemperedHands(int mod) {
		temperedHandsUsesLeft--;
//		minorsLeft--;
		useMinor();
		temperedHandsMod += mod;
		if (temperedHandsUsesLeft == 0) BattleGUI.resetMinorButtons();
	}

	public void endTurn() {
		Unit[] copiedMarkedUnits = new Unit[markedUnits.Count];
		markedUnits.CopyTo(copiedMarkedUnits);
		foreach (Unit u in copiedMarkedUnits) {
			u.setMarked(false);
			if (!hasLineOfSightToUnit(u)) markedUnits.Remove(u);
		}
		idleAnimation(false);
		doTurrets();
		temperedHandsMod = 0;
	}

	public float getViewRadius() {
		return mapGenerator.viewRadius - (team==0 ? 0.0f : 2.0f);
	}

	public float getViewRadiusToUnit(Unit u) {
		if (team == 0 || team == u.team) return getViewRadius();
		int perception = getBasePerception() + 10;
		int st = u.stealth;
		int diff = Mathf.Max(0, st - perception);
		float viewRange = (mapGenerator.getCurrentUnit().team != team ? Mathf.Max (0.5f, getViewRadius() - diff) : Mathf.Max(1.5f, getViewRadius() - diff/2));
	//	Debug.Log(getName() + " view range to " + u.getName() + ":  " + viewRange);
		return viewRange;
	}

	public bool hasLineOfSightToTile(Tile t, Unit u = null, float distance = -1, bool manhattan = false, VisibilityMode visMode = VisibilityMode.Visibility) {
		if (distance == -1 && u != null) distance = getViewRadiusToUnit(u);
		if (distance == -1) distance = getViewRadius();
		return mapGenerator.hasLineOfSight(mapGenerator.tiles[(int)position.x,(int)-position.y], t, distance, manhattan, visMode);
	}

	public bool hasLineOfSightToUnit(Unit u, int distance = -1, bool manhattan = false, VisibilityMode visMode = VisibilityMode.Visibility) {
		return mapGenerator.hasLineOfSight(this, u, distance, manhattan, visMode);
	}

	public List<Unit> lineOfSightUnits(int distance = -1) {
		List<Unit> units = new List<Unit>();
		foreach (Unit u in mapGenerator.enemies) {
			if (hasLineOfSightToUnit(u, distance)) units.Add(u);
		}
		return units;
	}

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
		case StandardType.OverClock:
			return "Over Clock";
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
		case MovementType.BackStep:
			return "Back Step";
		default:
			return movement.ToString();
		}
	}

	public static string getNameOfMinorType(MinorType minor) {
		switch (minor) {
		case MinorType.TemperedHands:
			return "Tempered Hands";
		default:
			return minor.ToString();
		}
	}

	public void selectMinorType(MinorType t) {
		switch(t) {
		case MinorType.Escape:
			currentMoveDist = 2;
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		default:
			currentMoveDist = 0;
			mapGenerator.removePlayerPath();
			mapGenerator.resetRanges();
			break;
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
		case ClassFeature.Over_Clock:
			return StandardType.OverClock;
		case ClassFeature.Throw:
			return StandardType.Throw;
		case ClassFeature.Intimidate:
			return StandardType.Intimidate;
		default:
			return StandardType.None;
		}
	}

	public MinorType getMinorType(ClassFeature feature) {
		switch (feature) {
		case ClassFeature.Mark:
			return MinorType.Mark;
		case ClassFeature.Tempered_Hands:
			if (temperedHandsUsesLeft==0) return MinorType.None;
			return MinorType.TemperedHands;
		case ClassFeature.Escape:
			if (escapeUsed) return MinorType.None;
			return MinorType.Escape;
		case ClassFeature.Invoke:
			if (invokeUsesLeft==0) return MinorType.None;
			return MinorType.Invoke;
		default:
			return MinorType.None;
		}
	}


	public MovementType[] getMovementTypes() {
		List<MovementType> movementTypes = new List<MovementType>();
		if (isProne()) {
			movementTypes.Add(MovementType.Recover);
		}
		else {
			movementTypes.Add(MovementType.Move);
			if (moveDistLeft == maxMoveDist) movementTypes.Add(MovementType.BackStep);
		}
	//	movementTypes.Add(MovementType.Cancel);
		return movementTypes.ToArray();
	}

	public virtual bool hasTurret() {
		return characterSheet.characterSheet.inventory.hasTurret();
	}

	public virtual bool hasTrap() {
		return characterSheet.characterSheet.inventory.hasTrap();
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
		if (hasTurret()) standardTypes.Add(StandardType.Place_Turret);
		if (hasTrap()) standardTypes.Add(StandardType.Lay_Trap);
	//	standardTypes.Add(StandardType.Inventory);
	//	standardTypes.Add (StandardType.Cancel);
		return standardTypes.ToArray();
	}
	public MinorType[] getMinorTypes() {
		List<MinorType> minorTypes = new List<MinorType>();
	//	minorTypes.Add(MinorType.Loot);
		minorTypes.Add(MinorType.Stealth);
		ClassFeature[] features = characterSheet.characterProgress.getClassFeatures();
		foreach (ClassFeature feature in features) {
			MinorType mt = getMinorType(feature);
			if (mt != MinorType.None)
				minorTypes.Add(mt);
		}
		return minorTypes.ToArray();
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

	public int minReachableDistance(Unit u) {
		for (int n=1;n<10;n++) {
			if (canGetWithin(n, u, n)) return n;
		}
		return 1;
	}

	public void chooseNextBestActionType() {
		float closest = closestEnemyDist();
		if (!usedStandard && closest <= getAttackRange()) {
			GameGUI.selectStandardType(StandardType.Attack);
		}
		else if (!usedMovement && closest > 1.1f) {
			GameGUI.selectMovementType(MovementType.Move);
		}
		else if (!usedMovement && moveDistLeft == maxMoveDist && closest <= 1.1f) {
			GameGUI.selectMovementType(MovementType.BackStep);
		}
		else if (minorsLeft > 0) {
			GameGUI.selectMinorType(MinorType.Stealth);
		}
		else if (!usedMovement) {
			GameGUI.selectMovementType(MovementType.Move);
		}
		else if (!usedStandard) {
			GameGUI.selectStandardType(StandardType.Attack);
		}
		else {
			mapGenerator.nextPlayer();
		}
	}

	public int getAttackRange() {
		Weapon w = characterSheet.characterSheet.characterLoadout.rightHand;
		if (w==null) return 0;
		return w.range;
	}

	public bool canGetWithin(int dist, Unit u, int minDist = 1) {
		for (int n=-dist;n<=dist;n++) {
			for (int m=-dist;m<=dist;m++) {
				int d = Mathf.Abs(n) + Mathf.Abs(m);
				if (d > dist || d < minDist) continue;
				int x = (int)position.x + n;
				int y = (int)-position.y + m;
				if (x >= 0 && y>=0 && x < mapGenerator.actualWidth && y < mapGenerator.actualHeight) {
					Tile t = mapGenerator.tiles[x,y];
					if (t.canStand() && mapGenerator.hasLineOfSight(t, mapGenerator.tiles[(int)position.x,(int)-position.y], dist, true, (u.getWeapon().isRanged ? VisibilityMode.Ranged : VisibilityMode.Melee))) return true;
				}
			}
		}
		return false;
	}

	public void setMarked(bool marked) {
		isMarked = marked;
		getMarkSprite().enabled = marked;
		setMarkPosition();
	}

	public void setSelected() {
		return;
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

	public void setMarkPosition() {
		if (isMarked || true) {
			float factor = 1.0f/10.0f;
			float speed = 3.0f;
			float addedScale = Mathf.Sin(Time.time * speed) * factor;
			float posY = transform.position.y + 1.0f + addedScale;
		//	getMarkSprite().transform.localScale = new Vector3(scale, scale, 1.0f);
			getMark().transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			getMark().transform.position = new Vector3(transform.position.x, posY, transform.position.z);
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

	public void rollStealth() {
		int roll = Random.Range(1, 11);
		stealth = roll + characterSheet.skillScores.getScore(Skill.Stealth);
		BattleGUI.writeToConsole(getName() + " rolled a " + stealth + "(" + roll + " + " + (stealth-roll) + ") for stealth.");
		useMinor();
	}

	public void useMinor(bool changeAnyway = true, bool changeAtAll = true) {
		minorsLeft--;
		if (minorsLeft <= 0) BattleGUI.hideMinorArm();
		if (!changeAtAll && minorsLeft > 0) return;
		if (changeAnyway || minorsLeft <= 0)
			chooseNextBestActionType();
	}

	public void useStandard() {
		usedStandard = true;
		if (GameGUI.selectedStandard) {
			chooseNextBestActionType();
		}
		BattleGUI.hideStandardArm();
	}

	public void useMovement() {
		usedMovement = true;
		currentMoveDist = 0;
		moveDistLeft = 0;
		if (GameGUI.selectedMovement) {
			chooseNextBestActionType();
		}
		BattleGUI.hideMovementArm();
	}

	public int getInitiative() {
		return initiative;
	}

	public bool isEnemyOf(Unit cs) {
		return getTeam() != cs.getTeam();
	}

	public bool isAllyOf(Unit cs) {
		return getTeam() == cs.getTeam();
	}

	public virtual int getTeam() {
		return team;
	}
	public virtual string getStatusSummary() {
		return string.Format("{0}\nHP: {1}/{2}\nCP: {3}/{4}", getName(), getCurrentHealth(), getMaxHealth(), getCurrentComposure(), getMaxComposure());
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

	public Unit closestEnemy(bool attackUnconscious = false, bool needsLineOfSight = true) {
		return closestUnit(true, false, attackUnconscious, needsLineOfSight);
	}

	public Unit closestFriendly(bool attackUnconscious = false, bool needsLineOfSight = true) {
		return closestUnit(false, true, attackUnconscious, needsLineOfSight);
	}

	public Unit closestUnit(bool attackUnconscious = false, bool needsLineOfSight = true) {
		return closestUnit(true, true, attackUnconscious, needsLineOfSight);
	}

	public Unit closestUnit(bool enemies, bool friendlies, bool attackUnconscious = false, bool needsLineOfSight = true) {
		Unit closest = null;
		float dist = float.MaxValue;
		foreach (Unit u in mapGenerator.priorityOrder) {
			if (u!=this && (((enemies && u.team != team) || (friendlies && u.team == team)) && (attackUnconscious ? !u.isDead() :!u.deadOrDyingOrUnconscious()))) {
				float d = distanceFromUnit(u);
				if (d < dist && (!needsLineOfSight || mapGenerator.hasLineOfSight(this, u, -1))) {
					dist = d;
					closest = u;
				}
			}
		}
		
		for (int n=0;n<mapGenerator.turrets.transform.childCount;n++) {
			Transform tr = mapGenerator.turrets.transform.GetChild(n);
			TurretUnit tur = tr.GetComponent<TurretUnit>();
			if (tur != null && ((enemies && tur.team != team) || (friendlies && tur.team == team))) {
				float d = distanceFromUnit(tur);
				if (d < dist && (!needsLineOfSight || mapGenerator.hasLineOfSight(this, tur, -1))) {
					dist = d;
					closest = tur;
				}
			}
		}
		return closest;
	}
	

	public float closestEnemyDist(bool attackUnconscious = false) {
		return closestUnitDist(true, false, attackUnconscious);
	}
	
	public float closestFriendlyDist(bool attackUnconscious = false) {
		return closestUnitDist(false, true, attackUnconscious);
	}
	
	public float closestUnitDist(bool attackUnconscious = false) {
		return closestUnitDist(true, true, attackUnconscious);
	}

	public float closestUnitDist(bool enemies, bool friendlies, bool attackUnconscious = false) {
		float dist = 0;
		foreach (Unit u in mapGenerator.priorityOrder) {
			if (u!=this && (((enemies && u.team != team) || (friendlies && u.team == team)) && (attackUnconscious ? !u.isDead() :!u.deadOrDyingOrUnconscious()))) {
				float d = distanceFromUnit(u);
				if (d < dist || dist == 0) {
					dist = d;
				}
			}
		}
		for (int n=0;n<mapGenerator.turrets.transform.childCount;n++) {
			Transform tr = mapGenerator.turrets.transform.GetChild(n);
			TurretUnit tur = tr.GetComponent<TurretUnit>();
			if (tur != null && ((enemies && tur.team != team) || (friendlies && tur.team == team))) {
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

	public static float actionTime;
	public const float actionDelay = .25f;

	public PrimalState getPrimalState() {
		return characterSheet.characterSheet.personalInformation.getCharacterRace().getPrimalState();
	}

	public void performPrimal() {
		if (isPerformingAnAction() || mapGenerator.movingCamera) {
			actionTime = Time.time;
			return;
		}
		PrimalState ps = getPrimalState();
		if (ps == PrimalState.Threatened) {
			if (Time.time - actionTime < actionDelay) return;
		//	float closestDist = closestEnemyDist();
			Unit enemy = primalInstigator;
			float enemyDist = distanceFromUnit(enemy);
			if (enemy==null || enemy.isDead()) {
				enemy = closestUnit(true);
				enemyDist = closestUnitDist(true);
				Debug.Log(enemy.getName() + "  " + enemyDist);
			}
			if (!usedMovement) {
				if (enemyDist > 1) {
					currentMoveDist = 5;
					List<Unit> units = new List<Unit>();
					units.Add(enemy);
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
				if (enemyDist <= 1.0f) {
					usedStandard = true;
					attackEnemy = enemy;
					setRotationToAttackEnemy();
					if (enemy != null) enemy.setTarget();
					startAttacking();
					return;
				}
			}
			if (isPerformingAnAction() || mapGenerator.movingCamera) return;
			if ((usedStandard || enemyDist > 1.0f) && (usedMovement || enemyDist <= 1.0f)) {
				primalTurnsLeft--;
				if (primalTurnsLeft==0) {
					inPrimal = false;
					primalInstigator = null;
					characterSheet.characterSheet.combatScores.addComposure(1);
				}
				mapGenerator.nextPlayer();
			}
		}
		else if (ps == PrimalState.Passive) {
			primalTurnsLeft--;
			if (primalTurnsLeft==0) {
				inPrimal = false;
				primalInstigator = null;
				characterSheet.characterSheet.combatScores.addComposure(1);
			}
			mapGenerator.nextPlayer();
		}
		else if (ps == PrimalState.Reckless) {
			if (primalInstigator == null) {
				primalTurnsLeft--;
				if (primalTurnsLeft==0) {
					inPrimal = false;
					primalInstigator = null;
					characterSheet.characterSheet.combatScores.addComposure(1);
				}
				mapGenerator.nextPlayer();
			}
			if (!usedMovement) {
				if (isProne()) {
					recover();
					return;
				}
				int moveLeft = 5;
				bool first = true;
				List<Direction> availableDirections = new List<Direction>();
				Tile t = mapGenerator.tiles[(int)position.x, (int)-position.y];
				Direction lastDirection = Direction.None;
				while (moveLeft > 0 && (availableDirections.Count > 0 || first)) {
					if (first) first = false;
					else {
						Direction nextDirection = availableDirections[Random.Range(0,availableDirections.Count)];
						while (moveLeft > 0 && t.canPass(nextDirection, this, lastDirection)) {
							t = t.getTile(nextDirection);
							moveLeft--;
							currentPath.Add(new Vector2(t.x,t.y));
							lastDirection = nextDirection;
						}
					}
					if (t.x <= primalInstigator.position.x && t.canPass(Direction.Left, this, lastDirection)) {
						availableDirections.Add(Direction.Left);
					}
					if (t.x >= primalInstigator.position.x && t.canPass(Direction.Right, this, lastDirection)) {
						availableDirections.Add(Direction.Right);
					}
					if (t.y <= -primalInstigator.position.y && t.canPass(Direction.Up, this, lastDirection)) {
						availableDirections.Add(Direction.Up);
					}
					if (t.y >= -primalInstigator.position.y && t.canPass(Direction.Down, this, lastDirection)) {
						availableDirections.Add(Direction.Down);
					}
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
			}
			else {
				primalTurnsLeft--;
				if (primalTurnsLeft==0) {
					inPrimal = false;
					primalInstigator = null;
					characterSheet.characterSheet.combatScores.addComposure(1);
				}
				mapGenerator.nextPlayer();
			}
		}
	}
	
	
	public void performAI() {
		if (!aiActive) {
			mapGenerator.nextPlayer();
			return;
		}
		if (isPerformingAnAction() || mapGenerator.movingCamera) {
			actionTime = Time.time;
			return;
		}
		if (Time.time - actionTime < actionDelay) return;
		float closestDist = closestEnemyDist();
		Unit enemy = closestEnemy();
		if (!usedMovement) {
			if (isProne()) {
				recover();
				actionTime = Time.time;
				return;
			}
			if (closestDist > getAttackRange()) {
				currentMoveDist = 5;
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
				AStarNode no = node;
				string s = "Path:\n";
				while (no != null) {
					s += "\n" + no.parameters.toString();
					no = no.prev;
				}
				Debug.Log(s);
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
			if (closestDist <= getAttackRange()) {
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

	public bool guiContainsMouse(Vector2 mousePos) {
		if (UnitGUI.containsMouse(mousePos)) return true;
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
	public void drawGUI() {
		//UnitGUI.drawGUI(characterSheet, mapGenerator, this);
	}

	void selectUnit(Unit player) {
		if (player != mapGenerator.selectedUnit) {
			mapGenerator.deselectAllUnits();
			mapGenerator.selectUnit(player, false);
			if (player.transform.parent == mapGenerator.playerTransform || player.transform.parent == mapGenerator.enemyTransform)
				mapGenerator.moveCameraToSelected(false);
		}
	}

	bool isHovering = false;
	public void setHovering() {
		isHovering = true;
	}
	public void removeHovering() {
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

	public bool setRotationToMostInterestingTile() {	
		Tile t = mapGenerator.getMostInterestingTile(this);
		if (t != null)
			setRotationToTile(t);
		return t != null;
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
			if (needsOverlay) {
				doOverlay = true;
				needsOverlay = false;
			}
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
		return !deadOrDyingOrUnconscious() && !inPrimal && !getWeapon().isRanged;
	}

	public int attackOfOpp(Vector2 one, Direction dir) {
		int move = 0;
		/*for (int n=-1;n<=1;n++) {
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
		}*/
		Tile t = mapGenerator.tiles[(int)one.x,(int)one.y];
		foreach (Unit u in mapGenerator.priorityOrder) {
		//	if (u.team != team && (u.playerControlled || u.aiActive) && u.canAttOpp() && u.hasLineOfSightToUnit(this, u.getAttackRange(), true)) {
			if (t.provokesOpportunity(dir, this, u)) {
				move++;
				u.attackEnemy = this;
				u.setRotationToTile(new Vector2(one.x,-one.y));
				u.attackAnimation();
				u.attackAnimating = true;
			}
		}
		return move;
	}

	public void recover() {
		if (isProne()) {
			afflictions.Remove(Affliction.Prone);
			proneAnimation(false);
		}
	//	affliction ^= Affliction.Prone;
	//	affliction = Affliction.None;
		useMovement();
	}

	public bool isPerformingAnAction() {
		return attackAnimating || attacking || throwing || moving || intimidating;
	}

	public bool hasWeapon() {
		if (characterSheet == null) return false;
		Weapon w = characterSheet.characterSheet.characterLoadout.rightHand;
		if (w == null) return false;
		if (w is WeaponMechanical) {
			if (((WeaponMechanical)w).overClocked) return false;
		}
		return true;
	}

	public void markUnit() {
		if (attackEnemy != null) {
			markedUnits.Add(attackEnemy);
			attackEnemy.deselect();
			attackEnemy.setMarked(true);
			attackEnemy = null;
			useMinor(false);
		}
	}

	public bool hasMarkOn(Unit u) {
		if (!characterSheet.characterSheet.characterProgress.hasFeature(ClassFeature.Mark)) return false;
		return markedUnits.Contains(u);
	}

	public bool invoking = false;
	public void startInvoking() {
		if (attackEnemy != null && !invoking) {
			invoking = true;
			if (mapGenerator.getCurrentUnit()==this)
				mapGenerator.moveCameraToPosition(transform.position, false, 90.0f);
		}
	}

	public void startAttacking() {
		startAttacking(false);
	}

	public void startAttacking(bool overClocked) {
		overClockedAttack = overClocked;
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
		isBackStepping = backStepping;
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
			Vector2 from = (Vector2)currentPath[0];
			Vector2 to = (Vector2)currentPath[1];
			shouldMove = attackOfOpp(from, MapGenerator.getDirectionOfTile(mapGenerator.tiles[(int)from.x,(int)from.y], mapGenerator.tiles[(int)to.x,(int)to.y]));
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
		if (!isBackStepping && dist <= 0.5f && doAttOpp && currentPath.Count >= 3 && attopp) {
			Vector2 two = (Vector2)currentPath[2];
			attackOfOpp(one, MapGenerator.getDirectionOfTile(mapGenerator.tiles[(int)one.x,(int)one.y],mapGenerator.tiles[(int)two.x,(int)two.y]));
			doAttOpp = false;
		}
		//		float distX = one.x - zero.x;
		//		float distY = one.y - zero.y;
		if (Mathf.Abs(dist - moveDist) <= 0.001f || moveDist >= dist) {
			//			moving = false;
			unDrawGrid();
			setNewTilePosition(new Vector3(one.x,-one.y,0.0f));
			position = new Vector3(one.x, -one.y, 0.0f);
			Tile newTile = mapGenerator.tiles[(int)one.x,(int)one.y];
			if (newTile.standable) vaultAnimation(false);
			transform.localPosition = new Vector3(one.x + 0.5f, -one.y - 0.5f, 0.0f);
			currentPath.RemoveAt(0);
			moveDist = moveDist - dist;
			currentMoveDist--;
			if (GameGUI.selectedMovementType == MovementType.Move)
				moveDistLeft--;
			currentMaxPath = currentPath.Count - 1;
			mapGenerator.setCurrentUnitTile();
			shouldDoAthleticsCheck = true;
			doAttOpp = true;
			mapGenerator.activateEnemies();
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
				if (team == 0) needsOverlay = true;
			//	if (team == 0) mapGenerator.setOverlay();

			}
			redrawGrid();
		//	if (team == 0) mapGenerator.setOverlay();
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

	Transform markTrans = null;
	public Transform getMark() {
		if (markTrans == null) {
			markTrans = transform.FindChild("Mark");
		}
		return markTrans;
	}
	public SpriteRenderer getMarkSprite() {
		if (markSprite == null) {
			markSprite = getMark().GetComponent<SpriteRenderer>();
		}
		return markSprite;
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
		characterSheet = characterTemplate.loadData(this);
//		characterSheet.unit = this;
//		characterSheet.loadData();
	}

	public void loadCharacterSheetFromTextFile(string textFile) {
		if (characterSheetLoaded) return;
//		characterSheet = new Character();
//		characterSheet.unit = this;
//		characterSheet.loadCharacterFromTextFile(textFile);
		characterSheet = characterTemplate.loadData(textFile, this);
		characterSheet.characterId = textFile;
		characterSheetLoaded = true;
	}

	public void setMapGenerator(MapGenerator mg) {
		mapGenerator = mg;
	//	if (!playerControlled) {
			aiMap = new AStarEnemyMap(this, mapGenerator);
	//	}
	}

	public void removeTurret(TurretUnit tu) {
		if (turrets.Contains(tu)) turrets.Remove(tu);
	}

	public void addTurret(TurretUnit tu) {
		turrets.Add(tu);
	}

	public virtual void initializeVariables() {
//		characterSheet = gameObject.GetComponent<Character>();
		if (getMarkSprite()) {
			getMarkSprite().sortingOrder = MapGenerator.markOrder;
		}
		aiActive = false;
		setMarked(false);
		markedUnits = new List<Unit>();
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
		if (doOverlay) {
			mapGenerator.setOverlay();
			doOverlay = false;
		}
		doMovement();
		doRotation();
		doAttack();
		doThrow();
		doGetThrown();
		doIntimidate();
		doInvoke();
		doDeath();
		setLayer();
		setTargetObjectScale();
		setMarkPosition();
		setTrailRendererPosition();
		setCircleScale();
	}


	public void doTurrets() {
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
						int check = rollForSkill(Skill.Athletics);
						if (check >= passability) {
							BattleGUI.writeToConsole(getName() + " passed Athletics check with a roll of " + check + " (" + (check - athletics) + " + " + athletics + ")");
							vaultAnimation(true);
						}
						else {
							BattleGUI.writeToConsole(getName() + " failed Athletics check with a roll of " + check + " (" + (check - athletics) + " + " + athletics + ") and became prone.");
							shouldCancelMovement = true;
							becomeProne();
							mapGenerator.resetPlayerPath();
							mapGenerator.resetRanges();
							useMovement();
							if (team == 0) needsOverlay = true;
						//	if (team == 0) mapGenerator.setOverlay();

						}
						useMinor(false, false);
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
				if (GameGUI.selectedMovement) {
					if (currentMoveDist == 0) useMovement();
					else BattleGUI.resetMovementButtons();
				}
				if (!setRotationToMostInterestingTile()) {
			//		rotating = true;
/*					if (needsOverlay) {
						doOverlay = true;
						needsOverlay = false;
					}*/
				}
				if (!usedStandard && hasLineOfSightToUnit(closestEnemy(), getAttackRange(), true, (getWeapon().isRanged ? VisibilityMode.Ranged : VisibilityMode.Melee))) {
					GameGUI.selectStandardType(StandardType.Attack);
				}
				if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape) {
					useMinor();
				//	minorsLeft--;
				//	GameGUI.selectMinor(MinorType.None);
					escapeUsed = true;
					BattleGUI.resetMinorButtons();
				}
			}
		}
	}

	public void moveFinished() {
	//	Debug.Log("Move Finished");
		if (needsOverlay) {
			doOverlay = true;
			needsOverlay = false;
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
			if (moveDistLeft < maxMoveDist) {
				useMovement();
			}
			useStandard();
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
					if (u.rollForSkill(Skill.Athletics) < 15) {
//					u.affliction = Affliction.Prone;
						u.becomeProne();
						alsoProne = u;
					}
				}
			}
		}
		if (team == 0) {
			BattleGUI.writeToConsole(getName() + " was thrown " + (dis*5) + " feet by " + thrownBy.getName() + (becameProne ? " and was knocked prone" + (alsoProne!=null?" along with " + alsoProne.getName():"") : "") + "!", Color.red);
		}
		else {
			BattleGUI.writeToConsole(thrownBy.getName() + " threw " + getName() + " " + (dis*5) + " feet" + (becameProne ? " and knocked " + (characterSheet.characterSheet.personalInformation.getCharacterSex()==CharacterSex.Female ? "her":"him") + " prone" + (alsoProne!=null?" along with " + alsoProne.getName():"") : "") + "!", Log.greenColor);
		}
		gettingThrown = true;
//		gettingThrownPosition = new Vector3(x, -y, position.z);
		currentPath.Add(new Vector2(x, y));
	//	setPosition(new Vector3(x, -y, position.z));
	}

	void becomeProne() {
		if (!isProne()) {
			afflictions.Add(Affliction.Prone);
			proneAnimation(true);
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
			useMovement();
		}
	}

	public bool invokeAnimating = false;
	void doInvoke() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (invoking && !moving && !rotating) {
			invokeAnimation();
			invokeAnimating = true;
			invoking = false;
			useMovementIfStarted();
			useMinor(false);
//			minorsLeft--;
			invokeUsesLeft--;
			if (invokeUsesLeft == 0) {
				BattleGUI.resetMinorButtons();
				chooseNextBestActionType();
			}

		}
	}
	
	void invokeAnimation() {
		dealInvokeDamage();
		if (attackEnemy != null) {
			if (attackEnemy.isTarget) attackEnemy.deselect();
			attackEnemy = null;
		}
	}

	public bool intimidateAnimating = false;
	void doIntimidate() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (intimidating && !moving && !rotating) {
			intimidateAnimation();
			intimidateAnimating = true;
			intimidating = false;
			useStandard();
			useMovementIfStarted();
		}
	}

	void intimidateAnimation() {
		dealIntimidationDamage();
		mapGenerator.resetAttack(this);
		if (this == mapGenerator.getCurrentUnit())
			mapGenerator.resetRanges();
	}

	public virtual RaceName getRaceName() {
		return characterSheet.characterSheet.personalInformation.getCharacterRace().raceName;
	}

	public virtual bool hasUncannyKnowledge() {
		return characterSheet.characterSheet.characterProgress.hasFeature(ClassFeature.Uncanny_Knowledge);
	}

	public virtual bool attackEnemyIsFavoredRace() {
		if (attackEnemy==null) return false;
		return unitIsFavoredRace(attackEnemy);
	}

	public virtual bool unitIsFavoredRace(Unit u) {
		return raceIsFavoredRace(u.getRaceName());
	}

	public virtual bool raceIsFavoredRace(RaceName race) {
		return race == characterSheet.characterSheet.characterProgress.getFavoredRace() && race != RaceName.None;
	}

	public virtual int rollForSkill(Skill skill, bool favoredRace = false, int dieType = 10, int dieRoll = -1) {
		return characterSheet.rollForSkill(skill, favoredRace, dieType, dieRoll);
	}

	public virtual int getSkill(Skill skill) {
		return characterSheet.characterSheet.skillScores.getScore(skill);
	}


	void dealIntimidationDamage() {
		if (attackEnemy != null) {
			int sturdy = rollForSkill(Skill.Melee, attackEnemyIsFavoredRace(), 20);
			int wellV = 10 + attackEnemy.characterSheet.characterSheet.combatScores.getWellVersedMod();
			bool didHit = sturdy >= wellV;
			int wapoon = Mathf.Max(1, characterSheet.combatScores.getSturdyMod());
			DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
			damageDisplay.begin(wapoon, didHit, false, attackEnemy, Color.green);
			if (didHit) {
				attackEnemy.damageComposure(wapoon, this);
				attackEnemy.setRotationToCharacter(this);
			}
		}
	}

	void dealInvokeDamage() {
		if (attackEnemy != null) {
			int political = rollForSkill(Skill.Political, attackEnemyIsFavoredRace(), 20);
			int wellV = 10 + attackEnemy.characterSheet.characterSheet.combatScores.getWellVersedMod();
			bool didHit = political >= wellV;
			int wapoon = Mathf.Max(1, characterSheet.combatScores.getWellVersedMod());
			DamageDisplay damageDisplay = ((GameObject)GameObject.Instantiate(damagePrefab)).GetComponent<DamageDisplay>();
			damageDisplay.begin(wapoon, didHit, false, attackEnemy, Color.green);
			if (didHit) {
				attackEnemy.damageComposure(wapoon, this);
				attackEnemy.setRotationToCharacter(this);
			}
		}
	}

	

	public void damageComposure(int damage, Unit u) {
		if (damage > 0 && !characterSheet.characterSheet.combatScores.isInPrimalState()) {
			crushingHitSFX();
			characterSheet.combatScores.loseComposure(damage);
			if (characterSheet.characterSheet.combatScores.isInPrimalState()) {
				inPrimal = true;
				primalInstigator = u;
				primalTurnsLeft = u.characterSheet.characterSheet.combatScores.getDominion() + 1;
			}
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

	void vaultAnimation(bool vaulting) {
		anim.SetBool("Vault", vaulting);
		vaultAnimationAllSprites(vaulting);
	}

	void moveAnimation(bool moving) {
		anim.SetBool("Move",moving);
		movementAnimationAllSprites(moving);
	}

	void idleAnimation(bool idle) {
		anim.SetBool("Idle",idle);
		idleAnimationAllSprites(idle);
	}

	void proneAnimation(bool prone) {
		anim.SetBool("Prone",prone);
		proneAnimationAllSprites(prone);
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

	void idleAnimationAllSprites(bool idle) {
		setAllSpritesBool("Idle",idle);
	}

	void proneAnimationAllSprites(bool prone) {
		setAllSpritesBool("Prone",prone);
	}

	void deathAnimationAllSprites() {
		setAllSpritesTrigger("Death");
	}

	void vaultAnimationAllSprites(bool vaulting) {
		setAllSpritesBool("Vault", vaulting);
	}

	void setAllSpritesBool(string boolName, bool b) {
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
	public virtual int getMeleeScoreWithMods(Unit u) {
		return getMeleeScore() + (unitIsFavoredRace(u) ? 1 : 0) + (Combat.flanking(this,u) ? 2 : 0) + (hasUncannyKnowledge() ? 1 : 0) + (hasWeaponFocus() ? 2 : 0) - temperedHandsMod;
	}

	public virtual int rollDamage(bool crit) {
		return characterSheet.rollDamage(attackEnemy, crit) + temperedHandsMod;
	}

	public virtual int overClockDamage() {
		return characterSheet.overloadDamage();
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
		return getWeapon() != null && getWeapon().damageType != DamageType.None && getWeapon().damageType == characterSheet.characterSheet.characterProgress.getWeaponFocus();
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
		int wapoon = (overClockedAttack ?  overClockDamage() : rollDamage(crit));//.characterLoadout.rightHand.rollDamage();
		bool didHit = hit.hit >= enemyAC || hit.crit;
		attackEnemy.showDamage(wapoon, didHit, crit);
		BattleGUI.writeToConsole(getName() + (didHit ? (overClockedAttack ? " over clocked " : (crit ? " critted " : " hit ")) : " missed ") + attackEnemy.getName() + (didHit ? " with " + (getWeapon() == null ?  getGenderString() + " fist " : getWeapon().itemName + " ") + "for " + wapoon + " damage!" : "!"), (team==0 ? Log.greenColor : Color.red));
		if (didHit)
			attackEnemy.damage(wapoon, this);
		if (overClockedAttack) {
			Debug.Log("Over Clocked Attack!!!");
			Weapon w = characterSheet.characterSheet.characterLoadout.rightHand;
			if (w is ItemMechanical) {
				((WeaponMechanical)w).overClocked = true;
				GameGUI.selectedStandardType = StandardType.None;
			}
		}
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
		if (this.team==0) BattleGUI.writeToConsole(getName() + " " + enemy.deathString() + " " + enemy.getName() + "!", Log.greenColor);
		else BattleGUI.writeToConsole(enemy.getName() + " was " + enemy.deathString() +" by " + getName() + "!",Color.red);
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
		attackAnimating = false;
		if (this == mapGenerator.getCurrentUnit()) {
			mapGenerator.resetRanges();
			useMovementIfStarted();
			useStandard();
		}

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


	
	public void deleteCharacter() {
		characterSheet.deleteCharacter();
	}
	
	public void saveCharacter() {
		characterSheet.saveCharacter();
	}

}
