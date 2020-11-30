using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHub.Models.Entities
{

    [Index(nameof(EventId), IsUnique = false)]
    public class EventLog
    {

        public int Id { get; set; }

        [Required, MaxLength(90)]
        public string EventId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

    }
}
