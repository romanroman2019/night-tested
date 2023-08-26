using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models
{
	public class ShortDraft
	{
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string Port { get; set; }
        public string Author { get; set; }
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
        public Condition Condition { get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]
        public decimal ObservedFwd { get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]        
        public decimal ObservedAft { get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]
        public decimal LoadmasterFwd { get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]        
        public decimal LoadmasterAft{ get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]
        public decimal SensorFwd{ get; set; }
        [Required, Range(4.0, 10.0, ErrorMessage = "Draft must be between 4 and 10 meters")]        
        public decimal SensorAft { get; set; }                
        public Status Status { get; set; }

    }

    public enum Status
    {
        Draft = 1,
        Published = 2,
        Deleted = 3
    }

    public enum Condition{
        Loaded,
        Ballast,
        Other
    }
}

