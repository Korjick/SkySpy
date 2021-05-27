using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    public int maxHealth;
    private int _currentHealth;
    public Image healthBarImage;
    public Transform healthBarTransform;
    public Transform playerTransform;

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        Vector3 lookPos = playerTransform.position - healthBarTransform.position;
        healthBarTransform.rotation = Quaternion.LookRotation(lookPos);
    }

    public void GetDamage(int damage)
    {
        Debug.Log("enemy get damage");
        _currentHealth -= damage;
        healthBarImage.fillAmount = _currentHealth / maxHealth;

        if (_currentHealth <= 0)
        {
            Debug.Log("Die");
        }
    }
}