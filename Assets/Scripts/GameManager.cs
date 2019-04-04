using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using QPathFinder;

public class GameManager : MonoBehaviour {

    public CarController car;
    public GameObject target;
    public GameObject UIDialoguePanel;

    public bool isDialgueShowing;
    public float dialogueHidingOff;
    public float dialogueMovingSpeed;
    protected Vector3 showingPos;
    protected Vector3 hidingPos;
    protected Vector3 targetPos;

    protected Rigidbody carRigid;

    protected static float SQRT2OVER2 = Mathf.Sqrt(2) / 2f;

    private void Start()
    {
        carRigid = car.GetComponent<Rigidbody>();
        //UIDialoguePanel.SetActive(false);

        showingPos = UIDialoguePanel.transform.position;
        hidingPos = showingPos;
        hidingPos.y -= dialogueHidingOff;
    }

    private void Update()
    {
        // Update hidingPos for testing
        hidingPos = showingPos;
        hidingPos.y -= dialogueHidingOff;

        if (isDialgueShowing)
        {
            targetPos = showingPos;
        }
        else
        {
            targetPos = hidingPos;
        }

        UIDialoguePanel.transform.position = Vector3.Lerp(UIDialoguePanel.transform.position, targetPos, dialogueMovingSpeed * Time.deltaTime);

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

                        print(directionHint);
                        print(hint);
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
}