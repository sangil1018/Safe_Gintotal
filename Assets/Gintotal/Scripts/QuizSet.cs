using System;
using UnityEngine;

public class QuizSet : MonoBehaviour
{
    [SerializeField] private GameObject[] checks;
    [SerializeField] private GameObject[] hideObjs;
    [SerializeField] private GameObject[] showObjs;

    private int _count;

    private void Awake()
    {
        // _count = SessionManager.Instance.answerCorrect;
        // quizMain.SetActive(false);
    }

    public void InitialQuiz()
    {
        foreach (var hide in hideObjs)
        {
            hide.SetActive(false);
        }

        foreach (var show in showObjs)
        {
            show.SetActive(true);
        }
    }

    public void CorrectCount()
    {
        checks[_count].SetActive(true);
        _count += 1;

        if (_count == SessionManager.Instance.MAXCount)
        {
            Invoke(nameof(GoToEnding), 2f);
        }
    }

    private void GoToEnding()
    {
        SessionManager.Instance.QuizDone();
    }
}
