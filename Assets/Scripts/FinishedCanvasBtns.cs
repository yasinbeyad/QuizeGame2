using UnityEngine;

public class FinishedCanvasBtns : MonoBehaviour
{
    public GameObject finishedCanvas;
    public GameObject questionCanvas;
    public void Restart() {
        finishedCanvas.SetActive(false);
        questionCanvas.SetActive(true);
    }
       
}
