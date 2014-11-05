using UnityEngine;
using System.Collections;
using System.IO;

public class GraphicalUserInterface : MonoBehaviour
{
	string characterName = "";
	string characterLastName = "";
	public string[] cCProgression = new string[] {"Personal Information"};
	int cCProgressionSelect = 0;
	//bool abilityScoresHasBeenTriggered, skillsHasBeenTriggered, talentHasBeenTriggered = false;
	bool hasLastName = true;
	public string[] sex = new string[] {"Male", "Female"};
	public string[] race = new string[] {"Berrind", "Ashpian", "Rorrul"};
	public string[] backgroundBerrind = new string[] {"Fallen Noble", "White Gem"};
	public string[] backgroundAshpian = new string[] {"Commoner", "Immigrant"};
	public string[] backgroundRorrul = new string[] {"Sevrant", "Unknown"};
	public string[] characterClass = new string[] {"Ex-Soldier", "Engineer", "Investigator", "Researcher", "Orator"};
	int sexSelect, raceSelect, backgroundSelect, classSelect = 0;
	int age = 25;
	int ageUpperBound = 40;
	int ageLowerBound = 20;
	int height = 0;
	int weight = 0;

	int abilityScorePointsAvailable = 8;
	int sturdyScore = 1;
	int perceptionScore = 1;
	int techniqueScore =1;
	int wellVersedScore = 1;
	int scoreLowerBound = 1;

	int skillPointsAvailable = 2;
	int skillLowerBound = 0;
	int athleticsSkill, meleeSkill, rangedSkill, stealthSkill, mechanicalSkill, medicinalSkill, historicalSkill, politicalSkill = 0;

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	int calculateBoxHeight(int n)
	{
		int height = 0;

		height = 20 * n;

		return height;
	}

	int calculateMod(int abilityScore)
	{
		return abilityScore/2;
	}

	int raceModifications(int cRace, string modName)
	{
		switch(modName)
		{
		case "Health":
			if(cRace == 0)
			{
				return (-1);
			}
			else if(cRace == 2)
			{
				return 1;
			}
			break;
		case "Composure":
			if(cRace == 0)
			{
				return 1;
			}
			else if(cRace == 2)
			{
				return (-1);
			}
			break;
		default:
			break;
		}

		return 0;
	}

	int classModifications(int cClass, string modName)
	{
		switch(modName)
		{
		case "Health":
			if(cClass == 0)
			{
				return 2;
			}
			else if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Composure":
			if(cClass == 3)
			{
				return 2;
			}
			else if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Athletics":
			if(cClass == 0)
			{
				return 1;
			}
			break;
		case "Melee":
			if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Ranged":
			if(cClass == 0)
			{
				return 1;
			}
			break;
		case "Stealth":
			if(cClass == 2)
			{
				return 1;
			}
			break;
		case "Mechanical":
			if(cClass == 1)
			{
				return 2;
			}
			break;
		case "Medicinal":
			if(cClass == 3)
			{
				return 1;
			}
			break;
		case "Historical":
			if(cClass == 3)
			{
				return 1;
			}
			break;
		case "Political":
			if(cClass == 4)
			{
				return 2;
			}
			break;
		default:
			break;
		}

		return 0;
	}

	int setSkillDecreaseButton(int skill, int boxHeight)
	{
		if(skill == skillLowerBound)
		{
			GUI.enabled = false;
			if(GUI.Button(new Rect(260, calculateBoxHeight(boxHeight), 25, 20), "<"))
			{
				skillPointsAvailable++;
				skill = skill - 1;
			}
		}
		else
		{
			if(GUI.Button(new Rect(260, calculateBoxHeight(boxHeight), 25, 20), "<"))
			{
				skillPointsAvailable++;
				skill = skill - 1;
			}
		}
		GUI.enabled = true;

		return skill;
	}

