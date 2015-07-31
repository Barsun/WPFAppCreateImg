using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace WPFAppCreateImg
{
    public class GameDataPopulate
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class next_lottery_data
        {

            public next_lottery_dataDraw[] DrawField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("draw")]
            public next_lottery_dataDraw[] draw
            {
                get
                {
                    return this.DrawField;
                }
                set
                {
                    this.DrawField = value;
                }
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class next_lottery_dataDraw
        {

            private string nameField;

            private string idField;

            /// <remarks/>
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }



    }

    public class DataObject
    {
        public DataObject()
        {
            ListOfItems = new List<string>();


            try
            {

                string url = ConfigurationManager.AppSettings["serviceUrl"];
                string requestUrl = string.Format("{0}/GameValuesToPopulate", url);

                WebRequest request = WebRequest.Create(requestUrl);
                // Get response  
                using (HttpWebResponse resp = request.GetResponse()
                              as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    string responce = reader.ReadToEnd();
                    string xml = responce.Substring(1).Substring(0, responce.Length - 2).Replace("\\", "") + "";
                    ListOfItems = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x.name);
                    ListOfItemsData = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error access file on server to populate data, contact the administrator:  " + ex.Message, "Server", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            
            
            
        }

        public IEnumerable<string> ListOfItems { get; set; }
        public IEnumerable<GameDataPopulate.next_lottery_dataDraw> ListOfItemsData { get; set; }
        
    }
}
