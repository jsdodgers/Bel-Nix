using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UITooltip : MonoBehaviour {

    private bool visible = false;
    private GameObject tooltip;
    private const int PADDING = 5;

	// Use this for initialization
	void Start () {
		//Debug.Log(getTooltipText(gameObject));
        tooltip = generateTooltip();
        //Invoke("fitToScreenEdges", 5);
        hideTooltip();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    
    public void displayTooltip()
    {
        tooltip.SetActive(true);
        visible = true;
    }

    public void hideTooltip()
    {
        if (tooltip == null)
            return;
        tooltip.SetActive(false);
        visible = false;
    }

    private GameObject generateTooltip()
    {
        // Initialize the Panel and Text
        GameObject ttPanel  = new GameObject("Panel - UITooltip");
		ttPanel.transform.SetParent(gameObject.transform);
        ttPanel.AddComponent<RectTransform>();
        ttPanel.GetComponent<RectTransform>().localScale = Vector2.one;
        ttPanel.AddComponent<Image>();
		ttPanel.AddComponent<Canvas>();
        ttPanel.AddComponent<ContentSizeFitter>();
        ttPanel.AddComponent<HorizontalLayoutGroup>();
        
        GameObject ttText   = new GameObject("Text - UITooltip");
        ttText.AddComponent<RectTransform>();
        ttText.transform.SetParent(ttPanel.transform);
        ttText.GetComponent<RectTransform>().localScale = Vector2.one;
        ttText.AddComponent<Text>();
        
        // Initialize some components
        ttText.GetComponent<Text>().text = getTooltipText(gameObject);
		ttText.GetComponent<Text>().fontSize = 14;
        ttText.GetComponent<Text>().font = Resources.Load<Font>("Fonts/Courier New");
        ttText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        ttText.GetComponent<Text>().color = Color.black;
        Debug.Log("Line spacing is: " + ttText.GetComponent<Text>().lineSpacing);

		ttPanel.GetComponent<Canvas>().overrideSorting = true;
		ttPanel.GetComponent<Canvas>().sortingOrder = 5;
        ttPanel.GetComponent<Canvas>().overridePixelPerfect = true;
        ttPanel.GetComponent<Canvas>().pixelPerfect = true;
        ttPanel.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/tooltip_background");
        ttPanel.GetComponent<Image>().type = Image.Type.Sliced;
        ttPanel.GetComponent<Image>().fillCenter = true;
        ttPanel.GetComponent<RectTransform>().anchoredPosition = Vector3.zero + new Vector3(0, -40, 0);
        ttPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(80.0f, 40.0f);
        ttPanel.GetComponent<HorizontalLayoutGroup>().padding.left      = PADDING;
        ttPanel.GetComponent<HorizontalLayoutGroup>().padding.right     = PADDING;
        ttPanel.GetComponent<HorizontalLayoutGroup>().padding.top       = PADDING;
        ttPanel.GetComponent<HorizontalLayoutGroup>().padding.bottom    = PADDING;
        ttPanel.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        ttPanel.GetComponent<ContentSizeFitter>().verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        return ttPanel;
    }

    private void fitToScreenEdges()
    {
        /*
        RectTransform ttRect = tooltip.GetComponent<RectTransform>();
        Vector2 oldV = ttRect.sizeDelta;
        Vector2 v = new Vector2(ttRect.sizeDelta.x, ttRect.sizeDelta.y * 1000);
        ttRect.sizeDelta = v;
        if (RectTransformUtility.RectangleContainsScreenPoint(ttRect, new Vector2(0, 0), Camera.main))
        {
            Debug.Log("Whoaaaa!!");
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(ttRect, new Vector2(Screen.width, Screen.height), Camera.main))
        {
            Debug.Log("EEEEEEEEEE!!");
        }
        ttRect.sizeDelta = oldV;
         */
        RectTransform ttRect = tooltip.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        ttRect.GetWorldCorners(corners);
        float[] xCoords = new float[4];
        for (int i = 0; i < corners.Length; i++)// v in corners)
        {
            corners[i] = Camera.main.WorldToScreenPoint(corners[i]);
            xCoords[i] = corners[i].x;
        }
        foreach (float f in xCoords)
        {
            if (f < 0)
                Debug.Log("Whoaaaaa");
            else if (f > Screen.width)
                Debug.Log(f);//"EEEEEEEEEE");
        }
    }

    private string getTooltipText(GameObject UIElement)
    {
        switch (UIElement.name)
        {
            // Movement actions
            case "Button - Move":
                return "Move: \n" + "Run (up to) five squares.";
		case "Button - Back Step":
                return "Back Step: \n" + "Safely move one square away from an enemy.";
		case "Button - Recover":
                return "Recover: \n" + "Stand up after being knocked prone.";

            // Standard actions
		case "Button - Attack":
                return "Attack: \n" + "Attack an enemy within range.";
		case "Button - Over Clock":
                return "Over Clock: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Over_Clock);
		case "Button - Throw":
                return "Throw: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Throw);
		case "Button - Intimidate":
                return "Intimidate: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Intimidate);
		case "Button - Place Turret":
                return "Place Turret: \n" + "Place a turret and set its direction.";
		case "Button - Lay Trap":
                return "Lay Trap: \n" + "Lay a trap down.";
		case "Button - Inventory":
                return "Inventory: \n" + "Take an item out of your bag.";

            // Minor actions
		case "Button - Loot":
                return "Loot: \n" + "Grab an item from a nearby square.";
		case "Button - Stealth":
                return "Stealth: \n" + "Make yourself less visible to enemies.";
		case "Button - Mark":
                return "Mark: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Mark);
		case "Button - Tempered Hands":
                return "Tempered Hands: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Tempered_Hands);
		case "Button - Escape":
                return "Escape: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Escape);
		case "Button - Invoke":
                return "Invoke: \n" + ClassFeatures.getTooltipDescription(ClassFeature.Invoke);
            // One of Many goes here.

            // Class Features
            //case "Text - Class Feature":
            //    return "";
            //case "Text - Class Feature2":
            //    return "";

            default:
                return "";
        }
    }

}
