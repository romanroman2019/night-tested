using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Models;

namespace Client.Services
{
	public class ShortDraftsService
	{
		private readonly HttpClient http;
        private readonly NavigationManager navigationManager;
        public List<ShortDraft>? Summaries;

		public ShortDraftsService(HttpClient http, NavigationManager navigationManager)
		{
			ArgumentNullException.ThrowIfNull(http, nameof(http));
            ArgumentNullException.ThrowIfNull(navigationManager, nameof(navigationManager));
			this.http = http;
            this.navigationManager = navigationManager;
		}

		public async Task LoadShortDrafts()
		{
			if (Summaries == null)
			{
				Summaries = await http.GetFromJsonAsync<List<ShortDraft>>("api/shortdrafts");
			}
		}

        public async Task<ShortDraft?> GetShortDraftNow(
    Guid shortDraftId, string author)
        {
            ShortDraft? shortDraft = new();
            var result = await http.GetAsync($"api/shortdrafts/{author}/{shortDraftId}");
            if (!result.IsSuccessStatusCode)
            {
                navigationManager.NavigateTo("404");
                return null;
            }

            shortDraft = await result.Content.ReadFromJsonAsync<ShortDraft>();
            if (shortDraft is null)
            {
                navigationManager.NavigateTo("404");
                return null;
            }

            return shortDraft;

        }

        public async Task Update(ShortDraft shortDraft)
        {
            ArgumentNullException.ThrowIfNull(shortDraft, nameof(shortDraft));

            var content = JsonSerializer.Serialize(shortDraft);
            var data = new StringContent(content, Encoding.UTF8, "application/json");

            var result = await http.PutAsync("api/shortdrafts", data);
            result.EnsureSuccessStatusCode();

        }

        public async Task<ShortDraft> Create(ShortDraft shortDraft)
        {
            ArgumentNullException.ThrowIfNull(shortDraft,nameof(shortDraft));

            var content = JsonSerializer.Serialize(shortDraft);
            var data = new StringContent(content, Encoding.UTF8,"application/json");
            
            var result = await http.PostAsync("api/shortdrafts", data);
            result.EnsureSuccessStatusCode();
            ShortDraft? savedShortDraft = await result.Content.ReadFromJsonAsync<ShortDraft>();

            return savedShortDraft!;
        }
    }
}

