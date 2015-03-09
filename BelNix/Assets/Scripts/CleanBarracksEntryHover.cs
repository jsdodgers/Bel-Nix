using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CleanBarracksEntryHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {
    [SerializeField] private GameObject entry;
    [SerializeField] private GameObject options;

	// Use this for initialization
	void Start ()  {
        
	}
	
	// Update is called once per frame
	void Update ()  {
	
	}

    public void OnPointerEnter(PointerEventData e)
    {
        entry.GetComponent<BarracksEntry>().showPanel(options);
    }

    public void OnPointerExit(PointerEventData e)
    {
        entry.GetComponent<BarracksEntry>().onStopHovering();
    }
}
