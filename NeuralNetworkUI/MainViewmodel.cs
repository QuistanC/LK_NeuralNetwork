using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using LK_NeuronalNetwork;
using LK_NeuronalNetwork.Utilities;
using System.Windows.Media.Imaging;
using LK_NeuronalNetwork.Utilities.DataProcessHelper;
using LK_NeuronalNetwork.Utilities.ImageHelpers;

namespace NeuralNetworkUI
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private BitmapImage _originalImage;

        [ObservableProperty]
        private string _predictionResult;

        [ObservableProperty]
        private double _accuracy;

        [ObservableProperty]
        private double _error;

        [ObservableProperty]
        private string _selectedModel;

        private NeuralNetwork _currentNetwork;
        private string _selectedModelDirectory;
        private int _generation;

        public MainViewModel()
        {
            Models = new ObservableCollection<string>();
            _selectedModelDirectory = "Models";
            if (!Directory.Exists(_selectedModelDirectory))
                Directory.CreateDirectory(_selectedModelDirectory);

            TrainNetworkCommand = new RelayCommand(async () => await TrainNetwork());
            PredictCommand = new RelayCommand(Predict);
            InitializeModelCommand = new RelayCommand(InitializeNewModel);
            LoadModelCommand = new RelayCommand(LoadModel);

            LoadModels();
        }

        public ObservableCollection<string> Models { get; set; }

        public ICommand TrainNetworkCommand { get; }
        public ICommand PredictCommand { get; }
        public ICommand InitializeModelCommand { get; }
        public ICommand LoadModelCommand { get; }

        [RelayCommand]
        private void LoadModels()
        {
            Models.Clear();
            var modelFiles = Directory.GetFiles(_selectedModelDirectory, "*.txt");
            foreach (var file in modelFiles)
            {
                Models.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void InitializeNewModel()
        {
            // Default values for layers and neurons
            int inputSize = 784;
            int outputSize = 10;
            int numLayers = 2; // Default number of layers
            int neuronsPerLayer = 1; // Default number of neurons

            // You can allow the user to input these values as needed.
            _currentNetwork = new NeuralNetwork(inputSize, outputSize, numLayers, neuronsPerLayer)
            {
                ModelName = $"Model_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            _generation = 0;
            _currentNetwork.SaveModel(_selectedModelDirectory, _generation);

            PredictionResult = "New model initialized.";
        }

        private async Task TrainNetwork()
        {
            if (_currentNetwork == null)
            {
                PredictionResult = "Initialize or load a model first.";
                return;
            }

            // Load training data (dummy data in this case, replace with actual data)
            double[][] trainImages = GenerateTrainingData();
            double[][] trainLabels = GenerateTrainingLabels();

            // Train the network
            await Task.Run(() =>
            {
                _currentNetwork.Train(trainImages, trainLabels, epochs: 10);
            });

            // Increment generation
            _generation++;

            // Save the updated model
            _currentNetwork.SaveModel(_selectedModelDirectory, _generation);

            PredictionResult = $"Training completed. Generation: {_generation}";
        }

        private void Predict()
        {
            if (_currentNetwork == null)
            {
                PredictionResult = "Initialize or load a model first.";
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                double[] inputImage = ProcessImage(imagePath); // Implement image processing

                double[] output = _currentNetwork.Predict(inputImage);

                int predictedLabel = Array.IndexOf(output, output.Max());
                PredictionResult = $"Predicted Label: {predictedLabel}";

                // Calculate accuracy and error (dummy values for demonstration)
                Accuracy = 95.0;
                Error = 5.0;
            }
        }

        private void LoadModel()
        {
            if (string.IsNullOrEmpty(SelectedModel))
            {
                PredictionResult = "Select a model to load.";
                return;
            }

            string modelPath = Path.Combine(_selectedModelDirectory, $"{SelectedModel}.txt");
            _currentNetwork = NeuralNetwork.LoadModel(modelPath);
            PredictionResult = $"Model {SelectedModel} loaded.";
        }

        private double[][] GenerateTrainingData()
        {
            string trainingDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrainingData");

            string trainImagesPath = Path.Combine(trainingDataPath, "train-images.idx3-ubyte");

            double[][] trainImages = IdexFileReader.LoadImages(trainImagesPath);
            int originalWidth = 28, originalHeight = 28;
            int poolSize = 2, stride = 2;

            double[][] pooledTrainImages = trainImages
                .Select(image => Pooling.ApplyPooling(image, originalWidth, originalHeight, poolSize, stride))
                .ToArray();
            return pooledTrainImages;
        }
        private double[][] GenerateTrainingLabels()
        {
            string trainingDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrainingData");
            string trainLabelsPath = Path.Combine(trainingDataPath, "train-labels.idx1-ubyte");
            int[] trainLabels = IdexFileReader.LoadLabels(trainLabelsPath);
            double[][] trainLabelsOneHot = IdexFileReader.OneHotEncodeLabels(trainLabels, 10);

            return trainLabelsOneHot;
        }
        private double[] ProcessImage(string imagePath)
        {
            return null;
        }
    }
}
