using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject cam;
    public float parallaxAmount;

    float length, startPos;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate()
    {
        float dist = cam.transform.position.x * parallaxAmount;
        float temp = cam.transform.position.x * (1 - parallaxAmount);

        transform.position = new Vector2(startPos + dist, transform.position.y);
        transform.localPosition = new(transform.localPosition.x, 0, transform.localPosition.z);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
