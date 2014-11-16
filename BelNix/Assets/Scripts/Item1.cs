using UnityEngine;
using System.Collections;

public enum ItemType {WEAPON, ARMOR, USEABLE, AMMUNITION, MISC}

public class Item1 : MonoBehaviour {

	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
