﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureRacingGameWorkshop.AzureData
{
public class Settings
{
    // Player Name
    public static string PlayerName = "@willieisreal";

    // Storage Account
    public static string StorageAccountName = "gabracing2018storage";
    public static string StorageAccountKey = "IIpWLR6ui+yFa2OM1F4o/V1EVHUF2reV9QBK4bstEhN6Nvmj8eG3K2/u2JsAmfPEOXmu4SB7gWiMGs4kzEzRDw==";


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