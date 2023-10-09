using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text.RegularExpressions;
using static Google.Apis.Auth.OAuth2.AccessTokenWithHeaders;
using System.Net;

namespace GoogleDriveApi3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetArtifact();
            Console.ReadLine();
        }

        public static async void GetArtifact()
        {
            string buildId = "1221";
            string groupName = "SilhouetteResearch";
            string projectName = "Silhouette%20Suite";
            string dirName = "Silhouette%20Go%20Android";
            string artifactDownloadName = "";

            string apiUrl = $"https://dev.azure.com/{groupName}/{projectName}/_apis/build/builds/{buildId}/artifacts?artifactName={dirName}&api-version=4.1";

            // Get name of project
            // Define a regular expression pattern to match the artifactName parameter
            string pattern = @"[?&]artifactName=([^&]+)";

            // Use Regex.Match to find the parameter in the URL
            Match match = Regex.Match(apiUrl, pattern);

            if (match.Success)
            {
                // The captured group at index 1 contains the artifactName value
                string artifactName = match.Groups[1].Value;

                Console.WriteLine("Artifact Name: " + Uri.UnescapeDataString(artifactName));
                artifactName = Uri.UnescapeDataString(artifactName.Replace("%20", " "));

                Console.WriteLine($"AAAAAAAAA {artifactName}");

                artifactDownloadName = artifactName;
            }
            else
            {
                Console.WriteLine("Artifact Name not found in the URL.");
            }

            var httpClient = new HttpClient();

            // Set the Authorization header before making the request
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "Onp5ZGo2ZHU3bGh3aHpnbm8yd3ZmZGxsZWV6dm1pbzNiZndwZ2xpczN1bzduNDRmZTR2aWE=");

            try
            {
                Console.WriteLine("Starting download...");

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseBody);

                // Parse the JSON response and extract the user data
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

                await DownloadArtifact(artifactDownloadName, buildId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task DownloadArtifact(string artifactDownloadName, string buildId)
        {
            string downloadUrl = $"https://artproduks1.artifacts.visualstudio.com/Ad6968979-1781-402c-937c-af71d258be14/d8c71310-2a98-4ad2-9121-caafe8bf03df/_apis/artifact/cGlwZWxpbmVhcnRpZmFjdDovL1NpbGhvdWV0dGVSZXNlYXJjaC9wcm9qZWN0SWQvZDhjNzEzMTAtMmE5OC00YWQyLTkxMjEtY2FhZmU4YmYwM2RmL2J1aWxkSWQvMTIyMS9hcnRpZmFjdE5hbWUvU2lsaG91ZXR0ZStHbytBbmRyb2lk0/content?format=zip";

            var httpClient = new HttpClient();

            //string projectGuid = "Ad6968979-1781-402c-937c-af71d258be14";
            //string artifactId = "d8c71310-2a98-4ad2-9121-caafe8bf03df";
            //string contentName = "YourContentName"; 

            //string downloadUrl2 = $"https://artproduks1.artifacts.visualstudio.com/{projectGuid}/{artifactId}/_apis/artifact/cGlwZWxpbmVhcnRpZmFjdDovL1NpbGhvdWV0dGVSZXNlYXJjaC9wcm9qZWN0SWQvZDhjNzEzMTAtMmE5OC00YWQyLTkxMjEtY2FhZmU4YmYwM2RmL2J1aWxkSWQv1221/arc/content?format=zip";


            // Set the Authorization header before making the request
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "Onp5ZGo2ZHU3bGh3aHpnbm8yd3ZmZGxsZWV6dm1pbzNiZndwZ2xpczN1bzduNDRmZTR2aWE=");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                // Get the response content as a stream
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    // Save the content to a file or process it as needed
                    using (var fileStream = File.Create($"{artifactDownloadName} {buildId}.zip")) // You can change the file name and path as needed
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                OpenUrlInBrowser(downloadUrl, artifactDownloadName, buildId);

                Console.WriteLine("Download completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading artifact: {ex.Message}");
            }
        }

        public static void OpenUrlInBrowser(string url, string artifactDownloadName, string buildId)
        {
            try
            {
                Console.WriteLine("Upload starting...");

                // Use Process.Start to open the URL in the default web browser
                Process.Start(url);
                // CreateNewGoogleDriveFolder(artifactDownloadName, buildId);
                UploadFilesToGoogleDrive(artifactDownloadName, buildId);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"XXXX Error: {ex.Message}");
            }
        }

        public static void CreateNewGoogleDriveFolder(string artifactDownloadName, string buildId)
        {
            Console.WriteLine("1111111111111111111111111111");
            GoogleCredential credential = GoogleCredential.GetApplicationDefault()
                    .CreateScoped(DriveService.Scope.Drive);

            try
            {
                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Snippets"
                });

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "Invoices",
                    MimeType = "application/vnd.google-apps.folder"
                };

                // Create a new folder on drive.
                var request = service.Files.Create(fileMetadata);
                request.Fields = "id";
                var file = request.Execute();

                // Prints the created folder id.
                Console.WriteLine("Folder ID: " + file.Id);

                Console.WriteLine("222222222222222222222222222222");

                UploadFilesToGoogleDrive(artifactDownloadName, buildId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YYYYY Error: {ex.Message}");
            }

        }
        static void UploadFilesToGoogleDrive(string artifactDownloadName, string buildId)
        {
            Console.WriteLine("33333333333333333333333333333");
            string credentialsPath = "credentials.json";
            string folderId = "1Ro77nizxMbe_5RfsyLCPsIviHMJXiDd6";

            string[] filePaths = { $"{artifactDownloadName} {buildId}.zip" };

            GoogleCredential credential;

            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                {
                    DriveService.ScopeConstants.DriveFile
                });
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google DRIVE upload app"
            });

            foreach (var filePath in filePaths)
            {
                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(filePath),
                    Parents = new List<string> { folderId }
                };

                FilesResource.CreateMediaUpload request;

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetaData, stream, "");
                    request.Fields = "id";
                    request.Upload();
                }

                var uploadedFile = request.ResponseBody;

                Console.WriteLine($"FILE: {fileMetaData.Name} - uploaded with {uploadedFile.Id}");
            }
            Console.WriteLine("Upload Completed");

            Console.WriteLine("Program End...");
        }
    }
}
