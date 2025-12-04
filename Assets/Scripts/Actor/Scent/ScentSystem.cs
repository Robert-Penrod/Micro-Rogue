using System.Collections.Generic;
using UnityEngine;
using ManaSprite.EasyPooling;

public class ScentSystem : MonoBehaviour
{
    public float DropTime = 0.25f;
    float _dropTick = 0f;
    public float Lifetime = 4;
    public Actor Actor { get; private set; }
    List<ScentDrop> _dropList = new();
    ScentDrop _scentdropTemplate;

    private void OnDrawGizmos()
    {
        _dropList.RemoveAll(x => x == null || !x.gameObject.activeInHierarchy);
        for(int i = 0; i < _dropList.Count; i++)
        {
            float alpha = (i / (float)_dropList.Count).Remap(0f, 1f, 0.25f, 0.05f);
            Gizmos.color = Color.white.Alpha(alpha);

            if (i == 0)
            {
                Gizmos.DrawLine(transform.position, _dropList[i].transform.position);
            }
            else
            {
                Gizmos.DrawLine(_dropList[i - 1].transform.position, _dropList[i].transform.position);
            }
        }
    }

    private void Awake()
    {
        Actor = GetComponentInParent<Actor>();

        _scentdropTemplate = new GameObject("Scent Drop").AddComponent<ScentDrop>();
        _scentdropTemplate.gameObject.layer = LayerMask.NameToLayer("Scent");
        _scentdropTemplate.ScentSystem = this;
        CircleCollider2D col = _scentdropTemplate.gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.1f;
        _scentdropTemplate.gameObject.SetActive(false);
        _scentdropTemplate.gameObject.transform.SetParent(this.transform);
    }

    private void Start()
    {
        _dropTick = Random.Range(0f, DropTime);
    }

    private void FixedUpdate()
    {
        _dropTick += Time.fixedDeltaTime;
        if(_dropTick >= DropTime)
        {
            _dropTick -= DropTime;
            Drop();
        }
    }

    void Drop()
    {
        _dropList.RemoveAll(x => x == null || !x.gameObject.activeInHierarchy);
        ScentDrop scentDrop = _scentdropTemplate.gameObject.PooledInstantiate().GetComponent<ScentDrop>();
        scentDrop.transform.position = transform.position;
        scentDrop.gameObject.hideFlags = HideFlags.HideInHierarchy;
        scentDrop.gameObject.SetActive(true);
        _dropList.Insert(0, scentDrop);
    }

    public void ClearScentSystem()
    {
        while (_dropList.Count > 0)
        {
            ScentDrop drop = _dropList[0];
            if (drop != null) Destroy(drop.gameObject);
            _dropList.RemoveAt(0);
        }
        _dropList.Clear();
    }
}
