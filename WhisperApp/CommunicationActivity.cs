using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using WhisperApp.Resources;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class CommunicationActivity : Activity
    {
        TextView message;

        TextView mymessage;

        Button send;

        EditText enteredmessage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.communication);

            //Android.Media.AudioSource.VoiceCall

            message = FindViewById<TextView>(Resource.Id.textView2);

            mymessage = FindViewById<TextView>(Resource.Id.textView3);

            send = FindViewById<Button>(Resource.Id.button1);

            enteredmessage = FindViewById<EditText>(Resource.Id.editText1);



            send.Click += delegate
            {
                if (enteredmessage.Text != "")
                {
                    mymessage.Text = enteredmessage.Text;

                }
            };


            mymessage.TextChanged += delegate
             {

                 Thread.Sleep(2000);
                 message.Text = mymessage.Text;
             };
        }

        
    }
}