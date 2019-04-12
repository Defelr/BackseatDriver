using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using QPathFinder;

public class GameManager : MonoBehaviour {

    public CarController car;
    public Destination target;

    // For UI
    public GameObject UIDialoguePanel;
    public GameObject FareMeterePanel;
    public GameObject HintPanel;
    public Image potraitPhoto;
    public bool isDialgueShowing;
    public bool isHintShowing;
    public float dialogueHidingOff;
    public float hintShowOff;
    public float dialogueMovingSpeed;
    protected Vector3 showingPos;
    protected Vector3 hidingPos;
    protected Vector3 hintPos;
    protected Vector3 targetPos;
    protected GameObject acceptButton;
    protected GameObject cancelButton;
    protected Text dialogueText;
    protected Text fareMeterText;
    protected Text hintText;
    protected Slider contentSlider;

    // For gameplay
    protected Passenger waitingPassenger;
    protected IEnumerator coroutineForDialogueChecking;
    protected Vector3 carLastPos;

    protected Rigidbody carRigid;

    protected static float SQRT2OVER2 = Mathf.Sqrt(2) / 2f;

    private void Start()
    {
        carRigid = car.GetComponent<Rigidbody>();

        showingPos = UIDialoguePanel.transform.position;
        hidingPos = showingPos;
        hidingPos.y -= dialogueHidingOff;

        // For UI
        acceptButton = UIDialoguePanel.transform.Find("AcceptButton").gameObject;
        cancelButton = UIDialoguePanel.transform.Find("CancelButton").gameObject;
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
        dialogueText = UIDialoguePanel.transform.Find("Dialogue").GetComponent<Text>();
        fareMeterText = FareMeterePanel.transform.Find("Text").GetComponent<Text>();
        FareMeterePanel.SetActive(false);
        hintText = HintPanel.transform.Find("Dialogue").GetComponent<Text>();
        contentSlider = UIDialoguePanel.transform.Find("Slider").GetComponent<Slider>();
    }

    private void Update()
    {
        // Update hidingPos for testing
        hidingPos = showingPos;
        hidingPos.y -= dialogueHidingOff;

        hintPos = showingPos;
        hintPos.y -= hintShowOff;

        if (isDialgueShowing)
        {
            targetPos = showingPos;
        }
        else
        {
            if (isHintShowing)
            {
                targetPos = hintPos;
            }
            else
            {
                targetPos = hidingPos;
            }
        }

        UIDialoguePanel.transform.position = Vector3.Lerp(UIDialoguePanel.transform.position, targetPos, dialogueMovingSpeed * Time.deltaTime);

        if (target)
        {
            PathFinder.instance.FindShortestPathOfPoints(car.transform.position, target.transform.position, PathFinder.instance.graphData.lineType,
                Execution.Asynchronously,
                SearchMode.Complex,
                delegate (List<Vector3> points)
                {
                /*
                PathFollowerUtility.StopFollowing(playerObj.transform);
                if (useGroundSnap)
                {
                    FollowThePathWithGroundSnap(points);
                }
                else
                    FollowThePathNormally(points);

                */

                    if (carRigid.velocity.magnitude >= 0.1f)
                    {
                        if (points.Count > 3)
                        {
                            Vector3 nextDir = points[3] - car.transform.position;

                            float dot = Vector3.Dot(carRigid.transform.forward.normalized, nextDir.normalized);
                            float cross = Vector3.Cross(carRigid.transform.forward, nextDir).y;

                            string directionHint = "";
                            string hint = "";

                            if (dot > 0)
                            {
                                directionHint += "front ";
                                hint = "Move Forward";
                            }

                            if (dot < SQRT2OVER2)
                            {
                                if (cross > 0)
                                {
                                    directionHint += "right ";
                                    hint = "Turn Right On Next Crossroad";
                                }
                                if (cross < 0)
                                {
                                    directionHint += "left ";
                                    hint = "Turn Left On Next Crossroad";
                                }
                            }


                            if (dot < 0)
                            {
                                directionHint += "back ";
                                hint = "Turn Back";
                            }

                            //print(directionHint);
                            hintText.text = hint;
                        }
                    }

                    Debug.DrawLine(points[0], points[2], Color.blue);
                    for (int i = 2; i < points.Count - 1; i++)
                    {
                        Debug.DrawLine(points[i], points[i + 1], Color.blue);
                    }
                }
                );
        }

        if (car && car.passengerInCar)
        {
            potraitPhoto.sprite = car.passengerInCar.GetPotrait();
            car.passengerInCar.UpdateContentLevel(30f * (Vector3.Distance(carLastPos, target.transform.position)-Vector3.Distance(car.transform.position, target.transform.position)) * Time.deltaTime);
            carLastPos = car.transform.position;
            contentSlider.value = car.passengerInCar.GetContentLevel();
        }
    }

    public void OfferToCar(Passenger passenger)
    {
        if (coroutineForDialogueChecking != null)
        {
            StopCoroutine(coroutineForDialogueChecking);
            coroutineForDialogueChecking = null;
        }
        print("Destination is " + passenger.task.destination);
        print("You will gain " + passenger.task.fee);

        acceptButton.SetActive(true);
        cancelButton.SetActive(true);
        isDialgueShowing = true;
        dialogueText.text = "Passenger " + passenger.name + " needs you to take a ride to " + passenger.task.destination.name + ", and you will get "
            + passenger.task.fee + " dollar. Would you like to accecpt it?";
        waitingPassenger = passenger;
        car.StopMoving();
    }

    public void CheckForDestination(Destination destination)
    {
        if (destination == car.passengerInCar.task.destination)
        {
            car.passengerInCar.isCompleted = true;
            contentSlider.value = contentSlider.minValue;
            isDialgueShowing = true;
            // Ready to end the task and get paid
            if (car.passengerInCar.contentLevel > 50f)
            {
                car.earn += car.passengerInCar.task.fee;
                dialogueText.text = "Thank you for taking me to " + car.passengerInCar.task.destination.name + ". This is " + car.passengerInCar.task.fee + " dollar for you.";
            }
            else
            {
                dialogueText.text = "Thank you for taking me to " + car.passengerInCar.task.destination.name;
            }

            Invoke("HideDialogue", 2f);
            car.DropPassenger();
            car.passengerInCar = null;
        }
    }

    public void HideDialogue()
    {
        if (waitingPassenger == null)
        {
            isDialgueShowing = false;
            isHintShowing = false;
        }
    }

    public void AcceptTask()
    {
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
        //isDialgueShowing = false;
        isHintShowing = true;
        car.AcceptPassenger(waitingPassenger);
        target = waitingPassenger.task.destination;
        waitingPassenger = null;
        FareMeterePanel.SetActive(true);
        fareMeterText.text = car.passengerInCar.task.fee.ToString();
        car.StartMoving();
        carLastPos = car.transform.position;
    }

    public void CancelTask()
    {
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
        //isDialgueShowing = false;
        isHintShowing = false;
        waitingPassenger = null;
        FareMeterePanel.SetActive(false);
        if (coroutineForDialogueChecking != null)
        {
            StopCoroutine(coroutineForDialogueChecking);
            coroutineForDialogueChecking = null;
        }
        car.StartMoving();
    }

    public void StartCheckingDialogueValidility()
    {
        coroutineForDialogueChecking = CheckForDialogueValidality();
        StartCoroutine(coroutineForDialogueChecking);
    }

    public IEnumerator CheckForDialogueValidality()
    {
        float startTime = Time.time;
        if (Time.time - startTime < 2f) 
        {
            yield return null;
        }

        CancelTask();
    }
}