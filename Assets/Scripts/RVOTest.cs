using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;

public class RVOTest : MonoBehaviour {

    public Actor[] agents;

	public bool running;
	Circle circle = new Circle();
	
	// Use this for initialization
	void Start ()
	{
		SetupScene();
	}
	
	// Update is called once per frame
	void Update () {
		if( running )
		{
			if( !circle.reachedGoal() )
			{
				Simulator.Instance.setTimeStep( Time.deltaTime );
				circle.setPreferredVelocities();
				Simulator.Instance.doStep();
			}
			running = false;
		}
	}

	void OnGUI()
	{
		if( !running && GUILayout.Button( "Run" ))
		{
			
			running = true;
		}
	}

	void SetupScene()
	{
		circle.setupScenario();
	}
}
