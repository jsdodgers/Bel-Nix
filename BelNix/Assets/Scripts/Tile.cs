using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Direction {Up, Down, Right, Left, None};

public class Tile {

//	GameObject player;
//	GameObject enemy;
	Unit character;
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
	public int minDistUsedMinors;
	public bool canAttackCurr;
	public bool canStandCurr;
	public bool canTurn;
	public bool startingPoint;
	public int x;
	public int y;
	List<Item> items;

	public Vector2 getPosition() {
		return new Vector2(x, y);
	}

	public List<Item> getItems() {
	//	Debug.Log("getItems: " + items.Count);
		return items;
	}

	public List<Item> getReachableItems() {
		List<Item> i = new List<Item>();
		i = i.Concat(getItems()).ToList();
//		i.AddRange(items);
		foreach (Direction dir in directions) {
			Tile t = getTile(dir);
			if (t==null) continue;
//			i.AddRange(t.getItems());
			i = i.Concat(t.getItems()).ToList();
		}
	//	Debug.Log("Total Items: " + i.Count);
		return i;
	}

	public bool removeItem(Item i, int dist) {
		if (getItems().Contains(i)) {
			items.Remove(i);
			return true;
		}
		if (dist <= 0) return false;
		foreach (Direction dir in directions) {
			Tile t = getTile(dir);
			if (t==null) continue;
			if (t.removeItem(i,dist-1)) return true;
		}
		return false;
	}

	public void addItem(Item i) {
		items.Add(i);
	}

	public int getInterestingNess(Unit u) {
		if (hasEnemy(u) && !getEnemy(u).deadOrDyingOrUnconscious()) return 16;
		return 0;
	}

	public int getInterestingNess(Unit u, int distance) {
		int interestingNess = getInterestingNess(u);
		while (distance > 1) {
			distance--;
			interestingNess/=2;
		}
		return interestingNess;
	}

	public Tile() {
		items = new List<Item>();
		for (int n=0;n<8;n++) {
			if (Random.Range(0, 3)==1) {
				Item i;
				switch (n%4) {
				case 0:
					i = new TestGear();
					break;
				case 1:
					i = new TestApplicator();
					break;
				case 2:
					i = new TestFrame();
					break;
				default:
					i = new TestEnergySource();
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

	public static Direction directionBetweenTiles(Vector2 fromTile, Vector2 toTile) {
		if (fromTile.x < toTile.x) return Direction.Right;
		if (fromTile.x > toTile.x) return Direction.Left;
		if (fromTile.y > toTile.y) return Direction.Up;
		if (fromTile.y < toTile.y) return Direction.Down;
		return Direction.None;
	}

	public void resetStandability() {
		canAttackCurr = false;
		canStandCurr = false;
		minDistCurr = int.MaxValue;
		minAttackCurr = int.MaxValue;
		minDistUsedMinors = 0;
	}
	

	public void setCharacter(Unit cs) {
		character = cs;
	}

	public void removeCharacter() {
		character = null;
	}

	public bool hasEnemy(Unit cs) {
		return hasCharacter() && cs.isEnemyOf(character);
//		return enemy != null;
	}

	public bool hasAlly(Unit cs) {
		return hasCharacter() && cs.isAllyOf(character);
	}

	public bool hasCharacter() {
		return character != null;
	}

	public Unit getEnemy(Unit cs) {
		if (hasEnemy(cs)) {
			return getCharacter();
		}
		return null;
	}

	public Unit getAlly(Unit cs) {
		if (hasAlly(cs)) {
			return getCharacter();
		}
		return null;
	}

	public Unit getCharacter() {
		if (character == null) return null;
		return character;
	}

	public bool canStand() {
		return standable && !character;
	}

	public Tile getTile(Direction dir) {
		switch (dir) {
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
	public static Direction[] directions = new Direction[]{Direction.Down,Direction.Left,Direction.Right,Direction.Up};

	public bool shouldTakeAttOppLeaving(Unit u) {
		foreach (Direction dir in directions) {
			Tile t = getTile(dir);
			if (t != null && t.getEnemy(u)) return true;
		}
		return false;
	}

	public bool canPass(Direction direction, Unit cs, Direction previousDirection) {
	//	Debug.Log("Can Turn: " + canTurn);
		switch (direction) {
		case Direction.Left:
			return this.leftTile!=null && !this.leftTile.hasEnemy(cs) && this.passableLeft>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Right:
			return this.rightTile!=null && !this.rightTile.hasEnemy(cs) && this.passableRight>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Down:
			return this.downTile!=null && !this.downTile.hasEnemy(cs) && this.passableDown>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
		case Direction.Up:
			return this.upTile!=null && !this.upTile.hasEnemy(cs) && this.passableUp>0 && (this.canTurn || previousDirection == direction || previousDirection == Direction.None);
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

	public bool provokesOpportunity(Direction direction, Unit cs) {
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

	public bool hasEnemyDirection(Direction direction, Unit cs) {
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

	
	
	public static int xForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[0]);
	}
	
	public static int yForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[1]);
	}

}
