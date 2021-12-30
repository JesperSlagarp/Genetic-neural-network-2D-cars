using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    //public int timeScale;
    [SerializeField]
    private int intervall = 500;
    [SerializeField]
    private int intervallCounter;
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private int numAlive;

    private GameObject[] cars;
    private NeuralNetwork[] carsNN;
    private Driving[] carsDriving;

    private int currBestIndex;

    [SerializeField]
    private int genSize = 100;
    // Start is called before the first frame update
    void Start()
    {
        numAlive = genSize;
        cars = new GameObject[genSize];
        carsNN = new NeuralNetwork[genSize];
        carsDriving = new Driving[genSize];

        for (int i = 0; i < genSize; i++)
        {
            GameObject temp = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            cars[i] = temp;
            carsNN[i] = temp.GetComponent<NeuralNetwork>();
            //Debug.Log("Before initLayer");
            carsNN[i].initLayer(6, 6, true);
            //Debug.Log("After first initLayer");
            carsNN[i].initLayer(6, 6, true);
            //carsNN[i].initLayer(5, 7, true);
            //Debug.Log("After second initLayer");
            carsNN[i].initLayer(2, 6, false);
            //Debug.Log("After last initLayer");
            carsDriving[i] = temp.GetComponent<Driving>();
        }


    }
    void FixedUpdate()
    {
        intervallCounter++;
        if (intervallCounter >= intervall || numAlive < 1)
        {
            sort();
            currBestIndex = genSize - 1;
            Debug.Log("Best fitness: " + carsDriving[genSize - 1].fitness);
            numAlive = genSize;
            newGen();
            intervallCounter = 0;
        }
        currBestIndex = mostFitAgentIndex();
        //Time.timeScale = timeScale;
    }

    private void sort()
    {
        //for (int i = 0; i < genSize; i++)
            //Debug.Log("Index before sort: " + i + ": " + carsDriving[i].fitness);
        for (int i = 1; i < genSize; ++i)
        {
            //Debug.Log("Start of loop");
            GameObject key = cars[i];
            int j = i - 1;

            while (j >= 0 && carsDriving[j].fitness > key.GetComponent<Driving>().fitness)
            {
                //Debug.Log("Inner loop i:" + i + " j: " + j);
                //Debug.Log(carsDriving[j].fitness + ">?" + carsDriving[i].fitness);
                cars[j + 1] = cars[j];
                carsDriving[j + 1] = carsDriving[j];
                carsNN[j + 1] = carsNN[j];
                j--;
            }
            cars[j + 1] = key;
            carsDriving[j + 1] = key.GetComponent<Driving>();
            carsNN[j + 1] = key.GetComponent<NeuralNetwork>();
        }
        //for (int i = 0; i < genSize; i++)
          //  Debug.Log("Index: " + i + ": " + carsDriving[i].fitness);
    }
    
    private int mostFitAgentIndex() {
        float bestFitness = 0;
        int index = 0;

        for (int i = 0; i < genSize; i++)
        {
            if (carsDriving[i].fitness > bestFitness)
            {
                bestFitness = carsDriving[i].fitness;
                index = i;
            }
        }
        //Debug.Log("Best fitness: " + bestFitness);

        return index;
    }

    private GameObject mix(GameObject parentOne, GameObject parentTwo) 
    {
        NeuralNetwork NN1 = parentOne.GetComponent<NeuralNetwork>();
        NeuralNetwork NN2 = parentTwo.GetComponent<NeuralNetwork>();
        GameObject offspring = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        NeuralNetwork offspringNN = offspring.GetComponent<NeuralNetwork>();
        
        for (int i = 0;  i < NN1.numLayers; i++)
        {
            float[][] weights1 = NN1.getLayerWeights(i);
            float[][] weights2 = NN2.getLayerWeights(i);
            float[][] mixedWeights = new float[weights1.Length][];
            for(int j = 0; j < weights1.Length; j++)
            {
                mixedWeights[j] = new float[weights1[j].Length];
                for(int k = 0; k < weights1[j].Length; k++)
                {
                    int geneFromParent = Random.Range(1, 3); //1 or 2 
                    if(geneFromParent == 1)
                    {
                        mixedWeights[j][k] = weights1[j][k];
                    } 
                    else
                    {
                        mixedWeights[j][k] = weights2[j][k];
                    }
                }
            }
            if (i == NN1.numLayers - 1) offspringNN.initLayer(mixedWeights, false); //outputlayer
            else offspringNN.initLayer(mixedWeights, true); //hidden layer
        }
        offspringNN.mutate(0.3f);
        return offspring;
    }

    private GameObject copy(GameObject Original)
    {
        NeuralNetwork origNN = Original.GetComponent<NeuralNetwork>();
        GameObject copy = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        NeuralNetwork copyNN = copy.GetComponent<NeuralNetwork>();
        for (int i = 0; i < origNN.numLayers; i++)
        {
            float[][] origWeights = origNN.getLayerWeights(i);
            float[][] copyWeights = new float[origWeights.Length][];
            for (int j = 0; j < origWeights.Length; j++)
            {
                copyWeights[j] = new float[origWeights[j].Length];
                for (int k = 0; k < origWeights[j].Length; k++)
                {                  
                        copyWeights[j][k] = origWeights[j][k];
                }
            }
            if (i == origNN.numLayers - 1) copyNN.initLayer(copyWeights, false); //outputlayer
            else copyNN.initLayer(copyWeights, true); //hidden layer
        }

        return copy;
    }

    private void newGen()
    {
        List<GameObject> selected = new List<GameObject>();

        GameObject bestAgent = copy(cars[genSize - 1]);

        //Add the best 10% of agents into list
        for (int i = genSize; i > genSize * 0.9f; i--) 
        {
            selected.Add(cars[i - 1]);
        }

        int spawnedAgents = 0;
        while(selected.Count > 1) //While there is at least a pair left among the selected agents
        {
            //Get two random parents from the selected-list
            int p1Index = Random.Range(0, selected.Count - 1);
            GameObject parentOne = selected[p1Index];
            selected.RemoveAt(p1Index);

            int p2Index = Random.Range(0, selected.Count - 1);
            GameObject parentTwo = selected[p2Index];
            selected.RemoveAt(p2Index);

            //Debug.Log("Parent 1 fitness: " + parentOne.GetComponent<Driving>().fitness);
            //Debug.Log("Parent 2 fitness: " + parentTwo.GetComponent<Driving>().fitness);

            for (int i = 0; i < 20; i++)
            {
                GameObject offspring = mix(parentOne, parentTwo);
                Destroy(cars[spawnedAgents]);
                cars[spawnedAgents] = offspring;
                carsNN[spawnedAgents] = offspring.GetComponent<NeuralNetwork>();
                carsDriving[spawnedAgents] = offspring.GetComponent<Driving>();
                spawnedAgents++;
            }
        }
        /*Make sure the last generation's best agent is kept*/
        Destroy(cars[genSize - 1]);
        cars[genSize - 1] = bestAgent;
        carsNN[genSize - 1] = bestAgent.GetComponent<NeuralNetwork>();
        carsDriving[genSize - 1] = bestAgent.GetComponent<Driving>();

        /*
        for (int i = 0; i < genSize; i++)
        {
            if (i != currBestIndex)
            {
                Destroy(cars[i]);
                carsNN[i] = null;
                carsDriving[i] = null;

                cars[i] = Instantiate(carPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0));
                carsNN[i] = cars[i].GetComponent<NeuralNetwork>();
                for (int j = 0; j < 2; j++) // hidden layers
                {
                    float[][] hiddenweights = cars[currBestIndex].GetComponent<NeuralNetwork>().getLayerWeights(j);
                    carsNN[i].initLayer(hiddenweights, true);
                }
                float[][] weights = cars[currBestIndex].GetComponent<NeuralNetwork>().getLayerWeights(2);
                carsNN[i].initLayer(weights, false);
                carsNN[i].mutate(1f);
                carsDriving[i] = cars[i].GetComponent<Driving>();
            }
            else
            {
                cars[i].transform.position = spawnPoint.position;
                cars[i].transform.rotation = spawnPoint.rotation;
                carsDriving[i].enable();
            }
        }*/
    }

    public void decreaseAlive()
    {
        numAlive--;
    }

    public GameObject getBestAgent()
    {

        return cars[currBestIndex];
    }

    private void OnDisable()
    {
        for(int i = 0; i < genSize; i++)
        {
            Destroy(cars[i]);
        }
    }
}
