using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaursController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("DinoWander");
    }

    public void KillDinos()
    {
        Debug.Log("in KillDinos");
        animator.Play("DinoDie");
    }

    public void ComeBackDinos()
    {
        Debug.Log("in ComeBackDinos");
        animator.Play("DinoWander");
    }
}
