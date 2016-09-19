using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Web.Models.Requests
{
    public class EventsRequest
    {
        
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        //took required off description
        public string Description { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public int EventType { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        public int? MediaId { get; set; }

        public int[] Tags { get; set; }

        public LocationRequest Location { get; set; }

        public string ExternalEventId { get; set; }

    }
}
