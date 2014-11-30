using UnityEngine;
using System.Collections;

namespace CharacterInfo
{
	public class ClassFeatures
	{
		public static string getName(ClassFeature feature) {
			switch (feature) {

//**********EX-SOLDIER**********\\
			case ClassFeature.Decisive_Strike:
				return "Decisive Strike";
			case ClassFeature.Throw:
				return "Throw";
			case ClassFeature.Intimidate:
				return "Intimidate";
			case ClassFeature.Weapon_Focus:
				return "Weapon Focus";
			case ClassFeature.Combat_Reload:
				return "Combat Reload";
			case ClassFeature.Into_The_Fray:
				return "Into The Fray";
			case ClassFeature.Grapple:
				return "Grapple";
			case ClassFeature.Strike_Leg:
				return "Strike| Leg";
			case ClassFeature.Quick_Swap:
				return "Quick Swap";
			case ClassFeature.Trained_Eye:
				return "Trained Eye";
			case ClassFeature.Halting_Force:
				return "Halting Force";
			case ClassFeature.Bunker_Down:
				return "Bunker Down";
			case ClassFeature.Diehard:
				return "Diehard";

//************ENGINEER************\\

//**********INVESTIGATOR**********\\
			case ClassFeature.Mark:
				return "Mark";
			case ClassFeature.Sneak_Attack:
				return "Sneak Attack";
			case ClassFeature.Escape:
				return "Escape";
			case ClassFeature.Quick_Draw:
				return "Quick Draw";
			case ClassFeature.Loaded_Deck:
				return "Loaded Deck";
			case ClassFeature.Dual_Wield:
				return "Dual Wield";
			case ClassFeature.Reversal:
				return "Reversal";
			case ClassFeature.Strike_Hand:
				return "Strike| Hand";
			case ClassFeature.Acrobat:
				return "Acrobat";
			case ClassFeature.Feint:
				return "Feint";
			case ClassFeature.Dirty_Fighting:
				return "Dirty Fighting";
			case ClassFeature.Sunder:
				return "Sunder";
			case ClassFeature.Execute:
				return "Execute";
			default:
				return feature.ToString();
			}
		}
	}
}


