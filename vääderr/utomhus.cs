using System.Globalization;
using System.Text.RegularExpressions;

namespace Väderdata
{

    public static class Utomhus
    {

        public static void seeIn(this string input)
        {
            Console.WriteLine(new String('-', input.Length + 4));
            Console.WriteLine("| " + input + " |");
            Console.WriteLine(new String('-', input.Length + 4));
        }

        public static void SökMotor()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);
            Console.WriteLine("Ange datumet du vill kolla medeltemperaturen och medelluftfuktighet på (yyyy-mm-dd) mellan 2016-05-31 och 2017-01-10");
            string datum = Console.ReadLine();
            string Datum = "Datum: " + datum;
            Datum.seeIn();
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
                if (double.TryParse(temperature, out Medeltemp) && double.TryParse(humidity, out Medelluftfuktighet) && location == "Ute" && date == datum)
                {
                    temperatureData.Add(Medeltemp);
                    LuftfuktighetData.Add(Medelluftfuktighet);
                    Console.WriteLine("Datum: {0}, Tid: {1}, Plats: {2}, Temperatur: {3}, Luftfuktighet: {4}", date, time, location, Medeltemp, humidity);
                }
            }
            double averageTemperature = temperatureData.Average();
            double averageHumidity = LuftfuktighetData.Average();
            Console.WriteLine("\nslumpmässiga medelTemperaturer och medelfuktigheter\n");
            Console.WriteLine("MedelTemperatur: {0:F2}, MedelLuftfuktighet: {1:F2}", averageHumidity, averageTemperature);
        }

        public static void SortByAverageTemperature()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);
            var temperatureData = new List<double>();
            foreach (string line in lines)
            {
                Match match = regex.Match(line);
                string date = match.Groups[1].Value;
                string location = match.Groups[3].Value;
                string temperature = match.Groups[4].Value.Replace(".", ",");
                double Medeltemp;
                if (double.TryParse(temperature, out Medeltemp) && location == "Ute")
                {
                    temperatureData.Add(Medeltemp);
                }
            }
            var dataByDate = lines
                .Select(line =>
                {
                    Match match = regex.Match(line);
                    string date = match.Groups[1].Value;
                    string temperature = match.Groups[4].Value.Replace(".", ",");
                    double Medeltemp;
                    double Medelluftfuktighet;
                    if (double.TryParse(temperature, out Medeltemp) && match.Groups[3].Value == "Ute")
                    {
                        return new { Date = date, Temperature = Medeltemp };
                    }
                    return null;
                })
                .Where(x => x != null)
                .GroupBy(x => x.Date)
                .OrderBy(group => group.Average(d => d.Temperature))
                .ToList();

            foreach (var group in dataByDate)
            {
                Console.WriteLine("Datum: {0}", group.Key);
                Console.WriteLine("Medeltemperatur: {0:F2}", group.Average(d => d.Temperature));
            }
        }


        public static void SortByHumidity()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);
            var LuftfuktighetData = new List<double>();
            foreach (string line in lines)
            {
                Match match = regex.Match(line);
                string date = match.Groups[1].Value;
                string location = match.Groups[3].Value;
                string humidity = match.Groups[5].Value;
                double Medelluftfuktighet;
                if (double.TryParse(humidity, out Medelluftfuktighet) && location == "Ute")
                {
                    LuftfuktighetData.Add(Medelluftfuktighet);
                }
            }

            var dataByDate = lines
                .Select(line =>
                {
                    Match match = regex.Match(line);
                    string date = match.Groups[1].Value;
                    string humidity = match.Groups[5].Value;
                    double Medelluftfuktighet;
                    if (double.TryParse(humidity, out Medelluftfuktighet) && match.Groups[3].Value == "Ute")
                    {
                        return new { Date = date, Humidity = Medelluftfuktighet };
                    }
                    return null;
                })
               .Where(x => x != null)
                .GroupBy(x => x.Date)
                .OrderByDescending(group => group.Average(averageHumidity => averageHumidity.Humidity)) //s
                .ToList();

            foreach (var group in dataByDate)
            {
                Console.WriteLine("Datum: {0}", group.Key);
                Console.WriteLine("medelluftfuktighet: {0:F2}", group.Average(d => d.Humidity));

            }
        }
        public static void MoldRisk()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);
            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt"; // Anpassa sökvägen efter din miljö
            string[] lines = System.IO.File.ReadAllLines(filename);

            var groupedData = lines.Select(line => regex.Match(line))
                                   .Where(match => match.Success && match.Groups[3].Value == "Ute")
                                   .Select(match => new
                                   {
                                       Date = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                       Temperature = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                                       Humidity = double.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture)
                                   })
                                   .GroupBy(x => x.Date.Date)
                                   .Select(group => new
                                   {
                                       Datum = group.Key,
                                       Mögelrisk = group.Average(x => x.Temperature) * group.Average(x => x.Humidity) / 100
                                   })
                                   .OrderByDescending(x => x.Mögelrisk)
                                   .ToList();

            Console.WriteLine("\nDagar sorterade efter mögelrisk (Ute):");
            foreach (var dag in groupedData)
            {
                Console.WriteLine($"{dag.Datum.ToShortDateString()}: Mögelrisk = {dag.Mögelrisk:F2}");
            }
        }

    }
}

