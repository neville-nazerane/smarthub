namespace SmartHub.WebApp.Models
{

    public class DeviceData
    {
        public DeviceItem[] Items { get; set; }
    }

    public class DeviceItem
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ManufacturerName { get; set; }
        public string PresentationId { get; set; }
        public string LocationId { get; set; }
        public string RoomId { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceTypeName { get; set; }
        public string DeviceNetworkType { get; set; }
        public DeviceComponent[] Components { get; set; }
        public DeviceDth Dth { get; set; }
        public string Type { get; set; }
        public int RestrictionTier { get; set; }
        public DeviceProfile Profile { get; set; }
        public DeviceViper Viper { get; set; }
        public string OwnerId { get; set; }
        public DeviceChildDevice[] ChildDevices { get; set; }
        public string DeviceManufacturerCode { get; set; }
    }

    public class DeviceDth
    {
        public string DeviceTypeId { get; set; }
        public string DeviceTypeName { get; set; }
        public string DeviceNetworkType { get; set; }
        public bool CompletedSetup { get; set; }
        public string NetworkSecurityLevel { get; set; }
        public string HubId { get; set; }
    }

    public class DeviceProfile
    {
        public string Id { get; set; }
    }

    public class DeviceViper
    {
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public string HwVersion { get; set; }
    }

    public class DeviceComponent
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public DeviceCapability[] Capabilities { get; set; }
        public DeviceCategory[] Categories { get; set; }
    }

    public class DeviceCapability
    {
        public string Id { get; set; }
        public int Version { get; set; }
    }

    public class DeviceCategory
    {
        public string Name { get; set; }
    }

    public class DeviceChildDevice
    {
        public string DeviceId { get; set; }
    }

}
