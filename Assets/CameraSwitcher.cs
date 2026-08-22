using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public List<Camera> cameras = new List<Camera>();

    [Header("UI")]
    public TMP_Text cameraLabel;

    private int currentIndex = 0;

    private void Start()
    {
        if (cameras.Count == 0)
        {
            Debug.LogWarning("[CameraSwitcher] No cameras assigned.");
            return;
        }
        SetActiveCamera(currentIndex);
    }

    public void NextCamera()
    {
        if (cameras.Count == 0) return;
        currentIndex = (currentIndex + 1) % cameras.Count;
        SetActiveCamera(currentIndex);
    }

    public void PreviousCamera()
    {
        if (cameras.Count == 0) return;
        currentIndex = (currentIndex - 1 + cameras.Count) % cameras.Count;
        SetActiveCamera(currentIndex);
    }

    private void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
        }
        UpdateCameraLabel(index);
    }

    private void UpdateCameraLabel(int index)
    {
        if (cameraLabel == null) return;

        string cameraName = (index == 0) ? "Main Camera" : cameras[index].name;
        cameraLabel.text = cameraName;
    }
}