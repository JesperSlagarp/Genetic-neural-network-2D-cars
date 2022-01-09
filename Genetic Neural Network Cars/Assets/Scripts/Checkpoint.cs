using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private int addedTime;
    GeneticAlgorithm GA;
    private bool beenHit = false;
    // Start is called before the first frame update
    void Start()
    {
        try { GA = GameObject.FindGameObjectWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>(); }
        catch { }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Entered");
        if (!beenHit)
        {
            if(GA != null)
                GA.intervall += addedTime;
            beenHit = true;
        }
    }
}
