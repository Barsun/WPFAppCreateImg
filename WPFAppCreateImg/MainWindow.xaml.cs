using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Serialization;

namespace WPFAppCreateImg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(){
            InitializeComponent();
            DataContext = new DataObject();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e){
            
        }

        private void button_Click_BrowseImages(object sender, RoutedEventArgs e){
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Images (.jpg, .png)|*.jpg; *.png";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void button_Click_SendObject(object sender, RoutedEventArgs e){

            var dictionary = new Dictionary<string, string>();

            if (JackPot.IsSelected){
                dictionary["nameName"] = NameTextBox.Text;
                dictionary["game"] = GameComboBox.Text;
                dictionary["textplus"] = PlusTextBox.Text;
            }
            if (DrowDate.IsSelected){
                dictionary["nameName"] = NameTextBox.Text;
                dictionary["game"] = GameComboBox.Text;
                dictionary["textplus"] = PlusTextBox.Text;
            }


            
            dictionary["amount"] = AmountCheckBox.IsChecked.ToString();
            dictionary["fontFamily"] = FontFamilyComboBox.Text;
            dictionary["textSize"] = TextSizeTextBox.Text;
            dictionary["FontStyle"] = FontStyleBox.Text;
            dictionary["shadowColor"] = shadowFontColor.SelectedColor.ToString();
            dictionary["WidthText"] = WidthTextBox.Text;
            dictionary["MarginRight"] = MarginRightTextBox.Text;
            dictionary["fontColor"] = fontColorTextBox.SelectedColor.ToString();
            dictionary["FileName"] = FileNameTextBox.Text;
            dictionary["Height"] = HeightTextBox.Text;
            dictionary["MarginLeft"] = MarginLeftTextBox.Text;

            //var result = new CreateImageAPI.FileCreateApiClient();


            //result.CreateFiles(dictionary);


            // Create the REST request.
            string url = ConfigurationManager.AppSettings["serviceUrl"];
            string requestUrl = string.Format("{0}/UploadPhoto/{1}/{2}", url, System.IO.Path.GetFileName(FileNameTextBox.Text), "ssdfvbsdfv");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.Method = "POST";
            request.ContentType = "text/plain";

            byte[] fileToSend = File.ReadAllBytes(FileNameTextBox.Text);
            request.ContentLength = fileToSend.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the file as body request.
                requestStream.Write(fileToSend, 0, fileToSend.Length);
                requestStream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                Console.WriteLine("HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);

            MessageBox.Show("File sucessfully uploaded.", "Upload", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
