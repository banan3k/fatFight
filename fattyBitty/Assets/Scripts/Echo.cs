using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//data type
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using System.Collections.Specialized;

public class Echo : MonoBehaviour {

	AImove AImove;
	InputControl inputCon;

	[System.Serializable]
	public class rememberCombo
	{
		public string id = "";
		public int score = 0;
		public Situation situation;
		public rememberCombo(string id, int score, Situation sit) {
			this.id = id;
			this.score = score;
			situation = sit;
		}
		public rememberCombo(string id, int score) {
			this.id = id;
			this.score = score;
		}
	}

	[System.Serializable]
	public class Situation
	{
		public int distance = -1;
		public int lifeDiff = -1;
		public int angle = -1;
		public Situation(int distance=-1, int lifeDiff=-1, int angle=-1) {
			this.distance = distance;
			this.lifeDiff = lifeDiff;
			this.angle = angle;
		//	this.distance = actualSituation.distance;
		}
	}
	public Situation checkSituation() {
		Situation actualSit = new Situation(actualSituation.distance,
			actualSituation.lifeDiff,
			actualSituation.angle);
		return actualSit;
	}
	public Situation actualSituation;
	public Situation chosenSituationForRecord;

	//tutaj wiele wiecej sciezek do zapisu
	public void addComboToEcho(object newCombo) {
		rememberCombo comboFromInput = (rememberCombo)newCombo;
		/*string pathToSave="distance="+ comboFromInput.situation.distance + " and life="
			+ comboFromInput.situation.lifeDiff + " and angle=" + comboFromInput.situation.angle;

			for(int i=0; i<((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Count; i++) {
				if(comboFromInput.score>(int)((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"]))[i]) {
					((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Insert(i, comboFromInput.id, comboFromInput.score);
				}
			}
		*/
		Debug.Log("I added "+comboFromInput.id);
		Debug.Log(comboFromInput.situation.distance);

		//((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Add(comboFromInput.id, comboFromInput.score);

		/*string pathToSave="distance="+ comboFromInput.situation.distance + " or life="
			+ comboFromInput.situation.lifeDiff + " or angle=" + comboFromInput.situation.angle;

		int howManyRows = EchoRank.Select(pathToSave).Length;
		for(int i=0; i<howManyRows; i++){
			if(((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Count==0) {
				((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Add(comboFromInput.id, comboFromInput.score);
			} else {
				for(int i2=0; i2<((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Count; i2++) {
					if(comboFromInput.score>(int)((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"]))[i2]) {
						((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Insert(i2, comboFromInput.id, comboFromInput.score);
						break;
					}
				}
			}

		}*/

		string pathToSave="distance="+ comboFromInput.situation.distance + " or life="
			+ comboFromInput.situation.lifeDiff + " or angle=" + comboFromInput.situation.angle;

			Debug.Log("adding to rank starting");
		int howManyRows = EchoRank.Select(pathToSave).Length;
		Debug.Log(howManyRows);
		for(int i=0; i<howManyRows; i++){
			if(((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count==0) {
				((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Add(comboFromInput);
				Debug.Log("rank added into : "+((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0].id);
			} else {
				for(int i2=0; i2<((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count; i2++) {
					if(comboFromInput.score>(int)((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[i2].score) {
						((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Insert(i2, comboFromInput);
						Debug.Log("rank22 added into : "+((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0].id);
						break;
					}
				}
			}

		}

		if(!isThereAnyMove) {
			isThereAnyMove=true;
		}
	}
	public void changeOrder(object changedCombo) {
		rememberCombo comboToChange = (rememberCombo)changedCombo;

		string pathToSave="distance="+ chosenSituationForRecord.distance + " and life="
			+ chosenSituationForRecord.lifeDiff + " and angle=" + chosenSituationForRecord.angle;

		/*int oldScore = (int)((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Cast<DictionaryEntry>().ElementAt(0).Value;
		((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Remove(comboToChange.id);
		int howManyRows = ((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Count;
		for(int i=0; i<howManyRows; i++){
			//for(int i2=0; i2<((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Count; i2++) {
			if(comboToChange.score>(int)((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"]))[i]) {
				((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Insert(i, comboToChange.id, comboToChange.score);
				break;
			}
			//}
		}*/



		int howManyRows = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Count;
		for(int i=0; i<howManyRows; i++){
			if(comboToChange.score>(int)((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[i].score) {
				((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Insert(i, comboToChange);
				break;
			}
		}

	}
	public void getCombo2() {
		//yield return new WaitForSeconds(0);
		if(isThereAnyMove) {

			string pathToSave="distance="+ actualSituation.distance + " or life="
					+ actualSituation.lifeDiff + " or angle=" + actualSituation.angle;
			//DictionaryEntry chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Cast<DictionaryEntry>().ElementAt(0);
			rememberCombo chosenEntry = null;//((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0];
			/*if(((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0]!=null) {
	chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0];
				//InputControl.Combo chosenCombo =
				string chosenID = (string)chosenEntry.id;
				return GameObject.Find("Main Camera").GetComponent<InputControl>().everyCombo[chosenID];
			}*/
			int chosenSituation = -1;
			//Debug.Log("lolo "+((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Count);
			//Debug.Log(" first ID  :"+chosenEntry.id+" vs "+chosenEntry.score);
			int comboNumbers = EchoRank.Select(pathToSave).Length;


			for(int i=0; i<comboNumbers; i++) {
				Debug.Log(((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count);
				if(((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count>0 && (chosenEntry==null || (int)chosenEntry.score<(int)((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0].score)) {
					chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0];
					chosenSituation = i;
					Debug.Log(" going thru ID  :"+chosenEntry.id+" vs "+chosenEntry.score);
				}
			}
			if(chosenSituation>=0) {
				((List<rememberCombo>)(EchoRank.Select(pathToSave)[chosenSituation]["combo"])).RemoveAt(0);

				string chosenID = (string)chosenEntry.id;
				Debug.Log("chosen ID now :"+chosenID);
				InputControl.Combo chosenCombo = inputCon.everyCombo[chosenID];

				chosenSituationForRecord = actualSituation;

				AImove.chosenCombo = chosenCombo;
				Debug.Log("there isss");
				AImove.foundCombo(true);
				//return chosenCombo;
			} else {
				AImove.foundCombo(false);
			}
		} else {
			AImove.foundCombo(false);
		}
	}
	public InputControl.Combo getCombo() {
		if(!isThereAnyMove) {
			return null;
		}
	/*	string pathToSave="distance="+ actualSituation.distance + " and life="
			+ actualSituation.lifeDiff + " and angle=" + actualSituation.angle;
		DictionaryEntry chosenEntry = ((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Cast<DictionaryEntry>().ElementAt(0);
	//		string chosenID = (string)[0].Add();
		string chosenID = (string)chosenEntry.Key;
		InputControl.Combo chosenCombo = GameObject.Find("Main Camera").GetComponent<InputControl>().everyCombo[chosenID];
		return chosenCombo;*/

		/*string pathToSave="distance="+ actualSituation.distance + " or life="
				+ actualSituation.lifeDiff + " or angle=" + actualSituation.angle;
		DictionaryEntry chosenEntry = ((OrderedDictionary)(EchoRank.Select(pathToSave)[0]["echoCombo"])).Cast<DictionaryEntry>().ElementAt(0);
		for(int i=0; i<EchoRank.Select(pathToSave).Length; i++) {
			if((int)chosenEntry.Value<(int)((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Cast<DictionaryEntry>().ElementAt(0).Value) {
				chosenEntry = ((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["echoCombo"])).Cast<DictionaryEntry>().ElementAt(0);
			}
		}
		string chosenID = (string)chosenEntry.Key;
		InputControl.Combo chosenCombo = GameObject.Find("Main Camera").GetComponent<InputControl>().everyCombo[chosenID];

		chosenSituationForRecord = actualSituation;
*/



		string pathToSave="distance="+ actualSituation.distance + " or life="
				+ actualSituation.lifeDiff + " or angle=" + actualSituation.angle;
		//DictionaryEntry chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Cast<DictionaryEntry>().ElementAt(0);
		rememberCombo chosenEntry = null;//((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0];
		/*if(((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0]!=null) {
chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"]))[0];
			//InputControl.Combo chosenCombo =
			string chosenID = (string)chosenEntry.id;
			return GameObject.Find("Main Camera").GetComponent<InputControl>().everyCombo[chosenID];
		}*/
		int chosenSituation = -1;
		//Debug.Log("lolo "+((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Count);
		//Debug.Log(" first ID  :"+chosenEntry.id+" vs "+chosenEntry.score);
		int comboNumbers = EchoRank.Select(pathToSave).Length;


		for(int i=0; i<comboNumbers; i++) {
			Debug.Log(((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count);
			if(((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"])).Count>0 && (chosenEntry==null || (int)chosenEntry.score<(int)((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0].score)) {
				chosenEntry = ((List<rememberCombo>)(EchoRank.Select(pathToSave)[i]["combo"]))[0];
				chosenSituation = i;
				Debug.Log(" going thru ID  :"+chosenEntry.id+" vs "+chosenEntry.score);
			}
		}
		if(chosenSituation>=0) {
			((List<rememberCombo>)(EchoRank.Select(pathToSave)[chosenSituation]["combo"])).RemoveAt(0);

			string chosenID = (string)chosenEntry.id;
			Debug.Log("chosen ID now :"+chosenID);
			InputControl.Combo chosenCombo = GameObject.Find("Main Camera").GetComponent<InputControl>().everyCombo[chosenID];

			chosenSituationForRecord = actualSituation;

			return chosenCombo;
		} else {
			return null;
		}
	}

	public static float[] distanceSplit = new float[]{0.1f, 0.25f, 0.5f}; //unit distance
	public static int angleSplit = 30; //degrees
	public static int[] lifeSplit = new int[]{10, 25, 50, 75}; // out of 100

	bool listining = false;
	public DataTable EchoRank;// = new DataTable("EchoRank");
	bool isThereAnyMove = false;
	void Start () {
		AImove = GameObject.Find("Main Camera").GetComponent<AImove>();
		inputCon = GameObject.Find("Main Camera").GetComponent<InputControl>();
		//Debug.Log("lol");
		actualSituation = new Situation();
	//	EchoRank = GameObject.Find("Main Camera").GetComponent<InputControl>().loadAllRanks();
		if(true /*EchoRank==null*/) {
			EchoRank = new DataTable("EchoRank");
			EchoRank.Columns.Add("distance", typeof(int));
			EchoRank.Columns.Add("life", typeof(int));
			EchoRank.Columns.Add("angle", typeof(int));
			//EchoRank.Columns.Add("combo", typeof(OrderedDictionary));
			EchoRank.Columns.Add("combo", typeof(List<rememberCombo>));

		//	EchoRank.Rows.Add(0, 0, 0, new OrderedDictionary());
		//	EchoRank.Rows.Add(0, 0, 1, new OrderedDictionary());
		int temp = 0;
			for(int i=0; i<distanceSplit.Length; i++) {
				for(int i2=0; i2<lifeSplit.Length; i2++) {
					for(int i3=0; i3<=Mathf.Floor(360/angleSplit); i3++) {
						//EchoRank.Rows.Add(i, i2, i3, new OrderedDictionary());
//Debug.Log(i+" vs "+i2+" vs "+i3);
						/*
						string pathToSave="distance="+ i + " or life="
								+ i2 + " or angle=" + i3;
							Debug.Log(i+" vs "+i2+" vs "+i3);
						((OrderedDictionary)(EchoRank.Select(pathToSave)[i]["combo"])).Add((i*100+i2*10+i3, new rememberCombo("0", 1));
						*/
						//Debug.Log(temp);
						EchoRank.Rows.Add(i, i2, i3, new List<rememberCombo>());
						string pathToSave="distance="+ i + " and life="
								+ i2 + " and angle=" + i3;
							//Debug.Log(i+" vs "+i2+" vs "+i3);
						//((List<rememberCombo>)(EchoRank.Select(pathToSave)[0]["combo"])).Add(new rememberCombo(temp.ToString(), temp));
						//temp++;
					}
				}
			}
			//...
		} else {
			isThereAnyMove = true;
		}

		Avatar1 = GameObject.Find("Avatar1");
		Avatar2 = GameObject.Find("Avatar2");
		listining = true;

		AImove.startAImoves();
	}

	GameObject Avatar1, Avatar2;
	int defineDistanceState() {
		float defined = Vector2.Distance(Avatar1.transform.position, Avatar2.transform.position);
		for(int i=0; i<distanceSplit.Length; i++) {
			if(defined<=distanceSplit[i]) {
				return i;
			}
		}
		return distanceSplit.Length;
	}
	int defineAngleState() {
		int defined = (int)(Mathf.Floor(Quaternion.Angle(Avatar1.transform.rotation, Avatar2.transform.rotation)));
	//	defined = Mathf.Floor(defined/angleSplit);
		return defined;
	}
	int defineLifeState() {
		int defined = 1-1;
		return defined;
	}
	void Update () {
		if(listining) {
			//Debug.Log(actualSituation.distance+" vs "+actualSituation.lifeDiff+" vs "+actualSituation.angle);
			actualSituation.distance = defineDistanceState();
			actualSituation.lifeDiff = defineLifeState();
			actualSituation.angle = defineAngleState();
		}
	}
}
