using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarracksManager : MonoBehaviour {

    List<Character> characters;
    GameObject barracksEntryTemplate;

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
            var newBarracksEntry = newBarracksEntryPanel.GetComponent<BarracksEntry>();
            newBarracksEntryPanel.transform.SetParent(gameObject.transform, false);
            newBarracksEntry.assignCharacter(character);
        }
        //Destroy(barracksEntryTemplate);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
