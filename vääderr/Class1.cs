using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Väderdata
{
    internal class Class1
    {
        public static void Medeltemperatur()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"C:\Users\ramin\OneDrive\Dokument\visualstudio\repos\c#\.vs\c#\väder3\väder3\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);
            Console.WriteLine("Ange datumet du vill kolla medeltemperaturen och medelluftfuktighet på (yyyy-mm-dd) mellan 2016-05-31 och 2017-01-10");
            string datum = Console.ReadLine();

            List<(string date, double temperature, double humidity)> data = new List<(string date, double temperature, double humidity)>();

            foreach (string line in lines)
            {
                Match match = regex.Match(line);
                string date = match.Groups[1].Value;
                string temperature = match.Groups[4].Value;
                string humidity = match.Groups[5].Value;
                if (datum == date)
                {
                    if (double.TryParse(temperature, out double temp) &&
                        double.TryParse(humidity, out double hum))
                    {
                        data.Add((date, temp, hum));

                    }
                }
            }
            
            if (data.Count > 0)
            {
                double averageTemperature = data.Average(d => d.temperature);
                double avgHumidity = data.Average(d => d.humidity);
                Console.WriteLine("Medeltemperatur: {0}°C, Medelluftfuktighet: {1}%", averageTemperature, avgHumidity);
            }
            else
            {
                Console.WriteLine("Ingen data hittades för det angivna datumet.");
            }
        }
    }
}
