using UnityEngine;
using System.Collections;

public class BasePointAllocation : AbstractPointAllocation {

    [SerializeField] private BarracksEntry barracksEntry;
    private Character character;

    protected override void subClassStart()
    {
        character = barracksEntry.character;
        var abilityScores = character.characterSheet.abilityScores;
        sturdy = abilityScores.getSturdy();
        perception = abilityScores.getPerception(); ;
        technique = abilityScores.getTechnique();
        well_versed = abilityScores.getWellVersed();

        minSturdy       = sturdy;
        minPerception   = perception;
        minTechnique    = technique;
        minWellVersed   = well_versed;

        var skillScores = character.characterSheet.skillScores;
        athletics   = skillScores.getScore(Skill.Athletics);
        melee       = skillScores.getScore(Skill.Melee);
        ranged      = skillScores.getScore(Skill.Ranged);
        stealth     = skillScores.getScore(Skill.Stealth);
        mechanical  = skillScores.getScore(Skill.Mechanical);
        medicinal   = skillScores.getScore(Skill.Medicinal);
        historical  = skillScores.getScore(Skill.Historical);
        political   = skillScores.getScore(Skill.Political);

        minAthletics    = athletics;
        minMelee        = melee;
        minRanged       = ranged;
        minStealth      = stealth;
        minMechanical   = mechanical;
        minMedicinal    = medicinal;
        minHistorical   = historical;
        minPolitical    = political;
    }

    protected override int calculateSkill(int skill, int abilityScore, int skillNumber)
    {
        int skillTotal = 0;
        skillTotal += character.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getSkillModifiers()[skillNumber];
        skillTotal += skill;
        skillTotal += calculateMod(abilityScore);
        return skillTotal;
    }
    public override int calculateHealth()
    {
        var characterSheet = character.characterSheet;
        int health = 0;
        health += characterSheet.characterProgress.getCharacterClass().getClassModifiers().getHealthModifier();
        health += characterSheet.personalInformation.getCharacterRace().getHealthModifier();
        health += sturdy;
        health += perception;
        return health;
    }
    public override int calculateComposure()
    {
        int composure = 0;

        composure += character.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getComposureModifier();
        composure += character.characterSheet.personalInformation.getCharacterRace().getComposureModifier();
        composure += technique;
        composure += well_versed;
        return composure;
    }
}
