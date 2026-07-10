using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_A_Drop : SIE
{
    protected override void Awake()
    {
        base.Awake();
        _skillInstance.OnActivated += Drop;
    }

    void Drop()
    {
        Debug.Log("DROPPING!!!");
        transform.SetParent(null);
    }
}
