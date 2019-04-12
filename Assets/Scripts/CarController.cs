using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CarController : MonoBehaviour {

    
	float throttle=0f;
	float steer=0f;
	public float turnSpeed = 30;
	public float speed = 160; 
	public float antiSlip = 100.0f;
    public bool isStopped;
	
	Transform destination;
    Rigidbody rigidbody;

    // For Gameplay
    public float earn;
    public Passenger passengerInCar;

    //Pathfinder pathfinder; 

    void Start () {
        //Debug.Log (transform.forward);

        //GameObject pathfinderGameobject = GameObject.Find ("PathfindingManager");
        //pathfinder = pathfinderGameobject.GetComponent<Pathfinder>();
        tag = "Player";
        rigidbody = GetComponent<Rigidbody>();
        earn = 0f;
        isStopped = false;
    }
	
	void Update () {
		GetInput ();

        /*
        for (int i=0; i < navAgent.path.corners.Length; i++)
        {
            if (i != navAgent.path.corners.Length - 1) {
                Debug.DrawLine(navAgent.path.corners[i], navAgent.path.corners[i + 1], Color.blue);
            }
        }
        */
    }

	// use this for rigidbody physics updates
	void FixedUpdate () {

        if (isStopped)
        {
            rigidbody.velocity = Vector3.zero;
        }
        else
        {
            ApplyFriction();
            ApplyThrottle();
            ApplySteering();
        }
	}

	void GetInput() {
		throttle = Input.GetAxis ("Vertical");
		steer = Input.GetAxis ("Horizontal");

	}

	// 
	void ApplyFriction ()
	{
		Vector3 relativeVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);

		// apply sideways friction to prevent slipping

		float sqrVel = relativeVelocity.x * relativeVelocity.x;


		Vector3 antiSlipVecLocal = sqrVel * Vector3.right * Mathf.Sign (relativeVelocity.x) * antiSlip * -1;

		Vector3 antiSlipVec = transform.TransformDirection (antiSlipVecLocal);

		GetComponent<Rigidbody>().AddForce (antiSlipVec  * Time.fixedDeltaTime);
	}

	void ApplyThrottle ()
	{
		if (Input.GetKey (KeyCode.W))
			GetComponent<Rigidbody>().AddForce (transform.forward * Time.fixedDeltaTime * throttle * speed);
		else if (Input.GetKey (KeyCode.S))
			GetComponent<Rigidbody>().AddForce (transform.forward * Time.fixedDeltaTime * throttle * speed * 1.5f);
		else {
			//if no acceleration keys are being held down, apply force in the opposite direction of the car's movement to slow it to a stop.
			if(GetComponent<Rigidbody>().velocity.magnitude > 0.05) {
				GetComponent<Rigidbody>().AddForce (GetComponent<Rigidbody>().velocity * Time.fixedDeltaTime * speed * -0.5f);
			}
		}
	}

	void ApplySteering ()
	{
        float turnS = Mathf.Min(0.5f * rigidbody.velocity.magnitude * rigidbody.velocity.magnitude * rigidbody.velocity.magnitude * turnSpeed, turnSpeed);
        float isForward = Mathf.Sign(Vector3.Dot(rigidbody.velocity, transform.forward));
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.05f)
			transform.RotateAround (transform.position, Vector3.up, steer * Time.fixedDeltaTime * turnS * isForward);
	}

    public void AcceptPassenger(Passenger passenger)
    {
        passengerInCar = passenger;
        PickPassenger(passenger);
    }

    public void PickPassenger(Passenger passenger)
    {
        passenger.transform.localScale = Vector3.zero;
    }

    public void DropPassenger()
    {
        passengerInCar.transform.localScale = Vector3.one;
        passengerInCar.transform.position = transform.position + transform.right * 1f;
    }

    public void StopMoving()
    {
        isStopped = true;
    }

    public void StartMoving()
    {
        isStopped = false;
    }
}