using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This class represents a group of cards and the functionality it has
public class CardGroup : MonoBehaviour {
	public string groupId;
	private BoxCollider collider;

	public List<Card> cards;
	public Card selectedCard;

    public float desiredScale;

    private GameController controller;
	private IEnumerator showCaseCorou;

    private bool isSelecting;
    private bool onSchedule;
    private double scheduledTime;
    private double showTime;

	void Awake (){
        isSelecting = false;
	    controller = GameObject.Find("GameController").GetComponent<GameController>();
		cards = new List<Card> ();
		collider = GetComponent<BoxCollider> ();
		for(int i = 0; i < transform.childCount; i++){
			cards.Add(transform.GetChild(i).GetComponent<Card>());
		}
		//cardWidth = 0.1f;
		selectedCard = cards [0];
	}

    void Update() { 
        if (isSelecting) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Select(selectedCard);
            }
        }
        if(onSchedule && AudioSettings.dspTime > scheduledTime) {
            onSchedule = false;
            ShowcaseGroup(0, showTime);
        }
    }

    //This is used to set everything up withid the card group. Like where cards should be placed when the group is in focus (clicked)
    public void PositionCards(float frustumWidth, float frustumHeight, float cardWidth){
        groupId = gameObject.name;
        //Reduce frustum width to narrow card placement
        frustumWidth = frustumWidth * 0.8f;
        //Margin and x-y spacing calculated (hardcoded to 4x3)
        float margin = cardWidth;
        float ySpacing = (frustumHeight - (2 * margin)) / 2f;
        float xSpacing = (frustumWidth - (2 * margin)) / 3f;
        int i = 0;
        for(int r = 0; r < 3; r++){
            var yPos = ySpacing - (r * ySpacing);
            for(int c = 0; c < 4; c++){
                if (i < cards.Count){
                    var xPos = (-xSpacing * 1.5f) + (c * xSpacing);
                    var currentCard = cards[i];
                    //Set start pos and position
                    currentCard.startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.005f * i);
                    currentCard.transform.position = currentCard.startPos;
                    //Set where to place on foldout
                    currentCard.toPlace = new Vector3 (
                        xPos, yPos * 1.1f,
                        -0.4f);

                    //set picture
                    currentCard.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture =
                        ResourceManager.GetImage(groupId[0], currentCard.name);

                    i++;
                }
            }
        }
        selectedCard.startPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 0.01f);
    }


    public string NameOfSelected(){
        return selectedCard.name;
    }

    //Select a card in the group from a name (could be clearer with a char as input)
	public void setSelected(string selection){
		foreach (var card in cards){
			if (card.name.Equals(selection)){
				SelectInstant(card);
				return;
			}
		}
	}

    //Reset all the cards start position
    public void setStartPosition(){
        for (int i = 0; i < cards.Count; i++){
            cards[i].startPos = new Vector3(transform.position.x, transform.position.y, 0 + 0.005f * i);
        }
        selectedCard.startPos = new Vector3 (transform.position.x, transform.position.y, 0 - 0.01f);
    }

    //Called when the group is clicked
	void OnMouseDown(){
		if (EventSystem.current.IsPointerOverGameObject()){
			return;
		}
	    if (GameController.Instance.state != GameState.IDLE){
	        return;
	    }
        isSelecting = true;
	    collider.enabled = false;
	    controller.StartSelecting();
		foreach(Card card in cards){
		    if (card != selectedCard){
		        card.moveOut(0.5f);
		    }
		    card.ToggleCollider();
		}
	    selectedCard.moveOut(0.5f);
	}

    //Used for showcase during playback
    private void ShowcaseGroup(double delay, double time){
	    showCaseCorou = ShowCoroutine((float) delay, (float) time);
        StartCoroutine(showCaseCorou);
    }

    //Schedule a showcase based on the Audio.dsp timeline (for audio sync)
    public void ScheduleShowcase(double timeline, double _showTime) {
        onSchedule = true;
        scheduledTime = timeline;
        showTime = _showTime;
    }

    //A coroutine describing the steps of presenting a card
    private IEnumerator ShowCoroutine(float delay,float time){
        var pos = transform.position;
        var scale = transform.localScale;
        var lerpIn = gameObject.AddComponent<PositionLerper>();
        //Scale lerper!
        var scaleIn = gameObject.AddComponent<ScaleLerper>();
        yield return new WaitForSeconds(delay);
        lerpIn.setUp(new Vector3(pos.x, pos.y, pos.z - 0.2f), 1.0f);
        scaleIn.setUp(new Vector3(scale.x * 1.2f, scale.y * 1.2f, 1), 1.0f);
        yield return new WaitForSeconds(time);
        lerpIn = gameObject.AddComponent<PositionLerper>();
        scaleIn = gameObject.AddComponent<ScaleLerper>();
        lerpIn.setUp(pos, 1.0f);
        scaleIn.setUp(scale, 1.0f);
    }

    //Select this card within the group (used when a card is clicked)
	public void Select(Card cardToSelect){
		selectedCard = cardToSelect;
	    setStartPosition();
		foreach(Card card in cards){
		    if (card != selectedCard){
		        card.moveIn(0.5f);
		    }
		}
	    selectedCard.moveIn(0.5f);
		collider.enabled = true;
	    controller.StopSelecting();
        controller.UpdateField();
        isSelecting = false;
	}

    //Position the cards at their given start position
	public void placeAtStart(){
		if (showCaseCorou != null){
            StopCoroutine(showCaseCorou);
		}
        onSchedule = false;
		Destroy(GetComponent<PositionLerper>());
        Destroy(GetComponent<ScaleLerper>());
        transform.localScale = new Vector3(desiredScale, desiredScale, 1);
		foreach (var card in cards){
            card.transform.position = card.startPos;
		}
	}

    //Select a card without moving anything around
	public void SelectInstant(Card cardToSelect){
		selectedCard = cardToSelect;
		setStartPosition();
		placeAtStart();
	}
}
