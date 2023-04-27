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
            Console.WriteLine("Введите количество начальных популяций");
            int countOfStartPopulations = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество повторений без изменений для выхода");
            int countOfMutationWithoutChange = int.Parse(Console.ReadLine());

            StreamWriter writter = new StreamWriter("C:/Users/user/source/repos/C_Sharp_collection/EvrAlgLaba5/EvrAlgLaba5/output.txt");
            Console.SetOut(writter);
            List<int> tasksArray = new List<int>();
            int counterOfChilds = 0;

            // Создаем рандомные значения
            Random random = new Random();

            for (int i = 0; i < countOfTasks; i++) {
                tasksArray.Add(random.Next(valueMin, valueMax));
            }

            Console.WriteLine("Исходная матрица");
            for (int i = 0; i < tasksArray.Count; i++) {
                for (int j = 0; j < countOfProcessors; j++) {
                    Console.Write(tasksArray[i] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            // нужно хранить [задачу, ген][], minmax
            Console.WriteLine("Формируем гены для популяции\n");
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
                    Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    population.Add(child);
                }

                int maxSumInProcessors = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, population);
                minmaxArrays.Add(maxSumInProcessors);
                populations.Add(population);
            }

            Console.WriteLine("Minmax начальной особи:\n" + string.Join(", ", minmaxArrays));
            Console.WriteLine("Минимум = " + minmaxArrays.Min());

            Console.WriteLine("Формируем из начального поколения следующее поколение");
            // Convert.ToString(decimalNumber, 2)
            // делаем кроссовер + мутацию
            // кроссовер - объединение двух особей по рандомному символу в пределах от 1 до countOfTasks
            // мутация - перевод рандомного числа из объединенных строчек в двоичную систему и реверс символа на обратный
            // после этого проверка minmax, если стало больше, то не менем, иначе меняем.
            bool isChangedParent = false;
            int counterOfChangesWithoutMutation = 0;
            while (counterOfChangesWithoutMutation < countOfMutationWithoutChange) {
                Console.WriteLine("Поколение номер " + (counterOfChangesWithoutMutation+1));
                List<List<int>> newPopulation = new List<List<int>>();
                List<int> minmaxTmpArray = new List<int>();
                int indexOfFirstChild = random.Next(0, countOfStartPopulations);
                int indexOfSecondChild = indexOfFirstChild;
                minmaxTmpArray.Add(minmaxArrays[indexOfFirstChild]);
                // пока второй индекс не будет равен 
                while (indexOfFirstChild == indexOfSecondChild) {
                    indexOfSecondChild = random.Next(0, countOfStartPopulations);
                }
                int randomSplitIndex = random.Next(1, countOfTasks - 1);

                bool shouldBeCrossover = random.Next(0, 100) < 80;
                if (shouldBeCrossover) {
                    Console.WriteLine("Кроссовер происходит с особями под номерами: " + (indexOfFirstChild + 1) + " и " + (indexOfSecondChild + 1));
                    Console.WriteLine("Точка разбиения = " + (randomSplitIndex + 1));
                    Console.WriteLine("Первая начальная особь, minmax = " + minmaxArrays[indexOfFirstChild]);
                    foreach (var child in populations[indexOfFirstChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    Console.WriteLine("\nВторая начальная особь, minmax = " + minmaxArrays[indexOfSecondChild]);
                    foreach (var child in populations[indexOfSecondChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }

                    // если индекс текущего элемента меньше разделителя то добавляем из первой особи, иначе из второй
                    for (int i = 0; i < countOfTasks; i++) {
                        if (i < randomSplitIndex) {
                            newPopulation.Add(populations[indexOfFirstChild][i].ToList<int>());
                        } else {
                            newPopulation.Add(populations[indexOfSecondChild][i].ToList<int>());
                        }
                    }
                    Console.WriteLine("\nПосле кроссовера получилось следующее поколение:");
                    foreach (var child in newPopulation) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                } else {
                    Console.WriteLine("!!!!!Кроссовер не происходит, берем родителя");
                    newPopulation = populations[indexOfFirstChild];
                }

                //////////////кроссовер закончился, создана проверяемая популяция, переходим к мутации
                AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newPopulation);
                MutationProcess(countOfTasks, newPopulation);
                int maxSumInProcessorsNewPopulation = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newPopulation);
                minmaxTmpArray.Add(maxSumInProcessorsNewPopulation);



                ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////делаем кроссовер наоборот
                Console.WriteLine("Кроссовер теперь происходит с особями в обратном порядке под номерами: " + (indexOfSecondChild + 1) + " и " + (indexOfFirstChild + 1));
                Console.WriteLine("Первая начальная особь, minmax = " + minmaxArrays[indexOfSecondChild]);
                foreach (var child in populations[indexOfSecondChild]) {
                    Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                }
                Console.WriteLine("\nВторая начальная особь, minmax = " + minmaxArrays[indexOfFirstChild]);
                foreach (var child in populations[indexOfFirstChild]) {
                    Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                }
                List<List<int>> newSecondPopulation = new List<List<int>>();
                bool shouldBeCrossover2 = random.Next(0, 100) < 80;
                if (shouldBeCrossover2) {
                    for (int i = 0; i < countOfTasks; i++) {
                        if (i > randomSplitIndex) {
                            newSecondPopulation.Add(populations[indexOfFirstChild][i].ToList<int>());
                        } else {
                            newSecondPopulation.Add(populations[indexOfSecondChild][i].ToList<int>());
                        }
                    }
                    Console.WriteLine("\nПосле кроссовера получилось следующее поколение:");
                    foreach (var child in newSecondPopulation) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                } else {
                    Console.WriteLine("!!!!!Кроссовер не происходит, берем родителя");
                    newSecondPopulation = populations[indexOfFirstChild];
                }
                AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newSecondPopulation);
                MutationProcess(countOfTasks, newSecondPopulation);
                int maxSumInProcessorsNewSecondPopulation = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newSecondPopulation);
                minmaxTmpArray.Add(maxSumInProcessorsNewSecondPopulation);

                int indexOfMinResult = minmaxTmpArray.FindIndex(m => m == minmaxTmpArray.Min());
                Console.WriteLine("\nЛучшая особь под номером - " + (indexOfMinResult + 1));
                
                // если меньшая не родительская, то меняем
                if (indexOfMinResult != 0) {
                    minmaxArrays[indexOfFirstChild] = indexOfMinResult == 1 ? maxSumInProcessorsNewPopulation : maxSumInProcessorsNewSecondPopulation;
                    populations[indexOfFirstChild] = indexOfMinResult == 1 ? newPopulation : newSecondPopulation;
                    isChangedParent = true;
                }

                if (isChangedParent) {
                    counterOfChangesWithoutMutation = 0;
                    isChangedParent = false;
                    Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!Популяция изменилась!!");
                    Console.WriteLine("Стала особь с минмаксом: " + minmaxArrays[indexOfFirstChild]);
                    foreach (var child in populations[indexOfFirstChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    Console.WriteLine("\n!!!!!Минимальная популяция:");
                    Console.WriteLine("********************************************************************************");
                    WriteMinPopulation(minmaxArrays, populations);
                    Console.WriteLine("********************************************************************************");
                    Console.WriteLine("\n\n\n\t\tНачинаем заново");
                } else {
                    counterOfChangesWithoutMutation++;
                }
                counterOfChilds++;
                Console.WriteLine("\nномер поколения = " + counterOfChilds);
            }

            ////////// подводим итог
            Console.WriteLine("\n\n\n/////////////////////////////////////////////////////////////////////////////////////////////");
            Console.WriteLine("\nПолучилось всего поколений: " + counterOfChilds);
            Console.WriteLine("\nПолучились minmax популяций:");
            Console.WriteLine(string.Join(", ", minmaxArrays));
            WriteMinPopulation(minmaxArrays, populations);
            writter.Close();
        }

        static int AddTasksToProcessorsAndFindMaxValue(int countOfProcessors, List<List<int>> population) {
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
                int workNumber = child[1];
                if(workNumber == 0) {
                    workNumber = 1;
                }
                if(workNumber == 255) {
                    workNumber = 254;
                }
                int indexOfProcessor = (int)Math.Floor((double)workNumber / divider);
                processors[indexOfProcessor].Add(child[0]);
                sumProcessors[indexOfProcessor] += child[0];
            }

            foreach (var (processorTasks, processorNumber) in processors.WithIndex()) {
                Console.WriteLine("P" + processorNumber + ": " + String.Join(", ", processorTasks));
            }
            Console.WriteLine("minmax = " + sumProcessors.Max());
            Console.WriteLine("\n");

            return sumProcessors.Max();
        }

        static void MutationProcess(int countOfTasks, List<List<int>> population) {
            Random random = new Random();
            bool shouldBeMutation = random.Next(0, 100) < 80;
            if (shouldBeMutation) {
                int randomIndexForMutation = random.Next(1, countOfTasks - 1);
                // [задача, ген][]
                int workGen = population[randomIndexForMutation][1];
                string workGenTo2 = Convert.ToString(workGen, 2);
                if (workGenTo2.Length != 8) {
                    string zeroString = "";
                    for (int ind = 0; ind < (8 - workGenTo2.Length); ind++) {
                        zeroString += "0";
                    }

                    workGenTo2 = zeroString + workGenTo2;
                }
                int randomIndexForMutationInvert = random.Next(0, 7);
                string mutatedSymbol = workGenTo2[randomIndexForMutationInvert] == '0' ? "1" : "0";
                string changedWorkGenTo2 = workGenTo2.Substring(0, randomIndexForMutationInvert) + mutatedSymbol + workGenTo2.Substring(randomIndexForMutationInvert + 1);
                int changedGen = Convert.ToInt32(changedWorkGenTo2, 2);
                Console.WriteLine("Мутация: было задание " + population[randomIndexForMutation][0] + " ген = " + workGen + " стал ген = " + changedGen);
                population[randomIndexForMutation][1] = changedGen;
            } else {
                Console.WriteLine("!!!!!Мутация не произошла, ничего не изменилось");
            }
        }

        static void WriteMinPopulation(List<int> minmaxArrays, List<List<List<int>>> populations) {
            int minIndexSumArray = minmaxArrays.FindIndex(a => a == minmaxArrays.Min());
            Console.WriteLine("\nЛучшее поколение номер " + (minIndexSumArray + 1) + ", minmax = " + minmaxArrays[minIndexSumArray]);
            foreach (var child in populations[minIndexSumArray]) {
                Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
            }
        }
        }
}

static class MyExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> array) =>
        array.Select((item, index) => (item, index));
}
