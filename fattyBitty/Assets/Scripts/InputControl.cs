using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for save and load files
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

using System.Collections.Specialized;

using System.Threading;

using System.Data;
using System.Data.SqlClient;

public class InputControl : MonoBehaviour {

	bool listining = false;
	const int _howManyStrengthLevels = 4;


	[System.Serializable]
	public class Combo
	{
		public List<Move> Moves = new List<Move>();
		public string id = "0";
		public float totalDuration = 0;
		public int numberOfMoves = 0;
		public int score = 0;
		public bool attack = false;
		public Combo() {
			id = generateID();
		//	situation = sit;
		//	situation = GameObject.Find("Main Camera").GetComponent<Echo>().checkSituation();
		}
		string generateID()
		{
			System.Guid myGUID = System.Guid.NewGuid();
			return myGUID.ToString();;
		}
		public void addMove(Move newMove) {
			this.numberOfMoves++;
			this.Moves.Add(newMove);
			this.totalDuration += newMove.duration;
		}
	}


	[System.Serializable]
	public class Move
	{
	    public string analogStateL = "0.0"; //x.y - x - angle, y - power
			public string analogStateR = "0.0";
			public string blockState = "0.0"; //x.y - x - left block, y - right block
	    public List<int> actions = new List<int>();
			public float duration=1;
			public bool style=false;
	    public Move(string state="0.0", string state2="0.0", string blocking="0.0", List<int> actionList=null, float howLong=1, bool whatStyle=false)
	    {
	        analogStateL = state;
					analogStateR = state2;
					blockState = blocking;
					if(actionList==null) {
						actions = new List<int>();
					}
					duration = howLong;
					style = whatStyle;
	    }
	}
	void saveToFile(Dictionary<string, Combo> comboToSave, bool createBackup=true) {
		if(createBackup) {
			if(File.Exists("AImemory.gg")) {
					try
					{
							if (!Directory.Exists("AI backup"))
							{
									Directory.CreateDirectory("AI backup");
							}
							File.Copy("AImemory.gg", "AI backup/AImemory.gg");
							Debug.Log("Backup has been done 1/2");
					}
					catch (IOException ex)
					{
						 Debug.Log(ex.Message);
					}
			}
		}

		Stream stream = File.Open("AImemory.gg", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Serialize(stream, comboToSave);
		stream.Close();
		Debug.Log("Moves saved");

		if(createBackup) {
			if(File.Exists("AIrank.gg")) {
					try
					{
							if (!Directory.Exists("AI backup"))
							{
									Directory.CreateDirectory("AI backup");
							}
							File.Copy("AIrank.gg", "AI backup/AIrank.gg");
							Debug.Log("Backup has been done 2/2");
					}
					catch (IOException ex)
					{
						 Debug.Log(ex.Message);
					}
			}
		}

		//string pathToSave="distance=3";
	//	Debug.Log(((OrderedDictionary)(loadedMoves.Select(pathToSave)[0]["echoCombo"]))[0]);



		stream = File.Open("AIrank.gg", FileMode.Create);
		bformatter = new BinaryFormatter();
		bformatter.Serialize(stream, GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank);
		stream.Close();
		Debug.Log("Ranks saved");

		List<List<Echo.rememberCombo>> allRanksToSave = new List<List<Echo.rememberCombo>>();
		string pathToSave="";
		int howMany = GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave).Length;
		for(int i=0; i<howMany; i++) {
			//allRanksToSave.Add((List<Echo.rememberCombo>)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[i]["combo"]));
			//Debug.Log(allRanksToSave[i].Count);
		}

		int temp = 0;
		for(int i=0; i<Echo.distanceSplit.Length; i++) {
			for(int i2=0; i2<Echo.lifeSplit.Length; i2++) {
				for(int i3=0; i3<=Mathf.Floor(360/Echo.angleSplit); i3++) {
					pathToSave="distance="+ i + " and life="
							+ i2 + " and angle=" + i3;
					allRanksToSave.Add((List<Echo.rememberCombo>)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["combo"]));
					//Debug.Log(((List<Echo.rememberCombo>)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["combo"])).Count);
					//temp++;
				}
			}
		}



		Debug.Log(allRanksToSave.Count);
		stream = File.Open("AIrank.gg", FileMode.Create);
		bformatter = new BinaryFormatter();
		bformatter.Serialize(stream, allRanksToSave);
		stream.Close();
		Debug.Log("Ranks saved");

