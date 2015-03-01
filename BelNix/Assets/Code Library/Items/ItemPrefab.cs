using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public struct ItemTextureObject {
	public string name;
	public Sprite texture;
	public GameObject sprite;
}

public class ItemPrefab : MonoBehaviour {
	[SerializeField] ItemTextureObject[] itemTextureObjects;
	// Use this for initialization
	public Dictionary<string, ItemTextureObject> itemTextures = new Dictionary<string, ItemTextureObject>();
	static ItemPrefab itemPrefab;
	void Start () {
	}

	void Awake() {
		foreach (ItemTextureObject ito in itemTextureObjects) {
			itemTextures[ito.name] = ito;
		}
		itemPrefab = this;
	
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	public static ItemTextureObject getPrefab(string name) {
		if (itemPrefab == null || !itemPrefab.itemTextures.ContainsKey(name)) return new ItemTextureObject();
		return itemPrefab.itemTextures[name];
	}
}
