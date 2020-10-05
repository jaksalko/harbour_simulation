using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Vector3 targetPosition;//container destination position
    public Vector2 targetIndex;
    public Vector2 startIndex;
    public TransferCrane Tc;

    
    [Header("uselses container info")]
    public string ISO = "42G1"; //40fit ~ 12m General Purpose Container
    public string BIC = "AAAU1000001";
    
}
