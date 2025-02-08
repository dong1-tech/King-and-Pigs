using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private Rigidbody2D rigi;
    private Animator animator;
    private CircleCollider2D circleCollider;
    [SerializeField] private float explodeRadius;
    [SerializeField] private LayerMask playerLayerMask;
    private void Awake()
    {
        rigi = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        Transform player = GameManager.Instance.player;
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);
        float velocity = distanceX * Mathf.Sqrt(9.81f / (2 * distanceY));
        Transform direction = GetComponentInParent<Transform>();
        rigi.velocity = new Vector2(velocity * -direction.right.x, 0);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            GameManager.Instance.NotifyOnHit(collision.collider);
        }
        animator.Play("Explode");
    }

    void DamageOnExplode()
    {
        circleCollider.radius = explodeRadius;
    }

    void DestroyCannonBall()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
