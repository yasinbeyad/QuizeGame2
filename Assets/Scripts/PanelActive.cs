using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelActive : MonoBehaviour
{
    //GameObject Panel;
    public void PanelActiveState(GameObject panel) {
        bool state=panel.activeSelf;
        panel.SetActive(!state);
        
    }
}
