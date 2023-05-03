using System;
using System.Linq;
using System.Collections.Generic;

/*
    Определение типа формальной грамматики и языка по классификации Хомского
    L(G) = { wcwcw | w E {a, b}+} 
    Если слева больше одного символа, то это точно не 3 тип
    Если при этом больше одного нетерминального символа, то это не 2 тип
    1 тип неукорачиваемый - если слева длина <= чем справа
    и 1 тип контекстнозависимой если пройдет все проверки
    0 тип - если все остальные false
*/

namespace TALab7 {
    class Program {
        static void Main() {
            Console.WriteLine("4й вариант");
            Console.WriteLine("Введите терминальные символы через пробел");
            string terminalAlphabetWithSpaces = Console.ReadLine();
            List<string> terminalAlphabet = terminalAlphabetWithSpaces.Split(" ").ToList().Distinct().ToList();
            if (terminalAlphabet.Count != terminalAlphabetWithSpaces.Split(" ").ToList().Count) {
                Console.WriteLine("Все символы терминального алфавита должны быть уникальными");
                Environment.Exit(0);
            }
            foreach (string symbol in terminalAlphabet) {
                if (symbol.Length > 1) {
                    Console.WriteLine("Все терминальные символы должны иметь длину 1");
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Введите нетерминальные символы через пробел");
            string noTerminalAlphabetWithSpaces = Console.ReadLine();
            List<string> noTerminalAlphabet = noTerminalAlphabetWithSpaces.Split(" ").ToList().Distinct().ToList();
            if (noTerminalAlphabet.Count != noTerminalAlphabetWithSpaces.Split(" ").ToList().Count) {
                Console.WriteLine("Все нетерминальные символы должны быть уникальными");
                Environment.Exit(0);
            }
            foreach (string symbol in noTerminalAlphabet) {
                if (symbol.Length > 1) {
                    Console.WriteLine("Все нетерминальные символы должны иметь длину 1");
                    Environment.Exit(0);
                }
            }

            //Console.WriteLine("Введите начальный символ");
            //string startState = Console.ReadLine();

            List<List<string>> rulesArray = new List<List<string>>();
            string continueWorkSymbol = "+";
            while (continueWorkSymbol != "-") {
                Console.WriteLine("Введите левую часть правила:");
                string leftRule = Console.ReadLine();
                CheckHasAlphabetItemsInLine(terminalAlphabet, noTerminalAlphabet, leftRule);
                Console.WriteLine("Введите правую часть правила:");
                string rightRule = Console.ReadLine();
                if (rightRule != "E") {
                    CheckHasAlphabetItemsInLine(terminalAlphabet, noTerminalAlphabet, rightRule);
                }
                rulesArray.Add(new List<string>() { leftRule, rightRule });
                Console.WriteLine("Чтобы завершить ввод правил нажмите -, иначе любой другой символ");
                continueWorkSymbol = Console.ReadLine();
                Console.WriteLine();
            }

            // Если слева больше одного символа, то это точно не 3 тип
            // Если при этом больше одного нетерминального символа, то это не 2 тип
            // 1 тип неукорачиваемый - если слева длина <= чем справа
            // и 1 тип контекстнозависимой если пройдет все проверки
            // 0 тип - если все остальные false

            // проверка на 3 тип
            bool isRulesTypeThree = true;
            foreach (var rule in rulesArray) {
                // если слева больше одного символа
                if (rule[0].Length > 1) {
                    isRulesTypeThree = false;
                    // или если слева один символ, но терминальный
                } else if (GetCountOfNoTerminalSymbols(rule[0]) != 1) {
                    isRulesTypeThree = false;
                }

                // правая часть должна быть вида Аааа или аааА или ааа
                int countOfNoTerminalSymbolsRightRule = GetCountOfNoTerminalSymbols(rule[1]);
                if (countOfNoTerminalSymbolsRightRule > 1) {
                    isRulesTypeThree = false;
                    // если один нетерминальный, но не первый и не последний
                } else if (countOfNoTerminalSymbolsRightRule == 1 && !(char.IsUpper(rule[1][0]) || char.IsUpper(rule[1][rule[1].Length - 1]))) {
                    isRulesTypeThree = false;
                }

                if (rule[1] == "E") {
                    isRulesTypeThree = false;
                }
            }
            if (isRulesTypeThree) {
                Console.WriteLine("Эта грамматика является праволинейной - 3 тип");
                Environment.Exit(0);
            }

            // проверка на 2 тип
            bool isRulesTypeTwo = true;
            foreach (var rule in rulesArray) {
                int countOfNoTerminalSymbols = GetCountOfNoTerminalSymbols(rule[0]);
                // слева больше 1 символа
                if (rule[0].Length > 1) {
                    isRulesTypeTwo = false;
                    // слева больше одного нетерминального символа
                } else if (countOfNoTerminalSymbols > 1) {
                    isRulesTypeTwo = false;
                }
            }
            if (isRulesTypeTwo) {
                Console.WriteLine("Эта грамматика является контекстно-свободной - 2 тип");
                Environment.Exit(0);
            }

            // проверка на 1 тип
            bool isShortened = false;
            bool isNoContextSensitive = false;
            if (rulesArray.All(r => r[0].Length <= r[1].Length)) {
                Console.WriteLine("Эта грамматика является неукорачиваемой");
            } else {
                isShortened = true;
                Console.WriteLine("Эта грамматика является укорачиваемой");
            }
            foreach (var rule in rulesArray) {
                string leftRule = rule[0];
                string rightRule = rule[1];
                // считаем пси1 - совпадения в началах двух правил
                string psiOne = FindPrefix(new List<string> {
                    leftRule,
                    rightRule,
                });
                // считаем пси2 - совпадения в концах двух правил, заранее убрав пси1
                string psiTwo = FindPrefix(new List<string> {
                    new String(leftRule.Substring(psiOne.Length).Reverse().ToArray()),
                    new String(rightRule.Substring(psiOne.Length).Reverse().ToArray()),
                });
                // если у левого правила сумма длин пси + 1 равно длине левого правила и этот символ нетерминальный то является контекстно-зависимой
                // значит если одно из этих правил ложно, значит не является контекстно-зависимой 
                if (!(((psiOne.Length + psiTwo.Length + 1) == leftRule.Length) && char.IsUpper(leftRule[psiOne.Length]))) {
                    isNoContextSensitive = true;
                }
                // или если у правого сумма длин пси не равна длине правого правила, тогда является контекстно-зависимой
                // значит если длина сумм пси >= длины правого правила, то не является контекстно-зависимой
                if ((psiOne.Length + psiTwo.Length) >= rightRule.Length) {
                    isNoContextSensitive = true;
                }
            }
            if (isNoContextSensitive) {
                Console.WriteLine("Эта грамматика не является контекстно-зависимой");
            } else {
                Console.WriteLine("Эта грамматика является контекстно-зависимой");
            }
            // 1 тип - все кроме кейса когда укорачивающая + не контекстно-зависимая
            if (!(isShortened && isNoContextSensitive)) {
                Console.WriteLine("Эта грамматика является 1 типа");
                Environment.Exit(0);
            }

            Console.WriteLine("Эта грамматика является типа 0");
            Environment.Exit(0);
        }

        static void CheckHasAlphabetItemsInLine(List<string> terminalAlphabet, List<string> noTerminalAlphabet, string line) {
            char[] charLine = line.ToCharArray();
            foreach (var symbol in charLine) {
                if (char.IsUpper(symbol)) {
                    if (noTerminalAlphabet.FindIndex(n => n == (symbol + "")) == -1) {
                        Console.WriteLine("Неизвестный символ " + symbol);
                        Environment.Exit(0);
                    }
                } else {
                    if (terminalAlphabet.FindIndex(t => t == (symbol + "")) == -1) {
                        Console.WriteLine("Неизвестный символ " + symbol);
                        Environment.Exit(0);
                    }
                }
            }
        }

        static int GetCountOfNoTerminalSymbols(string line) {
            return line.ToList().FindAll(r => char.IsUpper(r)).Count;
        }

        // находит совпадения в началах строк
        public static string FindPrefix(IEnumerable<string> strings) {
            if (strings == null || !strings.Any()) {
                return string.Empty;
            }

            var prefix = strings
                .Aggregate((IEnumerable<char> string1, IEnumerable<char> string2) =>
                    string1.Zip(string2, (letter1, letter2) => new {
                        Letter1 = letter1,
                        Letter2 = letter2
                    })
                    .TakeWhile(pair => pair.Letter1 == pair.Letter2)
                    .Select(pair => pair.Letter1));

            return string.Join(string.Empty, prefix);
        }
    }
}
