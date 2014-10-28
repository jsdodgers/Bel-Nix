using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	Animator anim;
	KeyCode moveLeft, moveRight, moveFaster;
	bool ignoreInputLeft, ignoreInputRight;
	float speed;

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
		moveLeft = KeyCode.A;
		moveRight = KeyCode.D;
		moveFaster = KeyCode.LeftShift;
		ignoreInputLeft = false;
		ignoreInputRight = false;
		speed = 1;
	}
	
	// Update is called once per frame
	void Update()
	{
		MoveLeft();
		MoveRight();
	}

	void MoveLeft()
	{
		if(anim.GetBool("isFacingLeft") && !ignoreInputLeft)
		{
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Left"))
			{
				transform.Translate(Time.deltaTime * -speed, 0 ,0);
				ignoreInputRight = true;
			}
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Left"))
			{
				transform.Translate(Time.deltaTime * -speed * 2, 0 ,0);
				ignoreInputRight = true;
			}
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				ignoreInputRight = false;
			}
			if(Input.GetKey(moveLeft))
			{
				if(Input.GetKey(moveFaster))
				{
					anim.SetInteger("direction", -2);
				}
				else
				{
					anim.SetInteger("direction", -1);
				}
			}
			if(Input.GetKeyUp(moveLeft))
			{
				anim.SetInteger("direction", 0);
			}
			if(Input.GetKey(moveRight) && !ignoreInputRight)
			{
				anim.SetBool("isFacingLeft", false);
			}
		}
	}

	void MoveRight()
	{
		if(!anim.GetBool("isFacingLeft") && !ignoreInputRight)
		{
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Right"))
			{
				transform.Translate(Time.deltaTime * speed, 0 ,0);
				ignoreInputLeft = true;
			}
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Right"))
			{
				ignoreInputLeft = false;
			}
			if(Input.GetKey(moveRight))
			{
				anim.SetInteger("direction", 1);
			}
			if(Input.GetKeyUp(moveRight))
			{
				anim.SetInteger("direction", 0);
			}
			if(Input.GetKey(moveLeft) && !ignoreInputLeft)
			{
				anim.SetBool("isFacingLeft", true);
			}
		}
	}
}
