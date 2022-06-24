using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static SpotifyAPI.Web.Scopes;

namespace SpotifyLibraryManager.Services
{
    public static class Spotify
    {
        private const string CallbackUri = "http://localhost:5000/callback";
        private const string ClientId = "bfeb37c849e1488eacc3a4a8cbb8e224";
        private static readonly string TokensPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SpotifyLibraryManager\tokens.json";

        public static SpotifyClient? Client { get; private set; }

        public static async Task<bool> CheckClient()
        {
            // Check if there're saved tokens
            if (File.Exists(TokensPath))
            {
                // Use them to create client
                await CreateClient();
                return true;
            }
            else
            {
                // Don't do anything, let user click on login button
                return false;
            }
        }

        private static async Task CreateClient()
        {
            // Read tokens from file and make it into object
            var json = await File.ReadAllTextAsync(TokensPath);
            var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

            // Create an authenticator that automatically refreshes the token when it expires
            var authenticator = new PKCEAuthenticator(ClientId!, token!);
            // Save tokens every time they're refeshed
            authenticator.TokenRefreshed += (sender, token) => File.WriteAllText(TokensPath, JsonConvert.SerializeObject(token));

            // Create Spotify client
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(authenticator);

            Client = new SpotifyClient(config);
        }

        public static async Task StartAuthentication()
        {
            //Generate PKCE secret
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            // Create request to get authorization code
            var loginRequest = new LoginRequest(
                new Uri(CallbackUri),
                ClientId!,
                LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",
                Scope = new List<string> { UserLibraryRead }
            };
            string uri = loginRequest.ToUri().ToString(); // Request URI

            // Create HttpListener for handling callback and collecting the code
            var http = new HttpListener();
            http.Prefixes.Add(CallbackUri + "/"); // prefixes must end with '/'
            http.Start();

            // Open web browser and navigate to request URI
            Process browser = new Process();
            browser.StartInfo.UseShellExecute = true;
            browser.StartInfo.FileName = uri;
            browser.Start();

            // Wait for callback/redirect
            var context = await http.GetContextAsync();

            // Sends an response to the browser
            var response = context.Response;
            string responseString = "Authentication successful! You can close this page.";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            http.Stop();

            // Collect Authorization code
            string code = context.Request.QueryString.Get("code");

            // Obtain access token
            PKCETokenResponse token = await new OAuthClient().RequestToken(
                new PKCETokenRequest(ClientId!, code, new Uri(CallbackUri), verifier)
            );

            // Save tokens
            await File.WriteAllTextAsync(TokensPath, JsonConvert.SerializeObject(token));

            // Create client using access token
            await CreateClient();
        }

    }
}
