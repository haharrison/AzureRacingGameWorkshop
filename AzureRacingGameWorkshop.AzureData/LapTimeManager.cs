using AzureRacingGameWorkshop.AzureData.Entities;
using AzureRacingGameWorkshop.AzureData.Tables;
//using AzureRacingGameWorkshop.AzureData.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AzureRacingGameWorkshop.AzureData
{
    public class LapTimeManager
    {
        private const int NumberOfTracks = 3;

        public List<List<LapData>> LapTimes { get; set; }

        private LapDataTable m_LapDataTable;


        public LapTimeManager()
        {
            m_LapDataTable = new LapDataTable();

            //LapTimes = new List<List<LapData>>();

            //for (int tracl = 0; tracl < NumberOfTracks; tracl++)
            //{
            //    LapTimes.Add(new List<LapData>());
            //}
            Update();
        }

        public void Update()
        {
            LapTimes = new List<List<LapData>>();
  
            for (int trackNumber = 0; trackNumber < NumberOfTracks; trackNumber++)
            {
                List<LapData> lapData = m_LapDataTable.GetTopLapTimes(trackNumber, 100, true);
                LapTimes.Add(lapData);
            }
        }

        public int GetLapPosition(int track, int time)
        {
            int position = 0;

            Update();

            foreach (LapData lapData in LapTimes[track])
            {
                position++;
                if (time < lapData.LapTimeMs)
                {
                    return position;
                }
            }

            return position;
        }

        public Tuple<int, int> GetSectorPosition(int track, int sector, int time)
        {
            // Don't refresh the times, as it would disturb the game...
            // Or do it async, and callback to send the position...

            //if (sector > 1) Debugger.Break();

            int position = 1;
            int bestLapSectorTime = int.MaxValue;

            // Do we have a best sector time for that lap?
            //if (LapTimes[track].Count > 1 && LapTimes[track][1].SectorTimesMs.Count >= sector)
            //{
            //    bestLapSectorTime = LapTimes[track][1].SectorTimesMs[sector];
            //}

            foreach (LapData lapData in LapTimes[track])
            {
                try
                {
                    if (LapTimes[track][1].SectorTimesMs.Count >= sector && time > lapData.SectorTimesMs[sector])
                    {
                        position++;
                    }
                    if (LapTimes[track][1].SectorTimesMs.Count >= sector && bestLapSectorTime > lapData.SectorTimesMs[sector])
                    {
                        bestLapSectorTime = lapData.SectorTimesMs[sector];
                    }
                }
                catch
                {
                    Debug.WriteLine("GetSectorPosition Fail!");
                }
            }

            if (bestLapSectorTime == int.MaxValue)
            {
                bestLapSectorTime = time;
            }

            return new Tuple<int, int>(position, bestLapSectorTime);
        }



    }
}
