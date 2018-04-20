using AzureRacingGameWorkshop.AzureData.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureRacingGameWorkshop.AzureData.Tables
{
    public class LapDataTable
    {
        private CloudTableClient m_TableClient;

        public LapDataTable()
        {
            Debug.WriteLine(">>LapDataTable()");

            // Create the table client
            CloudStorageAccount account;

            StorageCredentials creds = new StorageCredentials
                (Settings.StorageAccountName, Settings.StorageAccountKey);
            account = new CloudStorageAccount(creds, true);

            m_TableClient = account.CreateCloudTableClient();

            // Create the tables
            CloudTable rankingsTable = m_TableClient.GetTableReference
                (Settings.RankingLapTimesTableName);

            rankingsTable.CreateIfNotExists();

            for (int track = 0; track < Settings.NumberOfTracks; track++)
            {
                CloudTable table = m_TableClient.GetTableReference
                    (GetPlayerLapTableName(track));

                table.CreateIfNotExists();
            }
        }

        public void InsertRankedTime(LapData lapData)
        {
            Debug.WriteLine(">>LapDataTable.InsertRankedTime");

            // Get a reference to the rankings table
            CloudTable table = m_TableClient.GetTableReference
                (Settings.RankingLapTimesTableName);

            // Create the partition key and row key for the entity
            string partitionKey =
                Settings.RankingLapTimesPartitionKeyPrefix + lapData.Level;
            string rowKey =
                lapData.LapTimeMs.ToString().PadLeft(8, '0') + "-" +
                Utils.GetAscendingRowKey(lapData.GameStartTime);

            // Create a new dynamic entity
            DynamicTableEntity sectorTimesEntity = new
                DynamicTableEntity(partitionKey, rowKey);

            // Add properties for the driver lap
            sectorTimesEntity.Properties.Add
                ("PlayerName", new EntityProperty(lapData.PlayerName));
            sectorTimesEntity.Properties.Add
                ("LapStartTime", new EntityProperty(lapData.GameStartTime));
            sectorTimesEntity.Properties.Add
                ("Level", new EntityProperty(lapData.Level));
            sectorTimesEntity.Properties.Add
                ("CarType", new EntityProperty(lapData.CarType));
            sectorTimesEntity.Properties.Add
                ("Dammage", new EntityProperty(lapData.Dammage));
            sectorTimesEntity.Properties.Add
                ("LapId", new EntityProperty(lapData.LapId));
            sectorTimesEntity.Properties.Add
                ("LapTimeMs", new EntityProperty(lapData.LapTimeMs));

            // Add properties for the sector data
            int sector = 1;
            foreach (int sectorTime in lapData.SectorTimesMs)
            {
                sectorTimesEntity.Properties.Add
                    ("Sector" + sector.ToString().PadLeft(2, '0'),
                    new EntityProperty(sectorTime));
                sector++;
            }

            // Insert the entity in the table.
            TableOperation insertOp =
                TableOperation.Insert(sectorTimesEntity);
            table.Execute(insertOp);

        }




        public void InsertOrReplaceDriverLapData(LapData lapData)
        {
            Debug.WriteLine(">>LapDataTable.InsertOrReplaceDriverLapData");

            try
            {

                CloudTable table = m_TableClient.GetTableReference
               (GetPlayerLapTableName(lapData.Level));

                DynamicTableEntity sectorTimesEntity =
                    new DynamicTableEntity(lapData.PlayerName,
                    Utils.GetDescendingRowKey(lapData.GameStartTime));

                sectorTimesEntity.Properties.Add
                    ("PlayerName", new EntityProperty(lapData.PlayerName));
                sectorTimesEntity.Properties.Add
                    ("LapStartTime", new EntityProperty(lapData.GameStartTime));
                sectorTimesEntity.Properties.Add
                    ("Level", new EntityProperty(lapData.Level));
                sectorTimesEntity.Properties.Add
                    ("CarType", new EntityProperty(lapData.CarType));
                sectorTimesEntity.Properties.Add
                    ("Dammage", new EntityProperty(lapData.Dammage));
                sectorTimesEntity.Properties.Add
                    ("LapId", new EntityProperty(lapData.LapId));
                sectorTimesEntity.Properties.Add
                    ("LapTimeMs", new EntityProperty(lapData.LapTimeMs));

                int sector = 1;
                foreach (int sectorTime in lapData.SectorTimesMs)
                {
                    sectorTimesEntity.Properties.Add
                        ("Sector" + sector.ToString().PadLeft(2, '0'),
                        new EntityProperty(sectorTime));
                    sector++;
                }

                TableOperation insertOrReplaceOp =
                TableOperation.InsertOrReplace(sectorTimesEntity);

                table.BeginExecute(insertOrReplaceOp, null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public List<LapData> GetTopLapTimes(int level, int number, bool bestLapOnly)
        {
            Debug.WriteLine(">>LapDataTable.GetTopLapTimes");

            //return new List<LapData>();

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

            return new List<LapData>();


        }




        private string GetPlayerLapTableName(int level)
        {
            return Settings.PlayerLapTableNamePrefix + level;
        }



    }
}

