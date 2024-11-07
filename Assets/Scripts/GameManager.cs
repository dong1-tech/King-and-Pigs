using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform player;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void NotifyOnHit(Collider2D objectGetHit)
    {
        IHitable hitable = objectGetHit.GetComponent<IHitable>();
        float remainHealth = (float)hitable?.OnHit();
        UIManager.Instance.HealthBarUpdate(objectGetHit, remainHealth);
    }
}
