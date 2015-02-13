using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapTooltip : MonoBehaviour {
    public float z = -1.0f;

    [SerializeField]
    private float updateIntervalInSeconds = 0.25f;

    private MapGenerator map;
    private static Vector2 mousePos;
    private Vector3 toolTipPos;
    private Text toolTipText;
    private GameObject tooltipPanel;


	// Use this for initialization
	void Start () {
        map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        mousePos = new Vector2();
        toolTipPos = new Vector3();
        toolTipText = gameObject.GetComponentInChildren<Text>();
        tooltipPanel = GameObject.Find("Panel - Tooltip");
        // Start the update cycle for the tooltip. I'm not doing this with Update() because I only need to check
        //  every quarter of a second or so, so it's wasted CPU cycles.
        InvokeRepeating("updateTooltip", updateIntervalInSeconds, updateIntervalInSeconds);
	}


	// Update is called once per frame
	void Update () {		
	
	}

    // Find the tile the mouse is over, place the tooltip above that tile, check for interesting objects inside that tile
    // and display the tooltip with that object's information inside.
    private void updateTooltip() {
        // Put the tooltip box into position.
        moveTooltipAboveHoveredTile();

        // Find the coordinates of the cursor in tile-space.
        Vector2 cursorHoverCoords = getHoveredTileCoordinates();

        if (coordsWithinTileGrid(cursorHoverCoords)) {
            Tile hoveredTile = map.tiles[(int)cursorHoverCoords.x, (int)cursorHoverCoords.y];
            // Check if there's a character at the hovered-over tile, and whether they're alive if so.
            if (hoveredTile.hasCharacter() && !hoveredTile.getCharacter().isDead())  {
                // Check if you've already updated the tooltip, evidenced by it being enabled.
                Unit hoveredUnit = hoveredTile.getCharacter();
                    // Check if the character is a party member
                    if (hoveredUnit.getTeam() == 0) {
                        // Show accurate health/composure
                        toolTipText.text = getPlayerStatusSummary(hoveredUnit);
                        //tooltipPanel.GetComponent<RectTransform>() = toolTipText.gameObject.GetComponent<RectTransform>().rect.height;
                    }
                    else {
                        // Show rough condition, like "Healthy", "Bruised", etc
                        toolTipText.text = getNPCStatusSummary(hoveredUnit);
                    }
                    gameObject.GetComponent<Canvas>().enabled = true;
            }
            // An if-else ladder could go here to check for things besides characters, such as traps/chests/etc.
            //  However, if none of these checks return true, then there's nothing interesting to view, so no tooltip.
            else gameObject.GetComponent<Canvas>().enabled = false;
        }
    }

    // Set the tooltip box's coordinates.
    private void moveTooltipAboveHoveredTile() {
        Vector3 tileCoords = getHoveredTileCoordinates();
        // The tooltip position should be centered one unit above the hovered tile.
        toolTipPos =  tileCoords + new Vector3(0.5f, -0.5f, z);
        toolTipPos.y *= -1;
        gameObject.GetComponent<RectTransform>().position = toolTipPos;
    }

    // Check the input coordinates against bounds of the 2D array containing all of the tiles.
    private bool coordsWithinTileGrid(Vector2 coords) {
        return  ((int)coords.x > 0) && (int)coords.x < map.tiles.GetLength(0) &&
                ((int)coords.y > 0) && (int)coords.y < map.tiles.GetLength(1);
    }


    // Get the coordinates in tile-space of the tile being hovered over.
    public static Vector2 getHoveredTileCoordinates()  {
        mousePos = Input.mousePosition;
        Vector2 mouseTileGridPos = Camera.main.ScreenToWorldPoint(mousePos);
        // The y axis needs to be flipped.
        mouseTileGridPos.y *= -1;
        return new Vector2(Mathf.Floor(mouseTileGridPos.x), Mathf.Floor(mouseTileGridPos.y));
    }

    public static string getHealthCondition(Unit u) {
        int health = u.getCurrentHealth();
        int maxHealth = u.getMaxHealth();

        float healthPercent = (health * 100) / maxHealth;

        if (healthPercent < 25)
            return "Screwed";
        else if (healthPercent <= 50)
            return "Bruised";
        else if (healthPercent <= 75)
            return "Alright";
        else return "Healthy";
    }

    private string getPlayerStatusSummary(Unit playerUnit) {
		return UnitGUI.getSmallCapsString(playerUnit.getStatusSummary(), Mathf.FloorToInt(toolTipText.fontSize * 0.67f));
    }
    private string getNPCStatusSummary(Unit npcUnit) {
		string name = npcUnit.getName();
        float chanceToHit = 0.0f;
        if (map.getCurrentUnit() != null)  {
            chanceToHit = 5 * (20 + map.getCurrentUnit().getMeleeScoreWithMods(npcUnit) - npcUnit.getAC());
        }
        chanceToHit = (chanceToHit > 100) ? 100 : chanceToHit;
        string healthCondition = getHealthCondition(npcUnit);
        string npcStatusSummary = string.Format("{0}\n{1}\nHit Chance: {2}%", name, healthCondition, chanceToHit);
        npcStatusSummary = UnitGUI.getSmallCapsString(npcStatusSummary, Mathf.FloorToInt(toolTipText.fontSize * 0.67f));
        return npcStatusSummary;
    }
}