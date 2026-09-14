using System.Collections.Generic;
using UnityEngine;

public class SimpleButton_RadioGroup : MonoBehaviour
{
    #region Vars
    [SerializeField] SimpleButton_OldUI _firstSelectedBtn;
    List<SimpleButton_OldUI> _buttonList = new();
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
        _buttonList.AddRange(GetComponentsInChildren<SimpleButton_OldUI>());
    }
    #endregion

    void HighlightButton(SimpleButton_OldUI buttonToHighlight)
    {
        if (!buttonToHighlight.GetInteractable()) return;
        _buttonList.ForEach(button =>
        {
            button.IsHighlighted = button == buttonToHighlight;
        });
    }
}
