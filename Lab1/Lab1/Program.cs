using System;
using System.Linq;

// Работа с исключениями
// Решение задач

namespace Lab1 {
    class Program {
        static void Main() {
            //FirstTask();
            //SecondTask();
            //ThirdTask();
            FourthTask();
        }

        static void FirstTask() {
            Console.WriteLine("Please enter array of numbers:");
            string[] stringArray = Console.ReadLine().Split(" ");
            int[] nuumberArray = Array.Empty<int>();
            try {
                nuumberArray = stringArray.Select(value => Convert.ToInt32(value)).ToArray();
            } catch (FormatException) {
                ErrorHandler("Can't correct cast string to number!");
            }
            int[] sortedArray = Sort(nuumberArray);
            FindLargestSequence(sortedArray);
        }

        static void SecondTask() {
            int[,] array = Enter2DArray();
            FindSum(array);
        }

        static void ThirdTask() {
            int[,] firstArray = Enter2DArray();
            int[,] secondArray = Enter2DArray();
            FindMatches(firstArray, secondArray);
        }

        static void FourthTask() {
            int[,] firstArray = Enter2DArray();
            int[,] secondArray = Enter2DArray();
            Multiplication2DArray(firstArray, secondArray);
        }

        static public int[] Sort(int[] array) {
            try {
                // Поиск восходящей подпоследовательности и сдвиг остальных элементов чтобы добавить текущий элемент, если он подходит
                for (int i = 1; i < array.Length; i++) {
                    int currentValue = array[i];
                    int newPositionIndex = i - 1;
                    for (; newPositionIndex >= 0 && array[newPositionIndex] > currentValue; newPositionIndex--) {
                        array[newPositionIndex + 1] = array[newPositionIndex];
                    }
                    array[newPositionIndex + 1] = currentValue;
                    string joindedArray = string.Join(" ", array);
                    Console.WriteLine(joindedArray);
                }
                return array;
            } catch (IndexOutOfRangeException) {
                ErrorHandler("Out of range!");
                return Array.Empty<int>();
            }
        }

        static public void FindLargestSequence(int[] array) {
            int lenghtLargestSequence = 1;
            int valueLargestSequence = array[0];
            try {
                for (int i = 0; i < array.Length - 1; i++) {
                    int tmpLenghtLargestSequence = 1;
                    // Идем по одинаковым элементам
                    for (int j = i + 1; j < array.Length && array[j] == array[i]; j++) {
                        tmpLenghtLargestSequence++;
                    }
                    if (tmpLenghtLargestSequence > lenghtLargestSequence) {
                        lenghtLargestSequence = tmpLenghtLargestSequence;
                        valueLargestSequence = array[i];
                        // Пропускаем уже просмотренную последовательность
                        i = i + tmpLenghtLargestSequence - 1;
                    }
                }
            } catch (IndexOutOfRangeException) {
                ErrorHandler("Out of range!");
            }
            Console.WriteLine("Largest sequence is: {0}, Length: {1}", valueLargestSequence, lenghtLargestSequence);
        }

        static public void ErrorHandler(string err) {
            Console.WriteLine(err);
            Console.WriteLine("Enter 1 for continue or another key for finish work.");
            int entered = Console.Read();
            // Код 1 равен 49
            if (entered != 49) {
                Environment.Exit(2);
            }
        }

        static public int[,] Enter2DArray() {
            try {
                Console.WriteLine("Let's enter new 2D array");
                Console.WriteLine("Please enter count of Columns");
                int columns = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please enter count of Rows");
                int rows = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Please enter 1 number in 1 line:");
                if (columns == 0 || rows == 0) {
                    ErrorHandler("Count of columns or rows can't be null");
                }
                int[,] array = new int[rows, columns];
                for (int i = 0; i < rows; i++) {
                    for (int j = 0; j < columns; j++) {
                        array[i, j] = Convert.ToInt32(Console.ReadLine());
                    }
                }
                return array;
            } catch (FormatException) {
                ErrorHandler("Can't correct cast string to number!");
                int[,] emptyArray = new int[0, 0];
                return emptyArray;
            }
        }

