using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scraping
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cardname = textBox1.Text;

            var result = Scraping.MCISource(cardname);

            textBox2.Text = result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var source = Scraping.MCISource(textBox1.Text);
            var ci = Scraping.ScrapeMCIForCard(textBox1.Text, source);
        }
    }
}
