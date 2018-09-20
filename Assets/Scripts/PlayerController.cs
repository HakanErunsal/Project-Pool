using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private CameraController CameraCtrl;

    public Camera HitCamera;
    public GameObject[] Balls;

    public GameObject Line;
    private LineRenderer LineRnd;
    private Transform GoTransform;
    public LayerMask PoolMask;

    private bool CanPlay;
    private bool IsWatching;
    private bool ReplayValid;

    private float MaxHitSpeed;
    private float HitSpeed;

    private float LastHitSpeed;
    private Vector3 LastCameraForward;
    private List<Vector3> LastBallPositions;

    void Start()
    {
        CanPlay = true;
        MaxHitSpeed = 500f;

        CameraCtrl = transform.GetComponent<CameraController>();
        LineRnd = Line.GetComponent<LineRenderer>();
        LastBallPositions = new List<Vector3>();

        GoTransform = Balls[0].transform;
    }

    void Update()
    {
        if (CameraCtrl)
        {
            Inputs();
        }

        if (IsWatching)
        {
            HitEndControl();
        }
    }

    void Inputs()
    {
        if (CanPlay)
        {
            //Ray setup to use for direction to hit
            Ray Ray_;
            Ray_ = new Ray(Balls[0].transform.position, new Vector3(HitCamera.transform.forward.x, 0f, HitCamera.transform.forward.z));

            if (Input.GetKey("space"))
            {
                //Hit speed lerp to maximum
                HitSpeed = Mathf.Lerp(HitSpeed, MaxHitSpeed, 0.1f);

                //Line trace for aim assist
                RaycastHit Hit;

                if (Physics.Raycast(Ray_, out Hit, Mathf.Infinity, PoolMask))
                {

                    LineRnd.SetPosition(1, Ray_.direction * Hit.distance);
                    Vector3 BounceDirection = Vector3.Reflect(Ray_.direction, Hit.normal);
                    Debug.DrawLine(Hit.point, Hit.point + (BounceDirection * 5f));
                    LineRnd.SetPosition(2, Hit.point + (BounceDirection.normalized * 5f));
                }
                else
                {
                    LineRnd.SetPosition(1, Vector3.zero);
                    LineRnd.SetPosition(2, LineRnd.GetPosition(1));
                }
            }
            if (Input.GetKeyUp("space"))
            {
                HitBall(Ray_.direction);

                Line.GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
                Line.GetComponent<LineRenderer>().SetPosition(2, Vector3.zero);
            }

            if (CanPlay && HitCamera.gameObject.activeSelf)
            {
                HitCamera.transform.RotateAround(Balls[0].transform.position, Vector3.up, Input.GetAxis("Horizontal"));
            }

        }
    }

    void HitBall(Vector3 Direction)
    {
        //Save replay information
        LastHitSpeed = HitSpeed;
        LastCameraForward = Direction;

        LastBallPositions.Clear();
        foreach (GameObject Ball in Balls)
        {
            LastBallPositions.Add(Ball.transform.position);
        }
        ReplayValid = true;

        //Hit to rigidbody
        Balls[0].GetComponent<Rigidbody>().AddForce(Direction * HitSpeed);
        HitSpeed = 0f;

        //Switch to watch mode
        CanPlay = false;
        IsWatching = true;
        CameraCtrl.ActivateCamera(true);
    }

    void HitEndControl()
    {
        //Check if all rigidbodies are sleeping
        bool AllStopped = false;

        while (!AllStopped)
        {
            AllStopped = true;

            foreach (GameObject Ball in Balls)
            {
                if (!Ball.GetComponent<Rigidbody>().IsSleeping())
                {
                    AllStopped = false;
                    return;
                }
            }
        }

        //Call hit end if all sleeping
        if (AllStopped)
        {
            HitEnd();
        }
    }

    void HitEnd()
    {
        //Switch to play mode
        Line.transform.position = Balls[0].transform.position;
        CanPlay = true;
        IsWatching = false;
        CameraCtrl.ActivateCamera(false);
    }
}
