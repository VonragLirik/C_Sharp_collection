using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


// Решение однородной минимаксной задачи с помощью модифицированной модели выбора(генетический алгоритм)

// Каждому заданию рандомно от 0 до 255 прикрепляем ген
// Формируем 3 начальных популяции рандомно распределяя гены и распределяя между процессорами(255/ количество процессоров шаг)
// После каждого распределения считаем минМакс сумму
// Формируем из начальных поколений следующий путем кроссовера + мутации
// Алгоритм заканчивается когда лучшая особь повторяется заданное количество раз(10)
// Вывод в файл

/*
отличие 6 лабы от 5
1. методички нет
2. матрица не однородная
3. кроссовер двухточечный(середина из второй особи)
4. мутация двухточечная(меняются местами два бита)
5. выбрали лучшую особь, добавляем её в массив вторых особей, основная популяция не меняется. 
   Два массива, когда их становится одинаковое количество объединяем, сортируем по минмаксу и берем лучшие.
*/

namespace EvrAlgLaba6 {
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

            StreamWriter writter = new StreamWriter("C:/Users/user/source/repos/C_Sharp_collection/EvrAlgLaba6/EvrAlgLaba6/output.txt");
            Console.SetOut(writter);
            List<List<int>> tasksArray = new List<List<int>>();
            int counterOfChilds = 0;

            // Создаем рандомные значения
            Random random = new Random();

            for (int i = 0; i < countOfTasks; i++) {
                List<int> lineTaskArray = new List<int>();
                for (int j = 0; j < countOfProcessors; j++) {
                    lineTaskArray.Add(random.Next(valueMin, valueMax));
                }
                tasksArray.Add(lineTaskArray);
            }

            Console.WriteLine("Исходная матрица");
            for (int i = 0; i < tasksArray.Count; i++) {
                for (int j = 0; j < countOfProcessors; j++) {
                    Console.Write(tasksArray[i][j] + " ");
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
                foreach (var taskLine in tasksArray) {
                    List<int> child = new List<int>();
                    int gen = random.Next(0, 255);
                    int indexOfTask = GetIndexOfProcessorByGen(countOfProcessors, gen);
                    child.Add(taskLine[indexOfTask]);
                    child.Add(gen);
                    Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    population.Add(child);
                }

                int maxSumInProcessors = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, population);
                minmaxArrays.Add(maxSumInProcessors);
                populations.Add(population);
            }

            Console.WriteLine("Minmax начального поколения:\n" + string.Join(", ", minmaxArrays));
            Console.WriteLine("Минимум = " + minmaxArrays.Min());
            Console.WriteLine("********************************************************************************\n\n\n");

