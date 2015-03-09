using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackMarketTabButton : MonoBehaviour {


	public BlackMarketSection blackMarketSection;
	public Text tabText;
	BaseManager baseManager;

	public void setupTab(BlackMarketSection section, BaseManager bm) {
		blackMarketSection = section;
		baseManager = bm;
		tabText.text = section.sectionName;
	}



	public void clickTab() {
		if (baseManager.currentSection != blackMarketSection)
			baseManager.openSection(blackMarketSection);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
