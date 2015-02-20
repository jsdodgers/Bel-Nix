using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollbarResetter : MonoBehaviour {
    private Scrollbar physicalFeatureScrollbar;
    void Awake()
    {
        physicalFeatureScrollbar = GetComponent<Scrollbar>();
        physicalFeatureScrollbar.value = 0.999f;
        resetScrollBar();
        Debug.Log("ScrollBar Awake");
    }

	// Use this for initialization
	void Start () {
        physicalFeatureScrollbar.value = 0.999f;
        resetScrollBar();
	}

    void OnEnable()
    {
        physicalFeatureScrollbar.value = 0.999f;
        resetScrollBar();
        Debug.Log("ScrollBar Enabled");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void resetScrollBar()
    {
        physicalFeatureScrollbar.value = 1;
    }
}
