using UnityEngine;
using System.Collections;


public class CharacterTemplate : MonoBehaviour
{

	public string textFile;
	public string firstName, lastName;
	public RaceName mCRaceName;
	public CharacterSex mCSex;
	public CharacterBackground mCBackground;
	public int age;
	public int height, weight;
	public int mClevel;
	public int mCexperience;
	public ClassName mCClassName;
	public int mCSturdy, mCPerception, mCTechnique, mCWellVersed;
	public int hairStyle;
	public GameObject hairPrefab;
	public Color characterColor;
	public Color headColor;
	public Color primaryColor;
	public Color secondaryColor;
	public CharacterLoadout characterLoadout;

	// Use this for initialization
	void Start ()
	{
	//	Character character = gameObject.GetComponent<Character>();

	}

	public Character loadData(Unit u) {
		return loadData(null, u);
	}

	public Character loadData(string textFile2, Unit u) {
		Character ch = new Character();
		ch.characterLoadout = characterLoadout;
		ch.unit = u;
		if (textFile2 != null && textFile2 != "") textFile = textFile2;
		if (textFile != null && textFile != "") {
			ch.loadCharacterFromTextFile(textFile);
		}
		else {
			CharacterRace mCRace = CharacterRace.getRace(mCRaceName);
			CharacterClass mCClass = CharacterClass.getClass(mCClassName);
			
			CharacterHairStyle hairSt;
			if (hairPrefab != null)
				hairSt = new CharacterHairStyle(hairPrefab);
			else hairSt = new CharacterHairStyle((hairStyle >=0 && hairStyle < PersonalInformation.hairTypes.Length ? hairStyle : 0));
			
			ch.loadCharacter(firstName, lastName, mCSex, mCRace, age,
			              mCBackground, height, weight, mCClass,
			              mCSturdy, mCPerception, mCTechnique, mCWellVersed, characterColor, headColor, primaryColor, secondaryColor, hairSt);
			int level = ch.characterProgress.setLevel(mClevel);
			int experience = ch.characterProgress.setExperience(mCexperience);
		}
		return ch;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
