using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHub.Models.Entities
{
    public class SceneState
    {

        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string SceneName { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

    }
}
