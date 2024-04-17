using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

#region jsonClass
[System.Serializable]
public class Answer
{
    public int id;
    public string answer;
    public bool isCorrect;
}

[System.Serializable]
public class Question
{
    public string question;
    public Answer[] answers;
    public string answerLocation;

}

[System.Serializable]
public class QuizData
{
    public Question[] Questions;
}

public class State
{
    public string[] answers;
    public Color[] colors;
}
#endregion

public class QuizManager : MonoBehaviour
{
    // public
    public QuizData quizData;
    public Button[] answerButtons;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answerLocationText;
    public Button nextButton;
    public Button backButton;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI centeredText;
    public Slider slider;

    // private
    private List<State> questionStates = new List<State>();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private Color defaultButtonColor;
    private bool gameOver = false;



    private void Start()
    {
        //Load text from a JSON file (under Assets/Resources/)
        var jsonTextFile = Resources.Load<TextAsset>("quizData");
        quizData = JsonUtility.FromJson<QuizData>(jsonTextFile.text);

        nextButton.onClick.AddListener(NextClicked);
        backButton.onClick.AddListener(BackClicked);

        defaultButtonColor = answerButtons[0].image.color;
        ResetQuiz();

        // init values
        slider.maxValue = quizData.Questions.Length;
        slider.minValue = 0;
    }

    private void ResetQuiz()
    {
        questionStates.Clear();
        correctAnswers = 0;
        currentQuestionIndex = 0;
        gameOver = false;
        answerLocationText.text = "";

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
            button.interactable = true; // Make sure buttons are interactable
        }

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(BackClicked);
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = "<- back";

        nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "next ->";

        // change visibility
        nextButton.gameObject.SetActive(false);
        centeredText.gameObject.SetActive(false);

