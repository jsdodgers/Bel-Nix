using UnityEngine;
using System.Collections;

public class ClanPawnAnimation : MonoBehaviour {
	Animator anim;
	float speed, para;
	float initHeight;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		speed = 3;
		para = 3;
		initHeight = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Stand_Up"))
		{
			transform.Translate(Time.deltaTime * -speed, 0 ,0);
		}
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Flying") || anim.GetCurrentAnimatorStateInfo(0).IsName("Flip") || anim.GetCurrentAnimatorStateInfo(0).IsName("Flying2"))
		{
			transform.Translate(Time.deltaTime * -speed, Time.deltaTime * para, 0);
			para -= 0.05f;
		}
		if(para <= 0.5f)
		{
			anim.SetInteger("flip", 1);
		}
		if(transform.position.y < initHeight)
		{
			anim.SetInteger("stop", 1);
			transform.position = new Vector3(transform.position.x, initHeight, transform.position.z);
		}
	}
}
