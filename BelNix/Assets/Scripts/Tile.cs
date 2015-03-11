using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public enum GameMasterType  {Damage1, Heal1, Heal10};
public class TileAction : IComparable  {
	public List<MinorType> minorTypes;
	public List<StandardType> standardTypes;
	public List<MovementType> movementTypes;
	public List<GameMasterType> gameMasterTypes;
	public Tile movementTile;
	public Tile actualTile;
	public int percentChance;
	public bool hasChance;

	public int value()  {
		if (standardTypes.Count > 0)  {
			if (movementTypes.Count > 0)  {
				if (minorTypes.Count > 0) return 10;
				return 9;
			}
			else if (minorTypes.Count > 0)  {
				return 8;
			}
			return 3;
		}
		else if (movementTypes.Count > 0)  {
			if (minorTypes.Count > 0) return 7;
			return 2;
		}
		else if (minorTypes.Count > 0)  {
			return 1;
		}
		else return 0;
	}

	public TileAction()  {
		minorTypes = new List<MinorType>();
		standardTypes = new List<StandardType>();
		movementTypes = new List<MovementType>();
		gameMasterTypes = new List<GameMasterType>();
	}

	public TileAction(MovementType[] movements, StandardType[] standards, MinorType[] minors, Tile tile, Tile mTile = null, int chance = -1) : this()  {
		if (movements != null) foreach (MovementType t in movements) addAction(t);
		if (standards != null) foreach (StandardType t in standards) addAction(t);
		if (minors != null) foreach (MinorType t in minors) addAction(t);
		movementTile = mTile;
		actualTile = tile;
		setPercentChance(chance);
	}
	public TileAction(GameMasterType[] gameMasters, Tile tile) : this()  {
		if (gameMasters != null) foreach (GameMasterType t in gameMasters) addAction(t);
		actualTile = tile;
	}

	public void setPercentChance(int chance)  {
		percentChance = chance;
		hasChance = (chance >= 0 && chance <= 100);
	}

	public void addAction(MinorType t)  {
		minorTypes.Add(t);
	}

	public void addAction(MovementType t)  {
		movementTypes.Add(t);
	}
	
	public void addAction(StandardType t)  {
		standardTypes.Add(t);
	}
	
	public void addAction(GameMasterType t)  {
		gameMasterTypes.Add(t);
	}

	public bool hasMovement()  {
		return movementTile != null;
	}

	public bool hasTrap()  {
		return standardTypes.Contains(StandardType.Lay_Trap);
	}

	public bool hasTurret()  {
		return standardTypes.Contains(StandardType.Place_Turret);
	}

	public int getMovementLength()  {
		if (movementTypes.Contains(MovementType.Move)) return MapGenerator.mg.getCurrentUnit().moveDistLeft;
		if (movementTypes.Contains(MovementType.BackStep)) return 1;
		if (minorTypes.Contains(MinorType.Escape)) return 2;
		return 0;
	}

	public int CompareTo(object obj)  {
		if (obj == null) return 1;
		TileAction otherAction = obj as TileAction;
		if (otherAction == null) return 1;
		return value().CompareTo(otherAction.value());
	}

	public string toString()  {
		bool has = false;
		string s = "";
		foreach (MovementType move in movementTypes)  {
			if (has) s += " & ";
			s += "<color=#131394>" + Unit.getNameOfMovementType(move) + "</color>";
			has = true;
		}
		foreach (StandardType standard in standardTypes)  {
			if (has) s += " & ";
			s += "<color=#941313>" + Unit.getNameOfStandardType(standard) + "</color>";
			has = true;
		}
		foreach (MinorType minor in minorTypes)  {
			if (has) s += " & ";
			s += "<color=#139423>" + Unit.getNameOfMinorType(minor) + "</color>";
			has = true;
		}
		foreach (GameMasterType master in gameMasterTypes)  {
			if (has) s += " & ";
			s += "<color=#941313>" + Tile.getNameOfGameMaster(master) + "</color>";
			has = true;
		}
		if (hasChance)  {
			if (minorTypes.Count > 0)  {
				s += "<color=#139423>";
			}
			else  {
				s += "<color=#941313>";
			}
			s += " (" + percentChance + "%)";
			s += "</color>";
		}
		return s;
	}
}

public enum Direction  {Up, Down, Right, Left, None};

public class Tile  {

	public static string getNameOfGameMaster(GameMasterType t)  {
		switch (t)  {
		case GameMasterType.Damage1:
			return "Deal 1 Damage";
		case GameMasterType.Heal1:
			return "Heal 1 Health";
		case GameMasterType.Heal10:
			return "Heal 10 Health";
		default:
			return t.ToString();
		}
	}

