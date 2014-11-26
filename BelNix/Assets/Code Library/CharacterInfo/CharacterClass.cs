using System;
using System.Collections.Generic;
namespace CharacterInfo
{
	public enum ClassName {ExSoldier, Engineer, Investigator, Researcher, Orator, None}
	public enum ClassFeature   {Throw, Decisive_Strike, Intimidate, Weapon_Focus, Combat_Reload, Into_The_Fray, Grapple, Strike_Leg, Quick_Swap, Trained_Eye, Halting_Force, Bunker_Down, Diehard,
								Construction, Efficient_Storage,
								Mark, Sneak_Attack, 
								Uncanny_Knowledge, Trained_Medic,
								Invoke, Primal_Control,
								None}

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
		public static CharacterClass getClass(ClassName name) {
			
			switch(name)
			{
			case ClassName.ExSoldier:
				return new Class_ExSoldier();
			case ClassName.Engineer:
				return new Class_Engineer();
			case ClassName.Investigator:
				return new Class_Investigator();
			case ClassName.Researcher:
				return new Class_Researcher();
			case ClassName.Orator:
				return new Class_Orator();
			default:
				return new Class_ExSoldier();
			}
		}
		public virtual ClassName getClassName() {return ClassName.None;}
		public ClassFeature[] chosenFeatures = new ClassFeature[]{ClassFeature.Into_The_Fray, ClassFeature.Trained_Eye};
		public virtual ClassFeature[] getPossibleFeatures(int level) {return new ClassFeature[]{};}
		public virtual ClassFeature[] getClassFeatures(int level) {return new ClassFeature[]{};}
		// 
	}

	public class Class_ExSoldier : CharacterClass
	{
		public Class_ExSoldier()
		{cModifiers = new ClassModifiers(2, 0, 1, 0, 1, 0, 0, 0, 0, 0);}
		public override ClassName getClassName() {return ClassName.ExSoldier;}
		public override ClassFeature[] getClassFeatures(int level) {
			List<ClassFeature> features = new List<ClassFeature>();
			switch (level) {
			case 10:
				features.Insert(0, ClassFeature.Diehard);
				goto case 9;
			case 9:
				features.Insert(0, ClassFeature.Bunker_Down);
				goto case 8;
			case 8:
				features.Insert(0, chosenFeatures[1]);
				goto case 7;
			case 7:
				features.Insert(0, ClassFeature.Quick_Swap);
				goto case 6;
			case 6:
				features.Insert(0, ClassFeature.Strike_Leg);
				goto case 5;
			case 5:
				features.Insert(0, ClassFeature.Grapple);
				goto case 4;
			case 4:
				features.Insert(0, chosenFeatures[0]);
				goto case 3;
			case 3:
				features.Insert(0, ClassFeature.Weapon_Focus);
				goto case 2;
			case 2:
				features.Insert(0, ClassFeature.Intimidate);
				goto case 1;
			case 1:
				features.Insert(0, ClassFeature.Throw);
				features.Insert(0, ClassFeature.Decisive_Strike);
				break;
			default:
				break;
			}
			return features.ToArray();
		}

		public override ClassFeature[] getPossibleFeatures(int level) {
			switch (level) {
			case 8:
				return new ClassFeature[]{ClassFeature.Trained_Eye, ClassFeature.Halting_Force};
			case 4:
				return new ClassFeature[]{ClassFeature.Combat_Reload, ClassFeature.Into_The_Fray};
			default:
				return new ClassFeature[]{};
			}
		}

	}
	public class Class_Engineer : CharacterClass
	{
		public Class_Engineer()
		{cModifiers = new ClassModifiers(0, 0, 0, 0, 0, 0, 2, 0, 0, 0);}
		public override ClassName getClassName() {return ClassName.Engineer;}
		public override ClassFeature[] getClassFeatures(int level) {return new ClassFeature[]{ClassFeature.Construction, ClassFeature.Efficient_Storage};}
	}
	public class Class_Investigator : CharacterClass
	{
		public Class_Investigator()
		{cModifiers = new ClassModifiers(1, 1, 0, 1, 0, 1, 0, 0, 0, 0);}
		public override ClassName getClassName() {return ClassName.Investigator;}
		public override ClassFeature[] getClassFeatures(int level) {return new ClassFeature[]{ClassFeature.Mark, ClassFeature.Sneak_Attack};}
	}
	public class Class_Researcher : CharacterClass
	{
		public Class_Researcher()
		{cModifiers = new ClassModifiers(0, 2, 0, 0, 0, 0, 0, 1, 1, 0);}
		public override ClassName getClassName() {return ClassName.Researcher;}
		public override ClassFeature[] getClassFeatures(int level) {return new ClassFeature[]{ClassFeature.Uncanny_Knowledge, ClassFeature.Trained_Medic};}
	}
	public class Class_Orator : CharacterClass
	{
		public Class_Orator()
		{cModifiers = new ClassModifiers(0, 0, 0, 0, 0, 0, 0, 0, 0, 2);}
		public override ClassName getClassName() {return ClassName.Orator;}
		public override ClassFeature[] getClassFeatures(int level) {return new ClassFeature[]{ClassFeature.Invoke, ClassFeature.Primal_Control};}
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

