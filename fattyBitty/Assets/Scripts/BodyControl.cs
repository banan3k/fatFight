using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour {

	public bool IsAI = false;


	bool isGrounded = false;

	public const int _punchForce = 4;

	const int _maxLife = 100;
	const float _startLifeForEyelid = 0.8f;
	const float _eyelidOpacity = 0.8f;
	const int _teethNumber = 4;
	const int _rotateSpeed = 50;
	const int _movingSpeed = 5;
	const int _jumpingPower = 300;
	const float _jumpingPowerMultiply = 1.5f;
	const int _jumpingPowerHigh = (int)((float)_jumpingPower*_jumpingPowerMultiply);
	const int _rotationTailSpeed = 2000;
	const int _movingTailSpeed = 2;
	const float _minToJump = 0.5f;
	const int _degreeMax = 35;
	int teethLevels = (int)(100/_teethNumber);
	int eyelidLife = (int)(_startLifeForEyelid * _maxLife);
	float eyelidStartingPosition;
	float lifeUp = _maxLife/2, lifeDown = _maxLife;

	float jumpCharge = 0;

	bool jumpAction = false;

	float startingTailMass = 0;
	const float _MinMass = 0.0001f;

	float blockSpeed = 5;

	Transform eyelid;
	Sprite[][] allMouth = new Sprite[5][];

Transform bottomAngle;
//Transform middleAngle;
Transform topAngle;
Transform topBody;
Transform topAngleHolder, bottomAngleHolder;



Transform mainCamera;
AImove ai;

Transform tailBody;

//Quaternion _QuaternionZero = new Quaternion(0,0,0,0);

Vector3 tailStartingPosition;



Texture2D highPunchIMG, lowPunchIMG, midPunchIMG1, midPunchIMG2;
Transform leftArm, rightArm, leftLeg, rightLeg;
Texture2D img;
Transform attackColl;

Transform blockLeftArm, blockRightArm;
Texture2D blockIMG;
Transform blockColl;

RigidbodyConstraints2D topBodyCons;
	void Start () {


		mainCamera = GameObject.Find("Main Camera").transform;
		ai = mainCamera.GetComponent<AImove>();
		//Debug.Log(Quaternion.identity);
		bottomAngle = gameObject.transform.Find("top/bottomAngle");
		//middleAngle = gameObject.transform.Find("top/middleAngle");
		topAngle = gameObject.transform.Find("top/topAngle");
		topAngleHolder = gameObject.transform.Find("top/topAngleHolder");
		bottomAngleHolder = gameObject.transform.Find("top/bottomAngleHolder");


		topBody = gameObject.transform.Find("top");

		tailBody = gameObject.transform.Find("top/bottom");

		eyelid = gameObject.transform.Find("top/eyes/eyelid");

		topBodyCons = topBody.GetComponent<Rigidbody2D>().constraints;

		tailStartingPosition = tailBody.localPosition;
		startingTailMass = tailBody.GetComponent<Rigidbody2D>().mass;
		//Debug.Log(tailStartingPosition);

		if(IsAI){
			attackColl = topBody.Find("attackColl");
			blockColl = topBody.Find("blockColl");
		} else {
			attackColl = topBody.Find("attackCollP");
			blockColl = topBody.Find("blockCollP");
		}
		leftArm = topBody.Find("leftArm");
		highPunchIMG = Resources.Load("attacks\\highPunch", typeof(Texture2D)) as Texture2D;
		lowPunchIMG = Resources.Load("attacks\\lowPunch", typeof(Texture2D)) as Texture2D;
		midPunchIMG1 = Resources.Load("attacks\\midPunch1", typeof(Texture2D)) as Texture2D;
		midPunchIMG2 = Resources.Load("attacks\\midPunch2", typeof(Texture2D)) as Texture2D;

		blockLeftArm = topBody.Find("blockLeftArm");
		blockRightArm = topBody.Find("blockRightArm");
		blockIMG = Resources.Load("attacks\\highPunch", typeof(Texture2D)) as Texture2D;


		//img = Resources.Load("attacks\\punchMid", typeof(Texture2D)) as Texture2D;
		//Sprite tempSprite = Sprite.Create(img, new Rect(0, 0, img.width/2, img.height), new Vector2(0, 1f), 100);
		//leftArm.GetComponent<SpriteRenderer>().sprite = tempSprite;
	//}\


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



	public void GetDmg(int dmg, int whatPart=0) {
		dmg -= (int)(whatControl() * ((float)dmg/2));
		switch(whatPart) {
			case 0:
				lifeDown -= dmg/2;
				lifeUp -= dmg/2;
				break;
			case 1:
				//upper
				lifeUp -= dmg;
				break;
			case 2:
				lifeDown -= dmg;
				break;
		}
	}
	float whatControl(int whatPart=0) {
		//Debug.Log(((lifeUp+lifeDown)/2)/100+" vs "+lifeUp+" and "+lifeDown);
		switch(whatPart) {
			default :
			case 0:
				return ((lifeUp+lifeDown)/2)/100;
			case 1:
				//upper
				return lifeUp/100;
			case 2:
				return lifeDown/100;
		}
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

				tailBody.GetComponent<Rigidbody2D>().mass = startingTailMass;
			}
			tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		}
	}

	void smoothTailReset() {
		float step = -_rotationTailSpeed * Time.deltaTime;
		float step2 = _movingTailSpeed * Time.deltaTime;

		if(IsAI) {
			if(giveDiff(tailBody.transform.eulerAngles.z, 90)>Time.fixedDeltaTime) {
				int direction = 1;
				if(tailBody.transform.rotation.eulerAngles.z>270) {
					direction = -1;
				}
				tailBody.transform.eulerAngles = new Vector3(0,0,tailBody.transform.eulerAngles.z+_movingTailSpeed*direction);
				//tailBody.transform.Rotate(Vector3.forward*_movingTailSpeed*Time.deltaTime*10, Space.World);
				//tailBody.transform.rotation = new Quaternion(0,0,tailBody.transform.rotation.z+0.1f,0);
			}
			//tailBody.transform.rotation = new Quaternion(0,0,0,0);
			//tailBody.transform.localPosition=tailStartingPosition;
		} else {


			if((tailBody.transform.rotation.eulerAngles.z<90 && topBody.transform.rotation.eulerAngles.z<180) || (tailBody.transform.rotation.eulerAngles.z<270 && topBody.transform.rotation.eulerAngles.z>180)) {
				step *= -1;
			}

			tailBody.transform.Rotate(Vector3.forward*Time.deltaTime*step);
			if(tailBody.transform.localPosition.y>tailStartingPosition.y) {
			//	step2 *= -1;
			}
			if(tailBody.transform.localPosition!=tailStartingPosition) {
				//tailBody.transform.localPosition=tailStartingPosition;
				tailBody.transform.localPosition = Vector2.MoveTowards(tailBody.transform.localPosition, tailStartingPosition, step2);
			}

		}
		//tailBody.transform.localPosition = Vector2.MoveTowards(tailBody.transform.localPosition, tailStartingPosition, step2);
	}

	bool lyingDown = false;
	float waitForAIStand = 5.5f;
	bool rotateAIToZero = false;

	bool tempStop = false;
	IEnumerator getUpAI() {
		tempStop = true;
		lyingDown = true;
		yield return new WaitForSeconds(waitForAIStand);
		rotateAIToZero = true;
		tailBody.GetComponent<Rigidbody2D>().mass = startingTailMass;

		topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,_jumpingPower*1.5f));
		tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,_jumpingPower*1.5f));
		//yield return new WaitForSeconds(waitForAIStand);
		//Debug.Log("be2222222222222222222222222222222222222");
		//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

		tailBody.GetComponent<Rigidbody2D>().simulated = true;

		topBody.GetComponent<Rigidbody2D>().constraints = topBodyCons;
		tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		//lyingDown = false;*/
	}

	const int maxLying = 10;
	float howLongLaying = maxLying;
	public bool isDead = false;
	void movementDeciding(float valueXL, float valueYL,float valueXR, float valueYR) {
		valueXL *= whatControl();
		valueYL *= whatControl();
		valueXR *= whatControl();
		valueYR *= whatControl();
		//Debug.Log("dsadsadsadsa "+whatControl());
		/*float valueXL = Input.GetAxis ("Horizontal");
		float valueYL = Input.GetAxis ("Vertical");

		float valueXR = Input.GetAxis ("Horizontal2");
		float valueYR = Input.GetAxis ("Vertical2");*/

		//Debug.Log(tailBody.transform.rotation.eulerAngles.z);

		/*float step = -_rotationTailSpeed * Time.deltaTime*100;
		float step2 = _rotationTailSpeed * Time.deltaTime;
		if(tailBody.transform.rotation.eulerAngles.z<90) {
			step *= -1;
		}*/
		//Debug.Log(" vs "+topBody.transform.rotation.eulerAngles.z+" vs "+Quaternion.identity);

		//tailBody.transform.rotation =  Quaternion.RotateTowards(tailBody.transform.rotation, _QuaternionZero, step*10000);

		//Debug.Log("ground "+isGrounded);
		//jumping
		//if(IsAI) {
			//Debug.Log("knoooooooooooooooooooooooooooooooocked "+isGrounded);
			//Debug.Log(topBody.transform.rotation.eulerAngles.z);
		//}
		Debug.Log(this.transform.GetChild(0).position.y);
		if(this.transform.GetChild(0).position.y<-10) {
			GameObject.Find("Main Camera").GetComponent<InputControl>().startAgain();
			isDead = true;
		}

		if(isGrounded) {
			if(tailBody.GetComponent<Rigidbody2D>().mass!=startingTailMass){
				//tailBody.GetComponent<Rigidbody2D>().mass = startingTailMass;
			}

			if((topBody.transform.rotation.eulerAngles.z>_degreeMax*1.5f && topBody.transform.rotation.eulerAngles.z<360-_degreeMax*1.5f)) {
				//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
				tailBody.GetComponent<Rigidbody2D>().simulated = false;


				topBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

				//if(tailBody.transform.rotation.eulerAngles.z<90.0f || tailBody.transform.rotation.eulerAngles.z>91.0f) {
					//tailBody.transform.Rotate(Vector3.forward*Time.deltaTime*step);
					//tailBody.transform.localPosition = tailStartingPosition;
					//tailBody.transform.localPosition = Vector2.MoveTowards(tailBody.transform.localPosition, tailStartingPosition, step2);
				//}
				smoothTailReset();
				howLongLaying -= Time.deltaTime;
				if(howLongLaying<=0) {
					isDead = true;
				}


				//tailBody.transform.rotation =  Quaternion.RotateTowards(tailBody.transform.rotation, _QuaternionZero, step*100);
		//		Debug.Log("aaa");
				valueXL = valueXR = 0;
				jumpAction = true;
				if(IsAI && !lyingDown) {
					StartCoroutine("getUpAI");
					Debug.Log(topBody.transform.rotation.eulerAngles.z+ "ai groundeddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd");

				} else if(valueYL>_minToJump && valueYR>_minToJump) {
					tailBody.GetComponent<Rigidbody2D>().mass = startingTailMass;

					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower*1.5f));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,(valueYL+valueYR)*_jumpingPower*1.5f));
					Debug.Log("be");
					//tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
					tailBody.GetComponent<Rigidbody2D>().simulated = true;

					topBody.GetComponent<Rigidbody2D>().constraints = topBodyCons;
					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
				}
			} else {
//tailBody.transform.localPosition=tailStartingPosition;
				topBody.GetComponent<Rigidbody2D>().constraints = topBodyCons;



			//Debug.Log(valueXL);
				//Debug.Log(topBody.transform.rotation.eulerAngles.z);
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
					//jumpAction = true;
					setJumpAction();

					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					//topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,sumOfTwo(valueYL, valueYR)*_jumpingPowerHigh));
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPowerHigh));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPowerHigh*_MinMass));
					//gameObject.transform.Translate(sumOfTwo(valueYL, valueYR)*transform.up*Time.deltaTime*_jumpingPowerHigh);
				} else if(valueYL>_minToJump && valueYR<=_minToJump) {
					//jumpAction = true;
					setJumpAction();

					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPower));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYL*_jumpingPower*_MinMass));
					//gameObject.transform.Translate(valueYL*transform.up*Time.deltaTime*_jumpingPower);
				} else if(valueYL<=_minToJump && valueYR>_minToJump) {
					//jumpAction = true;
					setJumpAction();

					tailBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					topBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPower));
					tailBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,valueYR*_jumpingPower*_MinMass));
					//gameObject.transform.Translate(valueYR*transform.up*Time.deltaTime*_jumpingPower);
				}

				if(valueYL<=0 && valueYR <=0 && sumOfTwo(valueYL,valueYR)>0) {
					jumpCharge += sumOfTwo(valueYL,valueYR)/2;
				}
			}





		} else {
			if(tailBody.GetComponent<Rigidbody2D>().mass==startingTailMass){
				//tailBody.GetComponent<Rigidbody2D>().mass = _MinMass;
			}
			//smoothTailReset();
			if(tailBody.transform.rotation.z!=0) {

				//tailBody.transform.rotation =  Quaternion.RotateTowards(tailBody.transform.rotation, Quaternion.identity, step);
			}
			jumpCharge = 0;

			howLongLaying = maxLying;
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

	void setJumpAction() {
		tailBody.GetComponent<Rigidbody2D>().mass = _MinMass;
		jumpAction = true;
	}

	void controlMouth() {
		SpriteRenderer eyelidSprite = gameObject.transform.Find("top/mouth").GetComponent<SpriteRenderer>();
		int levelLifeUp = (int)(_teethNumber-(lifeUp/teethLevels));
		int levelLifeDown = (int)(_teethNumber-(lifeDown/teethLevels));
		//string mouthId = "lose"+levelLifeUp+"Up"+levelLifeDown+"Down";
		//Debug.Log(levelLifeUp+" vs "+levelLifeDown);
		eyelidSprite.sprite = allMouth[levelLifeUp][levelLifeDown];
	}

	bool aiIsOff = false;
	public void aiOff(bool turnOff) {
		GameObject.Find("Main Camera").GetComponent<AImove>().enabled = !turnOff;
		aiIsOff = turnOff;
	}

	float giveDiff(float a, float b) {
		return Mathf.Abs(a-b);
	}

	float[] movementArray = new float[4];
	float rotationSpeed = 100;




	void FixedUpdate () {
		if(GameObject.Find("Main Camera").GetComponent<InputControl>().gameStarted) {



			if(IsAI) {
				//Debug.Log(tailBody.transform.rotation.eulerAngles.z+ "ai groundeddddddddddddcccccccccccccccccccccccccccccccc");

				if(rotateAIToZero) {
					//Debug.Log("ai rotation : "+topBody.transform.rotation);
					if(360-topBody.transform.rotation.z>topBody.transform.rotation.z) {
						topBody.transform.Rotate(Vector3.forward*Time.fixedDeltaTime*rotationSpeed);
					} else {
						topBody.transform.Rotate(-Vector3.forward*Time.fixedDeltaTime*-rotationSpeed);
					}

					//Quaternion.LookRotation(Vector3.RotateTowards(topBody.transform.forward, Vector3.zero, rotationSpeed*-Time.fixedDeltaTime, 0.0f));
					if(giveDiff(topBody.transform.rotation.z,0)<Time.fixedDeltaTime) {
						rotateAIToZero = false;
						lyingDown = false;
					}
				}
				if(aiIsOff) {
					movementDeciding(Input.GetAxis ("HorizontalN"), Input.GetAxis ("VerticalN"), Input.GetAxis ("HorizontalN2"), Input.GetAxis ("VerticalN2"));
				} else {
					//if(!lyingDown && !rotateAIToZero) {
					//	Debug.Log("ai sdasdsadlllllllllllllllllllllllllllLLLll");

					movementArray = ai.AIanalogs();
					movementArray[0] *= -1; //switching x axises
					movementArray[2] *= -1;
					if((lyingDown && rotateAIToZero) || tempStop) {
						movementArray[0] = 0;
						movementArray[1] = 0;
						movementArray[2] = 0;
						movementArray[3] = 0;
					}
					//Debug.Log("ai analoging: "+movementArray[0]+" vs "+movementArray[1]+" vs "+movementArray[2]+" vs "+movementArray[3]);
					movementDeciding(movementArray[0],movementArray[1],movementArray[2],movementArray[3]);

				}
			} else {
				//movementArray = {Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), Input.GetAxis ("Horizontal2"), Input.GetAxis ("Vertical2")};
				movementDeciding(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), Input.GetAxis ("Horizontal2"), Input.GetAxis ("Vertical2"));
			}

			makingAction();

			topAngle.transform.position=topAngleHolder.transform.position;
			bottomAngle.transform.position=bottomAngleHolder.transform.position;
			//if(topAngle.transform.position!=topAngleHolder.tra)
			//	GameObject.Find("Avatar1 (1)").transform.Find("top").transform.Rotate(Vector3.forward * Time.deltaTime*8);
		}
	}

	bool isActionProgress = false;
	float spriteSize = 0;
	Sprite spriteInAction;
	void makingAction() {
		if(isActionProgress) {
			Debug.Log("incrsi "+spriteSize);
			if(goingUp) {
				spriteSize = Mathf.Clamp(spriteSize+Time.deltaTime*1000, spriteSize, img.height);
			} else {
				spriteSize = Mathf.Clamp(spriteSize+Time.deltaTime*1000, spriteSize, img.width);
			}
			if((spriteSize == img.width && !goingUp) || (spriteSize == img.height && goingUp)) {
				spriteSize = 0;
				attackColl.localPosition = new Vector2(0,0);
				isActionProgress = false;
			}
			attackColl.Translate(destincyForPunch);
			if(goingUp) {

				spriteInAction = Sprite.Create(img, new Rect(0, 0, img.width, spriteSize), new Vector2(0.5f, 0), 100);
			} else {
				//leftArm.GetComponent<SpriteRenderer>().flipX = false;
				spriteInAction = Sprite.Create(img, new Rect(0, 0, spriteSize, img.height), new Vector2(0, 0.5f), 100);
			}
			leftArm.GetComponent<SpriteRenderer>().sprite = spriteInAction;
			//leftArm.Translate(new Vector2(-Time.deltaTime*5,0));
		}
	}
	Vector2 destincyForPunch = Vector2.zero;
	float punchSpeed = 20f;
	bool goingUp = false;

	public void MakeBlock(float L, float R) {
		Vector2 blockPlace = Vector2.zero;
		Vector2 blockCollPlace = Vector2.zero;
		int[] blockStates = {(int)L, (int)R};
		//Debug.Log(L+" block vs "+R);
		bool useLeft = false, useRight = false;

		//blockColl.transform.localPosition = new Vector2(0,0);
		if(blockStates[0]==0 && blockStates[1]==0) {
			TurnOffBlock();
		} else if(blockStates[0]==1 && blockStates[1]==0) {
			Debug.Log("light mid block left");
			useLeft = true;
			blockCollPlace = new Vector2(-3.5f,0);
		} else if(blockStates[0]==-1 && blockStates[1]==0) {
			Debug.Log("light up block left");
			blockPlace = new Vector2(-1.5f, 2);
			blockCollPlace = new Vector2(-3.5f,4);
			useLeft = true;
		} else if(blockStates[0]==0 && blockStates[1]==1) {
			Debug.Log("light mid block right");
			blockCollPlace = new Vector2(-3.5f,0);
			useRight = true;
		} else if(blockStates[0]==0 && blockStates[1]==-1) {
			Debug.Log("light up block right");
			blockPlace = new Vector2(-1.5f, 2);
			blockCollPlace = new Vector2(-3.5f,4);
			useRight = true;
		} else if(blockStates[0]==-1 && blockStates[1]==-1) {
			Debug.Log("heavy up block both");
			blockPlace = new Vector2(-1.5f, 2);
			blockCollPlace = new Vector2(-3.5f,4);
			useLeft = useRight = true;
		} else if(blockStates[0]==1 && blockStates[1]==1) {
			Debug.Log("heavy mid block both");
			blockCollPlace = new Vector2(-3.5f,0);
			useLeft = useRight = true;
		}
		if(blockColl.transform.localPosition.x!=blockCollPlace.x
				|| blockColl.transform.localPosition.y!=blockCollPlace.y) {
					Debug.Log(blockColl.transform.localPosition+" lerping "+blockCollPlace);
		//	blockColl.transform.localPosition = new Vector2(Vector2.MoveTowards(blockColl.transform.localPosition, blockCollPlace, Time.fixedDeltaTime));
				int directionTempX = 1;
				if(blockColl.transform.localPosition.x<=blockCollPlace.x) {
					directionTempX = -1;
				}
				int directionTempY = 1;
				if(blockColl.transform.localPosition.y>=blockCollPlace.y) {
					directionTempY = -1;
				}
				blockColl.transform.localPosition = new Vector2(blockColl.transform.localPosition.x-Time.deltaTime*blockSpeed*directionTempX,blockColl.transform.localPosition.y+Time.deltaTime*blockSpeed*directionTempY);
		}
		//blockColl.transform.localPosition = new Vector2(blockColl.transform.localPosition.x-Time.deltaTime,blockColl.transform.localPosition.y+Time.deltaTime);
//img = blockIMG;
		blockLeftArm.transform.localPosition = blockPlace;
		blockRightArm.transform.localPosition = new Vector2(blockPlace.x*1.33f, blockPlace.y*1.1f);
		blockLeftArm.GetComponent<SpriteRenderer>().enabled = useLeft;
		blockRightArm.GetComponent<SpriteRenderer>().enabled = useRight;
	}
	public void TurnOffBlock() {
		blockColl.transform.localPosition = new Vector2(0,0);
		blockLeftArm.transform.localPosition = new Vector2(0,0);
		blockLeftArm.GetComponent<SpriteRenderer>().enabled = false;
		blockRightArm.transform.localPosition = new Vector2(0,0);
		blockRightArm.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void MakeAction(int index){
		if(!isActionProgress || (IsAI && !rotateAIToZero)) {
			Debug.Log("making action : "+index);
			blockColl.GetComponent<Punch>().punchIsBlocked = false;
			//leftArm.transform.localPosition = new Vector2(-2,0);
			leftArm.GetComponent<SpriteRenderer>().sortingOrder = 2;
			switch(index) {
				//kicks
				case 7:
					leftArm.transform.localPosition = new Vector2(1,-3.5f);
					img = midPunchIMG1;
					attackColl.localPosition = new Vector2(1,-3.5f);
					destincyForPunch = new Vector2(-Time.deltaTime*punchSpeed,0);
					isActionProgress = true;
					leftArm.GetComponent<SpriteRenderer>().flipX = true;
					goingUp = false;
					break;
				case 10:
					leftArm.transform.localPosition = new Vector2(-2.25f,-3.5f);
					img = highPunchIMG;
					attackColl.localPosition = new Vector2(-2.25f,-3.5f);
					destincyForPunch = new Vector2(-Time.deltaTime*punchSpeed/3,Time.deltaTime*punchSpeed);
					isActionProgress = true;
					leftArm.GetComponent<SpriteRenderer>().flipX = false;
					leftArm.GetComponent<SpriteRenderer>().sortingOrder = 0;
					goingUp = true;
					break;
				//punches
				case 6:
					leftArm.transform.localPosition = new Vector2(0,0);
					img = midPunchIMG1;
					attackColl.localPosition = new Vector2(0,0);
					destincyForPunch = new Vector2(-Time.deltaTime*punchSpeed,0);
					isActionProgress = true;
					leftArm.GetComponent<SpriteRenderer>().flipX = true;
					goingUp = false;
					break;
				case 9:
					leftArm.transform.localPosition = new Vector2(-2.25f,0);
					img = highPunchIMG;
					attackColl.localPosition = new Vector2(-2.25f,0);
					destincyForPunch = new Vector2(-Time.deltaTime*punchSpeed/3,Time.deltaTime*punchSpeed);
					isActionProgress = true;
					leftArm.GetComponent<SpriteRenderer>().flipX = false;
					leftArm.GetComponent<SpriteRenderer>().sortingOrder = 0;
					goingUp = true;
					break;
				default:
					break;
			}
			/*if(index==6) {
				img = midPunchIMG1;
			}*/

			spriteSize = 0;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
  {
		if(col.gameObject.name=="attackCollP" && IsAI) {
			Debug.Log("ai has been punched22");
		} else if(col.gameObject.name=="attackColl" && !IsAI) {
			Debug.Log("player has been punched22");
		}
  }
	void OnTriggerEnter2D(Collider2D col)
  {
		Debug.Log("hitttted by"+col.gameObject.name+" vs "+col.gameObject.tag);
	}
}
