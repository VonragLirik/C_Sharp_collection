using System;
using System.Linq;
using System.Collections.Generic;

// Создать программу для минимизации детерминированного автомата
// Затем бесконечно проверять цепочку по преобразованному автомату

namespace TALab6 {
    class Program {
        static void Main() {
            Console.WriteLine("4й вариант");
            Console.WriteLine("Введите буквы алфавита через пробел");
            string alphabetWithSpaces = Console.ReadLine();
            List<string> alphabet = alphabetWithSpaces.Split(" ").ToList().Distinct().ToList();
            if (alphabet.Count != alphabetWithSpaces.Split(" ").ToList().Count) {
                Console.WriteLine("Все буквы алфавита должны быть уникальными");
                Environment.Exit(0);
            }
            foreach (string symbol in alphabet) {
                if (symbol.Length > 1) {
                    Console.WriteLine("В алфавите все символы должны иметь длину 1");
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Введите состояния через пробел вида: q0 q1 q2");
            string statesWithSpaces = Console.ReadLine();
            List<string> states = statesWithSpaces.Split(" ").ToList().Distinct().ToList();
            if (states.Count != statesWithSpaces.Split(" ").ToList().Count) {
                Console.WriteLine("Все состояния должны быть уникальными");
                Environment.Exit(0);
            }

            Console.WriteLine("Введите начальное состояние");
            string startState = Console.ReadLine();
            if (states.Find(s => s == startState) == null) {
                Console.WriteLine("Начальное состояние не найдено");
                Environment.Exit(0);
            }
            Console.WriteLine("Введите конечные состояния через пробел");
            List<string> endStates = Console.ReadLine().Split(" ").ToList();
            if (IsSubset(endStates, states)) {
                Console.WriteLine("Kонечное состояние не найдено");
                Environment.Exit(0);
            }

            Console.WriteLine("Введите данные по переходам. Если перехода нет, то поставьте знак -");
            List<List<string>> tableStates = new List<List<string>>();
            foreach (var state in states) {
                List<string> lineTableStates = new List<string>();
                foreach (var symbol in alphabet) {
                    Console.WriteLine("Введите значение (" + state + ", " + symbol + " ) = ");
                    string inputState = Console.ReadLine();
                    if (inputState != "-" && states.Find(s => s == inputState) == null) {
                        Console.WriteLine("Значение должно быть из списка состояний");
                        Environment.Exit(0);
                    }
                    lineTableStates.Add(inputState);
                }
                tableStates.Add(lineTableStates);
            }

            Console.WriteLine("Таблица автомата:\n");
            Console.WriteLine("       " + string.Join("    ", alphabet));
            int counter = 0;
            foreach (var oneLine in tableStates) {
                Console.WriteLine("q" + counter + "    " + string.Join("    ", oneLine));
                counter++;
            }

            // если в строке таблицы есть "-", значит автомат не полностью детерминирован
            if (tableStates.Find(tableStatesLine => tableStatesLine.Find(tableStatesLineItem => tableStatesLineItem != "-") == null) == null) {
                Console.WriteLine("\nАвтомат детерминирован");
            }

            // удаление недостижимых вершин
            // заводим массив с вызовом каждой вершины, если ни разу не вызывалась то останется false
            List<bool> isCallStateSomewhereArray = new List<bool>();
            foreach (var state in tableStates) {
                isCallStateSomewhereArray.Add(false);
            }
            foreach (var tableStatesLine in tableStates) {
                foreach (var tableStatesLineItem in tableStatesLine) {
                    int.TryParse(tableStatesLineItem.Split("q")[1], out int calledState);
                    isCallStateSomewhereArray[calledState] = true;
                }
            }
            int notCalledStateIndex = -1;
            for (int i = 0; i < isCallStateSomewhereArray.Count; i++) {
                if (!isCallStateSomewhereArray[i] && states[i] != startState) {
                    notCalledStateIndex = i;
                }
            }
            if (notCalledStateIndex != -1) {
                tableStates.RemoveAt(notCalledStateIndex);
                string stateName = "q" + notCalledStateIndex;
                int notCalledStateInEndStatesIndex = endStates.FindIndex(s => s == stateName);
                if (notCalledStateInEndStatesIndex != -1) {
                    endStates.RemoveAt(notCalledStateInEndStatesIndex);
                }
                int notCalledStateInStatesIndex = states.FindIndex(s => s == stateName);
                if (notCalledStateInStatesIndex != -1) {
                    states.RemoveAt(notCalledStateInStatesIndex);
                }
            }
            Console.WriteLine("\nАвтомат после удаления недостижимых вершин:");
            Console.WriteLine("Таблица автомата:\n");
            Console.WriteLine("       " + string.Join("    ", alphabet));
            counter = 0;
            foreach (var oneLine in tableStates) {
                Console.WriteLine("q" + counter + "    " + string.Join("    ", oneLine));
                counter++;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // Составление классов
            // Первые два класса - конечные вершины и все остальные
            // Затем проверяем переход по каждому элементу класса по 0 и 1
            // если переходит в элемент другого класса, значит выносим эту вершину в новый класс и начинаем заново
            // если ничего не изменилось проверив все - то значит получили минимальный автомат
            List<List<string>> classesArray = new List<List<string>>() { endStates };
            // формируем НЕ конечные вешрины как второй класс
            List<string> secondClass = new List<string>();
            foreach (var state in states) {
                if (classesArray[0].Find(c => c == state) == null) {
                    secondClass.Add(state);
                }
            }
            classesArray.Add(secondClass);

            // Приступаем к проверки классом
            bool haveChanges = true;
            while (haveChanges) {
                bool isChangedClasses = false;
                for (int indexOfClass = 0; indexOfClass < classesArray.Count; indexOfClass++) {
                    List<string> classesArrayItem = classesArray[indexOfClass];

                    // перебираем класс только если там больше 1 элемента
                    if (classesArrayItem.Count > 1) {
                        for (int alphabetSymbolIndex = 0; alphabetSymbolIndex < alphabet.Count; alphabetSymbolIndex++) {
                            // в массиве с состояними вне класса: [начальное, конечное состояние, индекс в классе]
                            // (q2, 0) -> q5  будет q2, q5
                            List<List<string>> errorStates = new List<List<string>>();
                            for (int indexOfClassState = 0; indexOfClassState < classesArrayItem.Count; indexOfClassState++) {
                                string classesArrayItemState = classesArray[indexOfClass][indexOfClassState];
                                int.TryParse(classesArrayItemState.Split("q")[1], out int calledStateIndex);
                                // если вызываемая вершина вне класса, то запоминаем её
                                if (classesArrayItem.Find(c => c == tableStates[calledStateIndex][alphabetSymbolIndex]) == null) {
                                    errorStates.Add(new List<string>() { classesArrayItemState, tableStates[calledStateIndex][alphabetSymbolIndex], indexOfClassState.ToString() });

                                }
                            }
                            if (errorStates.Count > 0) {
                                // ищем вершины с таким же конечным состоянием, чтобы вынести их в новый класс
                                List<string> errorStatesLikeFirst = new List<string>();
                                List<List<string>> errorStatesLikeFirstWithAllInfo = new List<List<string>>();
                                foreach (var errorStatesItem in errorStates) {
                                    if (errorStatesItem[1] == errorStates[0][1]) {
                                        errorStatesLikeFirst.Add(errorStatesItem[0]);
                                        errorStatesLikeFirstWithAllInfo.Add(errorStatesItem);
                                    }
                                }
                                // если еще нет такого класса до добавляем
                                if (classesArray.FindIndex(c => IsEqualArrays(c, errorStatesLikeFirst)) == -1) {
                                    // выносим в отдельный класс
                                    classesArray.Add(errorStatesLikeFirst);
                                    int countOfRemoved = 0;
                                    foreach (var errorStatesLikeFirstWithAllInfoItem in errorStatesLikeFirstWithAllInfo) {
                                        int.TryParse(errorStatesLikeFirstWithAllInfoItem[2], out int errorClassStateIndex);
                                        classesArray[indexOfClass].RemoveAt(errorClassStateIndex - countOfRemoved);
                                        countOfRemoved++;
                                    }
                                    isChangedClasses = true;
                                    break;
                                }
                            }
                            if (isChangedClasses) {
                                break;
                            }
                        }

                        if (isChangedClasses) {
                            break;
                        }
                    }
                }

                if (!isChangedClasses) {
                    haveChanges = false;
                }
            }

            // Сортируем по возрастанию вершин для удобства
            classesArray = classesArray.OrderBy(q => q[0]).ToList();
            Console.WriteLine();
            foreach (var classesArrayItem in classesArray) {
                Console.WriteLine("{ " + string.Join(", ", classesArrayItem) + " }");
            }

            /////////////////////////////////////////////////////////////////
            // Формируем таблицу по получившимся классам
            // достаточно проверить одну из вершин из класса
            // пробегаемся по строчке из таблицы это вершины и ищем в каком классе выходная вершина
            // записываем найденный индекс в новую табличку
            List<List<string>> minimizedTableStates = new List<List<string>>();
            foreach(var classesArrayItem in classesArray) {
                int.TryParse(classesArrayItem[0].Split("q")[1], out int stateIndex);
                List<string> minimizedTableStatesLine = new List<string>();
                foreach (var state in tableStates[stateIndex]) {
                    int indexOfClass = classesArray.FindIndex(c => c.Exists((ci) => ci == state));
                    minimizedTableStatesLine.Add("P" + indexOfClass);
                }
                minimizedTableStates.Add(minimizedTableStatesLine);
            }
            Console.WriteLine();
            foreach (var minimizedTableStatesLine in minimizedTableStates) {
                Console.WriteLine(string.Join(", ", minimizedTableStatesLine));
            }

            ///////////////////////////////////////////////////////////////////////////////
            // Проверка цепочки по получившемуся автомату
            bool isWorkWhile = true;
            while (isWorkWhile) {
                Console.WriteLine("Введите цепочку");
                string chain = Console.ReadLine();
                bool isCorrectChain = true;
                if (chain.Length == 0) {
                    isCorrectChain = false;
                    Console.WriteLine("Цепочка пустая");
                }
                foreach (var symbol in chain) {
                    if (alphabet.Find(a => a == symbol.ToString()) == null) {
                        isCorrectChain = false;
                        Console.WriteLine("Символ из цепочки не найден в алфавите");
                    }
                }
                if (isCorrectChain) {
                    List<string> visited = new List<string>();
                    if (IsCorrectChain(0, chain, minimizedTableStates, alphabet, endStates)) {
                        Console.WriteLine("Допускает");
                    } else {
                        Console.WriteLine("Не допускает");
                    };

                    Console.WriteLine("Для выхода введите символ .");
                    Console.WriteLine("Для продолжений - любой другой символ");
                    string exitSimbol = Console.ReadLine();
                    if (exitSimbol == ".") {
                        isWorkWhile = false;
                    }
                }
            }
        }

        // проверяет включается ли один массив в другой
        static bool IsSubset(List<string> mainArray, List<string> childArray) {
            foreach (var childArrayItem in childArray) {
                if (mainArray.Find(i => i == childArrayItem) == null) {
                    return false;
                }
            }
            return true;
        }

        // проверяет равность массивов
        static bool IsEqualArrays(List<string> mainArray, List<string> childArray) {
            return Enumerable.SequenceEqual(mainArray, childArray);
        }

        // проверяет правильность цепочки
        static bool IsCorrectChain(int startState, string chain, List<List<string>> minimizedTableStates, List<string> alphabet, List<string> endStates) {
            // если цепочка еще есть - рекурсивно идем до конца
            if (chain.Length > 0) {
                string symbol = chain[0].ToString();
                // Ищем индекс нового символа цепочки
                int symbolIndex = alphabet.FindIndex(a => a == symbol);
                string newP = minimizedTableStates[startState][symbolIndex];
                // если это какая-то Pшка, то идем дальше убирая этот символ из строки
                if (newP != "-") {
                    int.TryParse(newP.Split("P")[1], out int newPStateIndex);
                    return IsCorrectChain(newPStateIndex, chain.Substring(1), minimizedTableStates, alphabet, endStates);
                } else {
                    // если это -, то сразу говорим что не допускает
                    return false;
                }
            } else {
                // иначе проверяем содержит ли последнее состояние
                // если да, то true, иначе false
                return endStates.Find(e => e == ("q"+ startState)) != null;
            }
        }
    }
}
