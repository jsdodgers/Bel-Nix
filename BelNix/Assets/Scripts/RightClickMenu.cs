using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightClickMenu : MonoBehaviour {
	public GameObject rightClickActionButtonPrefab;
	public Transform rightClickActionButtonContainer;
	static RightClickMenu rcm = null;
	static MapGenerator map;
	static Vector2 mousePos;
	Vector3 toolTipPos;
	RectTransform trans;
	static bool pointerInside = false;
	Tile currentRightClickTile = null;
	public static bool shown;

	void Start () {
		rcm = this;
		trans = gameObject.GetComponent<RectTransform>();
		map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
		mousePos = new Vector2();
		toolTipPos = trans.anchoredPosition;
		hideMenu();
	}
	
	// Update is called once per frame
	void Update () {
	//	showMenu();
	//	setPos();
	}

	public static void hideMenu(bool anyway = false) {
		if (!pointerInside || anyway) {
			rcm.hide();
		}
	}

	public static void showMenu() {
		if (coordsWithinTileGrid() && currentUnitCanSeeCoords()) {
			if (!shown || !pointerInside)
				rcm.show();
		}
		else if (shown) {
			rcm.hide();
		}
	}

	void hide() {
		shown = false;
		gameObject.SetActive(false);
		for (int n = rightClickActionButtonContainer.childCount-1;n>=0;n--) {
			RightClickButton b = rightClickActionButtonContainer.GetChild(n).GetComponent<RightClickButton>();
			if (b.mouseOver) b.mouseExit();
			GameObject.Destroy(b.gameObject);
		}
		if (currentRightClickTile != null) {
			currentRightClickTile.currentRightClick = false;
			currentRightClickTile = null;
		}
	}

	void show() {
		Vector3 tileCoords = getHoveredTileCoordinates();
		Tile t = map.tiles[(int)tileCoords.x, (int)tileCoords.y];
		List<TileAction> tileActions = t.getTileActions(map.getCurrentUnit());
		if (tileActions.Count == 0) {
			hide ();
			return;
		}
		shown = true;
		moveTooltipAboveHoveredTile();
		gameObject.SetActive(true);
		for (int n = rightClickActionButtonContainer.childCount-1;n>=0;n--) {
			RightClickButton b = rightClickActionButtonContainer.GetChild(n).GetComponent<RightClickButton>();
			if (b.mouseOver) b.mouseExit();
			GameObject.Destroy(b.gameObject);
		}
	    foreach (TileAction tA in tileActions) {
			GameObject tileButton = GameObject.Instantiate(rightClickActionButtonPrefab) as GameObject;
			tileButton.transform.GetChild(0).GetComponent<Text>().text = tA.toString();
			tileButton.transform.SetParent(rightClickActionButtonContainer, false);
	//		tileButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	//		tileButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			RightClickButton b = tileButton.GetComponent<RightClickButton>();
			b.action = tA;
		}
		trans.sizeDelta = new Vector2(trans.sizeDelta.x, tileActions.Count * 50.0f);
		if (currentRightClickTile != null && currentRightClickTile != t) currentRightClickTile.currentRightClick = false;
		currentRightClickTile = t;
		currentRightClickTile.currentRightClick = true;
	}

	public void pointerEnter() {
		pointerInside = true;
	}

	public void pointerExit() {
		pointerInside = false;
	}

	void moveTooltipAboveHoveredTile() {
		Vector3 tileCoords = getHoveredTileCoordinates();
		// The tooltip position should be centered one unit above the hovered tile.
		toolTipPos =  tileCoords + new Vector3(0.5f, 0.0f, toolTipPos.z);
		toolTipPos.y *= -1;
		//gameObject.GetComponent<RectTransform>().position = toolTipPos;
		trans.anchoredPosition = toolTipPos;
	}
	
	// Check the input coordinates against bounds of the 2D array containing all of the tiles.
	static bool coordsWithinTileGrid() {
		return coordsWithinTileGrid(getHoveredTileCoordinates());
	}


	static bool coordsWithinTileGrid(Vector2 coords) {
		return  ((int)coords.x >= 0) && (int)coords.x < map.tiles.GetLength(0) &&
			((int)coords.y >= 0) && (int)coords.y < map.tiles.GetLength(1);
	}


	static bool currentUnitCanSeeCoords() {
		Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 v2 = new Vector2(v.x * map.gridSize, v.y * map.gridSize);
		return map.hasLineOfSight(map.getCurrentUnit(), v2);// Camera.main.ScreenToWorldPoint(Input.mousePosition)
	}

	// Get the coordinates in tile-space of the tile being hovered over.
	static Vector2 getHoveredTileCoordinates()  {
		mousePos = Input.mousePosition;
		Vector2 mouseTileGridPos = Camera.main.ScreenToWorldPoint(mousePos);
		// The y axis needs to be flipped.
		mouseTileGridPos.y *= -1;
		return new Vector2(Mathf.Floor(mouseTileGridPos.x), Mathf.Floor(mouseTileGridPos.y));
	}
}
