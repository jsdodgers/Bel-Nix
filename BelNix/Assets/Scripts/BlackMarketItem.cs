using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackMarketItem : MonoBehaviour {


	public EditorItem editorItem = null;
	public Item item = null;
	public Text itemText;
	public Image itemImage;
	public Text priceText;
	BaseManager bm = null;

	public void buyItem() {
		if (bm != null) {
			bm.buyItem(editorItem.getItem());
		}
	}
	public void setUp(EditorItem editorItem, Item item, BaseManager bm) {
		this.bm = bm;
		this.item = item;
		this.editorItem = editorItem;
		itemText.text = item.getBlackMarketText();
		priceText.text = item.getBlackMarketPriceText();
		itemImage.sprite = item.inventoryTexture;
		Vector2 size = item.getSize() * 32.0f;
		if (size.x > 90 || size.y > 90) size/=2.0f;
		itemImage.GetComponent<RectTransform>().sizeDelta = size;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
