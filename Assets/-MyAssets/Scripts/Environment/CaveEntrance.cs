using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.inCave = true;
            AudioManager.instance.inCaveTimer = 1.5f;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.inCave = false;
            AudioManager.instance.inCaveTimer = 1.5f;
        }
    }
}
