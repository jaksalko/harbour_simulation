using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;

public class ITV : MonoBehaviour
{
    public enum State { MoveToLD, MoveToDS, WaitLD, WaitDS, MoveToBreak, Break };

    //ITV는 정해진 움직임을 실행합니다. 맵 혹은 오브젝트의 위치가 변한다면 수정해야합니다. 

    private Vector3 initPos;
    private Vector3 rotation;

    [Header("VARIABLE")]

    public int ITVNumber = 0;
    public State state = State.Break;

    [Header("OBJECT")]
    public GantryCrane GC;
    public TransferCrane TC;
    public Transform container;

    public Job job;

    LineRenderer lr;
    NavMeshAgent navAgent;
    Vector3 destination = Vector3.zero;
    bool isRender = false;

   

    float maxSpeed = 50;
    [SerializeField]
    float speed = 0; 
    public float accelPower;



    [SerializeField]
    bool accel = true;
    public bool break_;
   
    [SerializeField] float break_speed = default;
    [SerializeField] Vector2 breakMinMax = default;

    public float maxDistance = 0;



    public float dir;
    void Start()
    {

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = true;
        navAgent.updatePosition = false;
        navAgent.updateRotation = false;

        lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        lr.startColor = Color.yellow;
        lr.endColor = Color.green;
        lr.gameObject.GetComponent<Renderer>().enabled = false;

        initPos = transform.position;
        rotation = transform.localEulerAngles;
    }

    private void Update()
    {

        if (destination != Vector3.zero && destination != null && isRender)
        {
            Draw();
        }
    }

    public void LDStart(Job job)
    {
        state = State.MoveToLD;
        job.itvs.Add(this);
        GC = job.gc;
        TC = job.tc;
        this.job = job;
        StartCoroutine(MoveToLDVessel());
    }

    public void DSStart(Job job)
    {
        state = State.MoveToLD;
        job.itvs.Add(this);
        GC = job.gc;
        TC = job.tc;
        this.job = job;
        StartCoroutine(MoveToLDTC());
        //StartCoroutine(MoveToLDVessel());
    }

    public void LeaveITVToTC()
    {
        StartCoroutine(MoveToDSTC());
    }
    public void LeaveITVToGC()
    {
        GC.waitnumber.Add(this);
        //GC.GetWorkFromITV(this);
        StartCoroutine(MoveToDSVessel());
    }


