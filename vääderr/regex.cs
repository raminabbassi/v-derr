using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Väderdata
{
    internal class SortingRegex
    {
        public static void Printerafilen()
        {
            string pattern = @"(\d{4}-\d{2}-\d{2})\s(\d{2}:\d{2}:\d{2}),(\w+),(\d+\.\d+),(\d+)";
            Regex regex = new Regex(pattern);

            string filename = @"..\..\..\Textfiler\tempdata5-med fel.txt";
            string[] lines = System.IO.File.ReadAllLines(filename);
            int count = 0;
            foreach (string line in lines)
            {
                if (count >= 20) break;

                Match match = regex.Match(line);
                if (match.Success)
                {
                    string date = match.Groups[1].Value;
                    string time = match.Groups[2].Value;
                    string location = match.Groups[3].Value;
                    string temperature = match.Groups[4].Value;
                    string humidity = match.Groups[5].Value;
                    double linqtemp = double.Parse(temperature);

                    Console.WriteLine("Datum: {0}, Tid: {1}, Plats: {2}, Temperatur: {3}, Luftfuktighet: {4}", date, time, location, temperature, humidity);
                    count++;
                }
            }
            Console.ReadLine();
        }
    }
}
