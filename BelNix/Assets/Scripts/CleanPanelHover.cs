using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CleanPanelHover : MonoBehaviour, IPointerEnterHandler {
    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private bool primaryColorSelector;
    CCGUI ccGUI;

	// Use this for initialization
	void Start () {
        ccGUI = rootCanvas.GetComponent<CCGUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerEnter(PointerEventData e)
    {
        if (primaryColorSelector)
        {
            ccGUI.settingPrimaryColor();
        }
        else
        {
            ccGUI.settingSecondaryColor();
        }
    }
}
