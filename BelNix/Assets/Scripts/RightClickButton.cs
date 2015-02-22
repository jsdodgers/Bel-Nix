using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RightClickButton : MonoBehaviour {

	public TileAction action;
	MapGenerator mg;
	List<Vector2> playerPath = null;
	int maxPath = 0;
	Tile keysTile;
	public bool mouseOver;

	// Use this for initialization
	void Start () {
		mg = MapGenerator.mg;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void mouseEnter() {
		mouseOver = true;
		if (action.hasMovement()) {
		//	playerPath = mg.lastPlayerPath;
			maxPath = mg.getCurrentUnit().currentMaxPath;
		//	mg.getCurrentUnit().resetPath();
			mg.resetPlayerPath();
			mg.getCurrentUnit().addPathTo(new Vector2(action.movementTile.x, action.movementTile.y), action.getMovementLength());
			mg.setPlayerPath(mg.getCurrentUnit().currentPath);
			mg.getCurrentUnit().setPathCount();
		}
	}

	public void mouseExit() {
		mouseOver = false;
		if (action.hasMovement()) {
		//	mg.lastPlayerPath = playerPath;
			mg.getCurrentUnit().currentMaxPath = maxPath;
			mg.getCurrentUnit().currentPath = mg.lastPlayerPath;
			mg.getCurrentUnit().setPathCount();
			mg.resetPlayerPath();
			mg.setPlayerPath(mg.lastPlayerPath);
		}
	}

	public void click() {
		Unit u = mg.getCurrentUnit();
		if (action.hasMovement()) {
			mg.lastPlayerPath = mg.getCurrentUnit().currentPath;
			mg.currentKeysTile = action.movementTile;
		}
		if (action.movementTypes.Contains(MovementType.Move)) {
			u.unitMovement = UnitMovement.Move;
			u.currentMoveDist = u.moveDistLeft;
			u.startMoving(false);
		}
		if (action.movementTypes.Contains(MovementType.BackStep)) {
			u.unitMovement = UnitMovement.BackStep;
			u.currentMoveDist = 1;
			u.startMoving(true);
		}
		if (action.minorTypes.Contains(MinorType.Escape)) {
			u.unitMovement = UnitMovement.Escape;
			u.currentMoveDist = 2;
			u.startMoving(true);
		}
		if (action.standardTypes.Contains(StandardType.Attack)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startAttacking();
		}
		if (action.standardTypes.Contains(StandardType.OverClock)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startAttacking(true);
		}
		if (action.standardTypes.Contains(StandardType.Throw)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startThrowing();
		}
		if (action.standardTypes.Contains(StandardType.Intimidate)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startIntimidating();
		}
		if (action.standardTypes.Contains(StandardType.InstillParanoia)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startInstillingParanoia();
		}
		if (action.minorTypes.Contains(MinorType.Invoke)) {
			u.attackEnemy = action.actualTile.getCharacter();
			u.startInvoking();
		}
		if (action.minorTypes.Contains(MinorType.Stealth)) {
			u.rollStealth();
		}
		if (action.movementTypes.Contains(MovementType.Recover)) {
			u.recover();
		}
	/*	if (action.minorTypes.Contains(MinorType.OneOfMany)) {

		}*/
		RightClickMenu.hideMenu(true);
	}
}
