[System.Serializable]
public class Questions
{
    public string question;
    public Answers[] answers;
}

[System.Serializable]
public class Answers
{
    public string answer;
    public bool isCorrect = false;
}
