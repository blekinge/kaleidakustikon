using System;
using UnityEngine;

//Class for moving a GameObject to a position over time. 
public class PositionLerper : MonoBehaviour {

	private Vector3 goal;
	private bool ready;
	private float time;

	private Vector3 startPos;
	private float elapsedTime;

	private Action callback;
	
	// Update is called once per frame
	void Update () {
		if (ready) {
			elapsedTime += Time.deltaTime;
			var t = elapsedTime / time;
			//Make movement non-linear to give more responsive feel
		    t = - Mathf.Pow(1.0f - t, 4) + 1.0f;
			var lerpedPos = Vector3.Lerp (startPos, goal, t);
			transform.position = new Vector3(lerpedPos.x, lerpedPos.y, lerpedPos.z);
			if (elapsedTime > time) {
				transform.position = goal;
			    if (callback != null){
			        callback ();
			    }
				Destroy (this);
			}
		}
	}
	
	//Setup the movement. Takes an action to perform one movement is done
	public void setUp(Vector3 _goal, float _time, Action _callback){
		startPos = transform.position;
		goal = _goal;
		time = _time;
		callback = _callback;
		ready = true;
	}

	//Setuo function without callback
    public void setUp(Vector3 _goal, float _time){
        startPos = transform.position;
        goal = _goal;
        time = _time;
        callback = null;
        ready = true;
    }
}
