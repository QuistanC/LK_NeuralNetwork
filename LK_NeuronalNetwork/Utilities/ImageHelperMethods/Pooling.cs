using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK_NeuronalNetwork.Utilities.ImageHelpers;

public class Pooling
{
    public double[,] ApplyMaxPooling(double[,] imageInput, int poolSize, int stride)
    {
        int inputHeight = imageInput.GetLength(0);
        int inputWidth = imageInput.GetLength(1);

        int outputHeight = (inputHeight - poolSize) / stride + 1;
        int outputWidth = (inputWidth - poolSize) / stride + 1;

        double[,] output = new double[outputHeight, outputWidth];

        for (int y = 0; y < outputHeight; y++)
        {
            for (int x = 0; x < outputWidth; x++)
            {
                double max = double.MinValue;

                for (int i = 0; i < poolSize; i++)
                {
                    for (int j = 0; j < poolSize; j++)
                    {
                        int inputY = y * stride + i;
                        int inputX = x * stride + j;

                        max = Math.Max(max, imageInput[inputY, inputX]);
                    }
                }

                output[y, x] = max;
            }
        }

        return output;
    }
}

