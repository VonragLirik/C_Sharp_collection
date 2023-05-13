using System;
using System.Collections.Generic;

// У админа есть все права на всех пользователей
// Вариант 4
// 6 пользователей
// 5 объектов
// мандатная политика

namespace ZashitaInformaciiLaba3 {
    class Program {
        static void Main() {
            Console.WriteLine("Лаба 3");
            Console.WriteLine("Выполнил Гарнов Кирилл\n");
            var system = new SecuritySystem(6, 5);

            while (true) {
                if (system.AuthorizedUser == null) {
                    system.getAllUsersList();
                    system.getListObj();
                    Console.WriteLine("\nВведите id пользователя под которым хотите авторизоваться:");
                    var id = int.Parse(Console.ReadLine());
                    if (id < system.Users.Length && id >= 0) {
                        system.authorization(id);
                    } else {
                        Console.WriteLine("Такого пользователя нет!");
                        continue;
                    }
                }

                Console.WriteLine("\nЖду ваших указаний: ");
                var command = Console.ReadLine();
                int objId;

                switch (command) {
                    case "quit":
                        Console.WriteLine("Работа завершена, досвидания.");
                        system.quit();
                        break;
                    case "read":
                        Console.WriteLine("Введите id объекта который будет прочитан:");
                        objId = int.Parse(Console.ReadLine());
                        system.read(objId);
                        break;
                    case "write":
                        Console.WriteLine("Введите id объекта который будет перезаписан:");
                        objId = int.Parse(Console.ReadLine());
                        system.write(objId);
                        break;
                    default:
                        Console.WriteLine("Такой команды нет");
                        break;
                }
            }

        }
    }


    public class Object {
        private int id;
        public int currentPermission { get; set; }

        public Object(int id, int permission) {
            this.id = id;
            currentPermission = permission;
        }

        public string Name {
            get => $"Объект с id = {id}. Доступ: {SecuritySystem.translations[(SecuritySystem.PERMISSIONS)currentPermission]}";
        }
    }

    public class SecuritySystem {
        private User[] users;
        private Object[] objects;
        private User myUser;
        Random rnd = new Random();
        public User AuthorizedUser {
            get => myUser;
        }

        public enum PERMISSIONS {
            OPEN = 0,
            CONFIDENTIAL = 1,
            SECRET = 2,
            SUPER_SECRET = 3,
        };

        public static Dictionary<PERMISSIONS, string> translations = new Dictionary<PERMISSIONS, string>()
        {
            { PERMISSIONS.OPEN, "Открытые данные"},
            { PERMISSIONS.CONFIDENTIAL, "Конфеденциальные данные"},
            { PERMISSIONS.SECRET, "Секретные данные"},
            { PERMISSIONS.SUPER_SECRET, "Совершенно секретные данные"},
        };

        public SecuritySystem(int subjectCount, int objectCount) {
            users = new User[subjectCount];
            objects = new Object[objectCount];

            for (int i = 0; i < users.Length; i++) {
                users[i] = new User(i, rnd.Next(0, User.nameDictionary.Length - 1), rnd.Next(0, 4));
            }

            for (int i = 0; i < objects.Length; i++) {
                objects[i] = new Object(i, rnd.Next(0, 4));
            }
        }

        public void getAllUsersList() {
            Console.WriteLine("Пользователи определенные в системе:");

            foreach (var user in users) {
                Console.WriteLine(user.Name);
            }
        }

        public void getListObj() {
            Console.WriteLine("\nОбъекты определенные в системе:");

            foreach (var obj in objects) {
                Console.WriteLine(obj.Name);
            }
        }

        public void read(int objId) {
            if (myUser.currentPermission >= objects[objId].currentPermission) {
                Console.WriteLine("Чтение прошло успешно!");
                return;
            }
            Console.WriteLine("У вас нет прав на чтение этого файла!");
        }

        public void write(int objId) {
            if (myUser.currentPermission <= objects[objId].currentPermission) {
                Console.WriteLine("запись прошла успешно!");
                return;
            }
            Console.WriteLine("У вас нет прав на запись этого файла!");
        }

        public void authorization(int id) {
            myUser = users[id];
        }

        public void quit() {
            myUser = null;
        }

        public User[] Users {
            get => users;
        }
    }

    public class User {
        private int id;
        private string name;
        public int currentPermission { get; set; }

        public static readonly string[] nameDictionary = {
            "Junette","Junia","Junie","Junina","Justina","Justine","Justinn","Jyoti",
            "Kacey","Kacie","Kacy","Kaela","Kai","Kaia","Kaila","Kaile","Kailey","Kaitlin",
            "Kaitlyn","Kaitlynn","Kaja","Kakalina","Kala","Kaleena","Kali","Kalie","Kalila",
            "Kalina","Kalinda","Kalindi","Kalli","Kally","Kameko","Kamila","Kamilah","Kamillah",
            "Kandace","Kandy","Kania","Kanya","Kara","Kara-Lynn","Karalee","Karalynn","Kare",
            "Karee","Karel","Karen","Karena","Kari","Karia","Karie","Karil","Karilynn","Karin",
            "Kathleen","Aria","Kayla","Annabelle","Gianna","Kennedy","Stella","eagan","Julia",
            "Bailey","Alexandra","Jordyn","Nora","Carolin","Mackenzie","Jasmine","Jocelyn",
            "Kendall","Morgan","Nevaeh","Maria","Eva","Juliana","Abby","Alexa","Summer",
            "Booke","Penelope","Violet","Kate","Hadley","Ashlyn","Sadie","Paige","Katherine", "Kirill"
        };

        public int Id {
            get => id;
        }

        public string Name {
            get {
                return $"Пользователь {name} с id = {id}. Доступ: {SecuritySystem.translations[(SecuritySystem.PERMISSIONS)currentPermission]}";
            }
        }

        public User(int id, int nameId, int permission) {
            this.id = id;
            name = nameDictionary[nameId];
            currentPermission = permission;
        }
    }
}