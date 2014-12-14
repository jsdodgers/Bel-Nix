using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using CharacterInfo;

public class GraphicalUserInterface : MonoBehaviour
{
	string characterName = "";
	string characterLastName = "";
	public string[] cCProgression = new string[] {"Personal Information"};
	int cCProgressionSelect = 0;
	//bool abilityScoresHasBeenTriggered, skillsHasBeenTriggered, talentHasBeenTriggered = false;
	public string[] sex = new string[] {"Male", "Female"};
	public string[] race = new string[] {"Berrind", "Ashpian", "Rorrul"};
	public string[] backgroundBerrind = new string[] {"Fallen Noble", "White Gem"};
	public string[] backgroundAshpian = new string[] {"Commoner", "Immigrant"};
	public string[] backgroundRorrul = new string[] {"Sevrant", "Unknown"};
	public string[] characterClass = new string[] {"Ex-Soldier", "Engineer", "Investigator", "Researcher", "Orator"};
	public string[] colorTypes = new string[] {"Body", "Hair", "Primary", "Secondary"};
	int sexSelect, raceSelect, backgroundSelect, classSelect, colorSelect = 0;
	int age = 25;
	int ageUpperBound = 40;
	int ageLowerBound = 20;
	int height = 0;
	int weight = 0;

	int abilityScorePointsAvailable = 8;
	int sturdyScore = 1;
	int perceptionScore = 1;
	int techniqueScore =1;
	int wellVersedScore = 1;
	int scoreLowerBound = 1;

	int skillPointsAvailable = 2;
	int skillLowerBound = 0;
	int athleticsSkill, meleeSkill, rangedSkill, stealthSkill, mechanicalSkill, medicinalSkill, historicalSkill, politicalSkill = 0;
	int hairStyle = 0;

	bool texturesSet = false;


	Color primaryColor;
	Color secondaryColor;
	Color berrindColor;
	Color ashpianColor;
	Color rorrulColor;
	Color hairColor;
	SpriteRenderer characterSprite;
	SpriteRenderer shirtSprite;
	SpriteRenderer pantsSprite;
	SpriteRenderer shoesSprite;
	SpriteRenderer hairSprite;
	GameObject hairGameObject;
	GUIStyle[] hairTextures;

	static Color createColor(float r, float g, float b) {
		return new Color(r/255.0f, g/255.0f, b/255.0f);
	}

	static Color[] berrindColors = new Color[]{Color.white, createColor(223, 223, 223), createColor(251, 235, 218), createColor(227, 216, 205)};
	static Color[] ashpianColors = new Color[]{createColor(255, 204, 147), createColor(197, 154, 109), createColor(180, 147, 122), createColor(177, 118, 105)};
	static Color[] rorrulColors = new Color[]{createColor(114, 90, 65), createColor(73, 52, 30), createColor(111, 98, 85), createColor(69, 63, 66)};
	static Color[] hairColors = new Color[]{Color.white, Color.black, createColor(100, 80, 60), Color.red, Color.blue, Color.green};
	static Color[] favoriteColors = new Color[]{Color.white, Color.black, Color.blue, Color.red, Color.green, Color.gray, Color.cyan, Color.magenta, Color.yellow};
	// Use this for initialization

	void setHairStyle() {
		if (hairGameObject != null) {
			GameObject.Destroy(hairGameObject);
		}
		hairGameObject = Instantiate(Resources.Load<GameObject>("Units/Hair/" + PersonalInformation.hairTypes[hairStyle])) as GameObject;
		hairGameObject.transform.parent = characterSprite.transform;
		hairGameObject.transform.localPosition = new Vector3(0, 0, 0);
		hairGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
		hairGameObject.transform.localScale = new Vector3(1, 1, 1);
		hairSprite = hairGameObject.GetComponent<SpriteRenderer>();
		hairSprite.sortingOrder = 10;
		hairSprite.color = hairColor;
	}

