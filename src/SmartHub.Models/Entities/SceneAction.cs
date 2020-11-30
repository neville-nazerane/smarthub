using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartHub.Models.Entities
{
    public class SceneAction
    {

        public string Id { get; set; }

        [Required, MaxLength(90)]
        public string SceneId { get; set; }

    }
}
