using UnityEngine;
using System;
using System.Collections.Generic;
public enum ClassName {ExSoldier, Engineer, Investigator, Researcher, Orator, None}
public enum ClassFeature   {Throw, Decisive_Strike, Intimidate, Weapon_Focus, Combat_Reload, Into_The_Fray, Grapple, Strike_Leg, Quick_Swap, Trained_Eye, Halting_Force, Bunker_Down, Diehard,
							Construction, Efficient_Storage, Metallic_Affinity, Over_Clock, Trap_Specialist, Turret_Specialist, Danger_Close, 
							Mark, Sneak_Attack, Escape, Quick_Draw, Loaded_Deck, Dual_Wield, Reversal, Strike_Hand, Acrobat, Feint, Dirty_Fighting, Sunder, Execute,
							Uncanny_Knowledge, Trained_Medic, Tempered_Hands, Favored_Race, Strike_Chest, Brush_With_Death, Quick_Operation,
							Invoke, Primal_Control, One_Of_Many, Instill_Paranoia, Terrify, Loud_Voice, Demoralize,
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
//	public ClassFeature[] chosenFeatures = new ClassFeature[]{ClassFeature.Into_The_Fray, ClassFeature.Trained_Eye};
	public int[] chosenFeatures;
	public virtual ClassFeature[] getPossibleFeatures(int level) {return new ClassFeature[]{};}
	public ClassFeature[] getClassFeatures(int level) {
		List<ClassFeature> features = new List<ClassFeature>();
		for (int n=1;n<=level;n++) {
			ClassFeature[] levelFeats = getPossibleFeatures(n);
			if (levelFeats.Length==0) continue;
			if (n%4==0) {
				if (chosenFeatures.Length<=n/4-1) continue;
				features.Add(levelFeats[chosenFeatures[n/4-1]]);
			}
			else features.AddRange(levelFeats);
		}
		return features.ToArray();
	}
}

public class Class_ExSoldier : CharacterClass
{
	public Class_ExSoldier()
	{cModifiers = new ClassModifiers(2, 0, 1, 0, 1, 0, 0, 0, 0, 0);}
	public override ClassName getClassName() {return ClassName.ExSoldier;}
	 

	public override ClassFeature[] getPossibleFeatures(int level) {
		switch (level) {
		case 10:
			return new ClassFeature[]{ClassFeature.Diehard};
		case 9:
			return new ClassFeature[]{ClassFeature.Bunker_Down};
		case 8:
			return new ClassFeature[]{ClassFeature.Trained_Eye, ClassFeature.Halting_Force};
		case 7:
			return new ClassFeature[]{ClassFeature.Quick_Swap};
		case 6:
			return new ClassFeature[]{ClassFeature.Strike_Leg};
		case 5:
			return new ClassFeature[]{ClassFeature.Grapple};
		case 4:
			return new ClassFeature[]{ClassFeature.Combat_Reload, ClassFeature.Into_The_Fray};
		case 3:
			return new ClassFeature[]{ClassFeature.Weapon_Focus};
		case 2:
			return new ClassFeature[]{ClassFeature.Intimidate};
		case 1:
			return new ClassFeature[]{ClassFeature.Decisive_Strike, ClassFeature.Throw};
		default:
			return new ClassFeature[]{};
		}
	}

}
public class Class_Engineer : CharacterClass
{
	public Class_Engineer()
	{cModifiers = new ClassModifiers(1, 1, 0, 0, 0, 0, 2, 0, 0, 0);}
	public override ClassName getClassName() {return ClassName.Engineer;}

	public override ClassFeature[] getPossibleFeatures(int level) {
		switch (level) {
		case 5:
			return new ClassFeature[]{ClassFeature.Danger_Close};
		case 4:
			return new ClassFeature[]{ClassFeature.Trap_Specialist, ClassFeature.Turret_Specialist};
		case 3:
			return new ClassFeature[]{ClassFeature.Over_Clock};
		case 2:
			return new ClassFeature[]{ClassFeature.Metallic_Affinity};
		case 1:
			return new ClassFeature[]{ClassFeature.Construction, ClassFeature.Efficient_Storage};
		default:
			return new ClassFeature[]{};
		}
	}
}
public class Class_Investigator : CharacterClass
{
	public Class_Investigator()
	{cModifiers = new ClassModifiers(1, 1, 0, 1, 0, 1, 0, 0, 0, 0);}
	public override ClassName getClassName() {return ClassName.Investigator;}
	
	public override ClassFeature[] getPossibleFeatures(int level) {
		switch (level) {
		case 10:
			return new ClassFeature[]{ClassFeature.Execute};
		case 9:
			return new ClassFeature[]{ClassFeature.Sunder};
		case 8:
			return new ClassFeature[]{ClassFeature.Feint, ClassFeature.Dirty_Fighting};
		case 7:
			return new ClassFeature[]{ClassFeature.Acrobat};
		case 6:
			return new ClassFeature[]{ClassFeature.Strike_Hand};
		case 5:
			return new ClassFeature[]{ClassFeature.Reversal};
		case 4:
			return new ClassFeature[]{ClassFeature.Loaded_Deck, ClassFeature.Dual_Wield};
		case 3:
			return new ClassFeature[]{ClassFeature.Quick_Draw};
		case 2:
			return new ClassFeature[]{ClassFeature.Escape};
		case 1:
			return new ClassFeature[]{ClassFeature.Mark, ClassFeature.Sneak_Attack};
		default:
			return new ClassFeature[]{};
		}
	}
}
public class Class_Researcher : CharacterClass
{
	public Class_Researcher()
	{cModifiers = new ClassModifiers(0, 2, 0, 0, 0, 0, 0, 1, 1, 0);}
	public override ClassName getClassName() {return ClassName.Researcher;}

	public override ClassFeature[] getPossibleFeatures(int level) {
		switch (level) {
		case 5:
			return new ClassFeature[]{ClassFeature.Quick_Operation};
		case 4:
			return new ClassFeature[]{ClassFeature.Strike_Chest, ClassFeature.Brush_With_Death};
		case 3:
			return new ClassFeature[]{ClassFeature.Favored_Race};
		case 2:
			return new ClassFeature[]{ClassFeature.Tempered_Hands};
		case 1:
			return new ClassFeature[]{ClassFeature.Uncanny_Knowledge, ClassFeature.Trained_Medic};
		default:
			return new ClassFeature[]{};
		}
	}
}
public class Class_Orator : CharacterClass
{
	public Class_Orator()
	{cModifiers = new ClassModifiers(0, 2, 0, 0, 0, 0, 0, 0, 0, 2);}
	public override ClassName getClassName() {return ClassName.Orator;}
	
	public override ClassFeature[] getPossibleFeatures(int level) {
		switch (level) {
		case 5:
			return new ClassFeature[]{ClassFeature.Demoralize};
		case 4:
			return new ClassFeature[]{ClassFeature.Terrify, ClassFeature.Loud_Voice};
		case 3:
			return new ClassFeature[]{ClassFeature.Instill_Paranoia};
		case 2:
			return new ClassFeature[]{ClassFeature.One_Of_Many};
		case 1:
			return new ClassFeature[]{ClassFeature.Invoke, ClassFeature.Primal_Control};
		default:
			return new ClassFeature[]{};
		}
	}

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


