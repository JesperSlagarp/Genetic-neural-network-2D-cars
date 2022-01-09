using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IO : MonoBehaviour
{
    [SerializeField]
    int timeScale;
    string m_Path;

    [SerializeField]
    private GameObject carPrefab;
    private Transform spawnPoint;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        timeScale = 1;

        //Get the path of the Game data folder
        m_Path = Application.persistentDataPath; 
    }

    private void Start()
    {
        spawnPoint = GameObject.FindWithTag("Respawn").transform;
    }

    private void test()
    {
        GeneticAlgorithm GA = GameObject.FindWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>();
        GA.isTesting = true;
        //GameObject agent = loadBestAgent();
        //Invoke("testFitness", 120);
    }

    private void testFitness()
    {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Cars");
        for(int i = 0; i < agents.Length; i++)
        {
            Debug.Log("fitness: " + agents[i].gameObject.GetComponent<Driving>().fitness);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("t"))
            test();
        if (Input.GetKeyDown("y"))
            gotoTestMap();

        if (Input.GetKeyDown("s"))
            saveBestAgent();
        if (Input.GetKeyDown("d"))
            disableGA();
        if (Input.GetKeyDown("l"))
            loadBestAgent();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            nextScene();

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            timeScale++;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            timeScale--;

        /*if (Input.GetKeyDown("a"))
            GameObject.Find("Finish").GetComponent<Finish>().isEnabled = true;
        if (Input.GetKeyDown("m"))
            GameObject.Find("Finish").GetComponent<Finish>().isEnabled = false;*/

        Time.timeScale = timeScale;

    }

    private void gotoTestMap()
    {
        SceneManager.LoadScene("TestTrack");
        disableGA();
    }

    public void nextScene() //Rotate scenes
    {
        Debug.Log("Active scene before nextScene: " + SceneManager.GetActiveScene().buildIndex);
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("nextScene: " + nextScene + " scenecount: " + SceneManager.sceneCountInBuildSettings);
        if (nextScene >= SceneManager.sceneCountInBuildSettings - 1)
            nextScene = 1;
        Debug.Log("new index: " + nextScene);
        SceneManager.LoadScene(nextScene);
        Debug.Log("Active scene after nextScene: " + SceneManager.GetActiveScene().buildIndex);
    }

    private void saveBestAgent()
    {
        Debug.Log(m_Path);
        StreamWriter sw = new StreamWriter(m_Path + "/save.txt");
        GeneticAlgorithm GA = GameObject.FindWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>();
        NeuralNetwork NN = GA.getBestAgent().GetComponent<NeuralNetwork>();

        sw.WriteLine(NN.numLayers);
        for (int i = 0; i < NN.numLayers; i++)
        {
            
            float[][] layerWeights = NN.getLayerWeights(i);
            sw.WriteLine(layerWeights.Length);
            for (int j = 0; j < layerWeights.Length; j++)
            {
                //string neuron = layerWeights[j].Length + "\n";
                sw.WriteLine(layerWeights[j].Length);
                for (int k = 0; k < layerWeights[j].Length; k++)
                {
                    //neuron += layerWeights[j][k] + "\n";
                    sw.WriteLine(layerWeights[j][k]);
                }
                //sw.WriteLine(neuron);
            }
        }
        sw.Close();
    }

    public void saveAgent(GameObject agent)
    {
        Debug.Log(m_Path);
        StreamWriter sw = new StreamWriter(m_Path + "/save.txt");
        NeuralNetwork NN = agent.GetComponent<NeuralNetwork>();

        sw.WriteLine(NN.numLayers);
        for (int i = 0; i < NN.numLayers; i++)
        {

            float[][] layerWeights = NN.getLayerWeights(i);
            sw.WriteLine(layerWeights.Length);
            for (int j = 0; j < layerWeights.Length; j++)
            {
                //string neuron = layerWeights[j].Length + "\n";
                sw.WriteLine(layerWeights[j].Length);
                for (int k = 0; k < layerWeights[j].Length; k++)
                {
                    //neuron += layerWeights[j][k] + "\n";
                    sw.WriteLine(layerWeights[j][k]);
                }
                //sw.WriteLine(neuron);
            }
        }
        sw.Close();
    }

    private GameObject loadBestAgent() 
    {
        spawnPoint = GameObject.FindWithTag("Respawn").transform;
        GameObject temp = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        NeuralNetwork NN = temp.GetComponent<NeuralNetwork>();
        StreamReader sr = new StreamReader(m_Path + "/save.txt");
        int numLayers = int.Parse(sr.ReadLine());
        for(int i = 0; i < numLayers; i++)
        {
            int numNeurons = int.Parse(sr.ReadLine());
            float[][] weights = new float[numNeurons][];
            for (int j = 0; j < numNeurons; j++)
            {
                int numWeights = int.Parse(sr.ReadLine());
                weights[j] = new float[numWeights];
                for(int k = 0; k < numWeights; k++)
                {
                    weights[j][k] = float.Parse(sr.ReadLine());
                }
            }
            if(i == numLayers - 1)
                NN.initLayer(weights, false);
            else
                NN.initLayer(weights, true);
        }
        Invoke("testFitness", 120);
        return temp;
    }

    private void disableGA()
    {
        GameObject.FindWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>().destoryAgents();
        GameObject.FindWithTag("Genetic Algorithm").SetActive(false);
    }

}
