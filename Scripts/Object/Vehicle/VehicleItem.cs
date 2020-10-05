using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleItem : MonoBehaviour
{
    public ITV itv;
    public Text vehicleName;
    public CameraRAY ray;
    public void VehicleItemClicked()
    {
        ray.FocusCamera(itv.transform);
    }

    public void Initialize(ITV itv)
    {
        this.itv = itv;
        vehicleName.text = itv.name;
    }
}
