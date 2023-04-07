using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Реализация регулярных выражений

// Е = { 0, 1, 2 }, допускает цепочки в которых не встречается рядом 0 и 2.
// E* = { 0, 1, 2, 00, 01, 10, 11, 12, 21, 22, 000, 001, 010, 011, 012, }
// (0 + 1 + 2)* 1 (0 + 1 + 2)*

namespace TALab2 {
    class Program {
        static void Main() {
            Console.WriteLine("Вариант 4: Цепочки в которых не встречается рядом 0 и 2");
            Console.WriteLine("Введите количество цепочек, которые вы хотите увидеть:");
            int countOfWords = int.Parse(Console.ReadLine());
            if (countOfWords < 1) {
                Console.WriteLine("Количество букв в алфавите должно быть больше или равно 1");
                Environment.Exit(0);
            }
            Console.WriteLine("\nРезультат:");
            int counter = 0;
            List<string> alphabet = new List<string>() { "0", "1", "2" };
            List<string> resultArray = new List<string>() { "0", "1", "2" };

            GenerateResultArray(resultArray, alphabet);

            while (counter < countOfWords) {
                Console.WriteLine(resultArray[counter]);
                counter++;
            }
        }

        static void GenerateResultArray(List<string> resultArray, List<string> alphabet) {
            // пробегаться по всем символам алфавита вложенностью positionInWord пока не найду подходящее условию
            // positionInWord увеличиваем если вся строка состоит из 2
            // нужно в цикле менять последние буквы до 2, потом предпоследние до 2, потом пред пред последние и тд
            string postfix = "0";

            // считаем все слова до длины 10
            for (int positionInWord = 1; positionInWord < 10;) {
                string word = "";
                for (int wordIndex = 0; wordIndex < alphabet.Count; wordIndex++) {
                    string newWord = postfix;
                    string symbol = alphabet[wordIndex];
                    if (newWord.Length == positionInWord) {
                        newWord += symbol;
                    } else {
                        newWord = newWord.Remove(positionInWord, 1).Insert(positionInWord, symbol);
                    }
                    //if (!newWord.Contains("02") && !newWord.Contains("20")) {
                    //    resultArray.Add(newWord);
                    //    word = newWord;
                    //}
                    if (Regex.IsMatch(newWord, @"^(?!.*02)(?!.*20)[012]+$")) {
                        resultArray.Add(newWord);
                        word = newWord;
                    }
                }
                // Если постфикс состоит только из двоек увеличиваем вложенность и меняем на все нули
                if (word == new string('2', positionInWord + 1)) {
                    positionInWord++;
                    postfix = new string('0', positionInWord);
                } else {
                    // Иначе идем с конца постфикса и ищем первую букву не равную 2
                    // Меняем её на соседнюю. Если она не последняя, то меняем все после неё 2 на 0.
                    for (int i = postfix.Length - 1; i >= 0; i--) {
                        if (postfix[i] != '2') {
                            if (postfix[i] == '0') {
                                postfix = postfix.Remove(i, 1).Insert(i, "1");
                                for (int j = postfix.Length - 1; j > i; j--) {
                                    postfix = postfix.Remove(j, 1).Insert(j, "0");
                                }
                                break;
                            } else if (postfix[i] == '1') {
                                postfix = postfix.Remove(i, 1).Insert(i, "2");
                                for (int j = postfix.Length - 1; j > i; j--) {
                                    postfix = postfix.Remove(j, 1).Insert(j, "0");
                                }
                                break;
                            }
                        }
                    }

                }
            }
        }
    }
}
