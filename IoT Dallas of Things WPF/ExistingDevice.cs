using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Dallas_of_Things_WPF
{
    public class ExistingDevice
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public ExistingName[] name { get; set; }
        public string parentDeviceTemplateId { get; set; }
        public ExistingState state { get; set; }
        public ExistingAttributes attributes { get; set; }
        public ExistingObservableevent[] observableEvents { get; set; }
        public bool isActive { get; set; }
        public ExistingAuthentication authentication { get; set; }
    }

    public class ExistingState
    {
        public string lifecycleState { get; set; }
        public string operationalState { get; set; }
    }

    public class ExistingAttributes
    {
        public ExistingStandard[] standard { get; set; }
    }

    public class ExistingStandard
    {
        public ExistingAttributetype attributeType { get; set; }
        public object value { get; set; }
    }

    public class ExistingAttributetype
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public string name { get; set; }
        public ExistingDescription[] description { get; set; }
        public string type { get; set; }
        public bool isActive { get; set; }
    }

    public class ExistingDescription
    {
        public string lang { get; set; }
        public string text { get; set; }
    }

    public class ExistingAuthentication
    {
        public string authenticationType { get; set; }
        public bool isSystemGeneratedAuthnCredential { get; set; }
    }

    public class ExistingName
    {
        public string lang { get; set; }
        public string text { get; set; }
    }

    public class ExistingObservableevent
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public string name { get; set; }
        public ObservableeventDescription[] description { get; set; }
        public ExistingEventfield[] eventFields { get; set; }
        public bool isActive { get; set; }
        public bool requiresAck { get; set; }
    }

    public class ObservableeventDescription
    {
        public string lang { get; set; }
        public string text { get; set; }
    }

    public class ExistingEventfield
    {
        public string name { get; set; }
        public string type { get; set; }
        public string boundAttributeTypeId { get; set; }
    }

}
