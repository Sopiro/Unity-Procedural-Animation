using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float Speed { get; } = 3.5f;

    void Start()
    {

    }

    void Update()
    {
        float ws = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

        transform.Translate(0, 0, ws);

        float ad = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

        transform.Translate(ad, 0, 0);
    }
}
