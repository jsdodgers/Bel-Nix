using UnityEngine;
using System.Collections;

public class BloodScript : MonoBehaviour {

    public static void spillBlood(GameObject attacker, GameObject enemy)
    {
        // Create and place the blood prefab
        GameObject blood = (GameObject)Instantiate(Resources.Load<GameObject>("Effects/Blood/blood_splatter"));
        blood.GetComponent<SpriteRenderer>().sortingOrder = MapGenerator.bloodOrder;
        blood.transform.SetParent(attacker.transform);
        Unit enemyUnit = enemy.GetComponent<Unit>();
        Vector3 enemyPosition = attacker.transform.InverseTransformPoint(enemyUnit.position);
        blood.transform.localPosition = Vector3.zero + new Vector3(0, 1, 0) + enemyPosition;
        blood.transform.localEulerAngles = attacker.transform.localEulerAngles;
        if (Unit.directionOf(attacker.GetComponent<Unit>(), enemyUnit) == Direction.Down)
            blood.transform.localEulerAngles += new Vector3(0, 0, 180);
        if (Unit.directionOf(attacker.GetComponent<Unit>(), enemyUnit) == Direction.Right)
            blood.transform.localEulerAngles += new Vector3(0, 0, 90);
        if (Unit.directionOf(attacker.GetComponent<Unit>(), enemyUnit) == Direction.Left)
            blood.transform.localEulerAngles += new Vector3(0, 0, 270);
        int bloodNumber = Random.Range(1, 11);

        // Start the blood animation
        blood.GetComponent<Animator>().SetInteger("BloodOption", bloodNumber);
        GameObject bloodContainer = new GameObject("Blood Container");

        // Put the blood in its final position
        bloodContainer.transform.position = attacker.transform.TransformPoint(enemyPosition) + new Vector3(0.5f, -0.5f, 0.0f);
        bloodContainer.transform.localEulerAngles = attacker.transform.localEulerAngles;
        blood.transform.SetParent(bloodContainer.transform);
    }


    /*
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
    */ 
}
