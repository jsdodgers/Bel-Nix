using System;
namespace CharacterInfo
{
	public class CharacterProgress
	{
		private CharacterClass cClass;
		private int cLevel;
		private int cExperience;
		public CharacterProgress (CharacterClass characterClass)
		{
			cClass		= characterClass;
			cLevel 		= 1;
			cExperience = 0;
		}
		public CharacterClass getCharacterClass() 	{ return cClass; }
		public int getCharacterLevel() 				{ return cLevel; }
		public int getCharacterExperience() 		{ return cExperience; }
		public int addExperience(int exp)			{ return cExperience += exp; }
		public int setExperience(int exp)			{ return cExperience = exp; }
		public int incrementLevel()					{ return ++cLevel; }
		public int setLevel(int level)				{ return cLevel = level; }
		public ClassFeature[] getClassFeatures() 	{ return getCharacterClass().getClassFeatures(cLevel);}
	}
}

