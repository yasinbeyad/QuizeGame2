using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HomeScene : MonoBehaviour
{
    public GameObject chanceWheel;
    public Text userScoreTxt;
    private void Start()
    {
        userScoreTxt.text = "" + PlayerPrefs.GetInt("Score");
    }

    public void WheelChance()
    {
        chanceWheel.GetComponent<Canvas>().sortingOrder = 2;
    }
    public void BackWheelChance()
    {
        chanceWheel.GetComponent<Canvas>().sortingOrder = -1;
    }

}
