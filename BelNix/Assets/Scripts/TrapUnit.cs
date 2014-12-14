﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapUnit : MechanicalUnit {
	
	public Unit owner;
	public Trap trap;
	public List<TrapUnit> fullTrap;
	public bool selectedForPlacement;

	public void setSelectedForPlacement() {
		selectedForPlacement = true;
	}

	public void unsetSelectedForPlacement() {
		selectedForPlacement = false;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	public void doPlacementExpand() {
		if (!selectedForPlacement) return;
		float factor = 1.0f/10.0f;
		float speed = 3.0f;
		float addedScale = Mathf.Sin(Time.time * speed) * factor;
		float scale = 1.0f + factor + addedScale;
		transform.localScale = new Vector3(scale, scale, 1.0f);
	}

	
	public override void setPosition(Vector3 pos) {
		//	setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
		//	currentMaxPath = 0;
		//	resetPath();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		doPlacementExpand();
		doDeath();
		doAttack();
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
		if (trap==null) return null;
		return trap.applicator;
	}
	
	public override string getGenderString() {
		return "its";
	}
	
	public override int rollDamage(bool crit) {
		return trap.rollDamage();
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
			trap.use();
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
				Tile t = mapGenerator.tiles[(int)position.x, (int)-position.y];
				if (t.getTrap()==this)
					t.removeTrap();
				Destroy(gameObject);
				mapGenerator.resetCharacterRange();
			}
		}
		//	Debug.Log("End Death");
	}

	public override bool hasWeaponFocus () {
		return false;
	}
	
	public override string getName() {
		return owner.getName() + "'s Trap";
	}
	
	public override bool isDead() {
		if (trap == null) return false;
		return !trap.hasUsesLeft();
	}
	public override string deathString() {
		return "destroyed";
	}


}