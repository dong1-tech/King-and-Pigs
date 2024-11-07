using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    public static UIManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void HealthBarUpdate(Collider2D other, float healthRemain)
    {
        IHealthBar healthBar = other.GetComponent<IHealthBar>();
        healthBar?.UpdateHealth(healthRemain);
    }

}
