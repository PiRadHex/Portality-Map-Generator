using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isDisabler = false;
    [SerializeField] Animator animator;
    [SerializeField] GameObject model;

    private void Start()
    {
        animator.enabled = false;
    }

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
            // disable animator after finishing its animation
            //Invoke("DisableAnimator", 0.6f);
        }

    }

    void DisableAnimator()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("DoorClose") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.enabled = false;
        }
    }

}
