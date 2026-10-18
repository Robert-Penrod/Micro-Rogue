using System.Collections.Generic;
using UnityEngine;

public class PhysicsPatch : MonoBehaviour
{
    [Header("Physics")]
    public float Drag;
    public float Follow;

    [Header("Immunity")]
    [SerializeField] List<string> ImuneActorNames = new();

    [Header("Statuses")]
    public float Pyro;
    public float Frost;
    public float Static;
    public float Webbed;

    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Actor
        Actor colActor = collision.GetComponent<Actor>();   
        if (colActor != null)
        {
            foreach (string name in ImuneActorNames)
            {
                if (colActor.gameObject.name.Contains(name, System.StringComparison.InvariantCultureIgnoreCase)) return;
            }

            colActor.Pyro += Pyro * Time.fixedDeltaTime;
            colActor.Frost += Frost * Time.fixedDeltaTime;
            colActor.Static += Static * Time.fixedDeltaTime;
            colActor.Webbed = Webbed * Time.fixedDeltaTime;
        }
        
        // Physics
        Rigidbody2D colBody = collision.GetComponent<Rigidbody2D>();
        if(colBody != null)
        {
            // Drag
            float dragMult = (1f - Drag * Time.fixedDeltaTime);
            if (collision.GetComponentInParent<SkillInstance>() != null) dragMult = dragMult.Lerp(1f, 0.5f);
            colBody.linearVelocity *= dragMult;

            // Follow
            if (_rb != null) _rb.AddForce(Follow * colBody.linearVelocity);
        }        
    }

    /*
    List<Rigidbody2D> _colBodyList = new();
    List<Actor> _colActorList = new();
    SkillInstance _skillInstance;
    [SerializeField] List<TagCollection.TagType> ImunityTags = new();

    #region Init
    private void Awake()
    {
        _skillInstance = GetComponentInParent<SkillInstance>();
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

        var colActor = col.GetComponent<Actor>();
        if (colActor != null)
        {
            if (_skillInstance != null)
            {
                // Can't hit self
                if (_skillInstance.Skill.Actor == colActor) return;

                // Immunity
                foreach (TagCollection.TagType imunityTag in ImunityTags)
                {
                    if (_skillInstance.Skill.Actor.Tags.HasTag(imunityTag)) return;
                }
            }
            foreach (string name in ImuneActorNames)
            {
                if (colActor.gameObject.name.Contains(name, System.StringComparison.InvariantCultureIgnoreCase)) return;
            }

            if (isColliding)
            {
                if (!_colActorList.Contains(colActor)) _colActorList.Add(colActor);
            }
            else
            {
                if (_colActorList.Contains(colActor)) _colActorList.Remove(colActor);
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
        _colBodyList.RemoveAll(x => x == null);
        _colBodyList.ForEach(body =>
        {
            body.linearVelocity *= (1f - Drag * Time.fixedDeltaTime);
        });
        _colActorList.ForEach(actor =>
        {
            actor.Webbed = 1f;
        });
    }
    */
}
