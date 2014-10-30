using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public MapGenerator mapGenerator;
	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;
	
	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}

	public void damage(int damage) {
		Debug.Log("Damage");
		hitPoints -= damage;
		if (hitPoints <= 0) died = true;
		Debug.Log("EndDamage");
	}

	void doDeath() {
		Debug.Log("Do Death");
	//	mapGenerator.
		if (died) dieTime += Time.deltaTime;
	//	if (dieTime >= 1) Destroy(gameObject);
		if (dieTime >= 0.5f) {
			if (!mapGenerator.selectedPlayer || !mapGenerator.selectedPlayer.GetComponent<Player>().attacking) {
				if (mapGenerator.selectedPlayer) {
					Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
					if (p.attackEnemy==this) p.attackEnemy = null;
				}
				mapGenerator.enemies.Remove(gameObject);
				Destroy(gameObject);
			}
		}
		Debug.Log("End Death");
	}

	// Use this for initialization
	void Start () {
		hitPoints = maxHitPoints;
	}
	
	// Update is called once per frame
	void Update () {
		doDeath();
	}
}
