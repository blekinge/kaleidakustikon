using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for controlling the UI behaviour of the play-button
public class PlayButton : MonoBehaviour{
    private ToolPanel panel;
    public Sprite StopSprite;
    private Sprite playSprite;
    public Text text;
    private Image image;
    //private Text text;

	// Use this for initialization
	void Start (){
        image = transform.Find("Image").GetComponent<Image>();
        playSprite = image.sprite;
	    panel = transform.parent.gameObject.GetComponent<ToolPanel>();
	}

    public void ClickPlay(){
        Action callBack = () => {
            image.sprite = playSprite;
            text.text = "Play";
        };
        if (GameController.Instance.Play(callBack)){
            image.sprite = StopSprite;
            text.text = "Stop";
        }
        else if(GameController.Instance.state == GameState.PLAYING) {
            Debug.Log("Trying to stop!");
            GameController.Instance.Stop();
            image.sprite = playSprite;
            text.text = "Play";
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
