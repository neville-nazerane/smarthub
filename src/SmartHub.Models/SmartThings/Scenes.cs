using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.Models.SmartThings
{

    public class SceneData
    {
        public SceneItem[] Items { get; set; }
        //public _Links _links { get; set; }
    }

    //public class _Links
    //{
    //    public object next { get; set; }
    //    public object previous { get; set; }
    //}

    public class SceneItem
    {
        public string SceneId { get; set; }
        public string SceneName { get; set; }
        public string SceneIcon { get; set; }
        public object SceneColor { get; set; }
        public string LocationId { get; set; }
        public string CreatedBy { get; set; }
        public long CreatedDate { get; set; }
        public long LastUpdatedDate { get; set; }
        public long LastExecutedDate { get; set; }
        public bool Editable { get; set; }
        public string ApiVersion { get; set; }
    }

}
