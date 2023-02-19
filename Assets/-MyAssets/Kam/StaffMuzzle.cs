using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffMuzzle : MonoBehaviour
{
    public KamAttack ka;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            ka.muzzleInGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            ka.muzzleInGround = false;
        }
    }
}
