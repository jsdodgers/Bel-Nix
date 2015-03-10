using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackMarketItemContainer : MonoBehaviour {


	public BlackMarketItem item = null;
	public Text itemText;
	public Image itemImage;
	public Text priceText;
	public Button buyButton;
	BaseManager bm = null;
	public bool selling = false;

	public void buyItem() {
		if (bm != null) {
			if (selling) {
				bm.sellItem(item.item);
			}
			else {
				bm.buyItem(item.editorItem.getItem());
			}
		}
	}
	public void setUp(BlackMarketItem item, BaseManager bm, bool selling) {
		this.selling = selling;
		this.bm = bm;
		this.item = item;
		itemText.text = UnitGUI.getSmallCapsString(item.item.getBlackMarketText(), 12);
		priceText.text = (selling ? item.item.getBlackMarketSellPriceText() :item.item.getBlackMarketPriceText());
		itemImage.sprite = item.item.inventoryTexture;
		Vector2 size = item.item.getSize() * 32.0f;
		if (size.x > 90 || size.y > 90) size/=2.0f;
		itemImage.GetComponent<RectTransform>().sizeDelta = size;
		setCanAfford();
	}

	public void setCanAfford() {

		int cost = item.item.copper + item.item.silver * 100 + item.item.gold * 10000;
		buyButton.interactable = selling || bm.stash.canAfford(cost);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
