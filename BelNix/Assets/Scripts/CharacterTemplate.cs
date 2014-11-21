using UnityEngine;
using System.Collections;
using CharacterInfo;


public class CharacterTemplate : Character
{

	public string textFile;
	public string firstName, lastName;
	public RaceName mCRaceName;
	public CharacterSex mCSex;
	public CharacterBackground mCBackground;
	public int age;
	public int height, weight;
	public int mClevel;
	public ClassName mCClassName;
	public int mCSturdy, mCPerception, mCTechnique, mCWellVersed;

	// Use this for initialization
	void Start ()
	{
	//	Character character = gameObject.GetComponent<Character>();
		if (textFile != null && textFile != "") {
			loadCharacterFromTextFile(textFile);
		}
		else {
			CharacterRace mCRace = CharacterRace.getRace(mCRaceName);
			CharacterClass mCClass = CharacterClass.getClass(mCClassName);



			loadCharacter(firstName, lastName, mCSex, mCRace, age,
			              mCBackground, height, weight, mCClass,
			              mCSturdy, mCPerception, mCTechnique, mCWellVersed);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
