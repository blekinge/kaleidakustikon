mergeInto(LibraryManager.library, {
  ExtOpenMei: function (comp) {
    var combination = Pointer_stringify(comp);
    OpenMei(combination);
  },
  
  ExtOpenMidi: function (comp) {
    var combination = Pointer_stringify(comp);
    openMidiPlayer(combination);
  },
  
  ExtPrintSheet: function (comp) {
    var combination = Pointer_stringify(comp);
    printSheetPdf(combination);
  },
  
  ExtPrintImages: function (comp) {
    var combination = Pointer_stringify(comp);
    PrintImagePdf(combination);
  },

  ExtSetUrl: function (comb) {
  	var combination = Pointer_stringify(comb);
  	SetUrlFromComb(combination);
  },
  
  ExtGetUrlComp: function () {
    var returnComb = GetCombParam();
    var buffer = _malloc(lengthBytesUTF8(returnComb) + 1);
    stringToUTF8(returnComb, buffer, returnComb.length + 1);
    return buffer;
  },

});