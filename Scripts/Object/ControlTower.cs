using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ControlTower : MonoBehaviour
{
    [Header("Objects")]
    public List<Ship> ships; // Add when ship arrive at harbour...
    public List<Ship> waitship;

    public List<TransferCrane> transferCranes; 
    public List<GantryCrane> gantryCranes;
    public List<ITV> itvs;

    [Header("Job")]
    public List<Job> jobs;
    
   
    public GameObject[] objectFilter;// 0 yt 1 qc 2 tc 3 th 4 rs 5 otr

    [Serializable]
    public class Area
    {
        
        public Ship ship;
        public float shipArrivePosition;

        public List<GantryCrane> GC_InArea;

        public void ActivateArea(Ship newShip)
        {
            ship = newShip;
            ship.arrivePosition = shipArrivePosition;
            ship.gcs.AddRange(GC_InArea);
        }
        public void InActivateArea()
        {
            ship = null;
        }
    }
    public List<Area> areas;// area 1 2 3



    // Start is called before the first frame update
    void Start()
    {
        GantryCrane.RemovePool += EndJob_CallByGC;

        GameObject[] gcs = GameObject.FindGameObjectsWithTag("GCObject");
        for(int i = 0; i < gcs.Length; i++)
        {
            
            gcs[i].transform.SetParent(objectFilter[1].transform);
            gantryCranes.Add(gcs[i].GetComponent<GantryCrane>());
        }

        GameObject[] tcs = GameObject.FindGameObjectsWithTag("TCObject");
        for (int i = 0; i < tcs.Length; i++)
        {
            tcs[i].transform.SetParent(objectFilter[2].transform);
            transferCranes.Add(tcs[i].GetComponent<TransferCrane>());
        }

        GameObject[] itv = GameObject.FindGameObjectsWithTag("ITVObject");
        for (int i = 0; i < itv.Length; i++)
        {
            itv[i].transform.SetParent(objectFilter[0].transform);
            itvs.Add(itv[i].GetComponent<ITV>());
        }

        int waitshipCount = waitship.Count;
        for (int i = 0; i < waitshipCount ; i++)
        {
            Debug.Log("call" + waitship.Count);
            int area_index = CheckAreaNull(areas.Count - 1);
            if (area_index < areas.Count)
            {
                Debug.Log("Activate");
                areas[area_index].ActivateArea(waitship[0]);
                waitship[0].gameObject.SetActive(true);
                
                waitship.RemoveAt(0);
            }
            else
            {
                Debug.Log("break " + i);
                break;
            }
        }

        Ship.AddPool += Add_job;


    }
   
    void Add_job(Job job)
    {
        jobs.Add(job);
    }
    void EndJob_CallByGC(Job job)
    {
        jobs.Remove(job);
        Ship jobship = job.ship;
        jobship.jobs.Remove(job);

        if(jobship.jobs.Count == 0)
        {
            jobship.StartCoroutine(jobship.LeaveHarbour());

            for(int i=0; i < areas.Count; i++)
            {
                if (areas[i].ship == jobship)
                    areas[i].InActivateArea();
            }

            ships.Remove(jobship); //remove area ship also

            int waitshipCount = waitship.Count;
            for (int i = 0; i < waitshipCount; i++)
            {
                int area_index = CheckAreaNull(areas.Count - 1);
                Debug.Log("check area null : " + area_index);
                if (area_index < areas.Count)
                {                   
                    
                    areas[area_index].ActivateArea(waitship[0]);
                    waitship[0].gameObject.SetActive(true);
                    waitship.RemoveAt(0);
                }
                else
                {
                    Debug.Log("break " + i);
                    break;
                }
            }


            
        }

    }
    int CheckAreaNull(int index)
    {

        if (areas[index].ship == null)
        {
            if (index == 0)
                return 0;
            return CheckAreaNull(index - 1);
        }
        else
        {
            return index+1;
        }
        
    }
    
}
