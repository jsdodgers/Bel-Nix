using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitGUI  {
	static Texture2D playerBannerTexture;
	static Texture2D bottomSheetTexture;
	static Texture2D portraitBorderTexture;
	public static Vector2 classFeaturesScrollPos = new Vector2(0.0f, 0.0f);
	public static Vector2 groundScrollPosition = new Vector2(0.0f, 0.0f);

	
	public static float cPerc = 0.0f;
	public static float kPerc = 0.0f; 
	const float paperDollHeadSize = 90.0f;

	const float portraitBorderSize = 107.0f;
	const float bagButtonSize = 60.0f;
	public const float bannerX = -130.0f;
	public const float bannerY = -160.0f;
	public const float bannerHeight = 317.0f;
	public const float bannerWidth = 500.0f;
	public const float bottomSheetWidth = 500.0f;
	public const float bottomSheetHeight = 500.0f;
	const float cLeft = 8.0f;
	const float kLeft = 14.0f;
	const float inventoryWidth = 300.0f;
	const float inventoryHeight = 400.0f;
	public const float inventoryCellSize = 24.0f;
	const float inventoryLineThickness = 2.0f;

	public const float baseY = 50.0f;
	public static float baseX = (Screen.width - inventoryWidth)/2.0f + 25.0f;
	public const float armorWidth = inventoryCellSize * 7;
	public const float change = inventoryCellSize - inventoryLineThickness;
	public const float change2 = inventoryCellSize - inventoryLineThickness/2.0f;
	public const float groundY = baseY + change*4 + inventoryCellSize;
	public const float groundHeight = inventoryHeight - groundY;
	public static float groundX = baseX + armorWidth;
	public static float groundWidth = inventoryWidth + (Screen.width - inventoryWidth)/2.0f - groundX - 3.0f;

	
	public static Tab openTab = Tab.None;
//	public static bool inventoryOpen = false;
	
	public static void clickTab(Tab tab)  {
		if (tab == Tab.B)  {
		/*	if (GameGUI.looting)  {
				GameGUI.selectedMinorType = MinorType.None;
				GameGUI.selectMinorType(MinorType.None);
			}
			else*/ //i//nventoryOpen = !inventoryOpen;
		//	InventoryGUI.setInventoryShown(inventoryOpen);
			return;
		}
		if (openTab==tab) openTab = Tab.None;
		else openTab = tab;
	}
	
	static GUIStyle courierStyle;
	public static GUIStyle getCourierStyle(int size)  {
		if (courierStyle == null)  {
			courierStyle = new GUIStyle("Label");
			courierStyle.font = Resources.Load<Font>("Fonts/Courier New");
			courierStyle.normal.textColor = Color.black;
		}
		courierStyle.fontSize = size;
		return courierStyle;
	}
	
	public static string getSmallCapsString(string original, int size)  {
		string newS = "";
		char[] chars = original.ToCharArray();
		bool inLowerCase = false;
		foreach (char c in chars)  {
			if (c >= 'a' && c <= 'z')  {
				if (!inLowerCase)  {
					newS += "<size=" + size + ">";
					inLowerCase = true;
				}
				int i = (int)c;
				i -= (int)'a';
				i += (int)'A';
				newS += ((char)i);
			}
			else  {
				if (c == '\'' || c == '-' || c=='.' || c==',' || c=='+') {// || (c>='0' && c<='9'))  {
					if (!inLowerCase)  {
						newS += "<size=" + size + ">";
						inLowerCase = true;
					}
				}
				else if (inLowerCase)  {
					newS += "</size>";
					inLowerCase = false;
				}
				newS += c;
			}
		}
		if (inLowerCase) newS += "</size>";
		return newS;
	}


	public static Vector2 getInventorySlotPos(InventorySlot slot)  {
		switch (slot)  {
		case InventorySlot.Head:
			return new Vector2(baseX + change2*2, baseY);
		case InventorySlot.Shoulder:
			return new Vector2(baseX, baseY + change2);
		case InventorySlot.Back:
			return new Vector2(baseX + change2*4, baseY + change2);
		case InventorySlot.Chest:
			return new Vector2(baseX + change2*2, baseY + change2*4);
		case InventorySlot.Glove:
			return new Vector2(baseX + change2*4, baseY + change2*5);
		case InventorySlot.RightHand:
			return new Vector2(baseX + change2*.5f, baseY + change2*7);
		case InventorySlot.LeftHand:
			return new Vector2(baseX + change2*3.5f, baseY + change2*7);
		case InventorySlot.Pants:
			return new Vector2(baseX + change2*2, baseY + change2*9);
		case InventorySlot.Boots:
			return new Vector2(baseX + change2*.5f, baseY + change2*11);
		case InventorySlot.Zero:
			return new Vector2(baseX + armorWidth, baseY);
		case InventorySlot.One:
			return new Vector2(baseX + armorWidth + change, baseY);
		case InventorySlot.Two:
			return new Vector2(baseX + armorWidth + change*2, baseY);
		case InventorySlot.Three:
			return new Vector2(baseX + armorWidth + change*3, baseY);
		case InventorySlot.Four:
			return new Vector2(baseX + armorWidth, baseY + change);
		case InventorySlot.Five:
			return new Vector2(baseX + armorWidth + change, baseY + change);
		case InventorySlot.Six:
			return new Vector2(baseX + armorWidth + change*2, baseY + change);
		case InventorySlot.Seven:
			return new Vector2(baseX + armorWidth + change*3, baseY + change);
		case InventorySlot.Eight:
			return new Vector2(baseX + armorWidth, baseY + change*2);
		case InventorySlot.Nine:
			return new Vector2(baseX + armorWidth + change, baseY + change*2);
		case InventorySlot.Ten:
			return new Vector2(baseX + armorWidth + change*2, baseY + change*2);
		case InventorySlot.Eleven:
			return new Vector2(baseX + armorWidth + change*3, baseY + change*2);
		case InventorySlot.Twelve:
			return new Vector2(baseX + armorWidth, baseY + change*3);
		case InventorySlot.Thirteen:
			return new Vector2(baseX + armorWidth + change, baseY + change*3);
		case InventorySlot.Fourteen:
			return new Vector2(baseX + armorWidth + change*2, baseY + change*3);
		case InventorySlot.Fifteen:
			return new Vector2(baseX + armorWidth + change*3, baseY + change*3);
			
		default:
			return new Vector2();
		}
	}

	
	public static Rect getInventorySlotRect(InventorySlot slot)  {
		Vector2 v = getInventorySlotPos(slot);
		switch (slot)  {
		case InventorySlot.Head:
		case InventorySlot.Shoulder:
		case InventorySlot.Back:
		case InventorySlot.Chest:
		case InventorySlot.Glove:
		case InventorySlot.RightHand:
		case InventorySlot.LeftHand:
		case InventorySlot.Pants:
		case InventorySlot.Boots:
			return new Rect(v.x, v.y, inventoryCellSize*2, inventoryCellSize*2);
		case InventorySlot.Zero:
		case InventorySlot.One:
		case InventorySlot.Two:
		case InventorySlot.Three:
		case InventorySlot.Four:
		case InventorySlot.Five:
		case InventorySlot.Six:
		case InventorySlot.Seven:
		case InventorySlot.Eight:
		case InventorySlot.Nine:
		case InventorySlot.Ten:
		case InventorySlot.Eleven:
		case InventorySlot.Twelve:
		case InventorySlot.Thirteen:
		case InventorySlot.Fourteen:
		case InventorySlot.Fifteen:
			return new Rect(v.x, v.y, inventoryCellSize, inventoryCellSize);
		default:
			return new Rect();
		}
	}
	
	public static InventorySlot[] armorSlots = new InventorySlot[] {InventorySlot.Head,InventorySlot.Shoulder,InventorySlot.Back,InventorySlot.Chest,InventorySlot.Glove,InventorySlot.RightHand,InventorySlot.LeftHand,InventorySlot.Pants,InventorySlot.Boots};
	public static InventorySlot[] inventorySlots = new InventorySlot[] {InventorySlot.Zero, InventorySlot.One,InventorySlot.Two,InventorySlot.Three,InventorySlot.Four,InventorySlot.Five,InventorySlot.Six,InventorySlot.Seven,InventorySlot.Eight,InventorySlot.Nine,InventorySlot.Ten,InventorySlot.Eleven, InventorySlot.Twelve, InventorySlot.Thirteen, InventorySlot.Fourteen, InventorySlot.Fifteen};
	public static InventorySlot[] trapTurretSlots = new InventorySlot[] {};//InventorySlot.Frame, InventorySlot.Applicator, InventorySlot.Gear, InventorySlot.TriggerEnergySource};
	public static InventorySlot  getInventorySlotFromIndex(Vector2 index)  {
		//		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return InventorySlot.None;
		//		int ind = (int)index.x + ((int)index.y)*4;
		int ind = getLinearIndexFromIndex(index);
		if (ind==-1) return InventorySlot.None;
		return inventorySlots[ind];
	}
	public static int getLinearIndexFromIndex(Vector2 index)  {
		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return -1;
		return (int)index.x + ((int)index.y)*4;
	}
	public static Vector2 getIndexFromLinearIndex(int index)  {
		if (index <0 || index > 15) return new Vector2(-1, -1);
		return new Vector2(index%4,index/4);
	}
	public static Vector2 getIndexOfSlot(InventorySlot slot)  {
		switch (slot)  {
		case InventorySlot.Zero:
			return new Vector2(0,0);
		case InventorySlot.One:
			return new Vector2(1,0);
		case InventorySlot.Two:
			return new Vector2(2,0);
		case InventorySlot.Three:
			return new Vector2(3,0);
		case InventorySlot.Four:
			return new Vector2(0,1);
		case InventorySlot.Five:
			return new Vector2(1,1);
		case InventorySlot.Six:
			return new Vector2(2,1);
		case InventorySlot.Seven:
			return new Vector2(3,1);
		case InventorySlot.Eight:
			return new Vector2(0,2);
		case InventorySlot.Nine:
			return new Vector2(1,2);
		case InventorySlot.Ten:
			return new Vector2(2,2);
		case InventorySlot.Eleven:
			return new Vector2(3,2);
		case InventorySlot.Twelve:
			return new Vector2(0,3);
		case InventorySlot.Thirteen:
			return new Vector2(1,3);
		case InventorySlot.Fourteen:
			return new Vector2(2,3);
		case InventorySlot.Fifteen:
			return new Vector2(3,3);
		default:
			return new Vector2(-1,-1);
		}
	}
	
	
	
	public static Item selectedItem;
	public static InventorySlot selectedItemWasInSlot;
	public static Vector3 selectedMousePos = new Vector3();
	public static Vector2 selectedItemPos = new Vector2();
	public static Vector2 selectedCell = new Vector2();
	public static void selectItem(Character characterSheet)  {
		selectItem(characterSheet, null, null);
	}
	public static void selectItem(Character characterSheet, MapGenerator mapGenerator, Unit u)  {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		foreach (InventorySlot slot in inventorySlots)  {
			Rect r = UnitGUI.getInventorySlotRect(slot);
			if (r.Contains(mousePos))  {
				Vector2 v = getIndexOfSlot(slot);
				//				Debug.Log(v);
				int ind = getLinearIndexFromIndex(v);
				InventoryItemSlot sl = characterSheet.characterSheet.inventory.inventory[ind];
				InventoryItemSlot slR = sl.itemSlot;
				if (slR==null) break;
				//	Item i = slR.item;
				Vector2 itemSlot = Inventory.getSlotForIndex(ind);
				ItemReturn ir = characterSheet.characterSheet.inventory.removeItemFromSlot(itemSlot);
				selectedItem = ir.item;
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))  {
					if (selectedItem.stackSize()>1)  {
						characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, itemSlot - ir.slot);
						selectedItem = selectedItem.popStack();
					}
				}
				selectedCell = ir.slot;
				selectedMousePos = mousePos;
				//				selectedItemPos = getInventorySlotPos();
				selectedItemPos = UnitGUI.getInventorySlotPos(inventorySlots[slR.index]);
				selectedItemWasInSlot = inventorySlots[slR.index];
				break;
			}
		}