    IEnumerator MoveToLDVessel()
    {
        state = State.MoveToLD;

        destination = GC.itvTargetPosition;

        dir = Mathf.Sign(GC.itvTargetPosition.z - transform.position.z);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 2f));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 550f)));
        yield return StartCoroutine(MoveStraight(
            new Vector3(GC.itvTargetPosition.x + 4.3f * (dir), transform.position.y, transform.position.z)));

        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, GC.itvTargetPosition.z - 2 * (dir))));
        yield return StartCoroutine(MoveStraight(new Vector3(GC.itvTargetPosition.x, transform.position.y, transform.position.z)));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, GC.itvTargetPosition.z)));

        state = State.WaitLD;
    }

    IEnumerator MoveToDSVessel()
    {
        state = State.MoveToDS;

        destination = new Vector3(GC.cabin_ready_position, GC.transform.position.y, GC.transform.position.z);


        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x - 15f, transform.position.y, transform.position.z + 8.6f)));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x -15f, transform.position.y, transform.position.z)));


        while (GC.waitnumber[0] != this)
        {
            yield return new WaitForSeconds(1f);
        }
        GC.GetWorkFromITV(this);

        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x - 8f, transform.position.y, transform.position.z + 8.6f)));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z)));

        dir = Mathf.Sign(GC.transform.position.z - transform.position.z);
        yield return StartCoroutine(MoveStraight(
           new Vector3(GC.cabin_ready_position + 4.3f * (dir), transform.position.y, transform.position.z)));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, GC.transform.position.z - 2 * (dir))));
        yield return StartCoroutine(MoveStraight(new Vector3(GC.cabin_ready_position, transform.position.y, transform.position.z)));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, GC.transform.position.z)));

        while (state != State.WaitDS)
        {
            yield return new WaitForSeconds(1f);
        }


        GC.LoadingCallByITV();
        //...

    }

    //Vessel Load
    IEnumerator MoveToLDTC()
    {
        state = State.MoveToLD;

        destination = TC.itvTargetPosition;

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 2f));

        while (TC.waitnumber.IndexOf(this) >2 )
        {
            yield return new WaitForSeconds(1f);
        }

        if (TC.itvTargetPosition.x < 180f)
        {
            dir = Mathf.Sign(transform.position.x - TC.transform.position.x);
            var sth =  TC.TCnum % 3 == 0 ? TC.TCnum / 3 - 1: TC.TCnum / 3;
           
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 60f + (sth * 240) + (10 * dir))));
            yield return StartCoroutine(MoveStraight(new Vector3(155f + ( 4.3f * (TC.TCnum/3)), transform.position.y, transform.position.z)));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, TC.itvTargetPosition.z +
                (15f * Mathf.Clamp(TC.waitnumber.IndexOf(this), 0f, 1f)))));
            yield return StartCoroutine(MoveStraight(new Vector3(TC.itvTargetPosition.x + 35f, transform.position.y, transform.position.z)));
           while (TC.waitnumber[0] != this)
            {
                yield return StartCoroutine(MoveStraight(new Vector3(-45f, transform.position.y, transform.position.z)));
               // yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, initPos.z)));
                //transform.localEulerAngles = rotation;
                StartCoroutine(MoveToLDTC());

                yield break;
            }
        }

        else
        {
            yield return StartCoroutine(MoveStraight(new Vector3(initPos.x + TC.TCnum / 3, transform.position.y, transform.position.z - 30f)));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, TC.itvTargetPosition.z +
                (15f * Mathf.Clamp(TC.waitnumber.IndexOf(this), 0f, 1f)))));
            yield return StartCoroutine(MoveStraight(new Vector3(TC.itvTargetPosition.x + 35f, transform.position.y, transform.position.z)));

            while (TC.waitnumber[0] != this)
            {
                var sth = TC.TCnum % 3 == 0 ? TC.TCnum / 3 - 1 : TC.TCnum / 3;
                dir = Mathf.Sign(transform.position.x - TC.transform.position.x);
                yield return StartCoroutine(MoveStraight(new Vector3(195f, transform.position.y, transform.position.z)));
                yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 60f + (sth * 240) + (5 * dir))));
                yield return StartCoroutine(MoveStraight(new Vector3(initPos.x, transform.position.y, transform.position.z)));
                // transform.localEulerAngles = rotation;

                StartCoroutine(MoveToLDTC());
                yield break;

            }
        }



        

        yield return StartCoroutine(MoveStraight(new Vector3(TC.itvTargetPosition.x + 5f, transform.position.y, TC.itvTargetPosition.z)));
        yield return StartCoroutine(MoveStraight(new Vector3(TC.itvTargetPosition.x, transform.position.y, transform.position.z)));

        TC.GetWorkFromITV(this);

        while (state != State.WaitLD)
        {
            yield return new WaitForSeconds(1f);
        }
       
        lr.positionCount = 0;
        TC.DischargeAction();

    }

    IEnumerator MoveToDSTC()
    {
        state = State.MoveToDS;

        destination = TC.itvTargetPosition;
        dir = Mathf.Sign(TC.itvTargetPosition.z + 25f - transform.position.z);
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x + 8.6f * (dir), transform.position.y, transform.position.z - (2 *dir) )));
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, TC.itvTargetPosition.z + 25f)));
        while (TC.waitnumber[0] != this)
        {
            yield return new WaitForSeconds(1f);
        }
        TC.GetWorkFromITV(this);

        if (TC.itvTargetPosition.x < 180f)
        {
            yield return StartCoroutine(MoveStraight(new Vector3(155, transform.position.y, transform.position.z)));
        }
        else
        {
            yield return StartCoroutine(MoveStraight(new Vector3(390, transform.position.y, transform.position.z)));
        }
        yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, TC.itvTargetPosition.z)));
        yield return StartCoroutine(MoveStraight(new Vector3(TC.itvTargetPosition.x, transform.position.y, transform.position.z)));

        state = State.WaitDS;
        TC.LoadingAction();

    }

    public void LeaveByTC()
    {
        state = State.MoveToBreak;
        StartCoroutine(MoveToBreak(true));
    }

    public void LeaveByGC()
    {
        state = State.MoveToBreak;
        StartCoroutine(MoveToBreak(false));
    }

    IEnumerator MoveToBreak(bool sth)
    {
        destination = initPos;

        if (sth)
        {
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z + 30f)));
            yield return StartCoroutine(MoveStraight(new Vector3(initPos.x - 20f, transform.position.y, transform.position.z)));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 760f)));
            yield return StartCoroutine(MoveStraight(new Vector3(initPos.x, transform.position.y, transform.position.z)));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, initPos.z)));
        }
        else
        {
            var ran = UnityEngine.Random.Range(-1, 3);
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x -20f, transform.position.y, transform.position.z + (2 * Mathf.Sign((740 - ran * 4.3f ) - transform.position.z)))));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, 740 - ran * 4.3f)));
            yield return StartCoroutine(MoveStraight(new Vector3(initPos.x, transform.position.y, transform.position.z)));
            yield return StartCoroutine(MoveStraight(new Vector3(transform.position.x, transform.position.y, initPos.z)));
        }

        transform.position = initPos;
        transform.localEulerAngles = rotation;

        job.itvs.Remove(this);
        state = State.Break;
        yield return null;

    }

    IEnumerator MoveStraight(Vector3 position)
    {
        navAgent.enabled = false;
        navAgent.updateRotation = false;
        navAgent.updatePosition = false;

        Quaternion lookat = Quaternion.LookRotation(position - transform.position);
        Vector3 angles = lookat.eulerAngles;

        while (transform.rotation != Quaternion.Euler(angles))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(angles), 50 * 3 * Time.deltaTime);
            yield return null;
        }
        speed = 0;
        while (transform.position != position)
        {
            if (!break_)
            {
                SetSpeed();
            }
            else
            {
                speed = 0;
            }
            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            yield return null;
        }
        yield break;
    }

    IEnumerator Rotate(Vector3 rot)
    {
        while (transform.rotation != Quaternion.Euler(rot))
         {
             transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rot), speed * 3 * Time.deltaTime);
             yield return null;
         }
        speed = 0;
        /* Quaternion lookat = Quaternion.LookRotation(transform.position, destination);
          while (transform.rotation != lookat)
          {
              transform.rotation = Quaternion.RotateTowards(transform.rotation, lookat, speed * 2 * Time.deltaTime);
              yield return null;
          }*/

        yield break;
    }
    void SetSpeed()
    {
        if (accel)
        {
            speed += accelPower;
            if (speed > maxSpeed)
                speed = maxSpeed;
        }
        else
        {

            speed -= accelPower * 10;
            if (speed < 0)
                speed = 0;
        }
    }
    IEnumerator MoveUsingAgent(Vector3 dest)
    {
        navAgent.enabled = true;

        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        navAgent.SetPath(path);

        navAgent.updateRotation = false;
        navAgent.updatePosition = true;

        bool temp = true;

        Quaternion lookat = Quaternion.LookRotation(dest - transform.position);
        Vector3 angles = lookat.eulerAngles;

        while (transform.rotation != Quaternion.Euler(angles))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(angles), 50 * 3 * Time.deltaTime);
            yield return null;
        }

        while (temp)
        {
            if (Vector3.Distance(dest, transform.position) <= 2)
            {
                navAgent.enabled = false;
                temp = false;

            }
            yield return null;
        }

        yield break;
    }

    public void GetContainer(Container con)
    {
        con.transform.SetParent(transform);
        TC.waitnumber.Add(this);
        container = con.transform;
    }

    public void GetContainerInfo(Container con)
    {
        TC.waitnumber.Add(this);
        container = con.transform;
    }

    public void LayerChange(int a, bool check)
    {
        lr.gameObject.GetComponent<Renderer>().enabled = check;

        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = a;
            }
        }

        gameObject.layer = a;

    }
    public void LineRenderController(bool check)
    {
        isRender = check;
        lr.gameObject.GetComponent<Renderer>().enabled = check;
    }

    void Draw()
    {
        lr.positionCount = 0;
        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;

        List<Vector3> tmp = new List<Vector3>
        {
            transform.position
        };

        for (int i = 1; i < path.corners.Length; i++)
        {
            tmp.Add(new Vector3(path.corners[i - 1].x, 3, path.corners[i].z));
            tmp.Add(new Vector3(path.corners[i].x, 3, path.corners[i].z));
        }

        lr.positionCount = tmp.Count;

        for (int i = 0; i < tmp.Count; i++)
        {
            lr.SetPosition(i, tmp[i]);
        }
    }


    /*private void OnTriggerEnter(Collider other)
     {
         if (other.transform.CompareTag("ITV"))
         {
             trigger_++;
             float distance = Vector3.Distance(transform.position, other.transform.position);
             if (maxDistance < distance)
                 maxDistance = distance;


             accel = false;
         }

     }*/

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("ITV"))
        {

            //Debug.Log(other.name);
            float distance = Vector3.Distance(transform.position, other.transform.position);
            /*if (minDistance > distance)
                minDistance = distance;
                */
            float sp = Mathf.Lerp(breakMinMax.y, breakMinMax.x, distance / maxDistance);
            if (break_speed > sp)
            {
                break_speed = sp;
            }


            accel = false;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        speed = 0;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("ITV"))
        {
            /*trigger_--;
            Debug.Log("exit  : " + name + "," + other.name);
            if(trigger_ == 0)
            {

               maxDistance = 0;
               minDistance = 9999; }*/

            accel = true;




        }
    }
}

