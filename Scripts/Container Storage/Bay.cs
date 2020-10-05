using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

public class Bay : ContainerStorage
{
    public int bay_num;
    float bayX;
    float bayZ;

    public float percentage;

    public event Action ChangePercentage;

    private void Awake()
    {
        bayX = transform.position.x;
        bayZ = transform.position.z;

        SetField();
        emptyField.ObserveEveryValueChanged(x => x.Count)
            .Subscribe(_ => SetField());

        fullField.ObserveEveryValueChanged(x => x.Count)
            .Subscribe(_ => SetField());

    }

    public void SetField()
    {
        emptyField.Clear();
        fullField.Clear();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (floors[i].containers[j] == null)
                    emptyField.Add(new Vector2(i, j));
                else
                    fullField.Add(new Vector2(i, j));
            }
        }
        percentage = (fullField.Count*100) / (emptyField.Count + fullField.Count);
        ChangePercentage?.Invoke();
    }

    public Vector3 GetContainerPostion(Vector2 index)
    {
        Vector3 vector;

        float y = 2.5f * index.x;//index.x = floor
        float z = bayZ + 3f * index.y;
        vector = new Vector3(bayX, y, z);
        return vector;
    }//수정해야함


    public void Add(int range)
    {
        List<Vector2> temp = emptyField;
        temp.RemoveRange(range, emptyField.Count - range);
        fullField.AddRange(temp);
        emptyField.RemoveRange(0, range);
    }

}
