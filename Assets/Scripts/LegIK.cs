using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LegIK : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform rayOrigin;
    public GameObject ikTarget;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve heightCurve;

    [SerializeField] private float animateLegHeight = 0.3f;

    [SerializeField] private float tipAnimationTime = 0.2f;
    [SerializeField] private float tipAnimationFrameTime = 1 / 60.0f;

    [SerializeField] private float ikOffset = 1.0f;
    [SerializeField] private float tipMoveDist = 0.5f;
    [SerializeField] private float maxRayDist = 5.0f;

    private Vector3 tipPos;
    private Vector3 raycastTipPos;

    private bool animating = false;

    void Start()
    {
        transform.parent = bodyTransform.transform;
        rayOrigin.parent = bodyTransform.transform;
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin.position, bodyTransform.up.normalized * -1, out hit, maxRayDist))
        {
            raycastTipPos = hit.point;
        }
        else
        {
            tipPos = raycastTipPos = rayOrigin.position + bodyTransform.up.normalized * -1 * maxRayDist;

            UpdateIKTargetTransform();

            return;
        }

        if ((raycastTipPos - tipPos).magnitude > tipMoveDist && !animating)
        {
            StartCoroutine(AnimateLegMove());
        }
    }

    private IEnumerator AnimateLegMove()
    {
        animating = true;

        float timer = 0.0f;
        float animTime;

        Vector3 startingTipPos = tipPos;
        Vector3 tipDirVec = (raycastTipPos - tipPos);

        Vector3 right = Vector3.Cross(tipDirVec.normalized, bodyTransform.up).normalized;
        Vector3 up = Vector3.Cross(right, tipDirVec.normalized);

        while (timer < tipAnimationTime + tipAnimationFrameTime)
        {
            animTime = speedCurve.Evaluate(timer / tipAnimationTime);

            float tipAcceleration = (raycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude;

            tipPos = (startingTipPos + tipDirVec * tipAcceleration * animTime); // Forward dir
            tipPos += (up * heightCurve.Evaluate(animTime) * animateLegHeight); // Upward dir

            UpdateIKTargetTransform();

            timer += tipAnimationFrameTime;

            yield return new WaitForSeconds(tipAnimationFrameTime);
        }

        animating = false;
    }

    private void UpdateIKTargetTransform()
    {
        ikTarget.transform.position = tipPos + bodyTransform.up.normalized * ikOffset;
        ikTarget.transform.rotation = Quaternion.LookRotation(tipPos - ikTarget.transform.position) * Quaternion.Euler(90, 0, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(raycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(tipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(tipPos, raycastTipPos);
    }
}
