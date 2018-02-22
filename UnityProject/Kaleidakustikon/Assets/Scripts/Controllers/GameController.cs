using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

public enum GameState
{
    IDLE,
    PLAYING,
    SELECTING
}

public class GameController : MonoBehaviour
{
    //Declaration of external functions described in plugin file
    [DllImport("__Internal")]
    private static extern int ExtOpenMei(String comp);
    [DllImport("__Internal")]
    private static extern int ExtOpenMidi(String comp);
    [DllImport("__Internal")]
    private static extern int ExtPrintSheet(String comp);
    [DllImport("__Internal")]
    private static extern int ExtPrintImages(String comp);
    [DllImport("__Internal")]
    private static extern string ExtGetUrlComp();
    [DllImport("__Internal")]
    private static extern int ExtSetUrl(String comb);

    public GameState state = GameState.IDLE;
    public GameObject BaseGroup;
    //public GameObject RowBg;
    public List<string> GroupIds;
    public CardPlayer Player;
    public ToolPanel toolPanel;
    public GameObject LoadingPause;
    public LightFader bgLight;
    public LightFader selectionLight;
    private GameObject[] Groups;
    private IEnumerator playingCorotine;
    public static GameController Instance;

    private bool runningWebgl;

    //If audio files have been swapped with new tempo. Change tempo here:
    private float clipTempo = 140;

    void Start()
    {
        Groups = new GameObject[GroupIds.Count];
        InstantiateCard();
        positionEverything();
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        var urlComb = "";

        //Used for webGL environment specific calls (like calls to external javascript)
        runningWebgl = true;

#if UNITY_EDITOR
        runningWebgl = false;
#endif
        /*
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
*/

        if(runningWebgl) {
            urlComb = ExtGetUrlComp();
        }

        if (urlComb.Length > 0)
        {
            setSelection(urlComb);
        }
        else
        {
            setSelection("222222222222222222222");
        }
    }

    //Load audio based on selection and start coroutine
    public bool Play(Action callBack)
    {
        if (state == GameState.IDLE)
        {
            Player.LoadAudio(GetSelectionList());
            playingCorotine = PlayCoroutine(() => {
                callBack();
                toolPanel.Activate();
            });
            StartCoroutine(playingCorotine);
            toolPanel.Deactivate();
            return true;
        }
        return false;
    }

    public void PauseGame() {
        LoadingPause.SetActive(true);
    }

    public void UnPauseGame() {
        LoadingPause.SetActive(false);
    }

    public void OpenMidi()
    {
        string selectionString = GetSelectionString();
        if(runningWebgl) {
			ExtOpenMidi(selectionString);
		}
    }

    public void OpenMei(){
        string selectionString = GetSelectionString();
        if(runningWebgl) {
            ExtOpenMei(selectionString);
        }
    }

    public void PrintSheet() {
        PauseGame();
        string selectionString = GetSelectionString();
        if(runningWebgl) {
            ExtPrintSheet(selectionString);
        } else {
            UnPauseGame();
        }
    }

    public void PrintImages() {
        PauseGame();
        string selectionString = GetSelectionString();
        if(runningWebgl) {
            ExtPrintImages(selectionString);
        } else {
            UnPauseGame();
        }
    }

    public void Stop(){
        if (state == GameState.PLAYING && playingCorotine != null){
            StopCoroutine(playingCorotine);
            Player.Stop();
            state = GameState.IDLE;
            playingCorotine = null;
            foreach (var group in Groups.Select(c => c.GetComponent<CardGroup>())){
                group.placeAtStart();
            }
            toolPanel.Activate();
        }
    }

    //Return the IEnumerator describing how a selection should be played, visuals and audio.
    private IEnumerator PlayCoroutine(Action callBack){
        string[] form = new[]{"A", "A", "B", "B", "A", "C", "C", "A"};
        //The cards have 140 bpm and measures are 3 beats long
        Double clipLength = 60.0/clipTempo * 3.0;
        //The selection of the current board as a string
        string toPlay = GetSelectionString();
        state = GameState.PLAYING;
        int currentClip = 0;
        int currentPart = 0;

        //Delay of a second
        double timeLine = AudioSettings.dspTime + 1.0;

        while (currentClip < toPlay.Length){
            double playTime = clipLength;

            if ((currentClip % 7) == 0){ //Start of part, add extra time
                playTime += clipLength;
            }
            //Schedule play:
            Player.PlayClip(currentClip, timeLine);
            Groups[currentClip].GetComponent<CardGroup>().ScheduleShowcase(timeLine - 0.4, playTime);
            timeLine += playTime;

            yield return new WaitForSeconds((float) playTime);

            if ((currentClip % 7) == 6){ //End of part
                currentPart++;
                if (currentPart < form.Length){
                    switch (form[currentPart]){
                        case "A":
                            currentClip = 0;
                            break;
                        case "B":
                            currentClip = 7;
                            break;
                        case "C":
                            currentClip = 14;
                            break;
                    }
                }
                else{
                    //It is over. Ring out
                    yield return new WaitForSeconds((float)clipLength);
                    break;
                }
            }
            else{
                currentClip++;
            }
        }
        state = GameState.IDLE;
        callBack();
    }

    void InstantiateCard(){
        for (int i = 0; i < GroupIds.Count; i++){
            Groups[i] = Instantiate(BaseGroup);
            Groups[i].name = GroupIds[i];
        }
    }

