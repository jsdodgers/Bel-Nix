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

        var skillScores = character.characterSheet.skillScores;
        athletics   = skillScores.getScore(Skill.Athletics);
        melee       = skillScores.getScore(Skill.Melee);
        ranged      = skillScores.getScore(Skill.Ranged);
        stealth     = skillScores.getScore(Skill.Stealth);
        mechanical  = skillScores.getScore(Skill.Mechanical);
        medicinal   = skillScores.getScore(Skill.Medicinal);
        historical  = skillScores.getScore(Skill.Historical);
        political   = skillScores.getScore(Skill.Political);
    }

    protected override int calculateSkill(int skill, int abilityScore, int skillNumber)
    {

        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
}
