using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject BallCamera, WatchCamera;
    public GameObject PlayerBall;

    private Vector3 Offset;

	void Start ()
    {
        //Save offset and activate the play camera
        Offset = BallCamera.transform.position - PlayerBall.transform.position;
        ActivateCamera(false);
    }

    void Update ()
    {

	}

    public void ActivateCamera(bool Watch)
    {
        //Switch camera type and audio listeners
        if (Watch)
        {
            Offset = BallCamera.transform.position - PlayerBall.transform.position;
        }
        else
        {
            BallCamera.transform.position = PlayerBall.transform.position + Offset;
        }

        WatchCamera.SetActive(Watch);
        WatchCamera.GetComponent<AudioListener>().enabled = Watch;
        BallCamera.SetActive(!Watch);
        WatchCamera.GetComponent<AudioListener>().enabled = !Watch;
    }
}
