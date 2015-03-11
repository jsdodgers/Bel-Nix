using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BarracksManager : MonoBehaviour {

    List<Character> characters;
	static Dictionary<Character, BarracksEntry> characterEntries = new Dictionary<Character, BarracksEntry>();
    //GameObject barracksEntryTemplate;
    [SerializeField] private GameObject barracksRoster;
    [SerializeField] private GameObject scrollBar;
	public static bool isShown;

	// Use this for initialization
	void Start () {
        
	}

    public void fillBarracks(GameObject barracksEntryTemplate, List<Character> characterList) {
        characters = characterList;
        foreach(Character character in characters) {
            AbilityScores abilityScores = character.characterSheet.abilityScores;
            GameObject newBarracksEntryPanel = (GameObject) Instantiate(barracksEntryTemplate);
            BarracksEntry newBarracksEntry = newBarracksEntryPanel.GetComponentInChildren<BarracksEntry>();
            newBarracksEntryPanel.transform.SetParent(barracksRoster.transform, false);
            newBarracksEntry.assignCharacter(character);
			characterEntries[character] = newBarracksEntry;
        }
        //Destroy(barracksEntryTemplate);
    }

	public static void updateCharacterEntry(Character character) {
		if (characterEntries.ContainsKey(character)) characterEntries[character].assignCharacter(character);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
