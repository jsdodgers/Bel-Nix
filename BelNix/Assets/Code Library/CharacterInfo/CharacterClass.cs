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
		public ClassModifiers CLASS_MODIFIERS() {return cModifiers;}

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
		public int HEALTH_MODIFIER()		{return mHealth;}
		public int COMPOSURE_MODIFIER()	{return mComposure;}
		public int ATHLETICS_MODIFIER()	{return mAthletics;}
		public int MELEE_MODIFIER()		{return mMelee;}
		public int RANGED_MODIFIER()		{return mRanged;}
		public int STEALTH_MODIFIER()		{return mStealth;}
		public int MECHANICAL_MODIFIER()	{return mMechanical;}
		public int MEDICINAL_MODIFIER()	{return mMedicinal;}
		public int HISTORICAL_MODIFIER()	{return mHistorical;}
		public int POLITICAL_MODIFIER()	{return mPolitical;}
		public int[] SKILL_MODIFERS() {return new int[]{mAthletics, mMelee, mRanged, mStealth, mMechanical, mMedicinal, mHistorical, mPolitical};}
	}
}

