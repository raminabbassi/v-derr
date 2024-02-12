using GruppUppgift_Väderdata;

namespace Väderdata
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool fortsätt = true;
            while (fortsätt)
            {
                Console.WriteLine("Vill du starta programmet? (ja/nej)");
                string svar = Console.ReadLine();

                if (svar.ToLower() == "nej")
                {
                    fortsätt = false;
                }
                else
                {
                   string filePath = @"..\..\..\Textfiler\tempdata5-med fel.txt";

                    Utomhus.SökMotor();
                    Utomhus.SortByAverageTemperature();
                    Utomhus.SortByHumidity();
                    Utomhus.MoldRisk();

                    Console.WriteLine("Ange datumet du vill söka på (YYYY-MM-DD) inomhus:");
                    string valtDatum = Console.ReadLine();

                    Inomhus.Sökmöjlighet();
                    Inomhus.SorteraVarmastTillKallaste(filePath);
                    Inomhus.SorteraTorrastTillFuktigaste(filePath);
                    Inomhus.SorteraEfterMögelrisk(filePath);





                }

            }

        }
    }


}

