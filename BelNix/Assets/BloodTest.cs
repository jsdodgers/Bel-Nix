using UnityEngine;
using System.Collections;

public class BloodTest : MonoBehaviour  {
	
    public GameObject blood;

	// Use this for initialization
	void Start ()  {
	
	}
	
	// Update is called once per frame
	void Update ()  {
        // Method signature needs an attack direction
	
		if(Input.GetKeyDown(KeyCode.Space))  {
			int bloodNumber = Random.Range(1, 2);
            GameObject bloodSplatter = (GameObject) Instantiate(blood, transform.position, transform.rotation);
            bloodSplatter.transform.SetParent(gameObject.transform); 
            //bloodSplatter.transform.localPosition = Vector3.zero;
            // bloodSplatter.transform.localEulerAngles = Vector3.zero /*+ direction */;
            bloodSplatter.GetComponent<Animator>().SetInteger("BloodOption", bloodNumber);
            //bloodSplatter.transform.SetParent(null);
		}
	}
}
