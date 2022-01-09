using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron //: MonoBehaviour
{

    private int numInputs;
    private float[] weights;
    private float bias = 1;
    private bool isHidden;

    public Neuron(int numInputs, bool isHidden) {
        this.numInputs = numInputs;
        this.weights = new float[this.numInputs + 1];
        genWeights(-2.0f, 2.0f);
        this.isHidden = isHidden;
    }

    public Neuron(float[] weights, bool isHidden) 
    {
        numInputs = weights.Length - 1;
        this.weights = weights;
        this.isHidden = isHidden;
    }

    /* The activation function varies depending on if neuron is hidden
     * -If hidden: ReLU
     * -If output: Sigmoid
     * 
     * Takes an array of inputs,
     * Returns neuron output
    */
    public float activate(float[] inputs) {
        //Debug.Log("Weights = " + string.Join(" | ", new List<float>(weights).ConvertAll(i => i.ToString()).ToArray()));

        if (inputs.Length < numInputs) Debug.LogError("too few inputs into neuron");
        else if (inputs.Length > numInputs) Debug.LogError("too many inputs into neuron");

        float output = dotProd(inputs);
        if (isHidden) return ReLU(output);
        else return sigmoid(output);
    }

    private float ReLU(float output)
    {
        //Debug.Log("neuron ReLU output: " + Mathf.Max(0, output));
        return Mathf.Max(0, output);
    }

    private float sigmoid(float output)
    {
        //Debug.Log("neuron sigmoid output: " + (1 / (1 + Mathf.Exp(-output))));
        return 1/(1 + Mathf.Exp(-output));
    }


    /* Return the dot product between the input and weight arrays (+bis) */
    private float dotProd(float[] inputs)
    {
        float prod = 0;
        for (int i = 0; i < numInputs; i++) {
            prod += inputs[i] * weights[i];
        }
        prod += bias * weights[numInputs];
        //Debug.Log("neuron raw output: " + prod);
        return prod;
    }

    public void mutate(float range, float chance) { 
        for(int i = 0; i < weights.Length; i++)
        {
            if (Random.Range(0f, 1f) < chance)
                weights[i] += Random.Range(-range, range);
        }
    }

    private void genWeights (float lowerLim, float upperLim)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(lowerLim, upperLim);
        }
    }

    public float[] getWeights() 
    {
        return copyArray(weights);
    }


    private float[] copyArray(float[] original)
    {
        float[] copy = new float[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = original[i];
        }
        return copy;
    }

}
