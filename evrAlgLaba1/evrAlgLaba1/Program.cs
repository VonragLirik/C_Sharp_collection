using System;
using System.Collections.Generic;
using System.Linq;

/*
    Алгорит критического пути.
    Алгорит половинного деления задач между процессорами.
    Вводим N, M. N - число процессов
    Нужно отсортировать по убыванию
 */

namespace evrAlgLaba1 {
    class Program {
        static void Main() {
            CMPInput();
            //HDMT();
        }

        static void CMPInput() {
            Console.WriteLine("Алгоритм CMPInput");
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу массива t1");
            int valueMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите верхнюю границу массива t2");
            int valueMax = int.Parse(Console.ReadLine());

            List<int> tasksArray = new List<int>();

            // Создаем рандомные значения
            Random random = new Random();

            for (int i = 0; i < countOfTasks; i++) {
                tasksArray.Add(random.Next(valueMin, valueMax));
            }

            Console.WriteLine("\n");
            Console.WriteLine("Исходная матрица");
            for (int i = 0; i < tasksArray.Count; i++) {
                for(int j = 0; j < countOfProcessors; j++) {
                    Console.Write(tasksArray[i] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            CMP(tasksArray, countOfProcessors);
        }

        static void CMP(List<int> tasksArray, int countOfProcessors) {

            //Сортируем по убыванию
            List<int> sortedTasksArray = tasksArray.OrderByDescending(t => t).ToList();

            Console.WriteLine("Отсортированная матрица");
            for (int i = 0; i < sortedTasksArray.Count; i++) {
                for (int j = 0; j < countOfProcessors; j++) {
                    Console.Write(sortedTasksArray[i] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            Console.WriteLine("Множество заданий: ");
            Console.WriteLine("T = [" + String.Join(", ", sortedTasksArray), "]\n\n");

            Console.WriteLine("Нажмите на любую клавишу для продолжения");
            Console.ReadLine();

            List<int> tasksOptimizationArray = new List<int>(new int[countOfProcessors]);
            for (int index = 0; index < sortedTasksArray.Count; index++) {
                int task = sortedTasksArray[index];
                // Два варианта - массив пустой или не пустой
                // Если не пустой - поиск минимального и прибавление туда
                if (index < countOfProcessors) {
                    tasksOptimizationArray[index] += task;
                } else {
                    int minValueIndex = FindMinValueIndexArray(tasksOptimizationArray);
                    tasksOptimizationArray[minValueIndex] += task;
                }
            }

            // Выводим результат
            Console.Write("max(" + String.Join(", ", tasksOptimizationArray) + ") = " + FindMaxValueInArray(tasksOptimizationArray) + "\n");
        }

        //static void HDMT() {
        //    Console.WriteLine("Алгоритм HDMT");
        //    Console.WriteLine("Введите количество процессоров N");
        //    int countOfProcessors = int.Parse(Console.ReadLine());
        //    if (countOfProcessors % 2 != 0) {
        //        Console.WriteLine("Количество процессоров должно быть кратно двум!");
        //        Environment.Exit(2);
        //    }
        //    Console.WriteLine("Введите количество заданий");
        //    int countOfTasks = int.Parse(Console.ReadLine());
        //    Console.WriteLine("Введите нижнюю границу массива");
        //    int valueMin = int.Parse(Console.ReadLine());
        //    Console.WriteLine("Введите верхнюю границу массива");
        //    int valueMax = int.Parse(Console.ReadLine());

        //    List<int> tasksArray = new List<int>();

        //    // Создаем рандомные значения
        //    Random random = new Random();

        //    for (int i = 0; i < countOfTasks; i++) {
        //        tasksArray.Add(random.Next(valueMin, valueMax));
        //    }
        //    //Сортируем по убыванию

        //    List<int> arrayA = new List<int>();
        //    List<int> arrayB = new List<int>();
        //    for (int index = 0; index < sortedTasksArray.Count; index++) {
        //        int task = sortedTasksArray[index];
        //        // Если в первом массиве сумма больше чем во втором, то записываем во второй, иначе в первый
        //        if (arrayA.Sum() < arrayB.Sum()) {
        //            arrayA.Add(task);
        //        } else {
        //            arrayB.Add(task);
        //        }
        //    }

        //    // Выводим результат
        //    CMP(arrayA, countOfProcessors / 2);
        //    CMP(arrayB, countOfProcessors / 2);
        //}

        static int FindMinValueIndexArray(List<int> array) {
            int minValue = 100000000;
            int minIndex = 0;

            for (int i = 0; i < array.Count; i++) {
                if (array[i] < minValue) { minValue = array[i]; minIndex = i; }
            }
            return minIndex;
        }

        static int FindMaxValueInArray(List<int> array) {
            int maxValue = 0;

            for (int i = 0; i < array.Count; i++) {
                if (array[i] > maxValue) { maxValue = array[i]; }
            }
            return maxValue;
        }
    }
}
