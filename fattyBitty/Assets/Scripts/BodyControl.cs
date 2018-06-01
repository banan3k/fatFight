using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour {

	bool isGrounded = false;

	const int _maxLife = 100;
	const float _startLifeForEyelid = 0.8f;
	const float _eyelidOpacity = 0.8f;
	const int _teethNumber = 4;
	const int _rotateSpeed = 50;
	const int _movingSpeed = 5;
	const int _jumpingPower = 500;
	const float _jumpingPowerMultiply = 1.5f;
	const int _jumpingPowerHigh = (int)((float)_jumpingPower*_jumpingPowerMultiply);
	const int _rotationTailSpeed = 10;
	const float _minToJump = 0.5f;
	const int _degreeMax = 35;
	int teethLevels = (int)(100/_teethNumber);
	int eyelidLife = (int)(_startLifeForEyelid * _maxLife);
	float eyelidStartingPosition;
	int lifeUp = _maxLife/3, lifeDown = _maxLife;

	float jumpCharge = 0;

	bool jumpAction = false;

	Transform eyelid;
	Sprite[][] allMouth = new Sprite[5][];

Transform bottomAngle;
Transform middleAngle;
Transform topAngle;
Transform topBody;
Transform topAngleHolder;

Transform tailBody;


RigidbodyConstraints2D topBodyCons;
	void Start () {

		bottomAngle = gameObject.transform.Find("top/bottomAngle");
		middleAngle = gameObject.transform.Find("top/middleAngle");
		topAngle = gameObject.transform.Find("top/topAngle");
		topAngleHolder = gameObject.transform.Find("top/topAngleHolder");

		topBody = gameObject.transform.Find("top");

		tailBody = gameObject.transform.Find("top/bottom");

		eyelid = gameObject.transform.Find("top/eyes/eyelid");

		topBodyCons = topBody.GetComponent<Rigidbody2D>().constraints;

		for(int i=0; i<=4; i++) {
			allMouth[i] = new Sprite[5];
			for(int i2=0; i2<=4; i2++) {
				allMouth[i][i2] = Resources.Load("mouth\\lose"+i+"Up"+i2+"Down", typeof(Sprite)) as Sprite;
		 	}
		}

		eyelidStartingPosition = eyelid.localPosition.y;
		controlEyelid();
		controlMouth();
	}

	void controlEyelid() {
		int life = (int)((lifeUp+lifeDown)/2);
		SpriteRenderer eyelidSprite = eyelid.GetComponent<SpriteRenderer>();
		Vector2 newPosition = eyelid.localPosition;

		Color tmp = eyelidSprite.color;
		if(life<=eyelidLife) {
			tmp.a = _eyelidOpacity;
			newPosition.y -= 2*eyelidStartingPosition*((_maxLife-life)/(float)_maxLife);
			eyelid.localPosition = newPosition;
		} else {
			tmp.a = 0f;
		}

		eyelidSprite.color = tmp;
	}


	void resetAngleBodies() {
		topBody.SetParent(gameObject.transform);
		topAngle.SetParent(topBody);
		bottomAngle.SetParent(topBody);
	}
	float maxFromTwo(float a, float b) {
		if(a>b)
			return a;
		else
			return b;
	}
	float sumOfTwo(float a, float b) {
		return Mathf.Abs(a)+Mathf.Abs(b);
	}
	void doubleRotate(float lx, float rx) {
		//float biggerRotation = maxFromTwo(lx, rx);
		if(lx<=0 && rx>=0) {
			topBody.transform.Rotate(sumOfTwo(lx, rx)*transform.forward*Time.deltaTime*_rotateSpeed);
		} else if(lx>=0 && rx<=0) {
			topBody.transform.Rotate(-sumOfTwo(lx, rx)*transform.forward*Time.deltaTime*_rotateSpeed);
		} else if(lx>=0 && rx>=0){
			//moving
			gameObject.transform.Translate(maxFromTwo(lx, rx)*transform.right*Time.deltaTime*_movingSpeed);
		} else if(lx<=0 && rx<=0) {
			gameObject.transform.Translate(maxFromTwo(lx, rx)*transform.right*Time.deltaTime*_movingSpeed);
		} else if((lx>0 || rx>0) || (lx<0 || rx<0)) {
			topBody.transform.Rotate(sumOfTwo(lx, rx)*transform.forward*Time.deltaTime*_rotateSpeed);
		}
	}

	public void changeGrounded(bool changeGround) {
		isGrounded = changeGround;
		if(isGrounded==true) {
			if(jumpAction) {
				//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
				//tailBody.GetComponent<Rigidbody2D>().simulated = true;

				jumpAction = false;
			}
			tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		}
	}

	void movementDeciding() {
		float valueXL = Input.GetAxis ("Horizontal");
		float valueYL = Input.GetAxis ("Vertical");

		float valueXR = Input.GetAxis ("Horizontal2");
		float valueYR = Input.GetAxis ("Vertical2");

		Debug.Log(topBody.transform.rotation.eulerAngles.z);
		float step = -_rotationTailSpeed * Time.deltaTime * 5;
		if(tailBody.transform.rotation.eulerAngles.z<180) {
			step *= -1;
		}
		Debug.Log(topBody.transform.rotation.eulerAngles.z+" vs "+Quaternion.identity);


Debug.Log("ground "+isGrounded);
		//jumping
		if(isGrounded) {

			if((topBody.transform.rotation.eulerAngles.z>70 && topBody.transform.rotation.eulerAngles.z<250)) {
				tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
				tailBody.GetComponent<Rigidbody2D>().simulated = false;

				topBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
				//tailBody.transform.rotation =  Quaternion.identity;//Quaternion.RotateTowards(tailBody.transform.rotation, Quaternion.identity, step);
				Debug.Log("aaa");
				valueXL = valueXR = 0;
				jumpAction = true;
				if(valueYL>_minToJump && valueYR>_minToJump) {
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower));
					Debug.Log("be");
					//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
					tailBody.GetComponent<Rigidbody2D>().simulated = true;

					topBody.GetComponent<Rigidbody2D>().constraints = topBodyCons;
				}
			} else {



			//Debug.Log(valueXL);
				Debug.Log(topBody.transform.rotation.eulerAngles.z);
				if(valueXL<0 && topBody.transform.rotation.eulerAngles.z>180 && topBody.transform.rotation.eulerAngles.z<360-_degreeMax) {
					valueXL=0;
				} else if(valueXL>0 && topBody.transform.rotation.eulerAngles.z<180 && topBody.transform.rotation.eulerAngles.z>_degreeMax) {
					valueXL=0;
				}
				if(valueXR>0 && topBody.transform.rotation.eulerAngles.z>180 && topBody.transform.rotation.eulerAngles.z<360-_degreeMax) {
					valueXR=0;
				} else if(valueXR<0 && topBody.transform.rotation.eulerAngles.z<180 && topBody.transform.rotation.eulerAngles.z>_degreeMax) {
					valueXR=0;
				}

				//topBody.Find("bottom").GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
				if(valueYL>_minToJump && valueYR>_minToJump) {
					jumpAction = true;
					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					//topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,sumOfTwo(valueYL, valueYR)*_jumpingPowerHigh));
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPowerHigh));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPowerHigh));
					//gameObject.transform.Translate(sumOfTwo(valueYL, valueYR)*transform.up*Time.deltaTime*_jumpingPowerHigh);
				} else if(valueYL>_minToJump && valueYR<=_minToJump) {
					jumpAction = true;

					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPower));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPower));
					//gameObject.transform.Translate(valueYL*transform.up*Time.deltaTime*_jumpingPower);
				} else if(valueYL<=_minToJump && valueYR>_minToJump) {
					jumpAction = true;

					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPower));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPower));
					//gameObject.transform.Translate(valueYR*transform.up*Time.deltaTime*_jumpingPower);
				}

				if(valueYL<=0 && valueYR <=0 && sumOfTwo(valueYL,valueYR)>0) {
					jumpCharge += sumOfTwo(valueYL,valueYR)/2;
				}
			}





		} else {
			if(tailBody.transform.rotation.z!=0) {

				//tailBody.transform.rotation =  Quaternion.RotateTowards(tailBody.transform.rotation, Quaternion.identity, step);
			}
			jumpCharge = 0;


		}


		//Debug.Log(jumpCharge);

		//rotating
		if(!jumpAction) {
			if((valueXL!=0 && valueXR!=0)) {
				if(topBody.parent != gameObject.transform) {
					resetAngleBodies();
				}
				doubleRotate(valueXL, valueXR);
				//topBody.transform.Rotate((valueXL+valueYL+valueXR+valueYR)*transform.forward*Time.deltaTime*_rotateSpeed);
			}
			else if(valueXL!=0) {
				if(topAngle.parent != gameObject.transform) {
					topAngle.SetParent(gameObject.transform);
				}
				if(topBody.parent != topAngle) {
					topBody.SetParent(topAngle);
				}
				topAngle.transform.Rotate(valueXL*transform.forward*Time.deltaTime*_rotateSpeed);
			} else if(valueXR!=0) {
				if(bottomAngle.parent != gameObject.transform) {
					bottomAngle.SetParent(gameObject.transform);
				}
				if(topBody.parent != bottomAngle) {
					topBody.SetParent(bottomAngle);
				}
				bottomAngle.transform.Rotate(-valueXR*transform.forward*Time.deltaTime*_rotateSpeed);
				//tailBody.GetComponent<Rigidbody2D>().velocity = new Vector3(0,-tailBody.GetComponent<Rigidbody2D>().velocity.y,0);
			} else if(topBody.parent != gameObject.transform){
				resetAngleBodies();
			}
		//Debug.Log(valueXL+" VS "+valueYL);
		} else {
			if(topBody.parent != gameObject.transform) {
				resetAngleBodies();
			}
			doubleRotate(valueXL, valueXR);
		}
	}



	void controlMouth() {
		SpriteRenderer eyelidSprite = gameObject.transform.Find("top/mouth").GetComponent<SpriteRenderer>();
		int levelLifeUp = _teethNumber-(lifeUp/teethLevels);
		int levelLifeDown = _teethNumber-(lifeDown/teethLevels);
		//string mouthId = "lose"+levelLifeUp+"Up"+levelLifeDown+"Down";
		Debug.Log(levelLifeUp+" vs "+levelLifeDown);
		eyelidSprite.sprite = allMouth[levelLifeUp][levelLifeDown];
	}

	// Update is called once per frame
	void FixedUpdate () {
		movementDeciding();

		topAngle.transform.position=topAngleHolder.transform.position;
		//if(topAngle.transform.position!=topAngleHolder.tra)
		//	GameObject.Find("Avatar1 (1)").transform.Find("top").transform.Rotate(Vector3.forward * Time.deltaTime*8);
	}

	IEnumerator switchCombo()
	{
		yield return new WaitForSeconds(5);
		Debug.Log("A");
	}
}
