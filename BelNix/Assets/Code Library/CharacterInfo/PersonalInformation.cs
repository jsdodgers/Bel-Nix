using System;
using UnityEngine;

public struct 	CharacterName 
{ 
	public string firstName;
	public string lastName;
	public CharacterName(string fN, string lN) {firstName = fN; lastName = lN;}
	public string fullName() { return firstName + " " + lastName;}
}
public enum 	CharacterSex {Male = 0, Female, Other, None}

public struct 	CharacterHeight 
{ 
	public int feet, inches;
	public CharacterHeight(int ft, int inch)
	{
		feet = ft;
		inches = inch;
	}

	public CharacterHeight(int inch) {
		feet = inch/12;
		inches = inch%12;
	}
}
public struct 	CharacterWeight 
{ 
	public int weight;
	public CharacterWeight(int lbs) { weight = lbs; }
}

public struct CharacterAge {
	public int age;
	public CharacterAge(int a) { age = a; }
}

public class CharacterHairStyle {
	public int hairStyle = 0;
	public GameObject hairStyleObject = null;
	public CharacterHairStyle(int hairStyle) { this.hairStyle = hairStyle; }
	public CharacterHairStyle(GameObject hairStyle) { this.hairStyleObject = hairStyle; }
	static GameObject[] hairPrefabs;
	
	public string getName() {
		return PersonalInformation.hairTypes[hairStyle];
	}
	public GameObject getHairPrefab() {
		if (hairStyleObject == null) {
			hairStyleObject = getHairPrefabs()[hairStyle];
		}
		return hairStyleObject;
	}
	public static GameObject[] getHairPrefabs() {
		if (hairPrefabs==null) {
			hairPrefabs = new GameObject[PersonalInformation.hairTypes.Length];
			for (int n=0;n<PersonalInformation.hairTypes.Length;n++) {
				hairPrefabs[n] = Resources.Load<GameObject>("Units/Hair/" + PersonalInformation.hairTypes[n]);
			}
		}
		return hairPrefabs;
	}
}

public class PersonalInformation
{
	private CharacterName		cName;
	private CharacterRace		cRace;
	private CharacterSex		cSex;
	private CharacterBackground cBackground;
	private CharacterHeight		cHeight;
	private CharacterWeight		cWeight;
	private CharacterAge		cAge;
	private CharacterHairStyle	cHair;

	public static string[] hairTypes = new string[]{"Hair_Short","Hair_Ponytail"};

	public PersonalInformation(CharacterName characterName, CharacterSex characterSex,
	                           CharacterRace characterRace, CharacterBackground characterBackground, CharacterAge characterAge,
	                           CharacterHeight characterHeight,CharacterWeight characterWeight, CharacterHairStyle hairStyle)
	{
		cName 		= characterName;
		cRace 		= characterRace;
		cSex 		= characterSex;
		cBackground = characterBackground;
		cHeight		= characterHeight;
		cWeight		= characterWeight;
		cAge 		= characterAge;
		cHair		= hairStyle;
	}

	public CharacterName getCharacterName() 			{ return cName; }
	public CharacterRace getCharacterRace() 			{ return cRace; }
	public CharacterSex getCharacterSex() 				{ return cSex; }
	public CharacterBackground getCharacterBackground() { return cBackground; }
	public CharacterHeight getCharacterHeight() 		{ return cHeight; }
	public CharacterWeight getCharacterWeight()			{ return cWeight; }
	public CharacterAge getCharacterAge()				{ return cAge; }
	public CharacterHairStyle getCharacterHairStyle()	{ return cHair; }
}