        answerLocationText.gameObject.SetActive(true);
        progressText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        slider.value = 0;
        SetBackBtnLogic();
        DisplayQuestion(); // Display the first question
        RefreshProgress();


    }

    private void SetBackBtnLogic()
    {
        if (currentQuestionIndex == 0)
        {
            // first question is return to main screen
            backButton.onClick.AddListener(ScreensManager.Instance().ReturnToHomeScreen);
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return";

        }
        else if (currentQuestionIndex == 1)
        {
            backButton.onClick.RemoveListener(ScreensManager.Instance().ReturnToHomeScreen);
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = "<- back";
        }
    }
    private void DisplayQuestion()
    {
        ResetButtonColors();
        PlayBackgroundQuizAudio(); // background music

        // show question
        Question question = quizData.Questions[currentQuestionIndex];
        questionText.text = question.question;

        // save description for later
        answerLocationText.gameObject.SetActive(false);
        answerLocationText.text = question.answerLocation;

        // check if the user already answer on that question
        if (currentQuestionIndex < questionStates.Count)
        {
            DisplayPastButtons();
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            // generate new question
            ShuffleAnswers(question);
            nextButton.gameObject.SetActive(false);
        }
        foreach (Button button in answerButtons)
            button.gameObject.SetActive(true);
    }

    private void ShuffleAnswers(Question question)
    {
        // simple shuffle and initialize for button
        List<Answer> shuffledAnswers = new List<Answer>(question.answers);
        shuffledAnswers.Shuffle();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffledAnswers[i].answer;
            bool isCorrect = shuffledAnswers[i].isCorrect;

            answerButtons[i].interactable = true;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClick(isCorrect));

            if (answerButtons[i].transform.Find("False") != null || answerButtons[i].transform.Find("True") != null) continue;

            GameObject answerData = new GameObject(isCorrect.ToString()); // hide data as object (name field)
            answerData.transform.SetParent(answerButtons[i].gameObject.transform);

        }
    }

    private void DisplayPastButtons()
    {
        // get the state of buttons for current question, and set it (buttons color)
        // it's to remember the user's choice
        State state = questionStates[currentQuestionIndex];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = state.answers[i];
            answerButtons[i].image.color = state.colors[i];
            answerButtons[i].interactable = false;
        }
        answerLocationText.text = quizData.Questions[currentQuestionIndex].answerLocation;
        answerLocationText.gameObject.SetActive(true);
    }

    private void OnAnswerButtonClick(bool isCorrect)
    {
        // answer buttons can be one of three, one of them correct. 
        // get clicked button object and color if  
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        clickedButton.image.color = isCorrect ? Color.green : Color.red;

        // check correct, and active the description
        if (isCorrect)
        {
            correctAnswers++;
            PlaySuccessSound();
        }
        else
            PlayWrongAnswerSound();

        answerLocationText.gameObject.SetActive(true);

        // create state and make buttons not clickable
        State state = new State()
        {
            colors = new Color[3],
            answers = new string[3]
        };

        for (int i = 0; i < answerButtons.Length; i++)
        {
            // set for now
            SetColorAndDeleteData(answerButtons[i]);
            answerButtons[i].interactable = false;

            // save for later
            state.colors[i] = answerButtons[i].image.color;
            state.answers[i] = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text;
        }
        questionStates.Add(state); // save to list

        // refresh and set next
        nextButton.gameObject.SetActive(true);
        RefreshProgress();
        if (questionStates.Count == quizData.Questions.Length)
        {
            gameOver = true;
            SetNextEndBtn(true);
        }
    }

    private void SetColorAndDeleteData(Button answerButton)
    {
        // i want to extract the data (correct or wrong) for each button. Show the user the right answer.
        Transform answerData = answerButton.transform.Find("True");
        if (answerData != null)
        {
            answerButton.image.color = Color.green;
            Destroy(answerData.gameObject);  // finish to work with this
            return;
        }
        // delete rest 
        answerData = answerButton.transform.Find("False");
        if (answerData == null) return;
        Destroy(answerData.gameObject);  // finish to work with this

    }
    private void SetNextEndBtn(bool isEnd)
    {
        if (isEnd)
        {
            // navigate to end-game screen            
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "End";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(EndGameScreen); // remove this lisetener inside
            PlayCheersSound();
        }
        else
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "next ->";
            nextButton.onClick.RemoveAllListeners();
            //nextButton.onClick.RemoveListener(EndGameScreen);
            nextButton.onClick.AddListener(NextClicked);
            PlayBackgroundQuizAudio();
        }
    }
    private void EndGameScreen()
    {
        slider.value = slider.maxValue;
        nextButton.onClick.RemoveListener(EndGameScreen);
        nextButton.onClick.AddListener(NextClicked);

        // make reset buttons
        backButton.GetComponentInChildren<TextMeshProUGUI>().text = "Reset";
        backButton.onClick.RemoveAllListeners();   // override the back btn previous 
        backButton.onClick.AddListener(ResetQuiz);

        nextButton.gameObject.SetActive(false);

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }
        answerLocationText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        centeredText.gameObject.SetActive(true);


        string badge = "";
        int totalQuestions = quizData.Questions.Length;
        float ratio = (float)correctAnswers / totalQuestions;


        if (ratio <= 0.3f)
            badge = "Bronze";
        else if (ratio <= 0.6f)
            badge = "Silver";
        else if (ratio < 1.0f)
            badge = "Gold";
        else
            badge = "Platinum"; // meaning its 1:1 ratio :D

        centeredText.text = $"Well Done!\n" +
            $"Correct answers: {correctAnswers} out of {totalQuestions}\n\n" +
            $"Badge: {badge}";
    }

    private void NextClicked()
    {

        currentQuestionIndex++;
        DisplayQuestion();
        RefreshProgress();

        MoveSlider();
        if (gameOver && currentQuestionIndex + 1 == quizData.Questions.Length)
            SetNextEndBtn(true);
        SetBackBtnLogic();

    }


    private Coroutine sliderAnimationCoroutine;

    private void MoveSlider()
    {
        float targetValue = (currentQuestionIndex) / (float)quizData.Questions.Length * slider.maxValue;

        slider.value = targetValue;
    }



    private void BackClicked()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            DisplayQuestion();
            RefreshProgress();
        }
        SetBackBtnLogic();
        SetNextEndBtn(false);
        MoveSlider();
    }

    private void RefreshProgress()
    {
        progressText.text = $"Question #{currentQuestionIndex + 1} / {quizData.Questions.Length}\n" +
            $"Correct answers: {correctAnswers}";
    }

    private void ResetButtonColors()
    {
        foreach (Button button in answerButtons)
        {
            button.image.color = defaultButtonColor;
        }
    }

    #region audio
    private AudioSource backgroundAudioSource;
    public AudioClip backgroundQuizAudio;
    public AudioClip successSound;
    public AudioClip wrongAnswerSound;
    public AudioClip cheersSound;
    //private Camera mainCamera; // im using this for position the audio listener

    public void PlayBackgroundQuizAudio()
    {
        backgroundAudioSource = GetComponent<AudioSource>();
        backgroundAudioSource.Stop();
        backgroundAudioSource.loop = true;
        backgroundAudioSource.volume = 0.5f;
        backgroundAudioSource.volume = 1f;
        backgroundAudioSource.clip = backgroundQuizAudio;
        backgroundAudioSource.Play();
        //AudioSource.PlayClipAtPoint(backgroundQuizAudio, Camera.main.transform.position);

    }

    public void PlaySuccessSound()
    {
        backgroundAudioSource.loop = false;
        backgroundAudioSource.Stop();

        AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position, 0.5f);

    }

    public void PlayWrongAnswerSound()
    {
        backgroundAudioSource.loop = false;
        AudioSource.PlayClipAtPoint(wrongAnswerSound, Camera.main.transform.position, 0.5f);
    }

    public void PlayCheersSound()
    {
        backgroundAudioSource.Stop();
        backgroundAudioSource.loop = false;
        backgroundAudioSource.clip = cheersSound;
        backgroundAudioSource.volume = 0.3f;
        backgroundAudioSource.Play();
        //AudioSource.PlayClipAtPoint(cheersSound, Camera.main.transform.position);
    }
    #endregion


}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
