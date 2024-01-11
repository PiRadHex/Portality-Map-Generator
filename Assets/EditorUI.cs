using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    [SerializeField] TMP_InputField seedInput;
    [SerializeField] GameObject editorPanel;
    [SerializeField] CinemachineVirtualCamera VCam;

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
            editorPanel.SetActive(!editorPanel.activeInHierarchy);
            VCam.enabled = editorPanel.activeInHierarchy;
        }
        
    }

}
