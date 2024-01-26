using UnityEngine;

public class DisableAnimatorOnEnable : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private GameObject targetGameObject;

    /// <summary>
    /// Disables the specified targetAnimator when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        // Disable the targetAnimator when the script is enabled
        if (targetAnimator != null)
        {
            targetAnimator.enabled = false;
        }

        StartCoroutine(DisableGameObjectNextFrame());
    }

    private System.Collections.IEnumerator DisableGameObjectNextFrame()
    {
        // Wait for the next frame to ensure Animator is not in the middle of processing
        yield return null;

        if (targetGameObject != null)
        {
            targetGameObject.SetActive(false);
        }

        // Disable this script after performing the initial operation
        enabled = false;
    }

    /// <summary>
    /// Enables the targetAnimator when the script is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (targetGameObject != null && targetAnimator != null)
        {
            targetAnimator.enabled = true;
        }

        // Ensure the Coroutine is stopped when the script is disabled
        //StopAllCoroutines();
    }
}
