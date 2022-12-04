using System;
using System.Collections;
using System.Collections.Generic;
using Core.GlobalEvents;
using Core.GlobalVariables;
using Unity.VisualScripting;
using UnityEngine;

public class Scratch : MonoBehaviour
{

    [SerializeField] private Animator animator;

    public bool doneAttacking = true;

    private void be()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }
        doneAttacking = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerCombat playerCombat = collision.GetComponent<PlayerCombat>();
        if (playerCombat)
        {
            playerCombat.TakeDamage(transform.position.x <= collision.gameObject.transform.position.x);
        }
    }
    

    public void ScratchAttack()
    {
        doneAttacking = false;
        animator.Play("Base Layer.Scratch");
    }


    private void Update()
    {
        if (doneAttacking)
        {
            gameObject.SetActive(false);
        }
    }
}