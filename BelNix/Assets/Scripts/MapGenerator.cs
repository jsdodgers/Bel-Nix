using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
public enum GameState {Playing, Won, Lost, None}
public enum VisibilityMode {Visibility, Melee, Ranged, None}

public class MapGenerator : MonoBehaviour {
	public static MapGenerator mg;
	Vector3 cameraPos;
	public GameObject overlayMeshPrefab;
	public GameObject[] overlayImage;
	public float smoothness = 720.0f;
	public bool doOverlay = false;
	public bool withLineOfSight = true;
	public bool testAnimations = false;
	public float timeScale = 0.05f;
	public GameObject overlayObject;
	Texture2D[,] mapOverlays;
//	Texture2D mapOverlay;
	public List<Unit> selectionUnits;
	public List<Unit> outOfGameUnits;
	public List<Vector2> itemPositions = new List<Vector2>();
	public List<EditorItem> items = new List<EditorItem>();
	public const int sortingOrderExtra = 1530*0;
	public const int gridOrder = 2 + sortingOrderExtra;
	public const int lineOrder = 3 + sortingOrderExtra;
	public const int trapOrder = 20 + sortingOrderExtra;
	public const int circleNormalOrder = 30 + sortingOrderExtra;
	public const int circleMovingOrder = 31 + sortingOrderExtra;
	public const int trailOrder = 40 + sortingOrderExtra;
	public const int arrowOrder = 60 + sortingOrderExtra;
	public const int warningOrder = 70 + sortingOrderExtra;
	public const int playerNormalOrder = 300 + sortingOrderExtra;
	public const int playerArmorOrder = 310 + sortingOrderExtra;
	public const int playerMovingOrder = 400 + sortingOrderExtra;
	public const int playerMovingArmorOrder = 410 + sortingOrderExtra;
	public const int mapOverlayOrder = 950 + sortingOrderExtra;
	public const int markOrder = 960 + sortingOrderExtra;
	public const int playerSelectOrder = 1000 + sortingOrderExtra;
	public const int playerSelectPlayerOrder = 1200 + sortingOrderExtra;
	public const int playerSelectPlayerArmorOrder = 1210 + sortingOrderExtra;
	public const int playerSelectSelectedPlayerOrder = 1300 + sortingOrderExtra;
	public const int playerSelectSelectedPlayerArmorOrder = 1310 + sortingOrderExtra;
	public const int mouseOverOrder = 10000 + sortingOrderExtra;

	public float viewRadius;
	public Unit mainUnit;
	public GameState gameState = GameState.Playing;
    public int experienceReward;
    public int copperReward;
	public string tileMapName;
	public int gridSize = 64;

	public AudioBank audioBank;
	GameObject lastHit;
	GameObject map;
	Transform mapTransform;
	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	float tapTime = 0.0f;
	float renderThing = 100.0f;
	GameObject targetObject;

/*	public GameObject upWallPrefab;
	public GameObject downWallPrefab;
	public GameObject leftWallPrefab;
	public GameObject rightWallPrefab;*/
	public GameObject anyWallPrefab;

	bool isOnGUI;
	public Transform playerTransform;
	public Transform enemyTransform;
//	public GameObject turretPrefab;
	GameObject arrowStraightPrefab;
	GameObject arrowCurvePrefab;
	GameObject arrowPointPrefab;
	GameObject playerPrefab;
	GameObject turretPrefab;
	public GameObject trapPrefab;
	GameObject enemyPrefab;
	GameObject warningRedPrefab;
	GameObject warningYellowPrefab;
	GameObject warningBothPrefab;
//	public GameObject selectedPlayer;
	public Unit selectedUnit;
	public List<Unit> selectedUnits;
	public List<TrapUnit> currentTrap;
	public TrapUnit currentlySelectedTrap;
	Unit hoveredCharacter;
	public List<Unit> players;
	public List<Unit> deadUnits;
	public List<Unit> nonAlertEnemies;
	public List<Unit> enemies;
	public GameObject turrets;
	public GameObject traps;
	GameObject grids;
	GameObject lines;
	GameObject path;
	GameObject enemiesObj;
	GameObject[,] gridArray;
	public bool[,] canSee;
	public Tile[,] tiles;
	public List<Vector2> lastPlayerPath;
	GameObject mouseOver;
	Vector2 startSquare;
	Vector2 lastPosDrag;
	Vector3 startSquareActual;
	int oldTouchCount;
//	bool editingPath = false;
	Vector2 lastArrowPos = new Vector2(-1, -1);
	
	GameObject currentGrid;
	GameObject mouseDownGrid;
	SpriteRenderer currentSprite;
	Color currentSpriteColor;
	
	GameObject gridPrefab;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;
	bool mouseDownGUI = false;
	bool mouseDown;
	bool mouseDownRight;
	bool mouseUp;
	
	public bool shiftDown;
	bool altDown;
	bool controlDown;
	bool spaceDown;
	bool escapeDown;
	bool commandDown;

	bool shiftDraggin = false;
	bool shiftRightDraggin = false;
	bool middleDraggin = false;
	bool normalDraggin = false;
	bool rightDraggin = false;
	bool scrolled = false;

	bool wasRightDraggin = false;
	bool rightDragginCancelled = false;
	bool wasShiftRightDraggin = false;
	bool shiftRightDragginCancelled = false;

	float lastClickTime = 0.0f;
	Tile lastClickTile = null;
	float doubleClickTime = 0.5f;
	

	Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 cameraMoveToPos;
	public bool movingCamera;
	
	public int actualWidth = 0;
	public int actualHeight = 0;
	float timeSinceSpace = 500.0f;
	int screenWidth = 0;
	int screenHeight = 0;

	public List<Unit> priorityOrder;

	public int currentUnit;

	public Tile currentUnitTile;
	public Tile currentKeysTile;
	public int currentKeysSize;

	public AudioManager aManager;

	public void resetCanSee(bool[,] old, bool resetTo = false) {
		for (int n=0;n<actualWidth * gridSize;n++) {
			for (int m=0;m<actualHeight * gridSize; m++) {
				if (old!=null)old[n,m] = canSee[n,m];
				canSee[n,m] = resetTo;
			}
		}
	}
	public bool hasChange(int x, int y, bool[,] old) {
		for (int n=x*gridSize;n<(x+1)*gridSize;n++) {
			for (int m=y*gridSize;m<(y+1)*gridSize;m++) {
				if (canSee[n,m] != old[n,m]) return true;
			}
		}
		return false;
	}
	public class MeshPos {
		internal Vector2 position;
		internal MeshGen meshGen;
		public MeshPos(Vector2 pos, MeshGen m) {
			meshGen = m;
			position = pos;
		}
	}
	bool first = true;
	Color blockedColor = Color.black;//Color.black;
	Color clearColor = Color.clear;//Color.clear;
	public void setOverlay() {
	//	blockedColor.a = .5f;
	//	clearColor.a = .5f;
		foreach (GameObject overlayI in overlayImage) {
			overlayI.SetActive(doOverlay);
		}
		if (!doOverlay) return;
//		bool[,] canSeeOld = new bool[actualWidth*gridSize,actualHeight*gridSize];
//		resetCanSee(canSeeOld);
//		List<Vector2> positions = new List<Vector2>();
		List<MeshPos> meshPoses = new List<MeshPos>();
		if (isInCharacterPlacement()) {
			foreach (Tile t in tiles) {
				if (t.startingPoint) {
					meshPoses.Add(getMeshPos(t));
				}
			}
		}
		else {
			foreach (Unit u in players) {
			//	positions.Add(new Vector2(u.transform.position.x, u.transform.position.y));
				meshPoses.Add(getMeshPos(u));
			}
			foreach (Tile t in tiles) {
				MeshGen mg = t.meshGen;
				if (mg != null) {
					GameObject.Destroy(mg.gameObject);
					t.meshGen = null;
				}
			}
		}
		foreach (MeshPos pos in meshPoses) {
			setOverlay(pos);
		/*	int printed = 0;
			int x = (int)(pos.x * (float)gridSize);
			int y = (int)(-pos.y * (float)gridSize);
			Vector2 originalPosition = new Vector2(pos.x * gridSize, pos.y * gridSize);
			Vector2 originalPositionPos = new Vector2(originalPosition.x, gridSize * actualHeight + originalPosition.y - 1);
			int dist = (int)((viewRadius) * (float)gridSize);

			for (int n= Mathf.Max(0, x - dist);n<Mathf.Min(actualWidth*gridSize,x + dist);n++) {
			int a = (int)Mathf.Sqrt((float)(dist * dist - (x - n) * (x - n)));
				int m = y + a;
				CollisionPoint cp = getLineOfSightCollisionPoint(originalPosition, new Vector2(n,-m));
				if (!cp.collides) {
					setVisibilityLine(originalPositionPos,new Vector2(n,actualHeight*gridSize - m - 1),canSee);
				}
				else {
					setVisibilityLine(originalPositionPos, new Vector2((int)(cp.point.x*gridSize),actualHeight*gridSize + (int)(cp.point.y*gridSize) - 1), canSee);
				}
				m = y - a;

				cp = getLineOfSightCollisionPoint(originalPosition, new Vector2(n,-m));
				if (!cp.collides) {
					setVisibilityLine(originalPositionPos,new Vector2(n,actualHeight*gridSize - m - 1),canSee);
				}
				else {
					setVisibilityLine(originalPositionPos, new Vector2((int)(cp.point.x*gridSize),actualHeight*gridSize + (int)(cp.point.y*gridSize) - 1), canSee);
				}
			}
			for (int m = Mathf.Max(0,y-dist);m<Mathf.Min(actualHeight*gridSize,y+dist);m++) {
				int a = (int)Mathf.Sqrt((float)(dist * dist - (y - m) * (y - m)));
				int n = x + a;


				CollisionPoint cp = getLineOfSightCollisionPoint(originalPosition, new Vector2(n,-m));
				if (!cp.collides) {
					setVisibilityLine(originalPositionPos,new Vector2(n,actualHeight*gridSize - m - 1),canSee);
				}
				else {
					setVisibilityLine(originalPositionPos, new Vector2((int)(cp.point.x*gridSize),actualHeight*gridSize + (int)(cp.point.y*gridSize) - 1), canSee);
				}
				n = x - a;
				cp = getLineOfSightCollisionPoint(originalPosition, new Vector2(n,-m));
				if (!cp.collides) {
					setVisibilityLine(originalPositionPos,new Vector2(n,actualHeight*gridSize - m - 1),canSee);
				}
				else {
					setVisibilityLine(originalPositionPos, new Vector2((int)(cp.point.x*gridSize),actualHeight*gridSize + (int)(cp.point.y*gridSize) - 1), canSee);
				}
			}*/
		}


/*		string s = "";
		for (int n=0;n<actualWidth * gridSize;n+=gridSize) {
			for (int m=0;m<actualHeight*gridSize;m+=gridSize) {
				s += canSee[n,m] + "  ";
			}
			s += "\n\n";
		}
		Debug.Log(s);
		Debug.Log(canSee);*/
	//	Color[] pixels = new Color[actualWidth * actualHeight * gridSize * gridSize];//mapOverlay.GetPixels();
	/*	for (int n=0;n<actualWidth*gridSize;n++) {
			Color[] pixels = new Color[actualHeight * gridSize];
			for (int m=0;m<actualHeight*gridSize;m++) {
				Color c = (canSee[n,m]?clearColor:blockedColor);
//				if (pixels[n + m*actualWidth]!=c) {
			//	c = Color.black;
					pixels[m] = c;
//					mapOverlay.SetPixel(n, m, c);
//				}
			}
			mapOverlay.SetPixels(n, 0, 1, actualHeight * gridSize, pixels);
		}
		mapOverlay.Apply();*/
		return;
/*		for (int n=0;n<actualWidth;n++) {
			for (int m=0;m<actualHeight;m++) {
				if (!first && !hasChange(n,m,canSeeOld)) continue;
				Color[] pixels = new Color[gridSize*gridSize];
				for (int o=0;o<gridSize*gridSize;o++) {
//					pixels[o] = (canSee[n*gridSize+o%gridSize,m*gridSize+o/gridSize]?clearColor:blockedColor);
					if (canSee[n*gridSize+o%gridSize,m*gridSize+o/gridSize]!=canSeeOld[n*gridSize+o%gridSize,m*gridSize+o/gridSize])
						mapOverlays[n,m].SetPixel(o%gridSize,o/gridSize,(canSee[n*gridSize+o%gridSize,m*gridSize+o/gridSize]?clearColor:blockedColor));
				}
//				mapOverlays[n,m].SetPixels(pixels);
				mapOverlays[n,m].Apply();
			}
		}
		first = false;*/
	}
	public MeshPos getMeshPos(Unit u) {
		MeshGen mg = u.meshGen;
		if (mg == null) {
			GameObject mgObj = GameObject.Instantiate(overlayMeshPrefab) as GameObject;
			mg = mgObj.GetComponent<MeshGen>();
			u.meshGen = mg;
			mgObj.transform.parent = overlayObject.transform;
		}
		return new MeshPos(new Vector2(u.transform.position.x, u.transform.position.y), mg);
	}
	public MeshPos getMeshPos(Tile t) {
		MeshGen mg = t.meshGen;
		if (mg == null) {
			GameObject mgObj = GameObject.Instantiate(overlayMeshPrefab) as GameObject;
			mg = mgObj.GetComponent<MeshGen>();
			t.meshGen = mg;
			mgObj.transform.parent = overlayObject.transform;
		}
		return new MeshPos(new Vector2(t.getPosition().x + .5f, -t.getPosition().y - .5f), mg);
		//		break;
	}
	public void setOverlay(Unit u, bool print = false) {
		setOverlay(getMeshPos(u), print);
	}
	public void removeOverlay(Unit u) {
		if (u.meshGen != null) {
			GameObject.Destroy(u.meshGen.gameObject);
			u.meshGen = null;
		}
	}
	public void setOverlay(MeshPos pos, bool print = false) {
		List<Vector2> points = new List<Vector2>();
		for (float n=2.0f*Mathf.PI;n>0;n-=(2*Mathf.PI)/smoothness) {
			float sin = Mathf.Sin(n);
			float cos = Mathf.Cos(n);
			float y = pos.position.y + sin * viewRadius;
			float x = pos.position.x + cos * viewRadius;
			Vector2 v = new Vector2(x, y);
			CollisionPoint cp = getLineOfSightCollisionPoint(pos.position, v, VisibilityMode.Visibility);
			if (cp.collides) {
				points.Add(cp.point);
			} 
			else {
				points.Add(v);
			}
		}
		pos.meshGen.createMesh(points.ToArray(), pos.position, print);
	}
	bool done = false;
	public void setVisibilityLine(Vector2 from, Vector2 to, bool[,] visibilities) {
		float slope = (to.x - from.x == 0.0f ? 10000.0f : Mathf.Abs(to.y - from.y)/Mathf.Abs(to.x-from.x));
		Vector2 fr = new Vector2();
		Vector2 t = new Vector2();
		bool switched = false;
	//	if (slope <= 1) {
			if (from.x <= to.x) {
				fr.x = from.x;
				fr.y = from.y;
				t.x = to.x;
				t.y = to.y;
			}
			else {
				fr.x = to.x;
				fr.y = to.y;
				t.x = from.x;
				t.y = from.y;
			}
/*		}
		else {
			switched = true;
			if (from.y <= to.y) {
				fr.x = from.y;
				fr.y = from.x;
				t.x = to.y;
				t.y = to.x;
			}
			else {
				fr.x = to.y;
				fr.y = to.x;
				t.x = from.y;
				t.y = from.x;
			}
		}*/
		int s = (t.y > fr.y ? 1 : -1);
		if (t.x - fr.x == 0.0f) return;
		float deltaerr = (t.x - fr.x == 0.0f ? Mathf.Abs(t.y - fr.y) : Mathf.Abs((t.y - fr.y)/(t.x - fr.x)));
		float err = 0;
		int y = (int)fr.y;
		for (int x=(int)fr.x;x<(int)t.x;x++) {

		//	visibilities[(switched?y:x),(switched?x:y)] = true;
			err += deltaerr;
			if (!setVis(x,y,switched,visibilities) && !done) {
				Debug.Log("aaa: " + x + " " + y + " " + switched + " " + from + "  " + to + "   " + deltaerr + "   " + err);
				done = true;
			}
			while (err >= 0.5f) {
				y+=s;
				err-=1;
				if (!setVis(x,y,switched,visibilities) && !done) {
					done = true;
					Debug.Log("bbb: " + x + " " + y + " " + switched + " " + from + "  " + to + "   " + deltaerr + "   " + err + "   " + s);
				}
//				visibilities[(switched?y:x),(switched?x:y)] = true;
			}
		}
	}
	public bool setVis(int x, int y, bool switched, bool[,] vis) {
		int xA = switched?y:x;
		int yA = switched?x:y;
		if (xA-1 < 0 || yA-1 < 0 || xA+1 >= gridSize*actualWidth || yA+1 >= gridSize*actualHeight) return false;
		vis[xA,yA] = true;
		vis[xA+1,yA] = true;
		vis[xA-1,yA] = true;
		vis[xA,yA-1] = true;
		vis[xA,yA+1] = true;
		return true;
	}
	bool logged = false;
/*	public bool hasLineOfSight(Tile from, Tile to) {
		int fromX = from.x;
		int fromY = from.y;
		float x = (float)from.x + 0.5f;
		float y = (float)from.y + 0.5f;
		Direction lrDir = Direction.Left;
		if (x < to.x) lrDir = Direction.Right;
		Direction udDir = Direction.Up;
		if (y < to.y) udDir = Direction.Down;
		float slope = (to.y - from.y)/(to.x - from.x);
		do {

			float newX = x + (lrDir == Direction.Left ? -1 : 1);
			float newY = y + slope * (newX - x);
			while ((udDir==Direction.Down ? newY > (t.y + 1) * gridSize : newY < t.y * gridSize)) {
				if (!logged) Debug.Log(!t.isVisibleFrom(udDir));
				if (!t.isVisibleFrom(udDir)) return false;
				t = t.getTile(udDir);
				if (t==null) return false;
			}
			if (!t.isVisibleFrom(lrDir)) {
				return false;
			}
			t = t.getTile(lrDir);
			if (t==null) return false;
			x = newX;
			y = newY;
		} while ((lrDir == Direction.Left ? x > to.x + .5f : x < to.x + .5f));
	}*/

