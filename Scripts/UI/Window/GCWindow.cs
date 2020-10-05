using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;


public class GCWindow : MonoBehaviour
{
    private Ship ship;

    [Header("Text")]
    [SerializeField] Text Status = default;
    [SerializeField] Text ID = default;

    [Header("2D View")]
    [SerializeField] GameObject Horizontal = default;
    [SerializeField] GameObject ZoomSector = default;
    [SerializeField] GameObject SpreaderImgae = default;
    [SerializeField] GameObject SpreaderContainer = default;
    [SerializeField] GameObject ITVImage = default;
    [SerializeField] GameObject ItvContainer = default;

    [Header("Itv View")]
    [SerializeField] GameObject ItvSide = default;
    [SerializeField] Text JobType = default;
    [SerializeField] Text ItvId = default;
    [SerializeField] Text ContainerCode = default;

    [Header("Job View")]
    [SerializeField] GameObject AssignedItvPrefab = default;
    [SerializeField] RectTransform content = default;

    private List<ITV> itvAssignedList = new List<ITV>();
    private GantryCrane GC;

    private float initLocZ;
    private string GcID;
     
    private int secNum = -1;

    private IDisposable[] stream = new IDisposable[11];
    public static event System.Action Close;

    private void ChangeShip(Ship r)
    {
        stream[10]?.Dispose();


        ship = r;

        foreach (var sector in ship.sectors)
        {
            stream[10] = sector.fullField.ObserveEveryValueChanged(_ => _.Count)
                .Subscribe(q => ArrangeSector(sector.sectorId));
        }


    }


    private int ZoomArrange()
    {
        ZoomSector.SetActive(false);
        ZoomSector.transform.SetAsLastSibling();

        int val = -1;

        for (int j = 0; j < 8; j++)
        {
            var color = ZoomSector.transform.GetChild(0).GetChild(j).GetComponent<Image>().color;
            color.a = 0;
            ZoomSector.transform.GetChild(0).GetChild(j).GetComponent<Image>().color = color;
        }

        foreach (var sector in ship.sectors)
        {
            if (GC.transform.position.z == sector.transform.position.z)
            {
                for (int j = 0; j < sector.fullField.Count; j++)
                {
                    var color = ZoomSector.transform.GetChild(0).GetChild((int)sector.fullField[j].y).GetComponent<Image>().color;
                    color.a += 0.25f;
                    ZoomSector.transform.GetChild(0).GetChild((int)sector.fullField[j].y).GetComponent<Image>().color = color;
                }

                Horizontal.transform.GetChild(sector.sectorId - 1).gameObject.SetActive(false);
                ZoomSector.transform.SetSiblingIndex(sector.sectorId - 1);
                ZoomSector.SetActive(true);

                val = sector.sectorId;
            }

        }
        return val;
    }

    private void Revive()
    {
        for (int i = 0; i < 12; i++)
        {
            if (secNum != i)
            {
                Horizontal.transform.GetChild(i).gameObject.SetActive(true);
            }

        }

    }

    private void ArrangeSector(int id)
    {
        int count = id - 1;
        int a = id - 1;

        if (id < secNum)
        {

            for (int j = 0; j < 8; j++)
            {
                var color = Horizontal.transform.GetChild(count).GetChild(j).GetComponent<Image>().color;
                color.a = 0f;
                Horizontal.transform.GetChild(count).GetChild(j).GetComponent<Image>().color = color;
            }


            for (int j = 0; j < ship.sectors[count].fullField.Count; j++)
            {
                var color = Horizontal.transform.GetChild(count).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color;
                color.a += 0.25f;
                Horizontal.transform.GetChild(count).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color = color;
            }

        }

        else if (id >= secNum)
        {

            if (secNum != -1)
            {
                a += 1;
            }
            for (int j = 0; j < 8; j++)
            {
                var color = Horizontal.transform.GetChild(a).GetChild(j).GetComponent<Image>().color;
                color.a = 0f;
                Horizontal.transform.GetChild(a).GetChild(j).GetComponent<Image>().color = color;
            }

            for (int j = 0; j < ship.sectors[count].fullField.Count; j++)
            {
                var color = Horizontal.transform.GetChild(a).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color;
                color.a += 0.25f;
                Horizontal.transform.GetChild(a).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color = color;
            }
        }

        if (id == secNum)
        {

            for (int j = 0; j < 8; j++)
            {
                var color = ZoomSector.transform.GetChild(0).GetChild(j).GetComponent<Image>().color;
                color.a = 0;
                ZoomSector.transform.GetChild(0).GetChild(j).GetComponent<Image>().color = color;
            }

            for (int j = 0; j < ship.sectors[count].fullField.Count; j++)
            {
                var color = ZoomSector.transform.GetChild(0).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color;
                color.a += 0.25f;
                ZoomSector.transform.GetChild(0).GetChild((int)ship.sectors[count].fullField[j].y).GetComponent<Image>().color = color;
            }
        }

    }

    private void LocalizePosition()
    {
        var tmp = GC.spreader.transform.position.x - GC.transform.position.x;

        if (tmp > -20)
        {
            SpreaderImgae.transform.localPosition = new Vector3(SpreaderImgae.transform.localPosition.x, Mathf.Clamp(-tmp * 1.375f, -22.3f, 0), SpreaderImgae.transform.localPosition.z);
        }
        else
        {
            SpreaderImgae.transform.localPosition = new Vector3(SpreaderImgae.transform.localPosition.x, Mathf.Clamp((-(tmp + 20) * 8.92f), 0, 176f), SpreaderImgae.transform.localPosition.z);
        }
    }



