using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinished : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //TODO bulya show "level finished" here
        }
    }
}
