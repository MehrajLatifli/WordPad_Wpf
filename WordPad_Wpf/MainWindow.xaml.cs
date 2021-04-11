using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace WordPad_Wpf
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

        string m = "";


        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (!Directory.Exists("Text Folder"))
            {
                Directory.CreateDirectory("Text Folder");
            }



            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (var file in files)
            {
                m = file;

                PathTextbox.Visibility = Visibility.Visible;

                PathTextbox.Text = m;
            }

            try
            {

                string dircopyfrom = m;

                string[] fileEnter1 = Directory.GetFiles(dircopyfrom);

                string dircopyto = $@"Text Folder";

                foreach (var f in fileEnter1)
                {
                    string filename = System.IO.Path.GetFileName(f);
                    File.Copy(f, dircopyto + "\\" + filename, true);
                    File.Delete(f);
                }

            }

            catch (Exception)
            {


            }

            File.Copy(m, $@"Text Folder\text.txt", true);

            using (FileStream fs = new FileStream($@"Text Folder\text.txt", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))

            {


                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                string text = Encoding.Default.GetString(bytes);


                richTextBox1.Selection.Text = text;
                // richTextBox1.Selection.Load(fs,DataFormats.Text);
            }


        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effects = DragDropEffects.All;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            richTextBox1.SelectAll();
            //   MessageBox.Show($"{richTextBox1.Selection.Text}");




            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (save.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(save.FileName))
                {
                    writer.Write(richTextBox1.Selection.Text);

                }

            }
        }

        bool on = true;
        bool togglelight = true;

        DispatcherTimer timer = new DispatcherTimer();



        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            AutoSaveButton.Content = "Auto Save\n Off";
            timer.Interval = new TimeSpan(0, 0, 0, 1);
        
        }

        private void t_tick(object sender, EventArgs e)
        {

            if (togglelight)
            {
                if (AutoSaveButton.IsChecked == true)
                {

                    if (File.Exists($@"Text Folder\text.txt"))
                    {
                        File.Delete($@"Text Folder\text.txt");
                    }


                    AutoSaveButton.Background = Brushes.GreenYellow;
                    AutoSaveButton.Content = "Auto Save\n On";



                    using (FileStream fs = new FileStream($@"Text Folder\text.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))

                    {
                        richTextBox1.SelectAll();

                        byte[] bytes = Encoding.Default.GetBytes(richTextBox1.Selection.Text);
                        fs.Write(bytes, 0, bytes.Length);

                    }

                }

                else
                {
                    AutoSaveButton.Content = "Auto Save\n Off";
                    AutoSaveButton.Background = Brushes.Black;


                    AutoSaveButton.Background = Brushes.Red;
                }

                togglelight = false;

            }

            else
            {

                AutoSaveButton.Background = new SolidColorBrush(Color.FromArgb(112, 250, 15, 130));
       

                togglelight = true;
            }
            
        }



        private void AutoSaveButton_Checked(object sender, RoutedEventArgs e)
        {


            if (AutoSaveButton.IsChecked == true)
            {
                if (on)
                {



                    timer.Tick += new EventHandler(t_tick);

                }

                timer.Start();

                on = false;
            }

            else
            {


                timer.Stop();
                on = true;
            }
        }

       




        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {

            Clipboard.SetText(richTextBox1.Selection.Text);
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {

            richTextBox1.Selection.Text = Clipboard.GetData(DataFormats.Text).ToString();
        }
        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (richTextBox1.Selection.Text != "")

                richTextBox1.Cut();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {

            richTextBox1.SelectAll();
        }
    }
}
