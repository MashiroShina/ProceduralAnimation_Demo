using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegCtrl : MonoBehaviour
{ 
    public Vector3 Default_Static_Targets_OBJPOS = Vector3.zero;
    public Vector3 restingPosition
    {
        get
        {
            //Default Static Targets :  worldpos->objPos
            return transform.TransformPoint(Default_Static_Targets_OBJPOS);
        }
    }
    public Vector3 worldVelocity = Vector3.zero;

    public Vector3 desiredPosition
    {
        get
        {
            return restingPosition + worldVelocity;
        }
    }
    public Vector3 worldTarget = Vector3.zero;
    public Transform ikTarget;
    public Transform ikHitPOS;

    public bool autoStep = true;

    public LayerMask solidLayer;
    public float stepRadius = 0.25f;
    public AnimationCurve stepHeightCurve;
    public float stepHeightMultiplier = 0.25f;
    public float stepCooldown = 1f;
    public float stepDuration = 0.5f;
    public float stepOffset;
    public float lastStep = 0;
    public float OffsetPlaneDistance=1;
    void Start()
    {
        worldVelocity = Vector3.zero;
        lastStep = Time.time + stepCooldown * stepOffset;
        //Debug.Log(lastStep + "__0");
        Step();
    }
    void Update()
    {
        UpdateIkTarget();
        if (Time.time > lastStep + stepCooldown && autoStep)
        {
            Step();
        }
    }
    public void UpdateIkTarget()
    {
        float percent = Mathf.Clamp01((Time.time - lastStep) / stepDuration);
        //Debug.Log(Time.time + "__2");
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldTarget, percent) + Vector3.up * stepHeightCurve.Evaluate(percent) * stepHeightMultiplier;
        //Debug.Log(percent + "__C:"+count++);
    }
    public void Step()
    {
        Vector3 direction = desiredPosition - ikHitPOS.position;
        RaycastHit hit;
        if (Physics.SphereCast(ikHitPOS.position, stepRadius, direction, out hit, direction.magnitude * 2f, solidLayer))
        {
            worldTarget = hit.point + new Vector3(0, OffsetPlaneDistance, 0);
        }
        else
        {
            worldTarget = restingPosition + new Vector3(0, OffsetPlaneDistance, 0);
        }
        lastStep = Time.time;
        //Debug.Log(lastStep + "__1");
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ikHitPOS.position, 0.1f);

        Vector3 direction = desiredPosition - ikHitPOS.position;
        RaycastHit hit;
        if (Physics.Raycast(ikHitPOS.position, direction, out hit, direction.magnitude * 2f, solidLayer))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ikHitPOS.position, hit.point);
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
