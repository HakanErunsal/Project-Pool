using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecordShot
{
    public float TimeMark = 0.0f;
    public Vector3 PositionMark;
    public Quaternion RotationMark;
    public bool IsFinal;
}
