﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CCDisplayPanel : MonoBehaviour  {
	// Use this for initialization
	void Start()  {
	
	}
	
	// Update is called once per frame
	void Update()  {
	
	}

	public void toggleAllExcept(GameObject thisOne)  {
		GameObject.Find("Canvas - Character Creation").GetComponent<CCGUI>().toggleAllExcept(thisOne);
	}

	public void toggleAllExcept(GameObject[] theseOnes)  {
		GameObject.Find("Canvas - Character Creation").GetComponent<CCGUI>().toggleAllExcept(theseOnes);
	}
}
