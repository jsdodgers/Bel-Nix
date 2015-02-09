using UnityEngine;
using System.Collections;

public class BloodTest : MonoBehaviour {
	
	public GameObject blood1;
	public GameObject blood2;
	public GameObject blood3;
	public GameObject blood4;
	public GameObject blood5;
	public GameObject blood6;
	public GameObject blood7;
	public GameObject blood8;
	public GameObject blood9;
	public GameObject blood10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Space))
		{
			int bloodNumber = Random.Range(1, 10);
			switch (bloodNumber)
			{
			case 1:
				GameObject.Instantiate(blood1);
				break;
			case 2:
				GameObject.Instantiate(blood2);
				break;
			case 3:
				GameObject.Instantiate(blood3);
				break;
			case 4:
				GameObject.Instantiate(blood4);
				break;
			case 5:
				GameObject.Instantiate(blood5);
				break;
			case 6:
				GameObject.Instantiate(blood6);
				break;
			case 7:
				GameObject.Instantiate(blood7);
				break;
			case 8:
				GameObject.Instantiate(blood8);
				break;
			case 9:
				GameObject.Instantiate(blood9);
				break;
			case 10:
				GameObject.Instantiate(blood10);
				break;
			}
		}
	}
}
