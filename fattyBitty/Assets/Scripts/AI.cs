using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;

public class AI : MonoBehaviour {

	Transform mainHandler;
	Echo echoScript;



	// Use this for initialization
	void Start () {
		trueXL = trueYL = trueXR = trueYR = 0;
		stepsToReachXL = stepsToReachYL = stepsToReachXR = stepsToReachYR = 0;

		mainHandler = GameObject.Find("Main Camera").transform;
		echoScript = mainHandler.GetComponent<Echo>();
	}

	/*InputControl.Combo*/void loadMove() {
		Debug.Log("ai start");
		InputControl.Combo chosenCombo = echoScript.getCombo();

		//ParameterizedThreadStart start = new ParameterizedThreadStart(resolveState);
		//Thread threadSaveNewCombo = new Thread(start);
		//threadSaveNewCombo.Start(chosenCombo);
		StartCoroutine(resolveState(chosenCombo));
		//resolveState(chosenCombo);
	}



	float XL, YL, XR, YR;
	IEnumerator resolveState(InputControl.Combo chosenCombo) {
		Debug.Log("AI combo ID: "+chosenCombo.id);
		Debug.Log(chosenCombo.Moves.Count+" last: "+chosenCombo.totalDuration);
		Debug.Log(chosenCombo.Moves[0].analogStateL+" vs "+chosenCombo.Moves[0].analogStateR);


		for(int i=0; i<chosenCombo.Moves.Count; i++) {



			XL = YL = XR = YR = 0;
			string stateL = chosenCombo.Moves[i].analogStateL;
			string[] statesL = stateL.Split('.');
			Debug.Log(statesL[0]+" vs "+statesL[1]+" duration: "+chosenCombo.Moves[i].duration);
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
			/*if(stateL1==10) {
				//X is bigger on -
				XL = -1;
				YL = 0;
			}*/
			float calcPower = ((float)(stateL2))/((float)(InputControl._howManyStrengthLevels));
			XL *= calcPower;
			YL *= calcPower;
			XR *= calcPower;
			YR *= calcPower;
			Debug.Log("left analog AI: "+XL+" vs "+YL);
			Debug.Log("right analog AI: "+XR+" vs "+YR);

			timeToReach = chosenCombo.Moves[i].duration;
			float timeStepTemp = chosenCombo.Moves[i].duration;
			//float howManySteps =
			stepsToReachXL = (XL - trueXL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
			stepsToReachYL = (YL - trueYL)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
			stepsToReachXR = (XR - trueXR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
			stepsToReachYR = (YR - trueYR)/Mathf.Ceil(timeToReach/Time.fixedDeltaTime);
			Debug.Log("calculated steps: "+(XL - trueXL)+" and "+Time.deltaTime+"and "+Time.fixedDeltaTime+" and "+chosenCombo.Moves[i].duration+" and "+timeToReach+" and "+stepsToReachXL+" vs "+stepsToReachYL+" vs "+stepsToReachXR+" vs "+stepsToReachYR);

			yield return new WaitForSeconds(timeStepTemp);//chosenCombo.Moves[i].duration);

		}

		//Echo.rememberCombo dataForEcho = new Echo.rememberCombo(recordingCombo.id, recordingCombo.score, recordingSituation);
		ParameterizedThreadStart start = new ParameterizedThreadStart(GameObject.Find("Main Camera").GetComponent<Echo>().changeOrder);
		Thread threadSaveNewCombo = new Thread(start);
		threadSaveNewCombo.Start(chosenCombo);
yield return new WaitForSeconds(3000);
		loadMove();
	}

	float timeToReach = 0;
	float stepsToReachXL, stepsToReachYL, stepsToReachXR, stepsToReachYR;
	float trueXL, trueYL, trueXR, trueYR;
	public float[] reachAnalogs() {
		//float[] AIanalogs = new float[4];
		if(trueXL!=XL) {
			trueXL += stepsToReachXL/10;//*Time.fixedDeltaTime;
		}
		if(trueYL!=YL) {
			trueYL += stepsToReachYL/10;//*Time.fixedDeltaTime;
		}
		if(trueXR!=XR) {
			trueXR += stepsToReachXR/10;//*Time.fixedDeltaTime;
		}
		if(trueYR!=YR) {
			trueYR += stepsToReachYR/10;//*Time.fixedDeltaTime;
		}
		Debug.Log("stepping: "+stepsToReachXL+" and "+stepsToReachXL*Time.deltaTime);
		//Debug.Log("true: "+trueXL+" vs "+ trueYL+" vs "+trueXR+" vs "+trueYR);
		//Debug.Log();
		return new float[4]{trueXL, trueYL, trueXR, trueYR};
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.M)) {
			 loadMove();
		}
		//reachAnalogs();
	}
}
