using System;
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
        private bool showingCamera = false;
        private CameraCaptureTask cameraCaptureTask;
        private PhotoChooserTask photoChooserTask;
        private const double MaxPictureSizeDiag = 600;

        public PhotoSelector()
        {
            this.cameraCaptureTask = new CameraCaptureTask();
            this.photoChooserTask = new PhotoChooserTask();

            this.cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(this.PhotoChooserCompleted);
            this.photoChooserTask.Completed += new System.EventHandler<PhotoResult>(this.PhotoChooserCompleted);
        }

        private void TakePicture_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            // Prevents re-opening of the camera
            if (this.showingCamera) return;
            this.showingCamera = true;
            this.cameraCaptureTask.Show();
        }

        private void OpenPicture_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            // Prevents opening more than one method of photo selection
            if (this.showingCamera) return;
            this.showingCamera = true;
            this.photoChooserTask.Show();
        }

        private void PhotoChooserCompleted(object sender, PhotoResult e)
        {
            this.showingCamera = false;
            if (!(e.TaskResult = TaskResult.OK)) return;
            this.ShowPicture(e.ChosenPhoto);
            this.StartOcrConversion(e.ChosenPhoto);
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
                    Extensions.SaveJpeg(wb, tempStream, newWidth, newHeight, 0, 100);
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
    }
}
