using System;
using System.Collections;
namespace CharacterInfo
{
	public class CharacterProgress
	{
		private CharacterClass cClass;
		private int cLevel;
		private int cExperience;
		private DamageType weaponFocus;
        public const int LEVEL_COEFFICIENT = 100;

		public CharacterProgress (CharacterClass characterClass)
		{
			cClass		= characterClass;
			cLevel 		= 1;
			cExperience = 0;
			weaponFocus = DamageType.None;
		}
		public CharacterClass getCharacterClass()	 	{ return cClass; }
		public int getCharacterLevel() 					{ return cLevel; }
		public int getCharacterExperience() 			{ return cExperience; }
		public int addExperience(int exp)				{ return cExperience += exp; }
		public int setExperience(int exp)				{ return cExperience = exp; }
		public int incrementLevel()						
        {
            if (cExperience > cLevel * LEVEL_COEFFICIENT)
            {
                cExperience -= cLevel * LEVEL_COEFFICIENT;
                cLevel++;
            }
            return cLevel; 
        }
		public int setLevel(int level)					{ return cLevel = level; }
		public ClassFeature[] getClassFeatures() 		{ return getCharacterClass().getClassFeatures(cLevel); }
		public bool hasFeature(ClassFeature feature)	{ return Array.IndexOf(getClassFeatures(),feature)>=0; }
		public DamageType getWeaponFocus()				{ return weaponFocus; }
		public void setWeaponFocus(DamageType type)		{ weaponFocus = type; }
	}
}

