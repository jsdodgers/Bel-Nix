using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarracksEntry : MonoBehaviour {
    private struct AtAGlance
    {
        public GameObject panel;
        public Text description;
        public Text status;
    }
    private struct Options
    {
        public GameObject panel;
        public Button stats;
        public Button classFeatures;
        public Button inventory;
        public Button levelUp;
    }
    private struct Physique
    {
        public GameObject panel;
        public Text sturdyAndMod;
        public Text athleticsAndMelee;
    }
    private struct Prowess
    {
        public GameObject panel;
        public Text perceptionAndMod;
        public Text rangedAndStealth;
    }
    private struct Mastery
    {
        public GameObject panel;
        public Text techniqueAndMod;
        public Text mechanicalAndMedicinal;
    }
    private struct Knowledge
    {
        public GameObject panel;
        public Text wellVersedAndMod;
        public Text historicalAndPolitical;
    }
    
    public Character character;
    private AtAGlance atAGlance;
    private Options options;
    private Physique physique;
    private Prowess prowess;
    private Mastery mastery;
    private Knowledge knowledge;

	// Use this for initialization
	void Start () {
        storeChildren();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void assignCharacter(Character character)
    {
        this.character = character;
        assignAtAGlance();
        assignStats();
        assignClassFeatures();
        assignInventory();
    }

    private void assignAtAGlance()
    {
        //Unit u = character;
        string s = character.characterSheet.personalInformation.getCharacterName().fullName();//.characterSheet;
        Debug.Log(s);
        CharacterSheet characterSheet = character.characterSheet;
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
    private void assignStats()
    {
        CharacterSheet characterSheet = character.characterSheet;
        physique.sturdyAndMod.text = string.Format(physique.sturdyAndMod.text,
            characterSheet.abilityScores.getSturdy(),
            characterSheet.combatScores.getSturdyMod());
        physique.athleticsAndMelee.text = string.Format(physique.athleticsAndMelee.text,
            characterSheet.skillScores.getScore(Skill.Athletics),
            characterSheet.skillScores.getScore(Skill.Melee));

        prowess.perceptionAndMod.text = string.Format(prowess.perceptionAndMod.text,
            characterSheet.abilityScores.getPerception(0),
            characterSheet.combatScores.getPerceptionMod(false));
        prowess.rangedAndStealth.text = string.Format(prowess.rangedAndStealth.text,
            characterSheet.skillScores.getScore(Skill.Ranged),
            characterSheet.skillScores.getScore(Skill.Stealth));

        mastery.techniqueAndMod.text = string.Format(mastery.techniqueAndMod.text,
            characterSheet.abilityScores.getTechnique(),
            characterSheet.combatScores.getTechniqueMod());
        mastery.mechanicalAndMedicinal.text = string.Format(mastery.mechanicalAndMedicinal.text,
            characterSheet.skillScores.getScore(Skill.Mechanical),
            characterSheet.skillScores.getScore(Skill.Medicinal));

        knowledge.wellVersedAndMod.text = string.Format(knowledge.wellVersedAndMod.text,
            characterSheet.abilityScores.getWellVersed(),
            characterSheet.combatScores.getWellVersedMod());
        knowledge.historicalAndPolitical.text = string.Format(knowledge.historicalAndPolitical.text,
            characterSheet.skillScores.getScore(Skill.Historical),
            characterSheet.skillScores.getScore(Skill.Political));
    }
    private void assignClassFeatures()
    {

    }
    private void assignInventory()
    {

    }
    private void storeChildren()
    {
        Transform atAGlancePanel = this.gameObject.transform.FindChild("Panel - Character At a Glance");
        atAGlance = new AtAGlance()
        {
            panel = atAGlancePanel.gameObject,
            description = atAGlancePanel.FindChild("Text - Description").gameObject.GetComponent<Text>(),
            status = atAGlancePanel.FindChild("Text - Status").gameObject.GetComponent<Text>()
        };
        Transform optionsPanel = this.gameObject.transform.FindChild("Panel - Options");
        options = new Options()
        {
            panel = optionsPanel.gameObject,
            stats = optionsPanel.FindChild("Button - Stats").GetComponent<Button>(),
            classFeatures = optionsPanel.FindChild("Button - Class Features").gameObject.GetComponent<Button>(),
            inventory = optionsPanel.FindChild("Button - Inventory").gameObject.GetComponent<Button>(),
            levelUp = optionsPanel.FindChild("Button - Level Up").gameObject.GetComponent<Button>()
        };
        Transform physiquePanel = this.gameObject.transform.FindChild("Panel - Character Stats").FindChild("Panel - Physique Stats");
        physique = new Physique()
        {
            panel = physiquePanel.gameObject,
            sturdyAndMod = physiquePanel.FindChild("Text - Sturdy & Mod").gameObject.GetComponent<Text>(),
            athleticsAndMelee = physiquePanel.FindChild("Text - Athletics & Melee").gameObject.GetComponent<Text>()
        };
        Transform prowessPanel = this.gameObject.transform.FindChild("Panel - Character Stats").FindChild("Panel - Prowess Stats");
        prowess = new Prowess()
        {
            panel = prowessPanel.gameObject,
            perceptionAndMod = prowessPanel.FindChild("Text - Perception & Mod").gameObject.GetComponent<Text>(),
            rangedAndStealth = prowessPanel.FindChild("Text - Ranged & Stealth").gameObject.GetComponent<Text>()
        };
        Transform masteryPanel = this.gameObject.transform.FindChild("Panel - Character Stats").FindChild("Panel - Mastery Stats");
        mastery = new Mastery()
        {
            panel = masteryPanel.gameObject,
            techniqueAndMod = masteryPanel.FindChild("Text - Technique & Mod").gameObject.GetComponent<Text>(),
            mechanicalAndMedicinal = masteryPanel.FindChild("Text - Mechanical & Medicinal").gameObject.GetComponent<Text>()
        };
        Transform knowledgePanel = this.gameObject.transform.FindChild("Panel - Character Stats").FindChild("Panel - Knowledge Stats");
        knowledge = new Knowledge()
        {
            panel = knowledgePanel.gameObject,
            wellVersedAndMod = knowledgePanel.FindChild("Text - Well-Versed & Mod").gameObject.GetComponent<Text>(),
            historicalAndPolitical = knowledgePanel.FindChild("Text - Historical & Political").gameObject.GetComponent<Text>()
        };
    }
}
