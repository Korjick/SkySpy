using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapElement : MonoBehaviour
{
    private void OnMouseEnter()
    {
        transform.position += Vector3.up;
    }

    private void OnMouseExit()
    {
        transform.position -= Vector3.up;
    }
}
