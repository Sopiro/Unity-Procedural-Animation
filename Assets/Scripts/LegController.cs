using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private LegIK[] legs;

    private float maxTipWait = 0.8f;

    private bool readyChangeOrder = false;
    private bool order = true;

    private float bodyHeightBase = 1.2f;

    private void Start()
    {
    }

    private void Update()
    {
        UpdateBodyTransform();

        if (legs.Length < 2)
            return;

        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].TipDistance > maxTipWait)
            {
                order = i % 2 == 0;
                break;
            }
        }

        foreach (LegIK leg in legs)
        {
            leg.Movable = order;
            order = !order;
        }
        
        int index = order ? 0 : 1;

        if (readyChangeOrder && !legs[index].Animating)
        {
            order = !order;
            readyChangeOrder = false;
        }

        if (legs[index].Animating)
        {
            readyChangeOrder = true;
        }
    }

    public void UpdateBodyTransform()
    {
        float y = 0.0f;

        foreach (LegIK leg in legs)
        {
            y += leg.TipPos.y;
        }

        y /= legs.Length;
        y += bodyHeightBase;

        Vector3 pos = bodyTransform.position;

        pos.y = y;

        bodyTransform.position = pos;
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(bodyCenter, 0.2f);
    }
}
