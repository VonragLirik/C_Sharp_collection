using System;
using System.Linq;
using System.Collections.Generic;

/*
- вводится алфавит
- вводятся состояния
- затем вводятся данные для таблицы по алфавиту и состояниям(по каждому)(не забыть добавить Е переходы)
- выводится табличка по всем этим данным
- выводится табличка с S1, S2, ...
- выводится табличка с P1, P2, ...
- выводится текст "Введите цепочку: "
- сделать валидацию что символы из алфавита
- после проверки выводится "допускается"/"не допускается"
 */

namespace TALaba5 {
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
            alphabet.Add("E");

            Console.WriteLine("Введите состояния через пробел вида: q0 q1 q2 ");
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
            Console.WriteLine("Введите конечное состояние");
            string endState = Console.ReadLine();
            if (states.Find(s => s == endState) == null) {
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
            Console.WriteLine(" " + string.Join("    ", alphabet));
            foreach (var oneLine in tableStates) {
                Console.WriteLine(string.Join("    ", oneLine));
            }

            Console.WriteLine("\n Формируем S:");
            List<List<string>> SArray = new List<List<string>>();
            int SArrayIndex = 0;
            foreach (var state in states) {
                List<string> SArrayItem = new List<string> { state };
                string epsilonState = tableStates[SArrayIndex][alphabet.Count - 1];
                if (epsilonState != "-") {
                    SArrayItem.Add(epsilonState);
                }
                SArray.Add(SArrayItem);
                SArrayIndex++;
                Console.WriteLine("E(" + state + ") = {" + string.Join(", ", SArray.Last()) + "}");
            }


            ///////////////////////////////////////////////////////////////////////////////
            // Строим табличку с S
            List<List<string>> SArrayTables = new List<List<string>>();
            //Пробегаем по каждой строчке таблицы с Sками
            foreach (var SSymbolArray in SArray) {
                List<string> SArrayTablesLine = new List<string>();

                // Пробегаемся по алфавиту каждому элементу в S
                // Например у S1 - это q0, q3
                // Значит мы по 0 пройдемся по q0, q3, затем по 1 пройдемся по q0, q3
                for (int i = 0; i < alphabet.Count - 1; i++) {
                    List<string> resultArray = new List<string>();
                    // Проходимся по 0 по q0, q3, затем по 1 пройдемся по q0, q3
                    foreach (var SSymbol in SSymbolArray) {
                        int.TryParse(SSymbol.Split("q")[1], out int indexSymbol);
                        // Проходим по строчке автомата элемента
                        // У q0 - это q1, -, q3. Первый раз берем q1, затем -
                        recursiveSCheck(tableStates, indexSymbol, i, true, resultArray);
                    }
                    if (resultArray.Count > 0) {
                        resultArray = resultArray.Distinct().ToList();
                        SArrayTablesLine.Add(convertStateToS(SArray, resultArray));
                    } else {
                        SArrayTablesLine.Add("-");
                    }
                }

                SArrayTables.Add(SArrayTablesLine);
            }

            alphabet.RemoveAt(alphabet.Count - 1);
            Console.WriteLine("\nТаблица с S:\n");
            Console.WriteLine(" " + string.Join("          ", alphabet));
            foreach (var oneLine in SArrayTables) {
                Console.WriteLine(string.Join("          ", oneLine));
            }


            ///////////////////////////////////////////////////////////////////////////////
            // Формируем P
            Console.WriteLine("\n Формируем таблицу с P:");
            List<List<string>> PArray = new List<List<string>> { new List<string> { "S0", "S3" } };
            bool isCheckAllArray = false;
            while (!isCheckAllArray) {
                foreach (var PArrayItem in PArray) {
                    bool isNotFoundP = false;
                    // Для текущей Pшки пробегаемся по алфавиту
                    // P0 = {S0, S3} - идем по S0 для 0 и 1, затем если их объединения уже есть в PArray, то идем по S3 по 0 и 1
                    for (int indexOfAlphabet = 0; indexOfAlphabet < alphabet.Count; indexOfAlphabet++) {
                        List<string> SArrayThisLine = new List<string>();
                        // Создаем массив с объединениями по Sкам
                        foreach (var PArrayItemS in PArrayItem) {
                            int.TryParse(PArrayItemS.Split("S")[1], out int PArrayItemSTableIndex);
                            if (SArrayTables[PArrayItemSTableIndex][indexOfAlphabet] != "-") {
                                foreach (var addItem in SArrayTables[PArrayItemSTableIndex][indexOfAlphabet].Split(",")) {
                                    SArrayThisLine.Add(addItem);
                                }
                            }
                        }
                        // Если объединив получаем 0 - то добавляем на эту позицию прочерк
                        if (SArrayThisLine.Count == 0) {
                            PArray.Add(new List<string> { "-" });
                            isNotFoundP = true;
                            break;
                        } else {
                            // Иначе проверяем есть ли в  PArray такая комбинация - если нет, то добавляем и начинаем все заново
                            SArrayThisLine = SArrayThisLine.Distinct().ToList();
                            if (PArray.Find(p => IsSubset(SArrayThisLine, p)) == null) {
                                PArray.Add(SArrayThisLine);
                                isNotFoundP = true;
                                break;
                            }
                        }
                    }
                    if (isNotFoundP) {
                        break;
                    } else {
                        isCheckAllArray = true;
                    }
                }
            }
            for (int index = 0; index < PArray.Count; index++) {
                Console.WriteLine("P(" + index + ") = {" + string.Join(", ", PArray[index]) + "}");
            }

            // Строим табличку с P
            Console.WriteLine("\nТаблица с P:\n");
            Console.WriteLine(" " + string.Join("      ", alphabet));
            List<List<string>> PArrayTables = new List<List<string>>();
            foreach (var PArrayItem in PArray) {
                List<string> PArrayTablesLine = new List<string>();
                // Для текущей Pшки пробегаемся по алфавиту
                // P0 = {S0, S3} - идем по S0 для 0 и 1, затем если их объединения уже есть в PArray, то идем по S3 по 0 и 1
                for (int indexOfAlphabet = 0; indexOfAlphabet < alphabet.Count; indexOfAlphabet++) {
                    List<string> SArrayThisLine = new List<string>();
                    // Создаем массив с объединениями по Sкам
                    foreach (var PArrayItemS in PArrayItem) {
                        int.TryParse(PArrayItemS.Split("S")[1], out int PArrayItemSTableIndex);
                        if (SArrayTables[PArrayItemSTableIndex][indexOfAlphabet] != "-") {
                            foreach (var addItem in SArrayTables[PArrayItemSTableIndex][indexOfAlphabet].Split(",")) {
                                SArrayThisLine.Add(addItem);
                            }
                        }
                    }
                    // Если объединив получаем 0 - то добавляем на эту позицию прочерк
                    if (SArrayThisLine.Count == 0) {
                        PArrayTablesLine.Add("-");
                    } else {
                        // Иначе проверяем есть ли в  PArray такая комбинация - если нет, то добавляем и начинаем все заново
                        SArrayThisLine = SArrayThisLine.Distinct().ToList();
                        int PArrayItemIndex = PArray.FindIndex(p => IsSubset(SArrayThisLine, p));
                        PArrayTablesLine.Add("P" + PArrayItemIndex);
                    }
                }
                PArrayTables.Add(PArrayTablesLine);
                Console.WriteLine(string.Join("      ", PArrayTablesLine));
            }

            // формируем табличку с вершинами преобразуя P
            List<List<string>> PArrayTablesWithS = new List<List<string>>();
            foreach (var PArrayTablesLine in PArrayTables) {
                List<string> PArrayTablesWithSLine = new List<string>();
                // Пробегаемся по всем Pшкам в строке
                foreach (var PArrayTablesLineItem in PArrayTablesLine) {
                    // Берем Pшку
                    if (PArrayTablesLineItem != "-") {
                        int.TryParse(PArrayTablesLineItem.Split("P")[1], out int PIndex);
                        List<string> SArrayThisLine = new List<string>();
                        // Создаем массив с объединениями по Sкам
                        foreach (var SArrayByPItem in PArray[PIndex]) {
                            int.TryParse(SArrayByPItem.Split("S")[1], out int SIndex);
                            SArrayThisLine.Add(string.Join(",", SArray[SIndex]));
                        }
                        // добавляем только уникальные q
                        PArrayTablesWithSLine.Add(string.Join(",", (string.Join(",", SArrayThisLine).Split(',').Distinct().ToList())));
                    } else {
                        PArrayTablesWithSLine.Add("-");
                    }
                }
                PArrayTablesWithS.Add(PArrayTablesWithSLine);
            }

            ///////////////////////////////////////////////////////////////////////////////
            // Проверка цепочки по получившемуся автомату
            bool isWorkWhile = true;
            while (isWorkWhile) {
                Console.WriteLine("Введите цепочку");
                string chain = Console.ReadLine();
                bool isCorrectChain = true;
                if(chain.Length == 0) {
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
                    if (IsCorrectChain(0, chain, PArrayTables, alphabet, PArrayTablesWithS, endState)) {
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

            // идем по строчке, если в элементе есть вершина, то переходи на эту вершину и меняем флаг на проверку только эпсилон переходов
            // Если нет вершины, то меняем второй индекс и идем дальше
            static void recursiveSCheck(List<List<string>> tableStates, int workFirstIndex, int workSecondIndex, bool canCheckAnotherState, List<string> resultArray) {
                if (!canCheckAnotherState) {
                    string lastElementInLine = tableStates[workFirstIndex][tableStates[workFirstIndex].Count - 1];
                    if (lastElementInLine != "-") {
                        // пойдем по эпсилон замыканию
                        resultArray.Add(lastElementInLine);
                        int.TryParse(lastElementInLine.Split("q")[1], out int newWorkFirstIndex);
                        recursiveSCheck(tableStates, newWorkFirstIndex, tableStates[newWorkFirstIndex].Count - 1, false, resultArray);
                    } else {
                        return;
                    }
                } else {
                    // сначала пойдем по значениям
                    // если у текущего элемента есть значение, то пойдем в это значение, иначе возврат
                    string currentElement = tableStates[workFirstIndex][workSecondIndex];
                    if (currentElement != "-") {
                        resultArray.Add(currentElement);
                        int.TryParse(currentElement.Split("q")[1], out int newWorkFirstIndex);
                        recursiveSCheck(tableStates, newWorkFirstIndex, 0, false, resultArray);
                    } else {
                        return;
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

            // Пробегаемся по всем Sкам
            // Если найденные вершины включаются содержат в себе S полностью, то добавляем их к результату
            // Иначе не добавляем
            // Если ничего не найдено возвращаем "-"
            static string convertStateToS(List<List<string>> SArray, List<string> stateArray) {
                List<string> resultStringArray = new List<string>();
                if (stateArray.Count == 0) {
                    return "-";
                }
                for (int i = 0; i < SArray.Count; i++) {
                    if (IsSubset(stateArray, SArray[i])) {
                        resultStringArray.Add("S" + i);
                    }
                }
                if (resultStringArray.Count == 0) {
                    return "-";
                } else {
                    return string.Join(",", resultStringArray);
                }
            }

            // проверяет правильность цепочки
            static bool IsCorrectChain(int startState, string chain, List<List<string>> PArrayTables, List<string> alphabet, List<List<string>> PArrayTablesWithS, string endState) {
                // если цепочка еще есть - рекурсивно идем до конца
                if (chain.Length > 0) {
                    string symbol = chain[0].ToString();
                    // Ищем индекс нового P
                    int symbolIndex = alphabet.FindIndex(a => a == symbol);
                    string newP = PArrayTables[startState][symbolIndex];
                    // если это какая-то Pшка, то идем дальше убирая этот сивол из строки
                    if (newP != "-") {
                        int.TryParse(newP.Split("P")[1], out int newPStateIndex);
                        return IsCorrectChain(newPStateIndex, chain.Substring(1), PArrayTables, alphabet, PArrayTablesWithS, endState);
                    } else {
                        // если это -, то сразу говорим что не допускает
                        return false;
                    }
                } else {
                    // иначе проверяем содержит ли последнее состояние(q2)
                    // если да, то true, иначе false
                    return PArrayTablesWithS[startState].Find(p => p == endState) == null;
                }
            }
        }
    }
}
