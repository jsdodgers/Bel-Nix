using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public MapGenerator mapGenerator;
	public Vector3 position;
	public int maxHitPoints = 10;
	public int hitPoints;
	public bool died = false;
	public float dieTime = 0;

	
	public bool moving = false;
	public bool rotating = false;
	public bool attacking = false;
	public Player attackPlayer = null;
	public Vector2 rotateFrom;
	public Vector2 rotateTo;


	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}

	public void damage(int damage) {
	//	Debug.Log("Damage");
		hitPoints -= damage;
		if (hitPoints <= 0) died = true;
	//	Debug.Log("EndDamage");
	}

	void doDeath() {
		Debug.Log("Do Death");
	//	mapGenerator.
		if (died) dieTime += Time.deltaTime;
	//	if (dieTime >= 1) Destroy(gameObject);
	//	if (dieTime >= 0.5f) {
		if (died) {
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

	void doRotation() {
		
		if (rotating) {
			float speed = 180.0f;// + 20.0f;
			float time = Time.deltaTime;
			float rotateDist = time * speed;
			//			float rotateGoal = (rotateTo.
			rotateBy(rotateDist);
		}
	}
	
	public void setRotatingPath() {
//		setRotationFrom((Vector2)currentPath[0],(Vector2)currentPath[1]);
	}
	
	public void setRotationToAttackPlayer() {
		if (attackPlayer != null) {
			setRotationToPlayer(attackPlayer);
		}
	}
	
	public void setRotationFrom(Vector2 from, Vector2 to) {
		rotateFrom = from;
		rotateTo = to;
		rotating = true;
	}
	
	public void setRotationToPlayer(Player player) {
		setRotationFrom(new Vector2(position.x + .001f, position.y), new Vector2(player.position.x, player.position.y));
	}
	
	void rotateBy(float rotateDist) {
		float midSlope = (rotateTo.y - rotateFrom.y)/(rotateTo.x - rotateFrom.x);
		float rotation = Mathf.Atan(midSlope) + Mathf.PI/2.0f;
		Vector3 rot1 = transform.eulerAngles;
		if (rotateTo.x > rotateFrom.x) {
			rotation += Mathf.PI;
		}
		rotation *= 180.0f / Mathf.PI;
		//		rot1.z = rotation;
		//		transform.eulerAngles = rot1;
		float rotation2 = rotation - 360.0f;
		float rotation3 = rotation + 360.0f;
		//	if (rotation == 0.0f) rotation2 = 360.0f;
		float difference1 = Mathf.Abs(rotation - rot1.z);
		float difference2 = Mathf.Abs(rotation2 - rot1.z);
		float difference3 = Mathf.Abs(rotation3 - rot1.z);
		float move1 = rotation - rot1.z;
		float move2 = rotation2 - rot1.z;
		float move3 = rotation3 - rot1.z;
		float sign1 = sign(move1);
		float sign2 = sign(move2);
		float sign3 = sign(move3);
		float s = sign1;
		float m = move1;
		float d = difference1;
		if (difference2 < d) {// || difference1 > 180.0f) {
			Debug.Log("Use 2!!");
			s = sign2;
			m = move2;
			d = difference2;
		}
		if (difference3 < d) {
			s = sign3;
			m = move3;
			d = difference3;
		}
		if (d <= rotateDist) {
			rot1.z = rotation;
			rotating = false;
		}
		else {
			rot1.z += rotateDist * s;
		}
		if (rot1.z <= 0) rot1.z += 360.0f;
		transform.eulerAngles = rot1;
		Debug.Log("Rotate Dist: " + rotateDist + " r1: " + rotation + " r2: " + rotation2 + "  m1: " + move1 + " m2: " + move2);
		//		rotating = false;
	}

	
	
	float sign(float num) {
		if (Mathf.Abs(num) < 0.0001f) return 0.0f;
		if (num > 0) return 1.0f;
		return -1.0f;
	}


	// Use this for initialization
	void Start () {
		hitPoints = maxHitPoints;
	}
	
	// Update is called once per frame
	void Update () {
		doDeath();
		doRotation();
	}
}
