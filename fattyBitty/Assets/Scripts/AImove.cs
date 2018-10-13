using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;

public class AImove : MonoBehaviour {

	Transform mainHandler;
	Echo echoScript;

	BodyControl bodyControl;

	bool moveCanBeloaded = false;
	int movingState = 0;

	public void startAImoves() {
		moveCanBeloaded = true;
	}

	// Use this for initialization
	void Start () {
		XL = YL = XR = YR = 0;
		trueXL = trueYL = trueXR = trueYR = 0;

		bodyControl = GameObject.Find("Avatar2").GetComponent<BodyControl>();

		mainHandler = GameObject.Find("Main Camera").transform;
		echoScript = mainHandler.GetComponent<Echo>();
	}

	float comboPassedTime = 0;
	float movePassedTime = 0;
	float actionPassedTime = 0;

	//float XLreach = 0, YLreach = 0, XRreach = 0, YRreach = 0;

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
	}*///const float actionTime = 0.5f; //half s.
	bool lookingForCombo = false;
	//bool canMakeCombo = false;
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
		//canMakeCombo = true;
		if(lookingForCombo) {
			foundCombo(true);
		} else {

		}
	}

	public InputControl.Combo chosenCombo = null;
	int idMove = 0;
	int movesCount = 0;

	public const float actionTime = 0.5f; //half s.

	float moveDurationFlag = 0;
	float actionTiming = 0;
	int actionIndex = 0;
	float resolveMoveTime(int index) {
		actionTiming = actionTime;
		actionIndex = 0;
		float tempTime = chosenCombo.Moves[index].duration;
		float actionMinTime = chosenCombo.Moves[index].actions.Count*actionTime;
		if(tempTime<actionMinTime) {
			return actionMinTime;
		}
		actionTiming = tempTime/actionTime;
		return tempTime;
	}


	void loadMove() {

		if(movingState==1) {
			resetAnalogs();
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
		//	Debug.Log("loaded combo as "+chosenCombo);
		//	Debug.Log("ai start moving ("+movesCount+")");
		//	Debug.Log("ai whole duration: "+chosenCombo.totalDuration+"/"+chosenCombo.wholeTime());
			movesCount = chosenCombo.Moves.Count;

			moveDurationFlag =  resolveMoveTime(idMove);
			if(chosenCombo.Moves[idMove].actions.Count>0) {
				bodyControl.MakeAction(chosenCombo.Moves[idMove].actions[actionIndex]);
			}
		}
		if(movingState==4) {


			resolveState(chosenCombo.Moves[idMove]);

	//		Debug.Log(chosenCombo.Moves[idMove].analogStateL);

		//	Debug.Log("actions: "+string.Join(",", chosenCombo.Moves[idMove].actions.ToArray()));
		//	Debug.Log("ai this move duration ("+idMove+"): "+chosenCombo.Moves[idMove].duration);

			//Debug.Log(Time.time - comboPassedTime);
			float passedComboTime = Time.time - comboPassedTime;
			float passedMoveTime = Time.time - movePassedTime;
			float passedActionTime = Time.time - actionPassedTime;
		//	Debug.Log("passed time for move "+idMove+" : "+passedMoveTime+"/"+chosenCombo.Moves[idMove].duration);
			//if(passedMoveTime>=chosenCombo.Moves[idMove].duration) {
			if(passedActionTime>=actionTiming && actionIndex<chosenCombo.Moves[idMove].actions.Count-1) {
				actionIndex++;
				//next action chosenCombo.Moves[idMove].actions[actionIndex];
				bodyControl.MakeAction(chosenCombo.Moves[idMove].actions[actionIndex]);
				actionPassedTime = Time.time;
			//	Debug.Log("next action by AI");
			}
			if(passedMoveTime>=moveDurationFlag) {
				if(idMove<movesCount-1) {
					idMove++;
					moveDurationFlag =  resolveMoveTime(idMove);
					movePassedTime = Time.time;
				//	Debug.Log("new move in combo");
				} else {
				//	Debug.Log("all moves finished");
				}
			}

			if(passedComboTime>=chosenCombo.wholeTime()) {
				ParameterizedThreadStart start = new ParameterizedThreadStart(echoScript.changeOrder);
				Thread threadSaveNewCombo = new Thread(start);
				threadSaveNewCombo.Start(chosenCombo);
				moveCanBeloaded = true;
				movingState = 0;
			//	Debug.Log("combo finished "+passedComboTime);
			}
		}
	}


	void resetAnalogs() {
		XL = YL = XR = YR = 0;
	}
	float XL, YL, XR, YR;
	float trueXL, trueYL, trueXR, trueYR;
	/*float[] */void resolveState(InputControl.Move chosenMove) {
		//resetAnalogs();
		string stateL = chosenMove.analogStateL;
		string[] statesL = stateL.Split('.');
		//Debug.Log("ai : "+statesL[0]+" vs "+statesL[1]+" duration: "+chosenMove.duration);
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
				XL = 0;
				YL = 0;
				break;
		}


		float calcPower = ((float)(stateL2))/((float)(InputControl._howManyStrengthLevels));
		XL *= calcPower;
		YL *= calcPower;





		string stateR = chosenMove.analogStateR;
		string[] statesR = stateR.Split('.');
		//Debug.Log("ai : "+statesR[0]+" vs "+statesR[1]+" duration: "+chosenMove.duration);
		int stateR1 = int.Parse(statesR[0]);
		int stateR2 = int.Parse(statesR[1]);

		switch(stateR1) {
			case 1:
				XR = 0.5f;
				YR = 1;
				break;
			case 2:
				XR = 0.5f;
				YR = 1;
				break;
			case 3:
				XR = 0.5f;
				YR = -1;
				break;
			case 4:
				XR = 1;
				YR = -0.5f;
				break;
			case 5:
				XR = -0.5f;
				YR = -1;
				break;
			case 6:
				XR = -1;
				YR = -0.5f;
				break;
			case 7:
				XR = -0.5f;
				YR = 1;
				break;
			case 8:
				XR = -1;
				YR = 0.5f;
				break;
			case 9:
				XR = 1;
				YR = 0;
				break;
			case 10:
				XR = 0;
				YR = 1;
				break;
			case 11:
				XR = 0;
				YR = -1;
				break;
			case 12:
				XR = -1;
				YR = 0;
				break;
			default:
				XR = 0;
				YR = 0;
				break;
		}


		calcPower = ((float)(stateR2))/((float)(InputControl._howManyStrengthLevels));
		XR *= calcPower;
		YR *= calcPower;
		//Debug.Log("left analog AI: "+XL+" vs "+YL);
		//Debug.Log("right analog AI: "+XR+" vs "+YR);

		//float timeToReach = chosenMove.duration;
		//float timeStepTemp = chosenMove.duration;
		//float howManySteps =
		/*stepsToReachXL = (XL - trueXL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachYL = (YL - trueYL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachXR = (XR - trueXR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		stepsToReachYR = (YR - trueYR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
		Debug.Log("calculated steps: "+(XL - trueXL)+" and "+Time.deltaTime+"and "+Time.fixedDeltaTime+" and "+chosenMove.duration+" and "+timeToReach+" and "+stepsToReachXL+" vs "+stepsToReachYL+" vs "+stepsToReachXR+" vs "+stepsToReachYR);
*/
		//return new float[4]{XL, YL, XR, YR};
	}

	public float[] AIanalogs() {
		return new float[4]{trueXL, trueYL, trueXR, trueYR};
	}

	void reachingAnalog() {
		if(trueXL!=XL) {
			if(trueXL<XL) {
				trueXL = Mathf.Clamp(trueXL+Time.deltaTime, trueXL, XL);
			} else {
				trueXL = Mathf.Clamp(trueXL-Time.deltaTime, XL, trueXL);
			}
		}
		if(trueYL!=YL) {
			if(trueYL<YL) {
				trueYL = Mathf.Clamp(trueYL+Time.deltaTime, trueYL, YL);
			} else {
				trueYL = Mathf.Clamp(trueYL-Time.deltaTime, YL, trueYL);
			}
		}


		if(trueXR!=XR) {
			if(trueXR<XR) {
				trueXR = Mathf.Clamp(trueXR+Time.deltaTime, trueXR, XR);
			} else {
				trueXR = Mathf.Clamp(trueXR-Time.deltaTime, XR, trueXR);
			}
		}
		if(trueYR!=YR) {
			if(trueYR<YR) {
				trueYR = Mathf.Clamp(trueYR+Time.deltaTime, trueYR, YR);
			} else {
				trueYR = Mathf.Clamp(trueYR-Time.deltaTime, YR, trueYR);
			}
		}
		//Debug.Log("AI analogs: "+trueXL+"/"+trueYL);
	}
	// Update is called once per frame
	//float temp = 0;
	void Update () {
		if(moveCanBeloaded) {
			moveCanBeloaded = false;
			movingState = 1;
			Debug.Log("move can be loaded");

		}
		if(movingState!=0) {
			loadMove();
		}
		reachingAnalog();
		//temp = Mathf.Clamp(temp+1, temp, 10);
		//Debug.Log("tttt: "+temp);
	}
}
