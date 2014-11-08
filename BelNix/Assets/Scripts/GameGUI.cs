using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public MapGenerator mapGenerator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Rect moveButtonRect() {
		float moveWidth = 90.0f;
		float moveHeight = 40.0f;
		float moveX = 10.0f;
		float moveY = Screen.height - moveHeight - 10.0f;
		return new Rect(moveX, moveY, moveWidth, moveHeight);
	}

	public Rect attackButtonRect() {
		Rect r = moveButtonRect();
		if (mapGenerator != null) {
			if (mapGenerator.selectedCharacter!=null && !mapGenerator.selectedCharacter.moving) {
				if (mapGenerator.lastPlayerPath.Count >1) {
					r.y -= r.height + 10.0f;
				}
			}
		}
		return r;
	}

	public bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (mapGenerator) {
			if (mapGenerator.selectedCharacter!=null) {
			//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
				if (mapGenerator.lastPlayerPath.Count >1 && !mapGenerator.selectedCharacter.moving) {
					if (moveButtonRect().Contains(mousePos)) {
						return true;
					}
				}
				if (mapGenerator.selectedCharacter.attackEnemy!=null && !mapGenerator.selectedCharacter.moving && !mapGenerator.selectedCharacter.attacking) {
					if (attackButtonRect().Contains(mousePos)) {
						return true;
					}
				}
			}
		}
		return false;
	}

	void OnGUI() {
	//	Debug.Log("OnGUI");
		if (mapGenerator == null) return;

		bool path = false;
		if (mapGenerator.selectedCharacter!=null) {
		//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
			CharacterScript p = mapGenerator.selectedCharacter;
			if (mapGenerator.lastPlayerPath.Count >1 && !p.moving) {
				path = true;
				if(GUI.Button(moveButtonRect(), "Move")) {
					Debug.Log("Move Player!");
					p.moving = true;
//					p.rotating = true;
					p.setRotatingPath();
			//		p.attacking = true;
				}
			}
			if (p.attackEnemy!=null && !p.moving && !p.attacking) {
				if (GUI.Button(attackButtonRect(), "Attack")) {
					if (path) {
						p.moving = true;
						p.setRotatingPath();
					}
					else {
					//	p.setRotationFrom(new Vector2(p.position.x + .001f, p.position.y), new Vector2(p.attackEnemy.position.x, p.attackEnemy.position.y));
						p.setRotationToAttackEnemy();
					}
					p.attacking = true;
				}
			}
		}
	//	Debug.Log("OnGUIEnd");
	}

}
