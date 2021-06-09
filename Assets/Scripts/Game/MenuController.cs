using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController menuController;

    public Canvas menu, map;
    
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
}
