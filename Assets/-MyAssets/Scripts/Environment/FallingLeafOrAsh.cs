using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLeafOrAsh : MonoBehaviour
{
    public GameObject fallingLeaf, fallingAsh;

    void Start()
    {
        if (LevelFinished.isDarkLevels)        
            Destroy(fallingLeaf);        
        else
            Destroy(fallingAsh);
    }
}
