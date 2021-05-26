using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IDamagable
    {
        public int health;
        public Text healthText;

        private int _currentHealth;
        
        void Awake()
        {
            _currentHealth = health;
            healthText.text = (_currentHealth + " / " + health);
        }
        
        public void GetDamage(int damage)
        {
            _currentHealth -= damage;
            healthText.text = (_currentHealth + " / " + health);

            if (_currentHealth <= 0)
            {
                Debug.Log("Die");
            }
        }
    }
}
