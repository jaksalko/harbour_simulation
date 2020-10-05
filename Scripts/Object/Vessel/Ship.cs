using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;

//Vessel script
public class Ship : MonoBehaviour
{
    public static event Action<Job> AddPool; //Add Pool Event for Tree View
   
    public Ship newVessel; // Clone this vessel

    public ControlTower ct; // control tower
    
    public Block[] blocks; // Block list in Harbour 

    public List<GantryCrane> gcs; // gc list assigned by control tower
    public List<Job> jobs; // job list for vessel
    public List<Sector> sectors; // sector list in Vessel
 
    
    public float arrivePosition; // Vessel Arrive Position 


    public Slider percentageSlider;
    int totalJobCount; 
    void Start()
    {
       

        ct.ships.Add(this);
        transform.SetParent(ct.objectFilter[6].transform);
        jobs = new List<Job>();

        newVessel = Instantiate(this, transform.position, Quaternion.identity);
        ct.waitship.Add(newVessel);
        newVessel.gameObject.SetActive(false);



        StartCoroutine(ArriveHarbour());
      
    }


   
  
    IEnumerator ArriveHarbour()
    {
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, arrivePosition)));
      
        GantryCrane selectedGC = gcs[0];

        for(int i = 0; i < gcs.Count; i++)
        {
            gcs[i].start_sector = (sectors.Count / gcs.Count) * i;
        }


        for (int s = 0; s < sectors.Count; s++)
        {
            Sector sector = sectors[s];
            for(int g = gcs.Count-1; g >= 0; g--)
            {                
                if(gcs[g].start_sector <= s)
                {
                    selectedGC = gcs[g];
                    break;
                }
            }

            int search = 0;

            while (search < 1)
            {
                search++;
                int ranBlock = UnityEngine.Random.Range(0, blocks.Length);
                int ranBay = UnityEngine.Random.Range(0, blocks[ranBlock].bays.Count);
                Block block = blocks[ranBlock];
                Bay bay = block.bays[ranBay];


                if (sector.fullField.Count <= bay.emptyField.Count && sector.fullField.Count > 0 && !sector.onjob && !bay.onjob)//discharge
                {
                    //sector to bay

                    List<Container> stob = sector.DischargeContainers();
                    for (int i = 0; i < sector.fullField.Count; i++)
                    {
                        stob[i].Tc = block.myTc;
                        stob[i].targetPosition = bay.GetContainerPostion(bay.emptyField[i]);
                        stob[i].startIndex = sector.fullField[i];
                        stob[i].targetIndex = bay.emptyField[i];
                    }
                    sector.onjob = true;
                    bay.onjob = true;


                    Job job = new Job(0,Job.JobType.DS, stob, block.myTc,selectedGC, bay, sector, this);

                    jobs.Add(job);
                    selectedGC.jobs.Add(job);
                    AddPool?.Invoke(job);
                    break;
                }
                else if (bay.fullField.Count <= sector.emptyField.Count && bay.fullField.Count > 0 && !sector.onjob && !bay.onjob)//load
                {
                    //bay to sector
                    List<Container> btos = bay.DischargeContainers();
                    for (int i = 0; i < bay.fullField.Count; i++)
                    {
                        btos[i].Tc = block.myTc;
                        btos[i].targetPosition = sector.GetContainerPostion(sector.emptyField[i]);
                        btos[i].startIndex = bay.fullField[i];
                        btos[i].targetIndex = sector.emptyField[i];
                    }
                    sector.onjob = true;
                    bay.onjob = true;
                    Job job = new Job(0,Job.JobType.LD, btos, block.myTc, selectedGC, bay, sector,this);
                    jobs.Add(job);
                    selectedGC.jobs.Add(job);
                    AddPool?.Invoke(job);
                    break;

                }

                



            
            }
        }
        totalJobCount = jobs.Count;
        percentageSlider.gameObject.SetActive(true);

        Camera.main.transform.ObserveEveryValueChanged(_ => _.position.y)
       .Where(_ => _ < 200)
       .Subscribe(_ => percentageSlider.gameObject.SetActive(false));
        Camera.main.transform.ObserveEveryValueChanged(_ => _.position.y)
            .Where(_ => _ >= 200)
            .Subscribe(_ => percentageSlider.gameObject.SetActive(true));



        jobs.ObserveEveryValueChanged(_ => _.Count)
          .Subscribe(_ => percentageSlider.value = (1 - (float)jobs.Count / totalJobCount));
        foreach (var gc in gcs)
        {
            if (gc.status == Crane.Status.InActive)
            {
                gc.status = Crane.Status.Active;
            }

        }

        yield break;
    }
  
    public IEnumerator LeaveHarbour()
    {
        percentageSlider.gameObject.SetActive(false);
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 1000f)));
        
        //newVessel.gameObject.SetActive(true);
        Destroy(gameObject, 0.3f);
        yield break;
    }
    IEnumerator MoveStraight(Vector3 position)
    {
        float speed = 100;
        Quaternion lookOnLook = Quaternion.LookRotation(position - transform.position);
        var dis = Vector3.Distance(position, transform.position);

        while (transform.rotation != lookOnLook)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookOnLook, speed * Time.deltaTime);
            yield return null;
        }

        while (transform.position != position)
        {

            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            yield return null;
        }

        yield break;
    }
}
