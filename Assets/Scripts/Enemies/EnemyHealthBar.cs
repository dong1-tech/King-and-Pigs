using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour, IHealthBar
{
    [SerializeField] private Canvas canvas;
    private Slider slider;

    void Awake()
    {
        slider = canvas.GetComponent<Slider>();
        slider.value = 1;
    }

    public void UpdateHealth(float healthRemain)
    {
        slider.value = healthRemain;
    }
}
