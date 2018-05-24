using UnityEngine;
using System.Collections;
using System.Threading;

public class test1 : MonoBehaviour
{

    public float timeWaiting = 5000000.0f;
    public string labelInitialText = "I`m the console here!";



    private string _label;
    private Thread _t1;
    private Thread _t2;
		private Thread _t3;
    private bool _t1Paused = false;
    private bool _t2Paused = false;

		int[] testowanie = new int[3];


    void Start () {
    /*    _label = labelInitialText;
        _t1 = new Thread(_func1);
        _t2 = new Thread(_func2);


				_t1.Start();
				_t2.Start();*/
    }
		void Update() {
		//	Debug.Log(testowanie[0]+" vs "+testowanie[1]+" vs ");
		}


    private void _func1()
    {
        while(!_t1Paused)
        {
          testowanie[0] = 1;
        }
				testowanie[0] = 2;
    }

    private void _func2()
    {
testowanie[1] = 5;
			while(!_t2Paused)
			{
				testowanie[0] = 3;

			}
			testowanie[0] = 4;
    }

    void OnGUI()
    {
        //--> Label that servers as a "console"
        GUI.Label(new Rect(0,0, 500, 500), _label);

        //--> Button for thread 1
        if(GUI.Button(new Rect(50, 50, 100, 50), "Thread T1"))
        {
            //if(!_t1.IsAlive)
            //    _t1.Start();
            //else
                _t1Paused = !_t1Paused;

        }

        //--> Button for thread 2
        if(GUI.Button(new Rect(50, 120, 100, 50), "Thread T2"))
        {
            //if(!_t2.IsAlive)
            //    _t2.Start();
            //else
                _t2Paused = !_t2Paused;
        }
    }

}
