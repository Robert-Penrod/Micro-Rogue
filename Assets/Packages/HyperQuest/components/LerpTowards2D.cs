using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTowards2D : MonoBehaviour
{
    public Transform Target;
    public float LerpSpeed = 3f;

    private void Update()
    {
        if (Target == null) return;
        transform.position = (Vector3)Vector2.Lerp(transform.position, Target.position, LerpSpeed * Time.deltaTime) + Vector3.forward * transform.position.z;
    }
}
