using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class ToggleButtonEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent eventSet1;
    [SerializeField] private UnityEvent eventSet2;

    [SerializeField] private Image imageComponent;
    [SerializeField] private Sprite spriteForSet1;
    [SerializeField] private Sprite spriteForSet2;

    private Button button;
    private int currentEventSet = 1;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning("ToggleButtonEvents script requires a Button component on the same GameObject.");
            return;
        }

        // Assign the initial set of events and sprite
        AssignEvents(eventSet1);
        SetSprite(spriteForSet1);

        // Add a listener to the button to toggle between event sets
        button.onClick.AddListener(ToggleEvents);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        AssignEvents(eventSet1);
        SetSprite(spriteForSet1);
        currentEventSet = 1;
        button.onClick.AddListener(ToggleEvents);
    }

    private void ToggleEvents()
    {
        // Clear the existing events
        button.onClick.RemoveAllListeners();

        // Toggle between the two sets of events and sprites
        if (currentEventSet == 1)
        {
            AssignEvents(eventSet2);
            SetSprite(spriteForSet2);
            currentEventSet = 2;
        }
        else
        {
            AssignEvents(eventSet1);
            SetSprite(spriteForSet1);
            currentEventSet = 1;
        }

        button.onClick.AddListener(ToggleEvents);
    }

    private void AssignEvents(UnityEvent unityEvent)
    {
        button.onClick.AddListener(() => unityEvent.Invoke());
    }

    private void SetSprite(Sprite sprite)
    {
        if (imageComponent != null && sprite != null)
        {
            imageComponent.sprite = sprite;
        }
    }
}