	void Start()
	{
		if (Application.loadedLevel == 0) {
			if (!Saves.hasCurrentSaveFile())
				Saves.createCurrentSaveFile();
			return;
		}
		characterSprite = GameObject.Find("Character").GetComponent<SpriteRenderer>();
		shirtSprite = GameObject.Find("Shirt").GetComponent<SpriteRenderer>();
		pantsSprite = GameObject.Find("Pants").GetComponent<SpriteRenderer>();
		shoesSprite = GameObject.Find("Shoes").GetComponent<SpriteRenderer>();
		berrindColor = berrindColors[Random.Range(0, berrindColors.Length)];
		ashpianColor = ashpianColors[Random.Range(0, ashpianColors.Length)];
		rorrulColor = rorrulColors[Random.Range(0, rorrulColors.Length)];
		hairColor = hairColors[Random.Range(0, hairColors.Length)];
		primaryColor = favoriteColors[Random.Range(0, favoriteColors.Length)];
		secondaryColor = favoriteColors[Random.Range(0, favoriteColors.Length)];
		hairStyle = Random.Range(0, PersonalInformation.hairTypes.Length);
		setHairStyle();
	}
	void setTexturesArray() {
		hairTextures = new GUIStyle[PersonalInformation.hairTypes.Length];
		int n=0;
		foreach (string s in PersonalInformation.hairTypes) {
			GUIStyle gs = new GUIStyle("Button");
			gs.normal.background = gs.hover.background = gs.active.background = Resources.Load<Texture>("Units/Hair/" + s) as Texture2D;
			hairTextures[n++] = gs;
		}
		texturesSet = true;
	}
	
	// Update is called once per frame
	void Update() {

	}

	int calculateBoxHeight(int n)
	{
		int height = 0;

		height = 20 * n;

		return height;
	}

	int calculateMod(int abilityScore)
	{
		return abilityScore/2;
	}

	int raceModifications(int cRace, string modName)
	{
		switch(modName)
		{
		case "Health":
			if(cRace == 0)
			{
				return (-1);
			}
			else if(cRace == 2)
			{
				return 1;
			}
			break;
		case "Composure":
			if(cRace == 0)
			{
				return 1;
			}
			else if(cRace == 2)
			{
				return (-1);
			}
			break;
		default:
			break;
		}

		return 0;
	}

	int classModifications(int cClass, string modName)
	{
		switch(modName)
		{
		case "Health":
			if(cClass == 0)
			{
				return 2;
			}
			else if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Composure":
			if(cClass == 3)
			{
				return 2;
			}
			else if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Athletics":
			if(cClass == 0)
			{
				return 1;
			}
			break;
		case "Melee":
			if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Ranged":
			if(cClass == 0)
			{
				return 1;
			}
			break;
		case "Stealth":
			if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Mechanical":
			if(cClass == 1)
			{
				return 2;
			}
			break;
		case "Medicinal":
			if(cClass == 3)
			{
				return 1;
			}
			break;
		case "Historical":
			if(cClass == 3)
			{
				return 1;
			}
			break;
		case "Political":
			if(cClass == 4)
			{
				return 2;
			}
			break;
		default:
			break;
		}

		return 0;
	}

	int setSkillDecreaseButton(int skill, int boxHeight)
	{
		if(skill == skillLowerBound)
		{
			GUI.enabled = false;
			if(GUI.Button(new Rect(260, calculateBoxHeight(boxHeight), 25, 20), "<"))
			{
				skillPointsAvailable++;
				skill = skill - 1;
			}
		}
		else
		{
			if(GUI.Button(new Rect(260, calculateBoxHeight(boxHeight), 25, 20), "<"))
			{
				skillPointsAvailable++;
				skill = skill - 1;
			}
		}
		GUI.enabled = true;

		return skill;
	}

	int setSkillIncreaseButton(int skill, int boxHeight)
	{
		if(skillPointsAvailable == 0)
		{
			GUI.enabled = false;
			if(GUI.Button(new Rect(335, calculateBoxHeight(boxHeight), 25, 20), ">"))
			{
				skillPointsAvailable--;
				skill++;
			}
		}
		else
		{
			if(GUI.Button(new Rect(335, calculateBoxHeight(boxHeight), 25, 20), ">"))
			{
				skillPointsAvailable--;
				skill++;
			}
		}
		GUI.enabled = true;

		return skill;
	}


