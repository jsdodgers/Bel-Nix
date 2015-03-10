using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackMarketTabButton : MonoBehaviour {


	public BlackMarketSection blackMarketSection;
	public Text tabText;
	BaseManager baseManager;
	public bool sell;
	/*
	public void setupTab(BlackMarketSection section, string tabName, BaseManager bm) {
		sell = true;
		blackMarketSection = section;
		tabText.text = tabName;
		baseManager = bm;
	}*/

	public void setupTab(BlackMarketSection section, BaseManager bm) {//, bool sell) {
		this.sell = section.sectionName == "Sell";
		blackMarketSection = section;
		baseManager = bm;
		tabText.text = section.sectionName;
	}



	public void clickTab() {
		if (baseManager.currentSection != blackMarketSection)
			baseManager.openSection(blackMarketSection, sell);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
