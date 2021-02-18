using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Leg[] legs;

    private float maxTipWait = 0.7f;

    private bool readyChangeOrder = false;
    private bool order = true;
    private float bodyHeightBase = 1.3f;

    private Vector3 bodyPos;
    private Vector3 bodyUp;
    private Vector3 bodyForward;
    private Vector3 bodyRight;
    private Quaternion bodyRotation;

    private float AdjustRatioPerTick = 1 / 10.0f;

    private void Start()
    {
        StartCoroutine(AdjustBodyTransform());
    }

    private void Update()
    {
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

        foreach (Leg leg in legs)
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

        if (!readyChangeOrder && legs[index].Animating)
        {
            readyChangeOrder = true;
        }
    }

    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            Vector3 tipCenter = Vector3.zero;
            bodyUp = Vector3.zero;

            foreach (Leg leg in legs)
            {
                tipCenter += leg.TipPos;
                bodyUp += leg.TipUpDir + leg.RaycastTipNormal;
            }

            RaycastHit hit;
            if (Physics.Raycast(bodyTransform.position, bodyTransform.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }

            tipCenter /= legs.Length;
            bodyUp.Normalize();

            bodyPos = tipCenter + bodyUp * bodyHeightBase;
            bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, AdjustRatioPerTick);

            bodyRight = Vector3.Cross(bodyUp, bodyTransform.forward);
            bodyForward = Vector3.Cross(bodyRight, bodyUp);

            bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
            bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, bodyRotation, AdjustRatioPerTick);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyRight);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyUp);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyForward);
    }
}
