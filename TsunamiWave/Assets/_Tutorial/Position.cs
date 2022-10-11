using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Position : MonoBehaviour
{

    public float range = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionDown = Vector3.forward;
        Ray theRayDown = new Ray(transform.position, transform.TransformDirection(directionDown * range));
        // Debug.DrawRay(transform.position, transform.TransformDirection(directionDown * range));

        Vector3 directionUp = Vector3.back;
        Ray theRayUp = new Ray(transform.position, transform.TransformDirection(directionUp * range));
        // Debug.DrawRay(transform.position, transform.TransformDirection(directionUp * range));


        if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out RaycastHit hitDown))
        {
            if (hitDown.collider.CompareTag("Wave"))
            {
            //    Debug.Log("The wave is below by " + hitDown.distance);
                transform.position = new Vector3(transform.position.x, transform.position.y - hitDown.distance + 0.4f, transform.position.z);
            }
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out RaycastHit hitUp))
        {
            if (hitUp.collider.CompareTag("Wave"))
            {
            //    Debug.Log("The wave is above by " + hitUp.distance);
                transform.position = new Vector3(transform.position.x, transform.position.y + hitUp.distance + 0.4f, transform.position.z);
            }
        }

    }
}
