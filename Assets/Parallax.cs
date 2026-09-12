using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float _amount;

    Vector2 _initPos;

    private void Awake()
    {
        _initPos = transform.position;
    }

    private void LateUpdate()
    {
        var camPos = Camera.main.transform.position;
        transform.position = (Vector3)(_initPos + (Vector2)camPos * _amount) + Vector3.forward * transform.position.z;
    }
}
