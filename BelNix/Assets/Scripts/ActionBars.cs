using UnityEngine;
using System.Collections;

public class ActionBars : MonoBehaviour  {
	// The action arms are stored here. In order, they are Minor, Standard, and Movement
	[SerializeField] private GameObject[] actionArms = new GameObject[4];
	// Grab the console for quick access
	[SerializeField] private GameObject consoleCanvas;
	[SerializeField] private bool autoHide = false;
    [SerializeField] private float cursorBubbleRadius = 100.0f;
    private bool hidden = true;

    public void delayedRetractActionArm(string armType, float delay)
    {
        StartCoroutine(retractActionArm(armType, delay));
    }
    private IEnumerator retractActionArm(string armType, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (GameObject actionArm in actionArms)
        {
            if (actionArm.name.Contains(armType))
            {
                actionArm.GetComponent<Animator>().SetBool("Hidden", true);
            }
        }
    }

    private void updateBubbleCursor() {
        // Get the cursor position
        Vector2 cursorPos = Input.mousePosition;

        // Find the closest arm
        GameObject closestArm = actionArms[0];
        foreach (GameObject arm in actionArms) {
            if (Vector2.Distance(getScreenPosition(arm), cursorPos) < Vector2.Distance(getScreenPosition(closestArm), cursorPos))
                closestArm = arm;
        }

        // If the closest arm is within the bubble radius of the cursor, extend it and lower the other arms, otherwise lower all arms
        if (Vector2.Distance(getScreenPosition(closestArm), cursorPos) < cursorBubbleRadius) {
            setArm(closestArm, true);
            foreach (GameObject arm in actionArms) {
                if (arm != closestArm)
                    setArm(arm, false);
            }
        }
        else
            collapseAllArms();
    }

    private Vector2 getScreenPosition(GameObject item) {
        return RectTransformUtility.WorldToScreenPoint(null, item.GetComponent<RectTransform>().anchoredPosition) + new Vector2(Screen.width/2 + 250, 200);
        
        //return GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(new Vector3(item.GetComponent<RectTransform>().position.x,
        //                                                   item.GetComponent<RectTransform>().position.y, 0));
    }

    public void toggleAutoHide() {
        autoHide = !autoHide;
        if (!autoHide)
            extendAllArms();
        else
            collapseAllArms();

        //Debug.Log(getScreenPosition(actionArms[0]));
        //Debug.Log(getScreenPosition(actionArms[1]));
        //Debug.Log(getScreenPosition(actionArms[2]));
    }

    public void setHidden(bool val) {
        hidden = val;
        foreach (GameObject arm in actionArms)
            arm.GetComponent<Animator>().SetBool("Hidden", val);
    }

	public void toggleExtendCollapse(GameObject actionArm)  {

//		actionArm.GetComponent<Animator>().SetBool("Extended", !actionArm.GetComponent<Animator>().GetBool("Extended"));
		setArm(actionArm, !actionArm.GetComponent<Animator>().GetBool("Extended"));
		//adjustForConsole();
	}
	public void extendAllArms()  {
		foreach(GameObject arm in actionArms)
			setArm(arm, true);
	}
	public void collapseAllArms()  {
		foreach(GameObject arm in actionArms)
			setArm(arm, false);
	}


	private void setArm(GameObject arm, bool val)  {
		if (arm.name.Contains("Minor")) BattleGUI.armsShown[(int)ActionArm.Minor] = val;
		if (arm.name.Contains("Standard")) BattleGUI.armsShown[(int)ActionArm.Standard] = val;
		if (arm.name.Contains("Movement")) BattleGUI.armsShown[(int)ActionArm.Movement] = val;
		arm.GetComponent<Animator>().SetBool("Extended", val);
		bool anyOpen = false;
		foreach (GameObject arm2 in actionArms)  {
			anyOpen |= !arm2.GetComponent<Animator>().GetBool("Extended");
			if (anyOpen) break;
		}
		autoHide = anyOpen;
//		gameObject.GetComponent<ButtonSwap>().setSprite(anyOpen);
	//	BattleGUI.setActionsButtonDefault(!anyOpen);
	}


	public void adjustArmsForConsole()  {
        /*
		if(consoleCanvas.GetComponent<Animator>().GetBool("Hidden"))  {
			foreach(GameObject arm in actionArms)
				arm.GetComponent<Animator>().SetBool("Console Expanded", false);
		}
		else  {
			foreach(GameObject arm in actionArms)
				arm.GetComponent<Animator>().SetBool("Console Expanded", true);
		}
        */
	}


	// Use this for initialization
	void Start ()  {
	
	}

	
	// Update is called once per frame
	void Update ()  {
        //if (autoHide)
            //updateBubbleCursor();
	}
}
