using System;
using System.Collections;
public class CharacterProgress {
	private CharacterClass cClass;
	private int cLevel;
	private int cExperience;
	private DamageType weaponFocus;
	private RaceName favoredRace;
public const int LEVEL_COEFFICIENT = 100;

	public CharacterProgress (CharacterClass characterClass)  {
		cClass		= characterClass;
		cLevel 		= 1;
		cExperience = 0;
		weaponFocus = DamageType.None;
		favoredRace = RaceName.None;
	}
	public CharacterClass getCharacterClass()	 	 { return cClass; }
	public int getCharacterLevel() 					 { return cLevel; }
	public int getCharacterExperience() 			 { return cExperience; }
	public int addExperience(int exp)				 { return cExperience += exp; }
	public int setExperience(int exp)				 { return cExperience = exp; }
	public int incrementLevel()						 {
    if (canLevelUp()) {
        cExperience -= cLevel * LEVEL_COEFFICIENT;
        cLevel++;
    }
    return cLevel; 
}
	public bool canLevelUp()  {
		return cExperience >= cLevel * LEVEL_COEFFICIENT;
	}
	public int setLevel(int level)					 { return cLevel = level; }
	public ClassFeature[] getClassFeatures() 		 { return getCharacterClass().getClassFeatures(cLevel); }
	public bool hasFeature(ClassFeature feature)	 { return Array.IndexOf(getClassFeatures(),feature)>=0; }
	public RaceName getFavoredRace()				 { return favoredRace; }
	public void setFavoredRace(RaceName race)		 { favoredRace = race; }
	public void setFavoredRace(int race)  {
		switch (race)  {
		case 1:
			favoredRace = RaceName.Berrind;
			break;
		case 2:
			favoredRace = RaceName.Ashpian;
			break;
		case 3:
			favoredRace = RaceName.Rorrul;
			break;
		default:
			favoredRace = RaceName.None;
			break;
		}
	}
	public int getFavoredRaceAsNumber()  {
		switch (favoredRace)  {
		case RaceName.Berrind:
			return 1;
		case RaceName.Ashpian:
			return 2;
		case RaceName.Rorrul:
			return 3;
		default:
			return 0;
		}
	}
	public DamageType getWeaponFocus()				 { return weaponFocus; }
	public void setWeaponFocus(DamageType type)		 { weaponFocus = type; }
	public void setWeaponFocus(int focus)  {
		switch (focus)  {
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
	public int getWeaponFocusAsNumber()  {
		switch (weaponFocus)  {
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

