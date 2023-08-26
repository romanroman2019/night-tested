using System;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Models;

namespace Client.Services
{
	public class ShortService
	{
		private readonly HttpClient http;
		private readonly NavigationManager navigationManager;

		public ShortService(HttpClient http, NavigationManager navigationManager)
		{
			ArgumentNullException.ThrowIfNull(http, nameof(http));
			ArgumentNullException.ThrowIfNull(navigationManager, nameof(navigationManager));

			this.http = http;
			this.navigationManager = navigationManager;
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
	}
}