	const int colorSquareWidth = 40;
	static Texture2D makeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	
	static Texture2D makeTexBorder(int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			//	Debug.Log("it is: " + (i/width));
			if (i/width == 0 || i/width == height-1) pix[i] = Color.red;
			else if (i%width == 0 || i % width == width-1) pix[i] = Color.red;
			else pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	static Dictionary<Color, GUIStyle> colorStyles = new Dictionary<Color, GUIStyle>();
	static GUIStyle getColorStyle(Color c) {
		if (colorStyles.ContainsKey(c)) return colorStyles[c];
		Texture2D tex = makeTex(colorSquareWidth, colorSquareWidth, c);
		GUIStyle st = new GUIStyle("Button");
		st.normal.background = st.hover.background = st.active.background = tex;
		colorStyles[c] = st;
		return st;
	}

	static Dictionary<Color, GUIStyle> colorStylesSelected = new Dictionary<Color, GUIStyle>();
	static GUIStyle getColorStyleSelected(Color c) {
		if (colorStylesSelected.ContainsKey(c)) return colorStylesSelected[c];
		Texture2D tex = makeTexBorder(colorSquareWidth, colorSquareWidth, c);
		GUIStyle st = new GUIStyle("Button");
		st.normal.background = st.hover.background = st.active.background = tex;
		colorStylesSelected[c] = st;
		return st;
	}
	bool loading = false;
	string loadingName = "";
	string[] saves;
	Vector2 loadingScrollPos = new Vector2();
	void OnGUI()
	{
		if(Application.loadedLevel == 0)
		{
			float boxX = Screen.width/4.0f;
			float boxY = Screen.height/2.0f;
			float boxHeight = 250.0f;
			float boxWidth = 200.0f;
			float buttX = boxX + 20.0f;
			float buttWidth = boxWidth - 20.0f*2.0f;
			GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), loading ? (loadingName == "" ? "Choose a file to load" : "Load: " + loadingName) : "Main Menu");
			if (loading) {
			//	float width = 250.0f;
			//	float height = Screen.height * .8f;
			//	float x = (Screen.width - width)/2.0f;
			//	float y = (Screen.height - height)/2.0f;
			//	float boxY = y;
			//	GUI.Box(new Rect(x, y, width, height), "");
				float buttonWidth = 80.0f;
				float buttonHeight = 40.0f;
				float buttonY = boxY + boxHeight - buttonHeight - 5.0f;
				float buttonX1 = boxX + 10.0f;
				float buttonX2 = buttonX1 + buttonWidth + 20.0f;
				if (GUI.Button(new Rect(buttonX1, buttonY, buttonWidth, buttonHeight), "Cancel")) {
					loading = false;
					loadingName = "";
				}
				if (GUI.Button(new Rect(buttonX2, buttonY, buttonWidth, buttonHeight), "Load")) {
					Saves.loadSave(loadingName);
				//	loading = false;
					Application.LoadLevel(2);
				}
			//	float textFieldHeight = 25.0f;
			//	saveName = GUI.TextField(new Rect(x + 5.0f, y + 5.0f, width - 10.0f, textFieldHeight), saveName);
				float savesHeight = 0.0f;
				GUIStyle st = BaseManager.getSaveButtonsStyle();
				foreach (string save in saves) {
					savesHeight += st.CalcSize(new GUIContent(save)).y;
				}
				float y = 5.0f + boxY + 20.0f;
				float scrollHeight = buttonY - y - 5.0f;
				float scrollX = boxX + 5.0f;
				float scrollWidth = boxWidth - (scrollX - boxX) * 2.0f;
				loadingScrollPos = GUI.BeginScrollView(new Rect(scrollX, y, scrollWidth, scrollHeight), loadingScrollPos, new Rect(scrollX, y, scrollWidth - 16.0f, savesHeight));
				foreach (string save in saves) {
					GUIContent gc = new GUIContent(save);
					float h = st.CalcSize(gc).y;
					if (GUI.Button(new Rect(scrollX, y, scrollWidth, h), gc, st)) {
						loadingName = save;
					}
					y += h;
				}
				GUI.EndScrollView();
			}
			else {
				if(GUI.Button(new Rect(buttX, Screen.height/2 + 20, buttWidth, 40), "New Game"))
				{
					Saves.removeFilesFromCurrentSaveFile();
					//Load into Character Creation
					Application.LoadLevel(1);
				}
				if(GUI.Button(new Rect(buttX, Screen.height/2 + 60, buttWidth, 40), "Load Game"))
				{
					saves = Saves.getSaveFiles();
					loading = true;
					loadingScrollPos = new Vector2();
	//				Application.LoadLevel(4);
				}
				if(GUI.Button(new Rect(buttX, Screen.height/2 + 100, buttWidth, 40), "Options"))
				{
					//Bring up Options UI.  Do NOT load into a new scene.
				}
				if(GUI.Button(new Rect(buttX, Screen.height/2 + 140, buttWidth, 40), "Quit"))
				{
					//Quit the Application
					Application.Quit();
				}
			}
		}
		else if(Application.loadedLevel == 1)
		{
			cCProgressionSelect = GUI.SelectionGrid(new Rect(225, Screen.height - 100, Screen.width - 450, 100), cCProgressionSelect, cCProgression, 4);
			GUI.Box(new Rect(Screen.width/2.0f - 150, 10, 300, 50), "Portrait/Looks");
			GUI.Box(new Rect(Screen.width/2.0f - 150, calculateBoxHeight(3), 300, calculateBoxHeight(16)), "");
			colorSelect = GUI.SelectionGrid(new Rect(Screen.width/2.0f - 150, calculateBoxHeight(3), 300, 20),colorSelect, colorTypes, 4);
			int num = 285 / (colorSquareWidth + 5);
			int totesWidth = num * (colorSquareWidth + 5);

			int x = Screen.width/2 - 150 + (300 - totesWidth)/2;
			int startX = x;
			int y = calculateBoxHeight(4) + 10;
			Color[] colorss = null;
			Color current = Color.clear;
			switch (colorSelect) {
			case 0:
				switch (raceSelect) {
				case 0:
					colorss = berrindColors;
					current = berrindColor;
					break;
				case 1:
					colorss = ashpianColors;
					current = ashpianColor;
					break;
				default:
					colorss = rorrulColors;
					current = rorrulColor;
					break;
				}
				break;
			case 1:
				colorss = hairColors;
				current = hairColor;
				break;
			case 2:
				current = primaryColor;
				colorss = favoriteColors;
				break;
			default:
				current = secondaryColor;
				colorss = favoriteColors;
				break;
			}
			int num2 = 0;
			foreach (Color c in colorss) {
				if (GUI.Button(new Rect(x, y, colorSquareWidth, colorSquareWidth), "", (c ==current ? getColorStyleSelected(c) : getColorStyle(c)))) {
					switch (colorSelect) {
					case 0:
						switch (raceSelect) {
						case 0:
							berrindColor = c;
							break;
						case 1:
							ashpianColor = c;
							break;
						default:
							rorrulColor = c;
							break;
						}
						break;
					case 1:
						hairColor = c;
						break;
					case 2:
						primaryColor = c;
						break;
					default:
						secondaryColor = c;
						break;
					}
				}
				x += colorSquareWidth + 5;
				num2++;
				if (num2%num==0) {
					x = startX;
					y += colorSquareWidth + 5;
				}
			}
			num2 = 0;
			if (colorSelect == 1) {
				if (!texturesSet) setTexturesArray();
				Color c = GUI.color;
				GUI.color = hairColor;
				for (int n=0;n<hairTextures.Length;n++) {
					GUIStyle t = hairTextures[n];
					if (GUI.Button(new Rect(x, y, colorSquareWidth, colorSquareWidth), "", t)) {
						hairStyle = n;
						setHairStyle();
					}
					x += colorSquareWidth + 5;
					num2++;
					if (num2%num==0) {
						x = startX;
						y += colorSquareWidth + 5;
					}
				}
				GUI.color = c;
			}
			shirtSprite.color = primaryColor;
			pantsSprite.color = secondaryColor;
			shoesSprite.color = secondaryColor;
			hairSprite.color = hairColor;

			switch(raceSelect)
			{
			case 0:
				characterSprite.color = berrindColor;
				break;
			case 1:
				characterSprite.color = ashpianColor;
				break;
			case 2:
				characterSprite.color = rorrulColor;
				break;
			default:
				break;
			}
			if(cCProgressionSelect == 0)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Personal Information");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "First Name:");
				characterName = GUI.TextField(new Rect(260, calculateBoxHeight(3), 250, 20), characterName);
				GUI.Box(new Rect(10, calculateBoxHeight(4), 230, 20), "Last Name:");
				characterLastName = GUI.TextField(new Rect(260, calculateBoxHeight(4), 250, 20), characterLastName);
				GUI.Box(new Rect(10, calculateBoxHeight(5), 250, 20), "Sex:");
				sexSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(5), 250, 20),sexSelect, sex, 2);
				GUI.Box(new Rect(10, calculateBoxHeight(6), 250, 20), "Race:");
				raceSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(6), 250, 20),raceSelect, race, 3);
				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Racial Stats:");
				GUI.Box(new Rect(135, calculateBoxHeight(8), 125, 20), "Primal State:");
				GUI.Box(new Rect(135, calculateBoxHeight(9), 125, 20), "Background:");
				switch(raceSelect)
				{
				case 0:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "-1 Health/ +1 Composure");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Reckless");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundBerrind, 2);
					break;
				case 1:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "No Changes");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Passive");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundAshpian, 2);
					break;
				case 2:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "+1 Health/ -1 Composure");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Threatened");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundRorrul, 2);
					break;
				default:
					break;
				}

				GUI.Box(new Rect(10, calculateBoxHeight(10), 500, 20), "Physical Features:");
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Age:");
				if(age == ageLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(11), 25, 20), "<") && age > ageLowerBound)
					{
						age--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(11), 25, 20), "<") && age > ageLowerBound)
					{
						age--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(11), 200, 20), age.ToString());
				if(age == ageUpperBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(11), 25, 20), ">") && age < ageUpperBound)
					{
						age++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(11), 25, 20), ">") && age < ageUpperBound)
					{
						age++;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Height:");
				if(GUI.Button(new Rect(260, calculateBoxHeight(12), 25, 20), "<"))
				{
					height--;
				}
				GUI.Box(new Rect(285, calculateBoxHeight(12), 200, 20), height.ToString());
				if(GUI.Button(new Rect(485, calculateBoxHeight(12), 25, 20), ">"))
				{
					height++;
				}
				
				GUI.Box(new Rect(135, calculateBoxHeight(13), 125, 20), "Weight:");
				if(GUI.Button(new Rect(260, calculateBoxHeight(13), 25, 20), "<"))
				{
					weight--;
				}
				GUI.Box(new Rect(285, calculateBoxHeight(13), 200, 20), weight.ToString());
				if(GUI.Button(new Rect(485, calculateBoxHeight(13), 25, 20), ">"))
				{
					weight++;
				}
				
				
				GUI.Box(new Rect(10, calculateBoxHeight(14), 250, 40), "Class:");
				classSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(14), 250, 40),classSelect, characterClass, 3);
				GUI.Box(new Rect(135, calculateBoxHeight(16), 125, 20), "Class Stats:");
				GUI.Box(new Rect(135, calculateBoxHeight(17), 125, 20), "Class Features:");
				switch(classSelect)
				{
				case 0:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Health/+1 Athletics/ +1 Ranged");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Throw");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Decisive Strike");
					break;
				case 1:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Mechanical");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Construction");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Efficient Storage");
					break;
				case 2:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+1 Health/+1 Composure/+1 Melee/ +1 Stealth");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Mark");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Sneak Attack");
					break;
				case 3:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Composure/+1 Medicinal/ +1 Historical");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Stabilize");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Combat Medic");
					break;
				case 4:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Political");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Invoke");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Primal Control");
					break;
				default:
					break;
				}

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Cancel"))
				{
					Application.LoadLevel(0);
				}

				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//abilityScoresHasBeenTriggered = true;
					cCProgressionSelect = 1;
					if(cCProgression.Length < 2)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores"};
					}
				}
			}
			else if(cCProgressionSelect == 1)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Ability Scores");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "Points Available:");
				GUI.Box(new Rect(260, calculateBoxHeight(3), 250, 20), abilityScorePointsAvailable.ToString());

				GUI.Box(new Rect(135, calculateBoxHeight(4), 125, 20), "Sturdy:");
				if(sturdyScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(4), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						sturdyScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(4), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						sturdyScore--;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(285, calculateBoxHeight(4), 200, 20), sturdyScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(4), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						sturdyScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(4), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						sturdyScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(135, calculateBoxHeight(5), 125, 20), "Perception:");
				if(perceptionScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(5), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						perceptionScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(5), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						perceptionScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(5), 200, 20), perceptionScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(5), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						perceptionScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(5), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						perceptionScore++;
					}
				}
				GUI.enabled = true;
				GUI.Box(new Rect(510, calculateBoxHeight(4), 125, 20), "Health:");
				GUI.Box(new Rect(510, calculateBoxHeight(5), 125, 20), (sturdyScore + perceptionScore + classModifications(classSelect, "Health") + raceModifications(raceSelect, "Health")).ToString());

				GUI.Box(new Rect(135, calculateBoxHeight(6), 125, 20), "Technique:");
				if(techniqueScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(6), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						techniqueScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(6), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						techniqueScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(6), 200, 20), techniqueScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(6), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						techniqueScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(6), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						techniqueScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Well-Versed:");
				if(wellVersedScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(7), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						wellVersedScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(7), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						wellVersedScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(7), 200, 20), wellVersedScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(7), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						wellVersedScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(7), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						wellVersedScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(510, calculateBoxHeight(6), 125, 20), "Composure:");
				GUI.Box(new Rect(510, calculateBoxHeight(7), 125, 20), (techniqueScore + wellVersedScore + classModifications(classSelect, "Composure") + raceModifications(raceSelect, "Composure")).ToString());

				GUI.Box(new Rect(10, calculateBoxHeight(9), 250, 20), "Combat Scores:");
				GUI.Box(new Rect(135, calculateBoxHeight(10), 125, 20), "Initiatve:");
				GUI.Box(new Rect(260, calculateBoxHeight(10), 125, 20), calculateMod(sturdyScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Critical:");
				GUI.Box(new Rect(260, calculateBoxHeight(11), 125, 20), calculateMod(perceptionScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Handling:");
				GUI.Box(new Rect(260, calculateBoxHeight(12), 125, 20), calculateMod(techniqueScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(13), 125, 20), "Dominion:");
				GUI.Box(new Rect(260, calculateBoxHeight(13), 125, 20), calculateMod(wellVersedScore).ToString());

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 0;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//skillsHasBeenTriggered = true;
					cCProgressionSelect = 2;
					if(cCProgression.Length < 3)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores", "Skills"};
					}
				}
			}
			else if(cCProgressionSelect == 2)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Skills");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "Points Available:");
				GUI.Box(new Rect(260, calculateBoxHeight(3), 250, 20), skillPointsAvailable.ToString());
				GUI.Box(new Rect(10, calculateBoxHeight(4), 125, 20), "Category:");
				GUI.Box(new Rect(135, calculateBoxHeight(4), 125, 20), "Skill:");
				GUI.Box(new Rect(460, calculateBoxHeight(4), 50, 20), "Total:");
				GUI.Box(new Rect(285, calculateBoxHeight(4), 50, 20), "Base:");
				GUI.Box(new Rect(385, calculateBoxHeight(4), 50, 20), "Mod:");
				GUI.Box(new Rect(10, calculateBoxHeight(5), 125, 40), "Physique:");
				GUI.Box(new Rect(135, calculateBoxHeight(5), 125, 20), "Athletics:");
				GUI.Box(new Rect(460, calculateBoxHeight(5), 50, 20), (athleticsSkill + calculateMod(sturdyScore) + classModifications(classSelect, "Athletics")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(5), 50, 20), (athleticsSkill + classModifications(classSelect, "Athletics")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(5), 50, 40), calculateMod(sturdyScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(5), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(5), 25, 40), "=");
				athleticsSkill = setSkillDecreaseButton(athleticsSkill, 5);
				athleticsSkill = setSkillIncreaseButton(athleticsSkill, 5);
				GUI.Box(new Rect(135, calculateBoxHeight(6), 125, 20), "Melee:");
				GUI.Box(new Rect(460, calculateBoxHeight(6), 50, 20), (meleeSkill + calculateMod(sturdyScore) + classModifications(classSelect, "Melee")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(6), 50, 20), (meleeSkill + classModifications(classSelect, "Melee")).ToString());
				meleeSkill = setSkillDecreaseButton(meleeSkill, 6);
				meleeSkill = setSkillIncreaseButton(meleeSkill, 6);
				GUI.Box(new Rect(10, calculateBoxHeight(7), 125, 40), "Prowess:");
				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Ranged:");
				GUI.Box(new Rect(460, calculateBoxHeight(7), 50, 20), (rangedSkill + calculateMod(perceptionScore) + classModifications(classSelect, "Ranged")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(7), 50, 20), (rangedSkill + classModifications(classSelect, "Ranged")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(7), 50, 40), calculateMod(perceptionScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(7), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(7), 25, 40), "=");
				rangedSkill = setSkillDecreaseButton(rangedSkill, 7);
				rangedSkill = setSkillIncreaseButton(rangedSkill, 7);
				GUI.Box(new Rect(135, calculateBoxHeight(8), 125, 20), "Stealth:");
				GUI.Box(new Rect(460, calculateBoxHeight(8), 50, 20), (stealthSkill + calculateMod(perceptionScore) + classModifications(classSelect, "Stealth")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(8), 50, 20), (stealthSkill + classModifications(classSelect, "Stealth")).ToString());
				stealthSkill = setSkillDecreaseButton(stealthSkill, 8);
				stealthSkill = setSkillIncreaseButton(stealthSkill, 8);
				GUI.Box(new Rect(10, calculateBoxHeight(9), 125, 40), "Mastery:");
				GUI.Box(new Rect(135, calculateBoxHeight(9), 125, 20), "Mechanical:");
				GUI.Box(new Rect(460, calculateBoxHeight(9), 50, 20), (mechanicalSkill + calculateMod(techniqueScore) + classModifications(classSelect, "Mechanical")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(9), 50, 20), (mechanicalSkill + classModifications(classSelect, "Mechanical")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(9), 50, 40), calculateMod(techniqueScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(9), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(9), 25, 40), "=");
				mechanicalSkill = setSkillDecreaseButton(mechanicalSkill, 9);
				mechanicalSkill = setSkillIncreaseButton(mechanicalSkill, 9);
				GUI.Box(new Rect(135, calculateBoxHeight(10), 125, 20), "Medicinal:");
				GUI.Box(new Rect(460, calculateBoxHeight(10), 50, 20), (medicinalSkill + calculateMod(techniqueScore) + classModifications(classSelect, "Medicinal")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(10), 50, 20), (medicinalSkill + classModifications(classSelect, "Medicinal")).ToString());
				medicinalSkill = setSkillDecreaseButton(medicinalSkill, 10);
				medicinalSkill = setSkillIncreaseButton(medicinalSkill, 10);
				GUI.Box(new Rect(10, calculateBoxHeight(11), 125, 40), "Knowledge:");
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Historical:");
				GUI.Box(new Rect(460, calculateBoxHeight(11), 50, 20), (historicalSkill + calculateMod(wellVersedScore) + classModifications(classSelect, "Historical")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(11), 50, 20), (historicalSkill + classModifications(classSelect, "Historical")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(11), 50, 40), calculateMod(wellVersedScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(11), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(11), 25, 40), "=");
				historicalSkill = setSkillDecreaseButton(historicalSkill, 11);
				historicalSkill = setSkillIncreaseButton(historicalSkill, 11);
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Political:");
				GUI.Box(new Rect(460, calculateBoxHeight(12), 50, 20), (politicalSkill + calculateMod(wellVersedScore) + classModifications(classSelect, "Political")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(12), 50, 20), (politicalSkill + classModifications(classSelect, "Political")).ToString());
				politicalSkill = setSkillDecreaseButton(politicalSkill, 12);
				politicalSkill = setSkillIncreaseButton(politicalSkill, 12);

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 1;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//talentHasBeenTriggered = true;
					cCProgressionSelect = 3;
					if(cCProgression.Length < 4)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores", "Skills", "Talent"};
					}
				}
			}
			else
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Talents");

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 2;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Finish"))
				{
					writeCharacter();
				}
			}
		}
	}
	const string delimiter = ";";
	bool saving = false;
	public void writeCharacter()
	{
		if (saving) return;
		saving = true;
		string characterStr = "";
		//********PERSONAL INFORMATION********\\
		//Adding player first name.
		characterStr += characterName + delimiter;
		//If the player has a last name, add it.
		characterStr += characterLastName + delimiter;
		//sexSelect 0 = Male, 1 = Female
		characterStr += sexSelect.ToString() + delimiter;
		//raceSelect 0 = Berrind, 1 = Ashpian, 2 = Rorrul
		characterStr += raceSelect.ToString() + delimiter;
		//backgroundSelect (contextualized by race)
		//For Berrind: 0 = Fallen Noble, 1 = White Gem
		//For Ashpian: 0 = Commoner, 1 = Immigrant
		//For Rorrul: 0 = Servant, 1 = Unknown
		characterStr += backgroundSelect.ToString() + delimiter;
		characterStr += age.ToString() + delimiter;
		characterStr += height.ToString() + delimiter;
		characterStr += weight.ToString() + delimiter;
		//classSelect 0 = Ex-Soldier, 1 = Engineer, 2 = Investigator, 3 = Researcher, 4 = Orator
		characterStr += classSelect.ToString() + delimiter;
		//********Ability Scores********\\
		characterStr += sturdyScore.ToString() + delimiter;
		characterStr += perceptionScore.ToString() + delimiter;
		characterStr += techniqueScore.ToString() + delimiter;
		characterStr += wellVersedScore.ToString() + delimiter;
		//********Skills********\\
		characterStr += athleticsSkill.ToString() + delimiter;
		characterStr += meleeSkill.ToString() + delimiter;
		characterStr += rangedSkill.ToString() + delimiter;
		characterStr += stealthSkill.ToString() + delimiter;
		characterStr += mechanicalSkill.ToString() + delimiter;
		characterStr += medicinalSkill.ToString() + delimiter;
		characterStr += historicalSkill.ToString() + delimiter;
		characterStr += politicalSkill.ToString() + delimiter;
		//********Talents********\\
		Color raceColor = Color.white;
		switch (raceSelect) {
		case 0:
			raceColor = berrindColor;
			break;
		case 1:
			raceColor = ashpianColor;
			break;
		default:
			raceColor = rorrulColor;
			break;
		}
		characterStr += colorString(raceColor);
		characterStr += colorString(hairColor);
		characterStr += colorString(primaryColor);
		characterStr += colorString(secondaryColor);
		//********Colors********\\
		characterStr += hairStyle + delimiter;
		//*********Hair*********\\

		Saves.addCharacter(characterStr);


//		TextAsset ta = Resources.Load<TextAsset>("Saves/Characters");
//		string characterList = "";
//		if (ta) characterList = ta.text;
//		if (characterList != "") characterList += ";";
//		characterList += fileN2;

//		StreamWriter sw = File.AppendText(Application.dataPath + "/Resources/Saves/Characters.txt");
//		sw.Write(fileN2 + ";");
//		sw.Close();
		Application.LoadLevel(2);
	//	Debug.Log(characterStr);
	}

	static string colorString(Color c) {
		return ((int)(c.r*255)) + delimiter + ((int)(c.g*255)) + delimiter + ((int)(c.b*255)) + delimiter;
	}

}
