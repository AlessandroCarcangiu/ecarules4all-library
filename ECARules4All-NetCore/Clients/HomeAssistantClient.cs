using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UnityEngine;


namespace ECARules4AllPack.Clients
{
    public class HomeAssistantClient : AbstractClient<HomeAssistantClient>
    {
        protected override async void sendRequestHandler(object sender, Update newItem)
        {
            Debug.Log("Sto mandando un update");
            if (string.IsNullOrEmpty(this.token) || string.IsNullOrEmpty(this.url))
            {
                Debug.Log("Token or url is empty");
            }

            //var lastContent = this.updates.GetUpdates();
            //var lastContent = new List<Update> {this.updates.Dequeue()};
            var lastContent = this.updates.Dequeue();
            string jsonBody = JsonSerializer.Serialize(new { update = lastContent });

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    string urlService = $"{this.url}/api/services/eud4xr/receive_update_from_unity";
                    Debug.Log($"Il contenuto che manderò è questo: {content}");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", this.token);
                    HttpResponseMessage response = await client.PostAsync(urlService, content);
                    response.EnsureSuccessStatusCode();
                    
                    Debug.Log($"Sent an update to Home Assistant Client at {this.url}");
                    // delete list content
                    //this.updates.Remove(lastContent);
                }
                catch (HttpRequestException e)
                {
                    Debug.Log($"An error occured during an update request - {e.Message}");
                }
            }
            Debug.Log("Ho mandando un update");
        }
    }
}