	public bool isWithinDistance(float distance, Vector2 from, Vector2 to, bool manhattan) {
		if (manhattan) return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) <= distance;
		else return (from.x - to.x) * (from.x - to.x) + (from.y - to.y) * (from.y - to.y) <= distance * distance;
	}

	public bool hasLineOfSight(Unit fromUnit, Unit toUnit, int distance = -1, bool manhattan = false, VisibilityMode visMode = VisibilityMode.Visibility) {
		if (toUnit == null) return false;
		Tile from = tiles[(int)fromUnit.position.x, (int)-fromUnit.position.y];
		Tile to = tiles[(int)toUnit.position.x, (int)-toUnit.position.y];
		float dist = (distance == -1 ? fromUnit.getViewRadiusToUnit(toUnit) : distance);
		return hasLineOfSight(from, to, dist, manhattan, visMode);
	}

	public bool hasLineOfSight(Unit fromUnit, Vector2 to, int distance = -1, bool manhattan = false, VisibilityMode visMode = VisibilityMode.Visibility) {
		if (fromUnit == null) return false;
	//	Tile from = tiles[(int)fromUnit.position.x, (int)-fromUnit.position.y];
	//	Tile to = tiles[(int)toUnit.position.x, (int)-toUnit.position.y];
		Vector2 fromVec = new Vector2((int)((fromUnit.position.x + 0.5f)*gridSize), (int)((fromUnit.position.y - 0.5f)*gridSize));
		float dist = (distance == -1 ? fromUnit.getViewRadius() : distance);
		return isWithinDistance(dist*gridSize, fromVec, to, manhattan) && hasLineOfSight(fromVec, to, visMode);
	}

	public bool hasLineOfSight(Tile from, Tile to, float dist, bool manhattan = false, VisibilityMode visMode = VisibilityMode.Visibility) {
		Vector2 fromVec = new Vector2((int)((from.x + 0.5f)*gridSize), -(int)((from.y + 0.5f)*gridSize));
		Vector2 toCenter = new Vector2((int)((to.x + 0.5f)*gridSize), -(int)((to.y + 0.5f)*gridSize));
		if (isWithinDistance(dist*gridSize, fromVec, toCenter, manhattan) && hasLineOfSight(fromVec, toCenter, visMode)) return true;
		if (!manhattan) {
			for (int n=-1;n<=1;n+=2) {
				for (int m=-1;m<=1;m+=2) {
					Vector2 next = new Vector2(toCenter.x + ((gridSize/2.0f) - 1)*n, toCenter.y + ((gridSize/2.0f) - 1) * m);
					if (isWithinDistance(dist*gridSize, fromVec, next, manhattan) && hasLineOfSight(fromVec, next, visMode)) return true;
				}
			}
		}
		return false;
	}

	public bool hasLineOfSight(Vector2 from, Vector2 to, VisibilityMode visMode = VisibilityMode.Visibility) {
		int layerMask = 0;
		Direction lrDir = Direction.Left;
		if (from.x < to.x) lrDir = Direction.Right;
		Direction udDir = Direction.Up;
		if (from.y > to.y) udDir = Direction.Down;
		if (lrDir == Direction.Left) layerMask += 1 << 17;
		else if (lrDir == Direction.Right) layerMask += 1 << 18;
		if (udDir == Direction.Up) layerMask += 1 << 15;
		else if (udDir == Direction.Down) layerMask += 1 << 16;
		layerMask = 1 << 19;
		RaycastHit hitInfo;
		RaycastHit2D[] hits = Physics2D.LinecastAll(from/gridSize, to/gridSize, layerMask);
//		bool ret = !Physics.Linecast(new Vector3(from.x / gridSize, from.y/gridSize, 0.0f), new Vector3(to.x / gridSize, to.y / gridSize, 0.0f), out hitInfo, layerMask);
		if (hits != null) {
			if (to.x == 17 * 64) Debug.Log("Hits: " + hits.Count() + "  " + from + " "  + to);
			foreach (RaycastHit2D hit in hits) {
				GameObject go = hit.collider.gameObject;
				Wall w = go.GetComponent<Wall>();
				if (w != null) {
					if (visMode == VisibilityMode.Visibility && w.visibility != 0) continue;
					if (visMode == VisibilityMode.Ranged && w.canRange) continue;
					if (visMode == VisibilityMode.Melee && w.canMelee) continue;
				}
				float angleDiff = go.transform.eulerAngles.z - getAngle(from, to);
				while (angleDiff < 0) angleDiff += 360.0f;
				while (angleDiff > 360) angleDiff -= 360.0f;
				if (go.GetComponent<Wall>().bothWays || (angleDiff > 180.0f && angleDiff < 360.0f)) {
					return false;
				}
			}
		}
		return true;
	}

    public bool hasLineOfSight(Unit enemy)
    {
        if (isInCharacterPlacement())
        {
            foreach (Tile t in tiles)
            {
                if (t.startingPoint)
                {
                    if (hasLineOfSight(t, tiles[(int)enemy.position.x, -(int)enemy.position.y], viewRadius))
                        return true;
                }
            }
        }
        else
        {
            foreach (Unit player in players)
            {
                if (player.hasLineOfSightToUnit(enemy))
                    return true;
            }
        }
        return false;
    }

	public class CollisionPoint {
		public Vector2 point;
		public bool collides;
		public CollisionPoint(Vector2 p, bool c) {
			point = p;
			collides = c;
		}
		public CollisionPoint(bool c) {
			collides = c;
		}
	}
	public CollisionPoint getLineOfSightCollisionPoint(Vector2 from, Vector2 to, VisibilityMode visMode = VisibilityMode.Visibility) {
		int layerMask = 1 << 19;
		RaycastHit2D[] hits = Physics2D.LinecastAll(from, to, layerMask);
		//		bool ret = !Physics.Linecast(new Vector3(from.x / gridSize, from.y/gridSize, 0.0f), new Vector3(to.x / gridSize, to.y / gridSize, 0.0f), out hitInfo, layerMask);
		if (hits != null) {
			if (to.x == 17 * 64) Debug.Log("Hits: " + hits.Count() + "  " + from + " "  + to);
			foreach (RaycastHit2D hit in hits) {
				GameObject go = hit.collider.gameObject;
				Wall w = go.GetComponent<Wall>();
				if (w != null) {
					if (visMode == VisibilityMode.Visibility && w.visibility != 0) continue;
					if (visMode == VisibilityMode.Ranged && w.canRange) continue;
					if (visMode == VisibilityMode.Melee && w.canMelee) continue;
				}
				float angleDiff = go.transform.eulerAngles.z - getAngle(from, to);
				while (angleDiff < 0) angleDiff += 360.0f;
				while (angleDiff > 360) angleDiff -= 360.0f;
				if (go.GetComponent<Wall>().bothWays || (angleDiff > 180.0f && angleDiff < 360.0f)) {
					return new CollisionPoint(hit.point, true);
				}
			}
		}
		return new CollisionPoint(false);
	}

	public void setGameState() {
		setGameState(GameState.None);
	}

	public void setGameState(GameState gs) {
		if (gs != GameState.None) {
			gameState = gs;
			if (gs == GameState.Lost) playerLost();
			if (gs == GameState.Won) playerWon();
			return;
		}
		bool enemy = false;
		bool player = false;
		foreach (Unit u in priorityOrder) {
			if (u.deadOrDyingOrUnconscious()) continue;
			if (u.team == 0) player = true;
			else enemy = true;
			if (player && enemy) {
				gameState = GameState.Playing;
				return;
			}
		}
		if (!player) {
			gameState = GameState.Lost;
			playerLost();
		}
        else if (!enemy)
        {
            gameState = GameState.Won;
			playerWon();
             
        }
        else gameState = GameState.Playing;
	}

	public void playerWon() {
		restPlayers();
		rewardPlayer(mainUnit);
		savePlayers();
		deleteDeadPlayers();
		
		BattleGUI.setEndGameUnits(copperReward, experienceReward, true);
	}

	public void restPlayers() {
		foreach (Unit u in outOfGameUnits) {
			CombatScores cs = u.characterSheet.characterSheet.combatScores;
			if (cs.getCurrentHealth() <= 0) cs.addHealth(1);
			else cs.setHealth(cs.getMaxHealth());
			cs.setComposure(cs.getMaxComposure());
		}
	}

	public void savePlayers() {
		foreach (Unit u in players) {
			if (!u.isDead()) {
				u.saveCharacter();
			}
			else {
				u.deleteCharacter();
			}
		}
		foreach (Unit u in outOfGameUnits) {
			u.saveCharacter();
		}
	}

	public void deleteDeadPlayers() {
		foreach (Unit u in deadUnits) {
			u.deleteCharacter();
		}
	}

	public void playerLost() {
		
		BattleGUI.setEndGameUnits(copperReward, experienceReward, false);
	}

    public void rewardPlayer(Unit mainCharacter)
    {
        // Count up the players, divide map reward by number of players, give that amount to every player
        List<Unit> livingPlayers = new List<Unit>();
        foreach (Unit u in players)
        {
            if (!u.isDead())
                livingPlayers.Add(u);
        }
        int individualExpReward = Mathf.FloorToInt( experienceReward / livingPlayers.Count );
        foreach (Unit u in livingPlayers) {
			u.characterSheet.characterSheet.characterProgress.addExperience(individualExpReward);
			if (u != mainCharacter)
				mainCharacter.characterSheet.characterSheet.inventory.purse.takeAllMoney(u.characterSheet.characterSheet.inventory.purse);
		}
		foreach (Unit u in outOfGameUnits) {
			if (u != mainCharacter)
				mainCharacter.characterSheet.characterSheet.inventory.purse.takeAllMoney(u.characterSheet.characterSheet.inventory.purse);
		}
        mainCharacter.characterSheet.characterSheet.inventory.purse.receiveMoney(copperReward, 0, 0);

    }

	public static float getAngle(Vector3 start, Vector3 end) {
		return getAngle(new Vector2(start.x, start.y), new Vector2(end.x, end.y));
	}

	public static float getAngle(Vector2 start, Vector2 end) {
		float smallAngle = Mathf.Atan(Mathf.Abs(start.y - end.y)/Mathf.Max(Mathf.Abs(start.x - end.x),0.000001f)) * 180.0f / Mathf.PI;
		if (end.y >= start.y) {
			if (end.x < start.x) {
				return 180.0f - smallAngle;
			}
		}
		else {
			if (end.x < start.x) {
				return 180.0f + smallAngle;
			}
			return 360.0f - smallAngle;
		}
		return smallAngle;
	}

	// Use this for initialization
	void Start() {
		mg = this;
		Debug.Log("Start");
		GameGUI.mapGenerator = this;
		GameGUI.resetVars();
		RenderTexture tex = new RenderTexture(100, 100, 1);
		tex.Create();
		Debug.Log("Starting Tests!");
		Debug.Log(hasLineOfSight(new Vector2(17, -28), new Vector2(17, -35)));
		Debug.Log(hasLineOfSight(new Vector2(17, -35), new Vector2(17, -28)));
		Debug.Log("Angle Tests");
		Debug.Log(getAngle(new Vector2(0,0), new Vector2(1,0)));
		Debug.Log(getAngle(new Vector2(0,0), new Vector2(0,1)));
		Debug.Log(getAngle(new Vector2(0,0), new Vector2(0,-1)));
		Debug.Log(getAngle(new Vector2(0,0), new Vector2(-1,0)));
		Debug.Log("Ended Tests!");
		Time.timeScale = 1;
		if (testAnimations) Time.timeScale = timeScale;
		GameObject mainCameraObj = GameObject.Find("Main Camera");
		cameraTransform = mainCameraObj.transform;
		cameraPos = cameraTransform.position;
		mainCamera = mainCameraObj.GetComponent<Camera>();
		currentTrap = new List<TrapUnit>();
		//	cameraOriginalSize = mainCamera.orthographicSize;
		//		sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		//		spr = sprend.sprite;
		//	grids = GameObject.Find("Grids");
		//	gridsArray = new Tile[gridX,gridY];
		//	gridPrefab = (GameObject)Resources.Load("Sprite/Square_70");
		//	lines = GameObject.Find("Lines");
		//		0.02857
		
		gridPrefab = (GameObject)Resources.Load("Materials/Square_70");
		map = GameObject.Find("Map");
		mapTransform = map.transform;
		targetObject = GameObject.Find("Target");
		audioBank = GameObject.Find("AudioBank").GetComponent<AudioBank>();
		aManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

		turrets = mapTransform.FindChild("Turrets").gameObject;
		traps = mapTransform.FindChild("Traps").gameObject;
		lines = mapTransform.FindChild("Lines").gameObject;
		grids = mapTransform.FindChild("Grid").gameObject;
		path = mapTransform.Find("Path").gameObject;
		enemiesObj = mapTransform.Find("Enemies").gameObject;
		playerTransform = mapTransform.Find("Players");
		enemyTransform = enemiesObj.transform;




		createGrid();
		createTiles();

		priorityOrder = new List<Unit>();
		players = new List<Unit>();
		deadUnits = new List<Unit>();
		nonAlertEnemies = new List<Unit>();
		//		playerPrefab = (GameObject)Resources.Load("Units/Jackie/JackieAnimPrefab");
		playerPrefab = (GameObject)Resources.Load("Units/Male_Base/Male_Base_Unit");
		turretPrefab = (GameObject)Resources.Load("Units/Turrets/TurretPrefab");
		trapPrefab = (GameObject)Resources.Load("Units/Turrets/TrapPrefab");
		arrowStraightPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowStraight");
		arrowCurvePrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowCurve");
		arrowPointPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowPoint");
		warningRedPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningRedPrefab");
		warningYellowPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningYellowPrefab");
		warningBothPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningBothPrefab");
//		Vector3[] positions = new Vector3[] {new Vector3(20, -36, 0), new Vector3(10, -36, 0)};
//		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0), new Vector3(14,-30,0), new Vector3(15, -31, 0)};
//		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0)};
/*		for (int n=0;n<positions.Length;n++) {
			Vector3 pos = positions[n];
			GameObject player = GameObject.Instantiate(playerPrefab) as GameObject;
			player.transform.parent = mapTransform;
			Player p = player.GetComponent<Player>();
			p.mapGenerator = this;
			p.setPosition(pos);
			players.Add(player);
		//	player.renderer.sortingOrder = 4;
			tiles[(int)pos.x,(int)-pos.y].setCharacter(p);
			p.setPriority();
			priorityOrder.Add(p);
			p.characterName = "Player" + (n+1);
	//		p.deselect();
		}*/
		enemies = new List<Unit>();
		enemyPrefab = (GameObject)Resources.Load("Units/Jackie/JackieEnemy");
	/*	Vector3[] positions2 = new Vector3[] {new Vector3(15, -28, 0), new Vector3(17, -27, 0), new Vector3(4, -23, 0)};
		for (int n=0;n<positions2.Length;n++) {
			Vector3 pos = positions2[n];
			GameObject enemy = GameObject.Instantiate(enemyPrefab) as GameObject;
			enemy.transform.parent = enemiesObj;
			Enemy e = enemy.GetComponent<Enemy>();
			e.setPosition(pos);
			e.mapGenerator = this;
			enemies.Add(enemy);
			enemy.renderer.sortingOrder = 3;
		}*/
		int bbb = 1;
		GameObject[] mapPlayers = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in mapPlayers) {
			Vector3 pos = player.transform.position;
			Player p = player.GetComponent<Player>();
			int x = (int)(pos.x - 0.5f);
			int y = (int)(pos.y + 0.5f);
			p.setPosition(new Vector3(x, y, pos.z));
			p.setMapGenerator(this);
//			p.mapGenerator = this;
			players.Add(p);
			//		enemy.renderer.sortingOrder = 3;
			tiles[x,-y].setCharacter(p);
			p.loadCharacterSheet();
			p.rollInitiative();
			priorityOrder.Add(p);
			p.renderer.sortingOrder = playerNormalOrder;
			player.GetComponent<SpriteRenderer>().color = p.characterSheet.characterSheet.characterColors.characterColor;
			p.addHair();
			p.setAllSpritesToRenderingOrder(playerArmorOrder);
			//		e.deselect();
			bbb++;
		}
		int aaa = 1;
		GameObject[] mapEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		Debug.Log(mapEnemies.Count());
		foreach (GameObject enemy in mapEnemies) {
			Vector3 pos = enemy.transform.position;
			Debug.Log(enemy + "  " + enemy.transform.parent + "  "  + pos);
			Enemy e = enemy.GetComponent<Enemy>();
			int x = (int)(pos.x - 0.5f);
			int y = (int)(pos.y + 0.5f);
			e.setPosition(new Vector3(x, y, pos.z));
			e.setMapGenerator(this);
//			e.mapGenerator = this;
			enemies.Add(e);
			nonAlertEnemies.Add(e);
	//		enemy.renderer.sortingOrder = 3;
			tiles[x,-y].setCharacter(e);
			e.loadCharacterSheet();
			e.rollInitiative();
			priorityOrder.Add(e);
			e.renderer.sortingOrder = playerNormalOrder;
			enemy.GetComponent<SpriteRenderer>().color = e.characterSheet.characterSheet.characterColors.characterColor;
			e.addHair();
			e.setAllSpritesToRenderingOrder(playerArmorOrder);
			e.setAllSpritesRenderQueue(1000);
	//		e.deselect();
			aaa++;
		}

		sortPriority();

	
		importGrid();
		addItemsToMap();
		createSelectionArea();
		createSelectionUnits();
		setOverlay();
	
