using UnityEngine;
using System.Collections;

public class BloodScript : MonoBehaviour {

	public int x = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		while(x <= 60)
		{
			transform.localScale += new Vector3(3.0f*Time.deltaTime, 3.0f*Time.deltaTime, 0.0f);
			transform.Translate(0.0f, 1.0f*Time.deltaTime, -0.001f);
			Debug.Log("Spawn");
			x += 1;
		}
	}
}