	public MeshGen meshGen;
//	GameObject player;
//	GameObject enemy;
	Unit character;
	TrapUnit trap;
	public bool standable;
	int passableLeft;
	int passableRight;
	int passableUp;
	int passableDown;
	int visibleLeft;
	int visibleRight;
	int visibleUp;
	int visibleDown;
	public Tile leftTile;
	public Tile rightTile;
	public Tile upTile;
	public Tile downTile;
	int trigger;
	int activation;
	public int minDistCurr;
	public int minAttackCurr;
	public int minSpecialCurr;
	public int minDistUsedMinors;
	public bool attackFromTurret = false;
	public bool canUseSpecialCurr;
	public bool canAttackCurr;
	public bool canStandCurr;
	public bool canTurn;
	public bool startingPoint;
	public int x;
	public int y;
	List<Item> items;
	public bool currentRightClick = false;
	public static List<TextAsset> playedConversations = new List<TextAsset>();
	public TextAsset conversationText = null;
	public int groundMoney;

	public void setMoney(int money) {
		groundMoney = money;
	}

	public int getMoney() {
		return groundMoney;
	}

	public string getMoneyString() {
		return Purse.moneyString(getMoney());
	}

	public void removeMoney() {
		groundMoney = 0;
	}
	
	
	public bool triggerBitSet(int bit)  {
		return ((1 << bit) & trigger) != 0;
	}

	public bool hasConversation()  {
		return conversationText != null && !playedConversations.Contains(conversationText);
	}

	public void playConversation()  {
		playedConversations.Add(conversationText);
		Conversation.beginConversation(conversationText);
	}


	public float distanceFromTile(Tile t, bool manhattan = true)  {
		if (manhattan) return Mathf.Abs(t.x - x) + Mathf.Abs(t.y - y);
		return Mathf.Sqrt((t.x - x) * (t.x - x) + (t.y - y) * (t.y - y));
	}