		/* stream = File.Open("AIrank.gg", FileMode.Open);
		 bformatter = new BinaryFormatter();
		UnityEngine.Debug.Log("Loading variables");

		allRanksToSave = (List<OrderedDictionary>)(bformatter.Deserialize(stream));
		Debug.Log(allRanksToSave.Count);
	//	Debug.Log(((Echo.rememberCombo)(allRanksToSave[0][0])).id);



		pathToSave="";
		howMany = GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave).Length;
		for(int i=0; i<howMany; i++) {
			GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[i]["combo"] = allRanksToSave[i];
			//Debug.Log(allRanksToSave[i].Count);
		}*/



//((OrderedDictionary)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["echoCombo"])
		/*string pathToSave="*";
		Debug.Log(((OrderedDictionary)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["echoCombo"]))[0]);
			string json = JsonUtility.ToJson(((OrderedDictionary)(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["echoCombo"])));
			Debug.Log(json);
		for(int i=0; i<GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave).Count; i++) {
			stream = File.Open("AIrank+"i+".gg", FileMode.Create);
			bformatter = new BinaryFormatter();
			bformatter.Serialize(stream, GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[i]["echoCombo"]);
			stream.Close();
			Debug.Log("Ranks saved "+i+"/"+GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave).Count);
		}*/


	}
	public DataTable loadAllRanks()
	{
		if(File.Exists("AIrank.gg")) {
			DataTable loadedMoves = new DataTable("EchoRank");

			loadedMoves.Columns.Add("distance", typeof(int));
			loadedMoves.Columns.Add("life", typeof(int));
			loadedMoves.Columns.Add("angle", typeof(int));
			loadedMoves.Columns.Add("combo", typeof(List<Echo.rememberCombo>));
			for(int i=0; i<Echo.distanceSplit.Length; i++) {
				for(int i2=0; i2<Echo.lifeSplit.Length; i2++) {
					for(int i3=0; i3<=Mathf.Floor(360/Echo.angleSplit); i3++) {
						loadedMoves.Rows.Add(i, i2, i3, new List<Echo.rememberCombo>());
					}
				}
			}
			Debug.Log(Echo.distanceSplit);
	/*		Stream stream = File.Open("AIrank.gg", FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			UnityEngine.Debug.Log("Loading variables");

			return (OrderedDictionary)(bformatter.Deserialize(stream));

			loadedMoves = (DataTable)bformatter.Deserialize(stream);
			stream.Close();


			string pathToSave="distance=0 and life=0 and angle=0";
Debug.Log(((OrderedDictionary)(loadedMoves.Select(pathToSave)[0]["echoCombo"]))[0]);
*/

			Stream stream = File.Open("AIrank.gg", FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			UnityEngine.Debug.Log("Loading variables");
			List<List<Echo.rememberCombo>> allRanksToSave = new List<List<Echo.rememberCombo>>();


			allRanksToSave = (List<List<Echo.rememberCombo>>)(bformatter.Deserialize(stream));
			Debug.Log(allRanksToSave.Count);
			stream.Close();
			string pathToSave="";
			int howMany = GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave).Length;
			for(int i=0; i<howMany; i++) {
			 //Debug.Log(GameObject.Find("Main Camera").GetComponent<Echo>().EchoRank.Select(pathToSave)[0]["combo"]);// = allRanksToSave[i];
			 //loadedMoves.Select(pathToSave)[0]["combo"] = allRanksToSave[i];
			 //Debug.Log(allRanksToSave[i].Count);
			}

			int temp=0;
			for(int i=0; i<Echo.distanceSplit.Length; i++) {
				for(int i2=0; i2<Echo.lifeSplit.Length; i2++) {
					for(int i3=0; i3<=Mathf.Floor(360/Echo.angleSplit); i3++) {
						pathToSave="distance="+ i + " and life="
								+ i2 + " and angle=" + i3;
						loadedMoves.Select(pathToSave)[0]["combo"] = allRanksToSave[temp];
						Debug.Log(((Echo.rememberCombo)(allRanksToSave[temp][0])).score);
						temp++;
					}
				}
			}

			return loadedMoves;
		} else {
			return null;
		}
	}
	public Dictionary<string, Combo> loadAllMoves()
	{
		Dictionary<string, Combo> loadedMoves = new Dictionary<string, Combo>();
		Stream stream = File.Open("AImemory.gg", FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		UnityEngine.Debug.Log("Loading variables");
		loadedMoves = (Dictionary<string, Combo>)bformatter.Deserialize(stream);
	//	Debug.Log(loadedMoves[0].Moves[0].analogStateL+" vs "+loadedMoves[0].Moves[0].analogStateR);
		stream.Close();
/*
		Debug.Log(loadedMoves.id+" vs "+recordingCombo.totalDuration+" vs "+
			recordingCombo.numberOfMoves+" vs "+recordingCombo.score);
		Debug.Log(recordingCombo.Moves.Count+" vs "+recordingCombo.Moves[0].analogStateR);
*/

		return loadedMoves;
	}

	int defineStrength(float valueToCheck) {
		if(valueToCheck<0)
			valueToCheck*=-1;
		float strengthCheck=1/(float)_howManyStrengthLevels;
		for(int i=1; i<=_howManyStrengthLevels; i++){
			if(valueToCheck<=(i*strengthCheck)) {
				return i;
			}
		}
		return 0;
	}
	string defineState(float X, float Y) {
		string state = null;
		float strengthX = X==0 ? 0 : defineStrength(X);
		float strengthY = Y==0 ? 0 : defineStrength(Y);

		float strength = strengthX>strengthY ? strengthX : strengthY;
		int angle = 0;
		if(X+Y==0) {
			return "0.0";
		} else if(X>=0) {
			if(Y==0) {
				angle=2;
			} else if(Y>0){
				float mainDiff = X/Y;
				if(mainDiff>=0 && mainDiff<1) {
					angle=1;
				} else{
					angle=2;
				}
			} else {
				float mainDiff = X/-Y;
				if(mainDiff>=0 && mainDiff<1) {
					angle=4;
				} else{
					angle=3;
				}
			}
		} else {
			if(Y==0) {
				angle=6;
			} else {
				if(Y<0) {
					float mainDiff = X/Y;
					if(mainDiff>=0 && mainDiff<1) {
						angle=5;
					} else {
						angle=6;
					}
				} else {
					float mainDiff = X/-Y;
					if(mainDiff>=1) {
						angle=7;
					} else {
						angle=8;
					}
				}
			}
		}
		state = angle+"."+strength;
		return state;
	}

	List<Combo> allCombos;
	public Dictionary<string, Combo> everyCombo = new Dictionary<string, Combo>();
	Echo EchoScript;
	Echo.Situation recordingSituation;

	void Start () {
		EchoScript = GameObject.Find("Main Camera").GetComponent<Echo>();
		recordingSituation = new Echo.Situation();

		allCombos = new List<Combo>();
		listining = true;
		Debug.Log(Input.GetJoystickNames()[0]);
		//Move tempMove = new Move();
	//	Move tempMove2 = new Move();
		//Combo newCombo = new Combo();


	//	newCombo.Moves.Add(tempMove);
	//	newCombo.Moves.Add(tempMove2);
	/*	newCombo.addMove(tempMove);
		newCombo.addMove(tempMove2);
		allCombos.Add(newCombo);
		saveToFile(allCombos);
		loadAllMoves();*/
	}

	int secondsToSwitchCombo = 2;


	IEnumerator switchCombo()
  {
			changeState=2;
			yield return new WaitForSeconds(secondsToSwitchCombo);
			allCombos.Add(recordingCombo);

			everyCombo.Add(recordingCombo.id,recordingCombo);

			Echo.rememberCombo dataForEcho = new Echo.rememberCombo(recordingCombo.id, recordingCombo.score, recordingSituation);
			ParameterizedThreadStart start = new ParameterizedThreadStart(GameObject.Find("Main Camera").GetComponent<Echo>().addComboToEcho);
			Thread threadSaveNewCombo = new Thread(start);
			threadSaveNewCombo.Start(dataForEcho);
//			threadSaveNewCombo = new Thread(GameObject.Find("Main Camera").GetComponent<Echo>().addComboToEcho);
	//		threadSaveNewCombo.Start();

			changeState=0;

			Debug.Log(recordingCombo.id+" vs "+recordingCombo.totalDuration+" vs "+
				recordingCombo.numberOfMoves+" vs "+recordingCombo.score);
			Debug.Log(recordingCombo.Moves.Count+" vs "+recordingCombo.Moves[0].analogStateR);
	}
	int changeState = 0;
	Combo recordingCombo = new Combo();
	string currentStateL="", lastStateL="";
	string currentStateR="", lastStateR="";
	float calculatedMoveTime = 0;
	string currentBlock = "";

	float limitForFast = 0.1f;
	bool wasChangeFast(float diffX, float diffY) {
		if(diffX<0)
			diffX *= -1;
		if(diffY<0)
			diffY *= -1;
		if(diffX<limitForFast || diffY<limitForFast) {
			return false;
		} //else {
			//return true;
		//}
		return false;
	}

	public int pressedAction() {

		for (int i = 0;i < 20; i++) {
				if(Input.GetKeyDown("joystick 1 button "+i)){
					print("joystick 1 button "+i);
					return i;
				}
		}
		return -1;
	}

	float speedChangeNewXL = 0, speedChangeLastXL = 0;
	float speedChangeNewYL = 0, speedChangeLastYL = 0;
	float diffXL = 0, diffYL = 0;
	float speedChangeNewXR = 0, speedChangeLastXR = 0;
	float speedChangeNewYR = 0, speedChangeLastYR = 0;
	float diffXR = 0, diffYR = 0;
	float blockL = 0, blockR = 0;

	int clickedAction = -1;
	void Update () {

		if (Input.GetKeyDown (KeyCode.S)) {
		//	 saveToFile(allCombos);
			 saveToFile(everyCombo);
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			 loadAllMoves();
			 EchoScript.EchoRank = loadAllRanks();
		}

		//Debug.Log(Input.GetAxis("L2")+" vs "+Input.GetAxis("R2"));

		blockL = Input.GetAxis("L2");
		blockR = Input.GetAxis("R2");

		float valueXL = Input.GetAxis ("Horizontal");
		float valueYL = Input.GetAxis ("Vertical");

		float valueXR = Input.GetAxis ("Horizontal2");
		float valueYR = Input.GetAxis ("Vertical2");
		//Debug.Log(Input.GetAxis ("Horizontal2")+" vs "+Input.GetAxis ("Vertical2"));

		if(listining) {
			clickedAction = pressedAction();

			if(speedChangeNewXL != valueXL) {
				speedChangeLastXL = speedChangeNewXL;
				speedChangeNewXL = valueXL;
			}
			if(speedChangeNewYL != valueYL) {
				speedChangeLastYL = speedChangeNewYL;
				speedChangeNewYL = valueYL;
			}
			diffXL = speedChangeNewXL - speedChangeLastXL;
			diffYL = speedChangeNewYL - speedChangeLastYL;
			if(speedChangeNewXR != valueXR) {
				speedChangeLastXR = speedChangeNewXR;
				speedChangeNewXR = valueXR;
			}
			if(speedChangeNewYR != valueYR) {
				speedChangeLastYR = speedChangeNewYR;
				speedChangeNewYR = valueYR;
			}
			diffXR = speedChangeNewXR - speedChangeLastXR;
			diffYR = speedChangeNewYR - speedChangeLastYR;
			//Debug.Log(wasChangeFast(diffX, diffY));
			//Debug.Log(speedChangeNewX - speedChangeLastX);
			if(valueXL+valueYL!=0 || valueXR+valueYR!=0 || clickedAction>=0) {
					StopCoroutine("switchCombo");


					if(changeState==0) {

						//if(recordingCombo.Moves.Count>0) {
					//		Debug.Log("added");
						//	allCombos.Add(recordingCombo);
						//}
						recordingCombo = new Combo();
						recordingSituation = EchoScript.checkSituation();
					}// else {
					currentStateL = defineState(valueXL, valueYL);
					currentStateR = defineState(valueXR, valueYR);

					currentBlock = blockL+"."+blockR;

					if(recordingCombo.numberOfMoves==0 || (recordingCombo.numberOfMoves>0 && (currentBlock!=recordingCombo.Moves[recordingCombo.numberOfMoves-1].blockState
						|| currentStateL!=recordingCombo.Moves[recordingCombo.numberOfMoves-1].analogStateL
						|| currentStateR!=recordingCombo.Moves[recordingCombo.numberOfMoves-1].analogStateR))) {
						if(recordingCombo.numberOfMoves>0) {
							recordingCombo.Moves[recordingCombo.numberOfMoves-1].duration = Time.time - calculatedMoveTime;
							if(currentStateL!=lastStateL) {
								recordingCombo.Moves[recordingCombo.numberOfMoves-1].style = wasChangeFast(diffXL, diffYL);
							} else if(currentStateR!=lastStateR) {
								recordingCombo.Moves[recordingCombo.numberOfMoves-1].style = wasChangeFast(diffXR, diffYR);
							}
						}
						Move newMove = new Move(currentStateL, currentStateR, currentBlock);
						Debug.Log(newMove.analogStateL+" vs "+newMove.analogStateR);
						if(clickedAction>=0) {

						//	newMove.actions= new List<int>();
						//	Debug.Log(newMove.actions);
							newMove.actions.Add(clickedAction);
						}
						recordingCombo.addMove(newMove);

						if(currentStateL!=lastStateL) {
							currentStateL = lastStateL;
						}
						else if(currentStateR!=lastStateR) {
							currentStateR = lastStateR;
						}
						calculatedMoveTime = Time.time;
					} else if(clickedAction>=0){
						recordingCombo.Moves[recordingCombo.numberOfMoves-1].actions.Add(clickedAction);
					}
				//	}
					changeState=1;
			//	Debug.Log(valueXL+" vs "+valueYL+" vs "+defineState(valueXL, valueYL));
			} else {
				if(changeState==1) {
					StartCoroutine(switchCombo());
				}
			}
		}
	}
}
