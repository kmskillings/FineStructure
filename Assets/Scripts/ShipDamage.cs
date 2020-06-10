using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipDamage : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public UnityEvent OnDie;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnDie?.Invoke();
            //Destroy(this);
        }
    }
}
