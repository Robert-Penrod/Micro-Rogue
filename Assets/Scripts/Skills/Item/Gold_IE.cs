using UnityEngine;

public class Gold_IE : ItemEffect
{
    public int GoldAmount = 25;
    bool _hasPayed;

    private void OnEnable()
    {
        if(!_hasPayed)
        {
            if(_skill.Actor.IsPlayer())
            {
                var player = _skill.Actor.GetComponentInParent<Player>();
                player.Data.Gold += GoldAmount;
                _hasPayed = true;
            }
        }
    }
}
