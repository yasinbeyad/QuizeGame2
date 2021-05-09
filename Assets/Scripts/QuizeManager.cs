using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuizeManager : MonoBehaviour
{
    public List<Questions> unAnswered = new List<Questions>();
    private List<Questions> answereds = new List<Questions>();
    public Text questionTxt;
    public Button[] answerBtns;
    private Text[] answerBtnsTxt;
    public float changeQuestionTime;
    public Text scoreTxt;
    public Text coinTxt;
    public GameObject plusTxt;
    public GameObject minusTxt;
    public int scoreForCorrect;
    public int scoreForWrong;
    [SerializeField] float startTime = 12f;
    [SerializeField] Slider TimerSlider;
    [SerializeField] Text timerTxt;
    //Helps
    public Button delete2Answers;
    //finished
    public Text finalScoreTxt;
    public Text finalCoin;
    public Text coinEarned;
    public Text finalCorrects;
    public GameObject finishedCanvas;
    public GameObject questionCanvas;
    private int correct = 0;

    // Start is called before the first frame update
    void Awake() {
        answerBtnsTxt = new Text[answerBtns.Length];
        for(int i = 0; i < answerBtns.Length; i++) {
            answerBtnsTxt[i] = answerBtns[i].gameObject.transform.GetChild(0).GetComponent<Text>();
        }
    }
    private void Start() {
    }
    void OnEnable() {
        SelectQuestion();
        SetScoreTxt();
    }
    private IEnumerator Timer() {
        float timer = startTime;
        do {
            timer -= Time.deltaTime;
            TimerSlider.value = timer / startTime;
            timerTxt.text = (int)(timer + 1f) + "";
            yield return null;
        } while(timer > 0f);
        SelectQuestion();
    }
    private void SetScoreTxt() {
        scoreTxt.text = "" + PlayerPrefs.GetInt("Score");
    }
    private void SetActiveOff() {
        plusTxt.SetActive(false);
        minusTxt.SetActive(false);
    }
    private void SelectQuestion() {
        IntractableAnswerBtns(true);
        SetActiveOff();
        if(unAnswered.Count == 0) {
            StopCoroutine("Timer");
            FinishGame();
            return;
        }
        StartCoroutine("Timer");
        int random = Random.Range(0, unAnswered.Count);
        questionTxt.text = unAnswered[random].question;
        for(int i = 0; i < answerBtns.Length; i++) {
            answerBtnsTxt[i].text = unAnswered[random].answers[i].answer;
            answerBtns[i].GetComponent<Image>().color = Color.white;
            Button currentBtn = answerBtns[i];
            delete2Answers.onClick.RemoveAllListeners();
            currentBtn.onClick.RemoveAllListeners();
            if(unAnswered[random].answers[i].isCorrect) {
           
                answerBtns[i].onClick.AddListener(() => StartCoroutine(CorrectAnswer(currentBtn)));
               

            }
            else {
                answerBtns[i].onClick.AddListener(() => StartCoroutine(WrongAnswer(currentBtn)));
            }
        }
        answereds.Add(unAnswered[random]);
        unAnswered.RemoveAt(random);
        if(delete2Answers.interactable)
            delete2Answers.onClick.AddListener(() => Delete2Choses());
    }
    void Delete2Choses() {
        for(int i = 0; i < 2; i++) {
            int rand;
            do {
                rand = Random.Range(0, answerBtns.Length);
            } while(answerBtns[rand].interactable == false
              || answereds[answereds.Count - 1].answers[rand].isCorrect);
            answerBtns[rand].interactable = false;
            answerBtns[rand].GetComponent<Image>().color = new Color32(200, 200, 200, 255);
            string txt = answerBtnsTxt[rand].text;
        }
        delete2Answers.interactable = false;
        
    }
    private void ShowScoreChange(Vector3 pos, GameObject scoreTxt) {
        scoreTxt.transform.position = pos;
        scoreTxt.SetActive(true);
    }
    IEnumerator CorrectAnswer(Button currentBtn) {
        delete2Answers.onClick.RemoveAllListeners();
        StopCoroutine("Timer");
        ShowScoreChange(currentBtn.transform.position, plusTxt);
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + scoreForCorrect);
        correct++;
        SetScoreTxt();
        Image btnImg = currentBtn.GetComponent<Image>();
        btnImg.color = Color.green;
        IntractableAnswerBtns(false);
        yield return new WaitForSeconds(changeQuestionTime);
        btnImg.color = Color.white;
        SelectQuestion();
    }
    IEnumerator WrongAnswer(Button currentBtn) {
        delete2Answers.onClick.RemoveAllListeners();
        StopCoroutine("Timer");
        if(PlayerPrefs.GetInt("Score") < 10)
            PlayerPrefs.SetInt("Score", 0);
        else {
            ShowScoreChange(currentBtn.transform.position, minusTxt);
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") - scoreForWrong);
        }
        SetScoreTxt();
        Image btnImg = currentBtn.GetComponent<Image>();
        btnImg.color = Color.red;
        IntractableAnswerBtns(false);
        yield return new WaitForSeconds(changeQuestionTime);
        btnImg.color = Color.white;
        SelectQuestion();
    }
    void IntractableAnswerBtns(bool i) {
        foreach(Button btn in answerBtns)
            btn.interactable = i;
    }
    private void CalculateCoin() {
        if(correct <= 4 && correct >= 0) {
            //1 star
            //should wait 10 minute and 0 coin
            coinEarned.text = "" + 0;
        }
        else if(correct <= 8) {
            //2 stars
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 15);
            coinEarned.text = "" + 15;
        }
        else if(correct==9){
            //3 stars
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 25);
            coinEarned.text = "" + 25;
        }
        else if(correct  == 10) {
            if(delete2Answers.interactable) {
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 40);
                coinEarned.text = "" + 40;
            }

            else {
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 25);
                coinEarned.text = "" + 25;
            }
        }
    }
    private void FinishGame() {
        questionCanvas.SetActive(false);
        finishedCanvas.SetActive(true);        
        finalScoreTxt.text = "" + PlayerPrefs.GetInt("Score");
        finalCorrects.text = "" + correct;
        finalCoin.text = "" + PlayerPrefs.GetInt("Coin");
        CalculateCoin();
        delete2Answers.interactable = true;
    }
    public void Restart() {
        unAnswered = new List<Questions>(answereds);
        answereds.Clear();
        correct = 0;
        questionCanvas.SetActive(true);
        finishedCanvas.SetActive(false);
    }
    private void Update() {//developer
        if(Input.GetKeyDown(KeyCode.S)) {
            PlayerPrefs.SetInt("Score", 0);
            Debug.Log("Score Sets to 0");
        }
        if(Input.GetKeyDown(KeyCode.C)) {
            PlayerPrefs.SetInt("Coin", 0);
            Debug.Log("Coins sets to 0");
        }
            
    }
}