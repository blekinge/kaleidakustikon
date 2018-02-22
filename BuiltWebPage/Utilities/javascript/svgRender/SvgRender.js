var vrvToolkit = new verovio.toolkit();
var page = 1;
var zoom = 30;
var pageHeight = 2000;
var pageWidth = 2800;
var zoom = 30;
var input;

function setOptions() {
    zoom = $(document).width() / (pageWidth / 100);
    options = {
        pageHeight: pageHeight,
        pageWidth: pageWidth,
        scale: zoom
    };
    vrvToolkit.setOptions(options);
}

function loadData(data) {
    setOptions();
    vrvToolkit.loadData(data);
    
    page = 1;
    loadPage();
}

function loadPage() {
    svg = vrvToolkit.renderPage(page, {});
    $("#svg_output").html(svg);
    
};

$(document).ready(function () {
    input = getParameterByName("comb");
    console.log(input);
});
var onSaxonLoad = function () {
    console.log("onSaxon!");
    
    loadData(loadFile(input));
};