using UnityEngine;
using System.Collections;

namespace CharacterInfo
{
	public class ClassFeatures
	{
		public static string getName(ClassFeature feature) {
			switch (feature) {
			case ClassFeature.Decisive_Strike:
				return "Decisive Strike";
				goto case ClassFeature.Decisive_Strike;
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
			default:
				return feature.ToString();
			}
		}
	}
}


