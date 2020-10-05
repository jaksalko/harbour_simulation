using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class SummaryView : MonoBehaviour
{
    public ControlTower ct;
    public GameObject[] viewList;
    public Text nowTimeTxt;
    public Text alarmTxt;
    public Text jobTxt;
    public Text vesselTxt;
    public Text pointerTxt;
    public Text connectionTxt;

    int job_temp;
    int done=0;
    int total=0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NowTime());

        ct.ships.ObserveEveryValueChanged(_ => _.Count)
            .Subscribe(_ => vesselTxt.text = "Vessel : " + _ );

        Ship.AddPool += AddJob;
        GantryCrane.RemovePool += DoneJob;
    }
    void DoneJob(Job job)
    {      
        done++;
        jobTxt.text = "Job " + done + "/" + total;
    }
    void AddJob(Job job)
    {
       
        total++;
        jobTxt.text = "Job " + done + "/" + total;
    }
    IEnumerator NowTime()
    {
        var wait = new WaitForSeconds(1f);
        while(true)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            nowTimeTxt.text = now;

            pointerTxt.text = Input.mousePosition.ToString();//위도 경도 출력


            connectionTxt.text = "FPS : " + (1 / Time.deltaTime);
            yield return wait;    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
