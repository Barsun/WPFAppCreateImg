using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        private void button_Click_SendObject(object sender, RoutedEventArgs e)
        {
            if (Validate()) return;
            SendParametersToServerDynImg();
            UploadImageOnServer();
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
            else if (DrowDate.IsSelected)
            {
                if (string.IsNullOrWhiteSpace(NameDrawDateTextBox.Text) && DrowDate.IsSelected)
                {
                    MessageBox.Show("Specify the description of the file to upload.", "Drow Date", MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    NameDrawDateTextBox.Focus();
                    return true;
                }
                return false;
             }
            return false;
        }

        private void UploadImageOnServer(){

            string url = ConfigurationManager.AppSettings["serviceUrl"];
            try{
                string requestUrl = string.Format("{0}/UploadPhoto/{1}", url, Path.GetFileName(FileNameTextBox.Text));
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

                MessageBox.Show("File sucessfully uploaded.", "Upload", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex){
                MessageBox.Show("Error during file upload: " + ex.Message, "Upload", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SendParametersToServerDynImg(){

            var codeLotteryDataDraw = _filesStructureDataObject.ListOfItemsData.FirstOrDefault(x => x.name==GameComboBox.Text);

            string url = ConfigurationManager.AppSettings["serviceUrl"];
            try{
                // Create the REST request.
                string requestUrl =
                    string.Format("{0}/CreateFiles/{1}/{2}/{3}/{4}/{5}//{6}/{7}/{8}/{9}/{10}/{11}/{12}/{13}/{14}", url,
                        NameTextBox.Text, codeLotteryDataDraw.id, PlusTextBox.Text, AmountCheckBox.IsChecked,
                        FontFamilyComboBox.Text, TextSizeTextBox.Text, FontStyleBox.Text,
                        shadowFontColor.SelectedColor.ToString().Remove(0, 1), WidthTextBox.Text,
                        MarginRightTextBox.Text, fontColorTextBox.SelectedColor.ToString().Remove(0, 1),
                        HeightTextBox.Text, MarginLeftTextBox.Text, JackPot.IsSelected);

                WebRequest request = WebRequest.Create(requestUrl);
                request.Method = "GET";

                // Get response  
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    Console.WriteLine("HTTP/{0} {1} {2}", response.ProtocolVersion, (int) response.StatusCode,
                        response.StatusDescription);
            } catch (Exception ex){
                MessageBox.Show("Error during creating file: " + ex.Message, "Create", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        void wb_LoadCompleted(object sender, NavigationEventArgs e)
            {
                string script = "document.body.style.overflow ='hidden'";
                WebBrowser wb = (WebBrowser)sender;
                wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
            }

        private void button_Click_Preview(object sender, RoutedEventArgs e)
        {
            if (Validate()) return;
            webBrowser.Navigate("http://dynamic.bflimg.com/Dimg/eurodd12.aspx");
        }
    }
}
