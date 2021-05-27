using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class GunSystem : MonoBehaviour
    {
        public PlayerInputHandler playerInputHandler;
        public bool affectedByPlayer = true;

        public int damage = 20;
        public float timeBetweenShooting = .05f, spread = .02f, range = 100, reloadTime = 1, timeBetweenShots = 0;
        public int magazineSize = 30, bulletsPerTap = 2, bulletHoleLifeTime = 3;
        public bool allowButtonHold = true;

        private int _bulletsLeft, _bulletsShot;
        private bool _shooting, _readyToShoot, _reloading, _particleActivated;

        public Transform head;
        public Transform[] attackPoints;
        public Bullet bullet;
        public float bulletSpeed = 100;
        
        public GameObject[] muzzleFlash;
        public Text text, pressText;

        public int BulletsLeft => _bulletsLeft;

        private void Awake()
        {
            _bulletsLeft = magazineSize;
            _readyToShoot = true;
            _bulletsShot = bulletsPerTap;
        }

        private void Update()
        {
            if (_bulletsLeft <= 0)
            {
                foreach (var flash in muzzleFlash)
                {
                    flash.SetActive(false);
                }
                _particleActivated = false;
                if(pressText) pressText.gameObject.SetActive(true);
            }
            
            if (!_shooting)
            {
                foreach (var flash in muzzleFlash)
                {
                    flash.SetActive(false);
                }
                _particleActivated = false;
            }
            
            if(affectedByPlayer) InputCheck();

            if (text) text.text = (_bulletsLeft + " / " + magazineSize);
        }

        private void InputCheck()
        {
            _shooting = allowButtonHold ? playerInputHandler.GetFireInputHold() : playerInputHandler.GetFireInputDown();

            if (playerInputHandler.GetReloadInputDown())
            {
                Reload();
            }
            
            if (_shooting)
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            if(!_readyToShoot || _reloading || _bulletsLeft <= 0) return;
            
            // Графика
            foreach (var flash in muzzleFlash)
            {
                if (!_particleActivated)
                {
                    flash.SetActive(true);
                }
            }
            _particleActivated = true;
            _readyToShoot = false;

            var x = Random.Range(-spread, spread);
            var y = Random.Range(-spread, spread);

            var direction = head.transform.forward + new Vector3(x, y, 0);

            // Bullet
            foreach (var point in attackPoints)
            {
                Bullet cur = Instantiate(bullet, point.position, Quaternion.Euler(0, 90, 0));
                cur.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            }

            _bulletsLeft--;
            _bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);

            if (_bulletsShot > 0 && _bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }

        private void ResetShot()
        {
            _readyToShoot = true;
        }

        public void Reload()
        {
            
            if(_bulletsLeft >= magazineSize || _reloading) return;
            _bulletsShot = bulletsPerTap;
            
            if (pressText)
            {
                pressText.gameObject.SetActive(true);
                pressText.text = "Reloading...";
            }
            _reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }

        private void ReloadFinished()
        {
            _bulletsLeft = magazineSize;
            _reloading = false;
            if (pressText)
            {
                pressText.gameObject.SetActive(false);
                pressText.text = "Press R to reload";
            }
        }
    }
}