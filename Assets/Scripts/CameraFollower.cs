using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    public GameObject car;
    public Dictionary<string, Vector3> direction;

    protected Vector3 targetDir;

    protected Vector3 diffPos;

	// Use this for initialization
	void Start () {
        direction = new Dictionary<string, Vector3>();
        direction["north"] = Vector3.forward;
        direction["south"] = Vector3.back;
        direction["west"] = Vector3.left;
        direction["east"] = Vector3.right;

        targetDir = GetClosestDirectionVector(car.transform.forward);

        diffPos = transform.position - car.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = car.transform.position + diffPos;
        //transform.rotation = Quaternion.LookRotation(targetDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir, Vector3.up), 180f * Time.deltaTime);

        targetDir = GetClosestDirectionVector(car.transform.forward);
    }

    Vector3 GetClosestDirectionVector(Vector3 input)
    {
        Vector3 inputNoY = input;
        inputNoY.y = 0f;

        foreach (string dirKey in direction.Keys)
        {
            float angle = Vector3.Angle(inputNoY, direction[dirKey]);
            if (angle < 45)
            {
                return direction[dirKey];
            }
        }

        return Vector3.zero;
    }
}
