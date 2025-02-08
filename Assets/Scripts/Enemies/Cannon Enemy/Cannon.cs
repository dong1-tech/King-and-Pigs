using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private CannonEnemies cannonEnemy;
    [SerializeField] private CannonBall cannonBall;
    [SerializeField] private Transform shootPoint;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init()
    {
        cannonEnemy.OnShooting += CannonShooting;
    }

    private void OnDestroy()
    {
        cannonEnemy.OnShooting -= CannonShooting;
    }

    private void CannonShooting()
    {
        animator.Play("Shoot");  
    }
    private void CannonBallShooting()
    {
        Instantiate(cannonBall, shootPoint.position, shootPoint.rotation);
    }
    private void CannonEndShooting()
    {
        animator.Play("Idle");
    }
}
