using System;
namespace CharacterInfo
{
	public struct 	CharacterName 
	{ 
		public string firstName;
		public string lastName;
		public CharacterName(string fN, string lN) {firstName = fN; lastName = lN;}
		public string fullName() { return firstName + " " + lastName;}
	}
	public enum 	CharacterSex {Male, Female, Other}

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

	public class PersonalInformation
	{
		private CharacterName		cName;
		private CharacterRace		cRace;
		private CharacterSex		cSex;
		private CharacterBackground cBackground;
		private CharacterHeight		cHeight;
		private CharacterWeight		cWeight;
		private CharacterAge		cAge;

		public PersonalInformation(CharacterName characterName, CharacterSex characterSex,
		                           CharacterRace characterRace, CharacterBackground characterBackground, CharacterAge characterAge,
		                           CharacterHeight characterHeight,CharacterWeight characterWeight)
		{
			cName 		= characterName;
			cRace 		= characterRace;
			cSex 		= characterSex;
			cBackground = characterBackground;
			cHeight		= characterHeight;
			cWeight		= characterWeight;
			cAge 		= characterAge;
		}

		public CharacterName getCharacterName() 			{ return cName; }
		public CharacterRace getCharacterRace() 			{ return cRace; }
		public CharacterSex getCharacterSex() 				{ return cSex; }
		public CharacterBackground getCharacterBackground() { return cBackground; }
		public CharacterHeight getCharacterHeight() 		{ return cHeight; }
		public CharacterWeight getCharacterWeight()			{ return cWeight; }
		public CharacterAge getCharacterAge()				{ return cAge; }
	}
}

