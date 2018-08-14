using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Gms.Vision.Detector;

namespace WhisperApp
{
    [Activity(Label = "Whisper", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class QRCodeScannerActivity : AppCompatActivity, ISurfaceHolderCallback,IProcessor
    {
        SurfaceView cameraPreview;
        TextView txtResult;
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        const int RequestCameraPermisionID = 1001;
        Button qrgenerator; 


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermisionID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                            {
                                ActivityCompat.RequestPermissions(this, new string[]
                                    {
                                        Manifest.Permission.Camera
                                    }, RequestCameraPermisionID);
                                return;
                            }
                            try
                            {
                                cameraSource.Start(cameraPreview.Holder);
                            }
                            catch (InvalidOperationException)
                            {

                            }
                        }
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.QRCodeScanner);

            cameraPreview = FindViewById<SurfaceView>(Resource.Id.cameraPreview);
            txtResult = FindViewById<TextView>(Resource.Id.cameraPreviewtetxtView);
            qrgenerator = FindViewById<Button>(Resource.Id.qrgeneratorbutton);

            barcodeDetector = new BarcodeDetector.Builder(this).SetBarcodeFormats(BarcodeFormat.QrCode).Build();
            cameraSource = new CameraSource.Builder(this, barcodeDetector).SetRequestedPreviewSize(700, 500).Build();

            cameraPreview.Holder.AddCallback(this);
            barcodeDetector.SetProcessor(this);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                    {
                        Manifest.Permission.Camera
                    }, RequestCameraPermisionID);
                return;
            }
            try
            {
                cameraSource.Start(cameraPreview.Holder);
            }
            catch (InvalidOperationException)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if(qrcodes.Size()!=0)
            {
                txtResult.Post(() =>
                {
                    Vibrator vib = (Vibrator)GetSystemService(Context.VibratorService);
                    vib.Vibrate(1000);
                    var qrtext = ((Barcode)qrcodes.ValueAt(0)).RawValue;

                    var username = qrtext.Split(',')[0].Substring(10);
                    var key = qrtext.Split(',')[1].Substring(6);

                         var keys = KeyGenerator();
                         SaveContact(username, key,keys["privateKey"]);

                    cameraPreview.Visibility = ViewStates.Invisible;
                    txtResult.Text = string.Format("Contact {0} saved!!", username);
                });
            }
        }

        public void Release()
        {
            
        }

        public void SaveContact(string username, string key, string privatekey)
        {
            var prefs = Application.Context.GetSharedPreferences("Contacts", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            var count = prefs.GetInt("count", 0);
            prefEditor.PutInt("count", ++count);
            prefEditor.PutString("username"+count, username);
            prefEditor.PutString("contactpublickey"+count, key);
            prefEditor.PutString("myprivatekey" + count, privatekey);
            prefEditor.Commit();
        }

        public Dictionary<string,string> KeyGenerator()
        {
            RSACryptoServiceProvider rSA = new RSACryptoServiceProvider();
            var privateKey = rSA.ToXmlString(true);
            var publicKey = rSA.ToXmlString(false);

            var privatekey = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey));
            var publickey = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey));

            var keys = new Dictionary<string, string>();

            keys.Add("publicKey", publickey);
            keys.Add("privateKey", privatekey);

            return keys;
        }

    }
}