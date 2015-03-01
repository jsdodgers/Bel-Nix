using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionMarker : MonoBehaviour {
    [SerializeField] private GameObject sparkObject;

    private GameObject sparks;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void spark()
    {
        //GetComponent<Animator>().SetBool("Explode", true);
        GameObject sparks = (GameObject) Instantiate(sparkObject);
        sparks.transform.SetParent(this.transform);
        sparks.transform.localPosition = Vector3.zero;
        sparks.transform.localEulerAngles = Vector3.zero;
        sparks.GetComponent<Animator>().Play("Action_Marker_On");
        Invoke("hideMarker", 0.25f);
        Invoke("stopExploding", 0.51f);
    }

    private void hideMarker()
    {
        GetComponent<Image>().enabled = false;   
    }
    private void stopExploding()
    {
        //GetComponent<Animator>().SetBool("Explode", false);
        if (sparks != null)
        {
            Destroy(sparks);
        }
    }
}
