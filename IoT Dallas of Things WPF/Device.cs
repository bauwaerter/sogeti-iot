using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT_Dallas_of_Things_WPF
{
    public class Device
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public Name[] name { get; set; }
        public string parentDeviceTemplateId { get; set; }
        public State state { get; set; }
        public Attributes attributes { get; set; }
        public ObservableEvent[] observableEvents { get; set; }
        public bool isActive { get; set; }
        public Authentication authentication { get; set; }
    }

    public class State
    {
        public string lifecycleState { get; set; }
        public string operationalState { get; set; }
    }

    public class Attributes
    {
        public Standard[] standard { get; set; }
    }

    public class Standard
    {
        public Attributetype attributeType { get; set; }
        public string attributeTypeId { get; set; }
        public string value { get; set; }
    }

    public class Attributetype
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public string name { get; set; }
        public Description[] description { get; set; }
        public string type { get; set; }
        public bool isActive { get; set; }
    }

    public class Authentication
    {
        public string authenticationType { get; set; }
        public bool isSystemGeneratedAuthnCredential { get; set; }
    }

    public class Name
    {
        public string lang { get; set; }
        public string text { get; set; }
    }

    public class ObservableEvent
    {
        public string id { get; set; }
        public string version { get; set; }
        public string creator { get; set; }
        public string creatorAppId { get; set; }
        public long creation { get; set; }
        public string realm { get; set; }
        public string name { get; set; }
        public Description[] description { get; set; }
        public Eventfield[] eventFields { get; set; }
        public bool isActive { get; set; }
        public bool requiresAck { get; set; }
    }

    public class Description
    {
        public string lang { get; set; }
        public string text { get; set; }
    }

    public class Eventfield
    {
        public string name { get; set; }
        public string type { get; set; }
        public string boundAttributeTypeId { get; set; }
    }
}







