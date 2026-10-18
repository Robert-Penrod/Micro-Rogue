using System;
using System.IO;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    [SerializeField] bool _isOn;
    float _captureTick;
    float _captureTime = 1f;

    void SetOn(bool isOn)
    {
        _isOn = isOn;
        _captureTick = _captureTime;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) SetOn(!_isOn);
        CaptureUpdate();
    }

    void CaptureUpdate()
    {
        if (!_isOn) return;

        _captureTick += Time.deltaTime;
        if (_captureTick >= _captureTime)
        {
            _captureTick -= _captureTime;
            Capture();
        }
    }

    void Capture()
    {
        // 1. Define the folder name
        string folderPath = Path.Combine(Application.dataPath, "../Screenshots");

        // 2. Check if the folder exists; if not, create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 3. Generate a unique name using a date-time timestamp
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"Screenshot_{timestamp}.png";

        // 4. Combine them into a full path and capture
        string fullPath = Path.Combine(folderPath, fileName);
        ScreenCapture.CaptureScreenshot(fullPath);
    }
}
