using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

public class GantryCrane : Crane
{
    public int start_sector;

   
    public static event Action<Job> RemovePool;
   
    [Header("GC Attribute")]

    [SerializeField] float gcSpeed = default;

    [SerializeField] float cabinSpeed = default;

    [Header("GC Components")]
    public Transform gc;
    public Transform cabin;
  

    [Header("Linked Objects")]
    public Ship ship;
    public Transform container;
    public ITV itv; // change to ITV
    [SerializeField] private List<ITV> ITVs = default;
    [SerializeField] private List<Container> containers = default;



    public float cabin_ready_position;//-59
    public float spreader_ready_position;//23 높여도됨

  

    int loadContainerCount;


    public List<Job> jobs;

    private void Start()
    {
        this.ObserveEveryValueChanged(x => status)
            .Where(x => x == Status.Active)
            .Subscribe(x => NextJob());
    }
    void NextJob()
    {
        
        if(jobs.Count !=0)
        {
            job = jobs[0];
            containers = job.containers;

            switch (job.jobType)
            {
                case Job.JobType.DS:
                    StartCoroutine(Discharging());//discharging loop while containers count != 0
                    break;
                case Job.JobType.LD:
                    loadContainerCount = containers.Count;
                    StartCoroutine(Loading());//loading find itv while containers count != 0
                    break;
                default:
                    print("JobType Error");
                    break;
            }
            jobs.Remove(job);

        }
        else        
            status = Status.InActive;

    }


    void FindITV()
    {
        for (int i = 0; i < ITVs.Count; i++)
        {
            if (ITVs[i].state == ITV.State.Break)
            {

                switch (job.jobType)
                {
                    case Job.JobType.DS:
                        itv = ITVs[i];

                        itv.LDStart(job);
                        break;
                    case Job.JobType.LD:
                        itv = ITVs[i];
                        ITVs[i].DSStart(job);
                        ITVs[i].GetContainerInfo(containers[0]);//dscontainer[0] 위치를 앍고 itv가 이동해야하기때문에                       
                        containers.RemoveAt(0);//줬으면 제거
                        break;
                }

                break;
            }

        }
    }


    IEnumerator Loading()
    {

        var wait = new WaitForSeconds(1f);
        status = Status.Processing;

        while (itv == null)
        {
            FindITV();//쉬는애있으면 컨테이너 0번 주고 정보넘기고 리스트에서 제거
            yield return wait;
        }

        //????????
        while(itv != null)
        {
            yield return wait;
        }

        if (containers.Count != 0)
        {
            StartCoroutine(Loading());
            yield break;
        }
        yield break;
    }

    IEnumerator Discharging()
    {
        job.bay.LD(containers[0].targetIndex, containers[0]);
        container = containers[0].transform;
        containers.RemoveAt(0);
        status = Status.Processing;
        itvTargetPosition = new Vector3(cabin_ready_position , 0, container.position.z);


        FindITV();
        yield return StartCoroutine(CatchContainer());//배에 컨테이너를 잡고
        IsSpreaderHasCon = true;
        yield return StartCoroutine(ReadyDS());//itv 대기위치로 이동한 후
        yield return StartCoroutine(ReadyITV());//itv가 오기를 기다린 후 도착하면  + FindITV()
        ItvArrive = true;
        IsITVHasCon = false;
        yield return StartCoroutine(DS());//하역을 한다.

        status = Status.Completed;
        itv.LeaveITVToTC();
        itv = null;

        ItvArrive = false;
        IsSpreaderHasCon = false;

        if (containers.Count != 0)
        {
            StartCoroutine(Discharging());
            yield break;
        }

      
        RemovePool?.Invoke(job);
        status = Status.Active;
        yield break;
    }
    IEnumerator ReadyITV()
    {
        var wait = new WaitForSeconds(1f);

        while(itv == null)
        {
            FindITV();

            yield return wait;
        }

        while (itv.state != ITV.State.WaitLD)
        {
            yield return wait;
        }

        yield break;
    }

