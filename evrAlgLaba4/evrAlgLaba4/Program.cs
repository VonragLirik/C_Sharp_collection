using System;
using System.Collections.Generic;
using System.Linq;

// Алгоритм Плотникова-Зверева по крадратичному аргументу

namespace evrAlgLaba4 {
    class Program {
        static void Main() {
            Console.WriteLine("Алгоритм Плотникова-Зверева по крадратичному аргументу");
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу массива t1");
            int valueMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите верхнюю границу массива t2");
            int valueMax = int.Parse(Console.ReadLine());

            Dictionary<string, List<int>> tasksArray = new Dictionary<string, List<int>>();
            List<int> sumArray = new List<int>();

            // Создаем рандомные значения
            Random random = new Random();

            for (int i = 0; i < countOfTasks; i++) {
                List<int> array = new List<int>();
                for (int j = 0; j < countOfProcessors; j++) {
                    array.Add(random.Next(valueMin, valueMax));
                }
                sumArray.Add(array.Sum());
                tasksArray.Add(array.Sum() + " - " + i, array);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Исходная матрица:");
            foreach (var oneLine in tasksArray) {
                Console.Write(string.Join(" ", oneLine.Value) + "\n");
            }
            Console.WriteLine("\n");

            //Сортируем по убыванию
            List<int> sortedSumArray = sumArray.OrderByDescending(t => t).ToList();
            Dictionary<string, List<int>> sortedTasksArray = new Dictionary<string, List<int>>();
            foreach (var sumItem in sortedSumArray) {
                foreach (var oneLine in tasksArray) {
                    int sumInTasks = int.Parse(oneLine.Key.Split(" - ")[0]);
                    if (sumInTasks == sumItem) {
                        sortedTasksArray.Add(oneLine.Key, oneLine.Value);
                        tasksArray.Remove(oneLine.Key);
                        break;
                    }
                }
            }
            Console.WriteLine("Отсортированная матрица:");
            foreach (var oneLine in sortedTasksArray) {
                Console.Write(string.Join(" ", oneLine.Value) + "\n");
            }
            Console.WriteLine("\n");
            Console.ReadLine();

            List<int> resultArr = new List<int>();
            for (int i = 0; i < countOfProcessors; i++) {
                resultArr.Add(0);
            }
            foreach (var oneLine in sortedTasksArray) {
                List<int> oneLineCopy = new List<int>();
                for (int i = 0; i < countOfProcessors; i++) {
                    int workProcessIndex = i;
                    oneLineCopy.Add(oneLine.Value[i]);
                    int squareValue = resultArr[i] + oneLine.Value[i];
                    oneLine.Value[i] = square(squareValue);
                    for (int index = 0; index < countOfProcessors; index++) { 
                        if(index != workProcessIndex) {
                            oneLine.Value[i] += square(resultArr[index]);
                        }
                    }
                    Console.WriteLine("\n T" + i + "=" + oneLine.Value[i]);
                }
                int minValueInString = oneLine.Value.Min();
                int indexOfMinValue = oneLine.Value.IndexOf(minValueInString);
                resultArr[indexOfMinValue] += oneLineCopy[indexOfMinValue];
            }
            Console.WriteLine("\n max(" + string.Join(", ", resultArr) + ") = " + resultArr.Max());
        }

        static int square(int value) {
            return value * value;
        }
    }
}
