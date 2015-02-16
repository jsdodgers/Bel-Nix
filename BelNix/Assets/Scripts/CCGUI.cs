using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CCGUI : MonoBehaviour
{
	public enum GUIState {SEX, RACE, /*PHYSICAL_FEATURES,*/ CLASS, ABILITY_SCORES, SKILLS, /*TALENTS,*/ NAME};
	[SerializeField] private GameObject[] stateList;
	private GUIState currentState;
	private GUIState furthestState;
	private CCPointAllocation pointAllocator;
	private Dictionary<string, GameObject> passport;

	private struct Character
	{
		public CharacterSex sex;
		public CharacterRace race;
		public CharacterBackground background;
		public CharacterClass cClass;
		public AbilityScores scores;
		public SkillScores skills;
		public CharacterName name;
	}

	Character character;

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
	}
	
	// Update is called once per frame
	void Update()
	{
	
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
		/*case GUIState.PHYSICAL_FEATURES:
			toggleAllExcept(stateList[(int)GUIState.PHYSICAL_FEATURES]);
			break;*/
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
				}
				else if(tempObj.GetComponent<Text>().text == "Berrind")
				{
					character.race = new Race_Berrind();
				}
				else
				{
					character.race = new Race_Rorrul();
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
		if(currentState != GUIState.SEX)
		{
			setState(--currentState);
		}
	}

	public void isNextAvailable()
	{
		if(currentState < GUIState.ABILITY_SCORES)
		{
			GameObject.Find("Button - Next").GetComponent<Button>().interactable = (currentState < furthestState);
		}
	}
}
