using UnityEngine;

//Different states of a card for positioning
public enum CardState {
	HIDDEN,
	SHOWN,
	MOVING
}

//This class represents an individual card
public class Card : MonoBehaviour
{

    public string name;

	public CardState state;
	public Vector3 toPlace;
	private BoxCollider collider;
	private CardGroup group;
	public Vector3 startPos;

	void Start () {
		state = CardState.HIDDEN;
		collider = GetComponent<BoxCollider> ();
		group = transform.parent.gameObject.GetComponent<CardGroup> ();
		collider.enabled = false;
	}

    //Adds a component that moves the card out in the time specified
	public void moveOut(float time){
		var lerper = gameObject.AddComponent<PositionLerper> ();
		lerper.setUp (toPlace, time, () => {
			state = CardState.SHOWN;
		});
		state = CardState.MOVING;
		gameObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Selection");
	}

	//Adds a component that moves the card in in the time specified
	public void moveIn(float time){
		var lerper = gameObject.AddComponent<PositionLerper> ();
		lerper.setUp (startPos, time, () => {
			state = CardState.HIDDEN;
			gameObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
		});
		state = CardState.MOVING;
		collider.enabled = false;
	}

    //Toggle whether the card should receive clicked input
    public void ToggleCollider(){
        collider.enabled = !collider.enabled;
    }

    //Called when the card is clicked
	void OnMouseDown(){
	    if (state == CardState.HIDDEN) return;
	    if (GetComponent<PositionLerper>() != null){
	        Destroy(GetComponent<PositionLerper>());
	    }
	    group.Select (this);
	}
}
