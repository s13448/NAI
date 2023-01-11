//Tomasz Sosiński s16216 && Marcin Biedrzycki s13448
using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Simple_Face_Recognition_App
{
    public partial class Form1 : Form
    {
        #region Variables
        static string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Form1)).CodeBase).Replace(@"\bin\Debug", @"\Resource").Remove(0, 6);
        private Capture videoCapture = null;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        private bool facesDetectionEnabled = false;
        static string haarcascadePath = (path + @"\haarcascade_frontalface_alt_tree.xml");
        CascadeClassifier faceCasacdeClassifier = new CascadeClassifier(haarcascadePath);

        Stopwatch stopWatch = new Stopwatch();
    
        #endregion

        public Form1()
        {
            InitializeComponent();
            stopWatch.Start();
        }

        private async void btnCapture_Click(object sender, EventArgs e)
        {
            //Dispose of Capture if it was created before
            if (videoCapture != null) videoCapture.Dispose();
            videoCapture = new Capture();
            Application.Idle += ProcessFrame;
            
        }

        //private void ProcessFrame(object sender, EventArgs e)
        private void ProcessFrame(object sender, EventArgs e)
        {



            //Step 1: Video Capture
            if (videoCapture != null && videoCapture.Ptr != IntPtr.Zero)
            {
                videoCapture.Retrieve(frame, 0);
                currentFrame = frame.ToImage<Bgr, Byte>().Resize(picCapture.Width, picCapture.Height, Inter.Cubic);

                

                //Step 2: Face Detection
                if (facesDetectionEnabled)
                {

                    //Convert from Bgr to Gray Image
                    Mat grayImage = new Mat();
                    CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                    //Enhance the image to get better result
                    CvInvoke.EqualizeHist(grayImage, grayImage);

                    Rectangle[] faces = faceCasacdeClassifier.DetectMultiScale(grayImage, 1.1, 3, Size.Empty, Size.Empty);
                    //If faces detected
                    if (faces.Length > 0)
                    {
                        stopWatch.Restart();

                        foreach (var face in faces)
                        {
                            picDetected.Image = Image.FromFile(path + @"\reklama.jpg");
                        }
                    }

                    if (faces.Length == 0)
                    {
                        picDetected.Image = Image.FromFile(path + @"\stop.jpg");
                        if (stopWatch.Elapsed.TotalSeconds > 3)
                            picDetected.Image = Image.FromFile(path + @"\alert.jpg");

                    }
                }

                //Render the video capture into the Picture Box picCapture
                picCapture.Image = currentFrame.Bitmap;
            }

            //Dispose the Current Frame after processing it to reduce the memory consumption.
            if (currentFrame != null)
                currentFrame.Dispose();
        }

        private void btnDetectFaces_Click(object sender, EventArgs e)
        {
            facesDetectionEnabled = true;
        }

    }
}
