using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ActionMarker : MonoBehaviour  {
    [SerializeField] private GameObject sparkObject;

    private GameObject sparks;
	// Use this for initialization
	void Start ()  {
	
	}
	
	// Update is called once per frame
	void Update ()  {
	
	}

    public void spark() {
        //GetComponent<Animator>().SetBool("Explode", true);
        sparks = (GameObject) Instantiate(sparkObject);
        sparks.transform.SetParent(gameObject.transform, false);
        if (GameObject.Find("MapGenerator").GetComponent<MapGenerator>().selectedUnit.team == 0)
            GameObject.Find("AudioManager").GetComponent<AudioManager>().playAudioClip("zap", 0.025f);
        //sparks.transform.localPosition = Vector3.zero;
        //sparks.transform.localEulerAngles = Vector3.zero;
        //sparks.AddComponent<SetRenderQueue>();
        //sparks.GetComponent<SpriteRenderer>().sortingOrder = MapGenerator.uiSpark;
        //sparks.GetComponent<Animator>().Play("Action_Marker_On");
        //Invoke("hideMarker", 0.25f);
        //Invoke("stopExploding", 0.51f);
    }

    private void hideMarker() {
        GetComponent<Image>().enabled = false;   
    }
    public void stopExploding(GameObject explosion) {
        //GetComponent<Animator>().SetBool("Explode", false);
        Destroy(explosion);
    }
}
