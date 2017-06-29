using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System;

namespace TextRecognitionWebAppCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()                                     
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Index(List<IFormFile> files)
        {
            OcrResults text = null;
            try
            {
                var client = new VisionServiceClient("40752f6a71124d1cacac57d5227bdee6", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
                if (files[0].Length > 0)
                {
                    using (files[0].OpenReadStream())
                    {
                        text = await client.RecognizeTextAsync(files[0].OpenReadStream());
                    }
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
            ViewBag.PhotoText = textString;
            Console.WriteLine(textString);
            return View();
        }
    }
}
