using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoard : MonoBehaviour
{
    protected Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        r.AddRelativeTorque(new Vector3(0f, 2f *Input.GetAxis("Horizontal"), 0f));
        r.AddRelativeForce(new Vector3(2f * Input.GetAxis("Vertical"), 0f, 0f));

        var locVel = transform.InverseTransformDirection(r.velocity);
        locVel.z = 0;
        r.velocity = transform.TransformDirection(locVel);

    }
}