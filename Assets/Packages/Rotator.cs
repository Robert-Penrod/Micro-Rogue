using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;

    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, Speed) * Time.deltaTime);
    }
}
