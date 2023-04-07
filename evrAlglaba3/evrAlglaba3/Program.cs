using System;
using System.Collections.Generic;
using System.Linq;

// Метод Крона и две модификации

namespace evrAlglaba3 {
    class Program {
        static void Main() {
            // CronInput();
            // Modification1();
            Modification2();
        }

        static void CronInput() {
            Console.WriteLine("Алгоритм ");
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу массива t1");
            int valueMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите верхнюю границу массива t2");
            int valueMax = int.Parse(Console.ReadLine());

            List<List<int>> basicArray = new List<List<int>>();

            // Создаем рандомные значения и создаем однородную матрицу
            Random random = new Random();
            for (int i = 0; i < countOfTasks; i++) {
                List<int> array = new List<int>();
                int randomedValue = random.Next(valueMin, valueMax);
                for (int j = 0; j < countOfProcessors; j++) {
                    array.Add(randomedValue);
                }
                basicArray.Add(array);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Исходная матрица:");
            foreach (var oneLine in basicArray) {
                Console.WriteLine(string.Join(" ", oneLine));
            }

            // инициализируем процессоры
            List<List<int>> processorArray = new List<List<int>>();
            for (int i = 0; i < countOfTasks; i++) {
                processorArray.Add(new List<int>());
            }

            // заполняем рандомно процессоры
            for (int i = 0; i < countOfTasks; i++) {
                int newRandomProc = random.Next(0, countOfProcessors);
                processorArray[newRandomProc].Add(basicArray[i][0]);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Матрица с заполненными процессорами:");
            PrintArray(processorArray);
            Cron(countOfProcessors, processorArray);
        }

        static void Cron(int countOfProcessors, List<List<int>> processorArray) {
            bool isEndStep1 = false;

            // переходим к шагу 1
            while (!isEndStep1) {
                // Находим сумму по каждому процессору
                List<int> sumArray = new List<int>();
                for (int i = 0; i < countOfProcessors; i++) {
                    sumArray.Add(processorArray[i].Sum());
                }

                Console.Write("sumArray " + string.Join(" ", sumArray) + "\n");

                // Считаем дельту
                int delta = sumArray.Max() - sumArray.Min();
                Console.WriteLine("Delta: " + delta);

                // Находим минимальный индекс и максимальный индекс процессора
                int minSumIndexArrayValue = sumArray.Min();
                List<int> minSumIndexArray = sumArray.FindAllIndexof(minSumIndexArrayValue);
                int maxSumIndexArrayValue = sumArray.Max();
                int maxSumIndexArray = sumArray.FindIndex(0, (val) => val == maxSumIndexArrayValue);

                bool found = false;

                // перекидываем из процессора где больше всего сумма и элемент больше дельты в меньший процессор
                foreach (var minSumIndex in minSumIndexArray) {
                    for (int i = 0; i < processorArray[maxSumIndexArray].Count; i++) {
                        if (processorArray[maxSumIndexArray][i] < delta) {
                            processorArray[minSumIndex].Add(processorArray[maxSumIndexArray][i]);
                            processorArray[maxSumIndexArray].RemoveRange(i, 1);
                            found = true;
                            break;
                        }
                    }
                }

                for(int i=0; i < processorArray.Count(); i++) {
                    processorArray[i] = processorArray[i].OrderByDescending(t => t).ToList();
                }

                PrintArray(processorArray);

                if (!found) {
                    isEndStep1 = true;
                    Console.WriteLine("-----------------------------------");
                }

            }





            Console.WriteLine("\n");
            Console.WriteLine("Начинаем шаг два");

            bool isEndStep2 = false;

            while (!isEndStep2) {
                // Находим сумму по каждому процессору
                List<int> sumArray = new List<int>();
                for (int i = 0; i < countOfProcessors; i++) {
                    sumArray.Add(processorArray[i].Sum());
                }

                Console.Write("sumArray " + string.Join(" ", sumArray) + "\n");

                // Считаем дельту
                int delta = sumArray.Max() - sumArray.Min();
                Console.WriteLine("Delta: " + delta);

                // Находим минимальный индекс и максимальный индекс процессора
                int minSumIndexArrayValue = sumArray.Min();
                List<int> minSumIndexArray = sumArray.FindAllIndexof(minSumIndexArrayValue);
                int maxSumIndexArrayValue = sumArray.Max();
                int maxSumIndexArray = sumArray.FindIndex(0, (val) => val == maxSumIndexArrayValue);

                bool found = false;

                // перекидываем из процессора где больше всего сумма и разница с меньшим элементом процессора больше дельты в меньший процессор
                foreach (var minSumIndex in minSumIndexArray) {
                    // по максимальному столбцу 
                    for (int i = 0; i < processorArray[maxSumIndexArray].Count; i++) {
                        // по минимальному столбцу
                        for (int j = 0; j < processorArray[minSumIndex].Count; j++) {
                            if (processorArray[maxSumIndexArray][i] - processorArray[minSumIndex][j] > 0 && ((processorArray[maxSumIndexArray][i] - processorArray[minSumIndex][j]) < delta)) {
                                int tmpMinVal = processorArray[minSumIndex][j];
                                processorArray[minSumIndex][j] = processorArray[maxSumIndexArray][i];
                                processorArray[maxSumIndexArray][i] = tmpMinVal;
                                found = true;
                                break;
                            }
                        }
                        if (found) {
                            break;
                        }
                    }
                }

                for (int i = 0; i < processorArray.Count(); i++) {
                    processorArray[i] = processorArray[i].OrderByDescending(t => t).ToList();
                }

                PrintArray(processorArray);
                Console.WriteLine();

                if (!found) {
                    Console.ReadLine();
                    isEndStep2 = true;
                }

            }

            List<int> resultSumArray = new List<int>();
            for (int i = 0; i < countOfProcessors; i++) {
                resultSumArray.Add(processorArray[i].Sum());
            }
            Console.WriteLine("max( " + string.Join(", ", resultSumArray) + " ) = " + resultSumArray.Max());
        }

        static void Modification1() {
            Console.WriteLine("Модификация 1");
            Console.WriteLine("Начальное состояние из 1 лабы(критического пути)");
            CMPInput();
        }

        static void CMPInput() {
            //Console.WriteLine("Введите количество процессоров N");
            //int countOfProcessors = int.Parse(Console.ReadLine());
            int countOfProcessors = 3;
            //Console.WriteLine("Введите количество заданий M");
            //int countOfTasks = int.Parse(Console.ReadLine());
            int countOfTasks = 11;
            //Console.WriteLine("Введите нижнюю границу массива t1");
            //int valueMin = int.Parse(Console.ReadLine());
            //Console.WriteLine("Введите верхнюю границу массива t2");
            //int valueMax = int.Parse(Console.ReadLine());

            List<int> tasksArray = new List<int>();

            // Создаем рандомные значения
            Random random = new Random();

            // for (int i = 0; i < countOfTasks; i++) {
            //tasksArray.Add(random.Next(valueMin, valueMax));
            // }
            tasksArray.Add(16);
            tasksArray.Add(12);
            tasksArray.Add(16);
            tasksArray.Add(16);
            tasksArray.Add(16);
            tasksArray.Add(16);
            tasksArray.Add(14);
            tasksArray.Add(13);
            tasksArray.Add(15);
            tasksArray.Add(17); 
            tasksArray.Add(12);

            Console.WriteLine("\n");
            Console.WriteLine("Исходная матрица");
            for (int i = 0; i < tasksArray.Count; i++) {
                for (int j = 0; j < countOfProcessors; j++) {
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

            List<List<int>> processorArray = new List<List<int>>();
            for (int i = 0; i < countOfProcessors; i++) {
                processorArray.Add(new List<int>());
            }
            for (int index = 0; index < sortedTasksArray.Count; index++) {
                int task = sortedTasksArray[index];

                // находим процессор с минимальной суммой
                List<int> sumArray = new List<int>();
                for (int i = 0; i < countOfProcessors; i++) {
                    sumArray.Add(processorArray[i].Sum());
                }
                int minSumIndexArrayValue = sumArray.Min();
                int minValueIndex = sumArray.IndexOf(minSumIndexArrayValue);

                processorArray[minValueIndex].Add(task);
            }

            Console.WriteLine("Распределенная матрица");
            PrintArray(processorArray);

            Cron(countOfProcessors, processorArray);
        }

        // случайно генерируем матрицу, в процессоры добавляем строго в порядке убывания
        static void Modification2() {
            Console.WriteLine("Модификация по убыванию");
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу массива t1");
            int valueMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите верхнюю границу массива t2");
            int valueMax = int.Parse(Console.ReadLine());

            List<List<int>> basicArray = new List<List<int>>();

            // Создаем рандомные значения и создаем однородную матрицу
            Random random = new Random();
            for (int i = 0; i < countOfTasks; i++) {
                List<int> array = new List<int>();
               
                for (int j = 0; j < countOfProcessors; j++) {
                    array.Add(random.Next(valueMin, valueMax));
                }
                basicArray.Add(array);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Исходная матрица:");
            foreach (var oneLine in basicArray) {
                Console.WriteLine(string.Join(" ", oneLine));
            }

            // сортируем по убыванию
            List<int> sortedBasicArrayLine = basicArray.Select(t => t.First()).OrderByDescending(t => t).ToList();
            List<List<int>> sortedBasicArray = new List<List<int>>();
            foreach (var sortedLine in sortedBasicArrayLine) {
                List<int> array = new List<int>();
                for (int j = 0; j < countOfProcessors; j++) {
                    array.Add(sortedLine);
                }
                sortedBasicArray.Add(array);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Отсортированная матрица:");
            PrintArray(sortedBasicArray);

            // инициализируем процессоры
            List<List<int>> processorArray = new List<List<int>>();
            for (int i = 0; i < countOfTasks; i++) {
                processorArray.Add(new List<int>());
            }

            // заполняем рандомно процессоры
            for (int i = 0; i < countOfTasks; i++) {
                int newRandomProc = random.Next(0, countOfProcessors);
                processorArray[newRandomProc].Add(sortedBasicArray[i][0]);
            }

            Console.WriteLine("\n");
            Console.WriteLine("Матрица с заполненными процессорами:");
            PrintArray(processorArray);
            Cron(countOfProcessors, processorArray);
        }

        static void PrintArray(List<List<int>> tasksArray) {
            for (int i = 0; i < tasksArray.Count(); i++) {
                if (tasksArray[i].Count > 0) {
                    Console.WriteLine("P" + (i + 1) + " " + string.Join(" ", tasksArray[i]));
                }
            }
            Console.WriteLine();
        }


    }
    public static class EM {
        public static List<int> FindAllIndexof(this List<int> values, int val) {
            return values.Select((b, i) => b == val ? i : -1).Where(i => i != -1).ToList();
        }
    }
}
