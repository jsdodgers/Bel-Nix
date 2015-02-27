using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitGUI {
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
	public static bool inventoryOpen = false;
	
	public static void clickTab(Tab tab) {
		if (tab == Tab.B) {
		/*	if (GameGUI.looting) {
				GameGUI.selectedMinorType = MinorType.None;
				GameGUI.selectMinorType(MinorType.None);
			}
			else*/ inventoryOpen = !inventoryOpen;
			BattleGUI.setInventoryShown(inventoryOpen);
			return;
		}
		if (openTab==tab) openTab = Tab.None;
		else openTab = tab;
	}
	
	static GUIStyle courierStyle;
	public static GUIStyle getCourierStyle(int size) {
		if (courierStyle == null) {
			courierStyle = new GUIStyle("Label");
			courierStyle.font = Resources.Load<Font>("Fonts/Courier New");
			courierStyle.normal.textColor = Color.black;
		}
		courierStyle.fontSize = size;
		return courierStyle;
	}
	
	public static string getSmallCapsString(string original, int size) {
		string newS = "";
		char[] chars = original.ToCharArray();
		bool inLowerCase = false;
		foreach (char c in chars) {
			if (c >= 'a' && c <= 'z') {
				if (!inLowerCase) {
					newS += "<size=" + size + ">";
					inLowerCase = true;
				}
				int i = (int)c;
				i -= (int)'a';
				i += (int)'A';
				newS += ((char)i);
			}
			else {
				if (c == '\'' || c == '-' || c=='.' || c==',' || c=='+' || (c>='0' && c<='9')) {
					if (!inLowerCase) {
						newS += "<size=" + size + ">";
						inLowerCase = true;
					}
				}
				else if (inLowerCase) {
					newS += "</size>";
					inLowerCase = false;
				}
				newS += c;
			}
		}
		if (inLowerCase) newS += "</size>";
		return newS;
	}


	public static Vector2 getInventorySlotPos(InventorySlot slot) {
		switch (slot) {
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

	
	public static Rect getInventorySlotRect(InventorySlot slot) {
		Vector2 v = getInventorySlotPos(slot);
		switch (slot) {
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
	
	public static InventorySlot[] armorSlots = new InventorySlot[]{InventorySlot.Head,InventorySlot.Shoulder,InventorySlot.Back,InventorySlot.Chest,InventorySlot.Glove,InventorySlot.RightHand,InventorySlot.LeftHand,InventorySlot.Pants,InventorySlot.Boots};
	public static InventorySlot[] inventorySlots = new InventorySlot[]{InventorySlot.Zero, InventorySlot.One,InventorySlot.Two,InventorySlot.Three,InventorySlot.Four,InventorySlot.Five,InventorySlot.Six,InventorySlot.Seven,InventorySlot.Eight,InventorySlot.Nine,InventorySlot.Ten,InventorySlot.Eleven, InventorySlot.Twelve, InventorySlot.Thirteen, InventorySlot.Fourteen, InventorySlot.Fifteen};
	public static InventorySlot[] trapTurretSlots = new InventorySlot[]{InventorySlot.Frame, InventorySlot.Applicator, InventorySlot.Gear, InventorySlot.TriggerEnergySource};
	public static InventorySlot  getInventorySlotFromIndex(Vector2 index) {
		//		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return InventorySlot.None;
		//		int ind = (int)index.x + ((int)index.y)*4;
		int ind = getLinearIndexFromIndex(index);
		if (ind==-1) return InventorySlot.None;
		return inventorySlots[ind];
	}
	public static int getLinearIndexFromIndex(Vector2 index) {
		if (index.x <0 || index.y < 0 || index.x >3 || index.y >3) return -1;
		return (int)index.x + ((int)index.y)*4;
	}
	public static Vector2 getIndexFromLinearIndex(int index) {
		if (index <0 || index > 15) return new Vector2(-1, -1);
		return new Vector2(index%4,index/4);
	}
	public static Vector2 getIndexOfSlot(InventorySlot slot) {
		switch (slot) {
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
	public static void selectItem(Character characterSheet) {
		selectItem(characterSheet, null, null);
	}
	public static void selectItem(Character characterSheet, MapGenerator mapGenerator, Unit u) {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		foreach (InventorySlot slot in inventorySlots) {
			Rect r = UnitGUI.getInventorySlotRect(slot);
			if (r.Contains(mousePos)) {
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
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
					if (selectedItem.stackSize()>1) {
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
		if (!GameGUI.looting || mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight) return;
		Vector2 scrollOff = UnitGUI.groundScrollPosition;
		float div = 20.0f;
		float y = div + UnitGUI.groundY - scrollOff.y;
		float mid = UnitGUI.groundX + UnitGUI.groundWidth/2.0f + scrollOff.x;
		//	mousePos.y += groundScrollPosition.y;
		selectedItem = null;
		if (mapGenerator != null) {
		List<Item> groundItems = mapGenerator.tiles[(int)u.position.x,(int)-u.position.y].getReachableItems();
		foreach (Item i in groundItems) {
			if (i.inventoryTexture==null) continue;
			//	Debug.Log(mousePos.x + "  " + mousePos.y + "       " + mid + "  " + y);
			Vector2 size = i.getSize();
			float x = mid - size.x*inventoryCellSize/2.0f;
			Rect r = new Rect(x, y, size.x*inventoryCellSize, size.y*UnitGUI.inventoryCellSize);
			if (r.Contains(mousePos)) {
				//	Debug.Log(i);
				selectedCell = new Vector2((int)((mousePos.x - x)/inventoryCellSize), (int)((mousePos.y - y)/inventoryCellSize));
				foreach (Vector2 cell in i.getShape()) {
					if (cell.x == selectedCell.x && cell.y == selectedCell.y) {
						selectedItemPos = new Vector2(x, y);
						selectedMousePos = mousePos;
						selectedItem = i;
						selectedItemWasInSlot = InventorySlot.None;
					}
				}
				Debug.Log(selectedCell);
				if (selectedItem!=null) {
					break;
				}
			}
			y += size.y*UnitGUI.inventoryCellSize + div;
		}
		}
	}
	public static void deselectItem(Character characterSheet) {
		deselectItem(characterSheet, null, null);
	}

	public static void deselectItem(Character characterSheet, MapGenerator mapGenerator, Unit u) {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		Tile t = (mapGenerator != null && u!=null ? mapGenerator.tiles[(int)u.position.x,(int)-u.position.y] : null);
		foreach (InventorySlot slot in inventorySlots) {
			Rect r = UnitGUI.getInventorySlotRect(slot);
			if (r.Contains(mousePos)) {
				Vector2 v2 = getIndexOfSlot(slot);
				Vector2 v = v2 - selectedCell;
				Debug.Log(v);
				if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, v)) {
					if (selectedItemWasInSlot == InventorySlot.None) {
						t.removeItem(selectedItem,1);
						u.useMinor(false, false);
					}
					characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, v);
					selectedItem = null;
					return;
				}
				else {
					InventoryItemSlot invSlot = characterSheet.characterSheet.inventory.inventory[Inventory.getIndexForSlot(v2)];
					Item invSlotItem = invSlot.getItem();
					if (invSlotItem != null && characterSheet.characterSheet.inventory.itemCanStackWith(invSlotItem, selectedItem)) {
						if (selectedItemWasInSlot == InventorySlot.None) {
							t.removeItem(selectedItem,1);
							u.useMinor(false, false);
						}
						characterSheet.characterSheet.inventory.stackItemWith(invSlotItem,selectedItem);
						selectedItem = null;
						return;
					}
				}
				break;
			}
		}
		if (GameGUI.looting && !(mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight)) {
			if (selectedItemWasInSlot!=InventorySlot.None && selectedItem!=null) {
				while (selectedItem.stackSize() > 1) t.addItem(selectedItem.popStack());
				t.addItem(selectedItem);
				u.useMinor(false, false);
				//		characterSheet.characterSheet.inventory.removeItemFromSlot(getInventorySlotPos(selectedItemWasInSlot));
			}
		}
		else if (selectedItemWasInSlot!=InventorySlot.None) {
			if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot))) {
				characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, getIndexOfSlot(selectedItemWasInSlot));
			}
			else {
				int slot1 = getLinearIndexFromIndex(getIndexOfSlot(selectedItemWasInSlot));
				if (slot1 > -1 && characterSheet.characterSheet.inventory.itemCanStackWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem)) {
					characterSheet.characterSheet.inventory.stackItemWith(characterSheet.characterSheet.inventory.inventory[slot1].getItem(),selectedItem);
				}
			}
		}
		selectedItem = null;
	}

	
	static Texture2D makeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	
	public static Texture2D makeTexBorder(int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
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
	public static Texture[] getPaperDollTexturesHead(Character characterSheet) {
		if (paperDollTexturesHead == null) {
			paperDollTexturesHead = new Texture[]{Resources.Load<Texture>("Units/Jackie/JackiePaperdollHead")};
		}
		return paperDollTexturesHead;
	}
	
	static Texture2D inventoryBackgroundTexture = null;
	static Texture2D getInventoryBackgroundTexture() {
		if (inventoryBackgroundTexture == null) {
			inventoryBackgroundTexture = makeTexBorder((int)inventoryWidth, (int)inventoryHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return inventoryBackgroundTexture;
	}
	static Color inventoryLineColor = Color.white;
	
	static Texture2D inventoryLineTall = null;
	public static Texture2D getInventoryLineTall() {
		if (inventoryLineTall == null) {
			inventoryLineTall = makeTex((int)inventoryLineThickness, (int)inventoryCellSize, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineTall;
	}
	
	static Texture2D inventoryLineWide = null;
	public static Texture2D getInventoryLineWide() {
		if (inventoryLineWide == null) {
			inventoryLineWide = makeTex((int)inventoryCellSize, (int)inventoryLineThickness, inventoryLineColor);//new Color(22.0f/255.0f, 44.0f/255.0f, 116.0f/255.0f));
		}
		return inventoryLineWide;
	}
	
	static Texture2D inventoryHoverBackground = null;
	public static Texture2D getInventoryHoverBackground() {
		if (inventoryHoverBackground == null) {
			inventoryHoverBackground = makeTex((int)inventoryCellSize,(int)inventoryCellSize, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return inventoryHoverBackground;
	}

	static Texture2D armorHoverBackground = null;
	public static Texture2D getArmorHoverBackground() {
		if (armorHoverBackground==null) {
			armorHoverBackground = makeTex((int)inventoryCellSize*2,(int)inventoryCellSize*2, new Color(80.0f/255.0f, 44.0f/255.0f, 120.0f/255.0f, 0.4f));
		}
		return armorHoverBackground;
	}

	static Texture2D armorRedBackground = null;
	public static Texture2D getArmorRedBackground() {
		if (armorRedBackground==null) {
			armorRedBackground = makeTex((int)inventoryCellSize*2, (int)inventoryCellSize*2, new Color(200.0f/255.0f, 20.0f/255.0f, 20.0f/255.0f, 0.4f));
		}
		return armorRedBackground;
	}
	
	static GUIStyle bagButtonStyle;
	static GUIStyle getBagButtonStyle() {
		if (bagButtonStyle == null) {
			bagButtonStyle = new GUIStyle("Button");
			bagButtonStyle.active.background = bagButtonStyle.normal.background = bagButtonStyle.hover.background = Resources.Load<Texture>("UI/bag-button") as Texture2D;
		}
		return bagButtonStyle;
	}

	
	static GUIStyle titleTextStyle = null;
	public static GUIStyle getTitleTextStyle() {
		if (titleTextStyle == null) {
			titleTextStyle = new GUIStyle("Label");
			titleTextStyle.normal.textColor = Color.white;
			titleTextStyle.fontSize = 20;
		}
		return titleTextStyle;
	}
	
	static GUIStyle stackStyle = null;
	public static GUIStyle getStackStyle() {
		if (stackStyle == null) {
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

	public static Rect fullIRect() {
		return new Rect((Screen.width - inventoryWidth)/2.0f, 0.0f, inventoryWidth, inventoryHeight);
	}
	public static bool containsMouse(Vector2 mousePos) {
		Rect kRect = new Rect (kMin.x + (kMax.x - kMin.x) * kPerc, kMin.y + (kMax.y - kMin.y) * kPerc, kMin.width + (kMax.width - kMin.width) * kPerc, kMin.height + (kMax.height - kMin.height) * kPerc - 18.0f);
		Rect cRect = new Rect (cMin.x + (cMax.x - cMin.x) * cPerc, cMin.y + (cMax.y - cMin.y) * cPerc, cMin.width + (cMax.width - cMin.width) * cPerc, cMin.height + (cMax.height - cMin.height) * cPerc - 18.0f);
		if (kRect.Contains(mousePos) || cRect.Contains(mousePos)) return true;
		if (inventoryOpen && fullIRect().Contains(mousePos)) return true;
		if (new Rect(UnitGUI.bannerX, UnitGUI.bannerY, UnitGUI.bannerWidth, UnitGUI.bannerHeight).Contains(mousePos)) return true;
		if (GameGUI.getTabButtonRect(Tab.C).Contains(mousePos) || GameGUI.getTabButtonRect(Tab.V).Contains(mousePos)) return true;
		return false;
	}

	public static void drawGUI(Character characterSheet, MapGenerator mapGenerator, Unit u) {
	/*	Item selectedItem = null;
		Vector2 selectedCell = new Vector2();
		Vector2 selectedItemPos = new Vector2();
		Vector2 selectedMousePos = new Vector2();*/
		Vector3 position = new Vector3();
		if (u != null) {
			position = u.position;
		}
		if (playerBannerTexture==null) {
			playerBannerTexture = Resources.Load<Texture>("UI/bottom-sheet") as Texture2D;
			bottomSheetTexture = Resources.Load<Texture>("UI/bottom-sheet-long") as Texture2D;
			portraitBorderTexture = Resources.Load<Texture>("UI/portrait-border") as Texture2D;
		}
		if (GUI.Button(GameGUI.getTabButtonRect(Tab.C), "C", GameGUI.getTabButtonRightStyle())) {
			Debug.Log("Click C - " + openTab);
			clickTab(Tab.C);
		}
		if (GUI.Button(GameGUI.getTabButtonRect(Tab.V), "V", GameGUI.getTabButtonRightStyle())) {
			Debug.Log("Click V");
			clickTab(Tab.V);
		}
		string sizeString = "<size=10>";
		string sizeString12 = "<size=12>";
		string sizeString14 = "<size=14>";
		string sizeEnd = "</size>";
		string divString = "<size=6>\n\n</size>";
		string otherDivString = "<size=3>\n\n</size>";
		string divString2 = "<size=5>\n\n</size>";
		
		float y = 0;
		float x = 0;
		Rect kRect = new Rect (kMin.x + (kMax.x - kMin.x) * kPerc, kMin.y + (kMax.y - kMin.y) * kPerc, kMin.width + (kMax.width - kMin.width) * kPerc, kMin.height + (kMax.height - kMin.height) * kPerc);
		Rect cRect = new Rect (cMin.x + (cMax.x - cMin.x) * cPerc, cMin.y + (cMax.y - cMin.y) * cPerc, cMin.width + (cMax.width - cMin.width) * cPerc, cMin.height + (cMax.height - cMin.height) * cPerc);
		GUI.DrawTexture(kRect, bottomSheetTexture);
		
		y = kRect.y + 370.0f;
		x = 10.0f;
		GUIStyle statsStyle = getCourierStyle(18);
		string info = "L" + sizeString12 + "EVEL" + sizeEnd + ":" + sizeString14 + characterSheet.characterSheet.characterProgress.getCharacterLevel() + sizeEnd +
			"\n" + "E" + sizeString12 + "XPERIENCE" + sizeEnd + ":" + sizeString14 + characterSheet.characterSheet.characterProgress.getCharacterExperience() + "/" + (characterSheet.characterSheet.characterProgress.getCharacterLevel()*100) + sizeEnd +
				"\n" + getSmallCapsString(characterSheet.characterSheet.characterProgress.getCharacterClass().getClassName().ToString(), 12) +
				"\n" + getSmallCapsString(characterSheet.characterSheet.personalInformation.getCharacterRace().getRaceString(), 12) +
				"\n" + getSmallCapsString(characterSheet.characterSheet.personalInformation.getCharacterBackground().ToString(), 12);
		GUIContent infoContent = new GUIContent(info);
		Vector2 infoSize = statsStyle.CalcSize(infoContent);
		GUI.Label(new Rect(x, y, infoSize.x, infoSize.y), infoContent, statsStyle);
		x += infoSize.x + 15.0f;
		y -= 10.0f;
		
		GUIStyle statsTitleStyle = getCourierStyle(20);
		string classFeaturesTitleString = "C<size=15>LASS</size> F<size=15>EATURES</size>";
		GUIContent classFeaturesTitleContent = new GUIContent(classFeaturesTitleString);
		Vector2 classFeaturesTitleSize = statsTitleStyle.CalcSize(classFeaturesTitleContent);
		GUI.Label(new Rect(x + (kMax.x + kMax.width - x - classFeaturesTitleSize.x)/2.0f, y, classFeaturesTitleSize.x, classFeaturesTitleSize.y), classFeaturesTitleContent, statsTitleStyle);
		y += classFeaturesTitleSize.y + 5.0f;
		statsStyle = getCourierStyle(18);
		
		
		ClassFeature[] classFeatures = characterSheet.characterSheet.characterProgress.getClassFeatures();
		string classFeaturesString = "";
		foreach (ClassFeature classFeature in classFeatures) {
			if (classFeaturesString != "") classFeaturesString += "\n";
			classFeaturesString += getSmallCapsString(ClassFeatures.getName(classFeature), 12);
		}
		GUIContent featuresContent = new GUIContent(classFeaturesString);
		Vector2 featuresSize = statsStyle.CalcSize(featuresContent);
		classFeaturesScrollPos = GUI.BeginScrollView(new Rect(x, y, (kRect.x + kRect.width - x - 11), kRect.y + kRect.height - y - 30), classFeaturesScrollPos, new Rect(x, y, (kRect.x + kRect.width - x - 11) - 16.0f, featuresSize.y));
		GUI.Label(new Rect(x, y, featuresSize.x, featuresSize.y), featuresContent, statsStyle);
		GUI.EndScrollView();
		
		
		GUI.DrawTexture(cRect, bottomSheetTexture);
		
		y = cRect.y + 265.0f;
		x = 10.0f;
		statsTitleStyle = getCourierStyle(20);
		string characterStatsString = "C<size=15>HARACTER STATS</size>";
		GUIContent characterStatsContent = new GUIContent(characterStatsString);
		Vector2 characterStatsSize = statsTitleStyle.CalcSize(characterStatsContent);
		GUI.Label(new Rect((cRect.x + cRect.width)/2.0f - characterStatsSize.x/2.0f, y, characterStatsSize.x, characterStatsSize.y), characterStatsContent, statsTitleStyle);
		y += characterStatsSize.y + 5.0f;
		statsStyle = getCourierStyle(15);
		string typesString = otherDivString + "P" + sizeString + "HYSIQUE" + sizeEnd + "\n" + otherDivString +
			divString2 + otherDivString + "P" + sizeString + "ROWESS" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "M" + sizeString + "ASTERY" + sizeEnd + "\n" + otherDivString +
				divString2 + otherDivString + "K" + sizeString + "NOWLEDGE" + sizeEnd + otherDivString;
		GUIContent typesContent = new GUIContent(typesString);
		Vector2 typesSize = statsStyle.CalcSize(typesContent);
		GUI.Label(new Rect(x, y, typesSize.x, typesSize.y), typesContent, statsStyle);
		x += typesSize.x + 20.0f;
		
		string statsString = "S" + sizeString + "TURDY" + sizeEnd + "\n" + characterSheet.characterSheet.abilityScores.getSturdy() + " (<size=13>MOD:" + characterSheet.characterSheet.combatScores.getInitiative() + "</size>)" +
			divString + "P" + sizeString + "ERCEPTION" + sizeEnd + "\n" + characterSheet.characterSheet.abilityScores.getPerception(0) + " (<size=13>MOD:" + characterSheet.characterSheet.combatScores.getCritical(false) + "</size>)" +
				divString + "T" + sizeString + "ECHNIQUE" + sizeEnd + "\n" + characterSheet.characterSheet.abilityScores.getTechnique() + " (<size=13>MOD:" + characterSheet.characterSheet.combatScores.getHandling() + "</size>)" +
				divString + "W" + sizeString + "ELL-VERSED" + sizeEnd + "\n" + characterSheet.characterSheet.abilityScores.getWellVersed() + " (<size=13>MOD:" + characterSheet.characterSheet.combatScores.getDominion() + "</size>)";
		GUIContent statsContent = new GUIContent(statsString);
		Vector2 statsSize = statsStyle.CalcSize(statsContent);
		GUI.Label(new Rect(x, y, statsSize.x, statsSize.y), statsContent, statsStyle);
		x += statsSize.x + 20.0f;
		
		string skillNamesString = "A" + sizeString + "THLETICS" + sizeEnd + ":\nM" + sizeString + "ELEE" + sizeEnd + ":" + 
			divString + "R" + sizeString + "ANGED" + sizeEnd + ":\nS" + sizeString + "TEALTH" + sizeEnd + ":" +
				divString + "M" + sizeString + "ECHANICAL" + sizeEnd + ":\nM" + sizeString + "EDICINAL" + sizeEnd + ":" +
				divString + "H" + sizeString + "ISTORICAL" + sizeEnd + ":\nP" + sizeString + "OLITICAL" + sizeEnd + ":";
		GUIContent skillNamesContent = new GUIContent(skillNamesString);
		Vector2 skillNamesSize = statsStyle.CalcSize(skillNamesContent);
		GUI.Label(new Rect(x, y, skillNamesSize.x, skillNamesSize.y), skillNamesContent, statsStyle);
		string skillStatsString = characterSheet.characterSheet.skillScores.getScore(Skill.Athletics) + "\n" + characterSheet.characterSheet.skillScores.getScore(Skill.Melee) + divString +
			characterSheet.characterSheet.skillScores.getScore(Skill.Ranged) + "\n" + characterSheet.characterSheet.skillScores.getScore(Skill.Stealth) + divString +
				characterSheet.characterSheet.skillScores.getScore(Skill.Mechanical)  + "\n" + characterSheet.characterSheet.skillScores.getScore(Skill.Medicinal) + divString +
				characterSheet.characterSheet.skillScores.getScore(Skill.Historical)  + "\n" + characterSheet.characterSheet.skillScores.getScore(Skill.Political);
		GUIContent skillStatsContent = new GUIContent(skillStatsString);
		Vector2 skillStatsSize = statsStyle.CalcSize(skillStatsContent);
		x += skillNamesSize.x + 10.0f;
		GUI.Label(new Rect(x, y, skillStatsSize.x, skillStatsSize.y), skillStatsContent, statsStyle);
		y += skillStatsSize.y + 5.0f;
		string armorClass = "A" + sizeString + "RMOR" + sizeEnd + " C" + sizeString + "LASS" + sizeEnd + ": " + characterSheet.characterSheet.characterLoadout.getAC();
		GUIContent armorClassContent = new GUIContent(armorClass);
		Vector2 armorClassSize = statsStyle.CalcSize(armorClassContent);
		GUI.Label(new Rect((cMax.x + cMax.width - armorClassSize.x)/2.0f, y, armorClassSize.x, armorClassSize.y), armorClassContent, statsStyle);
		
		
		float tabButtonsY = 0.0f;
		string playerText = "N<size=13>AME</size>/A<size=13>LIAS</size>:\n\"";
		string playerName = characterSheet.characterSheet.personalInformation.getCharacterName().fullName();
		playerText += getSmallCapsString(playerName, 13);
		
		playerText += "\"\n";
		playerText += "H<size=13>EALTH</size>:\n" + characterSheet.characterSheet.combatScores.getCurrentHealth() + "/" + characterSheet.characterSheet.combatScores.getMaxHealth() + "\n";
		playerText += "C<size=13>OMPOSURE</size>:\n" + characterSheet.characterSheet.combatScores.getCurrentComposure() + "/" + characterSheet.characterSheet.combatScores.getMaxComposure() + "\n";
		
		GUIContent playerContent = new GUIContent(playerText);
		Texture[] textures = getPaperDollTexturesHead(characterSheet);
		GUI.DrawTexture(new Rect(bannerX, bannerY, bannerWidth, bannerHeight), playerBannerTexture);
		foreach (Texture2D texture in textures) {
			GUI.DrawTexture(new Rect(24.0f, ((bannerHeight + bannerY) - paperDollHeadSize)/2.0f - 1.0f, paperDollHeadSize, paperDollHeadSize), texture);
		}
		GUI.DrawTexture(new Rect(15.0f, ((bannerHeight + bannerY) - portraitBorderSize)/2.0f, portraitBorderSize, portraitBorderSize), portraitBorderTexture);
		
		tabButtonsY = bannerHeight + bannerY;
		
		GUIStyle cStyle = getCourierStyle(20);
		Vector2 playerTextSize = cStyle.CalcSize(playerContent);
		float playerTextY = ((bannerHeight + bannerY) - playerTextSize.y)/2.0f;
		float playerTextX = 45.0f + paperDollHeadSize;
		GUI.Label(new Rect(playerTextX, playerTextY, playerTextSize.x, playerTextSize.y), playerContent, cStyle);
		if (GUI.Button(new Rect(bannerX + bannerWidth - 25.0f - bagButtonSize, bannerY + bannerHeight - bagButtonSize - 15.0f, bagButtonSize, bagButtonSize), "", getBagButtonStyle())) {
			clickTab(Tab.B);
		}

		if (inventoryOpen) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			GUI.DrawTexture(fullIRect(), getInventoryBackgroundTexture());
			GUIStyle titleStyle = getTitleTextStyle();
			GUIContent armour = new GUIContent("Armor");
			Vector2 armourSize = titleStyle.CalcSize(armour);
			GUI.Label(new Rect(fullIRect().x - 1.0f + inventoryWidth/3.0f - armourSize.x/2.0f, 0.0f, armourSize.x, armourSize.y), armour, titleStyle);
			GUIContent inventory = new GUIContent("Inventory");
			Vector2 inventorySize = titleStyle.CalcSize(inventory);
			GUI.Label(new Rect(fullIRect().x - 1.0f + inventoryWidth*2.0f/3.0f - inventorySize.x/2.0f, 0.0f, inventorySize.x, inventorySize.y), inventory, titleStyle);
			
			//			foreach (CharacterInfo.InventoryItemSlot slot in characterSheet.characterSheet.inventory.inventory) {
			
			//			}
			
			foreach (InventorySlot slot in inventorySlots) {
				Rect r = getInventorySlotRect(slot);
				if (r.Contains(mousePos)) {
					GUI.DrawTexture(r, getInventoryHoverBackground());
					if (selectedItem!=null) {
						Vector2 startPos = getIndexOfSlot(slot);
						foreach(Vector2 cell in selectedItem.getShape()) {
							Vector2 pos = startPos;
							pos.x += cell.x - selectedCell.x;
							pos.y += cell.y - selectedCell.y;
							if (pos.x == startPos.x && pos.y == startPos.y) continue;
							//	Debug.Log(startPos + "   " + pos);
							InventorySlot newSlot = getInventorySlotFromIndex(pos);
							if (newSlot != InventorySlot.None) {
								Rect r2 = getInventorySlotRect(newSlot);
								GUI.DrawTexture(r2, getInventoryHoverBackground());
							}
						}
					}
					break;
				}
			}
			foreach (InventorySlot slot in armorSlots) {
				Rect r = getInventorySlotRect(slot);
				if (r.Contains(mousePos)) {
					GUI.DrawTexture(r, getArmorHoverBackground());
					break;
				}
			}
			foreach (InventorySlot slot in armorSlots) {
				Rect r = getInventorySlotRect(slot);
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y+ inventoryCellSize,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				Item i = characterSheet.characterSheet.characterLoadout.getItemInSlot(slot);
				if (i != null && i.inventoryTexture != null) {
					float w = i.getSize().x*inventoryCellSize;
					float h = i.getSize().y*inventoryCellSize;
					x = r.x;
					y = r.y;
					if (h > r.height) y = r.y + r.height - h;
					else y = r.y + (r.height - h)/2.0f;
					x = r.x + (r.width - w)/2.0f;
//					GUI.DrawTexture(new Rect(x, y, w, h), i.inventoryTexture);
				}
			}
			foreach (InventorySlot slot in inventorySlots) {
				Rect r = getInventorySlotRect(slot);
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				GUI.DrawTexture(new Rect(r.x + inventoryCellSize - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),getInventoryLineTall());
				
				GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
				GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),getInventoryLineWide());
			}
			GUIStyle stackSt = getStackStyle();
			foreach (InventorySlot slot in inventorySlots) {
				Vector2 vec = getIndexOfSlot(slot);
				int ind = getLinearIndexFromIndex(vec);
				InventoryItemSlot isl = characterSheet.characterSheet.inventory.inventory[ind];
				Item i = isl.item;
				if (i == null) continue;
				Vector2 origin = getInventorySlotPos(slot);
				Vector2 size = i.getSize();
//				GUI.DrawTexture(new Rect(origin.x,origin.y, size.x*inventoryCellSize,size.y*inventoryCellSize),i.inventoryTexture);
				if (i.stackSize()>1) {
					Vector2 bottomRight = i.getBottomRightCell();
					bottomRight.x *= inventoryCellSize - inventoryLineThickness;
					bottomRight.y *= inventoryCellSize - inventoryLineThickness;
					Vector2 stackPos = origin + bottomRight;
					GUIContent content = new GUIContent("" + i.stackSize());
					GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
				}
			}
			if (mapGenerator != null) {
				List<Item> groundItems = mapGenerator.tiles[(int)position.x,(int)-position.y].getReachableItems();
				//	Debug.Log("ground Items: " + groundItems.Count + "   " + groundItems);
				float div = 20.0f;
				float height = div;
				foreach (Item i in groundItems) {
					if (i.inventoryTexture==null) continue;
					height += i.getSize().y*inventoryCellSize + div;
				}
				groundScrollPosition = GUI.BeginScrollView(new Rect(groundX, groundY, groundWidth, groundHeight), groundScrollPosition, new Rect(groundX, groundY, groundWidth-20.0f, height));
				y = div + groundY;
				float mid = groundX + groundWidth/2.0f;
				foreach (Item i in groundItems) {
					if (i.inventoryTexture==null) continue;
					Vector2 size = i.getSize();
					if (i!=selectedItem) {
//						GUI.DrawTexture(new Rect(mid - size.x*inventoryCellSize/2.0f, y, size.x*inventoryCellSize, size.y*inventoryCellSize), i.inventoryTexture);
					}
					y += size.y*inventoryCellSize + div;
				}
				GUI.EndScrollView();
			}
			if (selectedItem != null) {
				Vector2 size = selectedItem.getSize();
				Vector2 pos = selectedItemPos;
				pos.y += (mousePos.y - selectedMousePos.y);
				pos.x += (mousePos.x - selectedMousePos.x);
//				GUI.DrawTexture(new Rect(pos.x, pos.y,size.x*inventoryCellSize, size.y*inventoryCellSize), selectedItem.inventoryTexture);
				if (selectedItem.stackSize()>1) {
					Vector2 bottomRight = selectedItem.getBottomRightCell();
					bottomRight.x *= inventoryCellSize - inventoryLineThickness;
					bottomRight.y *= inventoryCellSize - inventoryLineThickness;
					Vector2 stackPos = pos + bottomRight;
					GUIContent content = new GUIContent("" + selectedItem.stackSize());
					GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
				}
			}
			
		}
	}

	
	const float tabSpeed = 4.0f;
	public static void doTabs() {
		if (openTab==Tab.C) {
			cPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else {
			cPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		if (openTab == Tab.V) {
			kPerc += Time.deltaTime * Time.timeScale * tabSpeed;
		}
		else {
			kPerc -= Time.deltaTime * Time.timeScale * tabSpeed;
		}
		cPerc = Mathf.Clamp01(cPerc);
		kPerc = Mathf.Clamp01(kPerc);
	}
	

}
