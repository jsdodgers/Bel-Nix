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
            if (canLevelUp())
            {
                cExperience -= cLevel * LEVEL_COEFFICIENT;
                cLevel++;
            }
            return cLevel; 
        }
		public bool canLevelUp() {
			return cExperience > cLevel * LEVEL_COEFFICIENT;
		}
		public int setLevel(int level)					{ return cLevel = level; }
		public ClassFeature[] getClassFeatures() 		{ return getCharacterClass().getClassFeatures(cLevel); }
		public bool hasFeature(ClassFeature feature)	{ return Array.IndexOf(getClassFeatures(),feature)>=0; }
		public DamageType getWeaponFocus()				{ return weaponFocus; }
		public void setWeaponFocus(DamageType type)		{ weaponFocus = type; }
		public void setWeaponFocus(int focus) {
			switch (focus) {
			case 1:
				weaponFocus = DamageType.Piercing;
				break;
			case 2:
				weaponFocus = DamageType.Slashing;
				break;
			case 3:
				weaponFocus = DamageType.Crushing;
				break;
			default:
				weaponFocus = DamageType.None;
				break;
			}
		}
		public int getWeaponFocusAsNumber() {
			switch (weaponFocus) {
			case DamageType.Piercing:
				return 1;
			case DamageType.Slashing:
				return 2;
			case DamageType.Crushing:
				return 3;
			default:
				return 0;
			}
		}
	}
}

