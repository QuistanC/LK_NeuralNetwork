using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LK_NeuronalNetwork.Utilities.Extensions;

public static class ArrayExtension
{
    public static double[] GetRow(this double[,] array, int row)
    {
        int columns = array.GetLength(1);
        double[] rowValues = new double[columns];

        for (int i = 0; i < columns; i++)
        {
            rowValues[i] = array[row, i];
        }

        return rowValues;
    }

    public static void SetRow(this double[,] array, int row, double[] values)
    {
        int columns = array.GetLength(1);

        if (values.Length != columns)
            throw new ArgumentException("Length of values array must match the number of columns.");

        for (int i = 0; i < columns; i++)
        {
            array[row, i] = values[i];
        }
    }
}

