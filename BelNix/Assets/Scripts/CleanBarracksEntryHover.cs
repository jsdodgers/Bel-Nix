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
		BarracksEntry be = entry.GetComponent<BarracksEntry>();
		BaseManager.currentHoverCharacter = be.character;
        be.showPanel(options);
    }

    public void OnPointerExit(PointerEventData e)
    {
		BarracksEntry be = entry.GetComponent<BarracksEntry>();
		BaseManager.currentHoverCharacter = null;
		be.onStopHovering();
    }
}
