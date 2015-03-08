using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodScript : MonoBehaviour {

   
    

    void Start()
    {
        //restrictedBloodAnimations ;
    }

    public static void spillBlood(Unit attacker, Unit enemy, int damage = -1)
    {
        // Create and place the blood prefab
        GameObject blood = (GameObject)Instantiate(Resources.Load<GameObject>("Effects/Blood/blood_splatter"));
		SpriteRenderer bloodSR = blood.GetComponent<SpriteRenderer>();
		bloodSR.sortingOrder = MapGenerator.bloodOrder;
        blood.transform.SetParent(attacker.transform);
		if (enemy is TurretUnit) {
			bloodSR.color = Color.black;
		}
        Unit enemyUnit = enemy;
        Vector3 enemyPosition = attacker.transform.InverseTransformPoint(enemyUnit.position);
        blood.transform.localPosition = Vector3.zero + new Vector3(0, 1, 0) + enemyPosition;
        blood.transform.localEulerAngles = attacker.transform.localEulerAngles;
        if (Unit.directionOf(attacker, enemyUnit) == Direction.Down)
            blood.transform.localEulerAngles += new Vector3(0, 0, 180);
        if (Unit.directionOf(attacker, enemyUnit) == Direction.Right)
            blood.transform.localEulerAngles += new Vector3(0, 0, 90);
        if (Unit.directionOf(attacker, enemyUnit) == Direction.Left)
            blood.transform.localEulerAngles += new Vector3(0, 0, 270);

		blood.transform.localEulerAngles = new Vector3(0, 0, (MapGenerator.getAngle(attacker.transform.position, enemyUnit.transform.position) + 90 + Random.Range(-10, 10)) % 360);
        BloodManager bloodManager;
        try
        {
            bloodManager = GameObject.Find("BloodManager").GetComponent<BloodManager>();
        }
        catch
        {
            Debug.Log("Creating a new BloodManager");
            GameObject newBloodManager = new GameObject("BloodManager", typeof(BloodManager));
            bloodManager = newBloodManager.GetComponent<BloodManager>();
        }
        int bloodNumber = bloodManager.generateBloodNumber();
        

        // Start the blood animation
        blood.GetComponent<Animator>().SetInteger("BloodOption", bloodNumber);
        GameObject bloodContainer = new GameObject("Blood Container");

        // Put the blood in its final position
        bloodContainer.transform.position = attacker.transform.TransformPoint(enemyPosition) + new Vector3(0.5f, -0.5f, 0.0f);
        bloodContainer.transform.localEulerAngles = attacker.transform.localEulerAngles;
        if (damage > -1)
            blood.transform.localScale *= (0.3f + (damage/5));
        blood.transform.SetParent(bloodContainer.transform);
    }
}

public class BloodManager : MonoBehaviour
{
    private const int QUEUE_SIZE = 5;
    private Queue<int> restrictedBloodAnimations;
    void Start()
    {
        restrictedBloodAnimations = new Queue<int>(QUEUE_SIZE);
    }
    public int generateBloodNumber()
    {
        int bloodNumber = Random.Range(1, 34);
        while (restrictedBloodAnimations.Contains(bloodNumber))
        {
            bloodNumber = Random.Range(1, 34);
        }
        if (restrictedBloodAnimations.Count >= QUEUE_SIZE)
            restrictedBloodAnimations.Dequeue();
        restrictedBloodAnimations.Enqueue(bloodNumber);
        return bloodNumber;
    }
}