	public List<TileAction> getTileActions(Unit u)  {
		Tile uTile = u.mapGenerator.tiles[(int)u.position.x,(int)-u.position.y];
		bool enemy = hasEnemy(u);
		bool ally = hasAlly(u);
		bool stand = canStand();
		bool med = u.getWeapon() is Medicinal;
		bool mech = u.getWeapon() is WeaponMechanical;
		bool canAttack = false;
		bool canBackStep = false;
		bool canAttackAfterBackStep = false;
		bool canAttackAfterMove = false;
		bool canMove = false;
		bool canLoot = false;
	//	bool canLootAfterBackStep = false;
		bool canLootAfterMove = false;
		Tile backstepLootTile = null;
		Tile moveLootTile = null;
		float backstepLootTileDist = float.MaxValue;
		float moveLootTileDist = float.MaxValue;
		bool canEscape = false;
		Tile backstepAttackTile = null;
		Tile moveAttackTile = null;
		float backstepAttackTileDist = float.MaxValue;
		float moveAttackTileDist = float.MaxValue;
		bool canThrow = false;
		bool canThrowAfterBackStep = false;
		bool canThrowAfterMove = false;
		Tile backstepThrowTile = null;
		Tile moveThrowTile = null;
		float backstepThrowTileDist = float.MaxValue;
		float moveThrowTileDist = float.MaxValue;
		bool canOrator = false;
		bool canOratorAfterBackStep = false;
		bool canOratorAfterMove = false;
		Tile backstepOratorTile = null;
		Tile moveOratorTile = null;
		float backstepOratorTileDist = float.MaxValue;
		float moveOratorTileDist = float.MaxValue;
		bool canMark = false;
		bool canStealth = false;
		bool canTemperedHands = false;
		bool canOneOfMany = false;
		bool canRecover = false;
		bool canExamine = false;
		bool canTrap = false;
		bool canTurret = false;
		bool canActivateTurret = false;
		bool canActivateTurretAfterMove = false;
		bool canPickUpTrapTurret = false;
		bool canPickUpTrapTurretAfterBackstep = false;
		bool canPickUpTrapTurretAfterMove = false;
		Tile backstepTrapTurretTile = null;
		Tile moveTrapTurretTile = null;
		float backstepTrapTurretTileDist = float.MaxValue;
		float moveTrapTurretTileDist = float.MaxValue;
		StandardType trapTurretType = StandardType.None;
		Tile moveTurretTile = null;
		float moveTurretTileDist = float.MaxValue;
		bool hasTrapTurret = false;
		bool canMoveBody = !u.usedStandard && hasCharacter() && getCharacter().deadOrDyingOrUnconscious() && u.mapGenerator.hasLineOfSight(uTile, this, 1, true);
		Trap tr = null;
		Turret tur = null;
		if (hasCharacter()) {
			Unit u2 = getCharacter();
			if (u2 is TurretUnit) {
				tur = ((TurretUnit)u2).turret;
				hasTrapTurret = ((TurretUnit)u2).owner == u;
				trapTurretType = StandardType.PickUpTurret;
			}
		}
		if (hasTrap()) {
			tr = getTrap().trap;
			hasTrapTurret = getTrap().owner == u;
			trapTurretType = StandardType.PickUpTrap;
		}
		Debug.Log("HasTrapTurret: " + hasTrapTurret + "  " + trapTurretType);
		u.mapGenerator.removeAllRanges(false);
		HashSet<Tile> movements = (u.usedMovement || u.isProne() ? new HashSet<Tile>() : u.mapGenerator.setCharacterCanStand((int)u.position.x, (int)-u.position.y, u.moveDistLeft, 0, u.getAttackRange(), u, false));
		canMove = canStandCurr && stand;
		u.mapGenerator.removeAllRanges(false);
		HashSet<Tile> backSteps = (!u.canBackStep() || u.usedMovement || u.isProne() ? new HashSet<Tile>() : u.mapGenerator.setCharacterCanStand((int)u.position.x, (int)-u.position.y, 1, 0, u.getAttackRange(), u, false));
		canBackStep = canStandCurr && stand;
		u.mapGenerator.removeAllRanges(false);
		if (!u.escapeUsed && u.hasClassFeature(ClassFeature.Escape) && u.minorsLeft > 0)  {
			HashSet<Tile> escapeSteps = u.mapGenerator.setCharacterCanStand((int)u.position.x, (int)-u.position.y, 2, 0, u.getAttackRange(), u);
			canEscape = canStandCurr && stand;
			u.mapGenerator.removeAllRanges(false);
		}
		u.mapGenerator.resetRanges(false);
		if (!u.usedStandard)  {
			canAttack = (med ? ally : enemy) && u.mapGenerator.hasLineOfSight(uTile, this, u.getAttackRange(), true, u.attackVisibilityMode());
			if (!canAttack && (med ? ally : enemy))  {
				foreach (Tile t in backSteps)  {
					if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, u.getAttackRange(), true, u.attackVisibilityMode()))  {
						canAttackAfterBackStep = true;
						float d = uTile.distanceFromTile(t);
						if (d < backstepAttackTileDist)  {
							backstepAttackTile = t;
							backstepAttackTileDist = d;
						}
					}
				}
			}
			if (!canAttackAfterBackStep && !canAttack && (med ? ally : enemy))  {
				foreach (Tile t in movements)  {
					if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, u.getAttackRange(), true, u.attackVisibilityMode()))  {
						canAttackAfterMove = true;
						float d = uTile.distanceFromTile(t);
						if (d < moveAttackTileDist)  {
							moveAttackTile = t;
							moveAttackTileDist = d;
						}
					}
				}
			}
			if (hasTrapTurret) {
				canPickUpTrapTurret = u.mapGenerator.hasLineOfSight(uTile, this, 1, true);
				if (!canPickUpTrapTurret) {
					foreach (Tile t in backSteps)  {
						if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 1, true))  {
							canPickUpTrapTurretAfterBackstep = true;
							float d = uTile.distanceFromTile(t);
							if (d < backstepTrapTurretTileDist)  {
								backstepTrapTurretTile = t;
								backstepTrapTurretTileDist = d;
							}
						}
					}
				}
				if (!canPickUpTrapTurret && !canPickUpTrapTurretAfterBackstep) {
					foreach (Tile t in movements)  {
						if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 1, true))  {
							canPickUpTrapTurretAfterMove = true;
							float d = uTile.distanceFromTile(t);
							if (d < moveTrapTurretTileDist)  {
								moveTrapTurretTile = t;
								moveTrapTurretTileDist = d;
							}
						}
					}
				}
			}

			if ((u.hasClassFeature(ClassFeature.Throw) || u.hasClassFeature(ClassFeature.Intimidate)) && this != uTile)  {
				canThrow = (enemy || ally) && u.mapGenerator.hasLineOfSight(uTile, this, 1, true, VisibilityMode.Melee);
				if (!canThrow && (enemy || ally))  {
					foreach (Tile t in backSteps)  {
						if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 1, true, VisibilityMode.Melee))  {
							canThrowAfterBackStep = true;
							float d = uTile.distanceFromTile(t);
							if (d < backstepThrowTileDist)  {
								backstepThrowTile = t;
								backstepThrowTileDist = d;
							}
						}
					}
				}
				if (!canThrowAfterBackStep && !canThrow && (enemy || ally))  {
					foreach (Tile t in movements)  {
						if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 1, true, VisibilityMode.Melee))  {
							canThrowAfterMove = true;
							float d = uTile.distanceFromTile(t);
							if (d < moveThrowTileDist)  {
								moveThrowTile = t;
								moveThrowTileDist = d;
							}
						}
					}
				}
			}
			if (u.hasTrap() && !hasCharacter() && u.hasLineOfSightToTile(this, null, 1, true))  {
				canTrap = true;
			}
			if (u.hasTurret() && !hasCharacter() && u.hasLineOfSightToTile(this, null, 1, true))  {
				canTurret = true;
			}

		}
		if (u.minorsLeft > 0)  {
			if (u.hasClassFeature(ClassFeature.Mark))  {
				canMark = enemy && u.hasLineOfSightToUnit(getCharacter());
			}
			if (uTile == this)  {
				canStealth = true;
				if (u.hasClassFeature(ClassFeature.One_Of_Many) && !u.oneOfManyUsed)  {
					canOneOfMany = true;
				}
				if (u.hasClassFeature(ClassFeature.Tempered_Hands) && u.temperedHandsUsesLeft > 0)  {
					canTemperedHands = true;
				}
			}
			if (enemy)  {
				canExamine = true;
			}
			if (getItems().Count > 0 && (!enemy || getEnemy(u).isDead()))  {
				canLoot = u.mapGenerator.hasLineOfSight(u, this, 1, true);
			/*	if (!canLoot)  {
					foreach (Tile t in backSteps)  {
						if (t.canStand() && t!=uTile && u.minorsLeft - t.minDistUsedMinors > 0 && u.mapGenerator.hasLineOfSight(t, this, 1, true))  {
							canLootAfterBackStep = true;
							float d = uTile.distanceFromTile(t);
							if (d < backstepLootTileDist)  {
								backstepLootTile = t;
								backstepLootTileDist = d;
							}
						}
					}
				}*/
				if (!canLoot)  {// && !canLootAfterBackStep)  {
					foreach (Tile t in movements)  {
						if (t.canStand() && t!=uTile && u.minorsLeft - t.minDistUsedMinors > 0 && u.mapGenerator.hasLineOfSight(t, this, 1, true))  {
							canLootAfterMove = true;
							float d = uTile.distanceFromTile(t);
							if (d < moveLootTileDist)  {
								moveLootTile = t;
								moveLootTileDist = d;
							}
						}
					}
				}
			}
			if (hasCharacter() && getCharacter() is TurretUnit && hasTrapTurret)  {
				canActivateTurret = u.mapGenerator.hasLineOfSight(u, this, 1, true);
				if (!canActivateTurret)  {
					foreach (Tile t in movements)  {
						if (t.canStand() && t!=uTile && u.minorsLeft - t.minDistUsedMinors > 0 && u.mapGenerator.hasLineOfSight(t, this, 1, true))  {
							canActivateTurretAfterMove = true;
							float d = uTile.distanceFromTile(t);
							if (d < moveTurretTileDist)  {
								moveTurretTile = t;
								moveTurretTileDist = d;
							}
						}
					}
				}
			}
//				u.mapGenerator.isWithinDistance(1.0f, new Vector2(u.position.x, -u.position.y), getPosition(), true);
		}
		if (!u.usedMovement)  {
			if (uTile == this)  {
				if (u.isProne())  {
					canRecover = true;
				}
			}
		}
		if ((u.hasClassFeature(ClassFeature.Instill_Paranoia) || (u.hasClassFeature(ClassFeature.Invoke) && u.invokeUsesLeft > 0)) && enemy)  {
			canOrator = u.mapGenerator.hasLineOfSight( uTile, this, 5, true);
			if (!canOrator)  {
				foreach (Tile t in backSteps)  {
					if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 5, true))  {
						canOratorAfterBackStep = true;
						float d = uTile.distanceFromTile(t);
						if (d < backstepOratorTileDist)  {
							backstepOratorTile = t;
							backstepOratorTileDist = d;
						}
					}
				}
			}
			if (!canOrator && !canOratorAfterBackStep)  {
				foreach (Tile t in movements)  {
					if (t.canStand() && t!=uTile && u.mapGenerator.hasLineOfSight(t, this, 5, true))  {
						canOratorAfterMove = true;
						float d = uTile.distanceFromTile(t);
						if (d < moveOratorTileDist)  {
							moveOratorTile = t;
							moveOratorTileDist = d;
						}
					}
				}
			}
		}
		/*
		bool canAttack = canAttackCurr && enemy;
		u.mapGenerator.removeAllRanges(false);
		if (u.canBackStep())
			u.mapGenerator.setCharacterCanStand((int)u.position.x, (int)-u.position.y, 1, 0, u.getAttackRange(), u, !canAttack);
		bool canBackStep = canStandCurr && stand;
		bool canAttackAfterBackStep = canAttackCurr && enemy;
		if (u.canBackStep())
			u.mapGenerator.removeAllRanges(false);
		Debug.Log("Count is: " + u.mapGenerator.setCharacterCanStand((int)u.position.x, (int)-u.position.y, u.moveDistLeft, 0, u.getAttackRange(), u, !canAttack && !canAttackAfterBackStep).Count);
		bool canAttackAfterMove = canAttackCurr && enemy;
		bool canMove = canStandCurr && stand;
		u.mapGenerator.removeAllRanges(false);
		bool canLoot = getItems().Count > 0 && u.mapGenerator.isWithinDistance(1.0f, new Vector2(u.position.x, -u.position.y), getPosition(), true);*/
		List<TileAction> tileActions = new List<TileAction>();
		bool addAttacks = !med || (u.getWeapon() as Medicinal).numberOfUses >= 1;
		StandardType att = (med ? StandardType.Heal : StandardType.Attack);
		if (hasCharacter() && getCharacter() is TurretUnit)  {
			MinorType turretActivationType = (getCharacter() as TurretUnit).isOn ? MinorType.TurretOff : MinorType.TurretOn;
			if (canActivateTurret) tileActions.Add(new TileAction(null, null, new MinorType[]  {turretActivationType}, this));
			if (canActivateTurretAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, null, new MinorType[]  {turretActivationType}, this, moveTurretTile));
		}
		if (canAttack && addAttacks)  {
			tileActions.Add(new TileAction(null, new StandardType[]  {att}, null, this, null, (med ? u.healHitChance(getCharacter()) :u.attackHitChance(getCharacter()))));
			if (u.hasClassFeature(ClassFeature.Over_Clock) && mech) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.OverClock}, null, this, null, u.attackHitChance(getCharacter())));
		}
		if (canBackStep) tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, null, null, this, this));
		if (canMove ) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, null, null, this, this));
		if (canAttackAfterMove && addAttacks)  {
			tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {att}, null, this, moveAttackTile, (med ? u.healHitChance(getCharacter()) :u.attackHitChance(getCharacter()))));
			if (u.hasClassFeature(ClassFeature.Over_Clock) && mech) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {StandardType.OverClock}, null, this, moveAttackTile, u.attackHitChance(getCharacter())));
		}
		if (canAttackAfterBackStep && addAttacks)  {
			tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {att}, null, this, backstepAttackTile, (med ? u.healHitChance(getCharacter()) :u.attackHitChance(getCharacter()))));
			if (u.hasClassFeature(ClassFeature.Over_Clock) && mech) tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {StandardType.OverClock}, null, this, backstepAttackTile, u.attackHitChance(getCharacter())));
		}
		if (hasTrapTurret) {
			if (tr != null || tur != null) {
				bool canPutInv = false;
				foreach (InventorySlot sl in UnitGUI.inventorySlots) {
					InventoryItemSlot slot = u.characterSheet.characterSheet.inventory.inventory[sl - InventorySlot.Zero];
					if ((slot.item != null && u.characterSheet.characterSheet.inventory.itemCanStackWith(slot.item, (tr != null ? (Item)tr: (Item)tur))) || u.characterSheet.characterSheet.inventory.canInsertItemInSlot((tr != null?(Item)tr:(Item)tur), UnitGUI.getIndexOfSlot(sl))) {
						canPutInv = true;
						break;
					}
				}
				if (canPutInv) {
					if (canPickUpTrapTurret) tileActions.Add(new TileAction(null, new StandardType[]  {trapTurretType}, null, this, null));
					if (canPickUpTrapTurretAfterBackstep)tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {trapTurretType}, null, this, backstepTrapTurretTile));
					if (canPickUpTrapTurretAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {trapTurretType}, null, this, moveTrapTurretTile));
				}
			}
		}
		if (canLoot) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Loot}, this));
		if (canLootAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, null, new MinorType[]  {MinorType.Loot}, this, moveLootTile));
