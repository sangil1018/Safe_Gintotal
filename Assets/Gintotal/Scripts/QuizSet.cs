using System;
using UnityEngine;
using UnityEngine.Animations;

public class QuizSet : MonoBehaviour
{
    [SerializeField] private GameObject[] checks;
    [SerializeField] private GameObject[] hideObjs;
    [SerializeField] private GameObject[] showObjs;

    private int _count;

    public void InitialQuiz()
    {
        var constrain = SessionManager.Instance.playerOrigin.GetComponent<ParentConstraint>();
        if (constrain != null) constrain.constraintActive = false;
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
            Invoke(nameof(GoToEnding), 5f);
        }
    }

    private void GoToEnding() => SessionManager.Instance.QuizDone();
}
