using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gists;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject MapNode;
    [SerializeField] GameObject Canvas;
    List<Vector2> points;

    void Start()
    {
        //Sampling();
    }

    public void Sampling()
    {
        points = FastPoissonDiskSampling.Sampling(new Vector2(0, 0), new Vector2(1920, 1080), 200,2);
        foreach(var p in points)
        {
            Debug.Log(p);
        }
        Debug.Log(points.Count);


    }
}
