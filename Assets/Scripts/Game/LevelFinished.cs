using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFinished : MonoBehaviour
{

    public Text text;
    public PlayerInputHandler input;
    public PlayerCharacterController controller;
    public GunSystem gun;

    public Button menuButton;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            input.enabled = false;
            gun.enabled = false;
            controller.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            
            text.text = "Level Finished";
            text.color = Color.white;
            text.gameObject.SetActive(true);

            Button menu = Instantiate(menuButton, text.transform.parent.parent);
            menu.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
