using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class InventoryItemEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
//	public void mouseHoverEnter(Image overlayObject) {
	public void OnPointerEnter(PointerEventData data) {
		Debug.Log("PointerEnter");
		BattleGUI.battleGUI.mouseHoverEnter(gameObject.GetComponent<Image>());
	}

//	public void mouseHoverLeave(Image overlayObject) {
	public void OnPointerExit(PointerEventData data) {
		Debug.Log("PointerExit");
		BattleGUI.battleGUI.mouseHoverLeave(gameObject.GetComponent<Image>());
	}
}
