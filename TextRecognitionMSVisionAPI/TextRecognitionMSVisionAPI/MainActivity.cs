using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ProjectOxford.Vision;
using System.Threading.Tasks;
using System;

namespace TextRecognitionMSVisionAPI
{
    [Activity(Label = "TextRecognitionMSVisionAPI", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView textView;
        ImageView imageView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button button = FindViewById<Button>(Resource.Id.myButton);
            textView = FindViewById<TextView>(Resource.Id.myTextView);
            button.Click += delegate {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                imageView = FindViewById<ImageView>(Resource.Id.myImageView);
                imageView.SetImageURI(data.Data);
                RunOnUiThread(async () => {
                    textView.Text = await ProcessImageForTextRecognitionAsync(data);
                });
            }
        }

        private async Task<string> ProcessImageForTextRecognitionAsync(Intent data)
        {
            OcrResults text;
            text = null;
            try
            {
                var client = new VisionServiceClient("40752f6a71124d1cacac57d5227bdee6", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0"); // This is my microsoft vision api key. You need to replace it with your own :)

                using (var photoStream = ContentResolver.OpenInputStream(data.Data))
                {
                    text = await client.RecognizeTextAsync(photoStream);
                }
            }
            catch(Exception ex)
            {

            }
            var textString = "";
            foreach (var region in text.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        textString += " " + word.Text;
                    }
                }
            }

            return textString;
        }
    }
}

