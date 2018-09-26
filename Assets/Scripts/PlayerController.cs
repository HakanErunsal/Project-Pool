using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class PlayerController : MonoBehaviour
{

    private CameraController CameraCtrl;

    //Buttons
    public GameObject ReplayButton;
    public GameObject RestartButton;

    public Camera HitCamera;
    public GameObject[] Balls;

    public Text CurrentTimeText;
    public Text ScoreText;

    public GameObject Line;
    private LineRenderer LineRnd;
    public LayerMask PoolMask;
    private Ray Ray_;

    private bool CanPlay;
    private bool IsWatching;
    private bool ReplayAvailable;

    private float MaxHitSpeed;
    private float HitSpeed;

    //Replay saves
    private float LastHitSpeed;
    private Vector3 LastHitDirection;
    private List<Vector3> LastBallPositions;
    private List<Vector3> CurrentBallPositions;
    private bool Replaying;

    //Current player data
    private int Score;
    private int HitCount;
    private int CurrentTime;
    private int WinningScore;
    private bool Ball1Hit, Ball2Hit;
    private bool ScoreLock;
    private bool TimeHandle;
    private bool CanHit;

    void Start()
    {
        CanPlay = true;
        MaxHitSpeed = 50f;
        CurrentTime = 0;
        TimeHandle = true;

        CameraCtrl = transform.GetComponent<CameraController>();
        LineRnd = Line.GetComponent<LineRenderer>();
        LastBallPositions = new List<Vector3>();
        CurrentBallPositions = new List<Vector3>();

        if (GameManager.GameInstance)
        {
            WinningScore = GameManager.GameInstance.WinningScore;
        }

        StartCoroutine(Timer());
    }

    //Input always in Update to prevent input loss
    void Update()
    {
        if (CameraCtrl)
        {
            Inputs();
        }
    }

    //Fixed update because physics needs to be deterministic for replay and multiplayer
    void FixedUpdate()
    {
        if (IsWatching)
        {
            HitEndControl();
        }

        if (CanHit)
        {
            HitBall(Ray_.direction, HitSpeed);

            Line.GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
            Line.GetComponent<LineRenderer>().SetPosition(2, Vector3.zero);
        }
    }

    void Inputs()
    {
        if (CanPlay)
        {
            //Ray setup to use for direction to hit
            
            Ray_ = new Ray(Balls[0].transform.position, new Vector3(HitCamera.transform.forward.x, 0f, HitCamera.transform.forward.z));

            if (Input.GetKey("space") && !Replaying)
            {
                //Hit speed lerp to maximum
                HitSpeed = Mathf.Lerp(HitSpeed, MaxHitSpeed, 0.01f);
                float RaySpeed = HitSpeed / 10f;

                //Line trace for aim assist
                RaycastHit Hit;

                if (Physics.Raycast(Ray_, out Hit, RaySpeed, PoolMask))
                {
                    LineRnd.SetPosition(1, Ray_.direction * Hit.distance);
                    Vector3 BounceDirection = Vector3.Reflect(Ray_.direction, Hit.normal);
                    Vector3 BouncedPosition = Hit.point + (BounceDirection * Mathf.Clamp((RaySpeed - (Hit.point - Balls[0].transform.position).magnitude), 0f, 0.5f));
                    LineRnd.SetPosition(2, BouncedPosition - Balls[0].transform.position);
                }
                else
                {
                    LineRnd.SetPosition(1, Ray_.direction * RaySpeed);
                    LineRnd.SetPosition(2, LineRnd.GetPosition(1));
                }
            }
            if (CanPlay && Input.GetKeyUp("space"))
            {
                Debug.Log("Hit Call");

                CanHit = true;
            }

            if (CanPlay && HitCamera.gameObject.activeSelf)
            {
                HitCamera.transform.RotateAround(Balls[0].transform.position, Vector3.up, Input.GetAxis("Horizontal"));
            }

        }
    }

    void HitBall(Vector3 Direction, float Speed)
    {
        if (!Replaying)
        {
            //Save replay information
            LastHitSpeed = Speed;
            LastHitDirection = Direction;
            HitCount++;
        }

        //Save positions
        LastBallPositions.Clear();
        foreach (GameObject Ball in Balls)
        {
            LastBallPositions.Add(Ball.transform.position);
        }

        //Hit to rigidbody
        Balls[0].GetComponent<Rigidbody>().AddForce(Direction * (Speed * 5f));
        HitSpeed = 0f;

        //Switch to watch mode
        CanPlay = CanHit = false;
        IsWatching = true;
        CameraCtrl.ActivateCamera(true, Replaying);

        ReplayButton.SetActive(false);
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

        if (Replaying)
        {
            for (int i = 0; i < Balls.Length; i++)
            {
                Balls[i].transform.position = CurrentBallPositions[i];
            }
        }

        ReplayAvailable = true;

        Line.transform.position = Balls[0].transform.position;
        CanPlay = true;
        IsWatching = false;
        CameraCtrl.ActivateCamera(false, Replaying);
        Replaying = false;
        Ball1Hit = Ball2Hit = ScoreLock = false;

        ReplayButton.SetActive(true);
    }

    void ReplayCall()
    {
        if (!Replaying && ReplayAvailable)
        {
            Replaying = true;
            CurrentBallPositions.Clear();
            for (int i = 0; i < Balls.Length; i++)
            {
                CurrentBallPositions.Add(Balls[i].transform.position);
                Balls[i].transform.position = LastBallPositions[i];
            }
            HitBall(LastHitDirection, LastHitSpeed);
        }
    }

    void EndGame(bool Success)
    {
        CanPlay = false;
        TimeHandle = false;

        PlayerScore PlayerScore_ = new PlayerScore
        {
            Score_ = Score,
            Time_ = CurrentTime,
            HitCount_ = HitCount
        };

        if (GameManager.GameInstance)
        {
            GameManager.GameInstance.SaveGame(PlayerScore_);
        }

        RestartButton.SetActive(true);
    }

    public void CollisionHandle(bool BallType)
    {
        if (!Replaying)
        {
            if (BallType)
            {
                Ball1Hit = true;
            }
            else
            {
                Ball2Hit = true;
            }

            if (!ScoreLock && (Ball1Hit && Ball2Hit))
            {
                Score++;
                ScoreLock = true;

                if (ScoreText)
                {
                    ScoreText.text = "Score: " + Score.ToString();
                }

                if (Score >= WinningScore)
                {
                    EndGame(true);
                }
            }
        }
    }

    public void GoToMainMenu()
    {
        if (GameManager.GameInstance)
        {
            GameManager.GameInstance.LoadScene();
        }
    }

    IEnumerator Timer()
    {
        if (TimeHandle)
        {
            CurrentTime++;
            yield return new WaitForSeconds(1f);
            StartCoroutine(Timer());

            string Minutes = Mathf.Floor(CurrentTime / 60).ToString("00");
            string Seconds = Mathf.RoundToInt(CurrentTime % 60).ToString("00");

            if (CurrentTimeText)
            {
                CurrentTimeText.text = "Time: " + Minutes + "." + Seconds;
            }
        }
        yield return null;
    }
}
