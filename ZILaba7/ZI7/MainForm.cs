using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ZI7 {
    public partial class MainForm : Form {
        private int countOfNotCorrectKeys = 0;
        private List<int> key = new List<int>();
        private List<User> users = new List<User>();
        private string INPUT_NAME_PLACEHOLDER = "Введите имя пользователя";
        private static Color ACTIVE = Color.GreenYellow;
        private static Color DISABLE = SystemColors.ActiveCaption;
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        public MainForm() {
            InitializeComponent();
            myTimer.Tick += new EventHandler(TimerCallbackFunction);
            myTimer.Interval = 30000;
            myTimer.Start();
        }

        private void button10_Click(object sender, EventArgs e) {
            if (textBox1.Text == "" || textBox1.Text == INPUT_NAME_PLACEHOLDER) {
                writeErrorText(INPUT_NAME_PLACEHOLDER);
                return;
            }

            if (key.Count == 0) {
                writeErrorText("Введите ключ");
                return;
            }

            User user = users.FirstOrDefault(x => x.Name.Equals(textBox1.Text));

            if (user == null) {
                users.Add(new User(textBox1.Text, key.ToArray()));
                writeSuccessText("Регистрация прошла успешно");
                removeEnteredKey();
                textBox1_OnLoseFocus();
            } else {
                writeErrorText("Такой пользователь уже существует");
            }

        }

        private void button11_Click(object sender, EventArgs e) {
            if (textBox1.Text == "") {
                writeErrorText(INPUT_NAME_PLACEHOLDER);
                return;
            }

            if (key.Count == 0) {
                writeErrorText("Введите ключ");
                return;
            }

            string name = textBox1.Text;

            User user = users.FirstOrDefault(x => x.Name.Equals(name) && x.Key.SequenceEqual(key.ToArray()));

            if (user != null) {
                writeSuccessText($"Добро пожаловать, {user.Name} и до свидания");
                removeEnteredKey();
                textBox1_OnLoseFocus();
            } else {
                countOfNotCorrectKeys++;
                writeErrorTextWithAwait("Неверно указаны данные", countOfNotCorrectKeys);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!button1.BackColor.Equals(ACTIVE)) {
                button1.BackColor = ACTIVE;
                key.Add(1);
            } else {
                button1.BackColor = DISABLE;
                key.Remove(1);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (!button2.BackColor.Equals(ACTIVE)) {
                button2.BackColor = ACTIVE;
                key.Add(2);
            } else {
                button2.BackColor = DISABLE;
                key.Remove(2);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (!button3.BackColor.Equals(ACTIVE)) {
                button3.BackColor = ACTIVE;
                key.Add(3);
            } else {
                button3.BackColor = DISABLE;
                key.Remove(3);
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            if (!button4.BackColor.Equals(ACTIVE)) {
                button4.BackColor = ACTIVE;
                key.Add(4);
            } else {
                button4.BackColor = DISABLE;
                key.Remove(4);
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            if (!button5.BackColor.Equals(ACTIVE)) {
                button5.BackColor = ACTIVE;
                key.Add(5);
            } else {
                button5.BackColor = DISABLE;
                key.Remove(5);
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            if (!button6.BackColor.Equals(ACTIVE)) {
                button6.BackColor = ACTIVE;
                key.Add(6);
            } else {
                button6.BackColor = DISABLE;
                key.Remove(6);
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            if (!button7.BackColor.Equals(ACTIVE)) {
                button7.BackColor = ACTIVE;
                key.Add(7);
            } else {
                button7.BackColor = DISABLE;
                key.Remove(7);
            }
        }

        private void button8_Click(object sender, EventArgs e) {
            if (!button8.BackColor.Equals(ACTIVE)) {
                button8.BackColor = ACTIVE;
                key.Add(8);
            } else {
                button8.BackColor = DISABLE;
                key.Remove(8);
            }
        }

        private void button9_Click(object sender, EventArgs e) {
            if (!button9.BackColor.Equals(ACTIVE)) {
                button9.BackColor = ACTIVE;
                key.Add(9);
            } else {
                button9.BackColor = DISABLE;
                key.Remove(9);
            }
        }

        public void writeErrorText(string errorText) {
            MessageBox.Show(errorText, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void writeSuccessText(string errorText) {
            MessageBox.Show(errorText, "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void writeErrorTextWithAwait(string errorText, int countOfNotCorrectKeys) {
            if (countOfNotCorrectKeys == 5) {
                MessageBox.Show(errorText + "\nВвод заблокирован на 30 секунд", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button10.Enabled = false;
                button11.Enabled = false;
                myTimer.Start();
            } else {
                MessageBox.Show(errorText, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void TimerCallbackFunction(Object myObject, EventArgs myEventArgs) {
            button10.Enabled = true;
            button11.Enabled = true;
            myTimer.Stop();
            countOfNotCorrectKeys = 0;
        }

        private void textBox1_OnFocus(object sender, EventArgs e) {
            this.textBox1.Text = "";
        }

        private void textBox1_OnLoseFocus() {
            this.textBox1.Text = INPUT_NAME_PLACEHOLDER;
        }

        private void removeEnteredKey() {
            this.countOfNotCorrectKeys = 0;
            this.key = new List<int>();
            this.button1.BackColor = DISABLE;
            this.button2.BackColor = DISABLE;
            this.button3.BackColor = DISABLE;
            this.button4.BackColor = DISABLE;
            this.button5.BackColor = DISABLE;
            this.button6.BackColor = DISABLE;
            this.button7.BackColor = DISABLE;
            this.button8.BackColor = DISABLE;
            this.button9.BackColor = DISABLE;
        }

    }

    public class User {
        private string name;

        private int[] key;

        public User(string name, int[] key) {
            this.name = name;
            this.key = key;
        }

        public string Name {
            get => name;
            set => name = value;
        }

        public int[] Key {
            get => key;
            set => key = value;
        }
    }
}