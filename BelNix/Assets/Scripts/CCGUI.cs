using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CCGUI : MonoBehaviour
{
	public enum GUIState {SEX, RACE, PHYSICAL_FEATURES, CLASS, ABILITY_SCORES, SKILLS, /*TALENTS,*/ NAME};
	[SerializeField] private GameObject[] stateList;
	[SerializeField] private GameObject[] progressionButtons;
	private GUIState currentState;
	private GUIState furthestState;
	private CCPointAllocation pointAllocator;
	private Dictionary<string, GameObject> passport;
	[SerializeField] private GameObject[] skinColorList;

	[SerializeField] private Transform hairTransform;

	string characterName;
	string characterLastName;

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
	int hairStyle = 0;

	static Color createColor(float r, float g, float b) {
		return new Color(r, g, b);
	}

	public struct CharacterCreator
	{
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
	void Start()
	{
		currentState = GUIState.SEX;
		furthestState = currentState;
		setState(currentState);
		//pointAllocator = GameObject.Find("Panel - Ability Scores").GetComponent<CCPointAllocation>();

		passport = new Dictionary<string, GameObject>();
		passport.Add("Sex", GameObject.Find("Text - Sex"));
		passport.Add("Race", GameObject.Find("Text - Race"));
		passport.Add("Background", GameObject.Find("Text - Background"));
		passport.Add("Class", GameObject.Find("Text - Class"));
		passport.Add("Ability Scores", GameObject.Find("Text - Ability Scores"));
		passport.Add("Skills", GameObject.Find("Text - Skills"));
		passport.Add("Name", GameObject.Find("Text - Name"));

		characterSprite = GameObject.Find("Character").GetComponent<SpriteRenderer>();
		shirtSprite = GameObject.Find("Shirt").GetComponent<SpriteRenderer>();
		pantsSprite = GameObject.Find("Pants").GetComponent<SpriteRenderer>();
		shoesSprite = GameObject.Find("Shoes").GetComponent<SpriteRenderer>();

		hairColor = createColor(100/255.0f, 73/255.0f, 41/255.0f);
		berrindColor = createColor(246/255.0f, 197/255.0f, 197/255.0f);
		ashpianColor = createColor(223/255.0f, 180/255.0f, 135/255.0f);
		rorrulColor = createColor(96/255.0f, 71/255.0f, 56/255.0f);
		primaryColor = createColor(101/255.0f, 101/255.0f, 101/255.0f);
		secondaryColor = createColor(30/255.0f, 30/255.0f, 30/255.0f);

		shirtSprite.color = primaryColor;
		pantsSprite.color = secondaryColor;
		shoesSprite.color = secondaryColor;
		setHairStyle(0);
	}

	public void setFirstName(Text firstName)
	{
		characterName = firstName.text;
	}

	public void setLastName(Text lastName)
	{
		characterLastName = lastName.text;
	}

	bool settingPrimary = true;
	public void settingPrimaryColor() {settingPrimary = true;}
	public void settingSecondaryColor() {settingPrimary = false;}

	public void setHairColor(GameObject newHairColor)
	{
		hairColor = createColor(newHairColor.GetComponent<Image>().color.r,
		                        newHairColor.GetComponent<Image>().color.g,
		                        newHairColor.GetComponent<Image>().color.b);
		hairGameObject.GetComponent<SpriteRenderer>().color = hairColor;
		for(int i = 0; i < hairTransform.childCount; i++)
		{
			hairTransform.GetChild(i).GetComponent<Image>().color = hairColor;
		}
	}

	public void setHairType(int newHairStyle)
	{
		hairStyle = newHairStyle;
		setHairStyle(newHairStyle);
	}

	void setHairStyle(int newHairStyle) {
		if (hairGameObject != null) {
			GameObject.Destroy(hairGameObject);
		}
		hairGameObject = Instantiate(Resources.Load<GameObject>("Units/Hair/" + PersonalInformation.hairTypes[newHairStyle])) as GameObject;
		hairGameObject.transform.parent = characterSprite.transform;
		hairGameObject.transform.localPosition = new Vector3(0, 0, 0);
		hairGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
		hairGameObject.transform.localScale = new Vector3(1, 1, 1);
		hairSprite = hairGameObject.GetComponent<SpriteRenderer>();
		hairSprite.sortingOrder = 10;
		hairSprite.color = hairColor;
	}

	public void setBerrindSkinColor(GameObject newSkinColor)
	{
		berrindColor = createColor(newSkinColor.GetComponent<Image>().color.r,
		                           newSkinColor.GetComponent<Image>().color.g,
		                           newSkinColor.GetComponent<Image>().color.b);
		characterSprite.color = berrindColor;
	}

	public void setAshpianSkinColor(GameObject newSkinColor)
	{
		ashpianColor = createColor(newSkinColor.GetComponent<Image>().color.r,
		                           newSkinColor.GetComponent<Image>().color.g,
		                           newSkinColor.GetComponent<Image>().color.b);
		characterSprite.color = ashpianColor;
	}

	public void setRorrulSkinColor(GameObject newSkinColor)
	{
		rorrulColor = createColor(newSkinColor.GetComponent<Image>().color.r,
		                          newSkinColor.GetComponent<Image>().color.g,
		                          newSkinColor.GetComponent<Image>().color.b);
		characterSprite.color = rorrulColor;
	}

	public void setPrimaryColor(GameObject newPrimaryColor)
	{
		if(settingPrimary)
		{
			primaryColor = createColor(newPrimaryColor.GetComponent<Image>().color.r,
			                           newPrimaryColor.GetComponent<Image>().color.g,
			                           newPrimaryColor.GetComponent<Image>().color.b);
			shirtSprite.color = primaryColor;
		}
		else
		{
			secondaryColor = createColor(newPrimaryColor.GetComponent<Image>().color.r,
			                           newPrimaryColor.GetComponent<Image>().color.g,
			                           newPrimaryColor.GetComponent<Image>().color.b);
			pantsSprite.color = secondaryColor;
			shoesSprite.color = secondaryColor;
		}
	}

	public void setProperSkinDisplay()
	{
		if(character.race.raceName == RaceName.Berrind)
		{
			toggleAllExcept(skinColorList[0]);
		}
		else if(character.race.raceName == RaceName.Ashpian)
		{
			toggleAllExcept(skinColorList[1]);
		}
		else
		{
			toggleAllExcept(skinColorList[2]);
		}
		skinColorList[3].SetActive(true);
	}

	public void toggleAllExcept(GameObject thisOne)
	{
		ArrayList siblings = new ArrayList();
		for (int i = 0; i < thisOne.transform.parent.childCount; i++)
		{
			siblings.Add(thisOne.transform.parent.GetChild(i).gameObject);
		}
		siblings.Remove(thisOne);
		
		foreach(GameObject sibling in siblings)
		{
			sibling.SetActive(false);
		}
		
		thisOne.SetActive(true);
	}

	public void setState(GUIState state)
	{
		switch(state)
		{
		case GUIState.SEX:
			toggleAllExcept(stateList[(int)GUIState.SEX]);
			break;
		case GUIState.RACE:
			toggleAllExcept(stateList[(int)GUIState.RACE]);
			break;
		case GUIState.PHYSICAL_FEATURES:
			toggleAllExcept(stateList[(int)GUIState.PHYSICAL_FEATURES]);
			break;
		case GUIState.CLASS:
			toggleAllExcept(stateList[(int)GUIState.CLASS]);
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
			toggleAllExcept(stateList[(int)GUIState.NAME]);
			break;
		default:
			break;
		}
	}

	public void nextState()
	{
		GameObject tempObj;
		switch(currentState)
		{
		case GUIState.SEX:
			if(passport.TryGetValue("Sex", out tempObj))
			{
				character.sex = (tempObj.GetComponent<Text>().text == "Male") ? CharacterSex.Male : CharacterSex.Female;
				progressionButtons[1].SetActive(true);
				progressionButtons[0].SetActive(false);
			}
			else
			{
				Debug.LogError("Sex GameObject does not exist.  You suck.");
			}
			break;
		case GUIState.RACE:
			if(passport.TryGetValue("Race", out tempObj))
			{
				if(tempObj.GetComponent<Text>().text == "Ashpian")
				{
					character.race = new Race_Ashpian();
					characterSprite.color = ashpianColor;
				}
				else if(tempObj.GetComponent<Text>().text == "Berrind")
				{
					character.race = new Race_Berrind();
					characterSprite.color = berrindColor;
				}
				else
				{
					character.race = new Race_Rorrul();
					characterSprite.color = rorrulColor;
				}
			}
			if(passport.TryGetValue("Background", out tempObj))
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
			}
			break;
		case GUIState.CLASS:
			if(passport.TryGetValue("Class", out tempObj))
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
			}
			break;
		case GUIState.ABILITY_SCORES:
			break;
		case GUIState.SKILLS:
			progressionButtons[3].SetActive(true);
			progressionButtons[2].SetActive(false);
			break;
		case GUIState.NAME:
			break;
		default:
			break;
		}

		if(currentState != GUIState.NAME)
		{
			setState(++currentState);
			if(currentState >= furthestState)
			{
				furthestState = currentState;
			}
		}
	}

	public void previousState()
	{
		if(currentState == GUIState.NAME)
		{
			setState(--currentState);
			progressionButtons[2].SetActive(true);
			progressionButtons[3].SetActive(false);
		}
		else if(currentState > GUIState.RACE && currentState < GUIState.NAME)
		{
			setState(--currentState);
		}
		else if (currentState == GUIState.RACE)
		{
			setState(--currentState);
			progressionButtons[0].SetActive(true);
			progressionButtons[1].SetActive(false);
		}
	}

	public void exitCharacterCreation()
	{
		Application.LoadLevel(PlayerPrefs.GetInt("playercreatefrom"));
	}

	const string delimiter = ";";
	bool saving = false;
	public void finishCharacterCreation()
	{
		if (saving) return;
		saving = true;
		Color raceColor = Color.white;
		string characterStr = "";
		//Character first and last name
		characterStr += characterName + delimiter;
		characterStr += characterLastName + delimiter;
		//Character Sex: 0 = Male, 1 = Female
		if(character.sex == CharacterSex.Male)
		{
			characterStr += "0" + delimiter;
		}
		else
		{
			characterStr += "1" + delimiter;
		}
		//Character Race: 0 = Berrind, 1 = Ashpian, 2 = Rorrul
		switch(character.race.raceName)
		{
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
		   character.background == CharacterBackground.Servant)
		{
			characterStr += "0" + delimiter;
		}
		else
		{
			characterStr += "1" + delimiter;
		}
		//Age, Height, Weight are unused, but still in the character sheet
		characterStr += "0" + delimiter;
		characterStr += "0" + delimiter;
		characterStr += "0" + delimiter;
		//Character Class: 0 = Ex-Soldier, 1 = Engineer, 2 = Investigator, 3 = Researcher, 4 = Orator
		switch(character.cClass.getClassName())
		{
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
		for(int i = 0; i < scores.Length; i++)
		{
			characterStr += scores[i] + delimiter;
		}
		for(int i = 0; i < skills.Length; i++)
		{
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
		if(character.background == CharacterBackground.FallenNoble)
		{
			characterStr += "50" + delimiter;
		}
		else if(character.background == CharacterBackground.Commoner)
		{
			characterStr += "10" + delimiter;
		}
		else if(character.background == CharacterBackground.Servant)
		{
			characterStr += "30" + delimiter;
		}
		else
		{
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
		characterStr += "0;";
		//Favored Race
		characterStr += "0;";
		Saves.addCharacter(characterStr);

		Application.LoadLevel(2);
	}

	static string colorString(Color c)
	{
		return ((int)(c.r*255)) + delimiter + ((int)(c.g*255)) + delimiter + ((int)(c.b*255)) + delimiter;
	}

	public void isNextAvailable()
	{
		if(currentState < GUIState.ABILITY_SCORES)
		{
			progressionButtons[2].GetComponent<Button>().interactable = (currentState < furthestState);
		}
		if(currentState == GUIState.PHYSICAL_FEATURES)
		{
			setProperSkinDisplay();
			progressionButtons[2].GetComponent<Button>().interactable = true;
		}
	}


}