//		if (canLootAfterBackStep) tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, null, new MinorType[]  {MinorType.Loot}, this, backstepLootTile));
		if (canEscape) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Escape}, this, this));
		if (u.hasClassFeature(ClassFeature.Throw))  {
			if (canThrow) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.Throw}, null, this));
			if (canThrowAfterBackStep)tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {StandardType.Throw}, null, this, backstepAttackTile));
			if (canThrowAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {StandardType.Throw}, null, this, moveAttackTile));
		}
		if (u.hasClassFeature(ClassFeature.Intimidate) && enemy)  {
			if (canThrow) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.Intimidate}, null, this, null, u.intimidateHitChance(getCharacter())));
			if (canThrowAfterBackStep)tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {StandardType.Intimidate}, null, this, backstepAttackTile, u.intimidateHitChance(getCharacter())));
			if (canThrowAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {StandardType.Intimidate}, null, this, moveAttackTile, u.intimidateHitChance(getCharacter())));
		}
		if (u.hasClassFeature(ClassFeature.Instill_Paranoia) && enemy && !u.usedStandard)  {
			if (canOrator) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.InstillParanoia}, null, this));
			if (canOratorAfterBackStep)tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, new StandardType[]  {StandardType.InstillParanoia}, null, this, backstepOratorTile));
			if (canOratorAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, new StandardType[]  {StandardType.InstillParanoia}, null, this, moveOratorTile));
		}
		if (u.hasClassFeature(ClassFeature.Invoke) && enemy && u.invokeUsesLeft > 0)  {
			if (canOrator) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Invoke}, this, null, u.invokeHitChance(getCharacter())));
			if (canOratorAfterBackStep)tileActions.Add(new TileAction(new MovementType[]  {MovementType.BackStep}, null, new MinorType[]  {MinorType.Invoke}, this, backstepOratorTile, u.invokeHitChance(getCharacter())));
			if (canOratorAfterMove) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Move}, null, new MinorType[]  {MinorType.Invoke}, this, moveOratorTile, u.invokeHitChance(getCharacter())));
		}
		if (canMark) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Mark}, this));
		if (canStealth) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Stealth}, this));
	//	if (canOneOfMany) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.OneOfMany}, this));
	//	if (canTemperedHands) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.TemperedHands}, this));
		if (canRecover) tileActions.Add(new TileAction(new MovementType[]  {MovementType.Recover}, null, null, this));
		if (canExamine) tileActions.Add(new TileAction(null, null, new MinorType[]  {MinorType.Examine}, this));
