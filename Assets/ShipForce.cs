using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipForce : MonoBehaviour
{
    public GameObject[] vertex;
    private Vector3[] debugPos;
    private Rigidbody rg;
    public float force = 10;
    // Start is called before the first frame update
    void Start()
    {
        debugPos = new Vector3[vertex.Length];
        rg = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < vertex.Length; i++)
        {
            RaycastHit hit;
            Ray ray = new Ray(vertex[i].transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Plane")
                {
                    debugPos[i] = hit.point;

                    float distance = Vector3.Distance(hit.point, vertex[i].transform.position);
                    distance = Mathf.Clamp(1-distance, 0, 1);
                    rg.AddForceAtPosition(Vector3.up * Mathf.Lerp(0, force, distance), vertex[i].transform.position);
                }
            }
        }
        ctrl();
    }
    private void ctrl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Vector3 targetDirection = new Vector3(h, 0, v);
            float y = Camera.main.transform.rotation.eulerAngles.y;
            targetDirection = Quaternion.Euler(0, y, 0) * targetDirection;
            transform.Translate(targetDirection*Time.deltaTime*3f , Space.World);
            Quaternion newQuaternion = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newQuaternion, Time.deltaTime * 10f);

        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (debugPos.Length == 0||vertex.Length==0) return;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(vertex[i].transform.position, debugPos[i]);
            Gizmos.DrawSphere(debugPos[i], .1f);
        }
    }
}
