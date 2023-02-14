using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffChain : MonoBehaviour
{
    public Transform staff;

    void Update()
    {
        transform.position = staff.position;        
    }
}
