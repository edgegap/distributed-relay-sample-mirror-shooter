using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera[] otherCameras;

    void Start()
    {
        // Get all the cameras in the scene
        Camera[] allCameras = Camera.allCameras;

        // Filter out the current camera
        int count = 0;
        for (int i = 0; i < allCameras.Length; i++)
        {
            if (allCameras[i] != this.GetComponent<Camera>())
            {
                count++;
            }
        }

        // Create a new array with the filtered cameras
        otherCameras = new Camera[count];
        count = 0;
        for (int i = 0; i < allCameras.Length; i++)
        {
            if (allCameras[i] != this.GetComponent<Camera>())
            {
                otherCameras[count] = allCameras[i];
                count++;
            }
        }

        // Disable all other AudioListeners
        foreach (Camera camera in otherCameras)
        {
            AudioListener audioListener = camera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }

        // Debug log the number of other cameras found
    }
}
