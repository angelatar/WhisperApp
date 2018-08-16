using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
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

            List<Contact> lstSource = GetContacts();
          
            var contactList = FindViewById<ListView>(Resource.Id.contactListView);

            if (lstSource != null)
            {
                var adapter = new CustomAdapter(this, lstSource);
                contactList.Adapter = adapter;
            }
        }

        protected List<Contact> GetContacts()
        {
            var prefs = Application.Context.GetSharedPreferences("Contacts", FileCreationMode.Private);
            var count = prefs.GetInt("Count", 0);
            if (count == 0)
                return null;
            else
            {
                var contacsts = new List<Contact>();
                for (int i = 1; i <= count; i++)
                {
                    var username = prefs.GetString(string.Format("cont{0}", i), null);
                    var publickey = prefs.GetString(string.Format("cont{0} public key", i), null);
                    var myprivatekey = prefs.GetString(string.Format("my private key with cont{0}", i), null);

                    var contact = new Contact()
                    {
                        Username = username,
                        PublicKey = publickey,
                        MyPrivateKey = myprivatekey
                    };

                    contacsts.Add(contact);
                }
                return contacsts;
            }
        }
    }
}