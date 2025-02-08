using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

public class TerrorismEnemies : MonoBehaviour
{
    [SerializeField] private float normalSpeed;
    [SerializeField] private float chasingSpeed;

    [SerializeField] private float enemiesChaseRange;
    [SerializeField] private float enemiesStartExplodeRange;
    [SerializeField] private float explodeRadius;

    [SerializeField] private Transform edgeDetect;
    [SerializeField] private LayerMask GroundLayer;

    private Rigidbody2D rigi;
    private Animator animator;
    [SerializeField] private Collider2D playerCollider; 

    private StateMachine enemies;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        enemies = new StateMachine();

        var normalState = enemies.CreateState("Normal");
        normalState.onEnter = delegate
        {
            animator.Play("Run");
        };
        normalState.onFrame = delegate
        {
            rigi.velocity = new Vector2(-transform.right.x * normalSpeed * Time.fixedDeltaTime, rigi.velocity.y);
            if (!EdgeDetect())
            {
                FlipEnemies();
            }
            float distance = Vector2.Distance(GameManager.Instance.player.position, transform.position);
            if(distance < enemiesChaseRange)
            {
                enemies.TransitionTo("Chasing");
            }
        };
        var chasingState = enemies.CreateState("Chasing");
        chasingState.onEnter = delegate
        {
            animator.Play("Run");
            animator.speed = 3;
            Invoke("ExplodeCountDown", 4);
        };
        chasingState.onFrame = delegate
        {
            if (GameManager.Instance.player.position.x > transform.position.x)
            {
                rigi.velocity = new Vector2(chasingSpeed * Time.fixedDeltaTime, rigi.velocity.y);
                transform.right = Vector3.left;
            }
            else
            {
                rigi.velocity = new Vector2(-chasingSpeed * Time.fixedDeltaTime, rigi.velocity.y);
                transform.right = Vector3.right;
            }
            float distance = Vector2.Distance(GameManager.Instance.player.position, transform.position);
            if (distance < enemiesStartExplodeRange)
            {
                enemies.TransitionTo("Explode");

            }
        };
        var explodeState = enemies.CreateState("Explode");
        explodeState.onEnter = delegate
        {
            animator.Play("Explode");
        };
        explodeState.onFrame = delegate
        {
            float distance = Vector2.Distance(GameManager.Instance.player.position, transform.position);
            if (distance < explodeRadius)
            {
                GameManager.Instance.NotifyOnHit(playerCollider);
            }
        };
    }

    private void ExplodeCountDown()
    {
        enemies.TransitionTo("Explode");
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isRunnning) return;
        enemies.Update();
    }
    private bool EdgeDetect()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, 0.2f, GroundLayer);
        return hit;
    }

    private void FlipEnemies()
    {
        transform.right = -transform.right;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemiesChaseRange);
        Gizmos.DrawWireSphere(transform.position, enemiesStartExplodeRange);
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }

    private void OnDead()
    {
        Destroy(gameObject);
        GameManager.Instance.NotifyOnDead(this.name);
    }
}


