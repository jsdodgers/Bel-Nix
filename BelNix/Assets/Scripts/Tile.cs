using UnityEngine;
using System.Collections;

public enum Direction {Up, Down, Right, Left};

public class Tile {

//	GameObject player;
//	GameObject enemy;
	CharacterScript character;
	public bool standable;
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

	public void resetStandability() {
		canAttackCurr = false;
		canStandCurr = false;
		minDistCurr = int.MaxValue;
		minAttackCurr = int.MaxValue;
	}
	

	public void setCharacter(CharacterScript cs) {
		character = cs;
	}

	public void removeCharacter() {
		character = null;
	}

	public bool hasEnemy(CharacterScript cs) {
		return hasCharacter() && cs.isEnemyOf(character);
//		return enemy != null;
	}

	public bool hasAlly(CharacterScript cs) {
		return hasCharacter() && cs.isAllyOf(character);
	}

	public bool hasCharacter() {
		return character != null;
	}

	public CharacterScript getEnemy(CharacterScript cs) {
		if (hasEnemy(cs)) {
			return getCharacter();
		}
		return null;
	}

	public CharacterScript getAlly(CharacterScript cs) {
		if (hasAlly(cs)) {
			return getCharacter();
		}
		return null;
	}

	public CharacterScript getCharacter() {
		if (character == null) return null;
		return character;
	}

	public bool canStand() {
		return standable && !character;
	}

	public bool canPass(Direction direction, CharacterScript cs) {
		switch (direction) {
		case Direction.Left:
			return this.leftTile!=null && !this.leftTile.hasEnemy(cs) && this.passableLeft>0;
		case Direction.Right:
			return this.rightTile!=null && !this.rightTile.hasEnemy(cs) && this.passableRight>0;
		case Direction.Down:
			return this.downTile!=null && !this.downTile.hasEnemy(cs) && this.passableDown>0;
		case Direction.Up:
			return this.upTile!=null && !this.upTile.hasEnemy(cs) && this.passableUp>0;
		default:
			return false;
		}
	}

	public bool isDifficultTerrain(Direction direction) {
		switch (direction) {
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


	public int passabilityInDirection(Direction direction) {
		switch (direction) {
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

	public bool provokesOpportunity(Direction direction, CharacterScript cs) {
//		switch (direction) {
//		case Direction.Left:
//			return (this.
//		}
		bool provokesOpportunity = false;
		if (direction != Direction.Left) provokesOpportunity |= hasEnemyDirection(Direction.Left, cs);
		if (direction != Direction.Right) provokesOpportunity |= hasEnemyDirection(Direction.Right, cs);
		if (direction != Direction.Up) provokesOpportunity |= hasEnemyDirection(Direction.Up, cs);
		if (direction != Direction.Down) provokesOpportunity |= hasEnemyDirection(Direction.Down, cs);
		return provokesOpportunity;
	}

	public bool hasEnemyDirection(Direction direction, CharacterScript cs) {
		switch (direction) {
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
