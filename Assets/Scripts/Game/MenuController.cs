using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController menuController;

    public Canvas menu, map;
    public Button demo;
    
    private void Awake()
    {
        if(menuController) Destroy(gameObject);
        menuController = this;
    }

    public void ShowMap(bool show)
    {
        map.gameObject.SetActive(show);
    }
    
    public void LoadLevel(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void SecretLevel()
    {
        if (PlayerPrefs.HasKey("Demo"))
        {
            demo.onClick.AddListener(() => SceneManager.LoadScene(2));
        }
    }
}
