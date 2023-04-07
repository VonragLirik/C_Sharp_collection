using System;
using System.Linq;
using System.Collections;

// Работа с IEnumerable и IEnumerator
// Работа с функцией-итератором

// Реализовать в виде последовательности генерацию первых N простых чисел.
// Сначала с помощью IEnumerable и IEnumerator
// Затем с помощью функции-итератора

namespace laba4 {
    class Program {
        static void Main() {
            int N;

            Console.WriteLine("Please write N > 1");
            try {
                N = int.Parse(Console.ReadLine());
                if (N > 1) {
                    Console.WriteLine();
                    FirstTask(N);
                    //SecondTask(N);
                } else {
                    Console.WriteLine("N should be more than 1");
                    Environment.Exit(2);
                }
            } catch (FormatException) {
                Console.WriteLine("Can't correct cast string to number!");
                Environment.Exit(2);
            }
        }

        public class FirstTaskEnumerator : IEnumerator {

            int position = -1;
            int[] digits;
            public FirstTaskEnumerator(int[] digits) => this.digits = digits;
            public object Current {
                get {
                    if (position == -1 || position >= digits.Length) {
                        throw new ArgumentException();
                    }
                    return digits[position];
                }
            }
            public bool MoveNext() {
                if (position < digits.Length - 1) {
                    position++;
                    return true;
                } else {
                    return false;
                }
            }
            public void Reset() => position = -1;
        }

        class FirstTaskEnumerable: IEnumerable {
            int[] numberArray;
            public FirstTaskEnumerable(int[] numberArray) => this.numberArray = numberArray;
            public IEnumerator GetEnumerator() {
                return new FirstTaskEnumerator(numberArray);
            }
        }


        static void FirstTask(int N) {
            int[] numberArray = new int[N - 2];
            for(int i = 2; i < N; i++) {
                numberArray[i - 2] = i;
            }

            //Инициируем кастомный IEnumerable
            IEnumerable numerableNumberArray = new FirstTaskEnumerable(numberArray);
            foreach (int number in numerableNumberArray) {
                if (IsSimple(number)) {
                    Console.Write(number + " ");
                }
            }
        }

        /////////////////////////////////////////////////////////////////
        static IEnumerable SecondTaskEnumerable(int[] numberArray) {
            for (int i = 0; i < numberArray.Length - 1; i++) {
                yield return numberArray[i];
            }
        }

        static void SecondTask(int N) {
            int[] numberArray = new int[N - 2];
            for (int i = 2; i < N; i++) {
                numberArray[i - 2] = i;
            }
            var numberIter = SecondTaskEnumerable(numberArray);
            foreach (int number in numberIter) {
                if (IsSimple(number)) {
                    Console.Write(number + " ");
                }
            }
        }

        /////////////////////////////////////////////////////////////////
        // Число простоe если не делится на числа до его половины
        static bool IsSimple(int N) {
            // Исключение из правила
            if (N == 4) return false;
            for (int i = 2; i < (int)(N / 2); i++) {
                if (N % i == 0) {
                    return false;
                }
            }
            return true;
        }
    }
}
