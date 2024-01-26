using UnityEngine;
using UnityEngine.UI;

public class DisableObjectOnClick : MonoBehaviour
{
    [SerializeField] private GameObject objectToDisable;

    private void Start()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("DisableObjectOnClick script requires a Button component on the same GameObject.");
        }
    }

    private void OnButtonClick()
    {
        // Disable the specified GameObject when the button is clicked
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to objectToDisable field.");
        }
    }
}
