using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;


public class TCWindow : MonoBehaviour
{

    [Header("Text")]
    [SerializeField] Text status = default;
    [SerializeField] Text sector = default;
    [SerializeField] Text ID =  default;

    [Header("2D View")]
    [SerializeField] GameObject CraneImage = default;
    [SerializeField] GameObject SpreaderImgae = default;
    [SerializeField] GameObject ITVImage = default;
    [SerializeField] Text ItvMessage = default;
    [SerializeField] GameObject ItvContainer = default;
    [SerializeField] GameObject SpreaderContainer = default;
    [SerializeField] RectTransform ContainerList = default;


    [Header("Itv View")]
    [SerializeField] GameObject ItvSide = default;
    [SerializeField] Text JobType = default;
    [SerializeField] Text ItvId = default;
    [SerializeField] Text ContainerCode = default;

    [Header("Job View")]
    [SerializeField] GameObject AssignedItvPrefab = default;
    [SerializeField] RectTransform content = default;



    private TransferCrane TC;
    private Vector3 initpos;
    private Vector3 initRepeatpos;
    private Vector3 uiinitRepeatPos;

    private List<ITV> itvAssignedList = new List<ITV>();
    string TcID;
    int secNum = -1;

    IDisposable[] stream = new IDisposable[7];
    public static event System.Action Close;

    private void Start()
    {

    }

    void Update()
    {
        if (transform.gameObject.activeSelf && TC != null)
        {

            SpreaderImgae.transform.localPosition = LocalizePosition(TC.transform.position, TC.spreader.transform.position);
            var sthScale = Mathf.Clamp(-(TC.transform.position.x - TC.itvTargetPosition.x) * 0.05f, -0.2f, 0.12f);
            CraneImage.transform.localScale = new Vector3(1f + sthScale, 1f + sthScale, 1f + sthScale);

        }

    }

    private void DrawContianerOnYard(int count, int index)
    {
        int i = 0;
        for (; i < count; i++)
        {
            ContainerList.GetChild(i).transform.localPosition = LocalizePosition(TC.transform.position, TC.yard.GetChild(index).GetChild(i).transform.position);
            ContainerList.GetChild(i).gameObject.SetActive(true);

        }

        for (; i < ContainerList.childCount; i++)
        {
            ContainerList.GetChild(i).gameObject.SetActive(false);
        }

    }

    private void SectorDraw(float con)
    {

        for (int i = 0; i < TC.yard.childCount; i++)
        {
            if (con == TC.yard.GetChild(i).transform.position.x)
            {
                ContainerList.gameObject.SetActive(true);
                stream[1]?.Dispose();
                secNum = i;
                sector.text = "work place :" + i.ToString();
                stream[1] = TC.yard.GetChild(i).ObserveEveryValueChanged(_ => _.childCount)
                    .Subscribe(_ => DrawContianerOnYard(_, secNum));
            }
        }

    }

    private Vector3 LocalizePosition(Vector3 standardPos, Vector3 targetPos)
    {
        var sth = targetPos.z - standardPos.z;
        var sthY = Mathf.Clamp(targetPos.y, 0f, 15f);
        var tmp = new Vector3(-sth * 6.8f, -15f + (sthY * 2.5f), 0);

        return tmp;
    }

    private void UpdateData(int count)
    {

        if (content.childCount != count)
        {
            if (content.childCount < count && count != 0)
            {
                ItvSide.SetActive(true);
                ItvMessage.gameObject.SetActive(true);
                StartCoroutine(BlinkingText(ItvMessage));

                ContainerCode.text = "   " + TC.waitnumber[0].container.GetComponent<Container>().BIC;
                ItvId.text = "ITV-00" + TC.waitnumber[0].ITVNumber.ToString();
                JobType.text = (TC.waitnumber[0].state == ITV.State.MoveToDS || TC.waitnumber[0].state == ITV.State.WaitDS) ? "DS" : "LD";


                for (int i = content.childCount; i < count; i++)
                {
                    itvAssignedList.Add(TC.waitnumber[i]);
                    itvAssignedList[i].LayerChange(CameraRAY.SelectorLayout, true);

                    GameObject item = Instantiate(AssignedItvPrefab, default(Vector3), Quaternion.identity);
                    item.transform.SetParent(content);
                    var p = item.GetComponent(typeof(AssignedItv)) as AssignedItv;

                    p.number.text = i.ToString();
                    p.context.text = TC.waitnumber[i].container.GetComponent<Container>().BIC + "   ";
                    p.context.text += "ITV-00" + TC.waitnumber[i].ITVNumber.ToString();
                    p.context.text += " -> " + TcID;
                    p.jobType.text = (TC.waitnumber[i].state == ITV.State.MoveToDS || TC.waitnumber[i].state == ITV.State.WaitDS) ? "DS" : "LD";

                }


            }

            else if (count < content.childCount)
            {
                ItvSide.SetActive(false);
                RemoveItv();
            }
        }

    }

    private void RemoveItv()
    {
        GameObject listitem = content.GetChild(0).gameObject;
        itvAssignedList[0].LayerChange(CameraRAY.DefaultLayout, false);
        itvAssignedList.RemoveAt(0);
        Destroy(listitem);
    }

    private void ItvComming(bool itvarrive)
    {
        ItvContainer.SetActive(itvarrive);
        ItvMessage.gameObject.SetActive(false);
    }

    private void SubscribeStream()
    {
        stream[0] = TC.waitnumber.ObserveEveryValueChanged(x => x.Count)
       .Subscribe(q => UpdateData(q));

        stream[2] = TC.ObserveEveryValueChanged(z => z.status)
            .Subscribe(r => status.text = "TC Stauts: " + r);

        stream[3] = TC.transform.ObserveEveryValueChanged(_x => _x.position)
            .Subscribe(r => SectorDraw(r.x));

        stream[4] = TC.ObserveEveryValueChanged(_y => _y.ItvArrive)
            .Subscribe(r => ITVImage.SetActive(r));

        stream[5] = TC.ObserveEveryValueChanged(_z => _z.IsITVHasCon)
            .Subscribe(r => ItvComming(r));

        stream[6] = TC.ObserveEveryValueChanged(__x => __x.IsSpreaderHasCon)
            .Subscribe(r => SpreaderContainer.SetActive(r));
    }


    IEnumerator BlinkingText(Text text)
    {
        while (!TC.ItvArrive)
        {
            var blink = text.color;

            if (text.color.a != 0)
            {
                yield return new WaitForSeconds(0.2f);
                blink.a = 0;
                text.color = blink;
            }
            else if (text.color.a == 0)
            {
                yield return new WaitForSeconds(0.2f);
                blink.a = 255;
                text.color = blink;
            }
            yield return null;
        }
        yield break;
    }


    public void Targeting(TransferCrane target)
    {
        if (!transform.gameObject.activeSelf)
            transform.gameObject.SetActive(true);


        TC = target;
        TcID = TC.name;
        ID.text = "TransferCrane _ " + TC.name;

        SubscribeStream();
    }

    public void Cancel()
    {
        for (int i = 0; i < stream.Length; i++)
        {
            stream[i]?.Dispose();
        }

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        foreach (var itv in itvAssignedList)
        {
            itv.LayerChange(CameraRAY.DefaultLayout, false);
        }

        Close?.Invoke();
        gameObject.SetActive(false);


    }



}