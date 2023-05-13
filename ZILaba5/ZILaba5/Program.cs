using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ZI5 {
    internal class Program {
        static void Main() {
            const double EPS = 3;

            while (true) {
                try {
                    Console.WriteLine("Для регистрации введите 1, для авторизации введите 2, для выхода 0");
                    string command = Console.ReadLine();

                    if (command == "0") {
                        Environment.Exit(0);
                    } else if (command == "1") {
                        string phrase = GetRandomPhraseFromFile("C:/Users/user/source/repos/C_Sharp_collection/ZILaba5/ZILaba5/phrases.txt");

                        double[] idealTimes = new double[4];
                        double[] deviationTimes = new double[4];

                        bool success = true;

                        for (int i = 0; i < 4; i++) {
                            Console.WriteLine($"\nПожалуйста введите данную фразу: \n{phrase}\n№{i} из 4");
                            var pair = getIdealValueAndDeviations(phrase);

                            idealTimes[i] = pair.IdealTime;
                            deviationTimes[i] = pair.Deviations;

                            if (idealTimes[i] == -1 || deviationTimes[i] == -1) {
                                success = false;
                                break;
                            }
                        }

                        if (success) {
                            Console.WriteLine("Регистрация прошла успешно");
                            double avgIdelTime = idealTimes.Average();
                            double avgdeviationTime = deviationTimes.Average();
                            string username = GetUsername();
                            SaveRegistrationInfo(username, phrase, avgIdelTime, avgdeviationTime);
                        } else {
                            Console.WriteLine("Входная фраза введена неверно");
                        }
                    } else if (command == "2") {
                        string username = GetUsername();
                        var timeLimit = GetTimeLimitForUser(username);
                        string phrase = GetPhraseForUser(username);

                        Console.WriteLine($"Введите данную фразу:\n{phrase}\nИдеальное время {timeLimit.IdealTime:F2}мс и отклонение {timeLimit.DeviationsTime:F2}мс");
                        var currentTimes = getIdealValueAndDeviations(phrase);

                        if (currentTimes.IdealTime == -1) {
                            Console.WriteLine("При авторизации произошла ошибка");
                            continue;
                        }

                        if (currentTimes.IdealTime - timeLimit.IdealTime < EPS &&
                            currentTimes.Deviations - timeLimit.DeviationsTime < EPS) {
                            Console.WriteLine("Вы успешно авторизировались");
                        } else {
                            Console.WriteLine("Авторизация не пройдена");
                        }
                    } else {
                        Console.WriteLine("Неизвестный параметр запуска работы программы");
                    }
                } catch {
                    Console.WriteLine("Ошибка выполнения программы");
                }
            }
        }

        static string GetRandomPhraseFromFile(string filename) {
            List<string> phrases = File.ReadAllLines(filename).ToList();
            Random random = new Random();
            return phrases[random.Next(0, phrases.Count)];
        }


        static string GetUsername() {
            Console.Write("Введите ваше имя\n");
            return Console.ReadLine();
        }

        

        static void SaveRegistrationInfo(string username, string phrase, double idealTime, double deviationsTime) {
            string filename = $"C:/Users/user/source/repos/C_Sharp_collection/ZILaba5/ZILaba5/{username}.txt";

            if (File.Exists(filename)) {
                Console.WriteLine("Такой аккаунт уже существует! Данные не будут сохранены");
                return;
            }

            StreamWriter file = File.CreateText(filename);
            file.WriteLine($"User name: {username}");
            file.WriteLine($"Phrase: {phrase}");
            file.WriteLine($"Ideal time: {idealTime}");
            file.WriteLine($"Deviations time: {deviationsTime}");
            file.Close();
        }

        static (double IdealTime, double Deviations) getIdealValueAndDeviations(string phrase) {
            Stopwatch watch = new Stopwatch();
            double[] inputValues = new double[phrase.Length];

            watch.Start();
            int i = 0;
            foreach (var chr in phrase) {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.KeyChar != chr) {
                    Console.WriteLine("\nОшибка при вводе фразы!");
                    return (-1, -1);
                }

                inputValues[i] = watch.ElapsedMilliseconds;
                Console.Write(key.KeyChar);
                i++;
            }
            watch.Stop();
            Console.WriteLine();

            double idealTimes = inputValues.Sum() / phrase.Length;

            for (int j = 0; j < inputValues.Length; j++) {
                inputValues[j] = Math.Abs(inputValues[j] - idealTimes);
            }

            double deviations = inputValues.Sum() / phrase.Length;

            Console.WriteLine($"Время на этой попытке = {idealTimes:F2}мс, а погрешность = {deviations:F2}мс");

            return (idealTimes, deviations);
        }

        // Читаем из файла идеальное время пользователя
        static (double IdealTime, double DeviationsTime) GetTimeLimitForUser(string username) {
            string filename = $"C:/Users/user/source/repos/C_Sharp_collection/ZILaba5/ZILaba5/{username}.txt";
            if (File.Exists(filename)) {
                string[] lines = File.ReadAllLines(filename);
                if (lines.Length >= 3) {
                    int indexl2 = lines[2].IndexOf(":");
                    if (indexl2 != -1) {
                        lines[2] = lines[2].Substring(indexl2 + 1).TrimStart();
                    }

                    int indexl3 = lines[3].IndexOf(":");
                    if (indexl3 != -1) {
                        lines[3] = lines[3].Substring(indexl3 + 1).TrimStart();
                    }

                    double.TryParse(lines[2], out double idealTime);
                    double.TryParse(lines[3], out double deviationsTime);

                    return (idealTime, deviationsTime);
                }
            }

            return (-1, -1);
        }

        static string GetPhraseForUser(string username) {
            string filename = $"C:/Users/user/source/repos/C_Sharp_collection/ZILaba5/ZILaba5/{username}.txt";
            if (File.Exists(filename)) {
                string[] lines = File.ReadAllLines(filename);
                var phrase = lines[1];
                int index = phrase.IndexOf(":");
                if (index != -1) {
                    phrase = phrase.Substring(index + 1).TrimStart();
                } else {
                    phrase = phrase.TrimStart();
                }

                return phrase;
            }
            return "";
        }
    }
}