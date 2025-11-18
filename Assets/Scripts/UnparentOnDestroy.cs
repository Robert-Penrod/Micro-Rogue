using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentOnDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        transform.parent = null;
    }
}
