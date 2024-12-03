using TMPro;
using UnityEngine;

public class CustomPopUp : MonoBehaviour
{
    [SerializeField] private GameObject popupUI;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private float showTime = 1f;
    
    private AudioSource _source;
    private GameObject _textObj;

    public void ShowPopWithText(GameObject popupString)
    {
        _textObj = popupString;
        
        popupText.text = _textObj.GetComponent<PopUpText>().popupText;
        _source = _textObj.GetComponent<AudioSource>();
        
        popupUI.SetActive(true);
        if (_source.clip != null) _source.Play();

        Invoke(nameof(HidePopUp), _source.clip.length + showTime);
    }

    private void HidePopUp()
    {
        popupUI.SetActive(false);
        _textObj.SetActive(false);
    }

    public void DelayedAction(float delayTime)
    {
        
    }
}