    private void LoadNDischarge(int count)
    {

        if (content.childCount != count)
        {
            if (content.childCount < count && count != 0)
            {
                ItvSide.SetActive(true);

                //JobType.text = (GC.waitnumber[0].state == ITV.State.MoveToDS || GC.waitnumber[0].state == ITV.State.WaitDS) ? "LD" : "DS";
                ContainerCode.text = "   " + GC.waitnumber[0]?.container.GetComponent<Container>().BIC;
                ItvId.text = "ITV-00" + GC.waitnumber[0]?.ITVNumber.ToString();
                JobType.text = GC.job.jobType.ToString();

                for (int i = content.childCount; i < count; i++)
                {
                    GameObject item = Instantiate(AssignedItvPrefab, default(Vector3), Quaternion.identity);
                    item.transform.SetParent(content);
                    var p = item.GetComponent(typeof(AssignedItv)) as AssignedItv;
                    itvAssignedList.Add(GC.waitnumber[i]);
                    itvAssignedList[i].LayerChange(CameraRAY.SelectorLayout, true);
                    p.number.text = i.ToString();
                    p.context.text = GC.waitnumber[i].container.GetComponent<Container>().BIC + "   ";
                    p.context.text += "ITV-00" + GC.waitnumber[i].ITVNumber.ToString();
                    p.context.text += " -> " + GcID;
                    p.jobType.text = JobType.text = GC.job.jobType.ToString();

                }


            }

            else if (count < content.childCount)
            {

                if (count == 0)
                {
                    ItvSide.SetActive(false);
                }
                else
                {
                    ContainerCode.text = "   " + GC.waitnumber[0].container.GetComponent<Container>().BIC;
                    ItvId.text = "ITV-00" + GC.waitnumber[0].ITVNumber.ToString();
                }

                RemoveItv();
            }
        }

    }

    private void LoadNDischarge(ITV x)
    {

        if (content.childCount > 1 && (GC.job.jobType == Job.JobType.DS))
        {
            RemoveItv();
            ItvSide.SetActive(false);
        }


        else if (x != null && (GC.job.jobType == Job.JobType.DS))
        {
            ItvSide.SetActive(true);

            //JobType.text = (GC.waitnumber[0].state == ITV.State.MoveToDS || GC.waitnumber[0].state == ITV.State.WaitDS) ? "LD" : "DS";
            ContainerCode.text = "   " + GC.container.GetComponent<Container>().BIC;
            ItvId.text = "ITV-00" + x.ITVNumber.ToString();
            JobType.text = GC.job.jobType.ToString();


            GameObject item = Instantiate(AssignedItvPrefab, default(Vector3), Quaternion.identity);
            item.transform.SetParent(content);
            var p = item.GetComponent(typeof(AssignedItv)) as AssignedItv;
            itvAssignedList.Add(x);
            x.LayerChange(CameraRAY.SelectorLayout, true);

            p.number.text = "0";
            p.context.text = GC.container.GetComponent<Container>().BIC + "   ";
            p.context.text += "ITV-00" + x.ITVNumber.ToString();
            p.context.text += " <- " + GcID;
            p.jobType.text = JobType.text = GC.job.jobType.ToString();
        }


    }

    private void RemoveItv()
    {
        GameObject listitem = content.GetChild(0).gameObject;
        itvAssignedList[0].LayerChange(CameraRAY.DefaultLayout, false);
        itvAssignedList.RemoveAt(0);
        Destroy(listitem);
    }


    private void SubscribeStream()
    {
        stream[0] = GC.ObserveEveryValueChanged(z => z.status)
            .Subscribe(r => Status.text = "GC Stauts: " + r);

        stream[1] = GC.ObserveEveryValueChanged(_ => _.job)
           .Where(_ => _.ship?.name != null)
           .Subscribe(_ => ChangeShip(_.ship));

        stream[2] = GC.spreader.ObserveEveryValueChanged(y => y.position)
             .Subscribe(r => LocalizePosition());

        stream[3] = GC.ObserveEveryValueChanged(_ => _.itv)
             .Subscribe(x => LoadNDischarge(x));

        stream[4] = this.ObserveEveryValueChanged(_ => _.secNum)
           .Where(r => r != -1)
           .Subscribe(r => Revive());

        stream[5] = GC.waitnumber.ObserveEveryValueChanged(_ => _.Count)
            .Subscribe(r => LoadNDischarge(r));

        stream[6] = GC.ObserveEveryValueChanged(_y => _y.ItvArrive)
            .Subscribe(r => ITVImage.SetActive(r));

        stream[7] = GC.ObserveEveryValueChanged(_z => _z.IsITVHasCon)
            .Subscribe(r => ItvContainer.SetActive(r));

        stream[8] = GC.ObserveEveryValueChanged(__x => __x.IsSpreaderHasCon)
            .Subscribe(r => SpreaderContainer.SetActive(r));

        stream[9] = GC.transform.ObserveEveryValueChanged(x => x.position.z)
            .Where(_ => ship?.name != null)
            .Subscribe(q => secNum = ZoomArrange());



    }



    public void Targeting(GantryCrane target)
    {
        if (!transform.gameObject.activeSelf)
            transform.gameObject.SetActive(true);

        GC = target;
        SubscribeStream();
        initLocZ = GC.transform.position.z;
        GcID = GC.name;
        ID.text = "Gantry Crane _ " + GC.name;
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