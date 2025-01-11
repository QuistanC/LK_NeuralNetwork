using LK_NeuronalNetwork.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LK_NeuronalNetwork
{
    public class NeuralNetwork
    {
        private List<Layer> Layers = new();
        public int InputSize { get; private set; }
        public int OutputSize { get; private set; }
        public int NumLayers { get; private set; }
        public int NeuronsPerLayer { get; private set; }
        public string ModelName { get; set; }

        public NeuralNetwork(int inputSize, int outputSize, int numLayers, int neuronsPerLayer)
        {
            InitializeNetwork(inputSize, outputSize, numLayers, neuronsPerLayer);
        }

        public void InitializeNetwork(int inputSize, int outputSize, int numLayers, int neuronsPerLayer)
        {
            InputSize = inputSize;
            OutputSize = outputSize;
            NumLayers = numLayers;
            NeuronsPerLayer = neuronsPerLayer;

            // Clear any existing layers
            Layers.Clear();

            // Initialize layers and connect them
            InitializeLayers();
            ConnectLayers();
        }

        private void ConnectLayers()
        {
            for (int i = 0; i < Layers.Count - 1; i++)
            {
                Layers[i].NextLayer = Layers[i + 1];
                Layers[i + 1].PrevLayer = Layers[i];
            }
        }

        private void InitializeLayers()
        {
            Layers.Add(new Layer(InputSize, NeuronsPerLayer));

            for (int i = 1; i < NumLayers - 1; i++)
            {
                Layers.Add(new Layer(NeuronsPerLayer, NeuronsPerLayer));
            }

            Layers.Add(new Layer(NeuronsPerLayer, OutputSize));
        }

        public void Train(double[][] inputs, double[][] targets, int epochs, double learningRate = 0.1)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                double totalError = 0.0;

                for (int i = 0; i < inputs.Length; i++)
                {
                    // Forward pass
                    double[] output = Predict(inputs[i]);

                    // Compute error
                    double error = 0.0;
                    for (int j = 0; j < output.Length; j++)
                    {
                        error += Math.Pow(output[j] - targets[i][j], 2);
                    }
                    totalError += error;

                    // Backward pass
                    Layers[^1].Backward(targets[i], learningRate);
                }

                Console.WriteLine($"Epoch {epoch + 1}, Error: {totalError}");
            }
        }

        public double[] Predict(double[] input)
        {
            double[] outputs = input;
            foreach (var layer in Layers)
            {
                layer.Inputs = outputs;
                layer.Forward();
                outputs = layer.Outputs;
            }
            return outputs;
        }

        public double EvaluateAccuracy(double[][] testInputs, int[] testLabels)
        {
            int correctPredictions = 0;

            for (int i = 0; i < testInputs.Length; i++)
            {
                double[] prediction = Predict(testInputs[i]);
                int predictedLabel = Array.IndexOf(prediction, prediction.Max());

                if (predictedLabel == testLabels[i])
                {
                    correctPredictions++;
                }
            }

            return (double)correctPredictions / testInputs.Length;
        }

        public void SaveModel(string directory, int generation)
        {
            string fileName = $"{ModelName}_{generation}.txt";
            string filePath = Path.Combine(directory, fileName);

            using StreamWriter writer = new(filePath);

            writer.WriteLine(ModelName);
            writer.WriteLine(generation);

            foreach (var layer in Layers)
            {
                for (int i = 0; i < layer.Weights.GetLength(0); i++)
                {
                    writer.WriteLine(string.Join(",", layer.Weights.GetRow(i)));
                }
                writer.WriteLine(string.Join(",", layer.Biases));
            }
        }

        
            public static NeuralNetwork LoadModel(string filePath)
            {
                using StreamReader reader = new StreamReader(filePath);

                // Read the model name and generation (metadata)
                string modelName = reader.ReadLine();
                int generation = int.Parse(reader.ReadLine());

                // Read the input size, output size, number of layers, and neurons per layer
                int inputSize = int.Parse(reader.ReadLine());
                int outputSize = int.Parse(reader.ReadLine());
                int numLayers = int.Parse(reader.ReadLine());
                int neuronsPerLayer = int.Parse(reader.ReadLine());

                // Initialize the network with the sizes
                NeuralNetwork network = new NeuralNetwork(inputSize, outputSize, numLayers, neuronsPerLayer)
                {
                    ModelName = modelName
                };

                // Load weights and biases for each layer
                foreach (var layer in network.Layers)
                {
                    // Load weights
                    for (int i = 0; i < layer.Weights.GetLength(0); i++)
                    {
                        double[] weights = reader.ReadLine()
                            .Split(',')
                            .Select(double.Parse)
                            .ToArray();
                        layer.Weights.SetRow(i, weights);
                    }

                    // Load biases
                    double[] biases = reader.ReadLine()
                        .Split(',')
                        .Select(double.Parse)
                        .ToArray();
                    layer.Biases = biases;
                }

                return network;
            
        }

        public void PrintImage(double[] image, int width = 28, int height = 28)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double pixelValue = image[y * width + x];
                    char pixelChar = pixelValue > 0.8 ? '#' : pixelValue > 0.5 ? '+' : pixelValue > 0.2 ? '.' : ' ';
                    Console.Write(pixelChar);
                }
                Console.WriteLine();
            }
        }
    }
}
