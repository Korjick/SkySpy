using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public int bulletLifeTime = 10;
    public int damage = 10;
    
    void Awake()
    {
        StartCoroutine(DestroyBullet());
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        
        if (collision.collider.GetComponent<IDamagable>() != null && !collision.collider.CompareTag(transform.tag))
            collision.collider.GetComponent<IDamagable>().GetDamage(damage);
        
        Destroy(gameObject);
    }
    
    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(gameObject);
    }
}
