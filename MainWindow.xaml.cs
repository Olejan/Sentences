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
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Word;
using System.Reflection;

namespace Sentences
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Microsoft.Office.Interop.Word._Application m_app;
        Microsoft.Office.Interop.Word._Document m_doc;

        Object missingObj = System.Reflection.Missing.Value;
        Object trueObj = true;
        Object falseObj = false;


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
            List<string> ens = ParseSentences(str);

            SetRTBText(rtbEn);
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

        private void CreateWordDocument()
        {
            //создаем обьект приложения word
            m_app = new Microsoft.Office.Interop.Word.Application();
            // создаем путь к файлу
            Object templatePathObj = System.IO.Path.GetTempFileName();

            // если вылетим не этом этапе, приложение останется открытым
            try
            {
                m_doc = m_app.Documents.Add(ref templatePathObj, ref missingObj, ref missingObj, ref missingObj);
            }
            catch (Exception error)
            {
                m_doc.Close(ref falseObj, ref missingObj, ref missingObj);
                m_app.Quit(ref missingObj, ref missingObj, ref missingObj);
                m_doc = null;
                m_app = null;
                throw error;
            }
            //m_app.Visible = true;
        }

        private void Btn_PasteRu_Click(object sender, RoutedEventArgs e)
        {
            SetRTBText(rtbRu);
        }
        private void Btn_ClearRu_Click(object sender, RoutedEventArgs e)
        {
            rtbRu.Document.Blocks.Clear();
        }

        private void Btn_PasteEn_Click(object sender, RoutedEventArgs e)
        {
            SetRTBText(rtbEn);
        }
        private void Btn_ClearEn_Click(object sender, RoutedEventArgs e)
        {
            rtbEn.Document.Blocks.Clear();
        }

        private void SetRTBText(RichTextBox a_rtb)
        {
            a_rtb.Document.Blocks.Clear();
            if (!Clipboard.ContainsText()) return;
            System.Windows.Documents.Paragraph prg = new System.Windows.Documents.Paragraph();
            prg.Inlines.Add(Clipboard.GetText());
            a_rtb.Document.Blocks.Add(prg);
        }

        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            string en = GetString(rtbEn);
            string ru = GetString(rtbRu);
            if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(ru)) return;
            // Parse
            var ens = ParseSentences(en);
            var rus = ParseSentences(ru);
            int nRows = ens.Count > rus.Count ? ens.Count : rus.Count;

            CreateWordDocument();
            if (m_app == null) return;
            object start = 0;
            object end = 0;
            Range curRange = m_doc.Range(ref start, ref end);
            Microsoft.Office.Interop.Word.Table table = m_doc.Tables.Add(curRange, nRows, 2, ref missingObj, ref missingObj);
            table.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleDouble;
            table.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleDouble;
            for (int i = 0; i < ens.Count; i++)
            {
                var range = table.Cell(i+1, 1).Range;
                range.Text = ens[i];
            }
            for (int i = 0; i < rus.Count; i++)
            {
                var range = table.Cell(i+1, 2).Range;
                range.Text = rus[i];
            }

            m_app.Visible = true;
        }

        private string GetString(RichTextBox a_rtb)
        {
            var textRange = new TextRange(a_rtb.Document.ContentStart, a_rtb.Document.ContentEnd);
            return textRange.Text;
        }
    }

    public class Main_VM
    {

    }
}
