using UnityEngine;
using System.Collections;
using CharacterInfo;

public enum ClassName {EX_SOLDIER, ENGINEER, INVESTIGATOR, RESEARCHER, ORATOR}
public enum RaceName {BERRIND, ASHPIAN, RORRUL}

public class EnemyTemplate : Character
{


	public string firstName, lastName;
	public RaceName mCRaceName;
	public CharacterSex mCSex;
	public CharacterBackground mCBackground;
	public int height, weight;
	public int mClevel;
	public ClassName mCClassName;
	public int mCSturdy, mCPerception, mCTechnique, mCWellVersed;

	// Use this for initialization
	void Start ()
	{
		Character character = gameObject.GetComponent<Character>();
		CharacterRace mCRace;
		CharacterClass mCClass;

		switch(mCRaceName)
		{
		case RaceName.BERRIND:
			mCRace = new Race_Berrind();
			break;
		case RaceName.ASHPIAN:
			mCRace = new Race_Ashpian();
			break;
		case RaceName.RORRUL:
			mCRace = new Race_Rorrul();
			break;
		default:
			mCRace = new Race_Ashpian();
			break;
		}

		switch(mCClassName)
		{
		case ClassName.EX_SOLDIER:
			mCClass = new Class_ExSoldier();
			break;
		case ClassName.ENGINEER:
			mCClass = new Class_Engineer();
			break;
		case ClassName.INVESTIGATOR:
			mCClass = new Class_Investigator();
			break;
		case ClassName.RESEARCHER:
			mCClass = new Class_Researcher();
			break;
		case ClassName.ORATOR:
			mCClass = new Class_Orator();
			break;
		default:
			mCClass = new Class_ExSoldier();
			break;
		}

		character.loadCharacter(firstName, lastName, mCRace, mCSex,
		                        mCBackground, height, weight, mCClass,
		                        mCSturdy, mCPerception, mCTechnique, mCWellVersed);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
