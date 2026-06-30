using System.Collections.Generic;
using UnityEngine;

public class SimpleButton_RadioGroup : MonoBehaviour
{
    #region Vars
    [SerializeField] SimpleButton _firstSelectedBtn;
    List<SimpleButton> _buttonList = new();
    #endregion

    #region Init

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        GetButtons();
        _buttonList.ForEach(button =>
        {
            button.IsHighlighted = false;
            button.OnDownEvent.AddListener(() =>
            {
                HighlightButton(button);
            });
        });
        if(_firstSelectedBtn != null) HighlightButton(_firstSelectedBtn);
    }

    void GetButtons()
    {
        _buttonList.Clear();
        _buttonList.AddRange(GetComponentsInChildren<SimpleButton>());
    }
    #endregion

    void HighlightButton(SimpleButton buttonToHighlight)
    {
        _buttonList.ForEach(button =>
        {
            button.IsHighlighted = button == buttonToHighlight;
        });
    }
}
