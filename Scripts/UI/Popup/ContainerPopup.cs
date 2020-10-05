using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerPopup : MonoBehaviour
{

    public static event System.Action Close;
    public Container container;
    public void ClosePopup()
    {
        Close?.Invoke();
        this.gameObject.SetActive(false);
    }
    public void GetContainer(Transform target)
    {
        container = target.GetComponent<Container>();
    }
}
