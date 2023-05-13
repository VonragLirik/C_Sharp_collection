using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// модифицированная модель гулбера при модификации комежора


namespace EvrAlgLaba7 {
    class Program {
        static void Main() {
            Console.WriteLine("Решение однородной минимаксной задачи с помощью модифицированной модели выбора");
            Console.WriteLine("Введите количество вершин");
            int countOfPoints = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите нижнюю границу массива t1");
            int valueMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите верхнюю границу массива t2");
            int valueMax = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество начальных популяций");
            int countOfStartPopulations = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите количество повторений без изменений для выхода");
            int countOfMutationWithoutChange = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите номер начальной вершины(с 0)");
            int firstPointNumber = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите вероятность кроссовера");
            int crossoverProbability = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите вероятность мутации");
            int mutationProbability = int.Parse(Console.ReadLine());

            StreamWriter writter = new StreamWriter("C:/Users/user/source/repos/C_Sharp_collection/EvrAlgLaba7/EvrAlgLaba7/output.txt");
            Console.SetOut(writter);
            List<List<int>> pointsArray = new List<List<int>>();
            int counterOfChilds = 1;

            // Создаем рандомные значения
            Random random = new Random();

            for (int i = 0; i < countOfPoints; i++) {
                List<int> linePointsArray = new List<int>();
                for (int j = 0; j < countOfPoints; j++) {
                    linePointsArray.Add(0);
                }
                pointsArray.Add(linePointsArray);
            }

            for (int i = 0; i < countOfPoints; i++) {
                for (int j = i; j < countOfPoints; j++) {
                    if (i != j) {
                        int randomPointValue = random.Next(valueMin, valueMax);
                        pointsArray[i][j] = randomPointValue; // Верхняя половина
                        pointsArray[j][i] = randomPointValue; // Нижняя половина (симметричное значение)
                    }
                }
            }

            Console.WriteLine("Исходная матрица");
            Console.WriteLine("   " + string.Join("  ", Enumerable.Range(0, countOfPoints)));
            for (int i = 0; i < pointsArray.Count; i++) {
                Console.Write(i + " ");
                for (int j = 0; j < countOfPoints; j++) {
                    int pointValue = pointsArray[i][j];
                    Console.Write((pointValue == 0 ? " " : "") + pointValue + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            List<int> minGraphPathArray = FindMinGraphPath(pointsArray, firstPointNumber);
            FindTotalMinGraphPath(pointsArray, minGraphPathArray);

            /////////////////////////////////////
            Console.WriteLine("\nФормируем популяции\n");
            List<List<int>> populations = new List<List<int>>();
            List<int> populationLengthsArray = new List<int>();
            for (int i = 0; i < countOfStartPopulations; i++) {
                List<int> population = new List<int>();
                for (int index = 0; index < countOfPoints; index++) {
                    int randomNumber = random.Next(0, countOfPoints);
                    while (population.Contains(randomNumber)) {
                        randomNumber = random.Next(0, countOfPoints);
                    }
                    population.Add(randomNumber);
                }
                population.Add(population[0]);

                int populationLength = FindTotalMinGraphPath(pointsArray, population);
                populationLengthsArray.Add(populationLength);
                populations.Add(population);
            }

            Console.WriteLine("\nПути начальной популяций:\n" + string.Join(", ", populationLengthsArray));
            Console.WriteLine("Минимум = " + populationLengthsArray.Min());
            Console.WriteLine("\n/////////////////////////////////////////////////////////////////////////////////////////////");


            Console.WriteLine("\nФормируем из начального поколения следующее поколение");
            // делаем кроссовер + мутацию
            bool isChangedParent = false;
            int counterOfChangesWithoutMutation = 0;
            while (counterOfChangesWithoutMutation < countOfMutationWithoutChange) {
                Console.WriteLine("\nНомер поколения = " + counterOfChilds);
                Console.WriteLine("Повторов без изменений(от 1) = " + (counterOfChangesWithoutMutation + 1) + "\n");
                List<int> populationLengthsTmpArray = new List<int>();
                int indexOfFirstChild = random.Next(0, countOfStartPopulations);
                int indexOfSecondChild = indexOfFirstChild;
                populationLengthsTmpArray.Add(populationLengthsArray[indexOfFirstChild]);
                // пока второй индекс не будет равен 
                while (indexOfFirstChild == indexOfSecondChild) {
                    indexOfSecondChild = random.Next(0, countOfStartPopulations);
                }
                int randomSplitIndex = random.Next(1, countOfPoints - 1);
                List<int> newPopulation = CrossoverProcess(crossoverProbability, countOfPoints, randomSplitIndex, indexOfFirstChild, indexOfSecondChild, populations, populationLengthsArray);
                MutationProcess(mutationProbability, countOfPoints, newPopulation);
                int firstChildLength = FindTotalMinGraphPath(pointsArray, newPopulation);
                Console.WriteLine("После первой мутации и кроссовера получилась длина пути = " + firstChildLength);
                populationLengthsTmpArray.Add(firstChildLength);


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////делаем кроссовер наоборот
                Console.WriteLine("\nТеперь делаем кроссовер и мутацию наоборот\n");
                List<int> newSecondPopulation = CrossoverProcess(crossoverProbability, countOfPoints, randomSplitIndex, indexOfSecondChild, indexOfFirstChild, populations, populationLengthsArray);
                MutationProcess(mutationProbability, countOfPoints, newSecondPopulation);
                int secondChildLength = FindTotalMinGraphPath(pointsArray, newSecondPopulation);
                Console.WriteLine("После второй мутации и кроссовера получилась длина пути = " + secondChildLength);
                populationLengthsTmpArray.Add(secondChildLength);

                int indexOfMinResult = populationLengthsTmpArray.FindIndex(m => m == populationLengthsTmpArray.Min());
                Console.WriteLine("\nПолучившиеся пути: " + string.Join("  ", populationLengthsTmpArray));
                Console.WriteLine("Лучшая особь под номером(от 1) - " + (indexOfMinResult + 1));

                // если меньшая не родительская, то меняем
                if (indexOfMinResult != 0) {
                    populationLengthsArray[indexOfFirstChild] = indexOfMinResult == 1 ? firstChildLength : secondChildLength;
                    populations[indexOfFirstChild] = indexOfMinResult == 1 ? newPopulation : newSecondPopulation;
                    isChangedParent = true;
                }

                if (isChangedParent) {
                    counterOfChangesWithoutMutation = 0;
                    isChangedParent = false;
                    Console.WriteLine("\n!!!!!!!!!!!!!!!!!!!!!Популяция изменилась!!");
                    Console.WriteLine("Стала особь с путем = " + populationLengthsArray[indexOfFirstChild]);
                    Console.WriteLine(string.Join("  ", populations[indexOfFirstChild]));
                    Console.WriteLine("\n!!!!!Минимальная особь в популяции:");
                    Console.WriteLine("********************************************************************************");
                    WriteMinPopulation(populationLengthsArray, populations);
                    Console.WriteLine("********************************************************************************");
                    Console.WriteLine("\n\n\n\t\tНачинаем заново");
                } else {
                    Console.WriteLine("////////////////////////////////\n\n");
                    counterOfChangesWithoutMutation++;
                }
                counterOfChilds++;
            }

            ////////// подводим итог
            Console.WriteLine("\n\n\n/////////////////////////////////////////////////////////////////////////////////////////////");
            Console.WriteLine("\nПолучилось всего поколений: " + counterOfChilds);
            Console.WriteLine("\nПолучились следующие пути поколения:");
            Console.WriteLine(string.Join(" ", populationLengthsArray));
            WriteMinPopulation(populationLengthsArray, populations);
            writter.Close();
        }

        static List<int> FindMinGraphPath(List<List<int>> graph, int firstPointNumber) {
            int numVertices = graph.Count();

            // Инициализация массива для хранения посещенных вершин
            bool[] visited = new bool[numVertices];
            visited[firstPointNumber] = true; // Начинаем с указанной первой вершины

            // Инициализация списка для хранения кратчайшего пути
            List<int> path = new List<int> { firstPointNumber };

            int currentVertex = firstPointNumber;
            for (int i = 0; i < numVertices - 1; i++) {
                int nextVertex = -1;
                int minDistance = int.MaxValue;

                // Поиск следующей ближайшей непосещенной вершины
                for (int j = 0; j < numVertices; j++) {
                    if (!visited[j] && (graph[currentVertex][j] != 0) && (graph[currentVertex][j] < minDistance)) {
                        nextVertex = j;
                        minDistance = graph[currentVertex][j];
                    }
                }

                if (nextVertex != -1) {
                    visited[nextVertex] = true;
                    path.Add(nextVertex);
                    currentVertex = nextVertex;
                }
            }

            path.Add(firstPointNumber);

            return path;
        }

        static int FindTotalMinGraphPath(List<List<int>> graph, List<int> pointsArray) {
            Console.WriteLine("Вершины по которым ищется путь");
            Console.WriteLine(string.Join(" ", pointsArray));

            int sumPointsLength = 0;
            int prevPoint = pointsArray[0];
            for (int i = 1;i<pointsArray.Count;i++) {
                int point = pointsArray[i];
                int lengthValue = graph[prevPoint][point];
                Console.Write(lengthValue + (i < pointsArray.Count - 1 ? " + ": ""));
                sumPointsLength += lengthValue;
                prevPoint = point;
            }
            Console.Write(" = " + sumPointsLength);
            Console.WriteLine();
            return sumPointsLength;
        }

        static void MutationProcess(int mutationProbability, int countOfTasks, List<int> population) {
            Random random = new Random();
            bool shouldBeMutation = random.Next(0, 100) < 80;
            if (shouldBeMutation) {
                int firstRandomIndexForMutationInvert = random.Next(1, countOfTasks - 1);
                int secondRandomIndexForMutationInvert = firstRandomIndexForMutationInvert;
                // пока второй индекс не будет уникальным 
                while (firstRandomIndexForMutationInvert == secondRandomIndexForMutationInvert) {
                    secondRandomIndexForMutationInvert = random.Next(1, countOfTasks - 1);
                }
                Console.WriteLine("\nМутация: вершины под номерами(от 1) " + (firstRandomIndexForMutationInvert + 1) + " и " + (secondRandomIndexForMutationInvert + 1) + " меняются местами");
                int tmpValue = population[firstRandomIndexForMutationInvert];
                population[firstRandomIndexForMutationInvert] = population[secondRandomIndexForMutationInvert];
                population[secondRandomIndexForMutationInvert] = tmpValue;
                Console.WriteLine("Мутация: после мутации получилась следующая особь:");
                Console.WriteLine(string.Join(" ", population));
            } else {
                Console.WriteLine("!!!!!Мутация не произошла, ничего не изменилось");
            }
        }
        static List<int> CrossoverProcess(int crossoverProbability, int countOfTasks, int randomSplitIndex, int indexOfFirstChild, int indexOfSecondChild, List<List<int>> populations, List<int> populationLengthsArray) {
            Random random = new Random();
            List<int> newPopulation = new List<int>();
            bool shouldBeCrossover = random.Next(0, 100) < crossoverProbability;
            if (shouldBeCrossover) {
                Console.WriteLine("Кроссовер происходит с особями под номерами: " + (indexOfFirstChild + 1) + " и " + (indexOfSecondChild + 1));
                Console.WriteLine("Точка разбиения = " + (randomSplitIndex + 1));
                Console.WriteLine("\nПервая начальная особь, путь = " + populationLengthsArray[indexOfFirstChild]);
                Console.WriteLine(string.Join(" ", populations[indexOfFirstChild]));
                Console.WriteLine("Вторая начальная особь, путь = " + populationLengthsArray[indexOfSecondChild]);
                Console.WriteLine(string.Join(" ", populations[indexOfSecondChild]));

                // если индекс текущего элемента меньше разделителя то добавляем из первой особи, иначе из второй
                for (int i = 0; i < countOfTasks; i++) {
                    if (i < randomSplitIndex) {
                        newPopulation.Add(populations[indexOfFirstChild][i]);
                    } else if(!newPopulation.Contains(populations[indexOfSecondChild][i])) {
                        newPopulation.Add(populations[indexOfSecondChild][i]);
                    }
                }
                if(newPopulation.Count != countOfTasks) {
                    for (int i = 0; i < countOfTasks; i++) {
                        if (!newPopulation.Contains(populations[indexOfSecondChild][i])) {
                            newPopulation.Add(populations[indexOfSecondChild][i]);
                        }
                        if(newPopulation.Count == countOfTasks) {
                            break;
                        }
                    }
                }
                newPopulation.Add(newPopulation[0]);
                Console.WriteLine("\nПосле кроссовера получилось следующая особь:");
                Console.WriteLine(string.Join(" ", newPopulation));
            } else {
                Console.WriteLine("!!!!!Кроссовер не происходит, берем родителя");
                newPopulation = populations[indexOfFirstChild];
            }

            return newPopulation;
        }

        static void WriteMinPopulation(List<int> populationLengthsArray, List<List<int>> populations) {
            int minIndexResultPopulation = populationLengthsArray.FindIndex(m => m == populationLengthsArray.Min());
            Console.WriteLine("Путь минимальной особи = " + populationLengthsArray[minIndexResultPopulation]);
            Console.WriteLine("Сама особь равна: " + string.Join("  ", populations[minIndexResultPopulation]));
        }
    }
}

static class MyExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> array) =>
        array.Select((item, index) => (item, index));
}

