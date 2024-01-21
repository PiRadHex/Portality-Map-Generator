using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorUI : MonoBehaviour
{
    [SerializeField] TMP_InputField seedInput;
    [SerializeField] GameObject editorPanel;
    [SerializeField] CinemachineVirtualCamera VCam;
    [SerializeField] List<GameObject> joysticks = new List<GameObject>();

    public bool isTouchControl = false;

    private void Start()
    {
        seedInput.text = SeedGenerator.Instance.GetSeed().ToString();
        editorPanel.SetActive(false);
        VCam.enabled = false;
    }

    public void RefreshUI()
    {
        seedInput.text = SeedGenerator.Instance.GetSeed().ToString();
    }

    public void UpdateUI()
    {
        int.TryParse(seedInput.text, out int seed);
        if (seedInput.text != "Enter Seed...") { SeedGenerator.Instance.SetCustomSeed(seed); }
        seedInput.text = SeedGenerator.Instance.GetSeed().ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleEditMode();
        }

    }

    public void ToggleEditMode()
    {
        editorPanel.SetActive(!editorPanel.activeInHierarchy);
        VCam.enabled = editorPanel.activeInHierarchy;
        if (Application.platform != RuntimePlatform.Android)
        {
            Cursor.visible = editorPanel.activeInHierarchy;
            if (!isTouchControl) Cursor.lockState = editorPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
        }
        foreach(var joystick in joysticks)
        {
            joystick.SetActive(!editorPanel.activeInHierarchy);
        }
    }

    public void ToggleTouchControl()
    {
        isTouchControl = !isTouchControl;
        Cursor.lockState = isTouchControl ? CursorLockMode.None : (editorPanel.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked);
    }

}
