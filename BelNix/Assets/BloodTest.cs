using UnityEngine;
using System.Collections;

public class BloodTest : MonoBehaviour {

	public GameObject blood;
	public GameObject blood1;
	public GameObject blood2;
	public string bloodstring;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject.Instantiate(blood1);
			//transform.localScale.Set(4.0f, 4.0f, 0.0f);

		}
	}
}
