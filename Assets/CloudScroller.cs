using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 1f;
    [SerializeField] float _scrollPeriod = 200f;

    private void Update()
    {
        transform.position += (Vector3)Vector2.right * _scrollSpeed * Time.deltaTime;
        if(transform.position.x > _scrollPeriod)
        {
            transform.position = transform.position - (Vector3)Vector2.right * _scrollPeriod;
        }
    }
}
