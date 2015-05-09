using UnityEngine;
using System.Collections;

public class ScreenShaker : MonoBehaviour  {
    private float maxRadius = 1;
    private float startSpeed = 10;
    private const float FINAL_SPEED = 0;
    private float shakeDuration = 1;
    private float decceleration = 0.1f;
    private float currentRadius;
    private Vector3 startingPosition;
    private MapGenerator map;

    //public bool cameraPanning = false;

	// Use this for initialization
	void Start ()  {
        // Subscribe the screenshaker to the attackHit event, triggered when damage is calculated.
        Combat.getAttackHandler().attackHit += OnAttackHit;
        map = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
	}

    void OnAttackHit(AttackEventArgs args) {
        if (args.criticalHit)
            shake(Camera.main.gameObject, 0.4f, 12, 0.2f);
    }
	
	// Update is called once per frame
	void Update ()  {
	
	}

    public void shakeDatCamera() {
        shake(gameObject, 0.3f, 10, 0.2f);
    }

    public void shake(GameObject objectToBeShaken, float shakeRadius, float shakeIntensity, float shakeDuration) {
        startingPosition = objectToBeShaken.transform.position;
        maxRadius = shakeRadius;
        currentRadius = maxRadius;
        startSpeed = shakeIntensity;
        this.shakeDuration = shakeDuration;
        decceleration = (shakeDuration * 2) / shakeIntensity;
        Transform objectTransform = objectToBeShaken.transform;
        Vector2 initialPosition = objectTransform.position;
        StartCoroutine(shakeLoop(objectTransform, initialPosition));
    }

    private IEnumerator shakeLoop(Transform transform, Vector2 initialPosition) {
        Vector2 targetPoint = nextPoint(initialPosition);
        //for (float currentSpeed = startSpeed; currentSpeed > FINAL_SPEED; currentSpeed -= decceleration)
        for (int i = 0; i < 10; i++) {
            float stepSize = startSpeed * Time.deltaTime;
            Vector2 currentPos = transform.position;
            while (currentPos != targetPoint) {
                if (map.movingCamera) {
                    yield break;
				}
                currentPos = setPos(transform, Vector2.MoveTowards(currentPos, targetPoint, stepSize));
                yield return null;
            }
            targetPoint = nextPoint(currentPos);
        }
    }

    private Vector2 nextPoint(Vector2 rootPosition) {
        currentRadius -= 0.1f;
        return new Vector2(startingPosition.x, startingPosition.y) + ((currentRadius <= 0) ? Vector2.zero : (new Vector2(Random.Range(0.2f, currentRadius*10)/10 * coinFlip(), Random.Range(0.2f, currentRadius*10)/10 * coinFlip())));
    }

    private Vector2 setPos(Transform transform, Vector2 newPosition) {
        //transform.position = newPosition;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingPosition.z);
        return newPosition;
    }
    private int coinFlip() {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }
}
