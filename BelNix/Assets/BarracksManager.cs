using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarracksManager : MonoBehaviour {

    List<Character> characters;
    GameObject barracksEntryTemplate;
    [SerializeField] private GameObject barracksRoster;

	// Use this for initialization
	void Start () {
        
	}

    public void fillBarracks(GameObject barracksEntryTemplate, List<Character> characterList)
    {
        characters = characterList;
        foreach(var character in characters)
        {
            var abilityScores = character.characterSheet.abilityScores;
            var newBarracksEntryPanel = (GameObject) Instantiate(barracksEntryTemplate);
            var newBarracksEntry = newBarracksEntryPanel.GetComponentInChildren<BarracksEntry>();
            newBarracksEntryPanel.transform.SetParent(barracksRoster.transform, false);
            newBarracksEntry.assignCharacter(character);
        }
        //Destroy(barracksEntryTemplate);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
