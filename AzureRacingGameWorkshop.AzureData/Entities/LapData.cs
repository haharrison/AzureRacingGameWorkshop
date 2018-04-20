using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureRacingGameWorkshop.AzureData.Entities
{
    [DataContract(Namespace = "http://cloudcasts.net/reddogracing/contracts")]
    public class LapData
    {
        [DataMember]
        public string PlayerName { get; set; }

        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public int CarType { get; set; }

        [DataMember]
        public string LapId { get; set; }

        [DataMember]
        public DateTime GameStartTime { get; set; }

        [DataMember]
        public int LapTimeMs { get; set; }

        [DataMember]
        public int Dammage { get; set; }

        [DataMember]
        public List<int> SectorTimesMs { get; set; }

        [DataMember]
        public int LapDelta { get; set; }

        [DataMember]
        public int Ranking { get; set; }

        public string WebLapTime
        {
            get { return TimeSpan.FromMilliseconds(LapTimeMs).ToString("mm\\:ss\\.fff"); }
        }

        public string WebDelta
        {
            get { return TimeSpan.FromMilliseconds(LapDelta).ToString("ss\\.fff"); }
        }

        public List<string> WebSectorTimes
        {
            get
            {
                List<string> webSectorTimes = new List<string>();
                foreach (int sectorTime in SectorTimesMs)
                {
                    webSectorTimes.Add(TimeSpan.FromMilliseconds(sectorTime).ToString("mm\\:ss\\.fff"));
                }

                return webSectorTimes;
            }
        }

        public LapData()
        {
            SectorTimesMs = new List<int>();
            GameStartTime = DateTime.UtcNow;
        }

        public LapData(DynamicTableEntity lapDataEntity)
        {
            PlayerName = lapDataEntity.Properties["PlayerName"].StringValue;
            Level = (int)lapDataEntity.Properties["Level"].Int32Value;

            try { CarType = (int)lapDataEntity.Properties["CarType"].Int32Value; }
            catch { }
            try { Dammage = (int)lapDataEntity.Properties["Dammage"].Int32Value; }
            catch { }

            LapId = lapDataEntity.Properties["LapId"].StringValue;
            LapTimeMs = (int)lapDataEntity.Properties["LapTimeMs"].Int32Value;

            // Set the sector times
            SectorTimesMs = new List<int>();
            for (int sector = 1; sector < int.MaxValue; sector++)
            {
                if (lapDataEntity.Properties.ContainsKey("Sector" +
                    sector.ToString().PadLeft(2, '0')))
                {
                    SectorTimesMs.Add((int)lapDataEntity.Properties
                        ["Sector" + sector.ToString().PadLeft(2, '0')].Int32Value);
                }
                else
                {
                    break;
                }
            }
        }


    }
}

