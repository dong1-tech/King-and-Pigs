using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour, IHealthBar
{
    [SerializeField] private List<GameObject> healthList;
    private Stack<GameObject> OnHealth;
    private Stack<GameObject> OffHealth;
    private Vector3 position;
    // Start is called before the first frame update

    void Awake()
    {
        OnHealth = new Stack<GameObject>();
        OffHealth = new Stack<GameObject>();
        foreach(var item in healthList)
        {
            OnHealth.Push(item);
        }
        
    }
    public void UpdateHealth(float healthRemain)
    {
        if (OnHealth.Count == 0) return;
        GameObject smallHealth = OnHealth.Pop();
        Animator animator = smallHealth.GetComponent<Animator>();
        animator.Play("Hit");
        OffHealth.Push(smallHealth);
        Invoke("MinusHealth", 0.4f);
    }

    private void MinusHealth()
    {
        OffHealth.Peek().SetActive(false);
    }
}
