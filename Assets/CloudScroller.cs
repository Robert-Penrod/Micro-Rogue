using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 1f;
    [SerializeField] float _scrollPeriod = 200f;

    private void Update()
    {
        transform.localPosition += (Vector3)Vector2.right * _scrollSpeed * Time.deltaTime;
        if(transform.localPosition.x > _scrollPeriod)
        {
            transform.localPosition = transform.localPosition - (Vector3)Vector2.right * _scrollPeriod;
        }
    }
}
