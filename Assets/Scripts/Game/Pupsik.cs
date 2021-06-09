using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pupsik : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.SetInt("Demo", 0);
        PlayerPrefs.Save();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerPrefs.SetInt("Demo", PlayerPrefs.GetInt("Demo", 0) + 1);
            PlayerPrefs.Save();
            Debug.Log("number: " + PlayerPrefs.GetInt("Demo", 0));
            Destroy(gameObject);
        }
    }
}
