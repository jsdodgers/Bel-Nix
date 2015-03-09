using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Spark : MonoBehaviour  {

	// Use this for initialization
	void Start ()  {
        //SetRenderQueue.setRendererQueue(GetComponent<Image>(), new int[]  { 1000 }); 
	}
	
	// Update is called once per frame
	void Update ()  {
	
	}

    public void disableParent() {
        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = false;
        //Invoke("deleteSelf", (7/20)); 
    }

    public void deleteSelf() {
        //Debug.Log("This is supposed to be shown shortly before deleting this object.");
        //Destroy(this);
        gameObject.transform.parent.gameObject.GetComponent<ActionMarker>().stopExploding(gameObject);
    }
}
