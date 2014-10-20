using System;
namespace CharacterInfo
{
	public struct 	CharacterName 
	{ 
		public string FIRST_NAME;
		public string LAST_NAME;
		public CharacterName(string firstName, string lastName) {FIRST_NAME = firstName; LAST_NAME = lastName;}
	}
	public enum 	CharacterSex {MALE, FEMALE, OTHER}

	public struct 	CharacterHeight 
	{ 
		public int feet, inches;
		public CharacterHeight(int ft, int inch)
		{
			feet = ft;
			inches = inch;
		}
	}
	public struct 	CharacterWeight 
	{ 
		public int weight;
		public CharacterWeight(int lbs) { weight = lbs; }
	}

	public class PersonalInformation
	{
		private CharacterName		cName;
		private CharacterRace		cRace;
		private CharacterSex		cSex;
		private CharacterBackground cBackground;
		private CharacterHeight		cHeight;
		private CharacterWeight		cWeight;

		public PersonalInformation (CharacterName characterName, CharacterRace characterRace, 
		                            CharacterSex characterSex, CharacterBackground characterBackground, 
		                            CharacterHeight characterHeight,CharacterWeight characterWeight)
		{
			cName 	= characterName;
			cRace 		= characterRace;
			cSex 		= characterSex;
			cBackground = characterBackground;
			cHeight		= characterHeight;
			cWeight		= characterWeight;
		}

		public CharacterName CHARACTER_NAME() 				{ return cName; }
		public CharacterRace CHARACTER_RACE() 				{ return cRace; }
		public CharacterSex CHARACTER_SEX() 				{ return cSex; }
		public CharacterBackground CHARACTER_BACKGROUND() 	{ return cBackground; }
		public CharacterHeight CHARACTER_HEIGHT() 			{ return cHeight; }
		public CharacterWeight CHARACTER_WEIGHT()			{ return cWeight; }
	}
}

