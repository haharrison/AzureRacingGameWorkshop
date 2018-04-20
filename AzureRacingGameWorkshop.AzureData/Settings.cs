using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureRacingGameWorkshop.AzureData
{
    public class Settings
    {
        // Player Name
        public static string PlayerName = "YOUR_NAME";

        // Storage Account
        public static string StorageAccountName = "";
        public static string StorageAccountKey = "";


        // Development Storage
        public static bool UseDevelopmentStorage = false;

        // Data
        public static bool SendTelemetryData = false;



        // Blobs 
        public static string ReplayBlobContainer = "replays";

        // Tables
        public static string PlayerLapTableNamePrefix = "PlayerLapTimes";
        public static string RankingLapTimesTableName = "RankingLapTimes";
        public static string RankingLapTimesPartitionKeyPrefix = "Track";

        // Game
        public static int MaxGhostCars = 20;
        public static int NumberOfTracks = 3;

        // Telemetry Intervals
        public static TimeSpan LogTelemetryInterval = TimeSpan.FromMilliseconds(100);
        public static TimeSpan SendTelemetryInterval = TimeSpan.FromMilliseconds(1000);


    }
}
