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

	// Use this for initialization
	void Awake ()  { 
        storeChildren();

        hidePanel(statsPanel);
        hidePanel(featuresPanel);
        hidePanel(options.panel);
        hidePanel(pointAllocationPanel);
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
        var characterSheet = character.characterSheet;
        atAGlance.description.text = string.Format(atAGlance.description.text,
            characterSheet.personalInformation.getCharacterName().fullName(), 
            characterSheet.characterProgress.getCharacterClass().getClassName().ToString(),
            characterSheet.personalInformation.getCharacterRace().getRaceString(), 
            characterSheet.personalInformation.getCharacterBackgroundString());
        atAGlance.status.text = string.Format(atAGlance.status.text,
            characterSheet.characterProgress.getCharacterLevel().ToString(),
            characterSheet.characterProgress.getCharacterExperience(),
            characterSheet.characterProgress.getCharacterLevel() * 100,
            characterSheet.combatScores.getCurrentHealth(),
            characterSheet.combatScores.getMaxHealth(),
            characterSheet.combatScores.getCurrentComposure(),
            characterSheet.combatScores.getMaxComposure());
    }

    private void assignStats() {
        var characterStats       = character.characterSheet.abilityScores;
        var statModifiers        = character.characterSheet.combatScores;
        var characterSkillScores = character.characterSheet.skillScores;

        physique.sturdyAndMod.text = string.Format(physique.sturdyAndMod.text,
            characterStats.getSturdy(),
            statModifiers.getSturdyMod());
        physique.athleticsAndMelee.text = string.Format(physique.athleticsAndMelee.text,
            characterSkillScores.getScore(Skill.Athletics),
            characterSkillScores.getScore(Skill.Melee));

        prowess.perceptionAndMod.text = string.Format(prowess.perceptionAndMod.text,
            characterStats.getPerception(),
            statModifiers.getPerceptionMod());
        prowess.rangedAndStealth.text = string.Format(prowess.rangedAndStealth.text,
            characterSkillScores.getScore(Skill.Ranged),
            characterSkillScores.getScore(Skill.Stealth));

        mastery.techniqueAndMod.text = string.Format(mastery.techniqueAndMod.text,
            characterStats.getTechnique(),
            statModifiers.getTechniqueMod());
        mastery.mechanicalAndMedicinal.text = string.Format(mastery.mechanicalAndMedicinal.text,
            characterSkillScores.getScore(Skill.Mechanical),
            characterSkillScores.getScore(Skill.Medicinal));

        knowledge.wellVersedAndMod.text = string.Format(knowledge.wellVersedAndMod.text,
            characterStats.getWellVersed(),
            statModifiers.getWellVersedMod());
        knowledge.historicalAndPolitical.text = string.Format(knowledge.historicalAndPolitical.text,
            characterSkillScores.getScore(Skill.Historical),
            characterSkillScores.getScore(Skill.Political));
    }
    private void assignClassFeatures() {
        var characterProgress = character.characterSheet.characterProgress;
        int characterLevel = characterProgress.getCharacterLevel();
        ClassFeature[] features = characterProgress.getCharacterClass().getPossibleFeatures(characterLevel);
        GameObject featureList = featuresPanel.transform.FindChild("Panel - Feature List").gameObject;
        GameObject exampleText = featureList.transform.GetChild(0).gameObject;
        foreach (var feature in features) {
            GameObject newText = (GameObject)Instantiate(exampleText);
            newText.transform.SetParent(featureList.transform);
            newText.GetComponent<Text>().text = ClassFeatures.getName(feature);
        }
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
        pointAllocationPanel = this.gameObject.transform.FindChild("Panel - Ability Scores").gameObject;
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

    private void setLevelUp()
    {
        if (character.characterSheet.characterProgress.canLevelUp())
            options.levelUp.interactable = true;
    }

    public void toggleInventory()
    {
        //Debug.Log("Waiting for Justin and a new version of InventoryGUI.setUpInvent that can take a Character instead of a Unit.");
        
        if (InventoryGUI.isShown)
            InventoryGUI.setInventoryShown(false);
        else
            InventoryGUI.setupInvent(character);
    }


    public void hidePanel(GameObject panel) {
        panel.SetActive(false);
    }
    public void showPanel(GameObject panel) {
        if (panel == statsPanel)
        {
            hidePanel(featuresPanel);
            hidePanel(pointAllocationPanel);
        }
        if (panel == featuresPanel)
        {
            hidePanel(statsPanel);
            hidePanel(pointAllocationPanel);
        }
        if (panel == pointAllocationPanel)
        {
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
        if (statsPanel.activeSelf || featuresPanel.activeSelf || pointAllocationPanel.activeSelf)
            return;
        else
        {
            hidePanel(options.panel);
        }
    }
}