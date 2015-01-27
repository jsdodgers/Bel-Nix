using UnityEngine;
using System.Collections;

public class MapTooltip : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 mousePos = Input.mousePosition;
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 toolTipPos = new Vector3(Mathf.Floor(mousePos.x)+0.5f, Mathf.Floor(mousePos.y)+1.5f, -9.5f);
		gameObject.GetComponent<RectTransform>().position = toolTipPos;
		Debug.Log(mousePos);
	}
}
