using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CCPointAllocation : MonoBehaviour {
	int sturdy = 1;
	int perception = 1;
	int technique = 1;
	int well_versed = 1;
	int totalPoints = 8;

	int athletics = 0;
	int melee = 0;
	int ranged = 0;
	int stealth = 0;
	int mechanical = 0;
	int medicinal = 0;
	int historical = 0;
	int political = 0;
	int totalSkillPoints = 2;

	// Use this for initialization
	void Start () {
		GameObject.Find("Text - Sturdy Points").GetComponent<Text>().text = sturdy.ToString();
		GameObject.Find("Text - Perception Points").GetComponent<Text>().text = perception.ToString();
		GameObject.Find("Text - Technique Points").GetComponent<Text>().text = technique.ToString();
		GameObject.Find("Text - Well-Versed Points").GetComponent<Text>().text = well_versed.ToString();
		GameObject.Find("Text - Total Points").GetComponent<Text>().text = totalPoints.ToString();

		GameObject.Find("Text - Athletics Points").GetComponent<Text>().text = calculateSkill(athletics, sturdy).ToString();
		GameObject.Find("Text - Melee Points").GetComponent<Text>().text = calculateSkill(melee, sturdy).ToString();
		GameObject.Find("Text - Ranged Points").GetComponent<Text>().text = calculateSkill(ranged, perception).ToString();
		GameObject.Find("Text - Stealth Points").GetComponent<Text>().text = calculateSkill(stealth, perception).ToString();
		GameObject.Find("Text - Mechanical Points").GetComponent<Text>().text = calculateSkill(mechanical, technique).ToString();
		GameObject.Find("Text - Medicinal Points").GetComponent<Text>().text = calculateSkill(medicinal, technique).ToString();
		GameObject.Find("Text - Historical Points").GetComponent<Text>().text = calculateSkill(historical, well_versed).ToString();
		GameObject.Find("Text - Political Points").GetComponent<Text>().text = calculateSkill(political, well_versed).ToString();
		GameObject.Find("Text - Total Skill Points").GetComponent<Text>().text = totalSkillPoints.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	int calculateSkill(int skill, int abilityScore)
	{
		return skill + (int) Mathf.Floor(abilityScore/2);
	}

	public void addPoint(string score)
	{
		if(totalPoints > 0)
		{
			switch(score)
			{
			case "sturdy":
				sturdy++;
				GameObject.Find("Text - Sturdy Points").GetComponent<Text>().text = sturdy.ToString();
				break;
			case "perception":
				perception++;
				GameObject.Find("Text - Perception Points").GetComponent<Text>().text = perception.ToString();
				break;
			case "technique":
				technique++;
				GameObject.Find("Text - Technique Points").GetComponent<Text>().text = technique.ToString();
				break;
			case "well-versed":
				well_versed++;
				GameObject.Find("Text - Well-Versed Points").GetComponent<Text>().text = well_versed.ToString();
				break;
			default:
				break;
			}
			totalPoints = adjustTotalPoints(totalPoints, false);
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
				GameObject.Find("Text - Athletics Points").GetComponent<Text>().text = calculateSkill(athletics, sturdy).ToString();
				break;
			case "melee":
				melee++;
				GameObject.Find("Text - Melee Points").GetComponent<Text>().text = calculateSkill(melee, sturdy).ToString();
				break;
			case "ranged":
				ranged++;
				GameObject.Find("Text - Ranged Points").GetComponent<Text>().text = calculateSkill(ranged, perception).ToString();
				break;
			case "stealth":
				stealth++;
				GameObject.Find("Text - Stealth Points").GetComponent<Text>().text = calculateSkill(stealth, perception).ToString();
				break;
			case "mechanical":
				mechanical++;
				GameObject.Find("Text - Mechanical Points").GetComponent<Text>().text = calculateSkill(mechanical, technique).ToString();
				break;
			case "medicinal":
				medicinal++;
				GameObject.Find("Text - Medicinal Points").GetComponent<Text>().text = calculateSkill(medicinal, technique).ToString();
				break;
			case "historical":
				historical++;
				GameObject.Find("Text - Historical Points").GetComponent<Text>().text = calculateSkill(historical, well_versed).ToString();
				break;
			case "political":
				political++;
				GameObject.Find("Text - Political Points").GetComponent<Text>().text = calculateSkill(political, well_versed).ToString();
				break;
			default:
				break;
			}
			totalSkillPoints = adjustTotalPoints(totalSkillPoints, false);
		}
	}

	int adjustTotalPoints(int pointType, bool positive)
	{
		if(positive)
		{
			pointType++;
		}
		else
		{
			pointType--;
		}
		GameObject.Find("Text - Total Points").GetComponent<Text>().text = pointType.ToString();

		return pointType;
	}

	public void subtractPoint(string score)
	{
		switch(score)
		{
		case "sturdy":
			if(sturdy > 1)
			{
				sturdy--;
				GameObject.Find("Text - Sturdy Points").GetComponent<Text>().text = sturdy.ToString();
				totalPoints = adjustTotalPoints(totalPoints, true);
			}
			break;
		case "perception":
			if(perception > 1)
			{
				perception--;
				GameObject.Find("Text - Perception Points").GetComponent<Text>().text = perception.ToString();
				totalPoints = adjustTotalPoints(totalPoints, true);
			}
			break;
		case "technique":
			if(technique > 1)
			{
				technique--;
				GameObject.Find("Text - Technique Points").GetComponent<Text>().text = technique.ToString();
				totalPoints = adjustTotalPoints(totalPoints, true);
			}
			break;
		case "well-versed":
			if(well_versed > 1)
			{
				well_versed--;
				GameObject.Find("Text - Well-Versed Points").GetComponent<Text>().text = well_versed.ToString();
				totalPoints = adjustTotalPoints(totalPoints, true);
			}
			break;
		default:
			break;
		}
	}
}
