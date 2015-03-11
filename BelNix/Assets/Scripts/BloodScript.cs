using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodScript : MonoBehaviour  {


    public static void spillBlood(Unit attacker, Unit enemy, int damage) {
        // Create and place the blood prefab
        GameObject blood = (GameObject)Instantiate(Resources.Load<GameObject>("Effects/Blood/blood_splatter"));
		SpriteRenderer bloodSR = blood.GetComponent<SpriteRenderer>();
		bloodSR.sortingOrder = MapGenerator.bloodOrder;
        blood.transform.SetParent(attacker.transform);
		if (enemy is TurretUnit)  {
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
        try {
            bloodManager = GameObject.Find("BloodManager").GetComponent<BloodManager>();
        }
        catch {
            Debug.Log("Creating a new BloodManager");
            GameObject newBloodManager = new GameObject("BloodManager", typeof(BloodManager));
            bloodManager = newBloodManager.GetComponent<BloodManager>();
        }
        int bloodNumber = bloodManager.generateBloodNumber();

        // Put the blood in its final position
        GameObject bloodContainer = new GameObject("Blood Container");
        bloodContainer.transform.position = attacker.transform.TransformPoint(enemyPosition) + new Vector3(0.5f, -0.5f, 0.0f);
        bloodContainer.transform.localEulerAngles = attacker.transform.localEulerAngles;
        blood.transform.SetParent(bloodContainer.transform);


         // Start the blood animation
        Debug.Log("Dealing " + damage + " damage"); 
        blood.AddComponent<BloodSplash>();
        blood.GetComponent<BloodSplash>().bloodOption = bloodNumber;
        blood.GetComponent<BloodSplash>().bloodManager = bloodManager;
        blood.GetComponent<BloodSplash>().sizeOption = damage;
        
        // Play the blood sound!
        GameObject.Find("AudioManager").GetComponent<AudioManager>().playAudioClip("blood-splash", 0.025f);
    }
}

public class BloodManager : MonoBehaviour {
    private const int QUEUE_SIZE = 5;
    private Queue<int> restrictedBloodAnimations;
    private List<Sprite> bloodSprites;
    void Start() {
        restrictedBloodAnimations = new Queue<int>(QUEUE_SIZE);

        bloodSprites = new List<Sprite>();
        for (int i = 1; i < 34; i++) {
            string numberAsString;
            if (i < 10)
                numberAsString = "0" + i;
            else
                numberAsString = i.ToString();

            bloodSprites.Add(Resources.Load<Sprite>("Materials/Particles/blood_splatter_image" + numberAsString));
        }

        Debug.Log(bloodSprites.Count + "Sprites loaded");
    }
    public int generateBloodNumber() {
        int bloodNumber = Random.Range(1, 34);
        while (restrictedBloodAnimations.Contains(bloodNumber)) {
            bloodNumber = Random.Range(1, 34);
        }
        if (restrictedBloodAnimations.Count >= QUEUE_SIZE)
            restrictedBloodAnimations.Dequeue();
        restrictedBloodAnimations.Enqueue(bloodNumber);
        return bloodNumber;
    }

    public Sprite getSprite(int bloodNumber) {
        return bloodSprites[bloodNumber - 1];
    }
}

public class BloodSplash : MonoBehaviour {
    public int sizeOption = 5;
    public int bloodOption = 1;
    private const int SPEED = 2;
    private const int MAX_SCALE = 8;
    private const float DURATION = 1.0f;
    private Vector3 finalPosition;
    public BloodManager bloodManager;

    void Start() {
        transform.localPosition = Vector3.zero;
        finalPosition = new Vector3(0,1,0);
        StartCoroutine("scaleBlood");
    }

    private IEnumerator scaleBlood() {
        GetComponent<SpriteRenderer>().sprite = bloodManager.getSprite(bloodOption);
        float currentScale = 1;
        Vector3 currentPosition = transform.localPosition;
        float timeSoFar = 0;
        while (timeSoFar < DURATION) {
            timeSoFar += Time.deltaTime * SPEED;

            currentPosition = Vector3.MoveTowards(currentPosition, finalPosition, timeSoFar);
            transform.localPosition = currentPosition;

            currentScale = Mathf.Lerp(currentScale, sizeOption, timeSoFar);
            transform.localScale = new Vector3(currentScale, currentScale, transform.localScale.z);

            yield return null;
        }
    }
}
