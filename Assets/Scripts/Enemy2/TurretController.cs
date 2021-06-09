using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private bool isAiming;
    public GameObject player;
    public GunSystem gunSystem;
    private void Update()
    {
        if (isAiming)
        {
            Vector3 lookPos = player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookPos);

            if (gunSystem.BulletsLeft > 0)
            {
                gunSystem.Shoot();
            }
            else
            {
                gunSystem.Reload();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            isAiming = true;
        }
    }
}
