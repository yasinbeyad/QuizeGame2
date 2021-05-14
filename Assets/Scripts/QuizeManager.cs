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
    public Sprite imgWrong;
    public Sprite imgCorrect;
    public Sprite imgAnswer;
    private bool isTimeBuyed;
    [SerializeField] float startTime = 12f;
    [SerializeField] Slider TimerSlider;
    [SerializeField] Text timerTxt;
    [SerializeField] Button timerBtn;
    [SerializeField] GameObject goNext;
    //Helps
    public Button delete2Answers;
    public Button tryAgain;
    public GameObject coinWarning;
    private bool isTried = false;
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
        SetCoinTxt();
        isTimeBuyed = false;
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
    private void BuyExtraTime() {
        if(PlayerPrefs.GetInt("Coin") >= 30)
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") - 30);
        else {
            StartCoroutine(CoinWarning());
            return;
        }
        SetCoinTxt();
        StopCoroutine("Timer");
        StartCoroutine("Timer");
        isTimeBuyed = true;
        timerBtn.interactable = false;
    }
    private void SetScoreTxt() {
        scoreTxt.text = "" + PlayerPrefs.GetInt("Score");
    }
    private void SetCoinTxt() {
        coinTxt.text = "" + PlayerPrefs.GetInt("Coin");
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
            timerBtn.onClick.RemoveAllListeners();
            if(unAnswered[random].answers[i].isCorrect) {

                answerBtns[i].onClick.AddListener(() => StartCoroutine(CorrectAnswer(currentBtn)));


            }
            else {
                answerBtns[i].onClick.AddListener(() => WrongAnswer(currentBtn));
            }
        }
        answereds.Add(unAnswered[random]);
        unAnswered.RemoveAt(random);
        timerBtn.interactable = true;
        timerBtn.onClick.AddListener(() => BuyExtraTime());
        if(delete2Answers.interactable)
            delete2Answers.onClick.AddListener(() => Delete2Choses());
    }
    void Delete2Choses() {
        if(PlayerPrefs.GetInt("Coin") >= 40) {
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") - 40);
            SetCoinTxt();
        }
        else {
            StartCoroutine(CoinWarning());
            return;
        }
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
        btnImg.sprite = imgCorrect;
        IntractableAnswerBtns(false);
        yield return new WaitForSeconds(changeQuestionTime);
        btnImg.sprite = imgAnswer;
        SelectQuestion();
    }
    private void WrongAnswer(Button currentBtn) {
        delete2Answers.onClick.RemoveAllListeners();
        StopCoroutine("Timer");
        Image btnImg = currentBtn.GetComponent<Image>();
        btnImg.sprite = imgWrong;
        IntractableAnswerBtns(false);
        goNext.GetComponent<Button>().onClick.RemoveAllListeners();
        goNext.SetActive(true);
        timerBtn.interactable = false;
        goNext.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(GoNextQuestion(btnImg, currentBtn)));
        if(!isTried)
            tryAgain.interactable = true;

    }
    IEnumerator GoNextQuestion(Image btnImg, Button currentBtn) {
        if(PlayerPrefs.GetInt("Score") < 10)
            PlayerPrefs.SetInt("Score", 0);
        else {
            ShowScoreChange(currentBtn.transform.position, minusTxt);
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") - scoreForWrong);
        }
        SetScoreTxt();
        yield return new WaitForSeconds(changeQuestionTime);
        btnImg.sprite = imgAnswer;
        goNext.SetActive(false);
        SelectQuestion();

    }
    public void TryAgain() {
        if(PlayerPrefs.GetInt("Coin") >= 40) {
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") - 40);
            SetCoinTxt();
        }
        else {
            StartCoroutine(CoinWarning());
            return;
        }
        //everything comes here
        timerBtn.interactable = true;
        goNext.SetActive(false);
        IntractableAnswerBtns(true);
        StartCoroutine("Timer");
        for(int i = 0; i < answerBtns.Length; i++) {
            answerBtns[i].GetComponent<Image>().sprite = imgAnswer;
        }
        tryAgain.interactable = false;
        isTried = true;
    }
    void IntractableAnswerBtns(bool i) {
        foreach(Button btn in answerBtns)
            btn.interactable = i;
    }
    IEnumerator CoinWarning() {
        coinWarning.SetActive(true);
        yield return new WaitForSeconds(1);
        coinWarning.SetActive(false);
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
        else if(correct == 9) {
            //3 stars
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 25);
            coinEarned.text = "" + 25;
        }
        else if(correct == 10) {
            if(UsedHelp()) {
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 25);
                coinEarned.text = "" + 25;
            }

            else {
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 40);
                coinEarned.text = "" + 40;
            }
        }
    }
    private bool UsedHelp() {
        if(!delete2Answers.interactable || isTimeBuyed || isTried)
            return true;
        else
            return false;
    }
    private void FinishGame() {
        questionCanvas.SetActive(false);
        finishedCanvas.SetActive(true);
        finalScoreTxt.text = "" + PlayerPrefs.GetInt("Score");
        finalCorrects.text = "" + correct;
        CalculateCoin();
        finalCoin.text = "" + PlayerPrefs.GetInt("Coin");
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
        if(Input.GetKeyDown(KeyCode.X)) {
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 10);
            Debug.Log("Added 10 coins");
            SetCoinTxt();
        }

    }
}