//		StartCoroutine(importGrid());
//		Debug.Log(b4 + "\n\n" + after);
//		Debug.Log(after);
//		priorityOrder = priorityOrder.
	}

	public void addItemsToMap() {
		if (itemPositions == null || items == null) return;
		for (int n=0;n<Mathf.Min(itemPositions.Count, items.Count);n++) {
			Vector2 pos = itemPositions[n];
			Debug.Log("Map Item");
			Item i = items[n].getItem();
			Debug.Log("End Map Item");
			tiles[(int)pos.x,(int)pos.y].addItem(i);
		}
	}

	public void enterPriority() {
		if (isInCharacterPlacement()) removeCharacterPlacementObjects();
		foreach (Unit u in priorityOrder) {
			u.rollInitiative();
			if(u is Player) {
				Vector3 tempVec = u.transform.position;
				tempVec.z = 0;
				u.transform.position = tempVec;
				u.setAllSpritesRenderQueue(1000);
			}
		}
		sortPriority();
		nextPlayer();
		setOverlay();
	}

	public void sortPriority() {
		List<Unit> po1 = new List<Unit>();
		foreach (Unit cs in priorityOrder) {
			po1.Add(cs);
		}
		priorityOrder.Sort((first, second) => (first.getInitiative() > second.getInitiative() ? -1 : (first.getInitiative() == second.getInitiative() && po1.IndexOf(first) < po1.IndexOf(second) ? -1 : 1)));
	}

	public void activateEnemies() {
		bool anyEnemiesActive = false;
		List<Unit> nonAlertEnemiesCopy = new List<Unit>();
		foreach (Unit u in nonAlertEnemies) nonAlertEnemiesCopy.Add(u);
		while (nonAlertEnemiesCopy.Count > 0) {
			Unit e = nonAlertEnemiesCopy[0];
			nonAlertEnemiesCopy.RemoveAt(0);
			foreach (Player p in players) {
				if (e.hasLineOfSightToUnit(p)) {
					anyEnemiesActive = true;
					e.setActive(true);
					List<Unit> newlyActivatedUnits = new List<Unit>();
					newlyActivatedUnits.Add(e);
					nonAlertEnemies.Remove(e);
					while (newlyActivatedUnits.Count > 0) {
						Unit curr = newlyActivatedUnits[0];
						newlyActivatedUnits.RemoveAt(0);
						for (int m=nonAlertEnemies.Count-1;m>=0;m--) {
							Unit ee = nonAlertEnemies[m];
							if (ee.hasLineOfSightToUnit(curr)) {
								ee.setActive(true);
								newlyActivatedUnits.Add(ee);
								nonAlertEnemies.Remove(ee);
								if (nonAlertEnemiesCopy.Contains(ee)) nonAlertEnemiesCopy.Remove(ee);
							}
						}
					}
					break;
				}
			}
		}
		if(anyEnemiesActive)
		{
			aManager.invokeFadeInMusic();
		}
	}

	public bool isInCharacterPlacement() {
		return currentUnit == -1;
	}

	public bool isInPriority() {
		return currentUnit != -1;
	}

	public int selectionWidth = 100;
	public float spriteSize = 64.0f;
	public float spriteSeparator = 30.0f;
	float scaleFactor = 100.0f/64.0f;

	public void removeCharacterPlacementObjects() {
		GameObject gold = GameObject.Find("Selection");
		if (gold != null)
			Destroy(gold);
		foreach (Unit u in selectionUnits) {
			players.Remove(u);
			priorityOrder.Remove(u);
//			if (u == mainUnit) {
				u.gameObject.SetActive(false);//.renderer.enabled = false;
				outOfGameUnits.Add(u);
//			}
//			else {
//				outOfGameUnits.Add(u);
//				Destroy(u.gameObject);
//			}
		}
	}

	public void removeCurrentTrap() {
		if (currentlySelectedTrap==null) return;
		for (int n = currentTrap.Count-1;n>=0;n--) {
			Destroy(currentTrap[n].gameObject);
			currentTrap.RemoveAt(n);
		}
		currentlySelectedTrap = null;
	}

	public void resetCurrentKeysTile() {
		if (isInCharacterPlacement()) return;
		
		if (turretBeingPlaced != null) {
			unsetTurretDirectionAttack((int)turretBeingPlaced.position.x, (int)-turretBeingPlaced.position.y, 5, turretBeingPlaced.direction, turretBeingPlaced);
			tiles[(int)turretBeingPlaced.position.x, (int)-turretBeingPlaced.position.y].removeCharacter();
			Destroy(turretBeingPlaced.gameObject);
			turretBeingPlacedInDirection = Direction.None;
			turretBeingPlaced = null;
			drawAllRanges();
		}
		if (currentTrap.Count > 0) {
			removeCurrentTrap();
		}
		currentKeysTile = currentUnitTile;
		if (GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.Lay_Trap)
			currentKeysSize = 5;
		else if ((GameGUI.selectedMovement && GameGUI.selectedMovementType == MovementType.Move) || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape))
			currentKeysSize = getCurrentUnit().maxMoveDist;
		else if (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock)) currentKeysSize = getCurrentUnit().getAttackRange();
		else currentKeysSize = 1;
	}

	public void setCurrentUnitTile() {
		if (isInCharacterPlacement()) return;
		currentUnitTile = tiles[(int)selectedUnit.position.x,(int)-selectedUnit.position.y];
		resetCurrentKeysTile();
	}

	public void createSelectionArea() {
		if (!isInCharacterPlacement()) return;
		selectionUnitsX = Screen.width/2.0f - (selectionWidth)/2.0f;
		float scrollHeight = spriteSeparator + (spriteSeparator + spriteSize) * (selectionUnits == null ? 0 : selectionUnits.Count + (selectionCurrentIndex>=0?1:0));

//		if (Screen.height < selectionUnits.Count * (spriteSize + spriteSeparator) + spriteSeparator)
		if (Screen.height < scrollHeight)
			selectionUnitsX -= (selectionWidth - spriteSize)/4.0f;
		repositionSelectionUnits();
		if (Screen.width == screenWidth && Screen.height == screenHeight) return;

//		selectionUnitsX *= scaleFactor;
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		GameObject gold = GameObject.Find("Selection");
		if (gold != null)
			Destroy(gold);
		float texWidth = selectionWidth * scaleFactor;
		float texHeight = Screen.height * scaleFactor;
		float texX = ((Screen.width) * scaleFactor/2.0f - texWidth)/100.0f;
		float texY = -texHeight/2.0f/100.0f;
		Texture2D tex = makeTexBorder((int)texWidth, (int)texHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		GameObject go = new GameObject();
		go.name = "Selection";
		SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
		sr.sortingOrder = playerSelectOrder;
//		sr.sprite.texture = tex;
		//		Sprite spr = Sprite.Create(tex, new Rect(Screen.width - 100, 0.0f, 100.0f, Screen.height), new Vector2(Screen.width - 100, 0.0f));
		Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, texWidth, texHeight), new Vector2(0.0f, 0.0f));
	//	tex.pix
		sr.sprite = spr;
		sr.transform.parent = Camera.main.transform;
		go.transform.localPosition = new Vector3(texX, texY, 1.0f);
	}

	float selectionUnitsX;
	public void createSelectionUnits() {
        if (!Directory.Exists(Saves.getCurrentSaveDirectory()) || !File.Exists(Saves.getCharactersListFilePath()))
        {
            Application.LoadLevel("MainMenu");
            return;
        }
		selectionUnits = new List<Unit>();
		outOfGameUnits = new List<Unit>();
//		TextAsset t = Resources.Load<TextAsset>("Saves/Characters");
		string[] chars = Saves.getCharacterList();
		for (int n=0;n<chars.Length-1;n++) {
			GameObject p = (GameObject)GameObject.Instantiate(playerPrefab);
			SpriteRenderer sr = p.GetComponent<SpriteRenderer>();
			sr.sortingOrder = playerSelectPlayerOrder;
//			p.transform.parent = Camera.main.transform;
			Unit pl = p.GetComponent<Unit>();
			if (n==0) mainUnit = pl;
		//	pl.characterId = chars[n];
//			pl.mapGenerator = this;
			pl.setMapGenerator(this);
			pl.loadCharacterSheetFromTextFile(chars[n]);
			if (pl.characterSheet.characterSheet.combatScores.getCurrentHealth() > 0) {
				p.transform.parent = Camera.main.transform;
				players.Add(pl);
				priorityOrder.Add(pl);
				pl.addHair();
				pl.setAllSpritesToRenderingOrder(playerSelectPlayerArmorOrder);
				sr.color = pl.characterSheet.characterSheet.characterColors.characterColor;
				pl.rollInitiative();
				selectionUnits.Add(pl);
			}
			else {
				outOfGameUnits.Add(pl);
				p.SetActive(false);
			}
		}
		sortPriority();
		repositionSelectionUnits();
	}

	public void repositionSelectionUnits() {
		if (selectionUnits == null) return;
		float y = Screen.height / 2.0f - spriteSeparator - spriteSize/2.0f + GameGUI.selectionUnitScrollPosition.y;
		float initialY = y;
		int n=0;
		foreach (Unit p in selectionUnits) {
			if (n==selectionCurrentIndex) {
				y -= spriteSeparator + spriteSize;
			}
			if (p.gameObject != selectedSelectionObject) {
				p.transform.localPosition = new Vector3(selectionUnitsX/spriteSize, y/spriteSize, p.transform.localPosition.z);
			}
			else {
		//		Debug.Log("Selected: " + p.characterSheet.personalInfo.getCharacterName().fullName());
			}
			y -= spriteSeparator + spriteSize;
			n++;
		}
		if (selectedSelectionObject!=null) {
			if (Input.mousePosition.x >= Screen.width - selectionWidth) {
				float y2 = spriteSeparator + spriteSize/2.0f - GameGUI.selectionUnitScrollPosition.y;
				int mouseY = (int)(Screen.height - Input.mousePosition.y);
				int p = (int)((mouseY - y2) / (spriteSeparator + spriteSize) + 1) - 1;
				if (p<selectionCurrentIndex || selectionCurrentIndex == -1) p++;
				if (p > selectionUnits.Count) p = selectionUnits.Count;
				selectionCurrentIndex = p;
			//	Debug.Log(Input.mousePosition.y + "   " +  p);
				int pos = (int)((initialY - selectedSelectionObject.transform.localPosition.y) / (spriteSeparator + spriteSize));
			//	selectionCurrentIndex = pos;
	//			Debug.Log(pos);
			}
			else {
				selectionCurrentIndex = -1;
			}
		}
	}

	Texture2D makeTexBorder(int width, int height, Color col )
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
	

	

	public Unit nextPlayer() {
		if (gameState != GameState.Playing) return null;
		GameGUI.selectMovementType(MovementType.None);
		GameGUI.selectStandardType(StandardType.None);
		GameGUI.selectMinorType(MinorType.None);
		if (currentUnit >=0 && currentUnit < priorityOrder.Count) {
			getCurrentUnit().removeCurrent();
			getCurrentUnit().endTurn();
		}
		Unit.actionTime = Time.time;
		currentUnit++;
		currentUnit%=priorityOrder.Count;
		resetPlayerPath();
	//	selectedUnit = getCurrentUnit();
		if (selectedUnit) {
			selectedUnit.resetVars();
			if (selectedUnit.attackEnemy) {
				selectedUnit.attackEnemy.deselect();
			}
		}
		/*
		if (selectedUnit) {
			resetAroundCharacter(selectedUnit);
			lastPlayerPath = new ArrayList();
			selectedUnit.resetPath();
			selectedUnit.attackEnemy = null;
			string color = "Materials/SelectionCircleGreen";
			if (selectedUnit.team != 0) color = "Materials/SelectionCircleRed";
		//	selectedUnit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(color);
	//		selectedUnit.transform.FindChild("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(color);
			selectedUnit.resetVars();
			selectedUnit.deselect();
		}
		Unit u;
		while (selectedUnits.Count != 0) {
			u = selectedUnits[0];
			resetAroundCharacter(u);
			u.deselect();
			selectedUnits.RemoveAt(0);
		}*/
		deselectAllUnits();
//		if (hoveredCharacter) {
//			resetAroundCharacter(hoveredCharacter);
//		}
	//	if (getCurrentUnit().hasWeapon())
	//		GameGUI.selectedStandardType = StandardType.Attack;
	//	else GameGUI.selectedStandardType = StandardType.None;
	//	GameGUI.selectedMovementType = MovementType.Move;
		selectedUnit = getCurrentUnit();
	//	selectedUnit.transform.FindChild("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Materials/SelectionCircleWhite");
		if (selectedUnit) {
			selectedUnit.beginTurn();
			setCurrentUnitTile();
			selectedUnit.resetVars();
//			setAroundCharacter(selectedUnit);
			addCharacterRange(selectedUnit);
			selectedUnit.setSelected();
			selectedUnit.setCurrent();
			if (selectedUnit.playerControlled || selectedUnit.aiActive)
				moveCameraToSelected();
			lastPlayerPath = selectedUnit.currentPath;
			float closestEnemy = selectedUnit.closestEnemyDist();
         /*   if (closestEnemy > selectedUnit.characterSheet.characterSheet.characterLoadout.rightHand.range)
            {
				GameGUI.selectMove();
			}
			else {
				GameGUI.selectAttack();
			}*/
			if (selectedUnit.deadOrDying()) {
				selectedUnit.damage(1,null);
				selectedUnit.showDamage(1, true, false);
				if (selectedUnit.isDead()) {
					removeCharacter(selectedUnit);
				}
			}
			if (selectedUnit.deadOrDyingOrUnconscious() || (!selectedUnit.playerControlled && !selectedUnit.aiActive)) {
				return nextPlayer();
			}
			BattleGUI.setPlayerTurnText(selectedUnit.getName() + "'s Turn!", selectedUnit.team == 0 ? Log.greenColor : Color.red);
			activateEnemies();
		//	selectedUnit.chooseNextBestActionType();

			//		editingPath = false;
		}
        //Invoke("beginTurn", 0.5f);
//		setTargetObjectPosition();
		return getCurrentUnit();
	}

    //private void beginTurn()
    //{
    //    BattleGUI.beginTurn();
    //}


	public Unit getCurrentUnit() {
		if (currentUnit == -1) return null;
		return priorityOrder[currentUnit];
	}

	public void importGrid() {
		string text = Resources.Load<TextAsset>("Maps/Tile Maps/" + tileMapName).text;// + ((tileMapName.Length >= 4 && tileMapName.EndsWith(".txt")) ? "" : ".txt")).text;
		Debug.Log(text);
		string[] tiles = text.Split(new char[]{';'});
		if (int.Parse(tiles[1])==actualWidth && int.Parse(tiles[2])==actualHeight) {
			////Debug.Log("Works!");
			parseTiles(tiles);
		}
		currentUnit = -1;
//		nextPlayer();
	//	Debug.Log (getCurrentUnit().characterName);
//		moveCameraToSelected(true);
		/*
		string pathName = Application.dataPath + "/Resources/Maps/Tile Maps/" + tileMapName;
		if (!(tileMapName.Length >= 4 && tileMapName.EndsWith(".txt"))) {
			pathName += ".txt";
		}
		Debug.Log(pathName);
		if (File.Exists(pathName)) {
			Debug.Log("File Exists");
			//Debug.Log("Exists!");
			WWW www = new WWW("file:///" + pathName);
			yield return www;
			string text = www.text;
			string[] tiles = text.Split(new char[]{';'});
			if (int.Parse(tiles[1])==actualWidth && int.Parse(tiles[2])==actualHeight) {
				////Debug.Log("Works!");
				parseTiles(tiles);
			}
			currentUnit = -1;
			nextPlayer();
			Debug.Log (getCurrentUnit().characterName);
			moveCameraToSelected(true);
		}*/
	}

	void parseTiles(string[] tilesArr) {
		bool vis = false;
		Transform walls = map.transform.FindChild("Walls");
		List<Vector2> positions = new List<Vector2>();
		for (int n=3;n<tilesArr.Length;n++) {
			if (!vis) {
				if (tilesArr[n]=="Visibility") vis = true;
				else {
					int x = Tile.xForTile(tilesArr[n]);
					int y = Tile.yForTile(tilesArr[n]);
					Tile t = tiles[x,y];
					t.parseTile(tilesArr[n]);
					/*
					if (!t.isVisibleFrom(Direction.Right)) {
						GameObject wall = GameObject.Instantiate(rightWallPrefab) as GameObject;
						wall.transform.parent = walls;
						wall.transform.position = new Vector3(x + 1.0f, -y - .5f, 0.0f);
					}
					if (!t.isVisibleFrom(Direction.Left)) {
						GameObject wall = GameObject.Instantiate(leftWallPrefab) as GameObject;
						wall.transform.parent = walls;
						wall.transform.position = new Vector3(x, -y - .5f, 0.0f);
					}
					if (!t.isVisibleFrom(Direction.Up)) {
						GameObject wall = GameObject.Instantiate(upWallPrefab) as GameObject;
						wall.transform.parent = walls;
						wall.transform.position = new Vector3(x + 0.5f, -y, 0.0f);
					}
					if (!t.isVisibleFrom(Direction.Down)) {
						GameObject wall = GameObject.Instantiate(downWallPrefab) as GameObject;
						wall.transform.parent = walls;
						wall.transform.position = new Vector3(x + 0.5f, -y - 1.0f, 0.0f);
					}*/
					if (t.startingPoint) {
						GameObject go = gridArray[x,y];
						SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
						Color c = Color.green;
						c.a = .4f;
						sr.color = c;
						positions.Add(new Vector2(go.transform.position.x, go.transform.position.y));
					}
				}
			}
			else {
				GameObject wall = GameObject.Instantiate(anyWallPrefab) as GameObject;
				wall.transform.parent = walls;
				Wall w = wall.GetComponent<Wall>();
				w.parseWall(tilesArr[n], this);
				Debug.Log(w.visibility + " " + w.transform.position);
			}
		}
		if (positions.Count > 0) {
			Vector2 avg = getAverage(positions);
			moveCameraToPosition(new Vector3(avg.x+.5f, avg.y-.5f, 0.0f), true);
		}
	}

	Vector2 getAverage(List<Vector2> positions) {
		Vector2 avg = new Vector2(0.0f, 0.0f);
		foreach (Vector2 pos in positions) {
			avg += pos;
		}
		avg /= positions.Count;
		return avg;
	}

	void createGrid() {
		
		
		overlayObject = map.transform.FindChild("Overlay").gameObject;
//		GameObject overlayCreate = GameObject.Instantiate(overlayObject) as GameObject;
		sprend = map.GetComponent<SpriteRenderer>();
		spr = sprend.sprite;
		int width = spr.texture.width;
		int height = spr.texture.height;
		actualWidth = width / gridSize;
		actualHeight = height / gridSize;
/*		mapOverlays = new Texture2D[actualWidth, actualHeight];
		for (int n=0;n<actualWidth;n++) {
			for (int m=0;m<actualHeight;m++) {
				mapOverlays[n,m] = new Texture2D(gridSize, gridSize, TextureFormat.ARGB32, false);
				for (int o=0;o<gridSize;o++) {
					for (int p=0;p<gridSize;p++) {
						mapOverlays[n,m].SetPixel(o,p, (doOverlay ? Color.black : Color.clear));//colors[(n + m)%2]);
					}
				}
				mapOverlays[n,m].Apply();
				GameObject go = GameObject.Instantiate(overlayCreate) as GameObject;
				go.transform.parent = overlayObject.transform;
				go.transform.position = new Vector3(n, -(actualHeight - m - 1), 0.0f);
				SpriteRenderer spr2 = go.GetComponent<SpriteRenderer>();
				spr2.sprite = Sprite.Create(mapOverlays[n,m], new Rect(0, 0, gridSize, gridSize), new Vector2(0.0f, 1.0f), gridSize);
			//	spr2.sprite.texture.filterMode = FilterMode.Point;
				mapOverlays[n,m].wrapMode = TextureWrapMode.Clamp;
				spr2.renderer.sortingOrder = mapOverlayOrder;
			}
		}*/
//		canSee = new bool[width, height];
//		resetCanSee(null, !doOverlay);







		/*	for (int n=0;n<width;n++) {
		//	if (n%2==0) {
			for (int m=0;m<height;m++) {
				mapOverlay.SetPixel(n, m, Color.clear);
		//		}
			}
		}
		mapOverlay.Apply();*/
//		mapOverlay.SetPixels(new Color[]{Color.red});
//		SpriteRenderer overlaySprite = overlayObject.GetComponent<SpriteRenderer>();
//		overlaySprite.sprite = Sprite.Create(mapOverlay, new Rect(0, 0, width, height), new Vector2(0, 1.0f), gridSize);
//		overlaySprite.renderer.sortingOrder = mapOverlayOrder;
//		if (spr.texture.width % 64 == 0) gridSize = 64;
		Camera.main.orthographicSize = Mathf.Min(Mathf.Max(actualWidth/3, actualHeight/2) + 2, 10.0f);
	//	Vector3 newPos = Camera.main.transform.position;
		cameraPos.x = ((float)actualWidth) / 2.0f;
		cameraPos.y = -((float)actualHeight)/ 2.0f;
	//	Camera.main.transform.position = newPos;
//		mainCamera.transform.position = new Vector3(((float)((int)(cameraPos.x * gridSize)))/((float)gridSize), ((float)((int)(cameraPos.y * gridSize)))/((float)gridSize), cameraPos.z);
		setCameraPos();
		//Debug.Log("width: " + width + ", height: " + height);
		//Debug.Log("actualWidth: " + actualWidth + ", actualHeight: " + actualHeight);
		//Debug.Log("newPos: " + newPos);
		//Debug.Log("End");
		selectedUnit = null;
		lastPlayerPath = new List<Vector2>();
		selectedUnits = new List<Unit>();
		for (int n=0;n<=actualHeight;n++) {
			createLineRenderer(-0.025f, actualWidth + 0.025f, -n, -n);
		}
		for (int n=0;n<=actualWidth;n++) {
			createLineRenderer(n, n, 0.025f, -actualHeight - 0.025f);
		}
		loadGrid(actualWidth, actualHeight);
		//		setAroundPlayer(currentMoveDist, viewDist);
	}

	void createTiles() {
		tiles = new Tile[actualWidth,actualHeight];
		for (int n=0;n<actualWidth;n++) {
			for (int m=0;m<actualHeight;m++) {
				Tile t1 = new Tile();
				tiles[n,m] = t1;
				if (n>0) {
					Tile t2 = tiles[n-1,m];
					t2.rightTile = t1;
					t1.leftTile = t2;
				}
				if (m>0) {
					Tile t2 = tiles[n,m-1];
					t2.downTile = t1;
					t1.upTile = t2;
				}
			}
		}
	}
	
	void createLineRenderer(float xStart, float xEnd, float yStart, float yEnd) {
		GameObject lrO = GameObject.Instantiate(Resources.Load("Materials/LineRenderer",typeof(GameObject)) as GameObject) as GameObject;
		LineRenderer lr = lrO.GetComponent<LineRenderer>();
//		Debug.Log("Line: " + lr.renderer.sortingOrder);
		lr.renderer.sortingOrder = lineOrder;
		lr.renderer.material.renderQueue = 1000;
		lr.SetPosition(0, new Vector3(xStart, yStart, 0.0f));
		lr.SetPosition(1, new Vector3(xEnd, yEnd, 0.0f));
		lrO.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		lrO.transform.parent = lines.transform;
	}

	public void removeCharacter(Unit cs) {

		cs.removeCurrent();
		int index = priorityOrder.IndexOf(cs);
		priorityOrder.Remove(cs);
		if (enemies.Contains(cs)) enemies.Remove(cs);
		if (players.Contains(cs)) {
			players.Remove(cs);
			deadUnits.Add(cs);
		}
		if (index < currentUnit) currentUnit--;
		else if (index == currentUnit) {
			currentUnit--;
			GameGUI.selectedMovement = false;
			nextPlayer();
		}

		bool anyEnemiesActive = false;
		foreach(Unit u in enemies)
		{
			if(u.aiActive)
			{
				anyEnemiesActive = true;
				break;
			}
		}

		if(!anyEnemiesActive)
		{
			aManager.invokeFadeOutMusic();
		}
	}

	public bool hasEnemy(int x, int y, Unit cs) {
		return tiles[x,y].hasEnemy(cs);
	}

	public bool hasAlly(int x, int y, Unit cs) {
		return tiles[x,y].hasAlly(cs);
	}

	public bool canPass(Direction dir, int x, int y, Unit cs, Direction dirFrom) {
		return tiles[x,y].canPass(dir, cs, dirFrom);
	}

	public int passibility(Direction dir, int x, int y) {
		return tiles[x,y].passabilityInDirection(dir);
	}

	public bool canAttack(Direction dir, int x, int y, Unit cs) {
		int pass = tiles[x,y].passabilityInDirection(dir);
		return pass >0 && pass <10;
	}

	// Update is called once per frame
	void Update () {
		handleInput();
		moveCamera();
		if (isInCharacterPlacement()) {
			createSelectionArea();
		}
		if (isInPriority()) {
			GameGUI.setConfirmShown();
		}
	//	setTargetObjectScale();
	}

	float cameraSpeed = 32.0f;
	void moveCamera() {
		if (!movingCamera) return;
	//	float speed = 32.0f;
		float dist = cameraSpeed * Time.deltaTime;
//		float distLeft = Mathf.
		Vector3 pos = cameraPos;//Camera.main.transform.position;
		Vector3 left = new Vector3(cameraMoveToPos.x - pos.x, cameraMoveToPos.y - pos.y, cameraMoveToPos.z - pos.z);
		float distLeft = Mathf.Sqrt(Mathf.Pow(left.x,2) + Mathf.Pow(left.y,2) + Mathf.Pow(left.z,2));
		if (distLeft <= dist) {
			cameraPos = cameraMoveToPos;
//			Camera.main.transform.position = cameraMoveToPos;
		//	mainCamera.transform.position = new Vector3(((float)((int)(cameraPos.x * gridSize)))/((float)gridSize), ((float)((int)(cameraPos.y * gridSize)))/((float)gridSize), cameraPos.z);
			setCameraPos();
			movingCamera = false;
		}
		else {
			Vector3 move = new Vector3(left.x, left.y, left.z);
			move.x /= distLeft;
			move.y /= distLeft;
			move.z /= distLeft;
			move.x *= dist;
			move.y *= dist;
			move.z *= dist;
			pos += move;
		//	Camera.main.transform.position = pos;
			cameraPos = pos;
		//	mainCamera.transform.position = new Vector3(((float)((int)(cameraPos.x * gridSize)))/((float)gridSize), ((float)((int)(cameraPos.y * gridSize)))/((float)gridSize), cameraPos.z);
			setCameraPos();
		}
	}

	public void setCameraPos() {
		float sc = gridSize/1.0f;
		mainCamera.transform.position = new Vector3(((float)((int)(cameraPos.x * sc)))/sc, ((float)((int)(cameraPos.y * sc)))/sc, cameraPos.z);

	}

	public void moveCameraToSelected(bool instantly = false, float speed = 32.0f) {
		if (selectedUnit == null || (selectedUnit.team != 0 && !hasLineOfSight(selectedUnit))) return;
		Vector3 sel = selectedUnit.transform.position;
		moveCameraToPosition(sel, instantly, speed);
	}

	public void moveCameraToPosition(Vector3 position, bool instantly = false, float speed = 32.0f) {
		cameraSpeed = speed;
		position.z = Camera.main.transform.position.z;
		if (instantly) {
			cameraPos = position;
			setCameraPos();
//			Camera.main.transform.position = position;
		}
		else {
			movingCamera = true;
			cameraMoveToPos = position;
		}
	}

	public void resetPlayerPath() {
	//	if (lastPlayerPath!=null) {
			for (int n=path.transform.childCount-1; n >= 0;n--) {
				Transform t = path.transform.GetChild(n);
				GameObject go = t.gameObject;
				t.parent = null;
				Destroy(go);
			
			}
	//	}
	}

	public void setPlayerPath(List<Vector2> path1) {
		for (int n=1;n<path1.Count;n++) {
			Vector2 v = path1[n];
			Vector2 v0 = path1[n-1];


			GameObject go;
			if (n == path1.Count-1) {
				go = GameObject.Instantiate(arrowPointPrefab) as GameObject;
				go.renderer.sortingOrder = arrowOrder;
				int xDif = (int)(v.x - v0.x);
				int yDif = (int)(v.y - v0.y);
				go.transform.eulerAngles = new Vector3(0.0f, 0.0f, (xDif==-1 ?90.0f : (xDif==1 ? 270.0f : (yDif == -1 ? 0.0f : 180.0f))));
			}
			else {
				Vector2 v2 = path1[n+1];
				if (v2.x == v0.x || v2.y == v0.y) {
					go = GameObject.Instantiate(arrowStraightPrefab) as GameObject;
					go.renderer.sortingOrder = arrowOrder;
					if (v2.y == v0.y) {
						go.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					}
				}
				else {
					int xDif1 = (int)(v2.x - v.x);
					int yDif1 = (int)(v2.y - v.y);
					int xDif2 = (int)(v.x - v0.x);
					int yDif2 = (int)(v.y - v0.y);
			//		//Debug.Log("xDif1: " + xDif1 + " yDif1: " + yDif1 + " xDif2: " + xDif2 + " yDif2: " + yDif2);
					go = GameObject.Instantiate(arrowCurvePrefab) as GameObject;
					go.renderer.sortingOrder = arrowOrder;
					float rot = 0.0f;
					if (xDif1 == -1 && yDif2 == -1) rot = 270.0f;
					else if (xDif2 == 1 && yDif1 == -1) rot = 180.0f;
					else if (xDif2 == 1 && yDif1 == 1) rot = 270.0f;
					else if (xDif1 == 1 && yDif2 == 1) rot = 90.0f;
					else if (xDif1 == -1 && yDif2 == 1) rot = 180.0f;
					else if (xDif2 == -1 && yDif1 == -1) rot = 90.0f;
				//	if (xDif == -1 && yDif == -1) rot = 90.0f;
				//	if (xDif == 1 && yDif == 1) rot = 270.0f;
				//	if (xDif == -1 && yDif == 1) rot = 180.0f;
					go.transform.eulerAngles = new Vector3(0.0f, 0.0f, rot);
				}
			}
		//	go.renderer.sortingOrder = 2;
		//	= GameObject.Instantiate(arrowStraightPrefab) as GameObject;
			go.transform.parent = path.transform;
			go.transform.localPosition = new Vector3(v.x + 0.5f - 1/64.0f, -v.y - 0.5f - 1/64.0f, 0.0f);//-arrowOrder/renderThing);

			Tile t = tiles[(int)v0.x,(int)v0.y];
			Direction direction = Direction.Left;
			if (v0.x < v.x) direction = Direction.Right;
			if (v0.y < v.y) direction = Direction.Down;
			if (v0.y > v.y) direction = Direction.Up;
			bool isDifficult = t.isDifficultTerrain(direction);
			bool provokes = (getCurrentUnit().unitMovement == UnitMovement.Move && t.provokesOpportunity(direction, selectedUnit));
			if (isDifficult || provokes) {
				GameObject warning;
				if (isDifficult && provokes) warning = GameObject.Instantiate(warningBothPrefab) as GameObject;
				else if (isDifficult) warning = GameObject.Instantiate(warningYellowPrefab) as GameObject;
				else warning = GameObject.Instantiate(warningRedPrefab) as GameObject;
				warning.renderer.sortingOrder = warningOrder;
				warning.transform.parent = path.transform;
				warning.transform.localPosition = new Vector3(v0.x + (direction==Direction.Right ? 1.0f : (direction==Direction.Left ? 0.0f : 0.5f)), -v0.y - (direction==Direction.Down ? 1.0f : (direction==Direction.Up ? 0.0f : 0.5f)), 0.0f);// -warningOrder/renderThing);
			}

		}
	}

	public void resetCharacterRange() {
		Unit p = null;
		if (this.selectedUnit) p = this.selectedUnit;
		else if (this.hoveredCharacter) p = this.hoveredCharacter;
		if (p != null) {
			resetAroundCharacter(p);
		}
		foreach (Unit u in selectedUnits) {
			resetAroundCharacter(u);
		}
		if (p != null) {
			setAroundCharacter(p);
		}
		foreach (Unit u in selectedUnits) {
			setAroundCharacter(u);
		}
	}

	public void resetAroundCharacter(Unit cs) {
		resetAroundCharacter(cs, cs.viewDist);
	}

	public void resetAroundCharacter(Unit cs, int view) {
		for (int x = (int)Mathf.Max(cs.position.x - view,0); x < (int)Mathf.Min(cs.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-cs.position.y - view,0); y < (int)Mathf.Min(-cs.position.y + 1.0f + view, actualHeight); y ++) {
				GameObject go = gridArray[x,y];
				tiles[x,y].resetStandability();
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Color c = Color.clear;
				if (sr!=currentSprite) {
					sr.color = c;
				}
				else {
					currentSpriteColor = c;
				}
			}
		}
		setCurrentSpriteColor();
	}

	public void setSpriteColor(Tile t, SpriteRenderer sr) {
		Color c = Color.clear;
		if (t.canStandCurr) c = new Color(0.0f, 0.0f, 1.0f, 0.4f);
		else if (t.canAttackCurr) c = new Color(1.0f, (t.canUseSpecialCurr?0.5f:0.0f), 0.0f, 0.4f);
		else if (t.canUseSpecialCurr) c = new Color(0.0f, 1.0f, 0.0f, 0.4f);
		else if (isInCharacterPlacement() && t.startingPoint) c = new Color(0.0f, 1.0f, 0.0f, 0.4f);
		if (sr != currentSprite) {
			sr.color = c;
		}
		else {
			currentSpriteColor = c;
		}
	}

	public void setAroundCharacter(Unit cs) {
		setAroundCharacter(cs, ((cs == getCurrentUnit() && selectedUnits.Count==0) ? cs.currentMoveDist : cs.moveDistLeft), cs.viewDist, cs.attackRange);
	}

	public HashSet<Tile> setAroundCharacter(Unit cs, int radius, int view, int attackRange) {
        //Debug.Log(radius);
		return setCharacterCanStand((int)cs.position.x, (int)-cs.position.y, radius, 0, attackRange, cs);
		int type = 4;
		/*
		for (int x = (int)Mathf.Max(cs.position.x - view,0); x < (int)Mathf.Min(cs.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-cs.transform.localPosition.y - view,0); y < (int)Mathf.Min(-cs.position.y + 1.0f + view, actualHeight); y++) {
				GameObject go = gridArray[x,y];
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Tile t = tiles[x,y];
				setSpriteColor(t, sr);
			}
		}*/
	}

	public HashSet<Tile> setCharacterCanStand(int x, int y, int radiusLeft, int currRadius, int attackRange, Unit cs, bool doAttack = false, int minorsUsed = 0, Direction dirFrom = Direction.None, HashSet<Tile> tileSet = null) {
	//	if (currRadius == 0) //Debug.Log(attackRange);
		HashSet<Tile> ts = new HashSet<Tile>();
		if (x < 0 || y < 0 || x >= actualWidth || y >= actualHeight) return ts;
		if (minorsUsed > cs.minorsLeft) return ts;
		Tile t = tiles[x,y];
        //if (tileSet == null)                //
        //    tileSet = new HashSet<Tile>();  //
        //if (tileSet.Contains(t)) return;    //
        //tileSet.Add(t);                     //
		if (t.canStandCurr && t.minDistCurr <= currRadius && t.minDistUsedMinors <= minorsUsed) return ts;
		if (t.minDistCurr > currRadius) {
			if (!t.canStandCurr) ts.Add(t);
			t.canStandCurr = true;
			t.minDistCurr = currRadius;
			t.minDistUsedMinors = minorsUsed;
		}
	//	if ((selectedUnits.Count != 0 || selectedUnit != getCurrentUnit()) && (GameGUI.showAttack || doAttack))
	//		setCharacterCanAttack(x, y, attackRange, 0, cs);
		if (doAttack) {
		//	Debug.Log("Attack!");
			setCharacterCanAttack(x, y, attackRange, 0, cs);
		}
		if (radiusLeft <= 0) return ts;
        //Debug.Log(x +" "+ y +" "+ radiusLeft +" "+ currRadius +" "+ minorsUsed +" "+ dirFrom);
		if (canPass(Direction.Left, x, y, cs, dirFrom))
			ts.UnionWith(setCharacterCanStand(x-1, y, radiusLeft-1, currRadius+1, attackRange, cs, doAttack, minorsUsed + (t.passabilityInDirection(Direction.Left) > 1 ? 1 : 0), Direction.Left));//, tileSet);    //
        if (canPass(Direction.Right, x, y, cs, dirFrom))
			ts.UnionWith(setCharacterCanStand(x+1, y, radiusLeft-1, currRadius+1, attackRange, cs, doAttack, minorsUsed + (t.passabilityInDirection(Direction.Right) > 1 ? 1 : 0), Direction.Right));//, tileSet);  //
        if (canPass(Direction.Up, x, y, cs, dirFrom))   
			ts.UnionWith(setCharacterCanStand(x, y-1, radiusLeft-1, currRadius+1, attackRange, cs, doAttack, minorsUsed + (t.passabilityInDirection(Direction.Up) > 1 ? 1 : 0), Direction.Up));//, tileSet);        //
        if (canPass(Direction.Down, x, y, cs, dirFrom))
			ts.UnionWith(setCharacterCanStand(x, y+1, radiusLeft-1, currRadius+1, attackRange, cs, doAttack, minorsUsed + (t.passabilityInDirection(Direction.Down) > 1 ? 1 : 0), Direction.Down));//, tileSet);    //
		return ts;
	}

	public void setCharacterCanAttack(int x, int y, int radiusLeft, int currRadius, Unit cs, VisibilityMode vis = VisibilityMode.None) {
	if (x < 0 || y < 0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		for (int n=Mathf.Max(0, x - radiusLeft); n <= Mathf.Min(actualWidth - 1, x + radiusLeft); n++) {
			for (int m=Mathf.Max(0, y - radiusLeft); m <= Mathf.Min(actualHeight - 1, y + radiusLeft);m++) {
				if (n == x && m == y) continue;
				Tile t2 = tiles[n,m];
				if (t2.canAttackCurr) continue;
				if (t2.standable && hasLineOfSight(t, t2, radiusLeft, true, (vis == VisibilityMode.None ? cs.attackVisibilityMode() : vis))) {
					t2.canAttackCurr = true;
				}
			}
		}
		/*
		if (t.canStandCurr && currRadius != 0) return;
		if (t.canAttackCurr && t.minAttackCurr <= currRadius) return;
		if (t.standable) {
			t.canAttackCurr = true;
			t.minAttackCurr = currRadius;
		}
	//	Debug.Log("can attack: " + x + ", " + y);
		if (radiusLeft == 0) return;
		if (canAttack(Direction.Left, x, y, cs))
			setCharacterCanAttack(x-1,y,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Right, x, y, cs))
			setCharacterCanAttack(x+1,y,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Up, x, y, cs))
			setCharacterCanAttack(x,y-1,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Down, x, y, cs))
			setCharacterCanAttack(x,y+1,radiusLeft-1,currRadius+1, cs);
			*/
	}

	public void setCharacterCanPlaceTurret(int x, int y, int radiusLeft, int currRadius, Unit cs) {
		if (x <0 || y <0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canUseSpecialCurr && t.minSpecialCurr <= currRadius) return;
		if (t.standable && !t.hasCharacter()) {
			t.canUseSpecialCurr = true;
			t.minSpecialCurr = currRadius;
		}
		if (radiusLeft == 0) return;
		//if (canmo(Direction.Left, x, y, cs))
		if (passibility(Direction.Left, x, y)>=1)
			setCharacterCanPlaceTurret(x-1,y,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Right, x, y)>=1)
			setCharacterCanPlaceTurret(x+1,y,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Up, x, y)>=1)
			setCharacterCanPlaceTurret(x,y-1,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Down, x, y)>=1)
			setCharacterCanPlaceTurret(x,y+1,radiusLeft-1,currRadius+1, cs);
	}

	public void setCharacterCanLayTrap(int x, int y, int radiusLeft, int currRadius, Unit cs) {
		if (x <0 || y <0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canUseSpecialCurr && t.minSpecialCurr <= currRadius) return;
		if (t.standable && !t.hasCharacter() && !t.hasTrap()) {
			t.canUseSpecialCurr = true;
			t.minSpecialCurr = currRadius;
		}
		if (radiusLeft == 0) return;
		//if (canmo(Direction.Left, x, y, cs))
		if (passibility(Direction.Left, x, y)>=1)
			setCharacterCanLayTrap(x-1,y,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Right, x, y)>=1)
			setCharacterCanLayTrap(x+1,y,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Up, x, y)>=1)
			setCharacterCanLayTrap(x,y-1,radiusLeft-1,currRadius+1, cs);
		if (passibility(Direction.Down, x, y)>=1)
			setCharacterCanLayTrap(x,y+1,radiusLeft-1,currRadius+1, cs);
	}

	public void setTurretDirectionAttack(int x, int y, int radiusLeft, Direction dir, Unit cs) {
		if (x <0 || y <0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canAttackCurr) t.attackFromTurret = false;
		else if (t.getCharacter()!=cs) {
			t.attackFromTurret = true;
			t.canAttackCurr = true;
		}
		if (radiusLeft == 0) return;
		int oldX = x;
		int oldY = y;
		switch (dir) {
		case Direction.Left:
			x--;
			break;
		case Direction.Right:
			x++;
			break;
		case Direction.Up:
			y--;
			break;
		case Direction.Down:
			y++;
			break;
		default:
			break;
		}
		if (passibility(dir, oldX, oldY)>0)
			setTurretDirectionAttack(x, y, radiusLeft-1, dir, cs);
	}

	public void unsetTurretDirectionAttack(int x, int y, int radiusLeft, Direction dir, Unit cs) {
		if (x <0 || y <0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.attackFromTurret) {
			t.attackFromTurret = false;
			t.canAttackCurr = false;
		}
		if (radiusLeft==0) return;
		int oldX = x;
		int oldY = y;
		switch (dir) {
		case Direction.Left:
			x--;
			break;
		case Direction.Right:
			x++;
			break;
		case Direction.Up:
			y--;
			break;
		case Direction.Down:
			y++;
			break;
		default:
			break;
		}
		if (passibility(dir, oldX, oldY)>0)
			unsetTurretDirectionAttack(x, y, radiusLeft-1, dir, cs);
	}

	public void setTrapPlacementRange(int x, int y, int radiusLeft) {
		if (x <0 || y <0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
//		Debug.Log(t.hasCharacter() + " " + t.hasTrap() + "  " + (!t.hasTrap() || t.getTrap().fullTrap != currentTrap));
		if (t.canUseSpecialCurr || t.hasCharacter() || (t.hasTrap() && t.getTrap().fullTrap != currentTrap)) return;
		if (currentTrap.Count>=GameGUI.selectedTrap.getMaxSize() && !t.hasTrap()) return;
		t.canUseSpecialCurr = true;
		if (radiusLeft==0 && !t.getTrap()) return;
		if (passibility(Direction.Left, x, y)>=1)
			setTrapPlacementRange(x-1,y,(t.getTile(Direction.Left).getTrap()?1:radiusLeft-1));
		if (passibility(Direction.Right, x, y)>=1)
			setTrapPlacementRange(x+1,y,(t.getTile(Direction.Right).getTrap()?1:radiusLeft-1));
		if (passibility(Direction.Up, x, y)>=1)
			setTrapPlacementRange(x,y-1,(t.getTile(Direction.Up).getTrap()?1:radiusLeft-1));
		if (passibility(Direction.Down, x, y)>=1)
			setTrapPlacementRange(x,y+1,(t.getTile(Direction.Down).getTrap()?1:radiusLeft-1));
	}
/*
	bool isInPlayerRadius(Player player1, int radius, int x, int y) {
		return Mathf.Abs(player1.position.x - x) + Mathf.Abs(-player1.position.y - y) <= radius;
	}
	
	bool isInCircularRadius(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) < Mathf.Pow(radius,2);
	}
	
	bool isInCircularRadius2(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) <= Mathf.Pow(radius,2);
	}
	
	bool isInCircularRadius3(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) - 2 <= Mathf.Pow(radius,2);
	}*/
	
	int totalMoveDist(Unit cs, int x, int y) {
		return (int)Mathf.Abs(cs.position.x - x) + (int)Mathf.Abs(-cs.position.y  - y);
	}
	void loadGrid(int x, int y) {
		gridArray = new GameObject[x,y];
		int xcur = 0;
		for (float n=0;n<x;n++) {
			int ycur = 0;
			for (float m=0;m<y;m++) {
				GameObject go = (GameObject)Instantiate(gridPrefab);
				gridArray[(int)n,(int)m] = go;
				go.transform.position = new Vector3(n,m*-1,0.0f);//-gridOrder/renderThing);
				go.transform.parent = grids.transform;
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				sr.sortingOrder = gridOrder;
				sr.color = Color.clear;
				ycur++;
			}
			xcur++;
		}
	}
	
	public bool currentUnitIsAI() {
		return getCurrentUnit() != null && unitIsAI(getCurrentUnit()) && !isInCharacterPlacement();
	}

	public bool unitIsAI(Unit u) {
		return !u.playerControlled;
	}

	public bool currentUnitIsPrimal() {
		return getCurrentUnit() != null && unitIsPrimal(getCurrentUnit()) && !isInCharacterPlacement();
	}

	public bool unitIsPrimal(Unit u) {
		return u.inPrimal;
	}

	void handleInput() {
		handleKeys();
		if (GameGUI.escapeMenuOpen) return;
		if (currentUnitIsPrimal()) {
			getCurrentUnit().performPrimal();
			return;
		}
		if (currentUnitIsAI()) {
			getCurrentUnit().performAI();
			return;
		}
		handleGUIPos();
		handleMouseScrollWheel();

		handleMouseButtons();
		handleKeyPan();
		handleMouseClicks();
		handleMouseSelect();
		handleMouseMovement();
		if (mouseDown && UnitGUI.inventoryOpen) {
			if (BattleGUI.battleGUI.overlayObjects.Count > 0) {
				BattleGUI.selectItem();
			}
		}
		if (mouseUp && UnitGUI.inventoryOpen) {
			BattleGUI.deselectItem();
		}
	}
	

	void handleGUIPos() {
	//	isOnGUI = GameGUI.moveButtonRect().Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		isOnGUI = /*GameGUI.mouseIsOnGUI() || */ GameObject.Find("EventSystem").GetComponent<EventSystem>().IsPointerOverGameObject();
	}
	
	
	void handleMouseScrollWheel() {
		//	if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
		Camera.main.orthographicSize = Screen.height / 128.0f;
	}
	
	
	
	void handleKeys() {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		escapeDown = Input.GetKey(KeyCode.Escape);
		spaceDown = Input.GetKey(KeyCode.Space);
		commandDown = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
		if (shiftDown && altDown && (controlDown || commandDown)) {
			if (Input.GetKeyDown(KeyCode.W)) {
				setGameState(GameState.Won);
			}
			else if (Input.GetKeyDown(KeyCode.L)) {
				setGameState(GameState.Lost);
			}
			else if (Input.GetKeyDown(KeyCode.K)) {
				getCurrentUnit().damage(1000,null);
			}
			else if (Input.GetKeyDown(KeyCode.H)) {
				getCurrentUnit().gainHealth(1);
			}
		}
	/*	if (Input.GetKeyDown(KeyCode.Alpha7)) {
			setOverlay();
		}*/
		if (Input.GetKeyDown(KeyCode.R)) {
//			GameGUI.clickTab(Tab.R);
			GameGUI.clipboardTab = Tab.R;
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			UnitGUI.clickTab(Tab.C);
		}
		if (Input.GetKeyDown(KeyCode.V)) {
			UnitGUI.clickTab(Tab.V);
		}
		if (Input.GetKeyDown(KeyCode.B)) {
			UnitGUI.clickTab(Tab.B);
		}
		/*
		if (Input.GetKeyDown(KeyCode.R)) {
			GameGUI.clickStandard();
	//		GameGUI.openTab = (GameGUI.openTab==Tab.T ? Tab.None : Tab.T);
		}*/
		if (Input.GetKeyDown(KeyCode.L)) {
			GameGUI.selectMinor(MinorType.Loot);
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			if (getCurrentUnit().getStandardTypes().Contains(StandardType.Place_Turret))
				GameGUI.selectStandard(StandardType.Place_Turret);
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			GameGUI.clickMovement();
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			GameGUI.clipboardTab = Tab.T;
		}
		if (Input.GetKeyDown(KeyCode.Q)) {
			GameGUI.clickWait();
		}
		/*
		if (Input.GetKeyDown(KeyCode.B)) {
			GameGUI.selectMovement(MovementType.BackStep);
		}
		*/
	//	if (Input.GetKeyDown(KeyCode.V)) {
	//		GameGUI.selectMovement(MovementType.Move);
	//	}
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			GameGUI.selectActionAt(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			GameGUI.selectActionAt(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			GameGUI.selectActionAt(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			GameGUI.selectActionAt(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5)) {
			GameGUI.selectActionAt(4);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6)) {
			GameGUI.selectActionAt(5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7)) {
			GameGUI.selectActionAt(6);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8)) {
			GameGUI.selectActionAt(7);
		}
		if (Input.GetKeyDown(KeyCode.Alpha9)) {
			GameGUI.selectActionAt(8);
		}
		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			GameGUI.selectActionAt(9);
		}
		if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) {
			deleteCurrentTrap();
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {// && !normalDraggin && !shiftDraggin) {
			if (UnitGUI.inventoryOpen) {
				UnitGUI.inventoryOpen = false;
			}
			else if (RightClickMenu.shown) {
				RightClickMenu.hideMenu();
			}
			else {
				if (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Lay_Trap || GameGUI.selectedStandardType == StandardType.Place_Turret) && GameGUI.selectedTrapTurret) {
				/*	if (GameGUI.selectedStandardType==StandardType.Lay_Trap && GameGUI.selectedTrap != null) {
						GameGUI.selectedTrap = null;
						resetRanges();
					}
					else GameGUI.selectStandard(GameGUI.selectedStandardType);*/
					GameGUI.selectedTrapTurret = false;
					resetRanges();
					BattleGUI.hideTurretSelect(false, true);
				}
			/*	else if (GameGUI.selectedMovement && GameGUI.selectedMovementType!=MovementType.None) {
					GameGUI.selectMovement(GameGUI.selectedMovementType);
				}
				else if (GameGUI.selectedMinor && GameGUI.selectedMinorType != MinorType.None) {
					GameGUI.selectMinor(GameGUI.selectedMinorType);
				}*/
		//		else if (selectedUnit == null) {
		//			openEscapeMenu();
//					selectUnit(getCurrentUnit(),false);
		//		}
				else {
		//			deselectAllUnits();
					openEscapeMenu();
				}
			}
//			else UnitGUI.openTab = Tab.None;
		}
		if (leftClickIsMakingSelection()) {// && !shiftDown) {
			if (Input.GetKeyDown(KeyCode.W)) {
				handleKeyInput(Direction.Up);
			}
			if (Input.GetKeyDown(KeyCode.S)) {
				handleKeyInput(Direction.Down);
			}
			if (Input.GetKeyDown(KeyCode.A)) {
				handleKeyInput(Direction.Left);
			}
			if (Input.GetKeyDown(KeyCode.D)) {
				handleKeyInput(Direction.Right);
			}
			if (Input.GetKeyUp(KeyCode.W)) {
				handleKeyUpInput(Direction.Up);
			}
			if (Input.GetKeyUp(KeyCode.S)) {
				handleKeyUpInput(Direction.Down);
			}
			if (Input.GetKeyUp(KeyCode.A)) {
				handleKeyUpInput(Direction.Left);
			}
			if (Input.GetKeyUp(KeyCode.D)) {
				handleKeyUpInput(Direction.Right);
			}
		}
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
			performAction();
		}
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (shiftDown) {
				GameGUI.selectPreviousAction();//.selectPreviousOfType();
			}
			else {
				GameGUI.selectNextAction();//OfType();
			}
		}
		if (Input.GetKeyDown(KeyCode.Semicolon) && shiftDown) {
			getCurrentUnit().minorsLeft += 2;
			BattleGUI.showMinorButtons();//.hideMinorArm(false);
		}
		if (Input.GetKeyDown(KeyCode.Quote) && shiftDown) {
			getCurrentUnit().moveDistLeft = 5;
			getCurrentUnit().usedMovement = false;
			BattleGUI.showMovementButtons();//.hideMovementArm(false);
		}
		if (Input.GetKeyDown(KeyCode.LeftBracket) && shiftDown) {
			getCurrentUnit().usedStandard = false;
			BattleGUI.showStandardButtons();//.hideStandardArm(false);
		}

		handleArrows();
		handleSpace();
	}

	public void openEscapeMenu() {
		BattleGUI.hitEscape();
	}

	public void deleteCurrentTrap() {
		if (currentlySelectedTrap!=null) {
			Tile t = currentKeysTile;
			bool first = currentTrap[0]==currentlySelectedTrap;
			if (t!=null) t.removeTrap();
			currentTrap.Remove(currentlySelectedTrap);
			Destroy(currentlySelectedTrap.gameObject);
			currentlySelectedTrap = null;
			if (currentTrap.Count==0) {
				resetCurrentKeysTile();
			}
			else {
				List<TrapUnit> trs = new List<TrapUnit>();
				if (!first) {
					trs.Add(currentTrap[0]);
					currentTrap.RemoveAt(0);
				}
				for (int n=0;n<trs.Count && currentTrap.Count > 0 && !first;n++) {
					TrapUnit currTr = trs[n];
					Tile til = tiles[(int)currTr.position.x,(int)-currTr.position.y];
					foreach (Direction d in Tile.directions) {
						Tile tDir = til.getTile(d);
						TrapUnit unitTr = tDir.getTrap();
						if (unitTr != null) {
							if (unitTr.fullTrap==currentTrap && currentTrap.Contains(unitTr)) {
								currentTrap.Remove(unitTr);
								trs.Add(unitTr);
							}
						}
					}
				}
				while (currentTrap.Count>0) {
					Tile tt = tiles[(int)currentTrap[0].position.x,(int)-currentTrap[0].position.y];
					tt.removeTrap();
					Destroy(currentTrap[0].gameObject);
					currentTrap.RemoveAt(0);
				}
				while (trs.Count > 0) {
					currentTrap.Add(trs[0]);
					trs.RemoveAt(0);
				}
				if (first) {
					resetCurrentKeysTile();
				}
				else {
				foreach (Direction d in Tile.directions) {
					Tile tDir = t.getTile(d);
					if (tDir.hasTrap() && tDir.getTrap().fullTrap==currentTrap) {
						currentlySelectedTrap = tDir.getTrap();
						currentlySelectedTrap.setSelectedForPlacement();
						currentKeysTile = tDir;
						break;
					}
				}
				}
			}
			resetRanges(false);
		}
	}

	public Direction turretBeingPlacedInDirection = Direction.None;
	public TurretUnit turretBeingPlaced = null;
	public List<Direction> turretPlaceDirections;

	public void performAction() {
		if (performingAction() || currentUnitIsAI() || !leftClickIsMakingSelection()) return;
		Unit p = selectedUnit;
		if (((GameGUI.selectedMovement && (GameGUI.selectedMovementType == MovementType.BackStep || GameGUI.selectedMovementType == MovementType.Move)) || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) && getCurrentUnit().currentPath.Count > 1) {
			if (lastPlayerPath.Count > 1 && !p.moving) {
				bool changed = false;
				for (int n=selectedUnit.currentPath.Count-1;n>=1;n--) {
					Vector2 v = (Vector2)selectedUnit.currentPath[n];
					if (!tiles[(int)v.x,(int)v.y].canStand()) {
						changed = true;
						selectedUnit.currentPath.RemoveAt(n);
						selectedUnit.setPathCount();
					}
					else {
						break;
					}
				}
				if (changed) {
					resetPlayerPath();
					lastPlayerPath = selectedUnit.currentPath;
					setPlayerPath(lastPlayerPath);
					Vector2 v = lastPlayerPath[lastPlayerPath.Count-1];
					currentKeysTile = tiles[(int)v.x,(int)v.y];
					return;
				}
				p.startMoving((GameGUI.selectedMovement && GameGUI.selectedMovementType == MovementType.BackStep) || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape));
					//		p.attacking = true;
			}
		}

		
		else if (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.Intimidate) && getCurrentUnit().attackEnemy != null) {
			if (GameGUI.selectedStandardType == StandardType.Attack) {
				p.startAttacking();
			}
			else if (GameGUI.selectedStandardType == StandardType.OverClock) {
				p.startAttacking(true);
			}
			else if (GameGUI.selectedStandardType == StandardType.Throw) {
				p.startThrowing();
			}
			else if (GameGUI.selectedStandardType == StandardType.Intimidate) {
				p.startIntimidating();
			}
		}

		else if (GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.Place_Turret) {
			if (turretBeingPlaced != null) {
			//	Turret turret = GameGUI.getCurrentTurret();
			//	turretBeingPlaced.turret = turret;
				getCurrentUnit().addTurret(turretBeingPlaced);
				if (GameGUI.selectedTurret != null) getCurrentUnit().characterSheet.characterSheet.inventory.removeItem(GameGUI.selectedTurret);
				turretBeingPlacedInDirection = Direction.None;
				turretBeingPlaced = null;
				getCurrentUnit().useMovementIfStarted();
				getCurrentUnit().useStandard();
			}
		}
		else if (GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.Lay_Trap) {
			if (!GameGUI.selectedTrapTurret) {
				GameGUI.selectedTrapTurret = true;
				resetRanges();
			}
			else {
				if (currentTrap.Count > 0) {
					getCurrentUnit().characterSheet.characterSheet.inventory.removeItem(currentTrap[0].trap);
					while (currentTrap.Count > 0) currentTrap.RemoveAt(0);
					if (currentlySelectedTrap != null) {
						currentlySelectedTrap.unsetSelectedForPlacement();
						currentlySelectedTrap = null;
					}
					getCurrentUnit().useMovementIfStarted();
					getCurrentUnit().useStandard();
					currentTrap = new List<TrapUnit>();
				}
			}
		}

		else if (GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.InstillParanoia) {
			p.startInstillingParanoia();
			//		resetRanges();
		}
		else if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Mark) {
			p.markUnit();
	//		resetRanges();
		}
		else if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Invoke) {
			p.startInvoking();
	//		resetRanges();
		}
		else if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Stealth) {
			p.rollStealth();
		}
		else if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.TemperedHands) {
			GameGUI.useTemperedHands();
		}
		else if (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.OneOfMany) {
			p.useOneOfMany(OneOfManyMode.Hidden);
		}
	}

	public bool directionsAreOpposite(Direction dir, Direction dir2) {
		if (dir == Direction.Down && dir2 == Direction.Up) return true;
		if (dir == Direction.Up && dir2 == Direction.Down) return true;
		if (dir == Direction.Right && dir2 == Direction.Left) return true;
		if (dir == Direction.Left && dir2 == Direction.Right) return true;
		return false;
	}

	public static Direction getDirectionOfTile(Tile from, Tile to) {
		foreach (Direction dir in Tile.directions) {
			if (from.getTile(dir)==to) return dir;
		}
		return Direction.None;
	}

	public void handleKeyUpInput(Direction dir) {
		if (turretBeingPlacedInDirection==dir) {
			turretBeingPlacedInDirection = Direction.None;
			turretPlaceDirections = null;
		}
		else {
			if (turretPlaceDirections != null && turretPlaceDirections.Contains(dir)) {
				turretPlaceDirections.Remove(dir);
				if (turretPlaceDirections.Count > 0) {
					handleKeyInput(turretPlaceDirections[turretPlaceDirections.Count-1]);
				}
				else handleKeyInput(turretBeingPlacedInDirection);
			}
		//	if (turretDirectionIsOnlyKey()) handleKeyInput(turretBeingPlacedInDirection);
		}
	}

	public bool turretDirectionIsOnlyKey() {
		if (turretBeingPlacedInDirection==Direction.None) return false;
		int num = 0;
		if (Input.GetKey(KeyCode.W)) num++;
		if (Input.GetKey(KeyCode.S)) num++;
		if (Input.GetKey(KeyCode.D)) num++;
		if (Input.GetKey(KeyCode.A)) num++;
		return num == 1;
	}

	public void handleKeyInput(Direction dir) {
		if (performingAction()) return;
		Debug.Log("Direction: " + dir);
		Tile t = null;
		if (turretBeingPlacedInDirection != Direction.None) {
			Debug.Log("Turret it is!");
			TurretUnit tur = turretBeingPlaced;
			if (tur != null) {
				int x = (int)tur.position.x;
				int y = (int)-tur.position.y;
				unsetTurretDirectionAttack(x, y, 5, tur.direction, tur);
				tur.setDirection(dir);
				if (!turretPlaceDirections.Contains(dir)) turretPlaceDirections.Add(dir);
				setTurretDirectionAttack(x, y, 5, dir, tur);
				drawAllRanges();
			}
		}
		else if (currentKeysSize==1) {
			Direction oldDirection = getDirectionOfTile(currentUnitTile, currentKeysTile);
			Tile oldTile = currentKeysTile;
			t = currentUnitTile.getTile(dir);
			if ((GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) && !t.canStandCurr) t = null;
			if (GameGUI.selectedStandard && !t.canAttackCurr && !t.canUseSpecialCurr) t = null;
			if ((GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) && currentKeysTile.getTile(dir)==currentUnitTile) t = currentUnitTile;
			if (t==null) {
	//			Debug.Log("null");
				if (directionsAreOpposite(dir,oldDirection)) {
					currentKeysTile = currentUnitTile;
				}
			}
			else {
	//			Debug.Log(t.getPosition() + "  " + currentKeysTile.getPosition() + "   " + currentUnitTile.getPosition());
				currentKeysTile = t;
			}
			if (GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Place_Turret) {
				if (oldTile != null) {
					Unit u = oldTile.getCharacter();
					if (u!=null && u == turretBeingPlaced) {
			//			TurretUnit tur = u as TurretUnit;
					//	unsetTurretDirectionAttack((int)tur.position.x, (int)-tur.position.y, 5, tur.direction, tur);
						oldTile.removeCharacter();
//						Destroy(u.gameObject);
//						drawAllRanges();
					}
				}
				if (turretBeingPlaced != null) {
					unsetTurretDirectionAttack((int)turretBeingPlaced.position.x, (int)-turretBeingPlaced.position.y, 5, turretBeingPlaced.direction, turretBeingPlaced);
					if (t==null) {
						Destroy(turretBeingPlaced.gameObject);
						turretBeingPlaced = null;
						turretBeingPlacedInDirection = Direction.None;
					}
					drawAllRanges();
				}
			}
			handleKeyAction(t, dir);
		}
		else {
			t = currentKeysTile;
			do {
				t = t.getTile(dir);
			} while (t != null && ((GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) ? !t.standable : false));
			if (t==null || ((GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) ? !t.canStandCurr : !t.canUseSpecialCurr && !t.canAttackCurr)) return;
			currentKeysTile = t;
			handleKeyAction(t, dir);
		}
	}
	public void logPoop() {
		Debug.Log("Poop");
	}
	public void handleKeyAction(Tile t, Direction dir) {
		Unit u = getCurrentUnit();
		if (GameGUI.selectedStandard) {
			if (GameGUI.selectedStandardType == StandardType.Place_Turret) {
				if (t!=null && t.canUseSpecialCurr && t!=currentUnitTile) {
					GameObject g;
					TurretUnit tu;
					if (turretBeingPlaced != null) {
						tu = turretBeingPlaced;
						g = tu.gameObject;
					}
					else {
						g = Instantiate(turretPrefab) as GameObject;
						g.transform.parent = turrets.transform;
						tu = g.GetComponent<TurretUnit>();
						tu.mapGenerator = this;
						tu.team = getCurrentUnit().team;
						turretBeingPlaced = tu;
						turretBeingPlaced.owner = getCurrentUnit();
						turretBeingPlaced.turret = GameGUI.selectedTurret;
					}
					t.setCharacter(tu);
					Vector2 v = t.getPosition();
					v.y *= -1;
					tu.setPosition(new Vector3(v.x, v.y, 0.0f));
					tu.setDirection(dir);
					setTurretDirectionAttack((int)v.x, (int)-v.y, 5, dir, tu);
					turretBeingPlacedInDirection = dir;
					turretPlaceDirections = new List<Direction>();
					drawAllRanges();
				}
				else currentKeysTile = currentUnitTile;
			}
			else if (GameGUI.selectedStandardType == StandardType.Lay_Trap) {
				if (t!=null && t.canUseSpecialCurr && !t.hasCharacter() && !t.hasTrap()) {
					GameObject g = Instantiate(trapPrefab) as GameObject;
					g.renderer.sortingOrder = trapOrder;
					g.transform.parent = traps.transform;
					TrapUnit tu = g.GetComponent<TrapUnit>();
					tu.owner = getCurrentUnit();
					tu.trap = GameGUI.selectedTrap;
					if (currentlySelectedTrap != null)
						currentlySelectedTrap.unsetSelectedForPlacement();
					currentlySelectedTrap = tu;
					tu.setSelectedForPlacement();
					tu.mapGenerator = this;
					tu.team = getCurrentUnit().team;
					tu.fullTrap = currentTrap;
					currentTrap.Add(tu);
					t.setTrap(tu);
					Vector2 v = t.getPosition();
					v.y *= -1;
					tu.setPosition(new Vector3(v.x, v.y, 0.0f));
					resetRanges(false);
				}
				else if (t!=null && t.canUseSpecialCurr && t.hasTrap()) {
					if (currentlySelectedTrap != null)
						currentlySelectedTrap.unsetSelectedForPlacement();
					currentlySelectedTrap = t.getTrap();
					currentlySelectedTrap.setSelectedForPlacement();
				}}
			else {
				if (selectedUnit.attackEnemy) {
					selectedUnit.attackEnemy.deselect();
					selectedUnit.attackEnemy = null;
				}
				if (t!=null && t.canAttackCurr && t!=currentUnitTile) {
					if (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.Intimidate)
						selectedUnit.attackEnemy = t.getEnemy(selectedUnit);
					else if (GameGUI.selectedStandardType == StandardType.Throw)
						selectedUnit.attackEnemy = t.getCharacter();
					selectedUnit.setRotationToAttackEnemy();
				}
				if (selectedUnit.attackEnemy)
					selectedUnit.attackEnemy.setTarget();
				else currentKeysTile = currentUnitTile;
			}
		}
		else if (GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) {
			resetPlayerPath();
			if (t==null) return;
			Vector2 v = t.getPosition();
			if (!lastPlayerPathContains(v)) {
				lastPlayerPath = selectedUnit.addPathTo(v);
			}
			else {
				lastPlayerPath = selectedUnit.removeFromPathTo(v);
			}
			if (lastPlayerPath.Count > 1)
				setPlayerPath(lastPlayerPath);
			lastArrowPos = v;
		}
	}


	float maxTimeSpace = .25f;
	void handleSpace() {
		timeSinceSpace += Time.deltaTime * Time.timeScale;
		if (Input.GetKeyDown(KeyCode.F) && isInPriority()) {
			if (false && GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Place_Turret) {
				
				if (shiftDown) GameGUI.selectedTurretIndex--;
				else GameGUI.selectedTurretIndex++;
				int numTurrets = getCurrentUnit().characterSheet.characterSheet.inventory.getTurrets().Count;
				GameGUI.selectedTurretIndex += numTurrets;
				GameGUI.selectedTurretIndex %= numTurrets;
				GameGUI.showCurrentTurret();
			}
			else if (false && GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Lay_Trap && GameGUI.selectedTrap==null) {
				if (shiftDown) GameGUI.selectedTrapIndex--;
				else GameGUI.selectedTrapIndex++;
				int numTraps = getCurrentUnit().characterSheet.characterSheet.inventory.getTraps().Count;
				GameGUI.selectedTrapIndex += numTraps;
				GameGUI.selectedTrapIndex %= numTraps;
				GameGUI.showCurrentTrap();
			}
			else {
				if (timeSinceSpace <= maxTimeSpace && isInPriority() || selectedUnit==null) {
					deselectAllUnits();
					selectUnit(getCurrentUnit(),false);
					lastPlayerPath = selectedUnit.currentPath;
				}
				moveCameraToSelected();
				timeSinceSpace = 0.0f;
			}
		}
	}

	void handleArrows() {
		return;
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
			if (selectedUnit) {

				if (selectedUnits.Count == 0) {
					int curr = priorityOrder.IndexOf(selectedUnit);
					curr++;
					curr %= priorityOrder.Count;

					/*
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();*/
					deselectAllUnits();
					selectUnit(priorityOrder[curr], false);
					lastPlayerPath = selectedUnit.currentPath;
				}
			}
			else {
				selectUnit(getCurrentUnit(), false);
			}
			moveCameraToSelected();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {

			if (selectedUnit) {
				if (selectedUnits.Count == 0) {
					int curr = priorityOrder.IndexOf(selectedUnit);
					curr--;
					while (curr < 0) curr += priorityOrder.Count;
					curr %= priorityOrder.Count;
					/*
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();
					selectedUnit = null;*/
					deselectAllUnits();
					selectUnit(priorityOrder[curr], false);
					lastPlayerPath = selectedUnit.currentPath;
				}
			}
			else {
				selectUnit(getCurrentUnit(), false);
			}
			moveCameraToSelected();
		}
	}
	
	bool lastPlayerPathContains(Vector2 v) {
		if (lastPlayerPath==null) return false;
		foreach (Vector2 v2 in lastPlayerPath) {
			if (v2.x == v.x && v2.y == v.y) return true;
		}
		return false;
	}

	bool leftClickIsMakingSelection() {
		return true;
		return selectedUnit == getCurrentUnit() && selectedUnits.Count == 0 && guiSelectionType();
	}

	bool guiSelectionType() {
		return (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.Intimidate || GameGUI.selectedStandardType == StandardType.InstillParanoia || (GameGUI.selectedStandardType==StandardType.Lay_Trap && (GameGUI.selectedTrap!=null || true)) || GameGUI.selectedStandardType==StandardType.Place_Turret)) ||
			(GameGUI.selectedMovement && (GameGUI.selectedMovementType == MovementType.Move || GameGUI.selectedMovementType == MovementType.BackStep)) || 
				performingAction() ||
				(GameGUI.selectedMinor && (GameGUI.selectedMinorType == MinorType.Stealth || (GameGUI.selectedMinorType == MinorType.OneOfMany && GameGUI.oneOfManyConfirm) || GameGUI.selectedMinorType == MinorType.Mark || GameGUI.selectedMinorType == MinorType.Invoke || GameGUI.selectedMinorType == MinorType.Escape || GameGUI.selectedMinorType == MinorType.TemperedHands));
	}


	void handleMouseButtons() {
		wasShiftRightDraggin = shiftDraggin;
		wasRightDraggin = normalDraggin;
		if (escapeDown && !leftClickIsMakingSelection()) {
			if (normalDraggin) rightDragginCancelled = true;
			normalDraggin = false;
			if (shiftDraggin) shiftRightDragginCancelled = true;
			shiftDraggin = false;
		}
		

		mouseLeftDown = Input.GetMouseButton(0);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (Input.touchCount == 2) Debug.Log("Time elapsed: " + (Time.time - tapTime));
		if (!shiftDraggin && (!normalDraggin || Time.time - tapTime <= 0.25f) && !rightDraggin && !shiftRightDraggin) {
		//	Debug.Log("Middle Set: " +   (middleDraggin && mouseMiddleDown) + "   "  + (!isOnGUI && Input.GetMouseButtonDown(2)) 
			middleDraggin = (middleDraggin && (mouseMiddleDown || (mouseLeftDown && Input.touchCount == 2))) || ((!isOnGUI || isOnGUI) && Input.GetMouseButtonDown(2)) || ((!isOnGUI || isOnGUI) && (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && normalDraggin)) && (Input.touchCount >= 2 && (oldTouchCount == 2 || true)));
			normalDraggin = normalDraggin && !middleDraggin;
		}
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) shiftDraggin = ((shiftDraggin && mouseLeftDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(0)));
		if (!shiftDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) {
			if (normalDraggin && mouseLeftDown) {
				normalDraggin = true;
			}
			else if (!isOnGUI && !shiftDown && Input.GetMouseButtonDown(0)) {
				normalDraggin = true;
				tapTime = Time.time;
			}
			else normalDraggin = false;
		}
		mouseRightDown = Input.GetMouseButton(1);
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftDraggin) shiftRightDraggin = ((shiftRightDraggin && mouseRightDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(1)));
		if (!shiftDraggin && !middleDraggin && !normalDraggin && !shiftRightDraggin) rightDraggin = (rightDraggin && mouseRightDown) || (!shiftDown && Input.GetMouseButtonDown(1));
		mouseDown = Input.GetMouseButtonDown(0) && Input.touchCount != 2;
		mouseUp = Input.GetMouseButtonUp(0) && Input.touchCount != 2;
		mouseDownRight = Input.GetMouseButtonDown(1) && Input.touchCount != 2;
	//	if (mouseDown || mouseUp)
	//		Debug.Log("MouseDown: " + mouseDown + " MouseUp: " + mouseUp);
		if (mouseDown) mouseDownGUI = isOnGUI;
