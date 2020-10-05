using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.EventSystems;

public class CameraRAY : MonoBehaviour
{
    public ITVPopup itvpopup;
    public GCPopup gcpopup;
    public GCPopup tcpopup;
    public ContainerPopup containerpopup;
    public TCWindow tcwindow;
    public GCWindow gcwindow;

    public Canvas popupcanvas;
    public Tuple<GameObject, GameObject, String> currentPopup;
    private Transform selectedTC;
    private Transform selectedGC;


    public UnityTemplateProjects.SimpleCameraController main;
    public FocusCameraController focus;
    // 0 : View 1 : Top View 2 : Focus View
    public static event Action<bool> isFocus;
    //public GameObject vesselUI;



    [Serializable]
    public class ViewPoint
    {
        public bool lockrotation;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;

        public ViewPoint(bool lockrot, Vector3 pos, Vector3 rot)
        {
            lockrotation = lockrot;
            cameraPosition = pos;
            cameraPosition = rot;
        }


    }
    public List<ViewPoint> viewPoints;// 10 viewpoint f1-f10
    public int selectedPoint;

    public const int DefaultLayout = 0;
    public const int SelectorLayout = 8;

    private void Start()
    {

        focus.ObserveEveryValueChanged(_ => _.gameObject.activeSelf)
            .Subscribe(_ => isFocus?.Invoke(_));

        var clickStream = this.UpdateAsObservable().Where(q => Input.GetMouseButtonDown(0));
        //  .Where(q => !EventSystem.current.IsPointerOverGameObject(-1));


        clickStream
           .Where(y => Input.GetMouseButtonDown(0))
             .Where(_ => EventSystem.current.currentSelectedGameObject == null)

                            .Select(_ => Camera.main.ScreenPointToRay(Input.mousePosition))
                            .Select(ray =>
                            {

                                RaycastHit result;
                                var isHit = Physics.Raycast(ray, out result, 1000.0f);
                                return Tuple.Create(isHit, result);

                            })

         .Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(200)))
         .Where(x => x.Count >= 2)
         .Subscribe(q => Focus(q[0]));


        clickStream

            .Where(y => Input.GetMouseButtonDown(0))
            .Where(_ => EventSystem.current.currentSelectedGameObject == null)
                               //.Where(_ => !EventSystem.current.IsPointerOverGameObject())
                               .Select(_ => Camera.main.ScreenPointToRay(Input.mousePosition))
                               .Select(ray =>
                               {
                                   RaycastHit result;
                                   var isHit = Physics.Raycast(ray, out result, 1000.0f);
                                   return Tuple.Create(isHit, result, Input.mousePosition);

                               })


            .Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(200)))
            .Where(x => x.Count == 1)
            .Subscribe(q => Popup(q[0]));

        ITVPopup.Close += RemovePopup;
        ContainerPopup.Close += RemovePopup;
        GCPopup.Close += RemovePopup;

        GCWindow.Close += RemoveGCWindow;
        TCWindow.Close += RemoveTCWindow;
    }

    private void Popup(Tuple<bool, RaycastHit, Vector3> q)
    {
        //if (EventSystem.current.currentSelectedGameObject != null)
        //{
        //    //Debug.Log(EventSystem.current.currentSelectedGameObject);

        //    return;
        //}
        if (q.Item1)
        {
            if (q.Item2.transform.CompareTag("container"))
            {
                var popup = Instantiate(containerpopup, q.Item3, Quaternion.identity);
                popup.transform.SetParent(popupcanvas.transform);
                popup.GetContainer(q.Item2.transform.parent);
                PopupController(Tuple.Create(q.Item2.transform.parent.gameObject, popup.transform.gameObject, "container"));

            }
            if (q.Item2.transform.CompareTag("ITV"))
            {
                var popup = Instantiate(itvpopup, q.Item3, Quaternion.identity);
                popup.transform.SetParent(popupcanvas.transform);
                popup.GetITV(q.Item2.transform.parent);
                PopupController(Tuple.Create(q.Item2.transform.parent.transform.gameObject, popup.transform.gameObject, "ITV"));
            }

            if (q.Item2.transform.CompareTag("GC"))
            {

                var popup = Instantiate(gcpopup, q.Item3, Quaternion.identity);
                popup.transform.SetParent(popupcanvas.transform);
                popup.GetGC(q.Item2.transform.parent);
                PopupController(Tuple.Create(q.Item2.transform.parent.parent.transform.gameObject, popup.transform.gameObject, "GC"));

            }

            if (q.Item2.transform.CompareTag("TransferCrane"))
            {

                var popup = Instantiate(tcpopup, q.Item3, Quaternion.identity);
                popup.transform.SetParent(popupcanvas.transform);
                popup.GetGC(q.Item2.transform.parent);
                PopupController(Tuple.Create(q.Item2.transform.parent.parent.transform.gameObject, popup.transform.gameObject, "TransferCrane"));

            }
        }
    }

    private void Focus(Tuple<bool, RaycastHit> q)
    {
        var check = q.Item1;

        if (check && q.Item2.transform.CompareTag("ITV"))
        {
            Debug.Log("Focus");
            FocusCamera(q.Item2.transform);
        }

        if (check && q.Item2.transform.CompareTag("GC"))
        {
            //gcwindow.Targeting(q.Item2.transform.parent.parent.GetComponent<GantryCrane>());
            var selected = q.Item2.transform.parent.parent;
            //selectedGC = WindowController(selectedGC, selected);
            //FocusCamera(selected.transform);

            callGCWindow(selected);
        }

        if (check && q.Item2.transform.CompareTag("TransferCrane"))
        {
            //tcwindow.Targeting(q.Item2.transform.parent.parent.GetComponent<TransferCrane>());
            var selected = q.Item2.transform.parent.parent;
            //selectedTC = WindowController(selectedTC, selected);
            //FocusCamera(selected.transform);

            callTCWindow(selected);
        }

    }


    private void PopupController(Tuple<GameObject, GameObject, String> data)
    {
        if (currentPopup?.Item1 != null)
        {
            RemovePopup();
        }

        if (data.Item3 == "ITV")
        {
            data.Item1.GetComponent<ITV>().LineRenderController(true);
        }

        LayerChange(data.Item1.transform, SelectorLayout);
        currentPopup = data;

    }

    private Transform WindowController(Transform selected, Transform t, bool which)
    {
        if (selected != null)
        {
            LayerChange(selected, DefaultLayout);
            if (which)
                tcwindow.Cancel();
            else
                gcwindow.Cancel();

            selected = null;
        }

        LayerChange(t, SelectorLayout);
        selected = t;

        return selected;
    }


    private void LayerChange(Transform t, int a)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                LayerChange(child, a);
            }
        }

        t.gameObject.layer = a;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            main.SetCamera(viewPoints[0].cameraPosition, viewPoints[0].cameraRotation, viewPoints[0].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            main.SetCamera(viewPoints[1].cameraPosition, viewPoints[1].cameraRotation, viewPoints[1].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            main.SetCamera(viewPoints[2].cameraPosition, viewPoints[2].cameraRotation, viewPoints[2].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            main.SetCamera(viewPoints[3].cameraPosition, viewPoints[3].cameraRotation, viewPoints[3].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            main.SetCamera(viewPoints[4].cameraPosition, viewPoints[4].cameraRotation, viewPoints[4].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            main.SetCamera(viewPoints[5].cameraPosition, viewPoints[5].cameraRotation, viewPoints[5].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            main.SetCamera(viewPoints[6].cameraPosition, viewPoints[6].cameraRotation, viewPoints[6].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            main.SetCamera(viewPoints[7].cameraPosition, viewPoints[7].cameraRotation, viewPoints[7].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            main.SetCamera(viewPoints[8].cameraPosition, viewPoints[8].cameraRotation, viewPoints[8].lockrotation);
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            main.SetCamera(viewPoints[9].cameraPosition, viewPoints[9].cameraRotation, viewPoints[9].lockrotation);
        }

    }

    public void FocusCamera(Transform t)
    {
        main.gameObject.SetActive(false);
        focus.gameObject.SetActive(true);
        focus.cameraPosition = t;
        focus.target = t;
    }

    public void RemovePopup()
    {
        LayerChange(currentPopup.Item1.transform, DefaultLayout);
        Destroy(currentPopup?.Item2);

        if (currentPopup.Item3 == "ITV")
        {
            currentPopup.Item1.GetComponent<ITV>().LineRenderController(false);
        }
        currentPopup = null;
    }

    public void RemoveGCWindow()
    {
        LayerChange(selectedGC, DefaultLayout);
        selectedGC = null;
    }

    public void RemoveTCWindow()
    {
        LayerChange(selectedTC, DefaultLayout);
        selectedTC = null;
    }

    public void callTCWindow(Transform q)
    {
        var tc = q;
        selectedTC = WindowController(selectedTC, tc , true);
        tcwindow.Targeting(q.GetComponent<TransferCrane>());
        selectedTC.GetComponent<TransferCrane>().focusView.LookAt(selectedTC);
        main.gameObject.SetActive(false);
        focus.gameObject.SetActive(true);
        focus.cameraPosition = selectedTC.GetComponent<TransferCrane>().focusView;
        focus.target = selectedTC;

    }
    public void callGCWindow(Transform q)
    {
        var gc = q;
        selectedGC = WindowController(selectedGC, gc , false);
        gcwindow.Targeting(q.GetComponent<GantryCrane>());
        selectedGC.GetComponent<GantryCrane>().focusView.LookAt(selectedGC);
        main.gameObject.SetActive(false);
        focus.gameObject.SetActive(true);
        focus.cameraPosition = selectedGC.GetComponent<GantryCrane>().focusView;
        focus.target = selectedGC;
    }


}





