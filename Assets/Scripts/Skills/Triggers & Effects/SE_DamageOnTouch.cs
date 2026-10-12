using UnityEngine;

public class SE_DamageOnTouch : SkillEffect
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == _skill.Actor.gameObject) return;
        var colActor = collision.GetComponent<Actor>();
        if (colActor == null) return;

        Debug.Log(colActor.GetName());
    }
}
