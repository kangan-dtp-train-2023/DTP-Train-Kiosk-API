﻿using System.Text.Json;
namespace KioskLib;

public class TrainList
{
    public List<Train>? departures { get; set; }
}

public class Train
{
    public int? run_id { get; set; }
    public int? direction_id { get; set; }
    public string? estimated_departure_utc { get; set; }
    public string? platform_number { get; set; }
}

public class TrainFetch : Fetch
{
    public TrainFetch()
    {
        init();
    }
    private int route_type = 0;
    public string Search(string station)
    {
        string url = $"/v3/search/{station}?route_types={route_type}&devid={devid}";
        return Request(url).Result;
    }
    public string Departures(int stopID, int destinationID)
    {
        //.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string url = $"/v3/departures/route_type/{route_type}/stop/{stopID}?direction_id={destinationID}&max_results=10&devid={devid}";
        TrainList? trains = JsonSerializer.Deserialize<TrainList>(Request(url).Result);
        foreach (Train t in trains.departures)
        {
            if(t.estimated_departure_utc == null || t.platform_number == null)
            {
                continue;
            }
            DateTime dt = DateTime.ParseExact(t.estimated_departure_utc, "yyyy-MM-ddTHH:mm:ssZ", null); 
            DateTime time = DateTime.UtcNow.Date.ToUniversalTime();
            if(dt >= time.AddMinutes(5))
            {
                string urll = $"/v3/runs/{t.run_id}?expand=None&devid={devid}";
                return JsonSerializer.Serialize(t);
            };
        }
        return "N/A";
    }
}