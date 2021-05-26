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

        public int damage = 20;
        public float timeBetweenShooting = .05f, spread = .02f, range = 100, reloadTime = 1, timeBetweenShots = 0;
        public int magazineSize = 30, bulletsPerTap = 2, bulletHoleLifeTime = 3;
        public bool allowButtonHold = true;

        private int _bulletsLeft, _bulletsShot;
        private bool _shooting, _readyToShoot, _reloading, _particleActivated;
        private Queue<GameObject> _bulletHoles;

        public Camera mainCamera;
        public Transform[] attackPoints;
        public RaycastHit rayHit;

        public GameObject bulletHoleGraphic;
        public GameObject[] muzzleFlash;
        public Text text, pressText;

        private void Awake()
        {
            _bulletsLeft = magazineSize;
            _readyToShoot = true;
            _bulletHoles = new Queue<GameObject>();
        }

        private void Update()
        {
            InputCheck();

            if (text) text.text = (_bulletsLeft + " / " + magazineSize);
        }

        private void InputCheck()
        {
            _shooting = allowButtonHold ? playerInputHandler.GetFireInputHold() : playerInputHandler.GetFireInputDown();

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
            
            if (playerInputHandler.GetReloadInputDown() && _bulletsLeft < magazineSize && !_reloading)
            {
                Reload();
            }
            
            if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0)
            {
                _bulletsShot = bulletsPerTap;
                Shoot();
            }
        }

        private void Shoot()
        {
            // Графика

            foreach (var flash in muzzleFlash)
            {
                if (!_particleActivated)
                {
                    flash.SetActive(true);
                }
            }
            _particleActivated = true;
            
            if (_bulletHoles.Count < 10)
            {
                _bulletHoles.Enqueue(Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0)));
                StartCoroutine(DestroyBulletHole());
            }
            else
            {
                var hole = _bulletHoles.Peek();
                hole.transform.position = rayHit.point;
                hole.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            _readyToShoot = false;

            var x = Random.Range(-spread, spread);
            var y = Random.Range(-spread, spread);

            var direction = mainCamera.transform.forward + new Vector3(x, y, 0);

            //RayCast
            if (Physics.Raycast(mainCamera.transform.position, direction, out rayHit, range))
            {
                Debug.Log(rayHit.collider.name);

                if (rayHit.collider.GetComponent<IDamagable>() != null)
                    rayHit.collider.GetComponent<IDamagable>().GetDamage(damage);
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

        private void Reload()
        {
            if (pressText) pressText.text = "Reloading...";
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

        private IEnumerator DestroyBulletHole()
        {
            yield return new WaitForSeconds(bulletHoleLifeTime);
            Destroy(_bulletHoles.Dequeue());
        }
    }
}