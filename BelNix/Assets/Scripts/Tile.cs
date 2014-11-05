using UnityEngine;
using System.Collections;

public enum Direction {Up, Down, Right, Left};

public class Tile {

	GameObject player;
	GameObject enemy;
	bool standable;
	int passableLeft;
	int passableRight;
	int passableUp;
	int passableDown;
	public Tile leftTile;
	public Tile rightTile;
	public Tile upTile;
	public Tile downTile;
	int trigger;
	int activation;
	public int minDistCurr;
	public int minAttackCurr;
	public bool canAttackCurr;
	public bool canStandCurr;

	public Tile() {
		player = null;
		enemy = null;
		standable = true;
		passableLeft = 1;
		passableRight = 1;
		passableUp = 1;
		passableDown = 1;
		trigger = 0;
		activation = 0;
		resetStandability();
	}

	public void resetStandability() {
		canAttackCurr = false;
		canStandCurr = false;
		minDistCurr = int.MaxValue;
		minAttackCurr = int.MaxValue;
	}
	

	public void setEnemy(GameObject e) {
		enemy = e;
	}

	public void removeEnemy() {
		enemy = null;
	}

	public bool hasEnemy() {
		return enemy != null;
	}

	public Enemy getEnemy() {
		if (enemy == null) return null;
		return enemy.GetComponent<Enemy>();
	}
	public void setPlayer(GameObject p) {
		player = p;
	}

	public void removePlayer() {
		player = null;
	}

	public bool hasPlayer() {
		return player != null;
	}
	public Player getPlayer() {
		if (player == null) return null;
		return player.GetComponent<Player>();
	}

	public bool canStand() {
		return standable && !player;
	}

	public bool canPass(Direction direction) {
		switch (direction) {
		case Direction.Left:
			return this.leftTile!=null && !this.leftTile.hasEnemy() && this.passableLeft>0;
		case Direction.Right:
			return this.rightTile!=null && !this.rightTile.hasEnemy() && this.passableRight>0;
		case Direction.Down:
			return this.downTile!=null && !this.downTile.hasEnemy() && this.passableDown>0;
		case Direction.Up:
			return this.upTile!=null && !this.upTile.hasEnemy() && this.passableUp>0;
		default:
			return false;
		}
	}
}
