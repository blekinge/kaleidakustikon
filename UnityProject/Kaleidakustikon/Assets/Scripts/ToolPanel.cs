using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToolState{
    SHOWING,
    HIDING
}

//Main class for the tool panel that contains all buttons
public class ToolPanel : MonoBehaviour{
    public float targetYPos;
    public float slideTime;
    public ToolState state = ToolState.HIDING;
    private float elapsedTime;
    private RectTransform rectTransform;

    private bool active;

    void Start(){
        rectTransform = GetComponent<RectTransform>();
    }

    public void ToggleTools(){
        if (state == ToolState.HIDING) {
            deleteLerper();
            PositionLerper posLerper = gameObject.AddComponent<PositionLerper>();
            posLerper.setUp( transform.TransformPoint(new Vector3(0, targetYPos, 0)), slideTime);
            //rectTransform.anchoredPosition = new Vector3(0, targetYPos, 0);
            state = ToolState.SHOWING;
        } else if (state == ToolState.SHOWING){
            //rectTransform.anchoredPosition = new Vector3(0, 0, 0);
            deleteLerper();
            PositionLerper posLerper = gameObject.AddComponent<PositionLerper>();
            posLerper.setUp(transform.TransformPoint(new Vector3(0, -targetYPos, 0)), slideTime);
            state = ToolState.HIDING;
        }
    }

	//Remove component that causes movement
    private void deleteLerper() {
        Destroy(gameObject.GetComponent<PositionLerper>());
    }
	
    public void setInputField(string input){
        var field = transform.Find("InputField").GetComponent<InputField>();
        field.text = input;
    }

    public void Activate() {
        active = true;
        var selectables = GetComponentsInChildren<Selectable>();
        foreach (Selectable s in selectables) {
            if (!s.gameObject.name.Equals("PlayButton") && !s.gameObject.name.Equals("show/hide")) {
                s.interactable = true;
            }
        }
    }

    public void Deactivate() {
        active = false;
        var selectables = GetComponentsInChildren<Selectable>();
        foreach (Selectable s in selectables) {
            if (!s.gameObject.name.Equals("PlayButton") && !s.gameObject.name.Equals("show/hide")) {
                s.interactable = false;
            }
        }
    }

    public bool isActive() {
        return active;
    }
}
