using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class TaskBuilder {

    Task task;
    System.Random rand = new System.Random();


    List<float> mean;
    List<float> stdDev;

    public TaskBuilder(Task t, List<float> mean, List<float> stdDev){
        this.mean = mean;
        this.stdDev = stdDev;
        task = t;
    }

    public Task GetTask(int i) {
        Task c = task.Clone();
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0f * Math.Log(u1)) * Math.Sin(2.0f * Math.PI * u2);
        c.Set((float) (mean[i] + stdDev[i]*randStdNormal));
        return c;
    }



}