//		if (!GameGUI.looting || mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight) return;
		Vector2 scrollOff = UnitGUI.groundScrollPosition;
		float div = 20.0f;
		float y = div + UnitGUI.groundY - scrollOff.y;
		float mid = UnitGUI.groundX + UnitGUI.groundWidth/2.0f + scrollOff.x;
		//	mousePos.y += groundScrollPosition.y;
		selectedItem = null;
		if (mapGenerator != null)  {
		List<Item> groundItems = mapGenerator.tiles[(int)u.position.x,(int)-u.position.y].getReachableItems();
		foreach (Item i in groundItems)  {
			if (i.inventoryTexture==null) continue;
			//	Debug.Log(mousePos.x + "  " + mousePos.y + "       " + mid + "  " + y);
			Vector2 size = i.getSize();
			float x = mid - size.x*inventoryCellSize/2.0f;
			Rect r = new Rect(x, y, size.x*inventoryCellSize, size.y*UnitGUI.inventoryCellSize);
			if (r.Contains(mousePos))  {
				//	Debug.Log(i);
				selectedCell = new Vector2((int)((mousePos.x - x)/inventoryCellSize), (int)((mousePos.y - y)/inventoryCellSize));
				foreach (Vector2 cell in i.getShape())  {
					if (cell.x == selectedCell.x && cell.y == selectedCell.y)  {
						selectedItemPos = new Vector2(x, y);
						selectedMousePos = mousePos;
						selectedItem = i;
						selectedItemWasInSlot = InventorySlot.None;
					}
				}
				Debug.Log(selectedCell);
				if (selectedItem!=null)  {
					break;
				}
			}
			y += size.y*UnitGUI.inventoryCellSize + div;
		}
		}
	}
	public static void deselectItem(Character characterSheet)  {
		deselectItem(characterSheet, null, null);
	}

	public static void deselectItem(Character characterSheet, MapGenerator mapGenerator, Unit u)  {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		Tile t = (mapGenerator != null && u!=null ? mapGenerator.tiles[(int)u.position.x,(int)-u.position.y] : null);
		foreach (InventorySlot slot in inventorySlots)  {
			Rect r = UnitGUI.getInventorySlotRect(slot);
			if (r.Contains(mousePos))  {
				Vector2 v2 = getIndexOfSlot(slot);
				Vector2 v = v2 - selectedCell;
				Debug.Log(v);
				if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, v))  {
					if (selectedItemWasInSlot == InventorySlot.None)  {
						t.removeItem(selectedItem,1);
						u.useMinor(MinorType.Loot, false, false);
					}
					characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, v);
					selectedItem = null;
					return;
				}
				else  {
					InventoryItemSlot invSlot = characterSheet.characterSheet.inventory.inventory[Inventory.getIndexForSlot(v2)];
					Item invSlotItem = invSlot.getItem();
					if (invSlotItem != null && characterSheet.characterSheet.inventory.itemCanStackWith(invSlotItem, selectedItem))  {
						if (selectedItemWasInSlot == InventorySlot.None)  {
							t.removeItem(selectedItem,1);
                            u.useMinor(MinorType.Loot, false, false);
						}
						characterSheet.characterSheet.inventory.stackItemWith(invSlotItem,selectedItem);
						selectedItem = null;
						return;
					}
				}
				break;
			}
		}
