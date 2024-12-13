using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LK_NeuronalNetwork;

namespace NeuralNetworkUI;

public partial class MainViewmodel : ObservableObject
{
    [ObservableProperty]
    private BitmapImage _originalImage;

    [ObservableProperty]
    private Bitmap _image;

    



    [RelayCommand]
    public void OpenDialog()
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".jpg";

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(dialog.FileName));
            OriginalImage = bitmapImage;
            Image = new Bitmap(dialog.FileName);

            ConvertImage();

        }
    }

    private void ConvertImage()
    {
        byte[,,] imageArr = LK_NeuronalNetwork.Utilities.ImageHelpers.ImageConverter.ImageToArray(Image) ?? new byte[,,] { };
    }

}
