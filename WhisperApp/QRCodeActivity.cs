using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class QRCodeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.QRCode);

            var qrcode = FindViewById<ImageView>(Resource.Id.imageView1);

            var qrtext = Intent.GetStringExtra("qrtext");

            qrcode.SetImageBitmap(GenerateQR(qrtext));
        }

        public Bitmap GenerateQR(string text)
        {
            var bw = new ZXing.BarcodeWriter();
            var encOptions = new ZXing.Common.EncodingOptions() { Width = 400, Height = 400, Margin = 0 };
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            var result = Bitmap.CreateBitmap(bw.Write(text));

            return result;
        }
    }
}