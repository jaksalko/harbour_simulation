using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferCrane : Crane
{

    [Header("TC Yard")]
    public Transform yard;

    [Header("TC Attribute")]
    [SerializeField] private float tcSpeed = default;
    [SerializeField] private float trolleySpeed = default;

    [SerializeField] private float ready_spreader = default;
    float default_trolley;//crane.z+5
    float default_crane;//vector.x = 0

    [Header("TC Components")]
    public Transform tc;
    public Transform trolley;
    public int TCnum;



    [Header("Linked Object")]  
    public Container container;
    [SerializeField]ITV itv;
 
    
    Vector3 containerLocation;




    private void Start()
    {
        status = Status.Active;//activate but not work
        default_crane = transform.position.x;
        default_trolley = transform.position.z+5;
        itvTargetPosition = transform.position + new Vector3(0,0,5);
    }

    public void GetWorkFromITV(ITV itv)//if itv start MoveToDSTC or MoveToLDTC
    {
        this.itv = itv;
        job = itv.job;
        container = itv.container.GetComponent<Container>();
        containerLocation = container.targetPosition;
        status = Status.Processing;

        if(itv.state == ITV.State.MoveToDS)
        {
            StartCoroutine(ReadyLD());
        }
        else if(itv.state == ITV.State.MoveToLD)
        {
            StartCoroutine(ReadyDS());//yard에서 컨테이너를 가지고 놓는 지점으로 이동시킴.
        }

        

        
    }
    public void LoadingAction()//if itv end MoveToDSTC
    {
        StartCoroutine(Loading());
    }
    public void DischargeAction()//called when ITV end  MoveToLDTC
    {
        StartCoroutine(Discharge());
    }

    IEnumerator ReadyDS()//yard에서 컨테이너를 가지고 놓는 지점으로 이동시킴. 1. 컨테이너포지션으로 이동시키고 2.스프레더로 잡고올리고 3.내려줄위치로
    {
        StartCoroutine(CraneMove(container.transform.position.x));
        yield return StartCoroutine(TrolleyMove(container.transform.position.z));

        yield return StartCoroutine(CatchContainer());

        StartCoroutine(CraneMove(default_crane));
        yield return StartCoroutine(TrolleyMove(default_trolley));

        itv.state = ITV.State.WaitLD;
        yield break;
    }

    IEnumerator Discharge()//itv 가 도착하면 readyds 로 해놓은 컨테이너를 전달
    {
        ItvArrive = true;
        IsITVHasCon = false;

        yield return StartCoroutine(SpreadMove(itv.transform.position.y + 3f));
        yield return StartCoroutine(TCtoITV());    
        yield return StartCoroutine(SpreadMove(ready_spreader));

        itv.LeaveITVToGC();

        if (waitnumber.Count != 0)
        {
            status = Status.Completed;
            waitnumber.RemoveAt(0);
        }
        else
        {
            status = Status.Active;
        }

        ItvArrive = false;
        yield break;
    }

    IEnumerator Loading()
    {
        ItvArrive = true;
        IsITVHasCon = true;

      
        yield return StartCoroutine(CatchContainer());

        itv.LeaveByTC();

        ItvArrive = false;

        yield return StartCoroutine(LoadingContainer());
        
        if (waitnumber.Count != 0)
        {
            status = Status.Completed;
            waitnumber.RemoveAt(0);
        }
        else
        {
            status = Status.Active;
        }
      
        yield break;
    }
    
    IEnumerator ReadyLD()//컨테이너 캐치 위치로 이동
    {
        StartCoroutine(CraneMove(default_crane));
        yield return StartCoroutine(TrolleyMove(default_trolley));

        yield break;
    }

    IEnumerator CatchContainer()
    {
        yield return StartCoroutine(SpreadMove(container.transform.position.y+2.5f));

        if (itv.state == ITV.State.WaitDS)
        {
            yield return StartCoroutine(ITVtoTC());
        }
        else if (itv.state == ITV.State.MoveToLD)
        {
            yield return StartCoroutine(YardToTC());//tc가 itv 에게 컨테이너를 전달
        }

        yield return StartCoroutine(SpreadMove(ready_spreader));
        //yield return StartCoroutine(ITVtoTC());
        yield break;
    }
    IEnumerator LoadingContainer()
    {

        StartCoroutine(CraneMove(containerLocation.x));
        yield return StartCoroutine(TrolleyMove(containerLocation.z));

        yield return StartCoroutine(SpreadMove(containerLocation.y+2.5f));
        yield return StartCoroutine(TCtoYard());
        yield return StartCoroutine(SpreadMove(ready_spreader));
        yield break;
    }


   
    IEnumerator CraneMove(float targetX)
    {
        float t = 0;
        while (t <= 1)
        {
            t += tcSpeed * Time.deltaTime;
            float tcX = Mathf.Lerp(tc.position.x, targetX, t);
            tc.position = new Vector3(tcX ,tc.position.y , tc.position.z);



            yield return null;
        }
        yield break;
    }
    IEnumerator TrolleyMove(float targetZ)
    {
        float t = 0;
        while (t <= 1)
        {
            t += trolleySpeed * Time.deltaTime;
            float trolleyZ = Mathf.Lerp(trolley.position.z, targetZ, t);
            trolley.position = new Vector3(trolley.position.x , trolley.position.y , trolleyZ);



            yield return null;
        }
        yield break;
    }

    IEnumerator ITVtoTC()//con
    {
        container.transform.SetParent(spreader);

        IsSpreaderHasCon = true;
        IsITVHasCon = false;

        yield break;

    }

    IEnumerator TCtoITV()
    {
        container.transform.parent = null;
        container = null;
        itv.container.SetParent(itv.transform);

        IsSpreaderHasCon = false;
        IsITVHasCon = true;

        yield break;
    }

    IEnumerator TCtoYard() //Add to Yard
    {
        
        container.transform.SetParent(job.bay.transform);
        if(job.containers.Count == 0)
            job.bay.onjob = false;
    

        container = null;
        IsSpreaderHasCon = false;

        yield break;
    }
    IEnumerator YardToTC()//con
    {
        
        container.transform.SetParent(spreader);
        job.bay.DS(container.startIndex);
        IsSpreaderHasCon = true;

        yield break;

    }

    //////////////////////////////////////////////////////////////////////////////


}
