using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GruppUppgift_Väderdata
{
    public static class Inomhus
    {
        private static string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(Inne),(\d+\.\d+),(\d+)";
        private static Regex regex = new Regex(pattern);

        public static void ViewBox(this string input)
        {
            Console.WriteLine(new String('-', input.Length + 4));
            Console.WriteLine("| " + input + " |");
            Console.WriteLine(new String('-', input.Length + 4));
        }

        public static void Sökmöjlighet()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);

            Console.WriteLine("Ange datumet du vill kolla medeltemperaturen och medelluftfuktighet på inomhus");
            string datum = Console.ReadLine();
            var temperatureData = new List<double>();
            var LuftfuktighetData = new List<double>();
            foreach (string line in lines)
            {
                Match match = regex.Match(line);
                string date = match.Groups[1].Value;
                string time = match.Groups[2].Value;
                string location = match.Groups[3].Value;
                string temperature = match.Groups[4].Value.Replace(".", ",");
                string humidity = match.Groups[5].Value;
                double Medeltemp;
                double Medelluftfuktighet;
                if (double.TryParse(temperature, out Medeltemp) && double.TryParse(humidity, out Medelluftfuktighet) && location == "Inne" && date == datum)
                {
                    temperatureData.Add(Medeltemp);
                    LuftfuktighetData.Add(Medelluftfuktighet);
                    Console.WriteLine("Datum: {0}, Tid: {1}, Plats: {2}, Temperatur: {3}, Luftfuktighet: {4}", date, time, location, Medeltemp, humidity);
                }
            }
            double averageTemperature = temperatureData.Average();
            double averageHumidity = LuftfuktighetData.Average();
            string Datum = "Datum: " + datum;
            Datum.ViewBox();
            Console.WriteLine("MedelTemperatur: {0:F2}, MedelLuftfuktighet: {1:F2}",
                                  averageHumidity, averageTemperature);
        }


        public static void SorteraVarmastTillKallaste(string filePath)
        {
            var data = LäsAllData(filePath, "Inne")
                .GroupBy(x => x.Datum.Date)
                .Select(group => new
                {
                    Datum = group.Key,
                    MedelTemperatur = group.Average(x => x.Temperatur)
                })
                .OrderByDescending(x => x.MedelTemperatur)
                .ToList();

            Console.WriteLine("\nVarmast till kallaste dagar (Inne):");
            foreach (var dag in data)
            {
                Console.WriteLine($"{dag.Datum.ToShortDateString()}: Medeltemperatur = {dag.MedelTemperatur:F2}°C");
            }
        }

        public static void SorteraTorrastTillFuktigaste(string filePath)
        {
            var data = LäsAllData(filePath, "Inne")
                .GroupBy(x => x.Datum.Date)
                .Select(group => new
                {
                    Datum = group.Key,
                    MedelLuftfuktighet = group.Average(x => x.Luftfuktighet)
                })
                .OrderBy(x => x.MedelLuftfuktighet)
                .ToList();

            Console.WriteLine("\nTorrast till fuktigaste dagar (Inne):");
            foreach (var dag in data)
            {
                Console.WriteLine($"{dag.Datum.ToShortDateString()}: Medelluftfuktighet = {dag.MedelLuftfuktighet:F2}%");
            }
        }

        public static void SorteraEfterMögelrisk(string filePath)
        {
            var data = LäsAllData(filePath, "Inne")
                .GroupBy(x => x.Datum.Date)
                .Select(group => new
                {
                    Datum = group.Key,
                    Mögelrisk = group.Average(x => x.Temperatur) * group.Average(x => x.Luftfuktighet) / 100
                })
                .OrderByDescending(x => x.Mögelrisk)
                .ToList();

            Console.WriteLine("\nDagar sorterade efter mögelrisk (Inne):");
            foreach (var dag in data)
            {
                Console.WriteLine($"{dag.Datum.ToShortDateString()}: Mögelrisk = {dag.Mögelrisk:F2}");
            }
        }

        public static List<(DateTime Datum, double Temperatur, double Luftfuktighet)> LäsAllData(string filePath, string plats)
        {
            var lines = File.ReadAllLines(filePath);
            var resultat = new List<(DateTime Datum, double Temperatur, double Luftfuktighet)>();

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success && match.Groups[3].Value.Equals(plats, StringComparison.OrdinalIgnoreCase))
                {
                    // Uppdaterat för att matcha det fullständiga datum- och tidsformatet
                    if (DateTime.TryParseExact(match.Groups[1].Value + " " + match.Groups[2].Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datum))
                    {
                        var temperatur = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                        var luftfuktighet = double.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture);
                        resultat.Add((datum, temperatur, luftfuktighet));
                    }
                }
            }

            return resultat;
        }

    }
}
