using System;

// Описание и наследование классов

// Вариант 7.
// Бумага, газета, книга, журнал, учебник, плакат, картина, библиотека, газетный киоск.
// Классы должны содержать поля, свойства, обычные и виртуальные методы.
// Виртуальные методы должны иметь разную реализацию в базовых и производных классах
// Библиотека - газета, книга, журнал, учебник - бумага
// Газетный киоск - газета, бумага, журнал
namespace Lab2 {
    class Program {
        static void Main() {
            Library library = new Library();
            NewspaperStand newspaperStand = new NewspaperStand();

            WhitePapper whitePapper = new WhitePapper();
            Book book = new Book();
            Journal journal = new Journal();
            TextBook textBook = new TextBook();
            Poster poster = new Poster();
            Picture picture = new Picture();

            // journal.Created();
            // poster.Created();

            library.journals[0] = journal;
            library.whitePappers[0] = whitePapper;
            library.books[0] = book;
            library.journals[0] = journal;
            library.posters[0] = poster;
            library.pictures[0] = picture;
            library.textBooks[0] = textBook;

            newspaperStand.whitePappers[0] = whitePapper;
            newspaperStand.journals[0] = journal;

            // 18+ years
            Book privateBook = new Book();
            privateBook.age = 18;
            Kindergarten kindergarten = new Kindergarten();
            kindergarten.SetNewBooks(privateBook);

            // 1+ years
            Book noPrivateBook = new Book();
            noPrivateBook.age = 1;
            kindergarten.SetNewBooks(noPrivateBook);
        }
    }
    abstract class Paper {
        public string color { get; set; }
        public int countOfSymbols = 0;
        public virtual void Created() {
            Console.WriteLine("Paper created!");
        }
    }
    class WhitePapper : Paper {
        private double price = 0;
        public int countOfPages = 1;
        public double GetPrice() {
            return price;
        }
        public void SetPrice(double newPrice) {
            price = newPrice;
        }
        public override void Created() {
            Console.WriteLine("WhitePapper created!");
        }
    }

    class Book : Paper {
        public double price = 0;
        public int countOfPages = 1;
        public string author;
        public int age { get; set; }
        public override void Created() {
            Console.WriteLine("Book created!");
        }
    }

    class Journal : Paper {
        public double price = 0;
        public int countOfPages = 1;
        public string author;
        public bool hasPrivatInfo = false;
        private string privateInfo;
        public string GetPrivateInfo() {
            if (hasPrivatInfo) {
                return privateInfo;
            }
            return "You haven't access to private info";
        }
        public void SetPrivateInfo(string newPrivateInfo) {
            privateInfo = newPrivateInfo;
        }
        public override void Created() {
            Console.WriteLine("Journal created!");
        }
    }
    class TextBook : Paper {
        public double price = 0;
        public int countOfPages = 1;
        public string author;
        public string subject;
        public override void Created() {
            Console.WriteLine("TextBook created!");
        }
    }
    class Poster : Paper {
        public int width = 0;
        public int height = 0;
        public bool hasPrivatInfo = false;
        private string privateInfo;
        public string GetPrivateInfo() {
            if (hasPrivatInfo) {
                return privateInfo;
            }
            return "You haven't access to private info";
        }
        public void SetPrivateInfo(string newPrivateInfo) {
            this.privateInfo = newPrivateInfo;
        }
        public override void Created() {
            Console.WriteLine("Poster created!");
        }
    }
    class Picture : Paper {
        public string name;
        public string author;
        public int createdAt;
        public override void Created() {
            Console.WriteLine("Picture created!");
        }
    }
    // Библиотека - газета, книги, журнал, учебник
    class Library {
        public string address { get; set; }
        protected int positionInLibrary { get; set; }
        public WhitePapper[] whitePappers = new WhitePapper[5];
        public Book[] books = new Book[5];
        public Journal[] journals = new Journal[5];
        public TextBook[] textBooks = new TextBook[5];
        public Poster[] posters = new Poster[5];
        public Picture[] pictures = new Picture[5];
    }
    //Газетный киоск - газета, бумага, журнал
    class NewspaperStand {
        public string address { get; set; }
        protected int positionInLibrary { get; set; }
        public WhitePapper[] whitePappers = new WhitePapper[5];
        public Journal[] journals = new Journal[5];
    }

    // Детский сад - книги
    class Kindergarten {
        public Book[] books = new Book[5];
        public void SetNewBooks(Book newBook) {
            if (newBook.age > 5) {
                this.books[0] = newBook;
                Console.WriteLine("Book added!");
            } else {
                Console.WriteLine("Book not created. Age should be more then 5");
            }
        }
    }
}

// детский сад в котором возрастное ограничение 5+