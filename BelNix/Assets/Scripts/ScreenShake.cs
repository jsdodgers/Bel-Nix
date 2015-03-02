using UnityEngine;
using System.Collections;

public class ScreenShaker : MonoBehaviour {
    private float maxRadius = 1;
    private float startSpeed = 10;
    private const float FINAL_SPEED = 0;
    private float shakeDuration = 1;
    private float decceleration = 0.1f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void shake(GameObject objectToBeShaken, float shakeRadius, float shakeIntensity, float shakeDuration)
    {
        maxRadius = shakeRadius;
        startSpeed = shakeIntensity;
        this.shakeDuration = shakeDuration;
        decceleration = (shakeDuration * 2) / shakeIntensity;
        Transform objectTransform = objectToBeShaken.transform;
        Vector2 initialPosition = objectTransform.position;
        StartCoroutine(shakeLoop(objectTransform, initialPosition));
    }

    private IEnumerator shakeLoop(Transform transform, Vector2 initialPosition)
    {
        Vector2 targetPoint = nextPoint(initialPosition);
        for (float currentSpeed = startSpeed; currentSpeed > 0; currentSpeed -= decceleration)
        {
            float stepSize = startSpeed * Time.deltaTime;
            Vector2 currentPos = transform.position;
            while (currentPos != targetPoint)
            {
                currentPos = setPos(transform, Vector2.MoveTowards(currentPos, targetPoint, stepSize));
                yield return null;
            }
        }
    }

    private Vector2 nextPoint(Vector2 rootPosition)
    {
        return Random.insideUnitCircle;
    }

    private Vector2 setPos(Transform transform, Vector2 newPosition)
    {
        transform.position = newPosition;
        return newPosition;
    }
}
