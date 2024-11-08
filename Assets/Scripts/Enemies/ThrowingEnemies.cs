using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Playables;

public class ThrowingEnemies : MonoBehaviour, IHitable
{
    [SerializeField] private float speed;

    [SerializeField] private float enemiesAlertRange;
    [SerializeField] private float enemiesStartAttackRange;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    private bool isThrowing = false;
    private bool isDead = false;

    [SerializeField] private Transform edgeDetect;
    [SerializeField] private LayerMask GroundLayer;

    private Rigidbody2D rigi;
    private Animator animator;

    [SerializeField] private BoxController box;

    private float lockTill;
    [SerializeField] private float lockThrowingTime;
    [SerializeField] private float lockPickingTime;

    private StateMachine enemyStateMachine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        enemyStateMachine = new StateMachine();

        var normalState = enemyStateMachine.CreateState("Normal");
        normalState.onEnter = delegate
        {
            animator.Play("Run");
        };
        normalState.onFrame = delegate
        {
            rigi.velocity = new Vector2(-transform.right.x * speed * Time.fixedDeltaTime, rigi.velocity.y);
            if (!EdgeDetect())
            {
                FlipEnemies();
            }
            StateChange();
        };

        var alertState = enemyStateMachine.CreateState("Alert");
        alertState.onEnter = delegate
        {
            animator.Play("Idle");
        };
        alertState.onFrame = delegate
        {
            rigi.velocity = new Vector2(0, rigi.velocity.y);
            StateChange();
        };

        var throwingState = enemyStateMachine.CreateState("Throwing");
        throwingState.onEnter = delegate
        {
            animator.Play("Throwing");
        };
        throwingState.onFrame = delegate
        {
            if (GameManager.Instance.player.position.x > transform.position.x)
            {
                transform.right = Vector3.left;
            }
            else
            {
                transform.right = Vector3.right;
            }
            StateChange();
        };

        var pickingState = enemyStateMachine.CreateState("Picking");
        pickingState.onEnter = delegate
        {
            animator.Play("Picking");
        };
        pickingState.onFrame = delegate
        {
            StateChange();
        };

        var deadState = enemyStateMachine.CreateState("Dead");
        deadState.onEnter = delegate
        {
            animator.Play("Dead");
        };
    }
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isRunnning) return;
        enemyStateMachine.Update();
    }
    private void StateChange()
    {
        if (isDead)
        {
            enemyStateMachine.TransitionTo("Dead");
            return;
        }
        if (Time.fixedTime < lockTill)
        {
            return;
        }
        float distance = Vector2.Distance(GameManager.Instance.player.position, transform.position);
        if(distance < enemiesStartAttackRange && !isThrowing)
        {
            enemyStateMachine.TransitionTo("Throwing");
            isThrowing = true;
            LockState(lockThrowingTime);
            return;
        }
        if (isThrowing)
        {
            enemyStateMachine.TransitionTo("Picking");
            isThrowing = false;
            LockState(lockPickingTime);
            return;
        }
        if (distance < enemiesAlertRange)
        {
            enemyStateMachine.TransitionTo("Alert");
        }else
        {
            enemyStateMachine.TransitionTo("Normal");
        }
    }
    void LockState(float lockTime)
    {
        lockTill = Time.fixedTime + lockTime;
    }
    private bool EdgeDetect()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, 0.4f, GroundLayer);
        return hit;
    }

    private void FlipEnemies()
    {
        transform.right = - transform.right;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemiesAlertRange);
        Gizmos.DrawWireSphere(transform.position, enemiesStartAttackRange);
    }
    
    private void OnThrowing()
    {
        Instantiate(box, transform);
    }

    public void OnDead()
    {
        isDead = true;
        GameManager.Instance.NotifyOnDead(this.name);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    public float OnHit()
    {
        if(currentHealth > 0)
        {
            currentHealth -= 1;
        }
        if(currentHealth == 0) 
        {
            OnDead();
        }
        return (float) currentHealth/maxHealth;
    }
}



