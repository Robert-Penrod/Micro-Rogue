using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public bool DrawCounter = true;

    // Public variables for UI display
    public int refreshRate = 10; // Number of frames before updating the FPS display

    // Private variables for calculations
    private int frameCount;
    private float deltaTime;
    private float fps;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift))
        {
            DrawCounter = !DrawCounter;
        }

        // Increment frame count and time elapsed
        frameCount++;
        deltaTime += Time.deltaTime;

        // Update FPS display at the specified refresh rate
        if (frameCount >= refreshRate)
        {
            fps = frameCount / deltaTime;
            fps *= Time.timeScale;

            // Reset counters for the next calculation period
            frameCount = 0;
            deltaTime = 0f;
        }
    }

    // Optional: Debug display in the Editor if no UI Text is assigned
    void OnGUI()
    {
        if (!DrawCounter) return;
        GUI.Label(new Rect(10, 10, 100, 20), $"FPS: {fps:F1}");
    }
}
