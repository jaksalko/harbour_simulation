using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ContainerStorage : MonoBehaviour
{
    public bool onjob;

    [Serializable]
    public struct Floor
    {
        [SerializeField]
        public Container[] containers;
    }

    [SerializeField]
    public Floor[] floors;


    public List<Vector2> emptyField;
    public List<Vector2> fullField;


    public List<Container> DischargeContainers()
    {
        List<Container> containers = new List<Container>();


        for (int i = fullField.Count - 1; i >= 0; i--)
        {
            Vector2 index = fullField[i];
            containers.Add(floors[(int)index.x].containers[(int)index.y]);
        }


        return containers;
    }

    public void Clear()
    {
        emptyField.AddRange(fullField);
        fullField.Clear();
    }


    public void LD(Vector2 index, Container container)
    {
        floors[(int)index.x].containers[(int)index.y] = container;
        emptyField.Remove(index);
        fullField.Add(index);
    }


    public Container DS(Vector2 index)
    {
        Container container = floors[(int)index.x].containers[(int)index.y];
        floors[(int)index.x].containers[(int)index.y] = null;
        fullField.Remove(index);
        emptyField.Add(index);

        return container;
    }

  
}
