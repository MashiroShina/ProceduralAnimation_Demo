using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCtrl : MonoBehaviour
{
    public LegCtrl[] legs;
    private int index;
    public bool dynamicGait = false;
    public float timeBetweenSteps = 0.25f;
    public float lastStep = 0;
    public float maxTargetDistance = 1f;
    public float averageRotationRadius = 3f;
    private float rSpeed = 10;
    private Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ctrl();
        if (dynamicGait)
        {
            timeBetweenSteps = maxTargetDistance / Mathf.Max(3 * velocity.magnitude, Mathf.Abs(rSpeed * Mathf.Deg2Rad * averageRotationRadius));
        }
        if (Time.time > lastStep + (timeBetweenSteps / legs.Length) && legs != null)
        {
            if (legs[index] == null) return;

            Vector3 legPoint = (legs[index].restingPosition + velocity);
            Vector3 legDirection = legPoint - transform.position;
            Vector3 rotationalPoint = ((Quaternion.Euler(0, rSpeed / 2f, 0) * legDirection) + transform.position) - legPoint;
            Debug.DrawRay(legPoint, rotationalPoint, Color.black, 1f);
            Vector3 rVelocity = rotationalPoint + velocity;

            legs[index].stepDuration = Mathf.Min(0.5f, timeBetweenSteps / 2f);
            legs[index].worldVelocity = rVelocity;
            legs[index].Step();
            lastStep = Time.time;
            index = (index + 1) % legs.Length;
        }
    }
    private void ctrl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Vector3 targetDirection = new Vector3(h, 0, v);
            velocity = targetDirection;
            float y = Camera.main.transform.rotation.eulerAngles.y;
            targetDirection = Quaternion.Euler(0, y, 0) * targetDirection;
            transform.Translate(targetDirection * Time.deltaTime * 3f, Space.World);
            Quaternion newQuaternion = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newQuaternion, Time.deltaTime * 10f);

        }
    }
}
