using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCGUI : MonoBehaviour
{
	public enum GUIState {SEX, RACE, /*PHYSICAL_FEATURES,*/ CLASS, ABILITY_SCORES, SKILLS, /*TALENTS,*/ NAME};
	[SerializeField] private GameObject[] stateList;
	private GUIState currentState;
	// Use this for initialization
	void Start()
	{
		currentState = GUIState.SEX;
		setState(currentState);
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
		if(currentState != GUIState.NAME)
		{
			setState(++currentState);
		}
	}

	public void previousState()
	{
		if(currentState != GUIState.SEX)
		{
			Debug.Log("currentState is: " + currentState);
			setState(--currentState);
			Debug.Log("currentState is now after --ing: " + currentState);
		}
	}
}
