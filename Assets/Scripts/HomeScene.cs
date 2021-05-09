using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomeScene : MonoBehaviour
{
    public Text userScoreTxt;
    private void Start() {
        userScoreTxt.text = "" + PlayerPrefs.GetInt("Score");
    }
}
