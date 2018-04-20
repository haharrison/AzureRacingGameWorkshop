using AzureRacingGameWorkshop.AzureData.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AzureRacingGameWorkshop.AzureData.Tables
{
    public class LapDataTable
    {

        public LapDataTable()
        {
            Debug.WriteLine(">>LapDataTable()");


        }

        public void InsertRankedTime(LapData lapData)
        {
            Debug.WriteLine(">>LapDataTable.InsertRankedTime");

        }




        public void InsertOrReplaceDriverLapData(LapData lapData)
        {
            Debug.WriteLine(">>LapDataTable.InsertOrReplaceDriverLapData");

            try
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public List<LapData> GetTopLapTimes(int level, int number, bool bestLapOnly)
        {
            Debug.WriteLine(">>LapDataTable.GetTopLapTimes");

            // return new List<LapData>();
            CloudTable table = m_TableClient.GetTableReference(Settings.RankingLapTimesTableName);

            IQueryable<DynamicTableEntity> query;

            if (bestLapOnly)
            {
                query = (from q
                    in table.CreateQuery<DynamicTableEntity>()
                         where q.PartitionKey.Equals(Settings.RankingLapTimesPartitionKeyPrefix + level)
                         select q);
            }
            else
            {
                query = (from q
                    in table.CreateQuery<DynamicTableEntity>()
                         where q.PartitionKey.Equals(Settings.RankingLapTimesPartitionKeyPrefix + level)
                         select q).Take(number);
            }
            List<LapData> topTimes = new List<LapData>();
            List<string> driverNames = new List<string>();

            int fastestTime = 0;
            int ranking = 1;

            foreach (DynamicTableEntity entity in query)
            {
                bool addLap = true;

                if (bestLapOnly)
                {
                    string driverName = entity.Properties["PlayerName"].StringValue;
                    if (driverNames.Contains(driverName))
                    {
                        addLap = false;
                    }
                    else
                    {
                        driverNames.Add(driverName);
                    }
                }
                if (addLap)
                {
                    LapData lapData = new LapData(entity);
                    if (ranking == 1)
                    {
                        fastestTime = lapData.LapTimeMs;
                    }
                    lapData.LapDelta = lapData.LapTimeMs - fastestTime;
                    lapData.Ranking = ranking;
                    topTimes.Add(lapData);
                    ranking++;
                }
            }
            return topTimes;
        }




        private string GetPlayerLapTableName(int level)
        {
            return Settings.PlayerLapTableNamePrefix + level;
        }



    }
}

