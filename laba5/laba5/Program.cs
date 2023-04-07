using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// Работа с файлами

/*
    Создать программу анализа лог-файла прокси-сервера, содержащего в каждой строке 
    информацию о клиенте прокси-сервера, запрашиваемом адресе, дате и времени поступления 
    запроса, размере переданных данных и пользователе, запросившем страницу. После анализа 
    предоставить возможность построения текстовых отчетов по суммарным оборотам пользователя, 
    запрошенных доменов за заданный промежуток времени.
*/

namespace laba5 {
    class Program {
        static void Main() {
            // Get file content
            string currentPath = Path.GetDirectoryName("C:/Users/user/source/repos/laba5/laba5/");
            string inputFileName = "\\log.txt";
            if (!File.Exists(currentPath + inputFileName)) {
                Console.WriteLine("File not found!");
                System.Environment.Exit(2);
            }
            string[] lines = File.ReadAllLines(@currentPath + inputFileName);
            FileInfo infoFromFile = new FileInfo();

            // Add information in FileInfo class
            foreach (string line in lines) {
                infoFromFile.clients.Add(line.Split(",")[0]);
                infoFromFile.addresses.Add(line.Split(",")[1].Trim());
                infoFromFile.dates.Add(ParseDateTimeFromString(line.Split(",")[2].Trim()));
                infoFromFile.dataBytes.Add(int.Parse(line.Split(",")[3].Trim()));
                infoFromFile.userNames.Add(line.Split(",")[4].Trim());
                Console.WriteLine(line);
            }
            // Enter filters
            string userName = EnterUserName();
            DateTime[] dateRange = EnterDateRange();

            // Filtering by enterred data
            FileInfo filteredInfoFromFile = new FileInfo();
            for (int index = 0; index < infoFromFile.userNames.Count; index++) {
                if (infoFromFile.userNames[index] == userName
                    && infoFromFile.dates[index] >= dateRange[0] 
                    && infoFromFile.dates[index] <= dateRange[1]
                ) {
                    filteredInfoFromFile.clients.Add(infoFromFile.clients[index]);
                    filteredInfoFromFile.addresses.Add(infoFromFile.addresses[index]);
                    filteredInfoFromFile.dates.Add(infoFromFile.dates[index]);
                    filteredInfoFromFile.dataBytes.Add(infoFromFile.dataBytes[index]);
                    filteredInfoFromFile.userNames.Add(infoFromFile.userNames[index]);
                }
            }

            // Generating report
            string outputFileName = "\\output.txt";
            string text = userName + " visitted next address in this time range: \n"
                + string.Join("\n", filteredInfoFromFile.addresses)
                + "\nTotally received " + (filteredInfoFromFile.dataBytes.Take(filteredInfoFromFile.dataBytes.Count).Sum()) + " bytes";

            File.WriteAllText(currentPath+outputFileName, text);
        }

        static string EnterUserName() {
            Console.WriteLine("Please enter user name for generate report:");
            string userName = Console.ReadLine();
            return userName;
        }

        // Returns [firstDate, secondDate]
        static DateTime[] EnterDateRange() {
            Console.WriteLine("Please enter start date for generate report");
            Console.WriteLine("Format should be dd-MM-yyyy HH:mm");
            string startDate = Console.ReadLine();
            if (startDate.IndexOf(" ") < 1) {
                Console.WriteLine("Incorrect start date format!");
                System.Environment.Exit(2);
            }

            Console.WriteLine("Please enter end date for generate report");
            Console.WriteLine("Format should be dd-MM-yyyy HH:mm");
            string endDate = Console.ReadLine();
            if (endDate.IndexOf(" ") < 1) {
                Console.WriteLine("Incorrect end date format!");
                System.Environment.Exit(2);
            }

            DateTime[] resultDateTimeRange = {
                ParseDateTimeFromString(startDate),
                ParseDateTimeFromString(endDate)
            };
            return resultDateTimeRange;
        }

        static DateTime ParseDateTimeFromString(string dateString) {
            return DateTime.ParseExact(dateString, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

    }

    class FileInfo {
        public List<string> clients = new List<string>();
        public List<string> addresses = new List<string>();
        public List<DateTime> dates = new List<DateTime>();
        public List<int> dataBytes = new List<int>();
        public List<string> userNames = new List<string>();
    }
}
