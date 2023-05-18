using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Работа с параллельными процессами

/*
   Вариант 1
   Вводится директория и даты для генерации отчетов
   Паралелльно парсятся файлы
   Три вида отчетов:
   Первый - по каждой машине не зависимо от дат (количество всех запросов, количество трафика, среднее за день)
   Второй - расчет по времени (объем трафика, домен с наибольшим трафиком, ip адрес с наибольшим трафиком)
   Третий - расчет по доменам (объем трафика, три популярных браузера, три ip адреса с макс. трафиком) 
*/

namespace laba8 {
    class Program {

        static void Main() {
            Console.WriteLine("Laba 7, variant 1, work with parallel process");
            // Enter filters
            string currentPath = EnterBasePath();
            DateTime[] dateRange = EnterDateRange();

            List<FileInfo> infoFromFiles = new List<FileInfo>();
            List<string> listFiles = new List<string> {
                currentPath + "\\log1.txt",
                currentPath + "\\log2.txt",
                currentPath + "\\log3.txt",
                currentPath + "\\log4.txt"
            };
            List<Thread> threadsArray = new List<Thread>();
            foreach (var filePath in listFiles) {
                Thread t = new Thread(() => ThreadFunc(filePath, infoFromFiles));
                t.Start();
                threadsArray.Add(t);
            }
            foreach (var currentThread in threadsArray) {
                currentThread.Join();
                Console.WriteLine("Thread stopped");
            }
            //Parallel.ForEach(listFiles, (currentFileName) => {
            //    FileInfo infoFromFile = ReadFromFile(currentFileName);
            //    infoFromFiles.Add(infoFromFile);
            //});

            // Generating report
            FileInfo resultInfo = new FileInfo();
            foreach (var info in infoFromFiles) {
                resultInfo.clients.AddRange(info.clients);
                resultInfo.addresses.AddRange(info.addresses);
                resultInfo.dates.AddRange(info.dates);
                resultInfo.dataBytes.AddRange(info.dataBytes);
                resultInfo.userNames.AddRange(info.userNames);
            }
            List<string> uniqAddresses = resultInfo.addresses.Distinct().ToList();
            List<string> uniqUserNames = resultInfo.userNames.Distinct().ToList();

            //Чистим файлы
            File.WriteAllText(currentPath + "\\output1.txt", "");
            File.WriteAllText(currentPath + "\\output2.txt", "");
            File.WriteAllText(currentPath + "\\output3.txt", "");

            // Первый - по каждой машине не зависимо от дат (количество всех запросов, количество трафика, среднее за день)
            foreach (var uniqUserName in uniqUserNames) {
                var clientRequests = resultInfo.userNames.Select((Value, Index) => new { Value, Index })
                  .Where(p => p.Value == uniqUserName);
                int countTotalBytes = 0;
                DateTime minDate = new DateTime(2023, 12, 2);
                DateTime maxDate = new DateTime(2000, 1, 1);
                foreach (var item in clientRequests) {
                    countTotalBytes += resultInfo.dataBytes[item.Index];
                    if (resultInfo.dates[item.Index] < minDate) {
                        minDate = resultInfo.dates[item.Index];
                    }
                    if (resultInfo.dates[item.Index] > maxDate) {
                        maxDate = resultInfo.dates[item.Index];
                    }
                }
                int countOfDateRange = (maxDate - minDate).Days == 0 ? 1 : (maxDate - minDate).Days;
                double averageDayRequests = Math.Round((double)(clientRequests.Count() / countOfDateRange));

                string textFirstFile = uniqUserName + " сделал всего " + clientRequests.Count() + " запросов,"
                + "\nвсего получено " + countTotalBytes + " байт,"
                + "\nв среднем за день делает " + averageDayRequests + " запросов.\n\n";

                File.AppendAllText(currentPath + "\\output1.txt", textFirstFile);
            }

            // Второй - расчет по времени (объем трафика, домен с наибольшим трафиком, ip адрес с наибольшим трафиком)
            FileInfo filteredInfoFromFile = new FileInfo();
            int totalBytes = 0;
            string domainNameWithMaxTraffic = "";
            int domainRequestsWithMaxTraffic = 0;
            string ipWithMaxTraffic = "";
            int ipRequestsWithMaxTraffic = 0;
            for (int index = 0; index < resultInfo.userNames.Count; index++) {
                if (resultInfo.dates[index] >= dateRange[0]
                    && resultInfo.dates[index] <= dateRange[1]
                ) {
                    filteredInfoFromFile.clients.Add(resultInfo.clients[index]);
                    filteredInfoFromFile.addresses.Add(resultInfo.addresses[index]);
                    filteredInfoFromFile.dates.Add(resultInfo.dates[index]);
                    filteredInfoFromFile.dataBytes.Add(resultInfo.dataBytes[index]);
                    filteredInfoFromFile.userNames.Add(resultInfo.userNames[index]);
                    totalBytes += resultInfo.dataBytes[index];
                }
            }

            foreach (var uniqAddress in uniqAddresses) {
                var addressesRequests = filteredInfoFromFile.addresses.Select((Value, Index) => new { Value, Index })
                  .Where(p => p.Value == uniqAddress);
                int sumBytes = 0;
                foreach (var item in addressesRequests) {
                    sumBytes += filteredInfoFromFile.dataBytes[item.Index];
                }

                if (sumBytes >= domainRequestsWithMaxTraffic) {
                    domainNameWithMaxTraffic = uniqAddress;
                    domainRequestsWithMaxTraffic = sumBytes;
                }
            }

            foreach (var uniqUserName in uniqUserNames) {
                var userRequests = filteredInfoFromFile.addresses.Select((Value, Index) => new { Value, Index })
                  .Where(p => p.Value == uniqUserName);
                int sumBytes = 0;
                foreach (var item in userRequests) {
                    sumBytes += filteredInfoFromFile.dataBytes[item.Index];
                }

                if (sumBytes >= ipRequestsWithMaxTraffic) {
                    ipWithMaxTraffic = uniqUserName;
                    ipRequestsWithMaxTraffic = sumBytes;
                }
            }

            string textSecondFile = "Всего за этот период времени отправлено " + totalBytes + " байт,"
                + "\nнаибольший трафик был на домене " + domainNameWithMaxTraffic + ","
                + "\nбольше всего запросов сделал " + ipWithMaxTraffic + "\n\n";
            File.AppendAllText(currentPath + "\\output2.txt", textSecondFile);

            // Третий - расчет по доменам (объем трафика, три популярных браузера, три ip адреса с макс. трафиком) 
            foreach (var uniqAddress in uniqAddresses) {
                var addressesRequests = resultInfo.addresses.Select((Value, Index) => new { Value, Index })
                  .Where(p => p.Value == uniqAddress);
                int sumBytes = 0;
                List<string> clientsByAddress = new List<string>();
                List<string> userNameByAddress = new List<string>();
                foreach (var item in addressesRequests) {
                    sumBytes += resultInfo.dataBytes[item.Index];
                    clientsByAddress.Add(resultInfo.clients[item.Index]);
                    userNameByAddress.Add(resultInfo.userNames[item.Index]);
                }

                string textThirdFile = "Всего на домен " + uniqAddress + " отправлено " + sumBytes + " байт,"
                + "\nтри самых популярных браузера: " + string.Join(", ", clientsByAddress.Distinct().Take(3)) + ","
                + "\nтри пользователя с макс. трафиком: " + string.Join(", ", userNameByAddress.Distinct().Take(3)) + "\n\n";
                File.AppendAllText(currentPath + "\\output3.txt", textThirdFile);
            }

            Console.ReadLine();
        }

        static FileInfo ReadFromFile(string filePath) {
            // Get file content
            if (!File.Exists(filePath)) {
                Console.WriteLine("File not found!");
                System.Environment.Exit(2);
            }
            string[] lines = File.ReadAllLines(@filePath);
            FileInfo infoFromFile = new FileInfo();

            // Add information in FileInfo class
            foreach (string line in lines) {
                infoFromFile.clients.Add(line.Split(",")[0]);
                infoFromFile.addresses.Add(line.Split(",")[1].Trim());
                infoFromFile.dates.Add(ParseDateTimeFromString(line.Split(",")[2].Trim()));
                infoFromFile.dataBytes.Add(int.Parse(line.Split(",")[3].Trim()));
                infoFromFile.userNames.Add(line.Split(",")[4].Trim());
            }
            return infoFromFile;
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

        static string EnterBasePath() {
            Console.WriteLine("Please enter absolute path to target directory:");
            string basePath = Console.ReadLine();
            return basePath;
        }

        public static void ThreadFunc(string filePath, List<FileInfo> infoFromFiles) {
            Console.WriteLine("New thread started");
            FileInfo infoFromFile = ReadFromFile(filePath);
            infoFromFiles.Add(infoFromFile);
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
