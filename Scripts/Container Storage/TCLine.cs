using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
public class TCLine : MonoBehaviour
{
    public List<Block> blocks;
    public List<Bay> bays;

    public Text percentageTxt;
    public Slider percent_slider;
    public GameObject percentUI;

    
    // Start is called before the first frame update
    void Awake()
    {
        CameraRAY.isFocus += PercentUIOn;
        Camera.main.transform.ObserveEveryValueChanged(_ => _.position.y)
            .Where(_ => _ < 200)
            .Subscribe(_ => percentUI.SetActive(false));
        Camera.main.transform.ObserveEveryValueChanged(_ => _.position.y)
            .Where(_ => _ >= 200)
            .Subscribe(_ => percentUI.SetActive(true));
        for (int i = 0; i < blocks.Count; i++)
        {
            bays.AddRange(blocks[i].bays);
        }
        foreach (var bay in bays)
        {
          
            bay.ChangePercentage += ChangePercentage;
            bay.SetField();
        }

    }

    void PercentUIOn(bool focusOn)
    {
        percentUI.SetActive(!focusOn);
    }
    void ChangePercentage()
    {
        float percentage = 0;
        for(int i = 0; i < bays.Count; i++)
        {
            percentage += bays[i].percentage;
        }
        percentage = percentage / bays.Count;
        string form = string.Format("{0:f1}", percentage);
        percentageTxt.text = form + "%";
        percent_slider.value = percentage / 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
