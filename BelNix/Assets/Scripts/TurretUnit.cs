using UnityEngine;
using System.Collections;

public class TurretUnit : Unit {

	public Turret turret;
	public Direction direction;
	public Unit owner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		doDeath();
		doAttack();
	}
	
	public override void loseHealth(int amount) {
		turret.takeDamage(amount);
	}

	public override bool givesDecisiveStrike() {
		return false;
	}

	public override int getMeleeScore() {
		return 0;
	}

	public override int getCritChance() {
		return 0;
	}
	
	public override Weapon getWeapon() {
		if (turret==null) return null;
		return turret.applicator;
	}
	
	public override string getGenderString() {
		return "its";
	}
	
	public override int rollDamage(bool crit) {
		return turret.rollDamage();
	}

	void doAttack() {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (attacking) {
			attacking = false;
			dealDamage();
			if (attackEnemy) {
				attackEnemy.wasBeingAttacked = attackEnemy.beingAttacked;
				attackEnemy.beingAttacked = false;
				attackEnemy.attackedByCharacter = null;
			}
			mapGenerator.resetAttack(this);
			if (this == mapGenerator.getCurrentUnit())
				mapGenerator.resetRanges();
			attackAnimating = false;
		}
	}
	public override void doDeath() {
		if (isDead()) {
			if (!mapGenerator.selectedUnit || !mapGenerator.selectedUnit.attacking) {
				if (mapGenerator.selectedUnit) {
					//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
					Unit p = mapGenerator.selectedUnit;
					if (p.attackEnemy==this) p.attackEnemy = null;
				}
				//				mapGenerator.enemies.Remove(gameObject);
			//	mapGenerator.removeCharacter(this);
				owner.removeTurret(this);
				Tile t = mapGenerator.tiles[(int)position.x, (int)-position.y];
				if (t.getCharacter()==this)
					t.removeCharacter();
				Destroy(gameObject);
				mapGenerator.resetCharacterRange();
			}
		}
		//	Debug.Log("End Death");
	}

	public override int getAC() {
		return turret.frame.getHardness();
	}

	public override bool canAttOpp() {
		return false;
	}

	public override string getName() {
		return owner.getName() + "'s Turret";
	}
	
	public override bool isDead() {
		if (turret == null) return false;
		return turret.isDestroyed();
	}
	public override string deathString() {
		return "destroyed";
	}


	public override bool deadOrDyingOrUnconscious() {
		return isDead();
//		return false;
	}

	public void setDirection(Direction dir) {
		direction = dir;
		switch (direction) {
		case Direction.Down:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
			break;
		case Direction.Up:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			break;
		case Direction.Left:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
			break;
		case Direction.Right:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
			break;
		default:
			break;
		}
	}

	
	public override void setPosition(Vector3 pos) {
	//	setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}

	public void fireOnTile(Tile t, int distLeft) {
		if (t==null) return;
		if (t.hasEnemy(this)) {
			Debug.Log("Has Enemy");
			attackEnemy = t.getEnemy(this);
			if (attackEnemy)
				attackEnemy.setTarget();
			attacking = true;
			return;
		}
		if (distLeft == 0) return;
		fireOnTile(t.getTile(direction), distLeft-1);
	}

	public void fire() {
		Debug.Log("Turret Fire");
		fireOnTile(mapGenerator.tiles[(int)position.x,(int)-position.y], 5);
	}

}
