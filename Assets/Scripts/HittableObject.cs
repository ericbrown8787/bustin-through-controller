using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Audio")]
    public AudioClip[] impacts;
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"{this.name} has taken damage!");
        if (currentHealth <= 0 ) {
            Death();
                }
    }
    void Death()
    {
        // Replace this with whatever we make the breaking behavior
        Destroy(gameObject);
    }

    public AudioClip GetCurrentImpactSound()
    {
        return impacts[maxHealth - currentHealth];
    }
}
