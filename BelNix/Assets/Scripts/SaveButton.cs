using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour {

	public BaseManager baseManager = null;
	// Use this for initialization
	void Start () {
	
	}
	public void setSaveString(Text t) {
		baseManager.setSaveText(t);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
