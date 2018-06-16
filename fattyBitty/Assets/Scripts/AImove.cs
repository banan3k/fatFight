using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;

public class AImove : MonoBehaviour {

	Transform mainHandler;
	Echo echoScript;

	bool moveCanBeloaded = false;
	int movingState = 0;

	public void startAImoves() {
		moveCanBeloaded = true;
	}

	// Use this for initialization
	void Start () {
		XL = YL = XR = YR = 0;
		trueXL = trueYL = trueXR = trueYL = 0;

		mainHandler = GameObject.Find("Main Camera").transform;
		echoScript = mainHandler.GetComponent<Echo>();
		Debug.Log("AAA");
	}

	float comboPassedTime = 0;
	float movePassedTime = 0;

	float XLreach = 0, YLreach = 0, XRreach = 0, YRreach = 0;

	/*async Task taskingCombo() {
		Debug.Log("A");
		chosenCombo = echoScript.getCombo();

		//Thread _thread = new Thread(echoScript.getCombo);
		//_thread.Start();
		if(chosenCombo!=null) {
			movingState = 2;
			comboPassedTime = Time.time;
			movePassedTime = Time.time;
			idMove = 0;
		} else {
			movingState = 1;
		}
	}*/
	bool lookingForCombo = false;
	bool canMakeCombo = false;
	public void foundCombo(bool isThere) {
		//Debug.Log(isThere+" isssss");
		if(isThere) {
			lookingForCombo = true;
			movingState = 3;
			Debug.Log("loaded move AI 3");
			//comboPassedTime = Time.time;
			//movePassedTime = Time.time;
			//idMove = 0;
		} else {
			movingState = 1;
		}
	}
	public void canMakeMoveAlready() {
		canMakeCombo = true;
		if(lookingForCombo) {
			foundCombo(true);
		} else {

		}
	}

	public InputControl.Combo chosenCombo = null;
	int idMove = 0;
	async void loadMove() {

		if(movingState==1) {
			//Debug.Log("c");
			//chosenCombo = (InputControl.Combo)(await echoScript.getCombo());
			movingState=2;
			//taskingCombo();
			Thread _thread = new Thread(echoScript.getCombo2);
			//Thread _thread = new Thread(()=>{Debug.Log("A");});//(echoScript.getCombo);
			//StartCoroutine(echoScript.getCombo2());
			_thread.Start();

			/*if(chosenCombo!=null) {
				movingState = 2;
				comboPassedTime = Time.time;
			  movePassedTime = Time.time;
				idMove = 0;
			}*/

		}
		if(movingState==3) {
			comboPassedTime = Time.time;
			movePassedTime = Time.time;
			idMove = 0;
			movingState=4;
			Debug.Log("loaded combo as "+chosenCombo);
		}
		if(movingState==4) {
			int movesCount = chosenCombo.Moves.Count;

			Debug.Log("ai start moving ("+movesCount+")");
			Debug.Log("ai whole duration: "+chosenCombo.totalDuration+"/"+chosenCombo.wholeTime());
			Debug.Log("ai this move duration ("+idMove+"): "+chosenCombo.Moves[idMove].duration);
			Debug.Log(chosenCombo.Moves[idMove].analogStateL);
			//Debug.Log(Time.time - comboPassedTime);
			float passedComboTime = Time.time - comboPassedTime;
			float passedMoveTime = Time.time - movePassedTime;
			Debug.Log("passed time for move "+idMove+" : "+passedMoveTime+"/"+chosenCombo.Moves[idMove].duration);
			if(passedMoveTime>=chosenCombo.Moves[idMove].duration) {
				if(idMove<movesCount) {
					idMove++;
					movePassedTime = Time.time;
					Debug.Log("new move in combo");
				} else {
					Debug.Log("all moves finished");
				}
			}

			if(passedComboTime>=chosenCombo.wholeTime()) {
				ParameterizedThreadStart start = new ParameterizedThreadStart(echoScript.changeOrder);
				Thread threadSaveNewCombo = new Thread(start);
				threadSaveNewCombo.Start(chosenCombo);
				moveCanBeloaded = true;
				movingState = 0;
				Debug.Log("combo finished "+passedComboTime);
			}
		}
	}



	float XL, YL, XR, YR;
	float trueXL, trueYL, trueXR, trueYR;
	float[] resolveState(InputControl.Move chosenMove) {
		XL = YL = XR = YR = 0;
		string stateL = chosenMove.analogStateL;
		string[] statesL = stateL.Split('.');
		Debug.Log("ai : "+statesL[0]+" vs "+statesL[1]+" duration: "+chosenMove.duration);
		int stateL1 = int.Parse(statesL[0]);
		int stateL2 = int.Parse(statesL[1]);

		switch(stateL1) {
			case 1:
					XL = 0.5f;
					YL = 1;
					break;
			case 2:
					XL = 0.5f;
					YL = 1;
					break;
			case 3:
					XL = 0.5f;
					YL = -1;
					break;
			case 4:
					XL = 1;
					YL = -0.5f;
					break;
			case 5:
					XL = -0.5f;
					YL = -1;
					break;
			case 6:
					XL = -1;
					YL = -0.5f;
					break;
			case 7:
					XL = -0.5f;
					YL = 1;
					break;
			case 8:
					XL = -1;
					YL = 0.5f;
					break;
			case 9:
				XL = 1;
				YL = 0;
				break;
			case 10:
				XL = 0;
				YL = 1;
				break;
			case 11:
				XL = 0;
				YL = -1;
				break;
			case 12:
				XL = -1;
				YL = 0;
				break;
			default:
				break;
		}

		float calcPower = ((float)(stateL2))/((float)(InputControl._howManyStrengthLevels));
		XL *= calcPower;
		YL *= calcPower;
		XR *= calcPower;
		YR *= calcPower;
		Debug.Log("left analog AI: "+XL+" vs "+YL);
		Debug.Log("right analog AI: "+XR+" vs "+YR);

		float timeToReach = chosenMove.duration;
		//float timeStepTemp = chosenMove.duration;
		//float howManySteps =
		/*stepsToReachXL = (XL - trueXL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachYL = (YL - trueYL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachXR = (XR - trueXR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachYR = (YR - trueYR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		Debug.Log("calculated steps: "+(XL - trueXL)+" and "+Time.deltaTime+"and "+Time.fixedDeltaTime+" and "+chosenMove.duration+" and "+timeToReach+" and "+stepsToReachXL+" vs "+stepsToReachYL+" vs "+stepsToReachXR+" vs "+stepsToReachYR);
*/
		return new float[4]{XL, YL, XR, YR};
	}


	// Update is called once per frame
	void Update () {
		if(moveCanBeloaded) {
			moveCanBeloaded = false;
			movingState = 1;
			Debug.Log("move can be loaded");

		}
		if(movingState!=0) {
			loadMove();
		}
	}
}
