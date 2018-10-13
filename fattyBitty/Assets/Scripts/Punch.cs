using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

	Transform topBody, tailBody;
	// Use this for initialization
	void Start () {
		if (gameObject.transform.parent.tag == "avatar1") {
			IsAI = false;
			toPush = GameObject.Find("Avatar2").transform;
			Debug.Log("from humananana");
			//startingTailMass = GameObject.Find("Avatar1").transform.Find("top/bottom").GetComponent<Rigidbody2D>().mass;
			//topBodyCons = GameObject.Find("Avatar1").transform.Find("top").GetComponent<Rigidbody2D>().constraints;
		} else if (gameObject.transform.parent.tag == "avatar2"){
			IsAI = true;
			toPush = GameObject.Find("Avatar1").transform;
			Debug.Log("from AIAIAIAIAI");
		}

		if(gameObject.name == "blockCollP") {
			IsAI = false;
			whatSide = -1;
			toPush = GameObject.Find("Avatar2").transform;
		} else if(gameObject.name == "blockColl"){
			IsAI = true;
			toPush = GameObject.Find("Avatar1").transform;
		}

		topBody = toPush.Find("top").transform;
		tailBody = toPush.Find("top/bottom").transform;
		startingTailMass = tailBody.GetComponent<Rigidbody2D>().mass;
		topBodyCons = topBody.GetComponent<Rigidbody2D>().constraints;
}

float giveDiff(float a, float b) {
	return Mathf.Abs(a-b);
}
RigidbodyConstraints2D topBodyCons;
float startingTailMass = 0;
float rotationPower = 15;

int whatSide = 1;
	// Update is called once per frame
	void Update () {
		if(punchLanded && (giveDiff(toPush.position.x,positionAfterPunch.x)>Time.deltaTime || toPush.position.y != positionAfterPunch.y)) {
			Debug.Log("movvvvvvving "+toPush.position+"/"+positionAfterPunch);
			if(toPush.position.x != positionAfterPunch.x) {
				Debug.Log("ech");
				Debug.Log("ech2 "+toPush.position.x+" vs "+positionAfterPunch.x);
			}
			if(toPush.position.y != positionAfterPunch.y) {
				Debug.Log("Ech3");
			}
			toPush.position = Vector2.MoveTowards(toPush.position, positionAfterPunch, Time.deltaTime);
			toPush.Rotate(whatSide*Vector3.forward * Time.deltaTime*rotationPower);
		} else {
			punchLanded = false;
		}
	}
	Transform toPush;
	bool IsAI;

	public bool punchIsBlocked = false;
	float punchJumpPower = 1000;

	float minForMid = 0.1f;
	int punchPower = 10;
	void OnTriggerEnter2D(Collider2D col)
  {
		Debug.Log(gameObject.name+" vs "+IsAI+" punch enter trigger: "+col.gameObject.name+" vs "+col.gameObject.tag);
		//if(/*!punchIsBlocked && */gameObject.name == "attackCollP" && col.gameObject.name == "Avatar2") {
		if(gameObject.name == "attackCollP" && (col.gameObject.name == "top" || col.gameObject.name == "bottoddsadsam") && col.gameObject.tag == "avatar2") {
			//enemy hitted!
			Debug.Log("ai has been punched");
			Vector2 dir = col.transform.parent.transform.position - transform.position;
	    dir = dir.normalized;
			dir.y=Mathf.Abs(dir.y*punchJumpPower);
			//dir.y *= 500;
			//dir.y *= BodyControl._punchForce*1;
			Debug.Log("with power of "+dir.x+"/"+dir.y);
			Debug.Log(transform.position+" from position : "+col.transform.position);
			positionAfterPunch = new Vector2(col.transform.parent.transform.position.x+dir.x, 0/*col.transform.position.y+dir.y*/);
			Debug.Log("into direction of "+positionAfterPunch);

			punchLanded = true;
			//positionAfterPunch.y=0;
			topBody.GetComponent<Rigidbody2D>().AddForce(dir);
			tailBody.GetComponent<Rigidbody2D>().AddForce(dir);

			int whatPartToDmg = 0;
			if(gameObject.transform.position.y > minForMid) {
				whatPartToDmg = 1;
			} else if(gameObject.transform.position.y < minForMid) {
				whatPartToDmg = 2;
			}
			if(col.gameObject.name != "bottom") {
				//col.transform.parent.GetComponent<BodyControl>().GetDmg(punchPower, whatPartToDmg);
			} else {
				if(col.transform.parent.name == "top") {
					//col.transform.parent.transform.parent.GetComponent<BodyControl>().GetDmg(punchPower, whatPartToDmg);
				}
			}
			//gameObject.transform.localPosition = Vector2.zero;
			//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		//	tailBody.GetComponent<Rigidbody2D>().simulated = false;
			StartCoroutine("delayCon");
		}


		if(gameObject.name == "attackColl" && col.gameObject.name == "blockCollP" && IsAI) {
			Debug.Log("player blocked");
			punchIsBlocked = true;
		} else if(gameObject.name == "attackCollP" && col.gameObject.name == "blockColl") {
			Debug.Log("ai blockedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			punchIsBlocked = true;
		}
	}

	IEnumerator delayCon() {
		yield return new WaitForSeconds(0.01f);
		tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

	}

	bool punchLanded;
	Vector2 positionAfterPunch;
	void OnCollisionEnter2D(Collision2D col)
  {
		Debug.Log(col.gameObject.transform.rotation);
		//if(col.gameObject.tag=="avatar2") {
		//Debug.Log("adsadsad"+col.gameObject.tag);
		if(col.gameObject.tag=="avatar1" && IsAI) {
			Debug.Log("player has been punched");
		} else if(col.gameObject.tag=="avatar2" && !IsAI) {
			/*Debug.Log("ai has been punched");
			Vector2 dir = col.transform.position - transform.position;
	    dir = dir.normalized;
			dir.y=1;
			dir.y *= 500;
			//dir.y *= BodyControl._punchForce*1;
			Debug.Log("with power of "+dir.x+"/"+dir.y);
			Debug.Log("from position : "+col.transform.parent.position);
			positionAfterPunch = new Vector2(col.transform.parent.position.x+dir.x, col.transform.parent.position.y+dir.y);
			Debug.Log("into direction of "+positionAfterPunch);
			punchLanded = true;

positionAfterPunch.y=0;
			//tailBody.GetComponent<Rigidbody2D>().mass = startingTailMass;

			//topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower));
			//tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower));
			Debug.Log("be");
			//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		//	tailBody.GetComponent<Rigidbody2D>().simulated = false;
			topBody.GetComponent<Rigidbody2D>().AddForce(dir);
			tailBody.GetComponent<Rigidbody2D>().AddForce(dir);
			//topBody.GetComponent<Rigidbody2D>().simulated = false;

			//topBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
			tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		*/
		}

		if(!punchIsBlocked) {
			//add dmg
		}

		/*if(col.gameObject.name == "blockCollP" && IsAI) {
			Debug.Log("player blocked");
		} else if(col.gameObject.name == "blockColl" && !IsAI) {
			Debug.Log("ai blockedaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
		}*/


	}
}
