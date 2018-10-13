using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class Menu : MonoBehaviour {

	//SpriteRenderer menuBackground;
	//int pointAtButton = 0;
	void Start () {
		//menuBackground = GameObject.Find("MenuObj").GetComponent<SpriteRenderer>();
		fileExplorerWidth = (int)(Screen.width * 0.35f);

		buttonMarginTop = (int)(Screen.height*0.25f);
		menuButtonHeight = (int)(Screen.height*0.1f);
		menuButtonWidth = (int)(Screen.width*0.2f);
		buttonGap = (int)(Screen.height*0.05f);
	}

	int menuButtonWidth = 0, menuButtonHeight=0;
	int buttonMarginTop = 0;
	int buttonGap = 0;

	bool menuOn = false;
	bool optionsOn = false;
	bool filesLoaded = false;

	int fileExplorerWidth;
	Vector2 scrollPosition = Vector2.zero;

	public Sprite BackgroundPic;
	public AudioClip FightMusic;
	void changeBackgroundPic(string fileName) {
		BackgroundPic = Resources.Load("gamePics/"+fileName) as Sprite;
		GameObject.Find("MenuObj").GetComponent<SpriteRenderer>().sprite = BackgroundPic;
		//gameObject.GetComponent<SpriteRenderer>().sprite = BackgroundPic;
	}
	void changeMusic(string fileName) {
		/*if(isFightMusic) {
			//gameObject.GetComponent<AudioSource>().AudioClip = FightMusic;
		} else {
			//gameObject.GetComponent<AudioSource>().AudioClip = MenuMusic;
		}*/
		AudioSource audio = gameObject.GetComponent<AudioSource>();
		FightMusic = Resources.Load("gameMusic/"+fileName) as AudioClip;
		audio.Stop();
		audio.clip = FightMusic;
		audio.Play();
	}

	int scrollHeight = 0;
	void OnGUI() {
		if(menuOn) {
			//Debug.Log("AAA");
			GUI.backgroundColor = Color.black;
			//GUI.BanchoredPosition = new Vector2(0,0);
			//GUI.color = new Color(1,1,1,1);
			//GUI.Box(new Rect(0, 0, Screen.width, Screen.height), menuTexture);
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			if(optionsOn) {
				if(filesLoaded) {
					scrollPosition = GUI.BeginScrollView(new Rect(250+menuButtonWidth, 0, fileExplorerWidth, Screen.height), scrollPosition, new Rect(0, 0, fileExplorerWidth, scrollHeight));
					//scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(100), GUILayout.Height(100));
					for(int i=0; i<fileNames.Count; i++) {
						if (GUI.Button(new Rect(0,menuButtonHeight/2+i*menuButtonHeight, fileExplorerWidth, menuButtonHeight), fileNames[i]))
						{
							if(changingMusic) {
								changeMusic(fileNames[i]);
							} else {
								changeBackgroundPic(fileNames[i]);
							}
						}
					}
					GUI.EndScrollView();
				}
				if (GUI.Button(new Rect(Screen.width*0.25f,menuButtonHeight/2+70, menuButtonWidth, menuButtonHeight), "Change background pic"))
		    {
					changingMusic = false;
					listingAllFiles("gamePics");
		    }
				if (GUI.Button(new Rect(Screen.width*0.25f,buttonGap+menuButtonHeight/2+menuButtonHeight+70, menuButtonWidth, menuButtonHeight), "Change in game music"))
		    {
					changingMusic = true;
					listingAllFiles("gameMusic");
		    }
				/*if (GUI.Button(new Rect(0,buttonGap*2+menuButtonHeight*2, menuButtonWidth*2, menuButtonHeight), "Change menu music"))
		    {
					listingAllFiles("menuMusic");
		    }*/
				if (GUI.Button(new Rect(Screen.width*0.25f,buttonGap*2+menuButtonHeight/2+menuButtonHeight*2+70, menuButtonWidth, menuButtonHeight), "Back"))
		    {
					optionsOn = false;
					filesLoaded = false;
		    }
			} else {
				if (GUI.Button(new Rect(Screen.width/2-menuButtonWidth/2, buttonMarginTop, menuButtonWidth, menuButtonHeight), "Resume"))
		    {
					bringUpMenuFromGame(false);
		    }
				if (GUI.Button(new Rect(Screen.width/2-menuButtonWidth/2, buttonMarginTop+buttonGap+menuButtonHeight, menuButtonWidth, menuButtonHeight), "Options"))
				{
					optionsOn = true;
				}
				if (GUI.Button(new Rect(Screen.width/2-menuButtonWidth/2,buttonMarginTop+buttonGap*2+menuButtonHeight*2, menuButtonWidth, menuButtonHeight), "Quit"))
		    {
					Application.Quit();
		    }
				if (GUI.Button(new Rect(Screen.width/2-menuButtonWidth/2,buttonMarginTop+buttonGap*3+menuButtonHeight*3, menuButtonWidth, menuButtonHeight), "Save & Quit"))
		    {
					GameObject.Find("Main Camera").GetComponent<InputControl>().saveToFile(null);
					Application.Quit();
		    }
			}
		}
	}

	bool changingMusic = false;
	List<string> fileNames;
	void listingAllFiles(string fromWhere) {
		filesLoaded = false;
		fileNames = new List<string>();
		DirectoryInfo dir = new DirectoryInfo ("Assets/Resources/"+fromWhere);
		FileInfo[] info = dir.GetFiles ("*.*");
		if(changingMusic) {
			foreach (FileInfo file in info) {
				if (file.Extension == ".mp3") {
					fileNames.Add(file.Name);
				}
			}
		} else {
			foreach (FileInfo file in info) {
				if (file.Extension == ".png" || file.Extension == ".jpg") {
					fileNames.Add(file.Name);
				}
			}
		}
		scrollHeight = fileNames.Count * menuButtonHeight;
		filesLoaded = true;
		//return fileNames;
	}

	public void bringUpMenuFromGame(bool turnOn = true) {
		if(turnOn && Time.timeScale == 0) {
			turnOn = false;
		}
		Time.timeScale = turnOn ? 0 : 1;
		menuOn = turnOn;
		//menuBackground.enabled = false;//turnOn;
		optionsOn = false;
		//pointAtButton = 0;
		filesLoaded = false;

	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown("joystick 1 button 4")) {
			 bringUpMenuFromGame();
		}
	}
}
