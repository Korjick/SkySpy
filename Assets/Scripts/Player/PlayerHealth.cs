using System;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IDamagable
    {
        public int health = 100;
        public Text healthText;
        public float killHeight = -50f;

        private int _currentHealth;
        
        void Awake()
        {
            _currentHealth = health;
            healthText.text = (_currentHealth + " / " + health);
        }
        
        public void GetDamage(int damage)
        {
            _currentHealth -= damage;
            healthText.text = (Mathf.Clamp(_currentHealth, 0, health) + " / " + health);

            if (_currentHealth <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void Update()
        {
            if(transform.position.y < killHeight) GetDamage(health);
        }
    }
}
