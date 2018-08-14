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
using WhisperApp.Resources;

namespace WhisperApp
{
    [Activity(Label = "Whisper")]
    public class ContactListActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ContactList);

            List<Contact> lstSource = new List<Contact>();
            for (int i = 0; i < 5; i++)
            {
                var cont = new Contact() { Username = "contact " + i };
                lstSource.Add(cont);

            }

            var contactList = FindViewById<ListView>(Resource.Id.contactListView);

            var adapter = new CustomAdapter(this, lstSource);
            contactList.Adapter = adapter;
        }
    }
}