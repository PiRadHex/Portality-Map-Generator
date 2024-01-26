using UnityEngine;
using UnityEngine.UI;

public class EnableObjectOnClick : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;

    private void Start()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("EnableObjectOnClick script requires a Button component on the same GameObject.");
        }
    }

    private void OnButtonClick()
    {
        // Enable the specified GameObject when the button is clicked
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No GameObject assigned to objectToEnable field.");
        }
    }
}
