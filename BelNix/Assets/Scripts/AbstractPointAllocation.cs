﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class AbstractPointAllocation : MonoBehaviour {
    [SerializeField] protected GameObject[] abilityScorePointList;
    [SerializeField] protected GameObject[] modPointList;
    [SerializeField] protected GameObject[] defensePointList;
    [SerializeField] protected GameObject[] skillScorePointList;


    protected int sturdy = 1;
    protected int perception = 1;
    protected int technique = 1;
    protected int well_versed = 1;
    protected int totalPoints = 8;

    protected int minSturdy = 1;
    protected int minPerception = 1;
    protected int minTechnique = 1;
    protected int minWellVersed = 1;

    public int[] getScores() { return new int[4] { sturdy, perception, technique, well_versed }; }

    protected int athletics = 0;
    protected int melee = 0;
    protected int ranged = 0;
    protected int stealth = 0;
    protected int mechanical = 0;
    protected int medicinal = 0;
    protected int historical = 0;
    protected int political = 0;
    protected int totalSkillPoints = 2;

    protected int minAthletics = 0;
    protected int minMelee = 0;
    protected int minRanged = 0;
    protected int minStealth = 0;
    protected int minMechanical = 0;
    protected int minMedicinal = 0;
    protected int minHistorical = 0;
    protected int minPolitical = 0;
    

    public int[] getSkills() { return new int[8] { athletics, melee, ranged, stealth, mechanical, medicinal, historical, political }; }

	// Use this for initialization
	void Start () {
        
        subClassStart();

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
		setPointButtons();
	}

    protected abstract void subClassStart();
    public virtual void extraOnClickLogic() {
		setPointButtons();
	}
    
	
	// Update is called once per frame
	void Update () {
	
	}


    public void updateScores() {
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

    abstract protected int calculateSkill(int skill, int abilityScore, int skillNumber);
    abstract public int calculateHealth();
    abstract public int calculateComposure();

    protected int calculateMod(int abilityScore) {
        return (int)Mathf.Floor(abilityScore / 2);
    }

    public void addPoint(string score) {
        if (totalPoints > 0) {
            switch (score) {
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
			extraOnClickLogic();
			setPointButtons();
        }
    }

    public void addSkillPoint(string skill) {
        if (totalSkillPoints > 0) {
            switch (skill) {
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
			extraOnClickLogic();
			setPointButtons();
        }
    }

    protected void adjustTotalPoints(bool positive)
    {
        if (positive) {
            totalPoints++;
        }
        else {
            totalPoints--;
        }
        abilityScorePointList[0].GetComponent<Text>().text = totalPoints.ToString();
    }

    protected void adjustTotalSkillPoints(bool positive)
    {
        if (positive){
            totalSkillPoints++;
        }
        else{
            totalSkillPoints--;
        }
        skillScorePointList[0].GetComponent<Text>().text = totalSkillPoints.ToString();
    }
	public Button[] minusButtons;
	public Button[] plusButtons;
	public void setPointButtons() {
		int[] points = new int[] {sturdy, perception, technique, well_versed, athletics, melee, ranged, stealth, mechanical, medicinal, historical, political};
		int[] mins = new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0};
		for (int n=0;n<points.Length;n++) {
			minusButtons[n].interactable = points[n] > mins[n];
			plusButtons[n].interactable = n < 4 ? totalPoints > 0 : totalSkillPoints > 0;
		}
	}
    public void subtractPoint(string score)
    {
        switch (score)
        {
            case "sturdy":
                if (sturdy > 1 && sturdy > minSturdy)
                {
                    sturdy--;
                    abilityScorePointList[1].GetComponent<Text>().text = sturdy.ToString();
                    modPointList[0].GetComponent<Text>().text = calculateMod(sturdy).ToString();
                    defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
                    adjustTotalPoints(true);
                }
                break;
            case "perception":
                if (perception > 1 && perception > minPerception)
                {
                    perception--;
                    abilityScorePointList[2].GetComponent<Text>().text = perception.ToString();
                    modPointList[1].GetComponent<Text>().text = calculateMod(perception).ToString();
                    defensePointList[0].GetComponent<Text>().text = calculateHealth().ToString();
                    adjustTotalPoints(true);
                }
                break;
            case "technique":
                if (technique > 1 && technique > minTechnique)
                {
                    technique--;
                    abilityScorePointList[3].GetComponent<Text>().text = technique.ToString();
                    modPointList[2].GetComponent<Text>().text = calculateMod(technique).ToString();
                    defensePointList[1].GetComponent<Text>().text = calculateComposure().ToString();
                    adjustTotalPoints(true);
                }
                break;
            case "well-versed":
                if (well_versed > 1 && well_versed > minWellVersed)
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
		extraOnClickLogic();
		setPointButtons();
    }

    public void subtractSkillPoint(string skill)
    {
        switch (skill)
        {
            case "athletics":
                if (athletics > 0 && athletics > minAthletics)
                {
                    athletics--;
                    skillScorePointList[1].GetComponent<Text>().text = calculateSkill(athletics, sturdy, 0).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "melee":
                if (melee > 0 && melee > minMelee)
                {
                    melee--;
                    skillScorePointList[2].GetComponent<Text>().text = calculateSkill(melee, sturdy, 1).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "ranged":
                if (ranged > 0 && ranged > minRanged)
                {
                    ranged--;
                    skillScorePointList[3].GetComponent<Text>().text = calculateSkill(ranged, perception, 2).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "stealth":
                if (stealth > 0 && stealth > minStealth)
                {
                    stealth--;
                    skillScorePointList[4].GetComponent<Text>().text = calculateSkill(stealth, perception, 3).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "mechanical":
                if (mechanical > 0 && mechanical > minMechanical)
                {
                    mechanical--;
                    skillScorePointList[5].GetComponent<Text>().text = calculateSkill(mechanical, technique, 4).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "medicinal":
                if (medicinal > 0 && medicinal > minMedicinal)
                {
                    medicinal--;
                    skillScorePointList[6].GetComponent<Text>().text = calculateSkill(medicinal, technique, 5).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "historical":
                if (historical > 0 && historical > minHistorical)
                {
                    historical--;
                    skillScorePointList[7].GetComponent<Text>().text = calculateSkill(historical, well_versed, 6).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            case "political":
                if (political > 0 && political > minPolitical)
                {
                    political--;
                    skillScorePointList[8].GetComponent<Text>().text = calculateSkill(political, well_versed, 7).ToString();
                    adjustTotalSkillPoints(true);
                }
                break;
            default:
                break;
        }
		extraOnClickLogic();
		setPointButtons();
    }
}