        static public void FindSum(int[,] array) {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);
            int sum = rows + columns;
            try {
                // Главная ось
                // От вернего левого угла до правой стенки или угла
                for (int i = 0; i < rows && i < columns; i++) {
                    sum += array[i, i];
                }

                // Побочная ось
                // От левого нижнего угла до правой стенки или угла
                for (int i = 0; i < rows && i < columns; i++) {
                    sum += array[rows - i - 1, i];
                }
            } catch (IndexOutOfRangeException) {
                ErrorHandler("Out of range!");
            }
            Console.WriteLine("Sum = {0}", sum);
        }

        static public void FindMatches(int[,] firstArray, int[,] secondArray) {
            int rowsFirstArray = firstArray.GetLength(0);
            int columnsFirstArray = firstArray.GetLength(1);
            int rowsSecondArray = secondArray.GetLength(0);
            int columnsSecondArray = secondArray.GetLength(1);
            if (rowsFirstArray < rowsSecondArray || columnsFirstArray < columnsSecondArray) {
                Console.WriteLine("Second array can't be more than first");
            } else {
                try {
                    int xIndexMatchedElement = 0;
                    int yIndexMatchedElement = 0;
                    // Достаточно проверить первые элементы первого массива по X
                    for (int j = 0; j <= rowsFirstArray - rowsSecondArray; j++) {
                        // Достаточно проверить первые элементы первого массива по Y
                        for (int i = 0; i <= columnsFirstArray - columnsSecondArray; i++) {
                            // Для оптимизации начинаем поиск только если первый элемент совпал
                            if (firstArray[j, i] == secondArray[yIndexMatchedElement, xIndexMatchedElement]) {
                                for (int l = j; l < j + rowsSecondArray; l++) {
                                    for (int k = i; k < i + columnsSecondArray; k++) {
                                        if (firstArray[l, k] == secondArray[yIndexMatchedElement, xIndexMatchedElement]) {
                                            xIndexMatchedElement++;
                                        } else {
                                            xIndexMatchedElement = 0;
                                            break;
                                        }
                                    }
                                    if (xIndexMatchedElement == 0) {
                                        break;
                                    } else {
                                        xIndexMatchedElement = 0;
                                        yIndexMatchedElement++;
                                    }
                                }
                                // Перестаем искать потому что уже нашли
                                if (yIndexMatchedElement == rowsSecondArray) {
                                    break;
                                } else {
                                    yIndexMatchedElement = 0;
                                    xIndexMatchedElement = 0;
                                }
                            } else {
                                yIndexMatchedElement = 0;
                            }
                        }
                        // Перестаем искать потому что уже нашли
                        if (yIndexMatchedElement == rowsSecondArray) {
                            break;
                        }
                    }

                    if (yIndexMatchedElement == rowsSecondArray) {
                        Console.WriteLine("First array has second array!");
                    } else {
                        Console.WriteLine("First array hasn't second array!");
                    }
                } catch (IndexOutOfRangeException) {
                    ErrorHandler("Out of range!");
                }
            }
        }

        static public void Multiplication2DArray(int[,] firstArray, int[,] secondArray) {
            int[,] resultArray = new int[firstArray.GetLength(0), secondArray.GetLength(1)];

            for (int i = 0; i < firstArray.GetLength(0); i++) {
                for (int j = 0; j < secondArray.GetLength(1); j++) {
                    for (int k = 0; k < secondArray.GetLength(0); k++) {
                        resultArray[i, j] += firstArray[i, k] * secondArray[k, j];
                    }
                }
            }
            for (int i = 0; i < resultArray.GetLength(0); i++) {
                for (int j = 0; j < resultArray.GetLength(1); j++) {
                    Console.Write(resultArray[i, j] + " ");
                }
                Console.Write('\n');
            }
            Console.WriteLine("First array has second array!");
        }
    }
}
