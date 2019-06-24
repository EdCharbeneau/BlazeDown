using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazeDown.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }
        protected string FileUrl { get; set; }
        protected string ContentValue { get; set; }

        protected void TextChanged(UIChangeEventArgs e)
        {
            ContentValue = e.Value.ToString();
        }

        protected async override Task OnInitAsync()
        {
            ContentValue = await GetContentFromUrl("/sample-data/example.md");
        }

        protected async void OnImportClicked()
        {
            string path = String.IsNullOrWhiteSpace(FileUrl) ? "/sample-data/example.md" : FileUrl;
            ContentValue = await GetContentFromUrl(path);
            StateHasChanged();
        }

        private async Task<string> GetContentFromUrl(string path)
        {
            HttpResponseMessage httpResponse = await Http.GetAsync(path);
            return httpResponse.IsSuccessStatusCode ?
            await httpResponse.Content.ReadAsStringAsync() : httpResponse.ReasonPhrase;
        }
    }
}
