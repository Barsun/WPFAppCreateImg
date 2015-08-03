using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace WPFAppCreateImg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DataObject _filesStructureDataObject = new DataObject();

        public MainWindow(){
            InitializeComponent();
            DataContext = _filesStructureDataObject;
        }

       
        private void button_Click_BrowseImages(object sender, RoutedEventArgs e){
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Images (.jpg)|*.jpg";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9-]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void button_Click_SendObject(object sender, RoutedEventArgs e)
        {

            if (Validate()) return;
            GridLoadingSpinner.Visibility = Visibility.Visible;
            SendParametersToServer();
            UploadImageOnServer();

            string url;
            if (JackPot.IsSelected){
                url = NameTextBox.Text;
            }else{
                url = NameDrawDateTextBox.Text;
            }
            webBrowser.Navigate("http://xxxxxxx.xxxx.xxx/xxx/" + url + ".aspx");
            AddressTextBox.Text = "http://xxxxxxx.xxxx.xxx/xxx/" + url + ".aspx";

            GridLoadingSpinner.Visibility = Visibility.Collapsed;
        }

        private bool Validate()
        {
            if (!File.Exists(FileNameTextBox.Text))
            {
                string message = string.Format("Unable to find '{0}'. Please check the file name and try again.",
                    FileNameTextBox.Text);
                MessageBox.Show(message, "Upload", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                FileNameTextBox.Focus();
                return true;
            }

            if (JackPot.IsSelected)
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Specifiy a name for file.", "Create", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    NameTextBox.Focus();
                    return true;
                }
                return false;
              }
            if (DrowDate.IsSelected)
            {
                if (string.IsNullOrWhiteSpace(NameDrawDateTextBox.Text))
                {
                    MessageBox.Show("Specify the description of the file to upload.", "Drow Date", MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    NameDrawDateTextBox.Focus();
                    return true;
                }
                return false;
             }

            if (string.IsNullOrWhiteSpace(MarginRightTextBox.Text))
            {
                MessageBox.Show("Specify the Margin Top of the file to upload.", "Margin", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                MarginRightTextBox.Focus();
                return true;
            }

            if (string.IsNullOrWhiteSpace(MarginLeftTextBox.Text))
            {
                MessageBox.Show("Specify the Margin Left of the file to upload.", "Margin", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                MarginLeftTextBox.Focus();
                return true;
            }
            return false;
        }

        private void SendParametersToServer()
        {
            string url = ConfigurationManager.AppSettings["serviceUrl"];
            try
            {
                string requestUrl = string.Format("{0}/SendParametersToServer", url);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);

                request.Method = "POST";
                request.ContentType = "text/plain";


                var codeLotteryGame = _filesStructureDataObject.ListOfItemsData.FirstOrDefault(x => x.name == GameComboBox.Text);
                var codeLotteryDateDrow = _filesStructureDataObject.ListOfItemsData.FirstOrDefault(x => x.name == GameDateComboBox.Text); 
                var details = new Dictionary<string, string>();

                details["name"] = !string.IsNullOrWhiteSpace(NameTextBox.Text)? NameTextBox.Text : NameDrawDateTextBox.Text;
                details["code"] = JackPot.IsSelected ? codeLotteryGame.id : codeLotteryDateDrow.id;

                if (JackPot.IsSelected){
                    details["name"] = NameTextBox.Text;
                    details["code"] = codeLotteryGame.id;
                } else{
                    details["name"] = NameDrawDateTextBox.Text;
                    details["code"] = codeLotteryDateDrow.id;
                }
                details["amount"] = AmountCheckBox.IsChecked.ToString();
                details["fontFamily"] = FontFamilyComboBox.Text;
                details["textSize"] = TextSizeTextBox.Text;
                details["fontStyle"] = FontStyleBox.Text;
                details["sFontColor"] = shadowFontColor.SelectedColor.ToString();
                details["width"] = WidthTextBox.Text;
                details["marginRight"] = MarginRightTextBox.Text;
                details["fontColor"] = fontColorTextBox.SelectedColor.ToString();
                details["height"] = HeightTextBox.Text;
                details["marginLeft"] = MarginLeftTextBox.Text;
                details["plusTextAfter"] = PlusTextBoxAfter.Text;
                details["plusText"] = PlusTextBox.Text;
                details["addTextBefore"] = AddTextBefore.Text;
                details["addTextAfter"] = AddTextAfter.Text;
                details["languageComboBox"] = LanguageComboBox.Text;
                details["jpSelected"] = JackPot.IsSelected.ToString();

                string myJsonString = (new JavaScriptSerializer()).Serialize(details);

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] fileToSend = encoding.GetBytes(myJsonString);
                request.ContentLength = fileToSend.Length;


                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileToSend, 0, fileToSend.Length);
                    requestStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    Console.WriteLine("HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during file upload: " + ex.Message, "Upload", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        
        private void UploadImageOnServer(){

            string url = ConfigurationManager.AppSettings["serviceUrl"];
            try{
                string imgName = JackPot.IsSelected ? NameTextBox.Text : NameDrawDateTextBox.Text;
                string requestUrl = string.Format("{0}/UploadPhoto/{1}", url, imgName);
                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(requestUrl);

                request.Method = "POST";
                request.ContentType = "text/plain";


                byte[] fileToSend = File.ReadAllBytes(FileNameTextBox.Text);
                request.ContentLength = fileToSend.Length;

                using (Stream requestStream = request.GetRequestStream()){
                    requestStream.Write(fileToSend, 0, fileToSend.Length);
                    requestStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    Console.WriteLine("HTTP/{0} {1} {2}", response.ProtocolVersion, (int) response.StatusCode, response.StatusDescription);

                MessageBox.Show("File sucessfully Created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex){
                MessageBox.Show("Error during file upload: " + ex.Message, "Upload", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void wb_LoadCompleted(object sender, NavigationEventArgs e)
            {
                string script = "document.body.style.overflow ='hidden'";
                WebBrowser wb = (WebBrowser)sender;
                wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
            }
    }
}
