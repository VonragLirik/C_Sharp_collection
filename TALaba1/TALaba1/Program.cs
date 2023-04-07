using System;
using System.Linq;
using System.Collections.Generic;

// Поиск мощности алфавита по цепочке и наоборот

// N = n^k-1*i1 + n^k-2*i2 + ... + ik
// n - мощность алфавита
// k - размер слова
// ik - номер к-той буквы в алфавите
namespace TALaba1 {
    class Program {
        static void Main() {
            Console.WriteLine("Введите количество букв в алфавите");
            int countAlphabetSymbols = int.Parse(Console.ReadLine());
            if (countAlphabetSymbols < 1) {
                Console.WriteLine("Количество букв в алфавите должно быть больше или равно 1");
                Environment.Exit(0);
            }
            Console.WriteLine("Введите буквы алфавита через пробел");
            string alphabetWithSpaces = Console.ReadLine();
            List<string> alphabet = alphabetWithSpaces.Split(" ").ToList().Distinct().ToList();
            if (alphabet.Count != countAlphabetSymbols) {
                Console.WriteLine("Все буквы алфавита должны быть уникальными");
                Environment.Exit(0);
            }
            foreach (string symbol in alphabet) {
                if (symbol.Length > 1) {
                    Console.WriteLine("В алфавите все символы должны иметь длину 1");
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Чтобы найти по номеру нажмите 1, чтобы найти по слову нажмите 2");
            int actionNumber = int.Parse(Console.ReadLine());

            if (actionNumber == 1) {
                Console.WriteLine("Введите номер");
                int number = int.Parse(Console.ReadLine());
                if (number == 0) {
                    Console.WriteLine("Соответствует пустому слову");
                    Environment.Exit(0);
                } else if (number < countAlphabetSymbols) {
                    Console.WriteLine("Соответствует слову: " + alphabet[number - 1]);
                    Environment.Exit(0);
                } else {
                    string word = "";
                    while ((number / countAlphabetSymbols) > 0) {
                        int proc = number % countAlphabetSymbols;
                        if (proc == 0) {
                            number /= countAlphabetSymbols;
                            number -= 1;
                            word += alphabet[countAlphabetSymbols - 1];
                            Console.WriteLine("( " + number + " * " + countAlphabetSymbols + " ) + " + countAlphabetSymbols);
                        } else {
                            number -= proc;
                            number /= countAlphabetSymbols;
                            word += alphabet[proc - 1];
                            Console.WriteLine("( " + number + " * " + countAlphabetSymbols + " ) + " + proc);
                        }
                    }
                    int procent = number % countAlphabetSymbols;
                    if (procent > 0) {
                        word += alphabet[procent - 1];
                    }
                    string reversedWord = String.Join("", word.ToCharArray().Reverse().ToArray());
                    GetNumberByWord(reversedWord, countAlphabetSymbols, alphabet);
                    Console.WriteLine("\nИскомое слово: " + reversedWord);
                }
            } else if (actionNumber == 2) {
                Console.WriteLine("Введите слово");
                string word = Console.ReadLine();
                double N = GetNumberByWord(word, countAlphabetSymbols, alphabet);
                Console.Write(" = " + N);
            }
        }

        static double GetNumberByWord(string word, int countAlphabetSymbols, List<string> alphabet) {
            double N = 0;
            Console.Write("N = ");
            for (int i = 1; i <= word.Length; i++) {
                string symbol = word[i - 1] + "";
                if (alphabet.FindIndex(s => s == symbol) != -1) {
                    int indexSymbolInAlphabet = alphabet.FindIndex(s => s == symbol) + 1;
                    N += Math.Pow(countAlphabetSymbols, word.Length - i) * indexSymbolInAlphabet;
                    Console.Write(countAlphabetSymbols + " ^ " + (word.Length - i) + " * " + indexSymbolInAlphabet);
                    if (i != word.Length) {
                        Console.Write(" + ");
                    }
                } else {
                    Console.WriteLine("Неизвестный символ");
                    Environment.Exit(0);
                }
            }
            return N;
        }
    }
}
