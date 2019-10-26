using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BullshitChallenge.Models;
using System.Net.Http;
using System.Text;

namespace BullshitChallenge.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                BullshitViewModel model = new BullshitViewModel();
                Bullshits bullshits = null;

                using (var client = new HttpClient())
                {
                    // Get the bullshit from the API (not supersecret)
                    var requestUri = new Uri("https://koodihaaste-api.solidabis.com/bullshit");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJidWxsc2hpdCI6ImJ1bGxzaGl0IiwiaWF0IjoxNTcyMDEyNDA2fQ.wA3gO5Ky3LYBSVr76-9OsUArVA4tycw9TUrZWYEXwDk");
                    var response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        bullshits = await response.Content.ReadAsAsync<Bullshits>();
                    }
                }

                if (bullshits == null)
                {
                    throw new NullReferenceException("Was not able to get the bullshit.");
                }

                model = DecipherTheBullshit(bullshits, model);
                foreach (var message in bullshits.Messages)
                {
                    model.Bullshit.Add(message.OneMessage);
                }

                return View(model);
            }
            catch (Exception e)
            {
                ViewBag.Message = "Bad thing happened, sorry! Please try again soon!";
                ViewBag.Exception = $"Here is the error: { e.Message}";
            }

            return View();
        }

        private BullshitViewModel DecipherTheBullshit(Bullshits bullshits, BullshitViewModel model)
        {
            // Magic words which determine if the message is bullshit or not. Might be a slightly limited dictionary.
            List<string> keywords = new List<string> { " on ", " ja ", " otti ", " olla ", " ollut ", "vanha", " voi ", " ovat ", " mutta ", " auton ", " tai ", "lääkäri" };

            var alphabet = "abcdefghijklmnopqrstuvwxyzåäö";

            foreach (var message in bullshits.Messages)
            {
                var deciphered = false;

                // Loop through all possible keys and check if the deciphered message makes sence (contains the keywords listed above.)
                for (int key = 0; key < alphabet.Length; key++)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (char character in message.OneMessage)
                    {
                        // Return spaces as spaces and dots as dots
                        if (!alphabet.Contains(char.ToLower(character)))
                        {
                            builder.Append(character);
                            continue;
                        }

                        // Get the index of the current letter and add the key to it.
                        int index = alphabet.IndexOf(char.ToLower(character));

                        // In case out of range 
                        if (index + key >= alphabet.Length)
                        {
                            index = index - alphabet.Length;
                        }

                        builder.Append(alphabet[index + key]);
                    }

                    var solved = builder.ToString();

                    if (keywords.Any(word => solved.Contains(word)) && !solved.Contains("x"))
                    {
                        // Make solved be a real sentence by making it begin with a capital letter (nitpicking)
                        StringBuilder capitalbuilder = GetCapsSentence(builder);
                        solved = capitalbuilder.ToString();
                        model.NotBullshit.Add(solved);
                        deciphered = true;
                        break;
                    }
                }

                // If unable to decipher the message add the original to list of bullshit.
                if (!deciphered)
                {
                    model.Bullshit.Add(message.OneMessage);
                }

            }

            return model;
        }

        private StringBuilder GetCapsSentence(StringBuilder builder)
        {
            var builderWithCaps = new StringBuilder();
            for (int ind = 0; ind < builder.Length; ind++)
            {
                if (ind == 0)
                {
                    builderWithCaps.Append(char.ToUpper(builder[ind]));
                }
                else
                {
                    builderWithCaps.Append(builder[ind]);
                }
            }

            return builderWithCaps;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
