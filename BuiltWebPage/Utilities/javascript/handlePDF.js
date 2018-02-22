//This script generates pdfs (using verovio for svg scores)

//Some compatibility code for IE
if (typeof SVGElement === 'object' && !SVGElement.prototype.outerHTML) {
    Object.defineProperty(SVGElement.prototype, 'outerHTML', {
        get: function () {
            var $node, $temp;
            $temp = document.createElement('div');
            $node = this.cloneNode(true);
            $temp.appendChild($node);
            return $temp.innerHTML;
        },
        enumerable: false,
        configurable: true
    });
}

var bg, cardImgs, imagePath = "images/";
var vrvToolkit = new verovio.toolkit();

//Verovio options
var zoom = 120;
var options =   {
                    pageHeight: 2000,
                    pageWidth: 2650,
                    scale: zoom
                };
vrvToolkit.setOptions(options);

//Map that maps from char to number representation of card
var fromStringCardToNumber = {
    "2": "02",
    "3": "03",
    "4": "04",
    "5": "05",
    "6": "06",
    "7": "07",
    "8": "08",
    "9": "09",
    "a": "10",
    "b": "11",
    "c": "12"
};

//Map that maps from numerical to char representation of a group
var fromIntToGroup = {
    0: "a",
    1: "b",
    2: "c",
    3: "d",
    4: "e",
    5: "f",
    6: "g",
    7: "h",
    8: "i",
    9: "k",
    10: "l",
    11: "m",
    12: "n",
    13: "o",
    14: "p",
    15: "q",
    16: "r",
    17: "s",
    18: "t",
    19: "u",
    20: "v"
};

//Creates a new empty pdf
var makePdf = function () {
    var pdf = new jsPDF('l', 'pt');
    var c = pdf.canvas;

    //load a svg snippet in the canvas with id = 'drawingArea'
    canvg(c, bg, {
        ignoreMouse: true,
        ignoreAnimation: true,
        ignoreDimensions: true
    });
    return pdf;
}

function notifyRenderFinished(){
    //console.log("Render finished!");
    //parent.postMessage("PDF_RENDERED", "*");
    gameInstance.SendMessage('GameController', 'UnPauseGame');
    console.log("The pdf has finished rendering");
}

//Render starts download of new generated pdf
function renderPdf(pdf, title){
    pdf.save(title + ".pdf");
    notifyRenderFinished();
    //This would be cooler but does not work (maybe too big data uri):
    //pdf.output('datauri');
}

//Gets the base64 data uri of an image
function getBase64Image(img) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);
    var dataURL = canvas.toDataURL("image/jpeg");
    return dataURL;
}

//Loads each of the cards based on a combination in the parameters and and passes them to
//the RenderImages function
function LoadImages(){
    var comb = getParameterByName("comb");
    var images = [];
    var loadedImg = 0;
    for(i = 0; i < 21; i++){
        images[i] = new Image();
        //Add image onLoad
        images[i].onload = function(){
            console.log("loaded image no. " + images.length);
            loadedImg++;
            if(loadedImg == 21){
                //All images have been loaded
                console.log("Finished!");
                RenderImages(images);
            }
        }
        //Set source and provoce an onload call
        var imageSrc = "Utilities/images/cards/" + fromIntToGroup[i] + "_" + fromStringCardToNumber[comb[i]] + ".jpg";
        console.log("Asking for: " + imageSrc);
        images[i].src = imageSrc;
    }
}

//Add images to a pdf at correct positions and pass the pdf to renderPdf function
function RenderImages(imageURIs) {
    var pdf = makePdf();
    var pageWidth = 842;
    var pageHeight = 595;
    var imgSize = 110;
    var startX = (pageWidth - (7 * imgSize)) / 2;
    var startY = 130;
    for (r = 0; r < 3; r++) {
        for (c = 0; c < 7; c++) {
            var currentUri = getBase64Image(imageURIs[7 * r + c]);
            pdf.addImage(currentUri, 'JPEG', startX + imgSize * c, startY + imgSize * r, imgSize, imgSize);
        }
    }
    renderPdf(pdf, "Facsimile");
}

//creates an svg file representing the combination in the url parameters,
//adds it to a pdf and calls the renderFunction
function RenderSheet() {
    var DOMURL = window.URL || window.webkitURL || window;
    
    var data = loadFile(getParameterByName("comb"));
    
    vrvToolkit.loadData(data)
    var svg = vrvToolkit.renderPage(1);

    var can = document.createElement('canvas');
    can.width = 3500;
    can.height = 2500;
    var ctx = can.getContext('2d');

    var image = new Image();
    var img = new Image();
    var svgBlob = new Blob([svg], {
        type: 'image/svg+xml'
    });
    var url = DOMURL.createObjectURL(svgBlob);

    img.onload = function () {
        ctx.drawImage(img, 0, 0);
        DOMURL.revokeObjectURL(url);
        var doc = makePdf();
        var pngDataUrl = can.toDataURL();
        doc.addImage(pngDataUrl, 'PNG', 24, 50, 820, 585);
        renderPdf(doc, "SheetMusic");
    }

    img.src = url;
}

var bgReady = false;
var saxonReady = false;

//Load the pdf-bg image and tryRender()
window.onload = function () {
    $.get("images/pdfBg.svg", function(data){
        bg = data;
        bgReady = true;
    });
};

//Try render when saxon is ready
var onSaxonLoad = function () {
    console.log("onSaxon!");
    saxonReady = true;
};