using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Sentences
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files (*.txt)|*.txt|Word Files (*.doc)|*.doc|All Files (*.*)|*.*";

            bool? rslt = dlg.ShowDialog();
            if (rslt.Value == false) return;
            //To do: save extension
            // int ind = dlg.FilterIndex;
            //dlg.FilterIndex = ind;
            string str = File.ReadAllText(dlg.FileName);
            //List<string> strs = new List<string>(); // промежуточный набор с разделителем "..."
            List<string> ens = ParseSentences(str);
        }

        private List<string> ParseSentences(string a_str)
        {
            List<string> res = new List<string>();
            string dots = "ᚒ";
            string str = a_str.Replace("...", dots); // заменяю ... на спец символ
            str = Regex.Replace(str, @"\s+", " "); // заменяю множественные пробелы и новые строки на одиночные пробелы
            string[] sentences = Regex.Split(str/*it*/, @"(?<=[.!?" + dots + "])");
            foreach (string sntc in sentences)
            {
                if (string.IsNullOrWhiteSpace(sntc) == false)
                {
                    string s = sntc.Replace(dots, "...").Trim(); // если есть спец символ, заменяю его на ...
                    res.Add(s);
                }
            }
            return res;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class Main_VM
    {

    }
}