    IEnumerator CatchContainer()//for discharging
    {
        

        yield return StartCoroutine(CraneMove(container.position.z));
        yield return StartCoroutine(CabinMove(container.position.x));        
        yield return StartCoroutine(SpreadMove(container.position.y + 2.5f));//catch container..
        yield return StartCoroutine(VesselToCrane());

      
        
        yield break;
    }
    
    IEnumerator ReadyDS()
    {
        yield return StartCoroutine(SpreadMove(spreader_ready_position));
        yield return StartCoroutine(CabinMove(cabin_ready_position));//move container..

       
        yield break;
    }
    IEnumerator DS()
    {
        
        yield return StartCoroutine(SpreadMove(3f));//container height + itv height
        yield return StartCoroutine(CraneToITV());
        yield return StartCoroutine(SpreadMove(spreader_ready_position));//move and discharge container...

       
        yield break;
    }


  

 
    public void GetWorkFromITV(ITV itv)//gc container 포지션으로 gc이동
    {
        this.itv = itv;
        container = itv.container;

        //cranemove - container
        //cabinmove - ready

        StartCoroutine(ReadyLD());


    }
    IEnumerator ReadyLD()
    {
        StartCoroutine(CraneMove(container.GetComponent<Container>().targetPosition.z));//container targetz
        yield return StartCoroutine(CabinMove(cabin_ready_position));//-59

        itv.state = ITV.State.WaitDS;
        yield break;
    }
    public void LoadingCallByITV()
    {
        StartCoroutine(LoadingAction());
        ItvArrive = true;
        IsITVHasCon = true;
    }
    IEnumerator LoadingAction()
    {
        yield return StartCoroutine(SpreadMove(container.position.y + 2.5f));
        yield return StartCoroutine(ITVtoGC());
        yield return StartCoroutine(SpreadMove(spreader_ready_position));
        itv.LeaveByGC();
        itv = null;
        ItvArrive = false;
        IsSpreaderHasCon = true;
        yield return StartCoroutine(CabinMove(container.GetComponent<Container>().targetPosition.x));//
        yield return StartCoroutine(SpreadMove(container.GetComponent<Container>().targetPosition.y + 2.5f));
        yield return StartCoroutine(GCtoVessel());
        IsSpreaderHasCon = false;
        yield return StartCoroutine(SpreadMove(spreader_ready_position));

        if (waitnumber.Count != 0)
            waitnumber.RemoveAt(0);

       

        yield break;
    }
    IEnumerator CraneMove(float targetZ)//crane local position.z = container.z
    {
        float t = 0;
        gcSpeed = gcSpeed / Math.Abs(transform.position.z - targetZ);
        while (t <= 1)
        {
            t += gcSpeed * Time.deltaTime;
            float gcZ = Mathf.Lerp(gc.position.z, targetZ, t);

            gc.position = new Vector3(gc.position.x, gc.position.y, gcZ);

            yield return null;
        }
        
        yield break;
    }

    IEnumerator CabinMove(float targetX)//cabin local position.z = crane.x
    {
        float t = 0;
        while (t <= 1)
        {
            t += cabinSpeed * Time.deltaTime;
            float cabinX = Mathf.Lerp(cabin.position.x, targetX, t);

            cabin.position = new Vector3(cabinX, cabin.position.y, cabin.position.z);
            yield return null;
        }
      
        yield break;
    }


    IEnumerator ITVtoGC()
    {
        container.SetParent(spreader);
        yield break;
    }
    IEnumerator GCtoVessel()
    {
       
        container.SetParent(job.sector.transform);
        job.sector.LD(container.GetComponent<Container>().targetIndex, container.GetComponent<Container>());
        loadContainerCount--;
        if (loadContainerCount == 0)
        {
            job.bay.onjob = false;
            RemovePool?.Invoke(job);
            
            status = Status.Active;          
        }
        yield break;
    }
    IEnumerator CraneToITV()//con
    {
        container.parent = null;
        itv.GetContainer(container.GetComponent<Container>());
        yield break;

    }
    IEnumerator VesselToCrane()
    {
       
        container.SetParent(spreader);
        job.sector.DS(container.GetComponent<Container>().startIndex);
        yield break;
    }



}
