using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameRate : MonoBehaviour
{
    public bool showFPS = true;
    [SerializeField] TextMeshProUGUI fpsText;
    public float updateTimer = 0.2f;

    private float fpsTimer;

    void Update()
    {
        FPS_Handler(showFPS);

    }

    private void FPS_Handler(bool _showFPS)
    {
        if (_showFPS)
        {
            fpsTimer -= Time.deltaTime;
            if (fpsTimer <= 0f)
            {
                float fps = 1.0f / Time.smoothDeltaTime;
                fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
                fpsText.enabled = true;
                fpsTimer = updateTimer;
            }

        }
        else
        {
            fpsText.enabled = false;
        }
    }

    public void ToggleShowFPS()
    {
        showFPS = !showFPS;
    }
}
