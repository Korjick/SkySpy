using System.Collections;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        public int bulletLifeTime = 10;
        public int damage = 10;
        public string tag = "Player";
        
        void Awake()
        {
            StartCoroutine(DestroyBullet());
        }

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.collider.name);
        
            if (collision.collider.gameObject.GetComponent<IDamagable>() != null && !collision.collider.transform.CompareTag(tag))
                collision.collider.gameObject.GetComponent<IDamagable>().GetDamage(damage);
        
            Destroy(gameObject);
        }
    
        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(bulletLifeTime);
            Destroy(gameObject);
        }
    }
}
