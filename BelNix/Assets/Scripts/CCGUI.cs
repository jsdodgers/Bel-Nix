using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CCGUI : MonoBehaviour {
	public enum GUIState  {SEX, RACE, PHYSICAL_FEATURES, CLASS, ABILITY_SCORES, SKILLS, /*TALENTS,*/ NAME};
	[SerializeField] private GameObject[] stateList;
	[SerializeField] private GameObject[] progressionButtons;
	[SerializeField] private GameObject blankText;
	private GUIState currentState;
	private GUIState furthestState;
	private CCPointAllocation pointAllocator;
	private Dictionary<string, GameObject> passport;
	[SerializeField] private GameObject[] skinColorList;
	[SerializeField] private Sprite[] hairStyles;

	[SerializeField] private Transform hairTransform;

	[SerializeField] private GameObject plank;
	[SerializeField] private GameObject shortSword;
	[SerializeField] private GameObject leatherSpaulder;
	[SerializeField] private GameObject leatherVest;
	[SerializeField] private GameObject leatherCap;
	[SerializeField] private GameObject paddedVest;
	[SerializeField] private GameObject handAxe;
	[SerializeField] private GameObject bullyStick;
	[SerializeField] private GameObject dagger;
	[SerializeField] private GameObject clothHood;
	[SerializeField] private GameObject clothChest;
	[SerializeField] private GameObject clothPants;
	[SerializeField] private GameObject clothBoots;
	[SerializeField] private GameObject startingTurret;
	[SerializeField] private GameObject startingTrap;
	[SerializeField] private Button turretButton;
	[SerializeField] private GameObject[] classTextFields;
	[SerializeField] private GameObject engineerButtons;
	[SerializeField] private GameObject[] raceTextFields;
	[SerializeField] private GameObject[] backgroundTextFields;
	[SerializeField] private GameObject[] sexFields;
	[SerializeField] private Transform berrindColors;
	[SerializeField] private Transform ashpianColors;
	[SerializeField] private Transform rorrulColors;
	[SerializeField] private Transform hairColors;
	[SerializeField] private Transform primaryColors;
	[SerializeField] private Transform secondaryColors;
	bool turret = false;
	Button selectedTurretButton = null;
	Button selectedClassButton = null;
	Button selectedRaceButton = null;
	Button selectedBackgroundButton = null;
	Button selectedSexButton = null;
	List<Button> selectedButtons = new List<Button>();

	string characterName;
	string characterLastName;

	Color primaryColor;
	Color secondaryColor;
	Color berrindColor;
	Color ashpianColor;
	Color rorrulColor;
	Color hairColor;
	Image characterSprite;
	Image shirtSprite;
	Image pantsSprite;
	Image shoesSprite;
	Image hairSprite;
    Image paperdollHair;
    Image paperdollSkin;
    Image paperdollPrimary;
    Image paperdollSecondary;
	GameObject hairGameObject;
	GameObject paperdollMain;
	GUIStyle[] hairTextures;
	int hairStyle = 0;
	public EventSystem system;
	public Selectable first;
	public Selectable last;
	bool shouldSelectFirst;

	void Update() {
		if (currentState != GUIState.NAME) return;
//		EventSystem system = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		GameObject currObj = system.currentSelectedGameObject;
		Selectable current = null;
		if (currObj != null) current = currObj.GetComponent<Selectable>();
		if (Input.GetKeyDown(KeyCode.Tab) || shouldSelectFirst) {// || current == null || (current != first && current != last)) {
		
			shouldSelectFirst = false;
//			system.firstSelectedGameObject
//			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
//			if (next == null) next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
		//	Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
			Selectable next = first;
			if (current == first) next = last;
			if (next != null) {
				
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
					inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
				
				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
			//else Debug.Log("next nagivation element not found");
			
		}
	}


	public void selectTurret(Button b)  {
		if (turret) return;
		turret = true;
		selectTurretButton(b);
	}
	
	public void selectTrap(Button b)  {
		if (!turret) return;
		turret = false;
		selectTurretButton(b);
	}
	public void selectTurretButton(Button b)  {
		if (selectedTurretButton != null)  {
			selectedTurretButton.GetComponent<Animator>().SetBool("Selected",false);
			selectedButtons.Remove(selectedTurretButton);
		}
		selectedTurretButton = b;
		if (selectedTurretButton != null)  {
			selectedTurretButton.GetComponent<Animator>().SetBool("Selected",true);
			selectedButtons.Add(selectedTurretButton);
		}
	}

	public void selectExSoldier(Button b)  {
		selectClass(b, new Class_ExSoldier());
	}
	public void selectEngineer(Button b)  {
		selectClass(b, new Class_Engineer());
	}
	public void selectInvestigator(Button b)  {
		selectClass(b, new Class_Investigator());
	}
	public void selectResearcher(Button b)  {
		selectClass(b, new Class_Researcher());
	}
	public void selectOrator(Button b)  {
		selectClass(b, new Class_Orator());
	}
	public void selectClass(Button b, CharacterClass cc)  {
		character.cClass = cc;
		selectClassButton(b);
		showClassPanel(cc);
	}
	public void showClassPanel(CharacterClass cc)  {
		if (cc is Class_ExSoldier)  {
			toggleAllExcept(classTextFields[0]);
		}
		else if (cc is Class_Engineer)  {
			toggleAllExcept(new GameObject[]  {classTextFields[1], engineerButtons});
		}
		else if (cc is Class_Investigator)  {
			toggleAllExcept(classTextFields[2]);
		}
		else if (cc is Class_Researcher)  {
			toggleAllExcept(classTextFields[3]);
		}
		else if (cc is Class_Orator)  {
			toggleAllExcept(classTextFields[4]);
		}
	}
	public void selectClassButton(Button b)  {
		if (selectedClassButton != null)  {
			selectedClassButton.GetComponent<Animator>().SetBool("Selected",false);
			selectedButtons.Remove(selectedClassButton);
		}
		selectedClassButton = b;
		if (selectedClassButton != null)  {
			selectedClassButton.GetComponent<Animator>().SetBool("Selected",true);
			selectedButtons.Add(selectedClassButton);
		}
	}
	public void selectRaceBerrind(Button b)  {
		selectRace(b, new Race_Berrind());
		setSkinColor(berrindColor);

	}
	public void selectRaceAshpian(Button b)  {
		selectRace(b, new Race_Ashpian());
		setSkinColor(ashpianColor);
	}
	public void selectRaceRorrul(Button b)  {
		selectRace(b, new Race_Rorrul());
		setSkinColor(rorrulColor);
	}
	public void selectRace(Button b, CharacterRace cr)  {
		character.race = cr;
		selectRaceButton(b);
		showRacePanel(cr);
		selectBackground(null, CharacterBackground.None);
	}
	public void showRacePanel(CharacterRace cr)  {
		toggleAllExcept(getRaceTextField(cr));
	}
	public GameObject getRaceTextField(CharacterRace cr)  {
		if (cr is Race_Berrind)  {
			return raceTextFields[0];
		}
		else if (cr is Race_Ashpian)  {
			return raceTextFields[1];
		}
		else if (cr is Race_Rorrul)  {
			return raceTextFields[2];
		}
		return null;
	}
	public void selectRaceButton(Button b)  {
		if (selectedRaceButton != null)  {
			selectedRaceButton.GetComponent<Animator>().SetBool("Selected",false);
			selectedButtons.Remove(selectedRaceButton);
		}
		selectedRaceButton = b;
		if (selectedRaceButton != null)  {
			selectedRaceButton.GetComponent<Animator>().SetBool("Selected",true);
			selectedButtons.Add(selectedRaceButton);
		}
	}
	
	public void selectBackgroundFallenNoble(Button b)  {
		selectBackground(b, CharacterBackground.FallenNoble);
	}
	public void selectBackgroundWhiteGem(Button b)  {
		selectBackground(b, CharacterBackground.WhiteGem);
	}
	public void selectBackgroundCommoner(Button b)  {
		selectBackground(b, CharacterBackground.Commoner);
	}
	public void selectBackgroundImmigrant(Button b)  {
		selectBackground(b, CharacterBackground.Immigrant);
	}
	public void selectBackgroundServant(Button b)  {
		selectBackground(b, CharacterBackground.Servant);
	}
	public void selectBackgroundUnknown(Button b)  {
		selectBackground(b, CharacterBackground.Unknown);
	}
	public void selectBackground(Button b, CharacterBackground cb)  {
		character.background = cb;
		selectBackgroundButton(b);
		showBackgroundPanel(cb);
	}
	public void showBackgroundPanel(CharacterBackground cb)  {
		GameObject go = getRaceTextField(character.race);
		if (go != null)  {
			if (cb != CharacterBackground.None)  {
				toggleAllExcept(new GameObject[]  {go, backgroundTextFields[(int)cb]});
			}
			else toggleAllExcept(go);
		}
		else if (cb != CharacterBackground.None)  {
			toggleAllExcept(backgroundTextFields[(int)cb]);
		}
	}
	public void selectBackgroundButton(Button b)  {
		if (selectedBackgroundButton != null)  {
			selectedBackgroundButton.GetComponent<Animator>().SetBool("Selected",false);
			selectedButtons.Remove(selectedBackgroundButton);
		}
		selectedBackgroundButton = b;
		if (selectedBackgroundButton != null)  {
			selectedBackgroundButton.GetComponent<Animator>().SetBool("Selected",true);
			selectedButtons.Add(selectedBackgroundButton);
		}
	}

	public void selectSexMale(Button b)  {
		selectSex(b, CharacterSex.Male);
	}
	public void selectSexFemale(Button b)  {
		selectSex(b, CharacterSex.Female);
	}
	public void selectSex(Button b, CharacterSex cs)  {
		character.sex = cs;
		selectSexButton(b);
		showSexPanel(cs);
	}
	public void showSexPanel(CharacterSex cs)  {
		if (cs == CharacterSex.None) toggleAllExcept(blankText);
		else toggleAllExcept(sexFields[(int)cs]);
	}
	public void selectSexButton(Button b)  {
		if (selectedSexButton != null)  {
			selectedSexButton.GetComponent<Animator>().SetBool("Selected",false);
			selectedButtons.Remove(selectedSexButton);
		}
		selectedSexButton = b;
		if (selectedSexButton != null)  {
			selectedSexButton.GetComponent<Animator>().SetBool("Selected",true);
			selectedButtons.Add(selectedSexButton);
		}
	}



	static Color createColor(float r, float g, float b)  {
		return new Color(r, g, b);
	}

	public struct CharacterCreator  {
		public CharacterSex sex;
		public CharacterRace race;
		public CharacterBackground background;
		public CharacterClass cClass;
		public AbilityScores scores;
		public SkillScores skills;
		public CharacterName name;
	}

	public CharacterCreator character;

	// Use this for initialization
	void Start()  {
		//pointAllocator = GameObject.Find("Panel - Ability Scores").GetComponent<CCPointAllocation>();

		passport = new Dictionary<string, GameObject>();
		passport.Add("Sex", GameObject.Find("Text - Sex"));
		passport.Add("Race", GameObject.Find("Text - Race"));
		passport.Add("Background", GameObject.Find("Text - Background"));
		passport.Add("Class", GameObject.Find("Text - Class"));
		passport.Add("Ability Scores", GameObject.Find("Text - Ability Scores"));
		passport.Add("Skills", GameObject.Find("Text - Skills"));
		passport.Add("Name", GameObject.Find("Text - Name"));

		characterSprite = GameObject.Find("Dude").GetComponent<Image>();
		shirtSprite = GameObject.Find("Shirt").GetComponent<Image>();
		pantsSprite = GameObject.Find("Pantaloons").GetComponent<Image>();
		shoesSprite = GameObject.Find("Shoes").GetComponent<Image>();
		hairSprite = GameObject.Find("Hair").GetComponent<Image>();
		paperdollMain = GameObject.Find("Panel - Paperdoll Male");
        paperdollHair = GameObject.Find("Image - Male Hair").GetComponent<Image>(); ;
        paperdollSkin = GameObject.Find("Image - Male Skin").GetComponent<Image>(); ;
        paperdollPrimary = GameObject.Find("Image - Male Primary").GetComponent<Image>(); ;
        paperdollSecondary = GameObject.Find("Image - Male Secondary").GetComponent<Image>(); ;
		/*
		hairColor = createColor(100/255.0f, 73/255.0f, 41/255.0f);
		berrindColor = createColor(246/255.0f, 197/255.0f, 197/255.0f);
		ashpianColor = createColor(223/255.0f, 180/255.0f, 135/255.0f);
		rorrulColor = createColor(96/255.0f, 71/255.0f, 56/255.0f);
		primaryColor = createColor(101/255.0f, 101/255.0f, 101/255.0f);
		secondaryColor = createColor(30/255.0f, 30/255.0f, 30/255.0f);

		shirtSprite.color = primaryColor;
		pantsSprite.color = secondaryColor;
		shoesSprite.color = secondaryColor;
		paperdollPrimary.color = primaryColor;
		paperdollSecondary.color = secondaryColor;
		paperdollHair.color = hairColor;
		setHairStyle(0);
        setHairColor(hairColor);*/

		setHairStyle(Random.Range(0,2));
		setHairColor(hairColors.GetChild(Random.Range(0, hairColors.childCount)).gameObject);
		setBerrindSkinColor(berrindColors.GetChild(Random.Range(0, berrindColors.childCount)).gameObject);
		setAshpianSkinColor(ashpianColors.GetChild(Random.Range(0, ashpianColors.childCount)).gameObject);
		setRorrulSkinColor(rorrulColors.GetChild(Random.Range(0, rorrulColors.childCount)).gameObject);
		settingPrimaryColor();
		setPrimaryColor(primaryColors.GetChild(Random.Range(0, primaryColors.childCount)).gameObject);
		settingSecondaryColor();
		setPrimaryColor(secondaryColors.GetChild(Random.Range(0, secondaryColors.childCount)).gameObject);
		settingPrimaryColor();

		character.sex = CharacterSex.None;
		selectTurret(turretButton);
		currentState = GUIState.SEX;
		furthestState = currentState;
		setState(currentState);
	}

	public void setFirstName(InputField firstName)  {
		characterName = firstName.text;
		isNextAvailable();
	}

	public void setLastName(InputField lastName)  {
		characterLastName = lastName.text;
		isNextAvailable();
	}

	bool settingPrimary = true;
	public void settingPrimaryColor()  {settingPrimary = true;}
	public void settingSecondaryColor()  {settingPrimary = false;}

	public void setHairColor(GameObject newHairColor)  {
        setHairColor(newHairColor.GetComponent<Image>().color);
		if (hairColorObj != null)  {
			hairColorObj.GetComponent<Outline>().enabled = false;
		}
		hairColorObj = newHairColor;
		if (hairColorObj != null)  {
			hairColorObj.GetComponent<Outline>().enabled = true;
		}
	}
    private void setHairColor(Color newHairColor) {
        hairColor = newHairColor;
     //   hairGameObject.GetComponent<SpriteRenderer>().color = hairColor;
		hairSprite.color = hairColor;
        for (int i = 0; i < hairTransform.childCount; i++) {
            hairTransform.GetChild(i).GetComponent<Image>().color = hairColor;
        }
        paperdollHair.color = newHairColor;
    }

	public void setHairType(int newHairStyle)  {
		hairStyle = newHairStyle;
		setHairStyle(newHairStyle);
	}

	void setHairStyle(int newHairStyle)  {
	//	if (hairGameObject != null)  {
	//		GameObject.Destroy(hairGameObject);
	//	}
	//	hairGameObject = Instantiate(Resources.Load<GameObject>("Units/Hair/" + PersonalInformation.hairTypes[newHairStyle])) as GameObject;
	//	hairGameObject.transform.parent = characterSprite.transform;
	//	hairGameObject.transform.localPosition = new Vector3(0, 0, 0);
	//	hairGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
	//	hairGameObject.transform.localScale = new Vector3(1, 1, 1);
	//	hairSprite = hairGameObject.GetComponent<Image>();
		hairSprite.sprite = hairStyles[newHairStyle];
//		hairSprite.sortingOrder = 10;
		hairSprite.color = hairColor;
	}
	public GameObject berrindColorObj;
	public GameObject ashpianColorObj;
	public GameObject rorrulColorObj;
	public GameObject hairColorObj;
	public GameObject primaryColorObj;
	public GameObject secondaryColorObj;

	public void setBerrindSkinColor(GameObject newSkinColor)  {
		berrindColor = newSkinColor.GetComponent<Image>().color;
       if (berrindColorObj != null)  {
			berrindColorObj.GetComponent<Outline>().enabled = false;
		}
		berrindColorObj = newSkinColor;
		if (berrindColorObj != null)  {
			berrindColorObj.GetComponent<Outline>().enabled = true;
		}
		setSkinColor(berrindColor);
	}

	public void setAshpianSkinColor(GameObject newSkinColor)  {
		ashpianColor = newSkinColor.GetComponent<Image>().color;
		if (ashpianColorObj != null)  {
			ashpianColorObj.GetComponent<Outline>().enabled = false;
		}
		ashpianColorObj = newSkinColor;
		if (ashpianColorObj != null)  {
			ashpianColorObj.GetComponent<Outline>().enabled = true;
		}
		setSkinColor(ashpianColor);
	}

	public void setRorrulSkinColor(GameObject newSkinColor)  {
		rorrulColor = newSkinColor.GetComponent<Image>().color;
		if (rorrulColorObj != null)  {
			rorrulColorObj.GetComponent<Outline>().enabled = false;
		}
		rorrulColorObj = newSkinColor;
		if (rorrulColorObj != null)  {
			rorrulColorObj.GetComponent<Outline>().enabled = true;
		}
		setSkinColor(rorrulColor);
	}
    private void setSkinColor(Color skinColor) {
        characterSprite.color = skinColor;
        paperdollSkin.color = skinColor;
    }

	public void setPrimaryColor(GameObject newPrimaryColor)  {
		if(settingPrimary)  {
			primaryColor = newPrimaryColor.GetComponent<Image>().color;
			if (primaryColorObj != null)  {
				primaryColorObj.GetComponent<Outline>().enabled = false;
			}
			primaryColorObj = newPrimaryColor;
			if (primaryColorObj != null)  {
				primaryColorObj.GetComponent<Outline>().enabled = true;
			}
			shirtSprite.color = primaryColor;
            paperdollPrimary.color = primaryColor;
		}
		else  {
			secondaryColor = newPrimaryColor.GetComponent<Image>().color;
			if (secondaryColorObj != null)  {
				secondaryColorObj.GetComponent<Outline>().enabled = false;
			}
			secondaryColorObj = newPrimaryColor;
			if (secondaryColorObj != null)  {
				secondaryColorObj.GetComponent<Outline>().enabled = true;
			}
			pantsSprite.color = secondaryColor;
			shoesSprite.color = secondaryColor;
            paperdollSecondary.color = secondaryColor;
		}
	}

	public void setProperSkinDisplay()  {
		if(character.race.raceName == RaceName.Berrind)  {
			toggleAllExcept(skinColorList[0]);
		}
		else if(character.race.raceName == RaceName.Ashpian)  {
			toggleAllExcept(skinColorList[1]);
		}
		else  {
			toggleAllExcept(skinColorList[2]);
		}
		skinColorList[3].SetActive(true);
		characterSprite.gameObject.SetActive(true);
		paperdollMain.SetActive(true);
	}

	public void toggleAllExcept(GameObject thisOne)  {
		if (thisOne == null) return;
		toggleAllExcept(new GameObject[]  {thisOne});
	}

	public void toggleAllExcept(GameObject[] theseOnes)  {
		if (theseOnes.Length == 0 || theseOnes == null) return;
		ArrayList siblings = new ArrayList();
		for (int i = 0; i < theseOnes[0].transform.parent.childCount; i++)  {
			siblings.Add(theseOnes[0].transform.parent.GetChild(i).gameObject);
		}
		foreach (GameObject thisOne in theseOnes)
			siblings.Remove(thisOne);
		
		foreach(GameObject sibling in siblings)  {
			sibling.SetActive(false);
		}
		foreach (GameObject thisOne in theseOnes)  {
			thisOne.SetActive(true);
			enableButtonObject(thisOne);
		}
	}
	public void enableButtonObject(GameObject go)  {
		Button b = go.GetComponent<Button>();
		if (b != null)  {
			if (selectedButtons.Contains(b))  {
				b.GetComponent<Animator>().SetBool("Selected", true);
			}
		}
		for (int n=0; n < go.transform.childCount;n++)  {
			enableButtonObject(go.transform.GetChild(n).gameObject);
		}
	}

	public void setState(GUIState state)  {
		switch(state)  {
		case GUIState.SEX:
			toggleAllExcept(stateList[(int)GUIState.SEX]);
			showSexPanel(character.sex);
			break;
		case GUIState.RACE:
			toggleAllExcept(stateList[(int)GUIState.RACE]);
			if (character.race == null) toggleAllExcept(blankText);
			else showBackgroundPanel(character.background);
			break;
		case GUIState.PHYSICAL_FEATURES:
			toggleAllExcept(stateList[(int)GUIState.PHYSICAL_FEATURES]);
			break;
		case GUIState.CLASS:
			toggleAllExcept(stateList[(int)GUIState.CLASS]);
			if (character.cClass == null) toggleAllExcept(blankText);
			else showClassPanel(character.cClass);
			break;
		case GUIState.ABILITY_SCORES:
			toggleAllExcept(stateList[(int)GUIState.ABILITY_SCORES]);
			break;
		case GUIState.SKILLS:
			toggleAllExcept(stateList[(int)GUIState.SKILLS]);
			break;
		/*case GUIState.TALENTS:
			toggleAllExcept(stateList[(int)GUIState.TALENTS]);
			break;*/
		case GUIState.NAME:
			shouldSelectFirst = true;
			toggleAllExcept(stateList[(int)GUIState.NAME]);
			break;
		default:
			break;
		}
		progressionButtons[1].GetComponent<Button>().interactable = currentState != GUIState.SEX;
		
		progressionButtons[3].SetActive(currentState == GUIState.NAME);
		progressionButtons[2].SetActive(currentState != GUIState.NAME);
	}

	public void nextState()  {
		GameObject tempObj;
		switch(currentState)  {
		case GUIState.SEX:
		/*	if(passport.TryGetValue("Sex", out tempObj))
			 {
				character.sex = (tempObj.GetComponent<Text>().text == "Male") ? CharacterSex.Male : CharacterSex.Female;
			//	progressionButtons[1].SetActive(true);
			//	progressionButtons[0].SetActive(false);
				progressionButtons[1].GetComponent<Button>().interactable = true;

			}
			else
			 {
				Debug.LogError("Sex GameObject does not exist.  You suck.");
			}*/
			break;
		case GUIState.RACE:
			/*
			if(passport.TryGetValue("Race", out tempObj))
			 {
				if(tempObj.GetComponent<Text>().text == "Ashpian")
				 {
					character.race = new Race_Ashpian();
					setSkinColor(ashpianColor);
				}
				else if(tempObj.GetComponent<Text>().text == "Berrind")
				 {
					character.race = new Race_Berrind();
					setSkinColor(berrindColor);
				}
				else
				 {
					character.race = new Race_Rorrul();
					setSkinColor(rorrulColor);
				}
			}*/
			/*if(passport.TryGetValue("Background", out tempObj))
			 {
				switch(tempObj.GetComponent<Text>().text)
				 {
				case "Commoner":
					character.background = CharacterBackground.Commoner;
					break;
				case "Immigrant":
					character.background = CharacterBackground.Immigrant;
					break;
				case "Fallen Noble":
					character.background = CharacterBackground.FallenNoble;
					break;
				case "White Gem":
					character.background = CharacterBackground.WhiteGem;
					break;
				case "Servant":
					character.background = CharacterBackground.Servant;
					break;
				case "Unknown":
					character.background = CharacterBackground.Unknown;
					break;
				default:
					break;
				}
			}
			else
			 {
				Debug.LogError("Race GameObject does not exist.  You suck.");
			}*/
		//	toggleAllExcept(blankText);
			break;
		case GUIState.CLASS:
		/*	if(passport.TryGetValue("Class", out tempObj))
			 {
				switch(tempObj.GetComponent<Text>().text)
				 {
				case "Ex-Soldier":
					character.cClass = new Class_ExSoldier();
					break;
				case "Engineer":
					character.cClass = new Class_Engineer();
					break;
				case "Investigator":
					character.cClass = new Class_Investigator();
					break;
				case "Researcher":
					character.cClass = new Class_Researcher();
					break;
				case "Orator":
					character.cClass = new Class_Orator();
					break;
				default:
					break;
				}
			}
			else
			 {
				Debug.LogError("Class GameObject does not exist.  You suck.");
			}*/
		//setsta	showClassPanel(character.cClass);
			break;
		case GUIState.ABILITY_SCORES:
		//	toggleAllExcept(blankText);
			break;
		case GUIState.SKILLS:
		//	progressionButtons[3].SetActive(true);
		//	progressionButtons[2].SetActive(false);
		//	toggleAllExcept(blankText);
			break;
		case GUIState.NAME:
		//	toggleAllExcept(blankText);
			break;
		default:
		//	toggleAllExcept(blankText);
			break;
		}
		toggleAllExcept(blankText);

		if(currentState != GUIState.NAME)  {
			setState(++currentState);
			if(currentState >= furthestState)
			 {
				furthestState = currentState;
			}
		}
	}

	public void previousState()  {
		toggleAllExcept(blankText);
		if(currentState == GUIState.NAME)  {
			setState(--currentState);
			progressionButtons[2].SetActive(true);
			progressionButtons[3].SetActive(false);
		}
		else if(currentState > GUIState.RACE && currentState < GUIState.NAME)  {
			setState(--currentState);
		}
		else if (currentState == GUIState.RACE)  {
			setState(--currentState);
//			progressionButtons[0].SetActive(true);
//			progressionButtons[1].SetActive(false);
//			progressionButtons[1].GetComponent<Button>().interactable = false;

		}
	//	toggleAllExcept(blankText);
	}

	public void exitCharacterCreation()  {
		Application.LoadLevel(PlayerPrefs.GetInt("playercreatefrom"));
	}

	const string delimiter = ";";
	bool saving = false;
	public void finishCharacterCreation()  {
		if (saving) return;
		string characterId = Saves.getnewCharacterUUID();
		saving = true;
		Color raceColor = Color.white;
		string characterStr = "";
		//Character first and last name
		characterStr += characterName + delimiter;
		characterStr += characterLastName + delimiter;
		//Character Sex: 0 = Male, 1 = Female
		if(character.sex == CharacterSex.Male)  {
			characterStr += "0" + delimiter;
		}
		else  {
			characterStr += "1" + delimiter;
		}
		//Character Race: 0 = Berrind, 1 = Ashpian, 2 = Rorrul
		switch(character.race.raceName)  {
		case RaceName.Berrind:
			characterStr += "0" + delimiter;
			raceColor = berrindColor;
			break;
		case RaceName.Ashpian:
			characterStr += "1" + delimiter;
			raceColor = ashpianColor;
			break;
		case RaceName.Rorrul:
			characterStr += "1" + delimiter;
			raceColor = rorrulColor;
			break;
		default:
			break;
		}
		//Character Background (contextualized by race): 0 = Fallen Noble | Commoner | Servant, 1 = White Gem | Immigrant | Unknown
		if(character.background == CharacterBackground.FallenNoble ||
		   character.background == CharacterBackground.Commoner ||
		   character.background == CharacterBackground.Servant)  {
			characterStr += "0" + delimiter;
		}
		else  {
			characterStr += "1" + delimiter;
		}
		//Age, Height, Weight are unused, but still in the character sheet
		characterStr += "0" + delimiter;
		characterStr += "0" + delimiter;
		characterStr += "0" + delimiter;
		//Character Class: 0 = Ex-Soldier, 1 = Engineer, 2 = Investigator, 3 = Researcher, 4 = Orator
		switch(character.cClass.getClassName())  {
		case ClassName.ExSoldier:
			characterStr += "0" + delimiter;
			break;
		case ClassName.Engineer:
			characterStr += "1" + delimiter;
			break;
		case ClassName.Investigator:
			characterStr += "2" + delimiter;
			break;
		case ClassName.Researcher:
			characterStr += "3" + delimiter;
			break;
		case ClassName.Orator:
			characterStr += "4" + delimiter;
			break;
		default:
			break;
		}
		int[] scores = gameObject.GetComponent<CCPointAllocation>().getScores();
		int[] skills = gameObject.GetComponent<CCPointAllocation>().getSkills();

		//Add all Ability Scores and Skills
		for(int i = 0; i < scores.Length; i++)  {
			characterStr += scores[i] + delimiter;
		}
		for(int i = 0; i < skills.Length; i++)  {
			characterStr += skills[i] + delimiter;
		}

		//Add all colors
		characterStr += colorString(raceColor);
		characterStr += colorString(hairColor);
		characterStr += colorString(primaryColor);
		characterStr += colorString(secondaryColor);
		//HairStyle
		characterStr += hairStyle + delimiter;
		//Level and Experience
		characterStr += "1;0;";
		//Add in any copper the character may start with
		if(character.background == CharacterBackground.FallenNoble)  {
			characterStr += "50" + delimiter;
		}
		else if(character.background == CharacterBackground.Commoner)  {
			characterStr += "10" + delimiter;
		}
		else if(character.background == CharacterBackground.Servant)  {
			characterStr += "30" + delimiter;
		}
		else  {
			characterStr += "0" + delimiter;
		}
		//Add in health and composure
		characterStr += gameObject.GetComponent<CCPointAllocation>().calculateHealth() + delimiter;
		characterStr += gameObject.GetComponent<CCPointAllocation>().calculateComposure() + delimiter;
		//number of chosen features followed by each one.
		characterStr += "0;";
		//Weapon Focus:
		characterStr += "0;";
		//Number of inventory items followed by that many items.
		CharacterLoadoutActual loadout = new CharacterLoadoutActual();
		loadout.setItemInSlot(InventorySlot.Chest, clothChest.GetComponent<ItemArmor>().getItem());
		loadout.setItemInSlot(InventorySlot.Boots, clothBoots.GetComponent<ItemArmor>().getItem());
		loadout.setItemInSlot(InventorySlot.Pants, clothPants.GetComponent<ItemArmor>().getItem());
		switch (character.background)  {
		case CharacterBackground.FallenNoble:
			loadout.setItemInSlot(InventorySlot.Chest, paddedVest.GetComponent<ItemArmor>().getItem());
			break;
		case CharacterBackground.WhiteGem:
			loadout.setItemInSlot(InventorySlot.RightHand, handAxe.GetComponent<ItemWeapon>().getItem());
			loadout.setItemInSlot(InventorySlot.Head, leatherCap.GetComponent<ItemArmor>().getItem());
			break;
		case CharacterBackground.Commoner:
			loadout.setItemInSlot(InventorySlot.RightHand, plank.GetComponent<ItemWeapon>().getItem());
			break;
		case CharacterBackground.Immigrant:
			loadout.setItemInSlot(InventorySlot.RightHand, shortSword.GetComponent<ItemWeapon>().getItem());
			loadout.setItemInSlot(InventorySlot.Shoulder, leatherSpaulder.GetComponent<ItemArmor>().getItem());
			break;
		case CharacterBackground.Unknown:
			loadout.setItemInSlot(InventorySlot.Head, clothHood.GetComponent<ItemArmor>().getItem());
			loadout.setItemInSlot(InventorySlot.RightHand, dagger.GetComponent<ItemWeapon>().getItem());
			break;
		default:
			break;
		}
//		characterStr += "0;";
		string inventoryString = "";
		int inventorySize = 0;
		
		if (character.cClass is Class_Engineer)  {
			inventorySize++;
			inventoryString += "0;";
			if (turret)  {
				Turret t = startingTurret.GetComponent<ItemMechanicalEditor>().getItem() as Turret;
				t.creatorId = characterId;
				inventoryString += (int)t.getItemCode() + delimiter;
				inventoryString += t.getItemData() + delimiter;
			}
			else  {
				Trap t = startingTrap.GetComponent<ItemMechanicalEditor>().getItem() as Trap;
				t.creatorId = characterId;
				inventoryString += (int)t.getItemCode() + delimiter;
				inventoryString += t.getItemData() + delimiter;
			}
		}
		foreach (InventorySlot slot in UnitGUI.armorSlots)  {
			Item i = loadout.getItemInSlot(slot);
			if (i != null)  {
				inventorySize++;
				inventoryString += Character.getArmorSlotIndex(slot) + delimiter;
				inventoryString += (int)i.getItemCode() + delimiter;
				inventoryString += i.getItemData() + delimiter;
			}
		}
		characterStr += inventorySize + delimiter + inventoryString;
		//Favored Race
		characterStr += "0;";
		Saves.addCharacter(characterStr, characterId);
		if (Application.loadedLevel != 2)
			Application.LoadLevel(2);
	}

	static string colorString(Color c)  {
		return ((int)(c.r*255)) + delimiter + ((int)(c.g*255)) + delimiter + ((int)(c.b*255)) + delimiter;
	}

	public void isNextAvailable()  {
		Debug.Log("Next available");
		switch (currentState)  {
		case GUIState.CLASS:
			progressionButtons[2].GetComponent<Button>().interactable = character.cClass != null;
			return;
		case GUIState.NAME:
			bool next = !string.IsNullOrEmpty(characterName) && !string.IsNullOrEmpty(characterLastName);
			Debug.Log("Next One: " + next + "|" + characterName + "|" + characterLastName + "|");
			progressionButtons[3].GetComponent<Button>().interactable = next;
			return;
		default:
			break;
		}
		if(currentState < GUIState.ABILITY_SCORES)  {
			progressionButtons[2].GetComponent<Button>().interactable = (currentState < furthestState);
		}
		if(currentState == GUIState.PHYSICAL_FEATURES)  {
			setProperSkinDisplay();
			progressionButtons[2].GetComponent<Button>().interactable = true;
		}
	}


}
