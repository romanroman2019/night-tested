using System;
using Models;
using System.Net.Http.Json;

namespace Client.Services
{
	public class DraftReadingSummaryService
	{
		private readonly HttpClient http;
		public List<DraftReading>? DraftSummaries;

		public DraftReadingSummaryService(HttpClient http)
		{
			ArgumentNullException.ThrowIfNull(http, nameof(http));
			this.http = http;
		}

		public async Task LoadDraftReadingSummaries()
		{
			if (DraftSummaries == null)
			{
				DraftSummaries = await
					http.GetFromJsonAsync<List<DraftReading>>(
						"api/draftreadings");
			}
		}

		public void Add(DraftReading draftReading)
		{
			ArgumentNullException.ThrowIfNull(draftReading, nameof(draftReading));
			if (DraftSummaries is null)
			{
				return;
			}

			if(!DraftSummaries.Any(summary =>
			summary.Id == draftReading.Id && summary.Port == draftReading.Port))
			{
				var summary = new DraftReading()
				{
					Id = draftReading.Id,
					Port = draftReading.Port,
					DatePublished = draftReading.DatePublished,
					Condition = draftReading.Condition
				};

				DraftSummaries.Add(summary);
			}
		}
	}
}

