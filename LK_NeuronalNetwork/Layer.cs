using LK_NeuronalNetwork.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LK_NeuronalNetwork.Utilities.Extensions;

namespace LK_NeuronalNetwork;

public class Layer
{
    private int LayerIndex { get; set; }
    private int NumNeurons { get; }
    public double[,] Weights { get; set; }
    public double[] Inputs { get; set; }
    public double[] Outputs { get; set; }
    public double[] Biases { get; set; }
    public double[] Gradients { get; set; }
    public Layer? PrevLayer { get; set; }
    public Layer? NextLayer { get; set; }

    public Layer(int numInputs, int numNeurons, int layerIndex)
    {
        NumNeurons = numNeurons;
        Outputs = new double[numNeurons];
        Inputs = new double[numInputs];
        Weights = new double[numNeurons, numInputs];
        Biases = new double[numNeurons];
        Gradients = new double[numNeurons];

       LayerIndex = layerIndex;

        Random rand = new Random();
        for (int i = 0; i < numNeurons; i++)
        {
            Biases[i] = rand.NextDouble();
            for (int j = 0; j < numNeurons; j++)
            {
                Weights[i, j] = rand.NextDouble() - 0.5;
            }
        }
    }

    public void Forward()
    {
        for (int i = 0; i < NumNeurons; i++)
        {
            Outputs[i] = 0.0;
            for (int j = 0; j < Inputs.Length; j++)
            {
                Outputs[i] += Inputs[j] * Weights[i, j];
            }
            Outputs[i] += Biases[i];
            Outputs[i] = SigmoidActivation(Outputs[i]);
        }

        if (NextLayer != null)
        {
            NextLayer.Inputs = Outputs;
            NextLayer.Forward();
        }
    }

    public void Backward(double[] target = null)
    {
        if (NextLayer == null)
        {
            for (int i = 0; i < NumNeurons; i++)
            {
                Gradients[i] = (Outputs[i] - target[i]) * ActivationDerivative(Outputs[i]);
            }
        }
        else
        {
            for (int i = 0; i < NumNeurons; i++)
            {
                Gradients[i] = 0.0;
                for (int j = 0; j < NextLayer.NumNeurons; j++)
                {
                    Gradients[i] += NextLayer.Gradients[j] * NextLayer.Weights[j, i];
                }
                Gradients[i] *= ActivationDerivative(Outputs[i]);
            }
        }

        for (int i = 0; i < NumNeurons; i++)
        {
            for (int j = 0; j < Inputs.Length; j++)
            {
                Weights[i, j] -= Gradients[i] * Inputs[j] * 0.01;
            }
            Biases[i] -= Gradients[i] * 0.01;
        }

        if (PrevLayer != null)
        {
            PrevLayer.Backward();
        }
    }

    private double SigmoidActivation(double x) => 1.0 / (1.0 + Math.Exp(-x)); 
    private double ActivationDerivative(double x) => x * (1.0 - x);


    public void Save(string filePath)
    {
        using StreamWriter writer = new StreamWriter(filePath);
        for (int i = 0; i < NumNeurons; i++)
        {
            writer.WriteLine(string.Join(",", Weights.GetRow(i))); // Serialize weights
            writer.WriteLine(Biases[i]); // Serialize biases
        }
    }

    public void Load(string filePath)
    {
        using StreamReader reader = new StreamReader(filePath);
        for (int i = 0; i < NumNeurons; i++)
        {
            Weights.SetRow(i, reader.ReadLine().Split(',').Select(double.Parse).ToArray());
            Biases[i] = double.Parse(reader.ReadLine());
        }
    }

 


}
