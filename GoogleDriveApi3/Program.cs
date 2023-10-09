using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.IO;
using System.Net;

namespace GoogleDriveApi3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("!111111111111");
            GetArtifact();
            Console.ReadLine();
        }

        public static async void GetArtifact()
        {
            string apiUrl = "https://dev.azure.com/SilhouetteResearch/Silhouette%20Suite/_apis/build/builds/1221/artifacts?artifactName=Silhouette%20Go%20Android&api-version=4.1";

            var httpClient = new HttpClient();

            // Set the Authorization header before making the request
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "Onp5ZGo2ZHU3bGh3aHpnbm8yd3ZmZGxsZWV6dm1pbzNiZndwZ2xpczN1bzduNDRmZTR2aWE=");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseBody);

                // Parse the JSON response and extract the user data
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

                Console.WriteLine($"AAAAAAAAAAAAAAA {jsonResponse}");

                await DownloadArtifact();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task DownloadArtifact()
        {
            Console.WriteLine($"BBBBBBBBBBBBBBBBBBBB");

            string downloadUrl = "https://artproduks1.artifacts.visualstudio.com/Ad6968979-1781-402c-937c-af71d258be14/d8c71310-2a98-4ad2-9121-caafe8bf03df/_apis/artifact/cGlwZWxpbmVhcnRpZmFjdDovL1NpbGhvdWV0dGVSZXNlYXJjaC9wcm9qZWN0SWQvZDhjNzEzMTAtMmE5OC00YWQyLTkxMjEtY2FhZmU4YmYwM2RmL2J1aWxkSWQvMTIyMS9hcnRpZmFjdE5hbWUvU2lsaG91ZXR0ZStHbytBbmRyb2lk0/content?format=zip";

            var httpClient = new HttpClient();

            // Set the Authorization header before making the request
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "Onp5ZGo2ZHU3bGh3aHpnbm8yd3ZmZGxsZWV6dm1pbzNiZndwZ2xpczN1bzduNDRmZTR2aWE=");


            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                Console.WriteLine($"333333333 {response}");

                // Get the response content as a stream
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    // Save the content to a file or process it as needed
                    using (var fileStream = File.Create("downloaded.zip")) // You can change the file name and path as needed
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                Console.WriteLine("Download completed.");

                OpenUrlInBrowser(downloadUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void OpenUrlInBrowser(string url)
        {
            Console.WriteLine("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");

            try
            {
                // Use Process.Start to open the URL in the default web browser
                Process.Start(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"XXXX Error: {ex.Message}");
            }
        }
    }
}