	int setSkillIncreaseButton(int skill, int boxHeight)
	{
		if(skillPointsAvailable == 0)
		{
			GUI.enabled = false;
			if(GUI.Button(new Rect(335, calculateBoxHeight(boxHeight), 25, 20), ">"))
			{
				skillPointsAvailable--;
				skill++;
			}
		}
		else
		{
			if(GUI.Button(new Rect(335, calculateBoxHeight(boxHeight), 25, 20), ">"))
			{
				skillPointsAvailable--;
				skill++;
			}
		}
		GUI.enabled = true;

		return skill;
	}

	void OnGUI()
	{
		if(Application.loadedLevel == 0)
		{
			GUI.Box(new Rect(Screen.width/4, Screen.height/2, 200, 250), "Main Menu");
			if(GUI.Button(new Rect(Screen.width/4 + 20, Screen.height/2 + 20, 160, 20), "New Game"))
			{
				//Load into Character Creation
				Application.LoadLevel(1);
			}
			if(GUI.Button(new Rect(Screen.width/4 + 20, Screen.height/2 + 40, 160, 20), "Options"))
			{
				//Bring up Options UI.  Do NOT load into a new scene.
			}
			if(GUI.Button(new Rect(Screen.width/4 + 20, Screen.height/2 + 60, 160, 20), "Quit"))
			{
				//Quit the Application
				Application.Quit();
			}
		}
		if(Application.loadedLevel == 1)
		{
			cCProgressionSelect = GUI.SelectionGrid(new Rect(225, Screen.height - 100, Screen.width - 450, 100), cCProgressionSelect, cCProgression, 4);
			GUI.Box(new Rect(Screen.width - 510, 10, 500, 50), "Portrait/Looks");
			GUI.Box(new Rect(Screen.width - 510, calculateBoxHeight(3), 500, 400), "");

			if(cCProgressionSelect == 0)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Personal Information");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "First Name:");
				characterName = GUI.TextField(new Rect(260, calculateBoxHeight(3), 250, 20), characterName);
				hasLastName = GUI.Toggle(new Rect(10, calculateBoxHeight(4), 20, 20), hasLastName, "");
				GUI.enabled = hasLastName;
				GUI.Box(new Rect(30, calculateBoxHeight(4), 230, 20), "Last Name:");
				characterLastName = GUI.TextField(new Rect(260, calculateBoxHeight(4), 250, 20), characterLastName);
				GUI.enabled = true;
				GUI.Box(new Rect(10, calculateBoxHeight(5), 250, 20), "Sex:");
				sexSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(5), 250, 20),sexSelect, sex, 2);
				GUI.Box(new Rect(10, calculateBoxHeight(6), 250, 20), "Race:");
				raceSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(6), 250, 20),raceSelect, race, 3);
				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Racial Stats:");
				GUI.Box(new Rect(135, calculateBoxHeight(8), 125, 20), "Primal State:");
				GUI.Box(new Rect(135, calculateBoxHeight(9), 125, 20), "Background:");
				switch(raceSelect)
				{
				case 0:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "-1 Health/ +1 Composure");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Reckless");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundBerrind, 2);
					break;
				case 1:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "No Changes");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Passive");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundAshpian, 2);
					break;
				case 2:
					GUI.Box(new Rect(260, calculateBoxHeight(7), 250, 20), "+1 Health/ -1 Composure");
					GUI.Box(new Rect(260, calculateBoxHeight(8), 250, 20), "Threatened");
					backgroundSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(9), 250, 20),backgroundSelect, backgroundRorrul, 2);
					break;
				default:
					break;
				}

				GUI.Box(new Rect(10, calculateBoxHeight(10), 500, 20), "Physical Features:");
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Age:");
				if(age == ageLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(11), 25, 20), "<") && age > ageLowerBound)
					{
						age--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(11), 25, 20), "<") && age > ageLowerBound)
					{
						age--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(11), 200, 20), age.ToString());
				if(age == ageUpperBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(11), 25, 20), ">") && age < ageUpperBound)
					{
						age++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(11), 25, 20), ">") && age < ageUpperBound)
					{
						age++;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Height:");
				if(GUI.Button(new Rect(260, calculateBoxHeight(12), 25, 20), "<"))
				{
					height--;
				}
				GUI.Box(new Rect(285, calculateBoxHeight(12), 200, 20), height.ToString());
				if(GUI.Button(new Rect(485, calculateBoxHeight(12), 25, 20), ">"))
				{
					height++;
				}
				
				GUI.Box(new Rect(135, calculateBoxHeight(13), 125, 20), "Weight:");
				if(GUI.Button(new Rect(260, calculateBoxHeight(13), 25, 20), "<"))
				{
					weight--;
				}
				GUI.Box(new Rect(285, calculateBoxHeight(13), 200, 20), weight.ToString());
				if(GUI.Button(new Rect(485, calculateBoxHeight(13), 25, 20), ">"))
				{
					weight++;
				}
				
				
				GUI.Box(new Rect(10, calculateBoxHeight(14), 250, 40), "Class:");
				classSelect = GUI.SelectionGrid(new Rect(260, calculateBoxHeight(14), 250, 40),classSelect, characterClass, 3);
				GUI.Box(new Rect(135, calculateBoxHeight(16), 125, 20), "Class Stats:");
				GUI.Box(new Rect(135, calculateBoxHeight(17), 125, 20), "Class Features:");
				switch(classSelect)
				{
				case 0:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Health/+1 Athletics/ +1 Ranged");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Throw");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Decisive Strike");
					break;
				case 1:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Mechanical");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Construction");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Efficient Storage");
					break;
				case 2:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+1 Health/+1 Composure/+1 Melee/ +1 Stealth");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Mark");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Sneak Attack");
					break;
				case 3:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Composure/+1 Medicinal/ +1 Historical");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Stabilize");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Combat Medic");
					break;
				case 4:
					GUI.Box(new Rect(260, calculateBoxHeight(16), 250, 20), "+2 Political");
					GUI.Box(new Rect(260, calculateBoxHeight(17), 250, 20), "Invoke");
					GUI.Box(new Rect(260, calculateBoxHeight(18), 250, 20), "Primal Control");
					break;
				default:
					break;
				}

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Cancel"))
				{
					Application.LoadLevel(0);
				}

				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//abilityScoresHasBeenTriggered = true;
					cCProgressionSelect = 1;
					if(cCProgression.Length < 2)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores"};
					}
				}
			}
			else if(cCProgressionSelect == 1)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Ability Scores");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "Points Available:");
				GUI.Box(new Rect(260, calculateBoxHeight(3), 250, 20), abilityScorePointsAvailable.ToString());

				GUI.Box(new Rect(135, calculateBoxHeight(4), 125, 20), "Sturdy:");
				if(sturdyScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(4), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						sturdyScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(4), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						sturdyScore--;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(285, calculateBoxHeight(4), 200, 20), sturdyScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(4), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						sturdyScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(4), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						sturdyScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(135, calculateBoxHeight(5), 125, 20), "Perception:");
				if(perceptionScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(5), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						perceptionScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(5), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						perceptionScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(5), 200, 20), perceptionScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(5), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						perceptionScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(5), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						perceptionScore++;
					}
				}
				GUI.enabled = true;
				GUI.Box(new Rect(510, calculateBoxHeight(4), 125, 20), "Health:");
				GUI.Box(new Rect(510, calculateBoxHeight(5), 125, 20), (sturdyScore + perceptionScore + classModifications(classSelect, "Health") + raceModifications(raceSelect, "Health")).ToString());

				GUI.Box(new Rect(135, calculateBoxHeight(6), 125, 20), "Technique:");
				if(techniqueScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(6), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						techniqueScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(6), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						techniqueScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(6), 200, 20), techniqueScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(6), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						techniqueScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(6), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						techniqueScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Well-Versed:");
				if(wellVersedScore == scoreLowerBound)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(260, calculateBoxHeight(7), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						wellVersedScore--;
					}
				}
				else
				{
					if(GUI.Button(new Rect(260, calculateBoxHeight(7), 25, 20), "<"))
					{
						abilityScorePointsAvailable++;
						wellVersedScore--;
					}
				}
				GUI.enabled = true;
				
				GUI.Box(new Rect(285, calculateBoxHeight(7), 200, 20), wellVersedScore.ToString());
				if(abilityScorePointsAvailable == 0)
				{
					GUI.enabled = false;
					if(GUI.Button(new Rect(485, calculateBoxHeight(7), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						wellVersedScore++;
					}
				}
				else
				{
					if(GUI.Button(new Rect(485, calculateBoxHeight(7), 25, 20), ">"))
					{
						abilityScorePointsAvailable--;
						wellVersedScore++;
					}
				}
				GUI.enabled = true;

				GUI.Box(new Rect(510, calculateBoxHeight(6), 125, 20), "Composure:");
				GUI.Box(new Rect(510, calculateBoxHeight(7), 125, 20), (techniqueScore + wellVersedScore + classModifications(classSelect, "Composure") + raceModifications(raceSelect, "Composure")).ToString());

				GUI.Box(new Rect(10, calculateBoxHeight(9), 250, 20), "Combat Scores:");
				GUI.Box(new Rect(135, calculateBoxHeight(10), 125, 20), "Initiatve:");
				GUI.Box(new Rect(260, calculateBoxHeight(10), 125, 20), calculateMod(sturdyScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Critical:");
				GUI.Box(new Rect(260, calculateBoxHeight(11), 125, 20), calculateMod(perceptionScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Handling:");
				GUI.Box(new Rect(260, calculateBoxHeight(12), 125, 20), calculateMod(techniqueScore).ToString());
				GUI.Box(new Rect(135, calculateBoxHeight(13), 125, 20), "Dominion:");
				GUI.Box(new Rect(260, calculateBoxHeight(13), 125, 20), calculateMod(wellVersedScore).ToString());

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 0;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//skillsHasBeenTriggered = true;
					cCProgressionSelect = 2;
					if(cCProgression.Length < 3)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores", "Skills"};
					}
				}
			}
			else if(cCProgressionSelect == 2)
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Skills");
				GUI.Box(new Rect(10, calculateBoxHeight(3), 250, 20), "Points Available:");
				GUI.Box(new Rect(260, calculateBoxHeight(3), 250, 20), skillPointsAvailable.ToString());
				GUI.Box(new Rect(10, calculateBoxHeight(4), 125, 20), "Category:");
				GUI.Box(new Rect(135, calculateBoxHeight(4), 125, 20), "Skill:");
				GUI.Box(new Rect(460, calculateBoxHeight(4), 50, 20), "Total:");
				GUI.Box(new Rect(285, calculateBoxHeight(4), 50, 20), "Base:");
				GUI.Box(new Rect(385, calculateBoxHeight(4), 50, 20), "Mod:");
				GUI.Box(new Rect(10, calculateBoxHeight(5), 125, 40), "Physique:");
				GUI.Box(new Rect(135, calculateBoxHeight(5), 125, 20), "Athletics:");
				GUI.Box(new Rect(460, calculateBoxHeight(5), 50, 20), (athleticsSkill + calculateMod(sturdyScore) + classModifications(classSelect, "Athletics")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(5), 50, 20), (athleticsSkill + classModifications(classSelect, "Athletics")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(5), 50, 40), calculateMod(sturdyScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(5), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(5), 25, 40), "=");
				athleticsSkill = setSkillDecreaseButton(athleticsSkill, 5);
				athleticsSkill = setSkillIncreaseButton(athleticsSkill, 5);
				GUI.Box(new Rect(135, calculateBoxHeight(6), 125, 20), "Melee:");
				GUI.Box(new Rect(460, calculateBoxHeight(6), 50, 20), (meleeSkill + calculateMod(sturdyScore) + classModifications(classSelect, "Melee")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(6), 50, 20), (meleeSkill + classModifications(classSelect, "Melee")).ToString());
				meleeSkill = setSkillDecreaseButton(meleeSkill, 6);
				meleeSkill = setSkillIncreaseButton(meleeSkill, 6);
				GUI.Box(new Rect(10, calculateBoxHeight(7), 125, 40), "Prowess:");
				GUI.Box(new Rect(135, calculateBoxHeight(7), 125, 20), "Ranged:");
				GUI.Box(new Rect(460, calculateBoxHeight(7), 50, 20), (rangedSkill + calculateMod(perceptionScore) + classModifications(classSelect, "Ranged")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(7), 50, 20), (rangedSkill + classModifications(classSelect, "Ranged")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(7), 50, 40), calculateMod(perceptionScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(7), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(7), 25, 40), "=");
				rangedSkill = setSkillDecreaseButton(rangedSkill, 7);
				rangedSkill = setSkillIncreaseButton(rangedSkill, 7);
				GUI.Box(new Rect(135, calculateBoxHeight(8), 125, 20), "Stealth:");
				GUI.Box(new Rect(460, calculateBoxHeight(8), 50, 20), (stealthSkill + calculateMod(perceptionScore) + classModifications(classSelect, "Stealth")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(8), 50, 20), (stealthSkill + classModifications(classSelect, "Stealth")).ToString());
				stealthSkill = setSkillDecreaseButton(stealthSkill, 8);
				stealthSkill = setSkillIncreaseButton(stealthSkill, 8);
				GUI.Box(new Rect(10, calculateBoxHeight(9), 125, 40), "Mastery:");
				GUI.Box(new Rect(135, calculateBoxHeight(9), 125, 20), "Mechanical:");
				GUI.Box(new Rect(460, calculateBoxHeight(9), 50, 20), (mechanicalSkill + calculateMod(techniqueScore) + classModifications(classSelect, "Mechanical")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(9), 50, 20), (mechanicalSkill + classModifications(classSelect, "Mechanical")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(9), 50, 40), calculateMod(techniqueScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(9), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(9), 25, 40), "=");
				mechanicalSkill = setSkillDecreaseButton(mechanicalSkill, 9);
				mechanicalSkill = setSkillIncreaseButton(mechanicalSkill, 9);
				GUI.Box(new Rect(135, calculateBoxHeight(10), 125, 20), "Medicinal:");
				GUI.Box(new Rect(460, calculateBoxHeight(10), 50, 20), (medicinalSkill + calculateMod(techniqueScore) + classModifications(classSelect, "Medicinal")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(10), 50, 20), (medicinalSkill + classModifications(classSelect, "Medicinal")).ToString());
				medicinalSkill = setSkillDecreaseButton(medicinalSkill, 10);
				medicinalSkill = setSkillIncreaseButton(medicinalSkill, 10);
				GUI.Box(new Rect(10, calculateBoxHeight(11), 125, 40), "Knowledge:");
				GUI.Box(new Rect(135, calculateBoxHeight(11), 125, 20), "Historical:");
				GUI.Box(new Rect(460, calculateBoxHeight(11), 50, 20), (historicalSkill + calculateMod(wellVersedScore) + classModifications(classSelect, "Historical")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(11), 50, 20), (historicalSkill + classModifications(classSelect, "Historical")).ToString());
				GUI.Box(new Rect(385, calculateBoxHeight(11), 50, 40), calculateMod(wellVersedScore).ToString());
				GUI.Box(new Rect(360, calculateBoxHeight(11), 25, 40), "+");
				GUI.Box(new Rect(435, calculateBoxHeight(11), 25, 40), "=");
				historicalSkill = setSkillDecreaseButton(historicalSkill, 11);
				historicalSkill = setSkillIncreaseButton(historicalSkill, 11);
				GUI.Box(new Rect(135, calculateBoxHeight(12), 125, 20), "Political:");
				GUI.Box(new Rect(460, calculateBoxHeight(12), 50, 20), (politicalSkill + calculateMod(wellVersedScore) + classModifications(classSelect, "Political")).ToString());
				GUI.Box(new Rect(285, calculateBoxHeight(12), 50, 20), (politicalSkill + classModifications(classSelect, "Political")).ToString());
				politicalSkill = setSkillDecreaseButton(politicalSkill, 12);
				politicalSkill = setSkillIncreaseButton(politicalSkill, 12);

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 1;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Next"))
				{
					//talentHasBeenTriggered = true;
					cCProgressionSelect = 3;
					if(cCProgression.Length < 4)
					{
						cCProgression = new string[] {"Personal Information", "Ability Scores", "Skills", "Talent"};
					}
				}
			}
			else
			{
				GUI.Box(new Rect(10, 10, 500, 50), "Character Creation: Talents");

				if(GUI.Button(new Rect(0, Screen.height - 40, 200, 40), "Back"))
				{
					cCProgressionSelect = 2;
				}
				
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Finish"))
				{
					writeCharacter();
				}
			}
		}
	}

	public void writeCharacter()
	{
		string characterStr = "";
		string delimiter = ";";
		//********PERSONAL INFORMATION ********
		//Adding player first name.
		characterStr += characterName + delimiter;
		//If the player has a last name, add it.
		characterStr += hasLastName.ToString() + delimiter;
		if(hasLastName)
		{
			characterStr += characterLastName + delimiter;
		}
		//sexSelect 0 = Male, 1 = Female
		characterStr += sexSelect.ToString() + delimiter;
		//raceSelect 0 = Berrind, 1 = Ashpian, 2 = Rorrul
		characterStr += raceSelect.ToString() + delimiter;
		//backgroundSelect (contextualized by race)
		//For Berrind: 0 = Fallen Noble, 1 = White Gem
		//For Ashpian: 0 = Commoner, 1 = Immigrant
		//For Rorrul: 0 = Servant, 1 = Unknown
		characterStr += backgroundSelect.ToString() + delimiter;
		characterStr += age.ToString() + delimiter;
		characterStr += height.ToString() + delimiter;
		characterStr += weight.ToString() + delimiter;
		//classSelect 0 = Ex-Soldier, 1 = Engineer, 2 = Investigator, 3 = Researcher, 4 = Orator
		characterStr += classSelect.ToString() + delimiter;
		//********Ability Scores********
		characterStr += sturdyScore.ToString() + delimiter;
		characterStr += perceptionScore.ToString() + delimiter;
		characterStr += techniqueScore.ToString() + delimiter;
		characterStr += wellVersedScore.ToString() + delimiter;
		//********Skills********
		characterStr += athleticsSkill.ToString() + delimiter;
		characterStr += meleeSkill.ToString() + delimiter;
		characterStr += rangedSkill.ToString() + delimiter;
		characterStr += stealthSkill.ToString() + delimiter;
		characterStr += mechanicalSkill.ToString() + delimiter;
		characterStr += medicinalSkill.ToString() + delimiter;
		characterStr += historicalSkill.ToString() + delimiter;
		characterStr += politicalSkill.ToString() + delimiter;
		//********Talents********


		int currAdd = 0;
		string fileDirectory = Application.dataPath + "/Saves/";
		string fileName = fileDirectory + characterName + characterLastName + (currAdd>0?"" +currAdd:"") + ".txt";
		while(File.Exists(fileName))
		{
			currAdd++;
			fileName = fileDirectory + characterName + characterLastName + (currAdd>0?"" +currAdd:"") + ".txt";
		}

		StreamWriter sr = File.CreateText(fileName);

		sr.WriteLine(characterStr);
		sr.Close();

		Debug.Log(characterStr);
	}
}
