using UnityEngine;
using System.Collections;

public class TurretUnit : MechanicalUnit  {

	public Turret turret;
	public Direction direction;
	public Unit owner;
	public bool isOn = true;

	// Use this for initialization
	void Start ()  {
	
	}
	
	// Update is called once per frame
	void Update ()  {
		doDeath();
		doAttack();
	}

	public override int getCurrentHealth()  {
		return turret.getHealth();
	}
	public override int getMaxHealth()  {
		return turret.getMaxHealth();
	}
	public override int getCurrentComposure()  {
		return 0;
	}
	public override int getMaxComposure()  {
		return 0;
	}

	public override float getComposurePercent()  {
		return 100.0f;
	}

	public override void loseHealth(int amount)  {
		turret.takeDamage(amount);
	}

	
	public override RaceName getRaceName()  {
		return RaceName.None;
	}
	
	public override bool attackEnemyIsFavoredRace()  {
		return unitIsFavoredRace(attackEnemy);
	}
	
	public override bool unitIsFavoredRace(Unit u)  {
		return raceIsFavoredRace(u.getRaceName());
	}
	
	public override bool raceIsFavoredRace(RaceName race)  {
		return false;
	}

	public override bool givesDecisiveStrike()  {
		return false;
	}

	public override int getMeleeScore()  {
		return owner == null ? 0 : owner.getSkill(Skill.Mechanical);
	}

	public override int getCritChance()  {
		return 0;
	}
	
	public override Weapon getWeapon()  {
		if (turret==null) return null;
		return turret.applicator;
	}
	
	public override string getGenderString()  {
		return "its";
	}
	
	public override int rollDamage(bool crit)  {
		return turret.rollDamage() + (owner.characterSheet.characterSheet.characterProgress.hasFeature(ClassFeature.Metallic_Affinity) && owner.characterSheet.characterId == turret.creatorId ? 1 : 0);
	}

	
	public override int rollForSkill(Skill skill, bool favoredRace = false, int dieType = 10, int dieRoll = -1)  {
		if (dieRoll == -1) dieRoll = Random.Range(1, dieType + 1);
		return (skill==Skill.Melee ? getMeleeScore() : 0) + (favoredRace?1:0) + dieRoll;
	}

	public override bool hasWeaponFocus()  {
		return false;
	}

	public override bool hasUncannyKnowledge()  {
		return false;
	}

	void doAttack()  {
		if (mapGenerator.movingCamera && mapGenerator.getCurrentUnit()==this) return;
		if (attacking)  {
			attacking = false;
			dealDamage();
			if (attackEnemy)  {
				attackEnemy.wasBeingAttacked = attackEnemy.beingAttacked;
				attackEnemy.beingAttacked = false;
				attackEnemy.attackedByCharacter = null;
			}
			mapGenerator.resetAttack(this);
			if (this == mapGenerator.getCurrentUnit())
				mapGenerator.resetRanges();
			attackAnimating = false;
			turret.use();
		}
	}
	public override void doDeath()  {
		if (isDead())  {
			if (!mapGenerator.selectedUnit || !mapGenerator.selectedUnit.attacking)  {
				if (mapGenerator.selectedUnit)  {
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

	public override string getStatusSummary()  {
		return string.Format(" {0}\nHP:  {1}/ {2}\nTurns Left:  {3}/ {4}", getName(), getCurrentHealth(), getMaxHealth(), turret.turnsLeft(), turret.maxTurns());
	}

	public override int getTeam()  {
		return owner.getTeam();
	}

	public override int getAC()  {
		if (turret == null) return 0;
		return turret.frame.getHardness();
	}

	public override bool canAttOpp(Unit u)  {
		return false;
	}

	public override string getName()  {
		return owner.getName() + "'s Turret";
	}
	
	public override bool isDead()  {
		if (turret == null) return false;
		return turret.isDestroyed() || !turret.hasUsesLeft();
	}
	public override string deathString()  {
		return "destroyed";
	}


	public override bool deadOrDyingOrUnconscious()  {
		return isDead();
//		return false;
	}

	public void setDirection(Direction dir)  {
		direction = dir;
		switch (direction)  {
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

	
	public override void setPosition(Vector3 pos)  {
	//	setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}

	public bool fireOnTile(Tile t, int distLeft)  {
		if (t==null) return false;
		if (t.hasCharacter() && t.getCharacter() != this)  {//.hasEnemy(this))  {
			Debug.Log("Has Enemy");
			attackEnemy = t.getCharacter();//.getEnemy(this);
			if (attackEnemy)
				attackEnemy.setTarget();
			attacking = true;
			return true;
		}
		if (distLeft == 0) return false;
		return fireOnTile(t.getTile(direction), distLeft-1);
	}

	public void fire()  {
		Debug.Log("Turret Fire");
		if (!isOn) return;
		fireOnTile(mapGenerator.tiles[(int)position.x,(int)-position.y], 5);
		turret.use();
	}

}
