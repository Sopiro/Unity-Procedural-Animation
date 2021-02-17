using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float MoveSpeed { get; } = 4.0f;
    private float RotSpeed { get; } = 100.0f;

    void Start()
    {

    }

    void Update()
    {
        float ws = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;

        transform.Translate(0, 0, ws);

        float ad = Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;

        transform.Translate(ad, 0, 0);

        if (Input.GetKey(KeyCode.Q)) transform.Rotate(0, -RotSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E)) transform.Rotate(0, RotSpeed * Time.deltaTime, 0);
    }
}
