var pathToIndex = "";

//General function for loading MEI from combination

function isMinor(combination) {
    'use strict';
    var minorCards = ["3", "4", "a", "b", "c"];
    var decidingCard = combination.charAt(7);

    if (minorCards.indexOf(decidingCard) !== -1) {
        return true;
    }
    return false;
}

//This function fetches (if it exists) a parameter from the url of the window
function getParameterByName(name, url) {
    'use strict';
    if (!url) { url = window.location.href; }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) { return null; }
    if (!results[2]) { return ''; }
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

//This function takes a combination string and returns the mei file with that combination (requires saxon)
function loadFile(combination) {
    'use strict';
    //Initial mei file without any measures
    var file = pathToIndex + "Utilities/mei/noMusic.mei";
    
    //Fetch xslt that collects correct measures from all mei files
    var collector = Saxon.requestXML(pathToIndex + "Utilities/xslt/collector.xsl");
    //Fetch xslt that gives unique id's to all measures
    var measureId = Saxon.requestXML(pathToIndex + "Utilities/xslt/giveMeasureId.xsl");
    //Fetch xslt that makes minor
    var minorMaker = Saxon.requestXML(pathToIndex +"Utilities/xslt/makeMinor.xsl");
    
    var xml = Saxon.requestXML(file);
    var xsltProc = Saxon.newXSLT20Processor(collector);
    xsltProc.setParameter(null, "comb", combination);
    var mei = xsltProc.transformToDocument(xml);
    if (isMinor(combination)) {
        xsltProc = Saxon.newXSLT20Processor(minorMaker);
        mei = xsltProc.transformToDocument(mei);
    }
    xsltProc = Saxon.newXSLT20Processor(measureId);
    var meiWithMeasureId = Saxon.serializeXML(xsltProc.transformToDocument(mei));
    return meiWithMeasureId;
}
