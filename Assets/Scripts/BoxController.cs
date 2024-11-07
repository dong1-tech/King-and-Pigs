using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    Rigidbody2D rigi;
    float timer;
    // Start is called before the first frame update
    private void Awake()
    {
        float x;
        if(GameManager.Instance.player.position.x < transform.position.x)
        {
            x = -1;
        }
        else
        {
            x = 1;
        }
        rigi = GetComponent<Rigidbody2D>();
        float L = Mathf.Abs(GameManager.Instance.player.position.x - transform.position.x);
        float a = Mathf.Acos(Mathf.Sqrt(L / 8 * 1));
        float v = Mathf.Sqrt(2 * 9.81f * 1) / Mathf.Sin(a);
        rigi.velocity = new Vector2(Mathf.Cos(a) * x, Mathf.Sin(a)) * v;
        timer = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            GameManager.Instance.NotifyOnHit(other);
        }
        if(Time.time - timer > 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
