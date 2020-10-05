using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

public class Sector : ContainerStorage
{
    public int sectorId;

    

    private void Awake()
    {
        SetField();
        emptyField.ObserveEveryValueChanged(x => x.Count)
            .Subscribe(_ => SetField());

        fullField.ObserveEveryValueChanged(x => x.Count)
            .Subscribe(_ => SetField());
    }

    private void SetField()
    {
        emptyField.Clear();
        fullField.Clear();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (floors[i].containers[j] == null)
                    emptyField.Add(new Vector2(i, j));
                else
                    fullField.Add(new Vector2(i, j));
            }
        }

    }

    public void Add(int range)
    {

        emptyField.RemoveRange(0, range);
    }

    public Vector3 GetContainerPostion(Vector2 index)
    {
        Vector3 vector;
        float x = -97.5f - 2.5f * index.y;
        float y = 10f + 2.5f * index.x;//index.x = floor
                                       //floors[index.x].containers[index.y].transform.position;

        float z = transform.position.z;
        //Debug.Log(z);
        vector = new Vector3(x, y, z);


        return vector;
    }


}