//		oldTouchCount = Input.touchCount;
	}

	public bool performingAction() {
		if (getCurrentUnit()==null) return false;
		return getCurrentUnit().isPerformingAnAction();
	}

	void handleMouseClicks() {
		if (performingAction()) return;
		handleMouseDown();
		handleMouseUp();
		if (mouseDownRight) RightClickMenu.showMenu();
		if (mouseDown) RightClickMenu.hideMenu();
	}

	public bool isSelectionTile(Tile t2) {
		return t2.canStandCurr || t2.canAttackCurr || t2.canUseSpecialCurr;
	}

	public enum TrapLayType {Add, Delete, None};
	public TrapLayType trapLayType = TrapLayType.None;
	public GameObject selectedSelectionObject = null;
	Vector2 selectedSelectionDiff = new Vector2(0,0);
	void handleMouseDown() {
		Tile t2 = null;
		bool didTrap = false;
		if (currentSprite != null) {
			GameObject go2 = currentSprite.gameObject;
			Transform transform2 = go2.transform;
			t2 = tiles[(int)transform2.localPosition.x,(int)-transform2.localPosition.y];
		}

		if (isOnGUI && mouseDown && !rightDraggin && !middleDraggin && !shiftDraggin) {
			if (UnitGUI.inventoryOpen && selectedUnit != null && selectedUnits.Count==0 && selectedUnit == getCurrentUnit()) {
				UnitGUI.selectItem(selectedUnit.characterSheet, this, selectedUnit);
		//		selectedUnit.selectItem();
			}
		}
		if ((isInCharacterPlacement() && mouseDown && !rightDraggin && !middleDraggin)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<10);
            if (hit)
            {
                GameObject go = hit.collider.gameObject;
                Unit uu = go.GetComponent<Unit>();
                if (uu.team == 0)
                {
			        selectedSelectionObject = go;
                    //				selectedUnit = go.GetComponent<Unit>();
                    //				selectedUnit.setSelected();
                    deselectAllUnits();
                    selectUnit(uu, false);
                    BattleGUI.setCharacterInfoVisibility(true);
                    BattleGUI.beginTurn(uu);
                    go.transform.parent = playerTransform;
                    go.GetComponent<SpriteRenderer>().sortingOrder = playerSelectSelectedPlayerOrder;
                    uu.setAllSpritesToRenderingOrder(playerSelectSelectedPlayerArmorOrder);
                    Vector3 pos = Input.mousePosition;
                    pos.z = 10.0f;
                    pos = Camera.main.ScreenToWorldPoint(pos);
                    selectedSelectionDiff = new Vector2(pos.x - go.transform.localPosition.x, pos.y - go.transform.localPosition.y);
                    if (lastHit)
                    {
                        int posX = (int)lastHit.transform.localPosition.x;
                        int posY = -(int)lastHit.transform.localPosition.y;
                        Tile t = tiles[posX, posY];
                        if (t.getCharacter() == go.GetComponent<Unit>())
                        {
                            selectionStartingTile = t;
                        }
                        else selectionStartingTile = null;
                    }
                    selectionCurrentIndex = selectionUnits.IndexOf(go.GetComponent<Unit>());
                    if (selectionStartingTile == null)
                    {
                        selectionStartingIndex = selectionCurrentIndex;
                    }
                    selectionUnits.Remove(go.GetComponent<Unit>());
                    selectionStartingPos = go.transform.position;
                }
            }
            else
            {
                BattleGUI.setCharacterInfoVisibility(false);
            }
		}
	
		
		if (mouseDown && !isOnGUI && !rightDraggin) {
			if ((GameGUI.selectedMinor && (GameGUI.selectedMinorType==MinorType.Mark || GameGUI.selectedMinorType==MinorType.Invoke)) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.InstillParanoia || GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.Intimidate || GameGUI.selectedStandardType == StandardType.Place_Turret || GameGUI.selectedStandardType == StandardType.Lay_Trap))) {
				if (lastHit) {
					int posX = (int)lastHit.transform.localPosition.x;
					int posY = -(int)lastHit.transform.localPosition.y;

					if (GameGUI.selectedStandardType == StandardType.Place_Turret) {
						Tile t = tiles[posX,posY];
						if (t!=null && t.canUseSpecialCurr && t!=currentUnitTile && (turretBeingPlaced==null || t.getCharacter() != turretBeingPlaced)) {
							Direction dir = getDirectionOfTile(currentUnitTile,t);
							GameObject g;
							TurretUnit tu;
							if (turretBeingPlaced != null) {
								tu = turretBeingPlaced;
								Tile t22 = tiles[(int)turretBeingPlaced.position.x,(int)-turretBeingPlaced.position.y];
								t22.removeCharacter();
								g = tu.gameObject;
								unsetTurretDirectionAttack((int)turretBeingPlaced.position.x,(int)-turretBeingPlaced.position.y, 5, turretBeingPlaced.direction, turretBeingPlaced);
							}
							else {
								g = Instantiate(turretPrefab) as GameObject;
								g.transform.parent = turrets.transform;
								tu = g.GetComponent<TurretUnit>();
								tu.mapGenerator = this;
								tu.team = getCurrentUnit().team;
								turretBeingPlaced = tu;
								turretBeingPlaced.owner = getCurrentUnit();
								turretBeingPlaced.turret = GameGUI.selectedTurret;
							}
							t.setCharacter(tu);
							Vector2 v = t.getPosition();
							v.y *= -1;
							tu.setPosition(new Vector3(v.x, v.y, 0.0f));
							tu.setDirection(dir);
							setTurretDirectionAttack((int)v.x, (int)-v.y, 5, dir, tu);
							turretBeingPlacedInDirection = Direction.None;
							turretPlaceDirections = new List<Direction>();
							drawAllRanges();
//							currentUnitTile = t;
						}
						else currentKeysTile = currentUnitTile;
					}
					else if (GameGUI.selectedStandardType==StandardType.Lay_Trap) {
						Tile t = tiles[posX,posY];
						if (t != null && t.canUseSpecialCurr) {
							currentKeysTile = t;
							if (t.hasTrap()) {
								if (currentlySelectedTrap != null)
									currentlySelectedTrap.unsetSelectedForPlacement();
								currentlySelectedTrap = t.getTrap();
								deleteCurrentTrap();
								didTrap = true;
								trapLayType = TrapLayType.Delete;
							}
							else {
								GameObject g = Instantiate(trapPrefab) as GameObject;
								g.renderer.sortingOrder = trapOrder;
								g.transform.parent = traps.transform;
								TrapUnit tu = g.GetComponent<TrapUnit>();
								tu.owner = getCurrentUnit();
								tu.trap = GameGUI.selectedTrap;
								if (currentlySelectedTrap != null)
									currentlySelectedTrap.unsetSelectedForPlacement();
								currentlySelectedTrap = tu;
								tu.setSelectedForPlacement();
								tu.mapGenerator = this;
								tu.team = getCurrentUnit().team;
								tu.fullTrap = currentTrap;
								currentTrap.Add(tu);
								t.setTrap(tu);
								Vector2 v = t.getPosition();
								v.y *= -1;
								tu.setPosition(new Vector3(v.x, v.y, 0.0f));
								resetRanges(false);
								didTrap = true;
								trapLayType = TrapLayType.Add;
							}
						}
					}
					else {
						if (selectedUnit.attackEnemy) {
							selectedUnit.attackEnemy.deselect();
							selectedUnit.attackEnemy = null;
						}
						if (tiles[posX,posY].canAttackCurr) {
							if ((GameGUI.selectedMinor && (GameGUI.selectedMinorType==MinorType.Mark || GameGUI.selectedMinorType==MinorType.Invoke)) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.InstillParanoia || GameGUI.selectedStandardType == StandardType.Intimidate)))
								selectedUnit.attackEnemy = tiles[posX,posY].getEnemy(selectedUnit);
							else if (GameGUI.selectedStandardType == StandardType.Throw)
								selectedUnit.attackEnemy = tiles[posX,posY].getCharacter();
							selectedUnit.setRotationToAttackEnemy();
						}
						if (selectedUnit.attackEnemy)
							selectedUnit.attackEnemy.setTarget();
					}
				}
			}
		}
		if (normalDraggin && !mouseDownGUI) {		
			
			int x = -1;
			int y = 1;
			if (currentGrid!=null) {
				x = (int)currentGrid.transform.localPosition.x;
				y = (int)currentGrid.transform.localPosition.y;
			}
			Vector2 v = new Vector2(x, -y);

			
			Tile t = (x >=0 && -y >= 0 ? tiles[x,-y] : null);

			if (selectedUnit && !Unit.vectorsEqual(v, lastArrowPos) && x>=0 && -y>=0 && isSelectionTile(t)) {
				//	Player p = selectedPlayer.GetComponent<Player>();
				//Debug.Log(p.currentMoveDist + "     aaa!!");
				if (GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) {
					resetPlayerPath();
					if (!lastPlayerPathContains(v)) {
						lastPlayerPath = selectedUnit.addPathTo(v);
					}
					else {
						lastPlayerPath = selectedUnit.removeFromPathTo(v);
					}
					if (lastPlayerPath.Count > 1)
						setPlayerPath(lastPlayerPath);
				}
				else if (GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Place_Turret && turretBeingPlaced!=null) {
					Direction dir = Direction.None;
					foreach (Direction direc in Tile.directions) {
						Tile t12 = tiles[(int)turretBeingPlaced.position.x,(int)-turretBeingPlaced.position.y].getTile(direc);
						if (t12!=null && t12==tiles[x,-y]) {	
							unsetTurretDirectionAttack((int)turretBeingPlaced.position.x,(int)-turretBeingPlaced.position.y, 5, turretBeingPlaced.direction, turretBeingPlaced);
							turretBeingPlaced.setDirection(direc);
							setTurretDirectionAttack((int)turretBeingPlaced.position.x,(int)-turretBeingPlaced.position.y, 5, turretBeingPlaced.direction, turretBeingPlaced);
							drawAllRanges();
							break;
						}
					}
				}
				else if (GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Lay_Trap && currentTrap.Count > 0 && !didTrap) {
					if (t != null && t.canUseSpecialCurr) {
						currentKeysTile = t;
						if (!t.hasTrap()) {
							if (trapLayType == TrapLayType.Add) {
								GameObject g = Instantiate(trapPrefab) as GameObject;
								g.renderer.sortingOrder = trapOrder;
								g.transform.parent = traps.transform;
								TrapUnit tu = g.GetComponent<TrapUnit>();
								if (currentlySelectedTrap != null)
									currentlySelectedTrap.unsetSelectedForPlacement();
								currentlySelectedTrap = tu;
								tu.trap = GameGUI.selectedTrap;
								tu.owner = getCurrentUnit();
								tu.setSelectedForPlacement();
								tu.mapGenerator = this;
								tu.team = getCurrentUnit().team;
								tu.fullTrap = currentTrap;
								currentTrap.Add(tu);
								t.setTrap(tu);
								Vector2 v22 = t.getPosition();
								v22.y *= -1;
								tu.setPosition(new Vector3(v22.x, v22.y, 0.0f));
								resetRanges(false);
							}
						}
						else {
							if (currentlySelectedTrap != null)
								currentlySelectedTrap.unsetSelectedForPlacement();
							currentlySelectedTrap = t.getTrap();
							currentlySelectedTrap.setSelectedForPlacement();
							if (trapLayType == TrapLayType.Delete) {
								deleteCurrentTrap();
							}

						}
					}
				}
				lastArrowPos = v;
			}
		}
	}

	Tile selectionStartingTile = null;
	int selectionStartingIndex = -1;
	public int selectionCurrentIndex = -1;
	Vector3 selectionStartingPos;
	void handleMouseUp() {
		if (mouseUp && !rightDraggin && !middleDraggin && !shiftDraggin) {
			if (UnitGUI.inventoryOpen && selectedUnit != null && selectedUnits.Count==0 && selectedUnit == getCurrentUnit()) {
//				selectedUnit.deselectItem();
				UnitGUI.deselectItem(selectedUnit.characterSheet, this, selectedUnit);
			}
		}
		if (mouseUp && isInCharacterPlacement() && !rightDraggin && !middleDraggin) {
			if (selectedSelectionObject) {
				rightDragginCancelled = true;
				int posX = (int)lastHit.transform.localPosition.x;
				int posY = -(int)lastHit.transform.localPosition.y;
				Vector3 mousePos = Input.mousePosition;
				bool overThing = mousePos.x >= Screen.width - selectionWidth;
				Tile t = tiles[posX, posY];
				Unit u2 = selectedSelectionObject.GetComponent<Unit>();
				if (t.startingPoint) {
					if (t.hasCharacter()) {
						Unit u = t.getCharacter();
						if (selectionStartingTile!=null) {
							selectionStartingTile.setCharacter(u);
							u.setPosition(new Vector3(selectionStartingPos.x - 0.5f, selectionStartingPos.y + 0.5f, u.transform.position.z));
						}
						else {
							u.transform.parent = cameraTransform;
							u.GetComponent<SpriteRenderer>().sortingOrder = playerSelectPlayerOrder;
							u.setAllSpritesToRenderingOrder(playerSelectPlayerArmorOrder);
							t.removeCharacter();
//							selectionUnits.Add(u);
//							selectionUnits.Insert(selectionUnits.IndexOf(selectedSelectionObject.GetComponent<Unit>()),u);
							selectionUnits.Insert(0, u);
						}
					}
					u2.setPosition(new Vector3(posX, -posY, u2.transform.position.z));
					t.setCharacter(u2);
					selectionUnits.Remove(u2);
					u2.GetComponent<SpriteRenderer>().sortingOrder = playerNormalOrder;
					u2.setAllSpritesToRenderingOrder(playerArmorOrder);
				}
				else {
					if (selectionStartingTile!=null && !overThing) {
						u2.GetComponent<SpriteRenderer>().sortingOrder = playerNormalOrder;
						u2.setAllSpritesToRenderingOrder(playerArmorOrder);
					}
					else {
						u2.GetComponent<SpriteRenderer>().sortingOrder = playerSelectPlayerOrder;
						u2.setAllSpritesToRenderingOrder(playerSelectPlayerArmorOrder);
						if (selectionStartingTile != null) {
							selectionStartingTile.removeCharacter();
						}
						u2.transform.parent = cameraTransform;
						if (!selectionUnits.Contains(u2)) {
							if (selectionStartingIndex<0 && selectionCurrentIndex<0) {
								selectionUnits.Add(u2);
							}
							else if (selectionCurrentIndex>=0) {
								selectionUnits.Insert(selectionCurrentIndex,u2);
							}
							else {
								selectionUnits.Insert(selectionStartingIndex,u2);
							}
						}
					}
					u2.transform.position = selectionStartingPos;
				}
				selectedSelectionObject = null;
				selectionStartingTile = null;
				selectionStartingIndex = -1;
				selectionCurrentIndex = -1;
			}
		}
		if (mouseUp && !shiftDraggin && !mouseDownGUI && !rightDraggin && !shiftRightDraggin && leftClickIsMakingSelection()) {// && getCurrentUnit()==selectedUnit && selectedUnits.Count == 0) {
			if (lastHit) {
//				selectedUnit.attackEnemy = null;

				int posX = (int)lastHit.transform.localPosition.x;
				int posY = -(int)lastHit.transform.localPosition.y;
				if ((GameGUI.selectedMinor && (GameGUI.selectedMinorType == MinorType.Mark || GameGUI.selectedMinorType==MinorType.Invoke || GameGUI.selectedMinorType == MinorType.Escape)) ||(GameGUI.selectedMovement && (GameGUI.selectedMovementType == MovementType.BackStep || GameGUI.selectedMovementType == MovementType.Move)) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.InstillParanoia || GameGUI.selectedStandardType==StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.Intimidate || GameGUI.selectedStandardType == StandardType.Place_Turret || GameGUI.selectedStandardType == StandardType.Lay_Trap))) {
					if (Time.time - lastClickTime <= doubleClickTime && tiles[posX, posY] == lastClickTile) {
						Debug.Log("performAction()");
						performAction();

					}
				}
				if (GameGUI.selectedMovement || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) {
					bool changed = false;
					for (int n=selectedUnit.currentPath.Count-1;n>=1;n--) {
						Vector2 v = (Vector2)selectedUnit.currentPath[n];
						if (!tiles[(int)v.x,(int)v.y].canStand()) {
							changed = true;
							selectedUnit.currentPath.RemoveAt(n);
							selectedUnit.setPathCount();
						}
						else {
							break;
						}
					}
					if (changed) {
						resetPlayerPath();
						lastPlayerPath = selectedUnit.currentPath;
						setPlayerPath(lastPlayerPath);
					}
					Vector2 v2 = (Vector2)lastPlayerPath[lastPlayerPath.Count-1];
					lastClickTile = tiles[(int)v2.x,(int)v2.y];
					lastClickTime = Time.time;
					currentKeysTile = lastClickTile;
					if (!currentKeysTile.canStandCurr) currentKeysTile = currentUnitTile;
				}
				else if ((GameGUI.selectedMinor && (GameGUI.selectedMinorType == MinorType.Mark || GameGUI.selectedMinorType==MinorType.Invoke)) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.InstillParanoia || GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock || GameGUI.selectedStandardType == StandardType.Intimidate || GameGUI.selectedStandardType == StandardType.Place_Turret))) {
					lastClickTile = tiles[posX, posY];
					lastClickTime = Time.time;
					currentKeysTile = lastClickTile;
					if (!currentKeysTile.canAttackCurr) currentKeysTile = currentUnitTile;
				}

			}
		//	editingPath = false;
			lastArrowPos = new Vector2(-1000, -1000);
		}
	}

	public void removePlayerPath() {
		resetPlayerPath();
//		lastPlayerPath = new ArrayList();
		selectedUnit.resetPath();
		lastPlayerPath = selectedUnit.currentPath;
	}
	
	public void selectUnit(Unit u, bool remove) {
		return;
		resetCurrentKeysTile();
		if (u) {
			if (!selectedUnit) {
				selectedUnit = u;
//				setAroundCharacter(u);
				addCharacterRange(u);
				u.setSelected();
			}
			else {
				if (selectedUnits.Count == 0) {
					removePlayerPath();
					selectedUnit.attackEnemy = null;
					lastPlayerPath = selectedUnit.currentPath;
				}
				if (selectedUnit == u) {
					if (remove) {
						selectedUnit.deselect();
						selectedUnit = null;
						if (selectedUnits.Count != 0) {
							selectedUnit = selectedUnits[0];
							selectedUnits.RemoveAt(0);
							lastPlayerPath = selectedUnit.currentPath;
						}
//						resetCharacterRange();
						resetRanges();
					}
				}
				else if (selectedUnits.Contains(u)) {
					if (remove) {
						u.deselect();
						selectedUnits.Remove(u);
					//	resetCharacterRange();
						resetRanges();
					}
				}
				else {
					if (u != getCurrentUnit())
						selectedUnits.Add(u);
					else {
						selectedUnits.Add(selectedUnit);
						selectedUnit = u;
						lastPlayerPath = selectedUnit.currentPath;
					}
//					setAroundCharacter(u);
					addCharacterRange(u);
					u.setSelected();
				}
			}
		}
	}

	public void deselectAllUnits() {
		return;
		if (selectedUnit) {
		//	resetAroundCharacter(selectedUnit);
			resetPlayerPath();
			lastPlayerPath = new List<Vector2>();
			selectedUnit.resetPath();
			if (selectedUnit.attackEnemy != null) {
				selectedUnit.attackEnemy.deselect();
				selectedUnit.attackEnemy = null;
			}
			selectedUnit.deselect();
			selectedUnit = null;
		}
		Unit u;
		while (selectedUnits.Count != 0) {
			u = selectedUnits[0];
		//	resetAroundCharacter(u);
			u.deselect();
			selectedUnits.RemoveAt(0);
		}
		removeAllRanges();
	}

	public void drawAllRanges() {
		drawRanges(0, 0, actualWidth, actualHeight);
	}

	public void drawRanges(int minX, int minY, int maxX, int maxY) {
		setCurrentSpriteColor();
		for (int x = minX; x < maxX; x++) {
			for (int y=minY; y < maxY; y++) {
				Tile t = tiles[x,y];
				SpriteRenderer sr = gridArray[x, y].GetComponent<SpriteRenderer>();
				setSpriteColor(t, sr);
			}
		}
	}

	public void resetAttack() {
		resetAttack(getCurrentUnit());
	}

	public void resetAttack(Unit u) {
		if (u.attackEnemy && u.attackEnemy.isTarget) {
			u.attackEnemy.deselect();
		}
		u.attackEnemy = null;
	}

	public void resetRanges(bool keys = true) {
		if (keys) resetCurrentKeysTile();
		removeAllRanges(false);
		if (!isInCharacterPlacement() && GameGUI.selectedStandard && GameGUI.selectedStandardType==StandardType.Lay_Trap && currentTrap.Count!=0 && GameGUI.selectedTrap!=null && GameGUI.selectedTrapTurret) {
			setTrapPlacementRange((int)currentTrap[0].position.x, (int)-currentTrap[0].position.y, 1);
		}
		else if (selectedUnit) {
			addCharacterRange(selectedUnit, false);
			foreach (Unit u in selectedUnits) {
				addCharacterRange(u, false);
			}
		}
		drawAllRanges();
	//	}
	}

	public void removeAllRanges() {
		removeAllRanges(true);
	}

	public void removeAllRanges(bool draw) {
		foreach (Tile t in tiles) {
			t.resetStandability();
		}
		if (draw) drawAllRanges();
	}
	public void addCharacterRange(Unit u) {
		addCharacterRange(u, true);
	}
	
	public void setMarkRange(Unit u, bool draw) {
		List<Unit> units = u.lineOfSightUnits();
		foreach (Unit e in units) {
			if (e.isMarked) continue;
			Tile t = tiles[(int)e.position.x,(int)-e.position.y];
			t.canAttackCurr = true;
		}
		if (draw) drawAllRanges();
	}
	
	public void setOratorRange(Unit u, bool draw, bool paranoia) {
		List<Unit> units = u.lineOfSightUnits(5, true);
		foreach (Unit e in units) {
			if (paranoia && e.paranoidOfUnits.Contains(u)) continue;
			Tile t = tiles[(int)e.position.x,(int)-e.position.y];
			t.canAttackCurr = true;
		}
		if (draw) drawAllRanges();
	}

	public void addCharacterRange(Unit u, bool draw) {
		bool isOther = selectedUnit != getCurrentUnit() || selectedUnits.Count > 0;
		if ((GameGUI.showMovement && isOther) || (((GameGUI.selectedMovement && (GameGUI.selectedMovementType == MovementType.Move || GameGUI.selectedMovementType == MovementType.BackStep)) || (GameGUI.selectedMinor && GameGUI.selectedMinorType == MinorType.Escape)) && !isOther))
			setAroundCharacter(u);
		else if ((GameGUI.showAttack && isOther) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Attack || GameGUI.selectedStandardType == StandardType.OverClock) && !isOther))
			setCharacterCanAttack((int)u.position.x, (int)-u.position.y, u.getAttackRange(),0, u);
		else if ((GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.Throw || GameGUI.selectedStandardType == StandardType.Intimidate) && !isOther))
			setCharacterCanAttack((int)u.position.x, (int)-u.position.y, 1, 0, u, VisibilityMode.Melee);
		else if ((GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.Place_Turret) && !isOther)
			setCharacterCanPlaceTurret((int)u.position.x, (int)-u.position.y, 1, 0, u);
		else if ((GameGUI.selectedStandard && GameGUI.selectedStandardType == StandardType.Lay_Trap && GameGUI.selectedTrap!=null && GameGUI.selectedTrapTurret) && !isOther)
			setCharacterCanLayTrap((int)u.position.x, (int)-u.position.y, 1, 0, u);
		else if (GameGUI.selectedMinor && (GameGUI.selectedMinorType==MinorType.Mark) && !isOther)
			setMarkRange(getCurrentUnit(), false);
		else if (((GameGUI.selectedMinor && (GameGUI.selectedMinorType == MinorType.Invoke)) || (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.InstillParanoia))) && !isOther)
			setOratorRange(getCurrentUnit(), false, (GameGUI.selectedStandard && (GameGUI.selectedStandardType == StandardType.InstillParanoia)));
		if (draw) drawAllRanges();
	}

	public void resetMoveDistances() {
	//	bool reset = true;
	//	foreach (GameObject player in players) {
	//		Player p = player.GetComponent<Player>();
	//		if (p.currentMoveDist!=0) reset = false;
	//	}
//		foreach (Unit character in priorityOrder) {
		///	Player p = player.GetComponent<Player>();
//			if (reset) p.currentMoveDist = p.maxMoveDist;
		//	character.setMoveDist(character.maxMoveDist);
//		}
		
	}


	void handleCharacterPlacementMovement() {
		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
		if (selectedSelectionObject) {
		//	Vector3 pos = Camera.main.WorldToScreenPoint(selectedSelectionObject.transform.position);
		//	pos.x += mouseMovement.x;
		//	pos.y += mouseMovement.y;
		//	pos.z = 10.0f;
		//	pos = Camera.main.ScreenToWorldPoint(pos);
			Vector3 pos = Input.mousePosition;
			pos.z = 10.0f;
			pos = Camera.main.ScreenToWorldPoint(pos);
			pos.x -= selectedSelectionDiff.x;
			pos.y -= selectedSelectionDiff.y;
			pos.z = selectedSelectionObject.transform.localPosition.z;
			selectedSelectionObject.transform.localPosition = pos;
		}
	}
	
	void handleMouseMovement() {
	//	if (normalDraggin && selectedUnit == getCurrentUnit() && selectedUnit.selectedItem!=null) {
	//		selectedUnit.moveItem();
	//	}
		if (isInCharacterPlacement()) handleCharacterPlacementMovement();
		
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (((middleDraggin && Input.touchCount == oldTouchCount) || scrolled || shiftDraggin) && !movingCamera) {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			//= mainCamera.WorldToScreenPoint(cameraTransform.position);
			if (!Input.GetMouseButtonDown(0) || scrolled) {
				float xDiff = pos1.x - lastPos.x;
				float yDiff = pos1.y - lastPos.y;
			//	Vector3 pos = mapTransform.position;
			//	pos.x += xDiff;
			//	pos.y += yDiff;
			//	mapTransform.position = pos;
			//	Vector3 pos = mainCamera.transform.position;
				cameraPos.x -= xDiff;
				cameraPos.y -= yDiff;
//				mainCamera.transform.position = pos;
				setCameraPos();
//				mainCamera.transform.position = new Vector3(((float)((int)(cameraPos.x * gridSize)))/((float)gridSize), ((float)((int)(cameraPos.y * gridSize)))/((float)gridSize), cameraPos.z);
			}
		}
		lastPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		oldTouchCount = Input.touchCount;
	}

	void handleKeyPan() {
		if (movingCamera) return;
		float xDiff = 0;
		float yDiff = 0;
		float eachFrame = 4.0f;
		if (shiftDown) eachFrame = 1.5f;
	//	if (!shiftDown && leftClickIsMakingSelection()) eachFrame = 0.0f;
		eachFrame *= Time.deltaTime;
		if (Input.GetKey(KeyCode.UpArrow)) yDiff -= eachFrame;
		if (Input.GetKey(KeyCode.DownArrow)) yDiff += eachFrame;
		if (Input.GetKey(KeyCode.LeftArrow)) xDiff += eachFrame;
		if (Input.GetKey(KeyCode.RightArrow)) xDiff -= eachFrame;
		if (xDiff==0 && yDiff==0) return;
	//	Vector3 pos = mapTransform.position;
	//	Vector3 pos = mainCamera.transform.position;
		cameraPos.x -= xDiff;
		cameraPos.y -= yDiff;
		setCameraPos();
	//	mapTransform.position = pos;
		lastPos.x -= xDiff;
		lastPos.y -= yDiff;
	}

	void rotatePlayerTowardsMouse() {
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (shiftDown && selectedUnit) {
			float midSlope = (pos1.y - selectedUnit.transform.position.y)/(pos1.x - selectedUnit.transform.position.x);
			float rotation = Mathf.Atan(midSlope) + Mathf.PI/2.0f;
			Vector3 rot1 = selectedUnit.transform.eulerAngles;
			if (pos1.x > selectedUnit.transform.position.x) {
				rotation += Mathf.PI;
			}
			rot1.z = rotation * 180 / Mathf.PI;
			selectedUnit.transform.eulerAngles = rot1;
		}
	}

	int getDistance(Unit u, int x, int y) {
		return Mathf.Abs((int)u.position.x - x) + Mathf.Abs((int)u.position.y + y);
	}

	public Tile getMostInterestingTile(Unit u) {
		Tile interesting = null;
		int interestingNess = 0;
		int tileDist = 0;
		for (int x = Mathf.Max(0, (int)u.position.x - u.maxMoveDist); x < Mathf.Min(actualWidth, (int)u.position.x + u.maxMoveDist + 1); x++) {
			for (int y = Mathf.Max(0, (int)-u.position.y - u.maxMoveDist); y < Mathf.Min(actualHeight, (int)-u.position.y + u.maxMoveDist + 1); y++) {
				Tile t = tiles[x,y];
				int d = getDistance(u, x, y);
				int i = t.getInterestingNess(u, d);
				if ((i > interestingNess || (i == interestingNess && d < tileDist)) && hasLineOfSight(tiles[(int)u.position.x,(int)-u.position.y],t, viewRadius)) {
					interestingNess = i;
					tileDist = d;
					interesting = t;
				}
			}
		}
		return interesting;
	}
	
	
	void handleMouseSelect() {
	//	if ((shiftRightDraggin || rightDraggin) && ((wasShiftRightDraggin || wasRightDraggin) || !isOnGUI)) {
		if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (shiftDraggin || normalDraggin) && ((wasRightDraggin || wasShiftRightDraggin) || !isOnGUI)) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			Vector3 posActual = new Vector3(v3.x,v3.y,v3.z);
		//	v3.x += gridX/2.0f;
		//	v3.y += gridY/2.0f;
		//	v3.y = gridY - v3.y;
			v3 -= mapTransform.position;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(-v3.y);
			//Debug.Log(v3);
			if (!(wasRightDraggin || wasShiftRightDraggin) || mouseOver == null) {
				startSquareActual = posActual;
				startSquare = new Vector2(v3.x,v3.y);
				//	startSquareActual = startSquare;
				mouseOver = (GameObject)Instantiate(gridPrefab);
				mouseOver.layer = 0;
				mouseOver.transform.parent = mapTransform;
				SpriteRenderer sr =  mouseOver.GetComponent<SpriteRenderer>();
				sr.color = new Color(1.0f,1.0f,1.0f,0.4f);
				sr.sortingOrder = mouseOverOrder;
			}
			else if (spaceDown) {
				Vector3 v4 = startSquareActual;
				v4.x += (posActual.x - lastPosDrag.x);
				v4.y += (posActual.y - lastPosDrag.y);
				Vector3 posActual4 = new Vector3(v4.x,v4.y,v4.z);
			//	v4.x += gridX/2.0f;
			//	v4.y += gridY/2.0f;
			//	v4.y = gridY - v4.y;
			//	v4.x = Mathf.Floor(v4.x);
			//	v4.y = Mathf.Floor(v4.y);
				startSquareActual = posActual4;
				startSquare = new Vector2(v4.x,-v4.y);
			}
			
			
			lastPosDrag = posActual;
			//			posActual = v3;
			Vector2 min = new Vector2(Mathf.Min(posActual.x,startSquareActual.x),Mathf.Min(posActual.y,startSquareActual.y));
			Vector2 max = new Vector2(Mathf.Max(posActual.x,startSquareActual.x),Mathf.Max(posActual.y,startSquareActual.y));
			//		Vector2 min2 = new Vector2(Mathf.Min(Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) - (((int)gridX)%2==1 ? 0.5f : 0.0f),Mathf.Min(Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) - (((int)gridY)%2==1 ? 0.5f : 0.0f));
			//		Vector2 max2 = new Vector2(Mathf.Max (Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) + (((int)gridX)%2==1 ? 0.5f : 1.0f) ,Mathf.Max (Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) + (((int)gridY)%2==1 ? 0.5f : 1.0f));
	//		Vector2 min2 = new Vector2(Mathf.Min(gridX, Mathf.Max(0.0f, Mathf.Min(v3.x, startSquare.x))) - gridX/2.0f, gridY/2.0f - Mathf.Min(gridY, Mathf.Max(0.0f, Mathf.Min(v3.y, startSquare.y))));
	//		Vector2 max2 = new Vector2(Mathf.Max(0.0f, Mathf.Min(gridX-1.0f,Mathf.Max(v3.x, startSquare.x)) + 1.0f) - gridX/2.0f, gridY/2.0f - Mathf.Max(0.0f, 1.0f + Mathf.Min(gridY-1.0f, Mathf.Max(v3.y, startSquare.y))));
			mouseOver.transform.localScale = new Vector3(max.x - min.x,max.y - min.y, 1.0f);
		//	mouseOver.transform.position = new Vector3((max.x + min.x)/2.0f,(max.y + min.y)/2.0f, 2.0f);
			mouseOver.transform.position = new Vector3(min.x,max.y, 2.0f);
		//	mouseOver2.transform.localScale = new Vector3(max2.x - min2.x, max2.y - min2.y, 1.0f);
		//	mouseOver2.transform.position = new Vector3((max2.x + min2.x)/2.0f, (max2.y + min2.y)/2.0f, 2.0f);
		}
		else if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (rightDragginCancelled || shiftRightDragginCancelled)) {
			if (mouseOver) {
				Destroy(mouseOver);
				mouseOver = null;
				if (!selectedUnit && hoveredCharacter && !rightDraggin && !shiftRightDraggin) {
				//	setAroundCharacter(hoveredCharacter);
				}
				setCurrentSpriteColor();
			}
			rightDragginCancelled = false;
			shiftRightDragginCancelled = false;
	//		Destroy(mouseOver2);
	//		mouseOver2 = null;
		}
		else if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (wasRightDraggin || wasShiftRightDraggin)) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
		//	v3.x += gridX/2.0f;
		//	v3.y += gridY/2.0f;
		//	v3.y = gridY - v3.y;
			v3 = v3 - mapTransform.position;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(-v3.y);
		//	startSquare.x -= mapTransform.position.x;
		//	startSquare.y += mapTransform.position.y;
		
			//Debug.Log("Start: " + startSquare);
			//Debug.Log("End: " + v3);
			Vector2 min = new Vector2(Mathf.Min(v3.x,startSquare.x),Mathf.Min(v3.y,startSquare.y));
			Vector2 max = new Vector2(Mathf.Max(v3.x,startSquare.x),Mathf.Max(v3.y,startSquare.y));
			Destroy(mouseOver);
			mouseOver = null;
	//		Destroy(mouseOver2);
	//		mouseOver2 = null;
	//		Debug.Log(min + "   " + max);
			if (Mathf.Abs (v3.x - startSquare.x) >= 0.0001f || Mathf.Abs(v3.y - startSquare.y) >= 0.0001f) {
				for (int n = (int)min.x; n <= (int)max.x;n++) {
					if (n >= 0 && n < actualWidth) {
						for (int m = (int)min.y; m <= (int)max.y; m++) {
							if (m >= 0 && m < actualHeight) {
								Tile t = tiles[n,m];
								if (t.hasCharacter()) {
//								t.getCharacter().setSelected();
									selectUnit(t.getCharacter(), false);
								}
							}
//							setProperties(t);
						}
					}
				}
			}
		}


		
	//	//Debug.Log("Start Mouse Select");
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<9);
		//		Physics2D.Ray
		GameObject go = null;
		if (hit.collider != null) go = hit.collider.gameObject;
		if (go != null && !isOnGUI) {
		//	Debug.Log(go + "  " + go.transform.position);
			if (go != lastHit) {
				lastHit = go;
				if (!selectedUnit) {
					if (hoveredCharacter) {
					//	resetAroundCharacter(hoveredCharacter, hoveredCharacter.viewDist);
					}
				}
				Tile t = tiles[(int)go.transform.localPosition.x,(int)-go.transform.localPosition.y];
				hoveredCharacter = t.getCharacter();
			}
			if (middleDraggin) {
				//	Tile t = go.GetComponent<TileHolder>().tile;
				//	setProperties(t);
				//		SpriteRenderer sR = go.GetComponent<SpriteRenderer>();
				//		sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,0.4f);
			}
			else {
				SpriteRenderer spr = go.GetComponent<SpriteRenderer>();
				if (spr) {
					if (spr != currentSprite) {
						currentGrid = go;
						if (currentSprite != null) {
							currentSprite.color = currentSpriteColor;
						}
						currentSprite = spr;
						currentSpriteColor = spr.color;
					//	if (!(rightDraggin || shiftRightDraggin)) {
						if (!((normalDraggin || shiftDraggin) && !leftClickIsMakingSelection() && !(isInCharacterPlacement() && selectedSelectionObject!=null))) {
							setCurrentSpriteColor();
						}
					}
				}
			}
		}
		else if (currentSprite!=null) {
			currentSprite.color = currentSpriteColor;
			currentGrid = null;
			currentSprite = null;
		}
	//	//Debug.Log("End Mouse Select");
	}

	public void setCurrentSpriteColor() {
		if (currentSprite == null) return;
		GameObject go = currentSprite.gameObject;
		Transform transform = go.transform;
		bool did = false;
	//	if (selectedPlayer) {
	//		Player p = selectedPlayer.GetComponent<Player>();
			//	//Debug.Log(" c: " + lastPlayerPath[0] + " d: " + new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y));
			//		if (Player.exists(lastPlayerPath, new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y))) {
			//		//Debug.Log("Yup!");
			//			spr.color = new Color(0.0f, 1.0f, 1.0f, 0.4f);
			//			did = true;
			//		}
			//	if (isInPlayerRadius(p, p.currentMoveDist,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
		Tile t = tiles[(int)transform.localPosition.x,(int)-transform.localPosition.y];
		if (t.canStandCurr) {
			currentSprite.color = new Color(0.0f, 0.0f, 0.50f, 0.4f);
			did = true;
		}
		//							else if (isInPlayerRadius(p, p.currentMoveDist + p.attackRange,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
		else if (t.canAttackCurr) {
			did = true;
			currentSprite.color = new Color(0.50f, (t.canUseSpecialCurr?0.25f:0.0f), 0.0f, 0.4f);
		}
		else if (t.canUseSpecialCurr) {
			did = true;
			currentSprite.color = new Color(0.0f, 0.50f, 0.0f, 0.4f);
		}
		else if (t.startingPoint && isInCharacterPlacement()) {
			did = true;
			currentSprite.color = new Color(0.0f, 0.5f, 0.0f, 0.4f);
		}
	//	}
		if (!did) {
			//	//Debug.Log("Nope...");
			currentSprite.color = new Color(0.40f, 0.40f, 0.40f, 0.4f);
		}
	}
	
}
