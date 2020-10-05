using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class PoolItem : MonoBehaviour
{
    public Text PoolnameTxt;
    public Text qcCountTxt;
    public Text vehicleCountTxt;

    public Job job;

    public Transform craneList;
    public Transform vehicleList;

    public CraneItem craneitem;
    public VehicleItem vehicleitem;
    public IDisposable stream;

    [SerializeField] private ContentSizeFitter csf = default;
    public void AddJob(Job job)
    {
        this.job = job;

        InitializePoolData(job);

        stream = job.itvs.ObserveEveryValueChanged(c => c.Count)
            .Subscribe(_ => UpdateVehicleList());
        

    }
    void InitializePoolData(Job job)
    {
      
        var newitem = Instantiate(craneitem, transform.position, Quaternion.identity);
        newitem.transform.SetParent(craneList);
        newitem.Initialize(job.gc);

        newitem = Instantiate(craneitem, transform.position, Quaternion.identity);
        newitem.transform.SetParent(craneList);
        newitem.Initialize(job.tc);



    }
    public void UpdateVehicleList()
    {
        int vCount = vehicleList.childCount;

        for(int i = 0; i < vCount; i++)
        {
            Destroy(vehicleList.GetChild(i).gameObject);
            
        }

        
        for (int i = 0; i < job.itvs.Count; i++)
        {
          
            var newitem = Instantiate(vehicleitem, transform.position, Quaternion.identity);
            newitem.Initialize(job.itvs[i]);
            newitem.transform.SetParent(vehicleList);
            
        }

       



    }

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)csf.transform);
    }

}
