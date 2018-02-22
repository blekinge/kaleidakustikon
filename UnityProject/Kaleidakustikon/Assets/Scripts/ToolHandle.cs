using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//   [DEPRECATED]   UI behaviour for the button that brings forward the tool
public class ToolHandle : MonoBehaviour {

    public ToolPanel panel;
    public RectTransform arrow;

    public void clickedHandle(){
        panel.ToggleTools();
        arrow.Rotate(new Vector3(0, 0, 180));
    }
}
