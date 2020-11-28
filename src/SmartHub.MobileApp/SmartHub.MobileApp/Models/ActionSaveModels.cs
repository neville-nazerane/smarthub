using SmartHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHub.MobileApp.Models
{
    public interface IActionSaveModel
    {

    }

    public class DeviceActionSaveModel : IActionSaveModel
    {
        public DeviceActionSaveModel(DeviceAction data)
        {
            Data = data;
        }

        public DeviceAction Data { get; }
    }

    public class SceneActionSaveModel : IActionSaveModel
    {
        public SceneActionSaveModel(string sceneId)
        {
            SceneId = sceneId;
        }

        public string SceneId { get; }

    }

    public class ActionDeleteModel : IActionSaveModel 
    {

        public static readonly ActionDeleteModel Instance = new();

        private ActionDeleteModel()
        {

        }
    
    }

}
