using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;

public class TreeView : MonoBehaviour
{
    List<PoolItem> pools;

    public PoolItem poolItem;
    public VehicleItem vehicleItem;
    public CraneItem craneItem;

    public Transform content;
    public Transform search_content;
    public GameObject treeViewScroll;
    public GameObject searchScroll;

    public InputField inputTxt;
    public ControlTower ct;

    // Start is called before the first frame update
    void Awake()
    {
       
        pools = new List<PoolItem>();
        Ship.AddPool += InstantiatePool;
        GantryCrane.RemovePool += RemovePool;
        gameObject.SetActive(false);
    }

    void InstantiatePool(Job job)
    {
        var newitem = poolItem;
       
        var pool = Instantiate(newitem, transform.position, Quaternion.identity);
        pools.Add(pool);
        pool.AddJob(job);
        pool.transform.SetParent(content);
    }
    void RemovePool(Job job)
    {

        //Debug.Log(job.job_num + "pools count : " + pools.Count);
        int poolsCount = pools.Count;
        for(int i = 0; i < poolsCount; i++)
        {
            if(pools[i].job.job_num == job.job_num)
            {
                pools[i].stream.Dispose();
                Destroy(pools[i].gameObject);
                pools.RemoveAt(i);
                
               
                break;
            }
        }
    }
    
    public void Refresh()
    {
        for(int i = 0; i < pools.Count; i++)
        {
            pools[i].UpdateVehicleList();
        }
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }
    public void OpenTreeView()
    {
        int c = search_content.transform.childCount;
        Debug.Log(c);
        for (int i = 0; i < c; i++)
        {
            Destroy(search_content.GetChild(i).gameObject);
        }

        treeViewScroll.SetActive(true);
        searchScroll.SetActive(false);
    }
    public void Search()
    {
        treeViewScroll.SetActive(false);
        searchScroll.SetActive(true);

        int c = search_content.transform.childCount;
        Debug.Log(c);
        for (int i = 0; i < c; i++)
        {
            Destroy(search_content.GetChild(i).gameObject);
        }




        string input = inputTxt.text;

        for(int i = 0; i < ct.itvs.Count; i++)
        {
            if(ct.itvs[i].name.Contains(input))
            {
                var searchitem = Instantiate(vehicleItem, transform.position, Quaternion.identity);
                searchitem.itv = ct.itvs[i];
                searchitem.vehicleName.text = ct.itvs[i].name;
                if (searchitem.itv.state == ITV.State.Break)
                    searchitem.vehicleName.fontStyle = FontStyle.Normal;
                searchitem.transform.SetParent(search_content);
            }
        }
        for (int i = 0; i < ct.gantryCranes.Count; i++)
        {
            if (ct.gantryCranes[i].name.Contains(input))
            {
                var searchitem = Instantiate(craneItem, transform.position, Quaternion.identity);
                searchitem.crane = ct.gantryCranes[i];
                searchitem.craneName.text = ct.gantryCranes[i].name;
                if (searchitem.crane.status == Crane.Status.Active)
                    searchitem.craneName.fontStyle = FontStyle.Normal;
                searchitem.transform.SetParent(search_content);
            }
        }
        for (int i = 0; i < ct.transferCranes.Count; i++)
        {
            if (ct.transferCranes[i].name.Contains(input))
            {
                var searchitem = Instantiate(craneItem, transform.position, Quaternion.identity);
                searchitem.crane = ct.transferCranes[i];
                searchitem.craneName.text = ct.transferCranes[i].name;
                if (searchitem.crane.status == Crane.Status.Active)
                    searchitem.craneName.fontStyle = FontStyle.Normal;
                searchitem.transform.SetParent(search_content);
            }
        }
    }
}
