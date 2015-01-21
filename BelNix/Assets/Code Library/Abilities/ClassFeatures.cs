using UnityEngine;
using System.Collections;
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
		case ClassFeature.Construction:
			return "Construction";
		case ClassFeature.Efficient_Storage:
			return "Efficient Storage";
		case ClassFeature.Metallic_Affinity:
			return "Metallic Affinity";
		case ClassFeature.Over_Clock:
			return "Over Clock";
		case ClassFeature.Trap_Specialist:
			return "Trap Specialist";
		case ClassFeature.Turret_Specialist:
			return "Turret Specialist";
		case ClassFeature.Danger_Close:
			return "Danger Close";

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

			
//**********RESEARCHER**********\\
		case ClassFeature.Uncanny_Knowledge:
			return "Uncanny Knowledge";
		case ClassFeature.Trained_Medic:
			return "Trained Medic";
		case ClassFeature.Tempered_Hands:
			return "Tempered Hands";
		case ClassFeature.Favored_Race:
			return "Favored Race";
		case ClassFeature.Strike_Chest:
			return "Strike | Chest";
		case ClassFeature.Brush_With_Death:
			return "Brush With Death";
		case ClassFeature.Quick_Operation:
			return "Quick Operation";
			
//**********ORATOR**********\\
		case ClassFeature.Invoke:
			return "Invoke";
		case ClassFeature.Primal_Control:
			return "Primal Control";
		case ClassFeature.One_Of_Many:
			return "One Of Many";
		case ClassFeature.Instill_Paranoia:
			return "Instill Paranoia";
		case ClassFeature.Terrify:
			return "Terrify";
		case ClassFeature.Loud_Voice:
			return "Loud Voice";
		case ClassFeature.Demoralize:
			return "Demoralize";


		default:
			return feature.ToString();
		}
	}

	public static string getDescription(ClassFeature feature) {
		switch (feature) {
		case ClassFeature.Decisive_Strike:
			return "If one of the Ex-Soldier's attacks downs his target, he gains an extra standard.  This only works once per turn (you cannot down one enemy, then down another with your standard and get yet another standard).";
		case ClassFeature.Throw:
			return "On a successful attack roll, player throws an adjacent enemy a number of spaces equal to their Sturdy Mod.  Target stops upon hitting a wall or object and will be knocked prone.  If the target hits a human, that human must make an Athletics check (DC 15) or will also be knocked prone.";
		case ClassFeature.Intimidate:
			return "On a successful intimidate check (Ex-Soldier's Sturdy Mod + 1d20 vs. Opponent's Well-Versed mod + 10), the player will roll composure damage equal to the Sturdy Mod.  There will be varying success on intimidation depending on the amount of composure the enemy has remaining.";
		case ClassFeature.Weapon_Focus:
			return "The player can choose a weapon type to specialize in (Piercing, Slashing, or Crushing).  This will add a +2 to hit with all weapons within this type.";
		case ClassFeature.Combat_Reload:
			return "When using any ranged weapon that has a reload time, the player will take one less turn to reload than all other Professionals.";
		case ClassFeature.Into_The_Fray:
			return "Charge double the amount of spaces you normally do.  You do not invoke attacks of opportunity while charging and you must be able to reach your target.  Upon reaching your target, you will gain combat advantage on them until your next turn (this feature is a full-action, so you will not be able to attack the same turn you use it).  This ability can be used once per combat.";
		case ClassFeature.Grapple:
			return "On a successful attack roll, the player can now choose to hold an enemy captive for a number of turns equal to their Sturdy modifier.  Each turn an enemy is held, they can oppose the player's original roll with a melee check of their own.  As a bi-product, the player loses all ability to attack, but gains a human shield.  All attackers, except from behind, have to roll player AC + captive AC to hit the player.  If they are able to hit the captive AC, but not the total, they will attack the captive instead.  This ability requires the main hand to be open and can be used twice per combat.";

			//******ENGINEER*****\\
		case ClassFeature.Construction:
			return "Your hands naturally find their way around a workbench.  You can construct traps and turrets.  Placing a trap or a turret is considered a standard action.  Switching a turret on or off is considered a minor action.";
		case ClassFeature.Efficient_Storage:
			return "All things mechanical now stack in your inventory in sets of three.  This includes any collapsible weapons, traps, turrets, building materials, and the like.  If the item in question is concealable prior to stacking, it maintains that status (I.E. three collapsible batons that are stacked are still concealed).";
		case ClassFeature.Metallic_Affinity:
			return "Your uncanny attraction to metallic objects has lead you to have a keen eye for anything mechanical.  While observing metallic objects, you can opt to use your mechanics skill over your Perception Mod.  Additionally if the engineer is using a turret or trap that they created, the will do +1 damage with it.";
		case ClassFeature.Over_Clock:
			return "When using a mechanical weapon, the engineer can maximize the damage output to deal maximum weapon damage plus the Engineer's Technique Mod.  The weapon is no longer usable for the rest of the combat.";
		case ClassFeature.Trap_Specialist:
			return "You can now mount a trap into your shoulder slot.  When attacked, your trap will attack back with the same stats that it would normally attack with.  Shoulder-mounted traps do not require a frame to make.";
		case ClassFeature.Turret_Specialist:
			return "You can now mount a turret into your shoulder slot.  When you have a shoulder-mounted turret, you can use a standard to activate it and attack with the same stats that the turret would normally attack with.  Shoulder-mounted turrets do not require a frame to make.";
		case ClassFeature.Danger_Close:
			return "Whenever you encounter traps or explosives,  you gain an immediate round before it detonates.  This ability can be used twice per combat and is considered a Minor Action.";

			//*****INVESTIGATOR*****\\

		case ClassFeature.Mark:
			return "Your eyes and mind have been trained to the point where you can foresee where targets will head if you focus your attention on them.  This process is known as Marking and you are allowed to mark a number of targets equal to your Perception Mod.  Marking a target grants a +2 Perception (+1 Perception Mod) against your closest marked target.  If line of sight with the target is lost over the duration of a round, at the end of your turn the mark is wiped.  Marking and unmarking is considered a Minor Action.";
		case ClassFeature.Sneak_Attack:
			return "Sneak attacks are activated when the Investigator has combat advantage on their target (either from a surprise round or flanking).  When an investigator is attacking within melee range, they add their entire perception mod to their damage roll.  If the Investigator is further than melee range, they add half of their perception mod (always taking the floor) to their damage roll.";
		case ClassFeature.Escape:
			return "Once per encounter, the investigator can move 10 extra feet without evoking attacks of opportunity.";
		case ClassFeature.Quick_Draw:
			return "The investigator can make an attack roll with their stealth skill (1d20 + stealth) to attempt to draw their weapon before their adversary.  If the check is successful (vs. target's AC), the player is granted a surprise round with combat advantage on his closest target within 2 squares.  If the investigator chooses to attack, the attack automatically hits.  This ability can only be used before combat starts.";
		case ClassFeature.Loaded_Deck:
			return "The player can hold an extra weapon in their sleeve that fits into a 2x2.  This weapon takes a minor action to prepare, but does require an empty hand to equip.  Any weapon in the loaded deck is considered concealed.  You can quick-draw immediately with a weapon in your loaded deck.";
		case ClassFeature.Dual_Wield:
			return "The player can hold an extra weapon in their offhand.  When dual-wielding, all attack rolls are -2 in the main hand and -4 in the offhand.  If using a weapon that grants dual-wielding all attack rolls are 0 in the main hand and -2 in the offhand.";
		case ClassFeature.Reversal:
			return "You have a chance (1d20 + Perception Mod vs. DC of enemy attack) to strike before your enemy when being attacked.  You must be within range to attack to use this feature.  This ability can be used twice per combat.";

			//*****RESEARCHER*****\\
		case ClassFeature.Uncanny_Knowledge:
			return "You know your way around Bel Nix and its citizens.  Eventually, you will know all of their secrets.  In combat, Uncanny Knowledge can be used to give a +1 to hit.  Outside of combat, your historical roll can be used to collect obscure knowledge on the situation at hand (table pending).";
		case ClassFeature.Trained_Medic:
			return "The Researcher understands when resources are strained and can make do with the smallest amount of medicinal supplies.  This doubles the amount of uses they gain from any medicinal-based items.";
		case ClassFeature.Tempered_Hands:
			return "Knowing the boundaries of both yourself and your opponent is critical on the field of battle.  You can sacrifice 1 point to hit for 1 point of damage (and vice versa) to a maximum of your Technique Mod.  Alternatively, the Researcher can use this on medicinal rolls as well.  This can be used twice per combat and is activated with a minor action.";
		case ClassFeature.Favored_Race:
			return "You have rigorously studied one of the three races (Berrind, Ashpian, or Rorrul), allowing you to know them both inside and out.  While rolling against your favored race, you gain a +1 for all skill rolls (including melee and ranged attack rolls while in combat).";
		case ClassFeature.Strike_Chest:
			return "On a successful attack (vs. 15 + the enemies Chest AC) the Researcher can strike the chest of their enemy causing them to bleed (internally or externally) for one damage for a number of turns equal to the critical chance of their weapon divided by 5.  This can be used twice per combat.";
		case ClassFeature.Brush_With_Death:
			return "On a strike that wound normally knock the player unconscious, you can make a medicinal check (vs. 10 + damage done) to stay standing.  This leaves the player at 1 HP and can only be used if the player has more than 1 HP available.  This can be used twice per combat.";
		case ClassFeature.Quick_Operation:
			return "The Researcher can take out and use their shoulder slot as a single minor action.  This requires a free hand and can only be done twice per combat.";

			//*****ORATOR*****\\
		case ClassFeature.Invoke:
			return "If the Orator rolls successfully (1d20 + political vs. 10 + target Well-Versed Mod), they damage their target's composure for a number equal to the Orator's Well-Versed Mod.  If this occurs during combat, the damage done to the composure of the target also diminishes the target's chance of hitting the Orator.  This is considered a minor action and can be used twice per combat.";
		case ClassFeature.Primal_Control:
			return "If the Orator is the leading factor in breaking the composure of a target, they have the choice of letting the primal state fire off as normal or can veer it onto a different course.  For Rorruls, the Orator can choose to have the Rorrul attack the closest target instead of them.  For Ashpians, the Orator can take direct control of the Ashpian immediately (the alternative is that they do what they believe is best for their survival).  For Berrinds, the Orator can influence the Berrind's goal after they have triggered their primal state.";
		case ClassFeature.One_Of_Many:
			return "If the Orator is within three squares of any neutral or friendly target, they can activate this ability to alleviate any eyes from them and blend in.  This causes the Orator to be completely ignored by any threats in the area for a minute or as long as they do not take any actions (movement, standard, attacks of opportunity or otherwise).  If they are outside of any range of their allies, the Orator can use this to add their Well-Versed Mod to either Hit, Damage, or AC for one minute.  Alternatively, they can use this to increase their movement by five feet for one turn.  This is considered a minor action and can be used once per combat.";
		case ClassFeature.Instill_Paranoia:
			return "Weaving your way into your opponent's mind has always been a strong point of an Orator.  Instilling Paranoia in your target allows them to be considered neutral to you, causing them to not attack, allow you through their square, or otherwise.";
		case ClassFeature.Terrify:
			return "Your foes have heard and fear you.  You will begin to terrify all of the enemies around you.  One damage will be dealt to composure to any enemies that end their turn within 1 square of the Orator.  For each ally around the target, the damage done to composure goes up by one.";
		case ClassFeature.Loud_Voice:
			return "The range of your voice (all Orator abilities) is increased by 2 squares (10 ft).";
		case ClassFeature.Demoralize:
			return "All enemies within 2 spaces take a negative to Hit equal to their lost composure for an amount of rounds equal to your Well-Versed Mod.  This ability can be used once per combat.";


		default:
			return "";
		}
	}
}


