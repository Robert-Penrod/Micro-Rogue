using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class PostToggler : MonoBehaviour
{
    Volume _volume;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            _volume.enabled = !_volume.enabled;
        }
    }
}
