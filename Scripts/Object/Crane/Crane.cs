using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Crane : MonoBehaviour
{
    public enum Status
    {
        InActive,
        Active,
        Processing,
        Completed,
    }
    public Status status;
    public Transform spreader;
    public float spreaderSpeed;

    public Vector3 itvTargetPosition;//itv wait position
    public List<ITV> waitnumber;
    public Job job;

    [Header("2d UI")]
    public bool ItvArrive = false;
    public bool IsITVHasCon = false;
    public bool IsSpreaderHasCon = false;


    public Transform focusView;
    public IEnumerator SpreadMove(float targetY)//spread position y = container.y +2.5
    {
        float t = 0;
        while (t <= 1)
        {
            t += spreaderSpeed * Time.deltaTime;

            float spreaderY = Mathf.Lerp(spreader.position.y, targetY, t);
            spreader.position = new Vector3(spreader.position.x, spreaderY, spreader.position.z);
            yield return null;
        }


        yield break;
    }

}
