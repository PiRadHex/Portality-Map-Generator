using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isDisabler = false;
    [SerializeField] Animator animator;
    [SerializeField] GameObject model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isDisabler)
            {
                model.SetActive(false);
            }
            else if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("Open");
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !isDisabler && animator != null)
        {
            animator.SetTrigger("Close");
            model.SetActive(true);
        }

    }
}
