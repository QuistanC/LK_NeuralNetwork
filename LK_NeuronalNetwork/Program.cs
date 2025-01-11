//Sigmoid-Funktion: y = (1/(1+(e^-x)))
//Summ x = aw.a + bw.b + cw.c
//ReLU function as activation function

using LK_NeuronalNetwork;
using LK_NeuronalNetwork.Utilities;
using LK_NeuronalNetwork.Utilities.DataProcessHelper;
using LK_NeuronalNetwork.Utilities.ImageHelpers;
using System.Drawing;
using System.Runtime.CompilerServices;

public class Program
{
    private static void Main(string[] args)
    {

        string trainingDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrainingData");

        string trainImagesPath = Path.Combine(trainingDataPath, "train-images.idx3-ubyte");
        string trainLabelsPath = Path.Combine(trainingDataPath, "train-labels.idx1-ubyte");
        string testImagesPath = Path.Combine(trainingDataPath, "t10k-images.idx3-ubyte");
        string testLabelsPath = Path.Combine(trainingDataPath, "t10k-labels.idx1-ubyte");

        double[][] trainImages = IdexFileReader.LoadImages(trainImagesPath);
        int[] trainLabels = IdexFileReader.LoadLabels(trainLabelsPath);

        double[][] testImages = IdexFileReader.LoadImages(testImagesPath);
        int[] testLabels = IdexFileReader.LoadLabels(testLabelsPath);

        double[][] trainLabelsOneHot = IdexFileReader.OneHotEncodeLabels(trainLabels, 10);
        double[][] testLabelsOneHot = IdexFileReader.OneHotEncodeLabels(testLabels, 10);


        int inputSize = 784;  // Input size for MNIST
        int outputSize = 10;  // Number of classes for MNIST
        int numLayers = 4;    // Total layers (input + hidden + output)
        int neuronsPerLayer = 64;

        int originalWidth = 28, originalHeight = 28;
        int poolSize = 2, stride = 2;

        double[][] pooledTrainImages = trainImages
            .Select(image => Pooling.ApplyPooling(image, originalWidth, originalHeight, poolSize, stride))
            .ToArray();

        int pooledWidth = (originalWidth - poolSize) / stride + 1;
        int pooledHeight = (originalHeight - poolSize) / stride + 1;
        int newInputSize = pooledWidth * pooledHeight;

        //NeuralNetwork network = new NeuralNetwork(inputSize, hiddennSizes, outputSize);
        NeuralNetwork network = new NeuralNetwork(inputSize, outputSize, numLayers, neuronsPerLayer);


        string modelPath = "model_weights.txt";
        if (File.Exists(modelPath))
        {
            network.LoadModel(modelPath);
            Console.WriteLine("Model weights loaded successfully.");
        }
        else
        {
            Console.WriteLine("No saved weights found, starting fresh training.");
        }

        int epochs = 5;
        double learningRate = 0.1;

        network.Train(trainImages, trainLabelsOneHot, epochs, learningRate);
        double[] prediction = network.Predict(testImages[0]);

        // Get the predicted label
        int predictedLabel = Array.IndexOf(prediction, prediction.Max());
        Console.WriteLine($"Predicted Label: {predictedLabel}");



        double accuracy = network.EvaluateAccuracy(testImages, testLabels);
        Console.WriteLine($"Test Accuracy: {accuracy * 100:F2}%");





        network.SaveModel("First Model", 1);


        // Train the network
        //TrainerMethod.Train(inputLayer, trainImages, trainLabelsOneHot, epochs: 10);

        //int predictedLabel = TrainerMethod.Predict(inputLayer, testInput);
        //Console.WriteLine($"Predicted Label: {predictedLabel}");


    }
}