//		if (canTurret) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.Place_Turret}, null, this));
//		if (canTrap) tileActions.Add(new TileAction(null, new StandardType[]  {StandardType.Lay_Trap}, null, this));
		if (canMoveBody) tileActions.Add(new TileAction(null, new StandardType[] {StandardType.MoveBody}, null, this));
		if (MapGenerator.gameMaster)  {
			if (hasCharacter())  {
				tileActions.Add(new TileAction(new GameMasterType[]  {GameMasterType.Damage1}, this));
				tileActions.Add(new TileAction(new GameMasterType[]  {GameMasterType.Heal1}, this));
				tileActions.Add(new TileAction(new GameMasterType[]  {GameMasterType.Heal10}, this));
			}
		}
		tileActions.Sort();
		return tileActions;
	}

	public Vector2 getPosition(int mod = 1)  {
		return new Vector2(x*mod, y*mod);
	}

	public List<Item> getItems()  {
	//	Debug.Log("getItems: " + items.Count);
		List<Item> it = new List<Item>(items);
		if (hasCharacter() && getCharacter().isDead())  {
			foreach (Item i in getCharacter().droppedItems)  {
				it.Add(i);
			}
		}
		return it;
	}

	public List<Item> getReachableItems()  {
		List<Item> i = new List<Item>();
		i = i.Concat(getItems()).ToList();
//		i.AddRange(items);
		foreach (Direction dir in directions)  {
			Tile t = getTile(dir);
			if (t==null) continue;
//			i.AddRange(t.getItems());
			i = i.Concat(t.getItems()).ToList();
		}
	//	Debug.Log("Total Items: " + i.Count);
		return i;
	}

	public bool removeItem(Item i, int dist)  {
		if (getItems().Contains(i))  {
			items.Remove(i);
			return true;
		}
		else  {
			if (hasCharacter() && getCharacter().isDead())  {
				if (getCharacter().droppedItems.Contains(i))  {
					getCharacter().droppedItems.Remove(i);
					return true;
				}
			}
		}
		if (dist <= 0) return false;
		foreach (Direction dir in directions)  {
			Tile t = getTile(dir);
			if (t==null) continue;
			if (t.removeItem(i,dist-1)) return true;
		}
		return false;
	}

	public void addItem(Item i)  {
		items.Add(i);
	}

	public int getInterestingNess(Unit u)  {
		if (hasEnemy(u) && !getEnemy(u).deadOrDyingOrUnconscious()) return 16;
		return 0;
	}

	public int getInterestingNess(Unit u, int distance)  {
		int interestingNess = getInterestingNess(u);
		while (distance > 1)  {
			distance--;
			interestingNess/=2;
		}
		return interestingNess;
	}

	public Tile()  {
		items = new List<Item>();
		for (int n=0;n<17;n++)  {
			if (UnityEngine.Random.Range(0, 3)==1)  {
				Item i;
				switch (n%17)  {
				case 0:
					i = new GearM1();
					break;
				case 1:
					i = new GearM2();
					break;
				case 2:
					i = new GearM3();
					break;
				case 3:
					i = new Knives();
					break;
				case 4:
					i = new BuzzSaws();
					break;
				case 5:
					i = new FrameM1();
					break;
				case 6:
					i = new FrameM2();
					break;
				case 7:
					i = new FrameM3();
					break;
				case 8:
					i = new TriggerM1();
					break;
				case 9:
					i = new TriggerM2();
					break;
				case 10:
					i = new TriggerM3();
					break;
				case 11:
					i = new EnergySourceM1();
					break;
				case 12:
					i = new EnergySourceM2();
					break;
				case 13:
					i = new EnergySourceM3();
					break;
				case 14:
					i = new Turret("", new TestFrame(), new TestApplicator(), new TestGear(), new TestEnergySource());
					break;
				case 15:
					i = new Trap("", new TestFrame(), new TestApplicator(), new TestGear(), new TestTrigger());
					break;
				default:
					i = new Turret("", new TestFrame(), new TestApplicator(), new TestGear(), new TestEnergySource());
					break;
				}
				addItem(i);
			}
		}
	//	player = null;
	//	enemy = null;
		character = null;
		standable = true;
		passableLeft = 1;
		passableRight = 1;
		passableUp = 1;
		passableDown = 1;
		trigger = 0;
		activation = 0;
		resetStandability();
	}

	public static Direction directionBetweenTiles(Vector2 fromTile, Vector2 toTile)  {
		if (fromTile.x < toTile.x) return Direction.Right;
		if (fromTile.x > toTile.x) return Direction.Left;
		if (fromTile.y > toTile.y) return Direction.Up;
		if (fromTile.y < toTile.y) return Direction.Down;
		return Direction.None;
	}

	public void resetStandability()  {
		canAttackCurr = false;
		canStandCurr = false;
		canUseSpecialCurr = false;
		attackFromTurret = false;
		minSpecialCurr = int.MaxValue;
		minDistCurr = int.MaxValue;
		minAttackCurr = int.MaxValue;
		minDistUsedMinors = 0;
	}

	public void doTrapDamage(Unit u)  {
		TrapUnit tr = getTrap();
		if (tr==null) return;
	//	Trap t = tr.trap;

	//	int damage = 
		tr.attackEnemy = u;
	//	u.setTarget();
//		tr.startAttacking();
		tr.attacking = true;
	}

	public void setTrap(TrapUnit tr)  {
		trap = tr;
	}

	public TrapUnit getTrap()  {
		return trap;
	}

	public bool hasTrap()  {
		return trap != null;
	}

	public void removeTrap()  {
		trap = null;
	}

	public void setCharacter(Unit cs)  {
		character = cs;
	}

	public void removeCharacter()  {
		character = null;
	}

	public bool hasAliveEnemy(Unit cs, bool blocking = false)  {
		return hasEnemy(cs, blocking) && !getCharacter().deadOrDyingOrUnconscious();
	}

	public bool hasEnemy(Unit cs, bool blocking = false)  {
		return hasCharacter() && (blocking ? character.isEnemyOf(cs) : cs.isEnemyOf(character));
//		return enemy != null;
	}

	public bool hasAliveAlly(Unit cs, bool blocking = false)  {
		return hasAlly(cs) && !getAlly(cs).deadOrDyingOrUnconscious();
	}

	public bool hasAlly(Unit cs, bool blocking = false)  {
		return hasCharacter() && (blocking ? character.isAllyOf(cs) : cs.isAllyOf(character));
	}

	public bool hasCharacter()  {
		return character != null;
	}

	public Unit getEnemy(Unit cs)  {
		if (hasEnemy(cs))  {
			return getCharacter();
		}
		return null;
	}

	public Unit getAlly(Unit cs)  {
		if (hasAlly(cs))  {
			return getCharacter();
		}
		return null;
	}

	public Unit getCharacter()  {
		return character;
	}

	public bool canStand()  {
		return standable && !character;
	}

	public Tile getTile(Direction dir)  {
		switch (dir)  {
		case Direction.Down:
			return downTile;
		case Direction.Left:
			return leftTile;
		case Direction.Right:
			return rightTile;
		case Direction.Up:
			return upTile;
		case Direction.None:
			return this;
		default:
			return null;
		}
	}
	public static Direction[] directions = new Direction[] {Direction.Down,Direction.Left,Direction.Right,Direction.Up};
	/*
	public bool shouldTakeAttOppLeaving(Unit u)  {
		foreach (Direction dir in directions)  {
			Tile t = getTile(dir);
			if (t != null && t.getEnemy(u)) return true;
		}
		return false;
	}
*/
	public bool canPass(Direction direction, Unit cs, Direction previousDirection)  {
	//	Debug.Log("Can Turn: " + canTurn);
		switch (direction)  {
		case Direction.Left:
			return this.leftTile!=null && !this.leftTile.hasAliveEnemy(cs, true) && this.passableLeft>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Right:
			return this.rightTile!=null && !this.rightTile.hasAliveEnemy(cs, true) && this.passableRight>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Down:
			return this.downTile!=null && !this.downTile.hasAliveEnemy(cs, true) && this.passableDown>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Up:
			return this.upTile!=null && !this.upTile.hasAliveEnemy(cs, true) && this.passableUp>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		default:
			return false;
		}
	}

	public bool isDifficultTerrain(Direction direction)  {
		switch (direction)  {
		case Direction.Left:
			return this.passableLeft > 1;
		case Direction.Right:
			return this.passableRight > 1;
		case Direction.Down:
			return this.passableDown > 1;
		case Direction.Up:
			return this.passableUp > 1;
		default:
			return false;
		}
	}


	public int passabilityInDirection(Direction direction)  {
		switch (direction)  {
		case Direction.Left:
			return this.passableLeft;
		case Direction.Right:
			return this.passableRight;
		case Direction.Down:
			return this.passableDown;
		case Direction.Up:
			return this.passableUp;
		default:
			return 0;
		}
	}

	public bool provokesOpportunity(Direction direction, Unit cs, Unit singleUnit = null)  {
//		switch (direction)  {
//		case Direction.Left:
//			return (this.
//		}
		List<Unit> units = new List<Unit>();
		if (singleUnit != null) units.Add(singleUnit);
		else units = new List<Unit>(cs.mapGenerator.priorityOrder);

		foreach (Unit u in units)  {
			if (u.isEnemyOf(cs) && (u.playerControlled || u.aiActive) && u.canAttOpp(cs) && u.hasLineOfSightToTile(this, cs, u.getAttackRange(), true, u.attackVisibilityMode()))  {
				Tile next = getTile(direction);
				if (!u.hasLineOfSightToTile(next, cs, u.getAttackRange(), true, u.attackVisibilityMode())) return true;
			}
		}
		return false;
	}

	public bool hasAliveEnemyDirection(Direction direction, Unit cs)  {
		switch (direction)  {
		case Direction.Left:
			return this.leftTile != null && this.leftTile.hasAliveEnemy(cs);
		case Direction.Right:
			return this.rightTile != null && this.rightTile.hasAliveEnemy(cs);
		case Direction.Up:
			return this.upTile != null && this.upTile.hasAliveEnemy(cs);
		case Direction.Down:
			return this.downTile != null && this.downTile.hasAliveEnemy(cs);
		default:
			return false;
		}
	}

	public bool hasEnemyDirection(Direction direction, Unit cs)  {
		switch (direction)  {
		case Direction.Left:
			return this.leftTile != null && this.leftTile.hasEnemy(cs);
		case Direction.Right:
			return this.rightTile != null && this.rightTile.hasEnemy(cs);
		case Direction.Up:
			return this.upTile != null && this.upTile.hasEnemy(cs);
		case Direction.Down:
			return this.downTile != null && this.downTile.hasEnemy(cs);
		default:
			return false;
		}
	}

	public bool isVisibleFrom(Direction dir)  {
	//	return false;
		switch (dir)  {
		case Direction.Up:
			return visibleUp != 0;
		case Direction.Down:
			return visibleDown != 0;
		case Direction.Left:
			return visibleLeft != 0;
		case  Direction.Right:
			return visibleRight != 0;
		default:
			return false;
		}
	}
	
	
	public void parseTile(string tile)  {
		string[] strs = tile.Split(new char[] {','});
		x = int.Parse(strs[0]);
		y = int.Parse(strs[1]);
		int curr = 6;
		passableUp = 1;
		passableDown = 1;
		passableLeft = 1;
		passableRight = 1;
		trigger = 0;
		activation = 0;
		visibleUp = 1;
		visibleRight = 1;
		visibleDown = 1;
		visibleLeft = 1;
		startingPoint = false;
		canTurn = true;
//		alpha = 0.4f;
	//	x = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
	//	y = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
	//	red = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
	//	green = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
	//	blue = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
	//	alpha = float.Parse(strs[curr])/255.0f;if (alpha==0) alpha = .4f;curr++;if (strs.Length<=curr) return;
	//	setSpriteColor();
		if (strs.Length<=curr) return;
		standable = int.Parse(strs[curr])==1;curr++;if (strs.Length<=curr) return;
		passableUp = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableRight = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableDown = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableLeft = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		trigger = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		activation = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		startingPoint = int.Parse(strs[curr])==1;curr++;if (strs.Length<=curr) return;
		visibleUp = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		visibleRight = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		visibleDown = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		visibleLeft = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		canTurn = int.Parse(strs[curr])==1;curr++;if (strs.Length<=curr) return;

		//		passable = int.Parse(strs[curr])==1;curr++;
	}

	
	
	public static int xForTile(string tile)  {
		string[] strs = tile.Split(new char[] {','});
		return int.Parse(strs[0]);
	}
	
	public static int yForTile(string tile)  {
		string[] strs = tile.Split(new char[] {','});
		return int.Parse(strs[1]);
	}

}
