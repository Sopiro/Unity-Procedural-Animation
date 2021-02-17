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

    private Vector3 bodyPos;
    private Vector3 bodyUp;

    private void Start()
    {
        StartCoroutine(UpdateBodyTransform());
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

        if (!readyChangeOrder && legs[index].Animating)
        {
            readyChangeOrder = true;
        }
    }

    public IEnumerator UpdateBodyTransform()
    {
        while (true)
        {
            Vector3 tipCenter = Vector3.zero;

            foreach (LegIK leg in legs)
            {
                tipCenter += leg.TipPos;
            }

            tipCenter /= legs.Length;

            bodyPos = tipCenter + bodyTransform.up * bodyHeightBase;

            bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, 1 / 20.0f);

            RaycastHit hit;
            if (Physics.Raycast(bodyTransform.position, bodyTransform.up * -1, out hit, 5.0f))
            {
                bodyUp = hit.normal;
            }

            bodyTransform.up = Vector3.Slerp(bodyTransform.up, bodyUp, 1 / 20.0f);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(bodyPos, 0.2f);
    }
}
