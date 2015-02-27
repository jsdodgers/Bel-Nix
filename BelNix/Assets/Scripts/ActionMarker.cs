using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void spark()
    {
        //GetComponent<Animator>().SetBool("Explode", true);
        GetComponent<Animator>().Play("Action_Marker_On");
        Invoke("hideMarker", 0.25f);
    }

    private void hideMarker()
    {
        GetComponent<Image>().enabled = false;
    }
    private void stopExploding()
    {
        GetComponent<Animator>().SetBool("Explode", false);
    }
}
