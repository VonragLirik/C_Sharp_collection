using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


// Решение однородной минимаксной задачи с помощью модифицированной модели выбора

// Каждому заданию рандомно от 0 до 255 прикрепляем ген
// Формируем 3 начальных популяции рандомно распределяя гены и распределяя между процессорами(255/ количество процессоров шаг)
// После каждого распределения считаем минМакс сумму
// Формируем из начальных поколений следующий путем кроссовера + мутации
// Алгоритм заканчивается когда лучшая особь повторяется заданное количество раз(10)
// Вывод в файл

namespace EvrAlgLaba5 {
    class Program {
        static void Main() {
            Console.WriteLine("Решение однородной минимаксной задачи с помощью модифицированной модели выбора");
            Console.WriteLine("Введите количество заданий M");
            int countOfTasks = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество процессоров N");
            int countOfProcessors = int.Parse(Console.ReadLine());
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
                for (int j = 0; j < countOfProcessors; j++) {
                    Console.Write(tasksArray[i] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            // нужно хранить [задачу, ген][], minmax
            Console.WriteLine("Формируем гены для популяции");
            int countOfStartPopulations = 3;
            List<List<List<int>>> populations = new List<List<List<int>>>();
            List<int> minmaxArrays = new List<int>();
            for (int i = 0; i < countOfStartPopulations; i++) {
                // [задача, ген][]
                List<List<int>> population = new List<List<int>>();
                // рандомно формируем гены
                foreach (var task in tasksArray) {
                    List<int> child = new List<int>();
                    child.Add(task);
                    child.Add(random.Next(0, 255));
                    Console.WriteLine("Задание = " + child[0] +  ", Ген = " + child[1]);
                    population.Add(child);
                }

                // распределяем между процессорами гены
                // ищем делитель чтобы раскидывать между процессорами
                int divider = (int)Math.Ceiling((double)255 / countOfProcessors);
                List<int> sumProcessors = new List<int>();
                List<List<int>> processors = new List<List<int>>();
                for (int j = 0; j < countOfProcessors; j++) {
                    sumProcessors.Add(0);
                    processors.Add(new List<int>());
                }
                foreach (var child in population) {
                    int indexOfProcessor = (int)Math.Floor((double)child[1] / divider);
                    processors[indexOfProcessor].Add(child[0]);
                    sumProcessors[indexOfProcessor] += child[0];
                }
                minmaxArrays.Add(sumProcessors.Max());
                populations.Add(population);

                foreach (var (processorTasks, processorNumber) in processors.WithIndex()) {
                    Console.WriteLine("P" + processorNumber + ": " + String.Join(", ", processorTasks));
                }
                Console.WriteLine("minmax = "+ sumProcessors.Max());
                Console.WriteLine("\n\n");
            }

            Console.WriteLine("Формируем из начального поколения следующее поколение");
        }

        static void WriteLine(string line) {
            File.AppendAllText("C:/Users/user/source/repos/EvrAlgLaba5/EvrAlgLaba5/output.txt", line);
        }
    }
}

static class MyExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> array) =>
        array.Select((item, index) => (item, index));
}
