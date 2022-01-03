using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHub.Models.Entities
{

    [Index(nameof(Name), IsUnique = true)]
    public class Setting
    {

        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Value { get; set; }

    }
}
