using System.Collections.Generic;
using UnityEngine;

public class PhysicsPatch : MonoBehaviour
{
    public float Drag;
    List<Rigidbody2D> _colBodyList = new();
    SkillInstance _skillInstance;
    [SerializeField] List<TagCollection.TagType> ImunityTags = new();

    #region Init
    private void Awake()
    {
        _skillInstance = GetComponent<SkillInstance>();
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) => HandleCollision(collision, true);
    private void OnTriggerStay2D(Collider2D collision) => HandleCollision(collision, true);
    private void OnTriggerExit2D(Collider2D collision) => HandleCollision(collision, false);
    void HandleCollision(Collider2D col, bool isColliding)
    {
        var colBody = col.GetComponent<Rigidbody2D>();
        if (colBody == null) return;

        if (_skillInstance != null)
        {
            var colActor = col.GetComponent<Actor>();
            if (colActor != null)
            {
                // Can't hit self
                if (_skillInstance.Skill.Actor == colActor) return;

                // Immunity
                foreach(TagCollection.TagType imunityTag in ImunityTags)
                {
                    if (_skillInstance.Skill.Actor.Tags.HasTag(imunityTag)) return;
                }
            }
        }

        if (isColliding)
        {
            if (!_colBodyList.Contains(colBody)) _colBodyList.Add(colBody);
        }
        else
        {
            if (_colBodyList.Contains(colBody)) _colBodyList.Remove(colBody);
        }
    }
    #endregion

    private void FixedUpdate()
    {
        _colBodyList.ForEach(body =>
        {
            body.linearVelocity *= (1f - Drag * Time.fixedDeltaTime);
        });
    }
}
