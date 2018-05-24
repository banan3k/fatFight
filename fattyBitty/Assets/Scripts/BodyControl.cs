using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour {

	const int _maxLife = 100;
	const float _startLifeForEyelid = 0.8f;
	const float _eyelidOpacity = 0.8f;
	const int _teethNumber = 4;
	int teethLevels = (int)(100/_teethNumber);
	int eyelidLife = (int)(_startLifeForEyelid * _maxLife);
	float eyelidStartingPosition;
	int lifeUp = _maxLife/3, lifeDown = _maxLife;

	Transform eyelid;
	Sprite[][] allMouth = new Sprite[5][];

	void Start () {
		eyelid = gameObject.transform.Find("top/eyes/eyelid");

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

	void controlMouth() {
		SpriteRenderer eyelidSprite = gameObject.transform.Find("top/mouth").GetComponent<SpriteRenderer>();
		int levelLifeUp = _teethNumber-(lifeUp/teethLevels);
		int levelLifeDown = _teethNumber-(lifeDown/teethLevels);
		//string mouthId = "lose"+levelLifeUp+"Up"+levelLifeDown+"Down";
		Debug.Log(levelLifeUp+" vs "+levelLifeDown);
		eyelidSprite.sprite = allMouth[levelLifeUp][levelLifeDown];
	}

	// Update is called once per frame
	void Update () {

		//	GameObject.Find("Avatar1 (1)").transform.Find("top").transform.Rotate(Vector3.forward * Time.deltaTime*8);
	}
}
