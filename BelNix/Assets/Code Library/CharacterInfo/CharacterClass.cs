using System;
namespace CharacterInfo
{
	public abstract class CharacterClass
	{
		// Class stat modifiers
			// Health
			// Composure
			// Athletics
			// Melee
			// Ranged
			// Stealth
			// Mechanical
			// Medicinal
			// Historical
			// Political
		protected ClassModifiers cModifiers;
		public ClassModifiers getClassModifiers() {return cModifiers;}

		// 
	}

	public class Class_ExSoldier : CharacterClass
	{
		public Class_ExSoldier()
		{cModifiers = new ClassModifiers(2, 0, 1, 0, 1, 0, 0, 0, 0, 0);}
	}
	public class Class_Engineer : CharacterClass
	{
		public Class_Engineer()
		{cModifiers = new ClassModifiers(0, 0, 0, 0, 0, 0, 2, 0, 0, 0);}
	}
	public class Class_Investigator : CharacterClass
	{
		public Class_Investigator()
		{cModifiers = new ClassModifiers(1, 1, 0, 1, 0, 1, 0, 0, 0, 0);}
	}
	public class Class_Researcher : CharacterClass
	{
		public Class_Researcher()
		{cModifiers = new ClassModifiers(0, 2, 0, 0, 0, 0, 0, 1, 1, 0);}
	}
	public class Class_Orator : CharacterClass
	{
		public Class_Orator()
		{cModifiers = new ClassModifiers(0, 0, 0, 0, 0, 0, 0, 0, 0, 2);}
	}


	public struct ClassModifiers
	{
		int mHealth, mComposure, mAthletics, mMelee, mRanged, mStealth, mMechanical, mMedicinal, mHistorical, mPolitical;
		public ClassModifiers(int health, int composure, int athletics, int melee, 
		               int ranged, int stealth, int mechanical, int medicinal, 
		               int historical, int political)
		{
			mHealth 	= health;
			mComposure 	= composure;
			mAthletics 	= athletics;
			mMelee 		= melee;
			mRanged 	= ranged;
			mStealth 	= stealth;
			mMechanical = mechanical;
			mMedicinal 	= medicinal;
			mHistorical = historical;
			mPolitical 	= political;
		}
		public int getHealthModifier()		{return mHealth;}
		public int getComposureModifier()	{return mComposure;}
		public int getAthleticsModifier()	{return mAthletics;}
		public int getMeleeModifier()		{return mMelee;}
		public int getRangedModifier()		{return mRanged;}
		public int getStealthModifier()		{return mStealth;}
		public int getMechanicalModifier()	{return mMechanical;}
		public int getMedicinalModifier()	{return mMedicinal;}
		public int getHistoricalModifier()	{return mHistorical;}
		public int getPoliticalModifier()	{return mPolitical;}
		public int[] getSkillModifiers() {return new int[]{mAthletics, mMelee, mRanged, mStealth, mMechanical, mMedicinal, mHistorical, mPolitical};}
	}
}

