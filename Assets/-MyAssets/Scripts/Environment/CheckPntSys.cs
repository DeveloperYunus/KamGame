using UnityEngine;

public class CheckPntSys : MonoBehaviour
{
    [SerializeField] public static Vector2 checkPnt;

    private void Start()
    {
        checkPnt = GameObject.Find("Kam").transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && checkPnt.x < gameObject.transform.position.x)
            checkPnt = gameObject.transform.position;
    }
}
