using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba8 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(1);
        }

        public void OpenProcessByNomerLab(int numberLab) {
            Process.Start("C:/Users/user/source/repos/C_Sharp_collection/laba8/laba8/labs/laba"+ numberLab + "/Laba" + numberLab + ".exe");
        }

        private void button2_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(2);
        }

        private void button3_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(3);
        }

        private void button4_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(4);
        }

        private void button5_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(5);
        }

        private void button6_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(6);
        }

        private void button7_Click(object sender, EventArgs e) {
            OpenProcessByNomerLab(7);
        }
    }
}
