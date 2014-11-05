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
		return standable && !player && !enemy;
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




	
	
	
	public void parseTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		int curr = 6;
		passableUp = 0;
		passableDown = 0;
		passableLeft = 0;
		passableRight = 0;
		trigger = 0;
		activation = 0;
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
		//		passable = int.Parse(strs[curr])==1;curr++;
	}

	
	
	public static int xForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[0]);
	}
	
	public static int yForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[1]);
	}

}
