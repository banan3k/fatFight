using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedControl : MonoBehaviour {

	public GameObject mainParent;

	void OnCollisionEnter2D(Collision2D theCollision){
		if(theCollision.gameObject.name == "Floor")
		{
				//controlComponent.isGrounded = true;
				controlComponent.changeGrounded(true);
				Debug.Log("grounded");
		}
	}
	void OnCollisionExit2D(Collision2D theCollision){
		if(theCollision.gameObject.name == "Floor")
		{
				//controlComponent.isGrounded = false;
				controlComponent.changeGrounded(false);
				Debug.Log("not grounded");
		}
	}


	BodyControl controlComponent;
	// Use this for initialization
	void Start () {
		controlComponent = mainParent.GetComponent<BodyControl>();
		//gameObject.transform.parent.transform.GetComponent<InputControl>();

	}

	// Update is called once per frame
	void Update () {

	}
}