            Console.WriteLine("Формируем из начального поколения следующее поколение");
            // делаем кроссовер + мутацию
            // кроссовер - объединение двух особей по рандомному символу в пределах от 1 до countOfTasks
            // мутация - перевод рандомного числа из объединенных строчек в двоичную систему и реверс символа на обратный
            // после этого проверка minmax, если стало больше, то не менем, иначе меняем.
            int counterOfChangesWithoutMutation = 0;
            int counterOfIterationBeforeConcat = 0;
            List<List<List<int>>> newPopulations = new List<List<List<int>>>();
            List<int> newPopulationMinmaxArrays = new List<int>();
            while (counterOfChangesWithoutMutation < countOfMutationWithoutChange) {
                Console.WriteLine("Поколение номер " + (counterOfChangesWithoutMutation + 1) + "\n");
                for (int index = 0; index < countOfStartPopulations; index++) {
                    Console.WriteLine("\nПоколение в итерации перед конкатенацией номер " + (counterOfIterationBeforeConcat + 1) + "\n");
                    List<int> minmaxTmpArray = new List<int>();
                    int indexOfFirstChild = random.Next(0, countOfStartPopulations);
                    int indexOfSecondChild = indexOfFirstChild;
                    minmaxTmpArray.Add(minmaxArrays[indexOfFirstChild]);
                    // пока второй индекс не будет равен 
                    while (indexOfFirstChild == indexOfSecondChild) {
                        indexOfSecondChild = random.Next(0, countOfStartPopulations);
                    }
                    int firstRandomSplitIndex = random.Next(1, countOfTasks - 1);
                    int secondRandomSplitIndex = firstRandomSplitIndex;
                    // пока второй индекс не будет уникальным 
                    while (firstRandomSplitIndex == secondRandomSplitIndex) {
                        secondRandomSplitIndex = random.Next(1, countOfTasks - 1);
                    }
                    Console.WriteLine("\nКроссовер происходит с особями под номерами: " + (indexOfFirstChild + 1) + " и " + (indexOfSecondChild + 1));
                    Console.WriteLine("Первая начальная особь, minmax = " + minmaxArrays[indexOfFirstChild]);
                    foreach (var child in populations[indexOfFirstChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    Console.WriteLine("\nВторая начальная особь, minmax = " + minmaxArrays[indexOfSecondChild]);
                    foreach (var child in populations[indexOfSecondChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    List<List<int>> newPopulation = CrossoverProcess(countOfTasks, new List<List<int>>(), firstRandomSplitIndex, secondRandomSplitIndex, indexOfFirstChild, indexOfSecondChild, populations);
                    //////////////кроссовер закончился, создана проверяемая популяция, переходим к мутации
                    AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newPopulation);
                    MutationProcess(countOfTasks, newPopulation, countOfProcessors, tasksArray);
                    int maxSumInProcessorsNewPopulation = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newPopulation);
                    minmaxTmpArray.Add(maxSumInProcessorsNewPopulation);



                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////делаем кроссовер наоборот
                    Console.WriteLine("Кроссовер теперь происходит в обратном порядке с особями под номерами: " + (indexOfSecondChild + 1) + " и " + (indexOfFirstChild + 1));
                    Console.WriteLine("\nПервая начальная особь, minmax = " + minmaxArrays[indexOfSecondChild]);
                    foreach (var child in populations[indexOfSecondChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    Console.WriteLine("\nВторая начальная особь, minmax = " + minmaxArrays[indexOfFirstChild]);
                    foreach (var child in populations[indexOfFirstChild]) {
                        Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                    }
                    List<List<int>> newSecondPopulation = CrossoverProcess(countOfTasks, new List<List<int>>(), firstRandomSplitIndex, secondRandomSplitIndex, indexOfSecondChild, indexOfFirstChild, populations);
                    AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newSecondPopulation);
                    MutationProcess(countOfTasks, newSecondPopulation, countOfProcessors, tasksArray);
                    int maxSumInProcessorsNewSecondPopulation = AddTasksToProcessorsAndFindMaxValue(countOfProcessors, newSecondPopulation);
                    minmaxTmpArray.Add(maxSumInProcessorsNewSecondPopulation);

                    int indexOfMinResult = minmaxTmpArray.FindIndex(m => m == minmaxTmpArray.Min());
                    Console.WriteLine("\nЛучшая особь под номером - " + (indexOfMinResult + 1));

                    // в список новых особей добавляем лучшую особь
                    List<List<int>> bestPopulation = indexOfMinResult == 1 ? newPopulation : indexOfMinResult == 2 ? newSecondPopulation : populations[indexOfFirstChild];
                    newPopulations.Add(bestPopulation);
                    newPopulationMinmaxArrays.Add(minmaxTmpArray.Min());
                    counterOfIterationBeforeConcat++;
                }
                counterOfIterationBeforeConcat = 0;

                // склеиваем два массива, сортируем по минмакс и подменяем на популяции
                List<List<List<int>>> concatedPopulations = new List<List<List<int>>>();
                List<string> concatedMinmaxArrays = new List<string>();
                foreach (var (population, ind) in populations.WithIndex()) {
                    concatedPopulations.Add(population);
                    concatedMinmaxArrays.Add(minmaxArrays[ind] + "-" + ind);
                }
                foreach (var (population, ind) in newPopulations.WithIndex()) {
                    concatedPopulations.Add(population);
                    concatedMinmaxArrays.Add(newPopulationMinmaxArrays[ind] + "-" + (ind + populations.Count));
                }
                List<string> sortedConcatedMinmaxArrays = concatedMinmaxArrays.OrderBy(t => int.Parse(t.Split("-")[0])).ToList();
                List<List<List<int>>> sortedConcatedPopulations = new List<List<List<int>>>();
                List<int> preparedSortedConcatedMinmaxArrays = new List<int>();
                for (int i = 0; i < countOfStartPopulations; i++) {
                    int indexOfPopulation = int.Parse(sortedConcatedMinmaxArrays[i].Split("-")[1]);
                    sortedConcatedPopulations.Add(concatedPopulations[indexOfPopulation]);
                    int minMaxValue = int.Parse(sortedConcatedMinmaxArrays[i].Split("-")[0]);
                    preparedSortedConcatedMinmaxArrays.Add(minMaxValue);
                }

                bool isChangedMinInPopulation = preparedSortedConcatedMinmaxArrays.Min() < minmaxArrays.Min();
                if (isChangedMinInPopulation) {
                    counterOfChangesWithoutMutation = 0;
                    minmaxArrays = preparedSortedConcatedMinmaxArrays.ToList();
                    populations = sortedConcatedPopulations.Take(countOfStartPopulations).ToList();
                    Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!Поколение изменилось!!");
                    Console.WriteLine("Minmax изменился на:\n" + string.Join(", ", minmaxArrays));
                    Console.WriteLine("********************************************************************************");
                    WriteMinPopulation(minmaxArrays, populations);
                    Console.WriteLine("********************************************************************************");
                    Console.WriteLine("\n\n\n\t\tНачинаем заново");
                } else {
                    counterOfChangesWithoutMutation++;
                    Console.WriteLine("\n\t\t!!Поколение НЕ изменилось!!\n\n");
                }
                counterOfChilds++;
                Console.WriteLine("\n Всего поколений = " + counterOfChilds);
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
            List<int> sumProcessors = new List<int>();
            List<List<int>> processors = new List<List<int>>();
            for (int j = 0; j < countOfProcessors; j++) {
                sumProcessors.Add(0);
                processors.Add(new List<int>());
            }
            foreach (var child in population) {
                int indexOfProcessor = GetIndexOfProcessorByGen(countOfProcessors, child[1]);
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

        static int GetIndexOfProcessorByGen(int countOfProcessors, int gen) {
            // ищем делитель чтобы раскидывать между процессорами
            int divider = (int)Math.Ceiling((double)255 / countOfProcessors);
            int workNumber = gen;
            if (workNumber == 0) {
                workNumber = 1;
            }
            if (workNumber == 255) {
                workNumber = 254;
            }
            return (int)Math.Floor((double)workNumber / divider);
        }

        static void MutationProcess(int countOfTasks, List<List<int>> population, int countOfProcessors, List<List<int>> tasksArray) {
            Random random = new Random();
            bool shouldBeMutation = random.Next(0, 100) < 25;
            if (shouldBeMutation) {
                int randomIndexForMutation = random.Next(1, countOfTasks - 1);
                // [задача, ген][]
                int workGen = population[randomIndexForMutation][1];
                string workGenTo2 = Convert.ToString(workGen, 2);
                // добавляем необходимое количество нулей до длины 8
                if (workGenTo2.Length != 8) {
                    string zeroString = "";
                    for (int ind = 0; ind < (8 - workGenTo2.Length); ind++) {
                        zeroString += "0";
                    }

                    workGenTo2 = zeroString + workGenTo2;
                }
                int firstRandomIndexForMutationInvert = random.Next(0, 7);
                int secondRandomIndexForMutationInvert = firstRandomIndexForMutationInvert;
                // пока второй индекс не будет уникальным 
                while (firstRandomIndexForMutationInvert == secondRandomIndexForMutationInvert) {
                    secondRandomIndexForMutationInvert = random.Next(0, 7);
                }
                Console.WriteLine("Мутация: биты по номерами(от 1) " + (firstRandomIndexForMutationInvert + 1) + " и " + (secondRandomIndexForMutationInvert + 1) + " меняются местами");
                // меняем биты местами
                char[] arr = workGenTo2.ToCharArray();
                char temp = workGenTo2[secondRandomIndexForMutationInvert];
                arr[secondRandomIndexForMutationInvert] = arr[firstRandomIndexForMutationInvert];
                arr[firstRandomIndexForMutationInvert] = temp;
                string changedWorkGenTo2 = new string(arr);
                int changedGen = Convert.ToInt32(changedWorkGenTo2, 2);
                Console.WriteLine("Мутация: индекс из матрицы заданий(от 1) " + (randomIndexForMutation + 1) + ", было задание " + population[randomIndexForMutation][0] + ", ген = " + workGen + ". Стал ген = " + changedGen);
                Console.WriteLine("Мутация: было " + workGenTo2 + ", стало: " + changedWorkGenTo2);
                int processorIndexOfPrevGen = GetIndexOfProcessorByGen(countOfProcessors, workGen);
                int processorIndexOfNewGen = GetIndexOfProcessorByGen(countOfProcessors, changedGen);
                // если номер процессора не поменялся после изменения гена - просто меняем ген
                if (processorIndexOfPrevGen == processorIndexOfNewGen) {
                    Console.WriteLine("Мутация: номер процессора НЕ поменялся, просто меняем ген на " + changedGen);
                    population[randomIndexForMutation][1] = changedGen;
                    // иначе задание берем из не однородной матрицы и меняем на новый ген
                } else {
                    Console.WriteLine("Мутация: номер процессора от 1 поменялся (c " + (processorIndexOfPrevGen + 1) + " на " + (processorIndexOfNewGen + 1) + ") , меняем задание на " + tasksArray[randomIndexForMutation][processorIndexOfNewGen] + "(было задание " + population[randomIndexForMutation][0] + " ) и ген на " + changedGen);
                    population[randomIndexForMutation][0] = tasksArray[randomIndexForMutation][processorIndexOfNewGen];
                    population[randomIndexForMutation][1] = changedGen;
                }
            } else {
                Console.WriteLine("!!!!!Мутация не произошла, ничего не изменилось");
            }
        }

        static List<List<int>> CrossoverProcess(int countOfTasks, List<List<int>> population, int firstRandomSplitIndex, int secondRandomSplitIndex, int indexOfFirstChild, int indexOfSecondChild, List<List<List<int>>> populations) {
            Random random = new Random();
            bool shouldBeCrossover = random.Next(0, 100) < 80;
            if (shouldBeCrossover) {
                int preparedFirstRandomSplitIndex = firstRandomSplitIndex > secondRandomSplitIndex ? secondRandomSplitIndex : firstRandomSplitIndex;
                int preparedSecondRandomSplitIndex = secondRandomSplitIndex < firstRandomSplitIndex ? firstRandomSplitIndex : secondRandomSplitIndex;
                Console.WriteLine("\nТочки разбиения(от 1, включая границы): " + (preparedFirstRandomSplitIndex + 1) + " и " + (preparedSecondRandomSplitIndex + 1));

                // если индекс текущего элемента меньше разделителя то добавляем из первой особи, иначе из второй
                for (int i = 0; i < countOfTasks; i++) {
                    if (i > preparedFirstRandomSplitIndex && i <= preparedSecondRandomSplitIndex) {
                        population.Add(populations[indexOfSecondChild][i].ToList<int>());
                    } else {
                        population.Add(populations[indexOfFirstChild][i].ToList<int>());
                    }
                }
                Console.WriteLine("\nПосле кроссовера получилось следующее поколение:");
                foreach (var child in population) {
                    Console.WriteLine("Задание = " + child[0] + ", Ген = " + child[1]);
                }
            } else {
                Console.WriteLine("!!!!!Кроссовер не происходит, берем родителя");
                population = populations[indexOfFirstChild];
            }

            return population;
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
