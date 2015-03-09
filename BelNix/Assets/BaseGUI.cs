using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaseGUI : MonoBehaviour {
    [SerializeField] private GameObject barracks;
    [SerializeField] private GameObject barracksEntryTemplate;
	// Use this for initialization
	void Start () {
        barracks.SetActive(false);
	}

    public void enableBarracks()
    {
        barracks.SetActive(true);
    }
    public void disableBarracks()
    {
        barracks.SetActive(false);
    }
    public void initializeBarracks(List<Character> characterList)
    {
        enableBarracks();
        barracks.GetComponent<BarracksManager>().fillBarracks(barracksEntryTemplate, characterList);
        disableBarracks();
    }
	// Update is called once per frame
	void Update () {
	
	}
}
