//This script is resbonsible for recieving function calls from the unity app and redirecting them

//Called when a message is received from the iframe when a pdf has finished rendering. Used for unpausing app
function receiveMessage(event) {
    'use strict';
    if (event.data === "PDF_RENDERED") {
        //Pass the message on to the gameInstance
        gameInstance.SendMessage('GameController', 'UnPauseGame');
        console.log("The pdf has finished rendering");
    }
}

//Setting the messeage receiver to the correct function
window.addEventListener("message", receiveMessage, false);

//Set the source directory of the utility iframe
function setUtilityFrameSource(source) {
    'use strict';
    var utilityFrame = document.getElementById("utilityFrame");
    utilityFrame.src = source;
}

//Open the midi player page
function openMidiPlayer(comb) {
    'use strict';
    window.open("Utilities/renderSVG.html?comb=" + comb);
}

//Make the utilityFrame generate sheet music pdf
function printSheetPdf(comb) {
    'use strict';
    RenderSheet();
}

//Make the utilityFrame generate facsimile pdf
function PrintImagePdf(comb) {
    'use strict';
    LoadImages();
}

//Open a combination as a textual mei file
function OpenMei(comb) {
    'use strict';
    window.open("Utilities/openMei.html?comb=" + comb);
}

//Fetch the combination parameter from the url
function GetCombParam() {
    'use strict';
    var name = "comb";
    var url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) { return ''; }
    if (!results[2]) { return ''; }
    var comb = decodeURIComponent(results[2].replace(/\+/g, " "));
    return comb;
}

//The the combination parameter in the url
function SetUrlFromComb(comb) {
    'use strict';
    var toAdd = "?comb=" + comb;
    window.history.pushState("ignored", "ignored", toAdd);
}