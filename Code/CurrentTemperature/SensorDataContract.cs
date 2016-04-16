using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CurrentTemperature
{
    /// <summary>
    /// From ConnectTheDots.io
    /// </summary>
    [DataContract]
    public class SensorDataContract
    {
        [DataMember(Name = "value")]
        public double Value { get; set; }

        [DataMember(Name = "guid")]
        public string Guid { get; set; }

        [DataMember(Name = "organization")]
        public string Organization { get; set; }

        [DataMember(Name = "displayname")]
        public string DisplayName { get; set; }

        [DataMember(Name = "unitofmeasure")]
        public string UnitOfMeasure { get; set; }

        [DataMember(Name = "measurename")]
        public string MeasureName { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "timecreated")]
        public DateTime TimeCreated { get; set; }
    }
}
