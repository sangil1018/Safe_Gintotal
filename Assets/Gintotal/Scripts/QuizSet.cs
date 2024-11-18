using System;
using UnityEngine;

public class QuizSet : MonoBehaviour
{
    [SerializeField] private GameObject quizMain;
    [SerializeField] private GameObject[] checks;
    [SerializeField] private GameObject[] hideObjs;
    [SerializeField] private GameObject[] showObjs;

    private int _count;

    private void Awake()
    {
        // _count = SessionManager.Instance.answerCorrect;
        quizMain.SetActive(false);
    }

    public void CorrectCount()
    {
        _count = SessionManager.Instance.answerCorrect++;
        checks[_count].SetActive(true);

        if (_count > SessionManager.Instance.MAXCount)
        {
            Invoke(nameof(GoToEnding), 2f);
        }
    }

    private void GoToEnding()
    {
        SessionManager.Instance.QuizDone();
    }
}
