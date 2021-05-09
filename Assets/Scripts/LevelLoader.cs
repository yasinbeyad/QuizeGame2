using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public int sceneIndex;
    public Slider slider;
    public Text progressTxt;
    private void Start() {
        LoadLevel();
       
    }
    public void LoadLevel() {
        StartCoroutine(LoadAsynchronously());
    }
    IEnumerator LoadAsynchronously() {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while(!operation.isDone){

            float progress = Mathf.Clamp01(operation.progress/0.9f);
            slider.value = progress;
            progressTxt.text = progress * 100f + "%";
            yield return null;
        }
             
    }
}
