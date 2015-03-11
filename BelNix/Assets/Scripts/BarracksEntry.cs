using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BarracksEntry : MonoBehaviour  {
    private struct AtAGlance {
        public GameObject panel;
        public Text description, status;
    }
    private struct Options {
        public GameObject panel;
        public Button stats, classFeatures, inventory, levelUp;
    }
    private struct Physique {
        public GameObject panel;
        public Text sturdyAndMod, athleticsAndMelee;
    }
    private struct Prowess {
        public GameObject panel;
        public Text perceptionAndMod, rangedAndStealth;
    }
    private struct Mastery {
        public GameObject panel;
        public Text techniqueAndMod, mechanicalAndMedicinal;
    }
    private struct Knowledge {
        public GameObject panel;
        public Text wellVersedAndMod, historicalAndPolitical;
    }
    
    public Character character;

    private AtAGlance atAGlance;
    private Options options;
    private GameObject statsPanel;
    private GameObject featuresPanel;
    private GameObject pointAllocationPanel;

    private Physique physique;
    private Prowess prowess;
    private Mastery mastery;
    private Knowledge knowledge;

    [SerializeField] private Button confirmLevelUpButton;

	// Use this for initialization
	void Awake ()  { 
        storeChildren();

        hidePanel(statsPanel);
        hidePanel(featuresPanel);
        hidePanel(options.panel);
     //   hidePanel(pointAllocationPanel);
	}
	
	// Update is called once per frame
	void Update ()  {
	
	}

    public void assignCharacter(Character character) {
        this.character = character;
        assignAtAGlance();
        assignStats();
        assignClassFeatures();
        assignInventory();
        setLevelUp();
    }

    private void assignAtAGlance() {
        //Debug.Log(atAGlance.panel.name);
        CharacterSheet characterSheet = character.characterSheet;
        atAGlance.description.text = characterSheet.personalInformation.getCharacterName().fullName() + "\n" + characterSheet.characterProgress.getCharacterClass().getClassName().ToString() + "\n" + characterSheet.personalInformation.getCharacterRace().getRaceString() + " " + characterSheet.personalInformation.getCharacterBackgroundString();
		int level = characterSheet.characterProgress.getCharacterLevel();
		int exp = characterSheet.characterProgress.getCharacterExperience();
		int expToLevel = level * 100;
		atAGlance.status.text = (exp >= expToLevel ? "<size=16><color=green>(Level Up!)</color></size> " : "") + "Level: " + level + "\nExperience: " + exp + " / " + expToLevel + "\nHealth: " + characterSheet.combatScores.getCurrentHealth() + " / " +characterSheet.combatScores.getMaxHealth() + "\nComposure: " + characterSheet.combatScores.getCurrentComposure() + " / " +characterSheet.combatScores.getMaxComposure();
    }

    private void assignStats() {
        AbilityScores characterStats       = character.characterSheet.abilityScores;
        CombatScores statModifiers        = character.characterSheet.combatScores;
        SkillScores characterSkillScores = character.characterSheet.skillScores;

		physique.sturdyAndMod.text = "Sturdy: " + characterStats.getSturdy() + "\nMod: " + statModifiers.getSturdyMod();
//            characterStats.getSturdy(),
  //          statModifiers.getSturdyMod());
        physique.athleticsAndMelee.text = "Athletics: " + characterSkillScores.getScore(Skill.Athletics) + "\nMelee: " + characterSkillScores.getScore(Skill.Melee);

        prowess.perceptionAndMod.text = "Perception: " + characterStats.getPerception() + "\nMod: " + statModifiers.getPerceptionMod();
        prowess.rangedAndStealth.text = "Ranged: " + characterSkillScores.getScore(Skill.Ranged) + "\nStealth: " + characterSkillScores.getScore(Skill.Stealth);

        mastery.techniqueAndMod.text = "Technique: " + characterStats.getTechnique() + "\nMod: " + statModifiers.getTechniqueMod();
        mastery.mechanicalAndMedicinal.text = "Mechanical: " + characterSkillScores.getScore(Skill.Mechanical) + "\nMedicinal: " + characterSkillScores.getScore(Skill.Medicinal);

        knowledge.wellVersedAndMod.text = "Well-Versed: " + characterStats.getWellVersed() + "\nMod: " + statModifiers.getWellVersedMod();
        knowledge.historicalAndPolitical.text = "Historical: " + characterSkillScores.getScore(Skill.Historical) + "\nPolitical: " + characterSkillScores.getScore(Skill.Political);
    }
    private void assignClassFeatures() {
        CharacterProgress characterProgress = character.characterSheet.characterProgress;
        int characterLevel = characterProgress.getCharacterLevel();
   //     ClassFeature[] features = characterProgress.getCharacterClass().getPossibleFeatures(characterLevel);
		ClassFeature[] features = characterProgress.getClassFeatures();
        GameObject featureList = featuresPanel.transform.FindChild("Panel - Feature List").gameObject;
        GameObject exampleText = featureList.transform.GetChild(0).gameObject;
		for (int n=featureList.transform.childCount-1;n>0;n--) GameObject.Destroy(featureList.transform.GetChild(n).gameObject);
        foreach (ClassFeature feature in features) {
            GameObject newText = (GameObject)Instantiate(exampleText);
            newText.transform.SetParent(featureList.transform, false);
            newText.GetComponent<Text>().text = ClassFeatures.getName(feature);
        }
//		exampleText.SetActive(false);
        Destroy(exampleText);
    }
    private void assignInventory() {

    }
    private void storeChildren() {
        Transform atAGlancePanel = this.gameObject.transform.FindChild("Panel - Character At a Glance");
        atAGlance = new AtAGlance() {
            panel = atAGlancePanel.gameObject,
            description = atAGlancePanel.FindChild("Text - Description").gameObject.GetComponent<Text>(),
            status = atAGlancePanel.FindChild("Text - Status").gameObject.GetComponent<Text>()
        };
        Transform optionsPanel = this.gameObject.transform.FindChild("Panel - Options");
        options = new Options() {
            panel = optionsPanel.gameObject,
            stats = optionsPanel.FindChild("Button - Stats").GetComponent<Button>(),
            classFeatures = optionsPanel.FindChild("Button - Class Features").gameObject.GetComponent<Button>(),
            inventory = optionsPanel.FindChild("Button - Inventory").gameObject.GetComponent<Button>(),
            levelUp = optionsPanel.FindChild("Button - Level Up").gameObject.GetComponent<Button>()
        };
        featuresPanel = this.gameObject.transform.FindChild("Panel - Class Features").gameObject;
        statsPanel = this.gameObject.transform.FindChild("Panel - Character Stats").gameObject;
//        pointAllocationPanel = this.gameObject.transform.FindChild("Panel - Ability Scores").gameObject;
        Transform physiquePanel = statsPanel.transform.FindChild("Panel - Physique Stats");
        physique = new Physique() {
            panel = physiquePanel.gameObject,
            sturdyAndMod = physiquePanel.FindChild("Text - Sturdy & Mod").gameObject.GetComponent<Text>(),
            athleticsAndMelee = physiquePanel.FindChild("Text - Athletics & Melee").gameObject.GetComponent<Text>()
        };
        Transform prowessPanel = statsPanel.transform.FindChild("Panel - Prowess Stats");
        prowess = new Prowess() {
            panel = prowessPanel.gameObject,
            perceptionAndMod = prowessPanel.FindChild("Text - Perception & Mod").gameObject.GetComponent<Text>(),
            rangedAndStealth = prowessPanel.FindChild("Text - Ranged & Stealth").gameObject.GetComponent<Text>()
        };
        Transform masteryPanel = statsPanel.transform.FindChild("Panel - Mastery Stats");
        mastery = new Mastery() {
            panel = masteryPanel.gameObject,
            techniqueAndMod = masteryPanel.FindChild("Text - Technique & Mod").gameObject.GetComponent<Text>(),
            mechanicalAndMedicinal = masteryPanel.FindChild("Text - Mechanical & Medicinal").gameObject.GetComponent<Text>()
        };
        Transform knowledgePanel = statsPanel.transform.FindChild("Panel - Knowledge Stats");
        knowledge = new Knowledge() {
            panel = knowledgePanel.gameObject,
            wellVersedAndMod = knowledgePanel.FindChild("Text - Well-Versed & Mod").gameObject.GetComponent<Text>(),
            historicalAndPolitical = knowledgePanel.FindChild("Text - Historical & Political").gameObject.GetComponent<Text>()
        };
    }


	private void setLevelUp() {
		if (character.characterSheet.characterProgress.canLevelUp())
			options.levelUp.interactable = true;
	}
	public void showLevelUp() {
		GameObject basee = (GameObject)GameObject.Find("Base");
		if (basee != null) {
			BaseManager bm = basee.GetComponent<BaseManager>();
//			bm.setInventory(character);
			bm.beginLevelUp(character);
		}
	}

	public void toggleInventory() {
		if (InventoryGUI.isShown)
            InventoryGUI.setInventoryShown(false);
        else {
			GameObject basee = (GameObject)GameObject.Find("Base");
			if (basee != null) {
				BaseManager bm = basee.GetComponent<BaseManager>();
				bm.setInventory(character);
			}
			InventoryGUI.setInventoryShown(true);
		}
    }


    public void hidePanel(GameObject panel) {
        panel.SetActive(false);
    }
    public void showPanel(GameObject panel) {
        if (panel == statsPanel)
        {
            hidePanel(featuresPanel);
         //   hidePanel(pointAllocationPanel);
        }
        if (panel == featuresPanel)
        {
            hidePanel(statsPanel);
       //     hidePanel(pointAllocationPanel);
        }
       if (panel == pointAllocationPanel)
        {
			return;
            hidePanel(featuresPanel);
            hidePanel(statsPanel);
        }
        panel.SetActive(true);
    }
    public void togglePanel(GameObject panel) {
        if (panel.activeSelf)
            hidePanel(panel);
        else
            showPanel(panel);
//        panel.SetActive(!panel.activeSelf);
    }
    public void onStopHovering()
    {
        if (statsPanel.activeSelf || featuresPanel.activeSelf)// || pointAllocationPanel.activeSelf)
            return;
        else
        {
            hidePanel(options.panel);
        }
    }

    private LevelUpContainer levelUp;
    public void submitLevelUp()
    {
		return;
        int[] abilityScores = pointAllocationPanel.GetComponent<BasePointAllocation>().getScores();
        int[] skillScores = pointAllocationPanel.GetComponent<BasePointAllocation>().getSkills();
        levelUp = new LevelUpContainer(){newAbilityScores=abilityScores, newSkillScores=skillScores};
        ClassFeature[] potentialFeatures = character.characterSheet.characterProgress.getCharacterClass().getPossibleFeatures(character.characterSheet.characterProgress.getCharacterLevel() + 1);
        //Debug.Log(potentialFeatures);
        if (potentialFeatures.Length > 0)
            GameObject.Find("Base").GetComponent<BaseManager>().enableNewClassFeaturePrompt(potentialFeatures, this);
        //saveCharacter(levelUp);
        //Debug.Log(ClassFeatures.getName(potentialFeatures[0]) + ((potentialFeatures[1] == null) ? "" : ClassFeatures.getName(potentialFeatures[1])));
    }
    public void receiveClassFeatureChoice(ClassFeature chosenFeature, int selectedOption)
    {
        levelUp.newClassFeature = chosenFeature;
        levelUp.selectedFeature = selectedOption;
        saveCharacter(levelUp);
        // Save character
    }
    private void saveCharacter(LevelUpContainer newStuff)
    {
        character.characterSheet.abilityScores.setScores(levelUp.newAbilityScores[0], levelUp.newAbilityScores[1], levelUp.newAbilityScores[2], levelUp.newAbilityScores[3]);
        character.characterSheet.skillScores.scores = levelUp.newSkillScores;
        
        // selectedFeature is the option (the first or second button) the user clicked while choosing between skills.

        int[] oldFeatures = character.characterSheet.characterProgress.getCharacterClass().chosenFeatures;
        int[] newFeatures = new int[oldFeatures.Length + 1];
        for (int n = 0; n < oldFeatures.Length; n++) newFeatures[n] = oldFeatures[n];
        newFeatures[newFeatures.Length - 1] = levelUp.selectedFeature;
        character.characterSheet.characterProgress.getCharacterClass().chosenFeatures = newFeatures;

        character.characterSheet.characterProgress.incrementLevel();
        character.saveCharacter();
        assignCharacter(character);
            //Debug.Log("Saving Character");
    }


    public void tryEnablingClassFeatureButton(BasePointAllocation pointAllocator)
    {
		return;
        if (pointAllocator.finishedAssigningPoints())
            confirmLevelUpButton.interactable = true;
        else
            confirmLevelUpButton.interactable = false;
    }

    private struct LevelUpContainer
    {
        public int[] newAbilityScores;
        public int[] newSkillScores;
        public ClassFeature newClassFeature;
        public int selectedFeature;
    }
}