using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class InventoryGUI : MonoBehaviour  {
	
//	[SerializeField] private GameObject inventoryCanvas;
	[SerializeField] public static InventoryGUI inventoryGUI;
	public static Unit selectedUnit;
	public static Character selectedCharacter;
	// Use this for initialization
	void Start ()  {
	//	mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
	}

	void Awake()  {
	//	Debug.Log("This ");
	//	inventoryGUI = this;
	}
	
	// Update is called once per frame
	void Update ()  {
		if (Input.GetMouseButtonDown(0)) Debug.Log(isShown);
		if (Input.GetMouseButtonDown(0) && isShown)  {
			if (overlayObjects.Count > 0)  {
				selectItem();
			}
		}
		if (Input.GetMouseButtonUp(0) && isShown)  {
			InventoryGUI.deselectItem();
		}
		moveSelectedItem();

	}






	
	[SerializeField] private GameObject inventoryItemPrefab;
	[SerializeField] private GameObject inventoryBackground;
	[SerializeField] private GameObject inventorySlots;
	[SerializeField] private GameObject inventoryHead;
	[SerializeField] private GameObject inventoryShoulders;
	[SerializeField] private GameObject inventoryChest;
	[SerializeField] private GameObject inventoryGloves;
	[SerializeField] private GameObject inventoryRightHand;
	[SerializeField] private GameObject inventoryLeftHand;
	[SerializeField] private GameObject inventoryLegs;
	[SerializeField] private GameObject inventoryBoots;
	[SerializeField] private Text inventoryAC;
	[SerializeField] private Transform lootContent;
	[SerializeField] private GameObject lootOverlay;
	[SerializeField] private Button lootMoneyButton;
	[SerializeField] private Text lootMoneyText;
	[SerializeField] private Text moneyText;
	
	GameObject selectedItem;
	Vector3 mouseSelectPos = new Vector3();
	Vector2 selectedCell = new Vector2();
	InventorySlot originalSlot = InventorySlot.None;

    public static bool isShown = false;

    public static void setInventoryGUI(InventoryGUI i)
    {
        inventoryGUI = i;
        isShown = false;
    }
	
	public static void setInventoryAC()  {
		inventoryGUI.setACText();
	}
	
	void setACText()  {
		if (selectedCharacter == null && selectedUnit == null) return;
		inventoryAC.text = "AC: " + (selectedUnit != null ? selectedUnit.getAC() :selectedCharacter.characterSheet.characterLoadout.getAC());
	}
	
	public void moveSelectedItem()  {
		if (selectedItem != null) selectedItem.transform.position += (Input.mousePosition - mouseSelectPos);
		mouseSelectPos = Input.mousePosition;
	}
	
	public static void deselectItem()  {
		inventoryGUI.deselectItem((inventoryGUI.overlayObjects.Count > 0 ? inventoryGUI.overlayObjects[0] : null));
	}
	
	public void deselectItem(Image overlayObject)  {
		if (selectedItem == null)  {
			setLootInteractable(true);
			return;
		}
		CharacterSheet cs = selectedCharacter.characterSheet;
		Item i = selectedItem.GetComponent<InventoryItem>().item;
		InventorySlot insertSlot = originalSlot;
		if (overlayObjects.Count > 0)  {
			InventorySlot sl = overlayObjects[0].GetComponent<InventoryItem>().slot;
			if (sl == InventorySlot.None)  {
				insertSlot = sl;
			}
			else if (UnitGUI.armorSlots.Contains(sl))  {
				if (cs.characterLoadout.canInsertItemInSlot(sl, i, originalSlot))  {
					insertSlot = sl;
				}
			}
			else if (UnitGUI.inventorySlots.Contains(sl))  {
				Vector2 vSlot = UnitGUI.getIndexOfSlot(sl);
				int oSlot = UnitGUI.getLinearIndexFromIndex(vSlot);
				vSlot -= selectedCell;
				int iSlot = UnitGUI.getLinearIndexFromIndex(vSlot);
				if (iSlot >= 0 && iSlot < 16 && cs.inventory.canInsertItemInSlot(i, vSlot))  {
					insertSlot = UnitGUI.getInventorySlotFromIndex(vSlot);
				}
				else if (oSlot >= 0 && oSlot < 16)  {
					Item inSlot = cs.inventory.inventory[oSlot].getItem();
					if (inSlot != null && cs.inventory.itemCanStackWith(inSlot, i))  {
						insertSlot = sl;
					}
				}
			}
		}
		ActionType at = Inventory.getActionTypeForMovement(originalSlot, insertSlot);
		if (!canUseActionType(at)) insertSlot = originalSlot;
//		if (selectedUnit != null) {
//			if ((at == ActionType.Minor && selectedUnit.minorsLeft <= 0) || (at == ActionType.Standard && selectedUnit.usedStandard) || (at == ActionType.Movement && selectedUnit.usedMovement)) return;
//		}
		if (UnitGUI.armorSlots.Contains(insertSlot))  {
			Item i2 = cs.characterLoadout.getItemInSlot(insertSlot);
			GameObject oldItem = null;
			if (i2 != null)  {
				oldItem = getArmourParent(insertSlot).transform.FindChild("InventoryItem").gameObject;
			}
			cs.characterLoadout.setItemInSlot(insertSlot,i);
			Vector2 size = i.getSize() * 32.0f;
			GameObject armourParent = getArmourParent(insertSlot);
			selectedItem.transform.SetParent(armourParent.transform, false);
			Vector2 v = new Vector2();
			v.x = 32.0f - size.x/2.0f;
			if (size.y >= 64.0f)  {
				v.y = -64.0f + size.y;
			}
			else v.y = -32.0f + size.y/2.0f;
			selectedItem.GetComponent<RectTransform>().anchoredPosition = v;
			selectedItem.GetComponent<InventoryItem>().slot = originalSlot;
			GameObject canv = armourParent.transform.FindChild("Canvas").gameObject;//.FindChild("Overlay");
			RectTransform rt = canv.GetComponent<RectTransform>();
			rt.anchoredPosition = v;
			rt.sizeDelta = size;
			i = i2;
			if (i != null)  {
				selectedItem = oldItem;
				InventorySlot sl = originalSlot;
				Vector2 vSlot = UnitGUI.getIndexOfSlot(sl);
				//	vSlot -= selectedCell;
				sl = UnitGUI.getInventorySlotFromIndex(vSlot);
				int iSlot = UnitGUI.getLinearIndexFromIndex(vSlot);
				Debug.Log("Drop has item: " + sl + "  " + vSlot + "   " + iSlot + "  " + selectedCell);
				Item inSlot = (iSlot <0 || iSlot >= 16 ? null : cs.inventory.inventory[iSlot].getItem());
				if (cs.inventory.canInsertItemInSlot(i, vSlot) || (inSlot != null && cs.inventory.itemCanStackWith(inSlot, i)))  {
					insertSlot = sl;
				}
				else  {
					foreach (InventorySlot sl2 in UnitGUI.inventorySlots)  {
						sl = sl2;
						vSlot = UnitGUI.getIndexOfSlot(sl);
						iSlot = UnitGUI.getLinearIndexFromIndex(vSlot);
						inSlot = cs.inventory.inventory[iSlot].getItem();
						if (cs.inventory.canInsertItemInSlot(i, vSlot) || (inSlot != null && cs.inventory.itemCanStackWith(inSlot, i)))  {
							insertSlot = sl;
							break;
						}
						
					}
				}
				Debug.Log("Drop has item2: " + insertSlot + "  " + vSlot + "   " + iSlot + "  " + selectedCell);
				selectedCell = new Vector2(0,0);
			}
			if (selectedUnit != null) {
				selectedUnit.idleAnimation(false);
				selectedUnit.Invoke("beginIdle",0.05f);
			}
			
		}
		if (UnitGUI.inventorySlots.Contains(insertSlot))  {
			if (cs.inventory.canInsertItemInSlot(i, UnitGUI.getIndexOfSlot(insertSlot)))  {
				cs.inventory.insertItemInSlot(i, UnitGUI.getIndexOfSlot(insertSlot));
				selectedItem.transform.SetParent(inventorySlots.transform, false);
				Vector2 v = 32.0f * UnitGUI.getIndexOfSlot(insertSlot);
				v.y *= -1;
				selectedItem.GetComponent<RectTransform>().anchoredPosition = v;
				selectedItem.GetComponent<InventoryItem>().slot = insertSlot;
			}
			else  {
				Vector2 vSlot = UnitGUI.getIndexOfSlot(insertSlot);
				int iSlot = UnitGUI.getLinearIndexFromIndex(vSlot);
				Item inSlot = cs.inventory.inventory[iSlot].getItem();
				if (inSlot != null && cs.inventory.itemCanStackWith(inSlot, i))  {
					cs.inventory.stackItemWith(inSlot, i);
					GameObject[] obj = GameObject.FindGameObjectsWithTag("inventoryitem");
					foreach (GameObject go in obj)  {
						InventoryItem invP = go.GetComponent<InventoryItem>();
						if (invP.item == inSlot)  {
							invP.transform.FindChild("Text").GetComponent<Text>().text = (inSlot.stackSize() > 1 ? inSlot.stackSize() + "" : "");
						}
					}
				}
				GameObject.Destroy(selectedItem);
			}
		}
		if (insertSlot == InventorySlot.None)  {
			if (currentLootTile != null)
				currentLootTile.addItem(i);
			else if (currentStash != null)
				currentStash.addItem(i);
			selectedItem.transform.SetParent(lootContent, false);
			selectedItem.GetComponent<InventoryItem>().slot = insertSlot;
			selectedItem.GetComponent<LayoutElement>().ignoreLayout = false;
			selectedItem.GetComponent<CanvasGroup>().blocksRaycasts = true;
			selectedItem.transform.FindChild("Overlay").gameObject.SetActive(true);
		}
		selectedItem = null;
		List<Image> hoveredCopy = new List<Image>(overlayObjects);
		foreach (Image im in hoveredCopy)  {
			mouseHoverLeave(im);
			mouseHoverEnter(im);
		}
		setACText();
		setLootInteractable(true);
		if (selectedUnit != null) {
			if (at == ActionType.Minor) selectedUnit.useMinor(MinorType.Loot, false, false);
			else if (at == ActionType.Standard) selectedUnit.useStandard();
			if (!selectedUnit.usedStandard) {
				StandardType[] standards = selectedUnit.getStandardTypes();
				if (!sameAsOldStandards(standards)) {
					BattleGUI.resetStandardButtons();
				}
			}
		}
	}

	public bool sameAsOldStandards(StandardType[] standards) {
		if (standards == null || beforeItemStandards == null) return true;
		foreach (StandardType st in standards) if (!beforeItemStandards.Contains(st)) return false;
		foreach (StandardType st in beforeItemStandards) if (!standards.Contains(st)) return false;
		return true;
	}
	
	public static void selectItem()  {
		inventoryGUI.selectItem((inventoryGUI.overlayObjects.Count > 0 ? inventoryGUI.overlayObjects[0] : null));
	}

	StandardType[] beforeItemStandards = null;
	public void selectItem(Image overlayObject)  {
		beforeItemStandards = (selectedUnit != null ? selectedUnit.getStandardTypes() : new StandardType[0]);
		InventoryItem ii = overlayObject.GetComponent<InventoryItem>();
		InventorySlot sl = ii.slot;
		Item i = null;
		if (sl == InventorySlot.None)  {
			Debug.Log(overlayObjects.Count);
			InventoryItem iii = overlayObject.transform.parent.GetComponent<InventoryItem>();
			if (iii != null)  {
				i = iii.item;
				if (currentLootTile != null)
					currentLootTile.removeItem(i,0);
				else if (currentStash != null)
					currentStash.removeItem(i);
				originalSlot = sl;
				overlayObject.gameObject.SetActive(false);
				overlayObject.transform.parent.GetComponent<LayoutElement>().ignoreLayout = true;
				overlayObject.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
				mouseHoverLeave(overlayObject);
				Invoke("resetLootScrollPos",0.05f);
			}
		}
		else if (UnitGUI.armorSlots.Contains(sl))  {
			i = selectedCharacter.characterSheet.characterLoadout.removeItemFromSlot(sl);
			getArmourParent(sl).transform.FindChild("Canvas").GetComponent<RectTransform>().sizeDelta= new Vector2(64.0f, 64.0f);
			getArmourParent(sl).transform.FindChild("Canvas").GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
			originalSlot = sl;
		}
		else if (UnitGUI.inventorySlots.Contains(sl))  {
			ItemReturn i2 = selectedCharacter.characterSheet.inventory.removeItemFromSlot(sl);
			i = i2.item;
			originalSlot = UnitGUI.getInventorySlotFromIndex(UnitGUI.getIndexOfSlot(sl) - i2.slot);
		}
		if (i == null) return;
		GameObject[] items = GameObject.FindGameObjectsWithTag("inventoryitem");
		foreach (GameObject item in items)  {
			if (item.GetComponent<InventoryItem>().item == i)  {
				selectedItem = item;
				item.transform.SetParent(inventoryBackground.transform);
				Vector3 pos = item.transform.position;
				pos.y = Screen.height - pos.y;
				Vector3 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				selectedCell = new Vector2((int)((mousePos.x - pos.x)/32.0f), (int)((mousePos.y - pos.y)/32.0f));
				Vector2 closest = selectedCell;
				float closestDist = float.MaxValue;
				foreach (Vector2 v in i.getShape())  {
					float dist = Mathf.Abs(v.x - selectedCell.x) + Mathf.Abs(v.y - selectedCell.y);
					if (dist < closestDist)  {
						closestDist = dist;
						closest = v;
					}
					if (dist <= Mathf.Epsilon) break;
				}
				mouseSelectPos.x += (selectedCell.x - closest.x)*32.0f;
				mouseSelectPos.y += (selectedCell.y - closest.y)*32.0f;
				selectedCell = closest;
				break;
			}   
		}
		setACText();
		setLootInteractable(false);
	}
	
	public void setLootInteractable(bool interactable)  {
		if (interactable && inventoryGUI.overlayObjects.Contains(inventoryGUI.lootOverlay.GetComponent<Image>()))  {
			inventoryGUI.mouseHoverLeave(inventoryGUI.lootOverlay.GetComponent<Image>());
		}
		lootContent.transform.parent.GetComponent<ScrollRect>().vertical = interactable;
		for (int n=lootContent.transform.childCount-1;n>=0;n--)  {//lootContent.transform.parent.childCount;n++)  {
			lootContent.transform.GetChild(n).FindChild("Overlay").gameObject.SetActive(interactable);
		}
		lootOverlay.SetActive(!interactable && originalSlot != InventorySlot.None);
	}
	
	public static void clearLootItems()  {
		inventoryGUI.clearLoot();
	}
	
	public void clearLoot()  {
		
		lootMoneyText.gameObject.SetActive(false);
		lootMoneyButton.gameObject.SetActive(false);
		for (int n = lootContent.childCount-1;n>=0;n--)  {
			GameObject.Destroy(lootContent.GetChild(n).gameObject);
		}
	}
	
	public Tile currentLootTile;
	public Stash currentStash;
	public static void setLootItems(List<Item> items, Tile t, Stash s = null)  {
		inventoryGUI.setLoot(items, t, s);
	}

	public void pickUpMoney() {
		if (currentLootTile != null && selectedCharacter != null) {
			selectedCharacter.characterSheet.inventory.purse.receiveMoney(currentLootTile.getMoney());
			currentLootTile.removeMoney();
		}
		lootMoneyText.gameObject.SetActive(false);
		lootMoneyButton.gameObject.SetActive(false);
		setupInventory(selectedUnit, selectedCharacter);
	}
	
	public void setLoot(List<Item> items, Tile t, Stash s = null)  {
		currentLootTile = t;
		currentStash = s;
		if (currentStash != null) {
			moneyText.text = currentStash.moneyString().ToUpper();
		}
		else if (t != null) {
			lootMoneyText.text = t.getMoneyString();
			if (t.getMoney() > 0) {
				lootMoneyText.gameObject.SetActive(true);
				lootMoneyButton.gameObject.SetActive(true);
			}
		}
		foreach (Item i in items)  {
			if (i.inventoryTexture != null)  {
				GameObject invP = GameObject.Instantiate(inventoryItemPrefab) as GameObject;
				invP.name = "InventoryItem";
				invP.GetComponent<Image>().sprite = i.inventoryTexture;
				Vector2 size = i.getSize() * 32.0f;
				invP.GetComponent<RectTransform>().sizeDelta = size;
				invP.transform.SetParent(lootContent.transform, false);
				/*Vector2 v = 32.0f * Inventory.getSlotForIndex(iis.index);
				v.y *= -1;
				invP.GetComponent<RectTransform>().anchoredPosition = v;*/
				invP.GetComponent<InventoryItem>().item = i;
				invP.GetComponent<InventoryItem>().slot = InventorySlot.None;
				invP.transform.FindChild("Overlay").gameObject.SetActive(true);
				invP.transform.FindChild("Text").GetComponent<Text>().text = (i.stackSize() > 1 ? i.stackSize() + "" : "");
				LayoutElement loe = invP.GetComponent<LayoutElement>();
				CanvasGroup cg = invP.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = true;
				loe.ignoreLayout = false;
				loe.preferredHeight = size.y;
				loe.preferredWidth = size.x;
				if (i is Armor)  {
					switch (((Armor)i).armorType)  {
					case ArmorType.Head:
					case ArmorType.Chest:
					case ArmorType.Shoulder:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.primaryColor;
						break;
					case ArmorType.Gloves:
					case ArmorType.Pants:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.secondaryColor;
						break;
					default:
						break;
					}
				}
				Invoke("setLootScrollBar", 0.05f);
			}
		}
	}

	public void setLootScrollBar()  {
		lootContent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
	}
	
	public void resetLootScrollPos()  {
		lootContent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = lootContent.parent.GetComponent<ScrollRect>().verticalScrollbar.value-.0001f;
		Invoke("resetLootScrollPos2",0.05f);
	}
	public void resetLootScrollPos2()  {
		lootContent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = lootContent.parent.GetComponent<ScrollRect>().verticalScrollbar.value+.0001f;
	}

	public static void setupInvent(Character cs) {
		inventoryGUI.setupInventory(null, cs);
	}

	public static void setupInvent(Unit u)  {
		inventoryGUI.setupInventory(u, null);
	}

	
	public void setupInventory(Unit u, Character cs)  {
		lootMoneyText.gameObject.SetActive(false);
		lootMoneyButton.gameObject.SetActive(false);
		selectedUnit = u;
		selectedCharacter = cs;
		if (cs == null && u != null) selectedCharacter = u.characterSheet;
		GameObject[] inventoryParents = new GameObject[]  {inventorySlots, inventoryHead, inventoryShoulders, inventoryChest, inventoryGloves, inventoryRightHand, inventoryLeftHand, inventoryLegs, inventoryBoots};
		/*		GameObject[] oldInventory = GameObject.FindGameObjectsWithTag("inventoryitem");
		for (int n = oldInventory.Length-1;n >= 0; n--)  {
			Debug.Log("Destroy " + n);
			GameObject.Destroy(oldInventory[n]);
		}*/
		if (selectedUnit != null) {
			moneyText.text = selectedUnit.characterSheet.characterSheet.inventory.purse.moneyString().ToUpper();
		}
		foreach (GameObject g in inventoryParents)  {
			for (int n=g.transform.childCount-1;n>=0;n--)  {
				GameObject g2 = g.transform.GetChild(n).gameObject;
				if (g2.name == "InventoryItem") GameObject.Destroy(g2);
			}
		}
		foreach (InventoryItemSlot iis in selectedCharacter.characterSheet.inventory.inventory)  {
			if (iis.item != null && iis.item.inventoryTexture != null)  {
				Item i = iis.item;
				GameObject invP = GameObject.Instantiate(inventoryItemPrefab) as GameObject;
				invP.name = "InventoryItem";
				invP.GetComponent<Image>().sprite = i.inventoryTexture;
				Vector2 size = i.getSize() * 32.0f;
				invP.GetComponent<RectTransform>().sizeDelta = size;
				invP.transform.SetParent(inventorySlots.transform, false);
				Vector2 v = 32.0f * Inventory.getSlotForIndex(iis.index);
				v.y *= -1;
				invP.GetComponent<RectTransform>().anchoredPosition = v;
				invP.GetComponent<InventoryItem>().item = i;
				invP.GetComponent<InventoryItem>().slot = UnitGUI.inventorySlots[iis.index];
				invP.transform.FindChild("Text").GetComponent<Text>().text = (i.stackSize() > 1 ? i.stackSize() + "" : "");
				LayoutElement loe = invP.GetComponent<LayoutElement>();
				CanvasGroup cg = invP.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = false;
				loe.ignoreLayout = true;
				loe.preferredHeight = size.y;
				loe.preferredWidth = size.x;
				if (i is Armor)  {
					switch (((Armor)i).armorType)  {
					case ArmorType.Head:
					case ArmorType.Chest:
					case ArmorType.Shoulder:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.primaryColor;
						break;
					case ArmorType.Gloves:
					case ArmorType.Pants:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.secondaryColor;
						break;
					default:
						break;
					}
				}
			}
		}
		foreach (InventorySlot slot in UnitGUI.armorSlots)  {
			Item i = selectedCharacter.characterSheet.characterLoadout.getItemInSlot(slot);
			if (i != null && i.inventoryTexture != null)  {
				GameObject invP = GameObject.Instantiate(inventoryItemPrefab) as GameObject;
				invP.name = "InventoryItem";
				invP.GetComponent<Image>().sprite = i.inventoryTexture;
				Vector2 size = i.getSize() * 32.0f;
				invP.GetComponent<RectTransform>().sizeDelta = size;
				invP.transform.FindChild("Text").GetComponent<Text>().text = (i.stackSize() > 1 ? i.stackSize() + "" : "");
				GameObject armourParent = getArmourParent(slot);
				invP.transform.SetParent(armourParent.transform, false);
				Vector2 v = new Vector2();
				v.x = 32.0f - size.x/2.0f;
				if (size.y >= 64.0f)  {
					v.y = -64.0f + size.y;
				}
				else v.y = -32.0f + size.y/2.0f;
				invP.GetComponent<RectTransform>().anchoredPosition = v;
				invP.GetComponent<InventoryItem>().item = i;
				invP.GetComponent<InventoryItem>().slot = slot;
				LayoutElement loe = invP.GetComponent<LayoutElement>();
				CanvasGroup cg = invP.GetComponent<CanvasGroup>();
				cg.blocksRaycasts = false;
				loe.ignoreLayout = true;
				loe.preferredHeight = size.y;
				loe.preferredWidth = size.x;
				GameObject canv = armourParent.transform.FindChild("Canvas").gameObject;//.FindChild("Overlay");
				RectTransform rt = canv.GetComponent<RectTransform>();
				rt.anchoredPosition = v;
				rt.sizeDelta = size;
				if (i is Armor)  {
					switch (((Armor)i).armorType)  {
					case ArmorType.Head:
					case ArmorType.Chest:
					case ArmorType.Shoulder:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.primaryColor;
						break;
					case ArmorType.Gloves:
					case ArmorType.Pants:
						invP.GetComponent<Image>().color = selectedCharacter.characterSheet.characterColors.secondaryColor;
						break;
						//	case ArmorType.Boots:
						//		invP.GetComponent<Image>().color = new Color(.7f,.7f,.2f);
						//		break;
					default:
						break;
					}
				}
			}
		}
		setACText();
	}
	public GameObject getArmourParent(InventorySlot slot)  {
		switch (slot)  {
		case InventorySlot.Head:
			return inventoryHead;
		case InventorySlot.Shoulder:
			return inventoryShoulders;
		case InventorySlot.Chest:
			return inventoryChest;
		case InventorySlot.Glove:
			return inventoryGloves;
		case InventorySlot.RightHand:
			return inventoryRightHand;
		case InventorySlot.LeftHand:
			return inventoryLeftHand;
		case InventorySlot.Pants:
			return inventoryLegs;
		case InventorySlot.Boots:
			return inventoryBoots;
		default:
			return inventorySlots;
		}
	}
	
	public static void setInventoryShown(bool shown)  {
        isShown = shown;
        inventoryGUI.gameObject.SetActive(shown);
		if (!shown)  {
			inventoryGUI.clearLoot();
			while (inventoryGUI.overlayObjects.Count > 0)  {
				inventoryGUI.mouseHoverLeave(inventoryGUI.overlayObjects[0]);
			}
		}
	}
	//182 72 221
	Color goodColor = new Color(182.0f/255.0f, 72.0f/255.0f, 221.0f/255.0f, 103.0f/255.0f);
	Color badColor = new Color(1.0f, 0.0f, 0.0f, 103.0f/255.0f);
	public List<Image> overlayObjects = new List<Image>();
	public Dictionary<Image, List<Image>> overlayObjectList = new Dictionary<Image, List<Image>>();
	public void mouseHoverEnter(Image overlayObject)  {
		Color c = goodColor;
		//	c.a = 103.0f/255.0f;
		InventorySlot slot = overlayObject.GetComponent<InventoryItem>().slot;
		List<Image> otherImages = new List<Image>();
		if (selectedItem == null)  {
			if (UnitGUI.inventorySlots.Contains(slot))  {
				Debug.Log(slot);
				InventoryItemSlot iis = (selectedCharacter == null ? null : selectedCharacter.characterSheet.inventory.inventory[(int)slot - (int)InventorySlot.Zero]);
				if (iis != null && iis.hasItem())  {
					List<InventoryItemSlot> sllls = new List<InventoryItemSlot>();
					if (iis.itemSlot != iis) sllls.Add(iis.itemSlot);
					foreach (InventoryItemSlot iis2 in iis.itemSlot.otherSlots)  {
						if (iis2 == iis) continue;
						sllls.Add(iis2);
					}
					foreach (InventoryItemSlot iis2 in sllls)  {
						int slotind = iis2.index;
						Image img = overlayObject.transform.parent.GetChild(slotind).GetComponent<Image>();
						img.color = c;
						otherImages.Add(img);
					}
				}
			}
		}
		else  {
			Item i = selectedItem.GetComponent<InventoryItem>().item;
			ActionType at = Inventory.getActionTypeForMovement(slot, originalSlot);
			if (UnitGUI.inventorySlots.Contains(slot))  {
				Vector2 currentHighlightSlot = UnitGUI.getIndexOfSlot(slot);
				Vector2 originSlot = currentHighlightSlot - selectedCell;
				Item i2 = selectedCharacter.characterSheet.inventory.inventory[UnitGUI.getLinearIndexFromIndex(currentHighlightSlot)].getItem();
				if (i2 != null)  {
					if (!selectedCharacter.characterSheet.inventory.itemCanStackWith(i2,i) || !canUseActionType(at))  {
						c = badColor;
					}
					else  {
						i = i2;
						originSlot = UnitGUI.getIndexFromLinearIndex(selectedCharacter.characterSheet.inventory.inventory[UnitGUI.getLinearIndexFromIndex(currentHighlightSlot)].itemSlot.index);
					}
				}
				else  {
					if (!canUseActionType(at) || !(originSlot.y >= 0 && originSlot.x >= 0 && originSlot.x < 4 && originSlot.y < 4 && selectedCharacter.characterSheet.inventory.canInsertItemInSlot(i, originSlot)))  {
						c = badColor;
					}
				}
				foreach (Vector2 cell in i.getShape())  {
					Vector2 cc = originSlot + cell;
					if (cc != currentHighlightSlot && cc.x >= 0 && cc.y >= 0 && cc.x < 4 && cc.y < 4)  {
						int ind = UnitGUI.getLinearIndexFromIndex(cc);
						Image img = overlayObject.transform.parent.GetChild(ind).GetComponent<Image>();
						img.color = c;
						otherImages.Add(img);
					}
				}
			}
			else if (UnitGUI.armorSlots.Contains(slot))  {
				if (!selectedCharacter.characterSheet.characterLoadout.canInsertItemInSlot(slot, i, originalSlot))  {
					c = badColor;
				}
			}
			else if (!canUseActionType(at))  {
				c = badColor;
			}
		}
		if (!overlayObjectList.ContainsKey(overlayObject))
			overlayObjectList.Add(overlayObject, otherImages);
		overlayObject.color = c;
		
		if (!overlayObjects.Contains(overlayObject)) overlayObjects.Add(overlayObject);
	}
	
	public bool canUseActionType(ActionType at)  {
		if (selectedUnit == null) return true;
		switch (at)  {
		case ActionType.Minor:
			return selectedUnit.minorsLeft > 0;
		case ActionType.Standard:
			return !selectedUnit.usedStandard;
		default:
			return true;
		}
	}
	
	public void mouseHoverLeave(Image overlayObject)  {
		Color c = overlayObject.color;
		c.a = 3.0f/255.0f;
		overlayObject.color = c;
		if (overlayObjectList.ContainsKey(overlayObject))  {
			foreach (Image i in overlayObjectList[overlayObject])  {
				i.color = c;
			}
			overlayObjectList.Remove(overlayObject);
		}
		if (overlayObjects.Contains(overlayObject)) overlayObjects.Remove(overlayObject);
	}
	

}
