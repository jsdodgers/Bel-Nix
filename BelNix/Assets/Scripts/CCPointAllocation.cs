using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CCPointAllocation : MonoBehaviour {
	[SerializeField] private GameObject[] abilityScorePointList;
	[SerializeField] private GameObject[] modPointList;
	[SerializeField] private GameObject[] defensePointList;
	[SerializeField] private GameObject[] skillScorePointList;

	[SerializeField] private GameObject ccGUI;

	int sturdy = 1;
	int perception = 1;
	int technique = 1;
	int well_versed = 1;
	int totalPoints = 8;

	public int[] getScores() {return new int[4] {sturdy, perception, technique, well_versed};}

	int athletics = 0;
	int melee = 0;
	int ranged = 0;
	int stealth = 0;
	int mechanical = 0;
	int medicinal = 0;
	int historical = 0;
	int political = 0;
	int totalSkillPoints = 2;

	public int[] getSkills() {return new int[8] {athletics, melee, ranged, stealth, mechanical, medicinal, historical, political};}

	// Use this for initialization
	void Start () {
		abilityScorePointList[1].GetComponent<Text>().text = sturdy.ToString();
		abilityScorePointList[2].GetComponent<Text>().text = perception.ToString();
		abilityScorePointList[3].GetComponent<Text>().text = technique.ToString();
		abilityScorePointList[4].GetComponent<Text>().text = well_versed.ToString();
		abilityScorePointList[0].GetComponent<Text>().text = totalPoints.ToString();

		modPointList[0].GetComponent<Text>().text = calculateMod(sturdy).ToString();
		modPointList[1].GetComponent<Text>().text = calculateMod(perception).ToString();
		modPointList[2].GetComponent<Text>().text = calculateMod(technique).ToString();
		modPointList[3].GetComponent<Text>().text = calculateMod(well_versed).ToString();

		defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
		defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();

		skillScorePointList[1].GetComponent<Text>().text = calculateSkill(athletics, sturdy, 0).ToString();
		skillScorePointList[2].GetComponent<Text>().text = calculateSkill(melee, sturdy, 1).ToString();
		skillScorePointList[3].GetComponent<Text>().text = calculateSkill(ranged, perception, 2).ToString();
		skillScorePointList[4].GetComponent<Text>().text = calculateSkill(stealth, perception, 3).ToString();
		skillScorePointList[5].GetComponent<Text>().text = calculateSkill(mechanical, technique, 4).ToString();
		skillScorePointList[6].GetComponent<Text>().text = calculateSkill(medicinal, technique, 5).ToString();
		skillScorePointList[7].GetComponent<Text>().text = calculateSkill(historical, well_versed, 6).ToString();
		skillScorePointList[8].GetComponent<Text>().text = calculateSkill(political, well_versed, 7).ToString();
		skillScorePointList[0].GetComponent<Text>().text = totalSkillPoints.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateScores()
	{
		abilityScorePointList[1].GetComponent<Text>().text = sturdy.ToString();
		abilityScorePointList[2].GetComponent<Text>().text = perception.ToString();
		abilityScorePointList[3].GetComponent<Text>().text = technique.ToString();
		abilityScorePointList[4].GetComponent<Text>().text = well_versed.ToString();
		abilityScorePointList[0].GetComponent<Text>().text = totalPoints.ToString();
		
		modPointList[0].GetComponent<Text>().text = calculateMod(sturdy).ToString();
		modPointList[1].GetComponent<Text>().text = calculateMod(perception).ToString();
		modPointList[2].GetComponent<Text>().text = calculateMod(technique).ToString();
		modPointList[3].GetComponent<Text>().text = calculateMod(well_versed).ToString();

		defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
		defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();

		skillScorePointList[1].GetComponent<Text>().text = calculateSkill(athletics, sturdy, 0).ToString();
		skillScorePointList[2].GetComponent<Text>().text = calculateSkill(melee, sturdy, 1).ToString();
		skillScorePointList[3].GetComponent<Text>().text = calculateSkill(ranged, perception, 2).ToString();
		skillScorePointList[4].GetComponent<Text>().text = calculateSkill(stealth, perception, 3).ToString();
		skillScorePointList[5].GetComponent<Text>().text = calculateSkill(mechanical, technique, 4).ToString();
		skillScorePointList[6].GetComponent<Text>().text = calculateSkill(medicinal, technique, 5).ToString();
		skillScorePointList[7].GetComponent<Text>().text = calculateSkill(historical, well_versed, 6).ToString();
		skillScorePointList[8].GetComponent<Text>().text = calculateSkill(political, well_versed, 7).ToString();
		skillScorePointList[0].GetComponent<Text>().text = totalSkillPoints.ToString();
	}

	int calculateSkill(int skill, int abilityScore, int skillNumber)
	{
		int skillTotal = 0;

		if(ccGUI.GetComponent<CCGUI>().character.cClass != null)
		{
			skillTotal += ccGUI.GetComponent<CCGUI>().character.cClass.getClassModifiers().getSkillModifiers()[skillNumber];
		}

		skillTotal += skill;
		skillTotal += calculateMod(abilityScore);
		return skillTotal;
	}

	int calculateMod(int abilityScore)
	{
		return (int) Mathf.Floor(abilityScore/2);
	}

	public int calculateHealth()
	{
		int health = 0;

		if(ccGUI.GetComponent<CCGUI>().character.cClass != null)
		{
			health += ccGUI.GetComponent<CCGUI>().character.cClass.getClassModifiers().getHealthModifier();
		}
		if(ccGUI.GetComponent<CCGUI>().character.race != null)
		{
			health += ccGUI.GetComponent<CCGUI>().character.race.getHealthModifier();
		}
		health += sturdy;
		health += perception;

		return health;
	}

	public int calculateComposure()
	{
		int composure = 0;

		if(ccGUI.GetComponent<CCGUI>().character.cClass != null)
		{
			composure += ccGUI.GetComponent<CCGUI>().character.cClass.getClassModifiers().getComposureModifier();
		}
		if(ccGUI.GetComponent<CCGUI>().character.race != null)
		{
			composure += ccGUI.GetComponent<CCGUI>().character.race.getComposureModifier();
		}
		composure += technique;
		composure += well_versed;

		return composure;
	}

	public void addPoint(string score)
	{
		if(totalPoints > 0)
		{
			switch(score)
			{
			case "sturdy":
				sturdy++;
				abilityScorePointList[1].GetComponent<Text>().text = sturdy.ToString();
				modPointList[0].GetComponent<Text>().text = calculateMod(sturdy).ToString();
				defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
				break;
			case "perception":
				perception++;
				abilityScorePointList[2].GetComponent<Text>().text = perception.ToString();
				modPointList[1].GetComponent<Text>().text = calculateMod(perception).ToString();
				defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
				break;
			case "technique":
				technique++;
				abilityScorePointList[3].GetComponent<Text>().text = technique.ToString();
				modPointList[2].GetComponent<Text>().text = calculateMod(technique).ToString();
				defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();

				break;
			case "well-versed":
				well_versed++;
				abilityScorePointList[4].GetComponent<Text>().text = well_versed.ToString();
				modPointList[3].GetComponent<Text>().text = calculateMod(well_versed).ToString();
				defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();
				break;
			default:
				break;
			}
			adjustTotalPoints(false);
		}
	}

	public void addSkillPoint(string skill)
	{
		if(totalSkillPoints > 0)
		{
			switch(skill)
			{
			case "athletics":
				athletics++;
				skillScorePointList[1].GetComponent<Text>().text = calculateSkill(athletics, sturdy, 0).ToString();
				break;
			case "melee":
				melee++;
				skillScorePointList[2].GetComponent<Text>().text = calculateSkill(melee, sturdy, 1).ToString();
				break;
			case "ranged":
				ranged++;
				skillScorePointList[3].GetComponent<Text>().text = calculateSkill(ranged, perception, 2).ToString();
				break;
			case "stealth":
				stealth++;
				skillScorePointList[4].GetComponent<Text>().text = calculateSkill(stealth, perception, 3).ToString();
				break;
			case "mechanical":
				mechanical++;
				skillScorePointList[5].GetComponent<Text>().text = calculateSkill(mechanical, technique, 4).ToString();
				break;
			case "medicinal":
				medicinal++;
				skillScorePointList[6].GetComponent<Text>().text = calculateSkill(medicinal, technique, 5).ToString();
				break;
			case "historical":
				historical++;
				skillScorePointList[7].GetComponent<Text>().text = calculateSkill(historical, well_versed, 6).ToString();
				break;
			case "political":
				political++;
				skillScorePointList[8].GetComponent<Text>().text = calculateSkill(political, well_versed, 7).ToString();
				break;
			default:
				break;
			}
			adjustTotalSkillPoints(false);
		}
	}

	void adjustTotalPoints(bool positive)
	{
		if(positive)
		{
			totalPoints++;
		}
		else
		{
			totalPoints--;
		}
		abilityScorePointList[0].GetComponent<Text>().text = totalPoints.ToString();
	}

	void adjustTotalSkillPoints(bool positive)
	{
		if(positive)
		{
			totalSkillPoints++;
		}
		else
		{
			totalSkillPoints--;
		}
		skillScorePointList[0].GetComponent<Text>().text = totalSkillPoints.ToString();
	}

	public void subtractPoint(string score)
	{
		switch(score)
		{
		case "sturdy":
			if(sturdy > 1)
			{
				sturdy--;
				abilityScorePointList[1].GetComponent<Text>().text = sturdy.ToString();
				modPointList[0].GetComponent<Text>().text = calculateMod(sturdy).ToString();
				defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
				adjustTotalPoints(true);
			}
			break;
		case "perception":
			if(perception > 1)
			{
				perception--;
				abilityScorePointList[2].GetComponent<Text>().text = perception.ToString();
				modPointList[1].GetComponent<Text>().text = calculateMod(perception).ToString();
				defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
				adjustTotalPoints(true);
			}
			break;
		case "technique":
			if(technique > 1)
			{
				technique--;
				abilityScorePointList[3].GetComponent<Text>().text = technique.ToString();
				modPointList[2].GetComponent<Text>().text = calculateMod(technique).ToString();
				defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();
				adjustTotalPoints(true);
			}
			break;
		case "well-versed":
			if(well_versed > 1)
			{
				well_versed--;
				abilityScorePointList[4].GetComponent<Text>().text = well_versed.ToString();
				modPointList[3].GetComponent<Text>().text = calculateMod(well_versed).ToString();
				defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();
				adjustTotalPoints(true);
			}
			break;
		default:
			break;
		}
	}

	public void subtractSkillPoint(string skill)
	{
		switch(skill)
		{
		case "athletics":
			if(athletics > 0)
			{
				athletics--;
				skillScorePointList[1].GetComponent<Text>().text = calculateSkill(athletics, sturdy, 0).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "melee":
			if(melee > 0)
			{
				melee--;
				skillScorePointList[2].GetComponent<Text>().text = calculateSkill(melee, sturdy, 1).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "ranged":
			if(ranged > 0)
			{
				ranged--;
				skillScorePointList[3].GetComponent<Text>().text = calculateSkill(ranged, perception, 2).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "stealth":
			if(stealth > 0)
			{
				stealth--;
				skillScorePointList[4].GetComponent<Text>().text = calculateSkill(stealth, perception, 3).ToString();
				adjustTotalSkillPoints(true);
			}
		break;
		case "mechanical":
			if(mechanical > 0)
			{
				mechanical--;
				skillScorePointList[5].GetComponent<Text>().text = calculateSkill(mechanical, technique, 4).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "medicinal":
			if(medicinal > 0)
			{
				medicinal--;
				skillScorePointList[6].GetComponent<Text>().text = calculateSkill(medicinal, technique, 5).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "historical":
			if(historical > 0)
			{
				historical--;
				skillScorePointList[7].GetComponent<Text>().text = calculateSkill(historical, well_versed, 6).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		case "political":
			if(political > 0)
			{
				political--;
				skillScorePointList[8].GetComponent<Text>().text = calculateSkill(political, well_versed, 7).ToString();
				adjustTotalSkillPoints(true);
			}
			break;
		default:
			break;
		}
	}
}
