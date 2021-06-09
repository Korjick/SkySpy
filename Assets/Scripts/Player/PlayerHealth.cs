using System;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Rendering;
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
        private bool _dead;
        
        public PlayerInputHandler input;
        public PlayerCharacterController controller;
        public GunSystem gun;

        public Button reloadButton;
        public Button menuButton;
        public Text playerText;
        public Canvas canvas;

        public Volume deadVolume;
        
        void Awake()
        {
            _currentHealth = health;
            healthText.text = (_currentHealth + " / " + health);
        }
        
        public void GetDamage(int damage)
        {
            _currentHealth -= damage;
            healthText.text = (Mathf.Clamp(_currentHealth, 0, health) + " / " + health);

            if (_currentHealth <= 0 && !_dead)
            {
                input.enabled = false;
                controller.enabled = false;
                gun.enabled = false;
                Cursor.lockState = CursorLockMode.None;
            
                playerText.text = "You Dead";
                playerText.color = Color.white;
                playerText.gameObject.SetActive(true);

                deadVolume.weight = 1;
                
                Button reload = Instantiate(reloadButton, canvas.transform);
                reload.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
                Button menu = Instantiate(menuButton, canvas.transform);
                menu.transform.position -= Vector3.up * 120;
                menu.onClick.AddListener(() => SceneManager.LoadScene(0));
                _dead = true;
            }
        }

        private void Update()
        {
            if(transform.position.y < killHeight) GetDamage(health);
        }
    }
}
