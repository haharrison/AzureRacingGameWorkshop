using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AzureRacingGameWorkshop.AzureData.DataContracts
{
    [DataContract]
    public class TrackVectorData
    {

        public TrackVectorData()
        {
            TrackPoints = new List<TrackVector>();
        }

        [DataMember]
        public List<TrackVector> TrackPoints { get; set; }
    }
}
