using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderRadius : MonoBehaviour
{
    public KamController kc;
    public float thunderRange;
    [HideInInspector] public bool collIsActive;

    CircleCollider2D colliderr;
    [HideInInspector]public List<GameObject> enemy = new();
    Vector2 enemyHolder;
    float radius;

    private void Start()
    {
        colliderr = GetComponent<CircleCollider2D>();
        colliderr.radius = thunderRange;
        collIsActive = false;
    }
    private void FixedUpdate()
    {
        if (collIsActive)
        {
            radius = Mathf.Lerp(radius, thunderRange, 0.2f);
            colliderr.radius = radius;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemy.Add(other.gameObject);
        }
    }

    public List<GameObject> ClosestEnemy()
    {
        /*if (enemy.Count == 0)
        {
            enemyHolder = new Vector2(transform.position.x + (2.5f * kc.facingRight), transform.position.y - 1.6f);
        }
        else
        {
            enemyHolder = enemy[0].transform.position;
        }*/

        radius = 0;
        colliderr.radius = 0;
        colliderr.enabled = false;
        collIsActive = false;
        return enemy;
    }
}
