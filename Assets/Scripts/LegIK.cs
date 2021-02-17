using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegIK : MonoBehaviour
{
    private LegController legController;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform rayOrigin;
    public GameObject ikTarget;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve heightCurve;

    private float tipMaxHeight = 0.2f;

    private float tipAnimationTime = 0.17f;
    private float tipAnimationFrameTime = 1 / 60.0f;

    public float IkOffset { get; } = 1.0f;
    private float tipMoveDist = 0.55f;
    private float maxRayDist = 7.0f;
    private float tipPassOver = 0.35f;

    public Vector3 TipPos { get; private set; }
    public Vector3 raycastTipPos { get; private set; }
    public Vector3 UpDir { get; private set; }

    public bool Animating { get; private set; } = false;
    public bool Movable { get; set; } = false;
    public float TipDistance { get; private set; }

    private void Awake()
    {
        legController = GetComponentInParent<LegController>();

        transform.parent = bodyTransform;
        rayOrigin.parent = bodyTransform;
        TipPos = ikTarget.transform.position;
    }

    private void Start()
    {
        UpdateIKTargetTransform();
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin.position, bodyTransform.up.normalized * -1, out hit, maxRayDist))
        {
            raycastTipPos = hit.point;
        }
        else
        {
            TipPos = raycastTipPos = rayOrigin.position + bodyTransform.up.normalized * -1 * maxRayDist;

            UpdateIKTargetTransform();

            return;
        }

        TipDistance = (raycastTipPos - TipPos).magnitude;

        if (!Animating && (TipDistance > tipMoveDist && Movable))
        {
            StartCoroutine(AnimateLegMove());
        }
    }

    private IEnumerator AnimateLegMove()
    {
        Animating = true;

        float timer = 0.0f;
        float animTime;

        Vector3 startingTipPos = TipPos;
        Vector3 tipDirVec = (raycastTipPos - TipPos);
        tipDirVec += tipDirVec.normalized * tipPassOver;

        Vector3 right = Vector3.Cross(tipDirVec.normalized, bodyTransform.up).normalized;
        UpDir = Vector3.Cross(right, tipDirVec.normalized);

        while (timer < tipAnimationTime + tipAnimationFrameTime)
        {
            animTime = speedCurve.Evaluate(timer / tipAnimationTime);

            float tipAcceleration = (raycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude;

            if (tipAcceleration < 1) tipAcceleration = 1.0f;

            TipPos = startingTipPos + tipDirVec * tipAcceleration * animTime; // Forward dir
            TipPos += UpDir * heightCurve.Evaluate(animTime) * tipMaxHeight; // Upward dir

            UpdateIKTargetTransform();

            timer += tipAnimationFrameTime;

            yield return new WaitForSeconds(tipAnimationFrameTime);
        }

        Animating = false;
    }

    private void UpdateIKTargetTransform()
    {
        ikTarget.transform.position = TipPos + bodyTransform.up.normalized * IkOffset;
        ikTarget.transform.rotation = Quaternion.LookRotation(TipPos - ikTarget.transform.position) * Quaternion.Euler(90, 0, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(raycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, raycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.transform.position, 0.1f);
    }
}
