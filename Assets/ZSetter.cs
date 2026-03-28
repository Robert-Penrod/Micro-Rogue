using UnityEngine;

public class ZSetter : MonoBehaviour
{
    [SerializeField] float _z;

    private void Start()
    {
        var p = transform.position;
        p.z = _z;
        transform.position = p; 
    }
}