    public void StartSelecting(){
        state = GameState.SELECTING;
        bgLight.fadeIntensity(0.5f, 0.5f);
    }

    public void StopSelecting(){
        state = GameState.IDLE;
        bgLight.fadeIntensity(1.4f, 0.5f);
    }

    //This function is used to place everything at the correct position in the world based on screen resolution and camera
    void positionEverything(){
        var width = (float) Screen.width;
        var newheight = (int) (width * 0.5f);
        Screen.SetResolution((int) width, newheight, false);


        var uiPart = toolPanel.GetComponent<RectTransform>().sizeDelta.y / newheight;
        var cam = GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ();
        cam.rect = new Rect(new Vector2(0, uiPart), Vector2.one);
        var distance = transform.position.z - cam.transform.position.z;
        var frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * cam.aspect;
        var cardWidth = 0.25f * frustumHeight;
        var xSpacing = cardWidth / 70;
        var ySpacing = xSpacing * 19;
        var margin = frustumWidth / 10;

        var xSpace = (frustumWidth - margin * 2) / 6;
        var ySpace = (frustumHeight - (margin * 0.7f) * 2) / 2;

        var i = 0;
        for (var r = 0; r < 3; r++){
            var yPos = (cardWidth + ySpacing) * (1 - r);

            for (var c = 0; c < 7; c++){
                if (i >= Groups.Length){
                    continue;
                }
                var cardGroup = Groups[i];
                var component = cardGroup.GetComponent<CardGroup>();
                component.desiredScale = cardWidth;
                var xPos = (cardWidth + xSpacing) * (c - 3);

                cardGroup.transform.localScale = new Vector3(cardWidth, cardWidth, 1.0f);
                cardGroup.transform.position = new Vector3(
                        xPos,
                        yPos, 0
                    );
                component.PositionCards(frustumWidth, frustumHeight, cardWidth);
                i++;
            }
        }
    }

    public void trySelection(){
        setSelection(getRandomString());
    }

    //Returns a random valid input
    public string getRandomString(){
        string result = "";
        for (int i = 0; i < 21; i++) {
            var nextCard = "" + UnityEngine.Random.Range(2, 13);
            if (nextCard.Equals("10")) nextCard = "a";
            else if (nextCard.Equals("11")) nextCard = "b";
            else if (nextCard.Equals("12")) nextCard = "c";
            result += nextCard;

        }
        return result;
    }

    //Updates the input field to match the combination on sreen
    public void UpdateField(){
        var selection = GetSelectionString();
        toolPanel.setInputField(selection);
        if (runningWebgl) {
            ExtSetUrl(selection);
        }
    }
    
    //Sets the combination on sceen and the input field to the given combination
    public void setSelection(string input) {
        if (input.Length > 21) input = input.Substring(0, 21);
        else if(input.Length < 21) {
            for (int j = input.Length; j < 21; j++){
                input += input.Last();
            }
        }

        var parsedInput = parseInputFromString(input);
        
        var i = 0;
        foreach (var cardGroup in Groups.Select(c => c.GetComponent<CardGroup>()).OrderBy(cg => cg.groupId)){
            cardGroup.setSelected(parsedInput[i]);
            i++;
        }

        UpdateField();
    }

    //Converts an input string to list of translated strings
    public List<string> parseInputFromString(string input) {
        var i = 0;
        var result = new List<string>();
        
        while (i < input.Length) {
            switch (input[i]) {
                    case 'a':
                        result.Add("10");
                        break;
                    case 'b':
                        result.Add("11");
                        break;
                    case 'c':
                        result.Add("12");
                        break;
                    default:
                        result.Add("" + input[i]);
                        break;
            }
            i++;
        }

        return result;
    }

    //Set the combination on screen to match the input field
    public void setFromTextField(Text field){
        var input = field.text;
        if (isValidInput(input)){
            setSelection(input);
        }
    }

    //Returns true if string is a valid input. Regex based
    public bool isValidInput(string input) {
        return Regex.IsMatch(input, @"\A\b[2-9a-cA-C]+\b\Z");
    }

    //Get a string representation of the current on screen combination
    public string GetSelectionString(){
        var result = "";
        var sb = new StringBuilder();
        foreach (var cardGroup in Groups.Select(c => c.GetComponent<CardGroup>()).OrderBy(cg => cg.groupId)){
            var cardGroupComponent = cardGroup.GetComponent<CardGroup>();
            switch (cardGroupComponent.NameOfSelected()) {
                case "10":
                    result += "a";
                    sb.Append("a");
                    break;
                case "11":
                    result += "b";
                    sb.Append("b");
                    break;
                case "12":
                    result += "c";
                    sb.Append("c");
                    break;
                default:
                    result += cardGroupComponent.NameOfSelected();
                    sb.Append(cardGroupComponent.NameOfSelected());
                    break;
            }
        }
        return sb.ToString();//result;
    }

    //Get the current on screan combination as a list of strings
    public List<string> GetSelectionList() {
        var result = new List<string>();
        foreach (var cardGroup in Groups.Select(c => c.GetComponent<CardGroup>()).OrderBy(cg => cg.groupId)) {
            var cardGroupComponent = cardGroup.GetComponent<CardGroup>();
            result.Add(cardGroupComponent.NameOfSelected());
        }
        return result;
    }
}
