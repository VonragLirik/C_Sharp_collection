using System;
using System.Collections.Generic;
using System.Linq;

// Алгоритм построения расписания с произвольной загрузкой

namespace evrAlglaba2 {
    class Program {
        static void Main() {
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
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
                    if(sumInTasks == sumItem) {
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
            for(int i=0; i< countOfProcessors; i++) {
                resultArr.Add(0);
            }
            foreach (var oneLine in sortedTasksArray) {
                for(int i = 0; i < countOfProcessors; i++) {
                    oneLine.Value[i] += resultArr[i];
                }
                int minValueInString = oneLine.Value.Min();
                int indexOfMinValue = oneLine.Value.IndexOf(minValueInString);
                resultArr[indexOfMinValue] = minValueInString;
            }
            Console.WriteLine("\n max(" + string.Join(" ,", resultArr) + ") = " + resultArr.Max());
            resultArr.Add(0);
        }
    }
}
