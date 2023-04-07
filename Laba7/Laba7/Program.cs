using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

// мини-компилятор(интерпритатор выражений)
// вариант 2
// парсит файл и в заранее определенные словари записывает значения, указанные в файле

namespace Laba7 {
    class Program {
        static void Main() {
            string currentPath = Path.GetDirectoryName("C:/Users/user/source/repos/laba7/laba7/");
            string inputFileName = "\\input.txt";
            if (!File.Exists(currentPath + inputFileName)) {
                Console.WriteLine("File not found!");
                System.Environment.Exit(2);
            }

            string name = "";
            int age = 0;
            var variablesDictionary = new Dictionary<string, object>() {
                { "name", name },
                { "age", age },
            };
            var objectsDictionary = new Dictionary<string, object>() {
                { "person", new Person() },
                { "car", new Car() },
            };

            string[] lines = File.ReadAllLines(@currentPath + inputFileName);

            foreach (string line in lines) {
                try {
                    string[] splittedLine = line.Split(" = ");
                    string variableName = splittedLine[0];
                    bool isDone = false;

                    // Если есть что-то после равно - присваиваем значение, иначе это метод и просто исполняем его
                    if (splittedLine.Length == 2) {
                        // Тут может быть 3 кейса:
                        // Если в variableName нет . и в value нет ( значит это присвоение значения переменной из поля класса
                        // Если в variableName нет . и в value есть ( это вызов метод у класса, причем с присваиваемым значением
                        // Если в variableName есть . значит это присвоение полю значения
                        string value = splittedLine[1];

                        // Вызов метода и присвоение результата
                        if (variableName.IndexOf(".") == -1 && value.IndexOf("(") >= 0) {
                            string className = value.Split(".")[0];
                            string methodName = value.Split(".")[1].Split("(")[0];
                            string arg = value.Split(".")[1].Split("(")[1].Split(")")[0].Replace("\"", "");
                            foreach (var variable in variablesDictionary) {
                                // Проверяем название переменной по словарю
                                if (variableName == variable.Key) {
                                    string result = CallMethod(className, methodName, arg, objectsDictionary);
                                    if (result != "") {
                                        variablesDictionary[variable.Key] = CallMethod(className, methodName, arg, objectsDictionary);
                                        Console.WriteLine("Успешно прошел вызов метода и присвоение результата!");
                                        isDone = true;
                                        break;
                                    }
                                }
                            }
                            // Присвоение значение переменной из поля класса
                        } else if (variableName.IndexOf(".") == -1 && value.IndexOf("(") == -1) {
                            string className = value.Split(".")[0];
                            string fieldName = value.Split(".")[1];
                            foreach (var variable in variablesDictionary) {
                                // Проверяем название переменной по словарю
                                if (variableName == variable.Key) {
                                    string result = FindFieldInClass(className, fieldName, objectsDictionary);
                                    if (result != "") {
                                        variablesDictionary[variable.Key] = result;
                                        Console.WriteLine("Успешно прошло присвоение значение переменной из поля класса!");
                                        isDone = true;
                                        break;
                                    }
                                }
                            }
                            // Присвоение полю класса значения
                        } else if (variableName.IndexOf(".") >= 0) {
                            string className = variableName.Split(".")[0];
                            string methodName = variableName.Split(".")[1];
                            string arg = value.Replace("\"", "");
                            ChangeFieldInClass(className, methodName, arg, objectsDictionary);
                            Console.WriteLine("Успешно прошло присвоение полю класса значения!");
                            isDone = true;
                        }
                    } else {
                        string className = variableName.Split(".")[0];
                        string methodName = variableName.Split(".")[1].Split("(")[0];
                        string arg = variableName.Split(".")[1].Split("(")[1].Split(")")[0].Replace("\"", "");
                        string result = CallMethod(className, methodName, arg, objectsDictionary);
                        if (result != "") {
                            Console.WriteLine("Успешно вызван метод!");
                            isDone = true;
                        }
                    }

                    if (isDone) {
                        Console.WriteLine("VariablesDictonary ");
                        foreach (var variable in variablesDictionary) {
                            Console.Write($"key: {variable.Key},  value: {variable.Value} \n");
                        }
                        Console.WriteLine("objectsDictonary ");
                        foreach (var customObject in objectsDictionary) {
                            Type type = customObject.Value.GetType();
                            if (type.GetField("name") != null) {
                                Console.Write($"key: name,  value: {type.GetField("name").GetValue(customObject.Value)} \n");
                            }
                            if (type.GetField("age") != null) {
                                Console.Write($"key: age,  value: {type.GetField("age").GetValue(customObject.Value)} \n");
                            }
                            if (type.GetField("countOfWheels") != null) {
                                Console.Write($"key: countOfWheels,  value: {type.GetField("countOfWheels").GetValue(customObject.Value)} \n");
                            }
                        }
                    } else {
                        Console.WriteLine("InvalidOperationException");
                    }

                    Console.WriteLine();

                } catch (InvalidOperationException) {
                    Console.WriteLine("InvalidOperationException");
                } catch (IndexOutOfRangeException) {
                    Console.WriteLine("IndexOutOfRangeException");
                }
            }

            // Метод для вызова неопределенного метода
            string CallMethod(string className, string methodName, string arg, Dictionary<string, object> objectsDictionary) {
                // Пробегаемся по всем классам из словаря
                foreach (var customObject in objectsDictionary) {
                    // Проверяем название класса по словарю
                    if (className == customObject.Key) {
                        Type type = customObject.Value.GetType();
                        // Смотрим есть ли метод у этого класса, если есть выполняем и возвращаем результат
                        if (type.GetMethod(methodName) != null) {
                            MethodInfo method = type.GetMethod(methodName);
                            if (int.TryParse(arg, out _)) {
                                int.TryParse(arg, out int intArg);
                                return method.Invoke(customObject.Value, new Object[] { intArg }).ToString();
                            } else {
                                return method.Invoke(customObject.Value, new Object[] { arg }).ToString();
                            }
                        }
                    }
                }
                return "";
            }

            // Поиск в классах словаря определенного поля
            string FindFieldInClass(string className, string fieldName, Dictionary<string, object> objectsDictionary) {
                // Пробегаемся по всем классам из словаря
                foreach (var customObject in objectsDictionary) {
                    // Проверяем название класса по словарю
                    if (className == customObject.Key) {
                        Type type = customObject.Value.GetType();
                        // Если есть поле берем его значение
                        if (type.GetField(fieldName) != null) {
                            return type.GetField(fieldName).GetValue(customObject.Value).ToString();
                        }
                    }
                }
                return "";
            }

            // Изменение поля в классах из словаря
            void ChangeFieldInClass(string className, string fieldName, string arg, Dictionary<string, object> objectsDictionary) {
                // Пробегаемся по всем классам из словаря
                foreach (var customObject in objectsDictionary) {
                    // Проверяем название класса по словарю
                    if (className == customObject.Key) {
                        Type type = customObject.Value.GetType();
                        // Если есть поле берем его значение
                        if (type.GetField(fieldName) != null) {
                            // если число, то сэтим число, иначе строка
                            if (int.TryParse(arg, out _)) {
                                int.TryParse(arg, out int intArg);
                                type.GetField(fieldName).SetValue(customObject.Value, intArg);
                            } else {
                                type.GetField(fieldName).SetValue(customObject.Value, arg);
                            }
                        }
                    }
                }
            }
        }

        class Person {
            public string name = "name123";
            public int age = 5;
            public string AddName(string newName) {
                this.name = newName;
                return newName + " added";
            }
        }

        class Car {
            public int countOfWheels = 4;
            public string ChangeCountOfWheels(int newCountOfWheels) {
                this.countOfWheels = newCountOfWheels;
                return newCountOfWheels.ToString();
            }
        }
    }
}
