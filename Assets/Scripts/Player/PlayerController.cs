using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour, IHitable
{
    //============= Health ================ 
    [SerializeField] private const int maxHealth = 3;
    private int currentHealth;

    //============= Move left, right; Jump =============
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    private Vector2 direction;

    //============== Wall check and sliding ==============
    [SerializeField] private float sldingSpeed;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallMask;

    //============= Attack =============
    private bool isResetAttack;
    private bool isAttack;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask enemiesLayer;

    //============= Hit and Dead ===============
    private bool isHit;
    private bool isDead;

    //============== Grounded Check ===============
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    //============= Physics ===============
    public Rigidbody2D rigi;

    //============= Input System ==============
    private PlayerInput playerInput;
    private InputAction move;
    private InputAction jump;
    private InputAction attack;

    //============= Player State ============
    private Animator animator;
    private float lockTill;
    private enum PlayerState
    {
        idle, run, fall, jump, attack, hit, dead
    }
    private PlayerState currentState;


    private void Awake()
    {
        playerInput = new PlayerInput();
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        rigi.gravityScale = 2;
        currentState = PlayerState.idle;
        currentHealth = maxHealth;
        isHit = false;
        isDead = false;
        isResetAttack = true;
    }

    private void OnEnable()
    {
        
        move = playerInput.Player.Move;
        move.Enable();

        jump = playerInput.Player.Jump;
        jump.Enable();
        jump.performed += OnJump;

        attack = playerInput.Player.Attack;
        attack.Enable();
        attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        attack.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        SelecState();
        PlayState();
    }

    private void FixedUpdate()
    {
        WallSliding();
        OnRun();
    }


    #region Handler Player State

    void ChangeState(PlayerState state)
    {
        if (currentState == state) return;
        currentState = state;
    }

    void PlayState()
    {
        switch (currentState)
        {
            case PlayerState.idle:
                animator.Play("Idle");
                break;
            case PlayerState.jump:
                animator.Play("Jump");
                break;
            case PlayerState.fall:
                animator.Play("Fall");
                break;
            case PlayerState.run:
                animator.Play("Run");
                break;
            case PlayerState.attack:
                animator.Play("Attack");
                break;
            case PlayerState.dead:
                animator.Play("Dead");
                break;
            case PlayerState.hit:
                animator.Play("Hit");
                break;
        }
    }

    void SelecState()
    {
        if (isDead)
        {
            ChangeState(PlayerState.dead);
            return;
        }
        if (isHit)
        {
            ChangeState(PlayerState.hit);
            return;
        }
        if (isAttack) return;
        if (attack.WasPerformedThisFrame() && isResetAttack)
        {
            ChangeState(PlayerState.attack);
            isAttack = true;
            isResetAttack = false;
            Invoke("AttackReset", 1.5f);
            return;
        }
        if(rigi.velocity == Vector2.zero)
        {
            ChangeState(PlayerState.idle);
        } 
        else if(rigi.velocity.x != 0 && rigi.velocity.y == 0)
        {
            ChangeState(PlayerState.run);
        }
        else if(rigi.velocity.y > 0)
        {
            ChangeState(PlayerState.jump);
        }
        else if(rigi.velocity.y < 0)
        {
            ChangeState(PlayerState.fall);
        }
    }

    void AttackDone()
    {
        isAttack = false;
    }
    void HitDone()
    {
        isHit = false;
    }
    void AttackReset()
    {
        isResetAttack = true;
    }
    public float OnHit()
    {
        int damage = 0;
        if (currentHealth > 0 && !isHit)
        {
            currentHealth -= 1;
            isHit = true;
            damage += 1;
        }
        if (currentHealth == 0)
        {
            OnDead();
        }
        return damage;
    }

    private void OnDead()
    {
        isDead = true;
    }
    #endregion

    #region Player Physics
    void OnRun()
    {
        if (isAttack || isHit) return;
        direction = move.ReadValue<Vector2>();
        //============= Flip player ===============
        if (direction.x < 0)
        {
            transform.right = new Vector3(-1f, 0, 0);
        }
        else if (direction.x == 0)
        {
            return;
        }
        else
        {
            transform.right = new Vector3(1f, 0, 0);
        }
        //============== Move Player ===============
        rigi.velocity = new Vector2(direction.x * Time.fixedDeltaTime * moveSpeed, rigi.velocity.y);
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundMask);
    }
    
    private bool WallCheck()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallMask);
    }

    private void WallSliding()
    {
        if(WallCheck() && !GroundCheck() && Input.GetAxis("Horizontal") != 0)
        {
            rigi.velocity = new Vector2(rigi.velocity.x, sldingSpeed);
        }
    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (isAttack || isHit) return;
        if (GroundCheck() )
        {
            rigi.velocity = new Vector2(rigi.velocity.x, jumpSpeed * Time.fixedDeltaTime);
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (isResetAttack)
        {
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemiesLayer);
            foreach (var hitableObject in hitObjects)
            {
                GameManager.Instance.NotifyOnHit(hitableObject);
            }
        }  
    }

    #endregion

}
