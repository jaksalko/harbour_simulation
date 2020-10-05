using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Job
{
    public enum JobType
    {
        DS,
        LD
    }


    public int job_num;
    public Ship ship;
    public Bay bay;
    public Sector sector;
    public JobType jobType;
    public List<Container> containers;
    public TransferCrane tc;
    public GantryCrane gc;
    public List<ITV> itvs;

    
    public Job(JobType jobType , List<Container> containers, TransferCrane tc,  Sector sector , Bay bay , Ship ship)
    {
        this.ship = ship;
        this.jobType = jobType;
        this.containers = containers;     
        itvs = new List<ITV>();
        this.bay = bay;
        this.sector = sector;

        this.tc = tc;
    }
    public void AssignCrane(GantryCrane gc , int num)
    {
       
        this.gc = gc;
        job_num = num;
    }
    public Job
        (int num , JobType jobType , List<Container> containers , TransferCrane tc , GantryCrane gc,Bay bay , Sector sector , Ship ship)//initialize job. def job type, set target containers.
    {
        this.ship = ship;
        this.jobType = jobType;
        this.containers = containers;
        this.tc = tc;
        this.gc = gc;
        itvs = new List<ITV>();
        this.bay = bay;
        this.sector = sector;
        job_num = num;
        

    }
     
    public void PrintJobObject()
    {
        Debug.Log(jobType + " TC :" + tc + "  GC : " + gc + "ITV : " + itvs.Count);
    }

}
