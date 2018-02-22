using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//This class is used to scale a gameobject over time
public class ScaleLerper : MonoBehaviour {

	private Vector3 goal;
	private bool ready;
	private float time;

	private Vector3 startScale;
	private float elapsedTime;

	private Action callback;


	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (ready)
		{
			elapsedTime += Time.deltaTime;
			var t = elapsedTime / time;
			//Make scaling non linear to give responsive feel
			t = -Mathf.Pow(1.0f - t, 4) + 1.0f;
			var lerpedScale = Vector3.Lerp(startScale, goal, t);
            transform.localScale = new Vector3(lerpedScale.x, lerpedScale.y, lerpedScale.z);
			if (elapsedTime > time)
			{
                transform.localScale = goal;
				if (callback != null)
				{
					callback();
				}
				Destroy(this);
			}
		}
	}

	//Setup the scaling with a callback action when the scaling is done
	public void setUp(Vector3 _goal, float _time, Action _callback)
	{
        startScale = transform.localScale;
		goal = _goal;
		time = _time;
		callback = _callback;
		ready = true;
	}
	
	//Setup the scaling witout a callback
	public void setUp(Vector3 _goal, float _time)
	{
        startScale = transform.localScale;
		goal = _goal;
		time = _time;
		callback = null;
		ready = true;
	}
}
