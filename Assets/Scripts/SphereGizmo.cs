using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGizmo : MonoBehaviour
{
    public static bool Enabled = true;

    public float size = 0.1f;
    public Color color = Color.red;

    // Draw sphere gizmo in the scene view
    private void OnDrawGizmos()
    {
        if (Enabled)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, size);
        }
    }
}
