using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Models;
using System.Text;
using System.Text.Json;

namespace Client.Services
{
	public class DraftReadingService
	{
		private readonly HttpClient http;
		private readonly NavigationManager navigationManager;
		private List<DraftReading> draftReadingCache = new();
		private readonly DraftReadingSummaryService draftReadingSummaryService;

		public DraftReadingService(
			HttpClient http,
			NavigationManager navigationManager,
			DraftReadingSummaryService draftReadingSummaryService)
		{
			ArgumentNullException.ThrowIfNull(http, nameof(http));
			ArgumentNullException.ThrowIfNull(navigationManager, nameof(navigationManager)); ArgumentNullException.ThrowIfNull(draftReadingSummaryService, nameof(draftReadingSummaryService));

			this.http = http;
			this.navigationManager = navigationManager;
			this.draftReadingSummaryService = draftReadingSummaryService;
		}

		public async Task<DraftReading?> GetDraftReading(Guid draftReadingId, string port)
		{
			DraftReading? draftReading = draftReadingCache.FirstOrDefault(dr =>
			dr.Id == draftReadingId && dr.Port == port);

			if (draftReading is null)
			{
				var result = await http.GetAsync($"api/draftreadings/{port}/{draftReadingId}");
				if (!result.IsSuccessStatusCode)
				{
					navigationManager.NavigateTo("404");
					return null;
				}

				draftReading = await result.Content.ReadFromJsonAsync<DraftReading>();
				if(draftReading is null)
				{
					navigationManager.NavigateTo("404");
					return null;
				}
				draftReadingCache.Add(draftReading);
			}
			return draftReading;
		}

		public async Task<DraftReading> Create(DraftReading draftReading)
		{
			ArgumentNullException.ThrowIfNull(draftReading, nameof(draftReading));

			var content = JsonSerializer.Serialize(draftReading);
			var data = new StringContent(content, Encoding.UTF8, "application/json");

			var result = await http.PostAsync("api/draftreadings", data);
			result.EnsureSuccessStatusCode();
			DraftReading? savedDraftReading = await result.Content.ReadFromJsonAsync<DraftReading>();

			draftReadingCache.Add(savedDraftReading!);
			draftReadingSummaryService.Add(savedDraftReading!);

			return savedDraftReading!;
			
		}
	}
}
