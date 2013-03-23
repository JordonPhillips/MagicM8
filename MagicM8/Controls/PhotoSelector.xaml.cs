using System;
using System.Text;
using System.Windows.Controls;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Ocr.Client;

namespace MagicM8.Controls
{
    public partial class PhotoSelector : UserControl
    {
        private bool showingCamera;
        private CameraCaptureTask cameraCaptureTask;
        private PhotoChooserTask photoChooserTask;
        private const double MaxPictureSizeDiag = 600;

        public PhotoSelector()
        {
            InitializeComponent();

            cameraCaptureTask = new CameraCaptureTask();
            photoChooserTask = new PhotoChooserTask();

            cameraCaptureTask.Completed += PhotoChooserCompleted;
            photoChooserTask.Completed += PhotoChooserCompleted;
        }

        private void TakePicture_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            // Prevents re-opening of the camera
            if (showingCamera) return;
            showingCamera = true;
            cameraCaptureTask.Show();
        }

        private void OpenPicture_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            // Prevents opening more than one method of photo selection
            if (showingCamera) return;
            showingCamera = true;
            photoChooserTask.Show();
        }

        private void PhotoChooserCompleted(object sender, PhotoResult e)
        {
            showingCamera = false;
            if (e.TaskResult != TaskResult.OK) return;
            ShowPicture(e.ChosenPhoto);
            StartOcrConversion(e.ChosenPhoto);
        }

        private static byte[] StreamToByteArray(Stream stream)
        {
            var buffer = new byte[stream.Length];

            var seekPosition = stream.Seek(0, SeekOrigin.Begin);
            var bytesRead = stream.Read(buffer, 0, buffer.Length);
            seekPosition = stream.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

        /// <summary>
        /// Hawaii limits the size of the image you can submit, so this checks to see if the given stream is valid.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="maxPictureSizeDiag"></param>
        /// <returns>Either a resized stream, or the original stream if it's the correct size.</returns>
        private static Stream ScaleImage(Stream imageStream, double maxPictureSizeDiag)
        {
            // Place image in a writeable bmp to determine its size
            var wb = new WriteableBitmap(1, 1);
            wb.SetSource(imageStream);

            // Is a scale down needed
            var imgDiag = Math.Sqrt(wb.PixelHeight*wb.PixelHeight + wb.PixelWidth*wb.PixelWidth);
            if (imgDiag > maxPictureSizeDiag)
            {
                var newWidth = (int) (wb.PixelWidth*maxPictureSizeDiag/imgDiag);
                var newHeight = (int) (wb.PixelHeight*maxPictureSizeDiag/imgDiag);

                Stream resizedStream = null;
                Stream tempStream = null;

                try
                {
                    tempStream = new MemoryStream();
                    wb.SaveJpeg(tempStream, newWidth, newHeight, 0, 100);
                    resizedStream = tempStream;
                    tempStream = null;
                }
                finally
                {
                    if (tempStream != null)
                    {
                        tempStream.Close();
                        tempStream = null;
                    }
                }

                return resizedStream;
            }
            else
            {
                // Returns the original stream if you don't need to scale down
                return imageStream;
            }
        }

        private void ShowPicture(Stream pictureStream)
        {


        }

        private void StartOcrConversion(Stream pictureStream)
        {
            pictureStream = ScaleImage(pictureStream, MaxPictureSizeDiag);
            var buf = StreamToByteArray(pictureStream);

            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                buf,
                output => Dispatcher.BeginInvoke(() => OnOcrCompleted(output)));
        }

        private void OnOcrCompleted(OcrServiceResult result)
        {
            if (result.Status == Status.Success)
            {
                var count = 0;
                var sb = new StringBuilder();
                foreach (var item in result.OcrResult.OcrTexts)
                {
                    count += item.Words.Count;
                    sb.Append(item.Text);
                    sb.Append("\n");
                }

                if (count == 0)
                {
                    // TODO: display empty result message
                }
                else
                {
                    // TODO: show result text
                }
            }
            else
            {
                // TODO: show error message indication conversion failure
            }
        }

        private void HideAll()
        {
            // TODO: hide all ui components
        }
    }
}
