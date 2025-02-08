using System;
using UnityEngine;

public class CannonEnemies : MonoBehaviour, IHitable
{
    [SerializeField] private float enemiesStartAttackRange;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    private bool isShootingDone;

    private Rigidbody2D rigi;
    private Animator animator;

    private StateMachine enemyStateMachine;

    public Action OnShooting;

    private void Awake()
    {

        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        isShootingDone = true;
        enemyStateMachine = new StateMachine();

        var idleState = enemyStateMachine.CreateState("Idle");
        idleState.onEnter = delegate
        {
            animator.Play("Idle");
        };
        idleState.onFrame = delegate
        {
            StateChange();
        };

        var shootingState = enemyStateMachine.CreateState("Shooting");
        shootingState.onEnter = delegate
        {
            animator.Play("Shoot");
        };
        shootingState.onFrame = delegate
        {
            StateChange();
        };
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isRunnning)
        {
            return;
        }
        enemyStateMachine.Update();
    }
    private void StateChange()
    {
        Transform player = GameManager.Instance.player; 
        float distance = Vector2.Distance(player.position, transform.position);
        if (distance > enemiesStartAttackRange && isShootingDone)
        {
            enemyStateMachine.TransitionTo("Idle");
            return;
        }
        
        if (distance < enemiesStartAttackRange && IsAbleShooting(player))
        {
            isShootingDone = false;
            enemyStateMachine.TransitionTo("Shooting");
            return;
        }

        if (!IsAbleShooting(player))
        {
            enemyStateMachine.TransitionTo("Idle");
        }
    }

    private void IsShootingDone()
    {
        isShootingDone = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemiesStartAttackRange);
    }

    private bool IsAbleShooting(Transform player)
    {
        if(transform.right.x > 0)
        {
            return player.position.x < transform.position.x ? true : false;
        }
        else
        {
            return player.position.x > transform.position.x ? true : false;
        }
    }

    private void Attack()
    {
        OnShooting?.Invoke();
    }

    public void OnDead()
    {
        Destroy(gameObject);
        GameManager.Instance.NotifyOnDead(this.name);
    }

    public float OnHit()
    {
        if (currentHealth > 0)
        {
            currentHealth -= 1;
        }
        if (currentHealth == 0)
        {
            OnDead();
        }
        return (float)currentHealth / maxHealth;
    }
}