/*		if (GameGUI.looting && !(mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight))  {
			if (selectedItemWasInSlot!=InventorySlot.None && selectedItem!=null)  {
				while (selectedItem.stackSize() > 1) t.addItem(selectedItem.popStack());
				t.addItem(selectedItem);
                u.useMinor(MinorType.Loot, false, false);
				//		characterSheet.characterSheet.inventory.removeItemFromSlot(getInventorySlotPos(selectedItemWasInSlot));
			}
		}
		else if (selectedItemWasInSlot!=InventorySlot.None)  {
			if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot)))  {
				characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot));
			}
			else  {
				int slot1 = getLinearIndexFromIndex(getIndexOfSlot(selectedItemWasInSlot));
				if (slot1 > -1 && characterSheet.characterSheet.inventory.itemCanStackWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem))  {
					characterSheet.characterSheet.inventory.stackItemWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem);
				}
			}
		}*/
		selectedItem = null;
	}

	
	static Texture2D makeTex( int width, int height, Color col )  {
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )  {
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	
	public static Texture2D makeTexBorder(int width, int height, Color col )  {
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )  {
			//	Debug.Log("it is: " + (i/width));
			if (i/width == 0 || i/width == height-1) pix[i] = Color.black;
			else if (i%width == 0 || i % width == width-1) pix[i] = Color.black;
			else pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	public static Texture[] paperDollTexturesHead;
	public static Texture[] getPaperDollTexturesHead(Character characterSheet)  {
		if (paperDollTexturesHead == null)  {
			paperDollTexturesHead = new Texture[] {Resources.Load<Texture>("Units/Jackie/JackiePaperdollHead")};
		}
		return paperDollTexturesHead;
	}
	
	static Texture2D inventoryBackgroundTexture = null;
	static Texture2D getInventoryBackgroundTexture()  {
		if (inventoryBackgroundTexture == null)  {
			inventoryBackgroundTexture = makeTexBorder((int)inventoryWidth, (int)inventoryHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return inventoryBackgroundTexture;
	}
	static Color inventoryLineColor = Color.white;
	
	static Texture2D inventoryLineTall = null;
	public static Texture2D getInventoryLineTall()  {
		if (inventoryLineTall == null)  {
			inventoryLineTall = makeTex((int)inventoryLineThickness, (int)inventoryCellSize, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineTall;
	}
	
	static Texture2D inventoryLineWide = null;
	public static Texture2D getInventoryLineWide()  {
		if (inventoryLineWide == null)  {
			inventoryLineWide = makeTex((int)inventoryCellSize, (int)inventoryLineThickness, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineWide;
	}
	
	static Texture2D inventoryHoverBackground = null;
	public static Texture2D getInventoryHoverBackground()  {
		if (inventoryHoverBackground == null)  {
			inventoryHoverBackground = makeTex((int)inventoryCellSize,(int)inventoryCellSize, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return inventoryHoverBackground;
	}

	static Texture2D armorHoverBackground = null;
	public static Texture2D getArmorHoverBackground()  {
		if (armorHoverBackground==null)  {
			armorHoverBackground = makeTex((int)inventoryCellSize*2,(int)inventoryCellSize*2, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return armorHoverBackground;
	}

	static Texture2D armorRedBackground = null;
	public static Texture2D getArmorRedBackground()  {
		if (armorRedBackground==null)  {
			armorRedBackground = makeTex((int)inventoryCellSize*2, (int)inventoryCellSize*2, new Color(200.0f/255.0f, 20.0f/255.0f, 20.0f/255.0f, 0.4f));
		}
		return armorRedBackground;
	}
	
	static GUIStyle bagButtonStyle;
	static GUIStyle getBagButtonStyle()  {
		if (bagButtonStyle == null)  {
			bagButtonStyle = new GUIStyle("Button");
			bagButtonStyle.active.background = bagButtonStyle.normal.background = bagButtonStyle.hover.background = Resources.Load<Texture>("UI/bag-button") as Texture2D;
		}
		return bagButtonStyle;
	}

	
	static GUIStyle titleTextStyle = null;
	public static GUIStyle getTitleTextStyle()  {
		if (titleTextStyle == null)  {
			titleTextStyle = new GUIStyle("Label");
			titleTextStyle.normal.textColor = Color.white;
			titleTextStyle.fontSize = 20;
		}
		return titleTextStyle;
	}
	
	static GUIStyle stackStyle = null;
	public static GUIStyle getStackStyle()  {
		if (stackStyle == null)  {
			stackStyle = new GUIStyle("Label");
			stackStyle.normal.textColor = Color.white;
			stackStyle.fontSize = 11;
			stackStyle.alignment = TextAnchor.LowerRight;
			stackStyle.padding = new RectOffset(0, 0, 0, 0);
		}
		return stackStyle;
	}

	public static Rect kMax = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - 8.0f, bannerY + (bannerHeight - bottomSheetHeight) + kLeft + 120.0f, bottomSheetWidth, bottomSheetHeight);
	public static Rect kMin = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - kLeft, bannerY + (bannerHeight - bottomSheetHeight) + kLeft + 10.0f, bottomSheetWidth, bottomSheetHeight);
	public static Rect cMax = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - 4.0f, bannerY + (bannerHeight - bottomSheetHeight) + cLeft + 225.0f, bottomSheetWidth, bottomSheetHeight);
	public static Rect cMin = new Rect(bannerX + (bannerWidth - bottomSheetWidth) - cLeft, bannerY + (bannerHeight - bottomSheetHeight) + cLeft + 10.0f, bottomSheetWidth, bottomSheetHeight);

	public static Rect fullIRect()  {
		return new Rect((Screen.width - inventoryWidth)/2.0f, 0.0f, inventoryWidth, inventoryHeight);
	}
	public static bool containsMouse(Vector2 mousePos)  {
		Rect kRect = new Rect (kMin.x + (kMax.x - kMin.x) * kPerc, kMin.y + (kMax.y - kMin.y) * kPerc, kMin.width + (kMax.width - kMin.width) * kPerc, kMin.height + (kMax.height - kMin.height) * kPerc - 18.0f);
		Rect cRect = new Rect (cMin.x + (cMax.x - cMin.x) * cPerc, cMin.y + (cMax.y - cMin.y) * cPerc, cMin.width + (cMax.width - cMin.width) * cPerc, cMin.height + (cMax.height - cMin.height) * cPerc - 18.0f);
		if (kRect.Contains(mousePos) || cRect.Contains(mousePos)) return true;
//		if (inventoryOpen && fullIRect().Contains(mousePos)) return true;
		if (new Rect(UnitGUI.bannerX, UnitGUI.bannerY, UnitGUI.bannerWidth, UnitGUI.bannerHeight).Contains(mousePos)) return true;
		if (GameGUI.getTabButtonRect(Tab.C).Contains(mousePos) || GameGUI.getTabButtonRect(Tab.V).Contains(mousePos)) return true;
		return false;
	}

	
	const float tabSpeed = 4.0f;
	public static void doTabs()  {
		if (openTab==Tab.C)  {
			cPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else  {
			cPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		if (openTab == Tab.V)  {
			kPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else  {
			kPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		cPerc = Mathf.Clamp01(cPerc);
		kPerc = Mathf.Clamp01(kPerc);
	}
	

}
