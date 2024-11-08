using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

public class MeleeEnemies : MonoBehaviour, IHitable
{
    [SerializeField] private float normalSpeed;
    [SerializeField] private float chasingSpeed;

    [SerializeField] private float enemiesChaseRange;
    [SerializeField] private float enemiesKeepChaseRange;
    [SerializeField] private float enemiesStartAttackRange;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    private bool isChasing;

    [SerializeField] private Transform edgeDetect;
    [SerializeField] private LayerMask GroundLayer;

    private bool isDead;

    private Rigidbody2D rigi;
    private Animator animator;
    

    private StateMachine enemies;

    private void Awake()
    {
        isChasing = false;
        isDead = false; 
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        enemies = new StateMachine();

        var normalState = enemies.CreateState("Normal");
        normalState.onEnter = delegate
        {
            animator.Play("Run");
        };
        normalState.onFrame = delegate 
        {
            rigi.velocity = new Vector2(- transform.right.x * normalSpeed * Time.fixedDeltaTime, rigi.velocity.y);
            if(!EdgeDetect())
            {
                FlipEnemies();
            }
            StateChange();
        };

        var alertState = enemies.CreateState("Alert");
        alertState.onEnter = delegate
        {
            animator.Play("Idle");
        };
        alertState.onFrame = delegate
        {
            rigi.velocity = new Vector2(0, rigi.velocity.y);
            StateChange();
        };

        var chasingState = enemies.CreateState("Chasing");
        chasingState.onEnter = delegate
        {
            animator.Play("Run");
        };
        chasingState.onFrame = delegate
        {
           if(GameManager.Instance.player.position.x > transform.position.x)
           {
                rigi.velocity = new Vector2(chasingSpeed * Time.fixedDeltaTime, rigi.velocity.y);
                transform.right = Vector3.left;
           }
           else
           {
                rigi.velocity = new Vector2(- chasingSpeed * Time.fixedDeltaTime, rigi.velocity.y);
                transform.right = Vector3.right;
           }
           StateChange();
        };
        var attackState = enemies.CreateState("Attack");
        attackState.onEnter = delegate
        {
            animator.Play("Attack");
        };
        attackState.onFrame = delegate
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
        var deadState = enemies.CreateState("Dead");
        deadState.onEnter = delegate
        {
            animator.Play("Dead");
        };
    }
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isRunnning) return;
        enemies.Update();
    }
    private void StateChange()
    {
        if (isDead)
        {
            enemies.TransitionTo("Dead");
            return;
        }
        float distance = Vector2.Distance(GameManager.Instance.player.position, transform.position);
        if(distance < enemiesStartAttackRange)
        {
            enemies.TransitionTo("Attack");
        }else if(distance < enemiesChaseRange && GameManager.Instance.player.position.y > transform.position.y)
        {
            enemies.TransitionTo("Chasing");
            isChasing = true;
        }else if(distance > enemiesKeepChaseRange)
        {
            enemies.TransitionTo("Normal");
            isChasing = false;
        }
        else 
        {
            if (!isChasing)
            {
                enemies.TransitionTo("Alert");
            }
        }
    }
    private bool EdgeDetect()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetect.position, Vector2.down, 0.2f, GroundLayer);
        return hit;
    }

    private void FlipEnemies()
    {
        transform.right = - transform.right;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemiesChaseRange);
        Gizmos.DrawWireSphere(transform.position, enemiesKeepChaseRange);
        Gizmos.DrawWireSphere(transform.position, enemiesStartAttackRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnAttack()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(attackPoint.position, attackRange); 
        foreach (Collider2D collider in others)
        {
            if(collider.name == "Player")
            {
                GameManager.Instance.NotifyOnHit(collider);
            }
        }
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

    public void OnDead()
    {
        isDead = true;
        GameManager.Instance.NotifyOnDead(this.name);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
    
    
