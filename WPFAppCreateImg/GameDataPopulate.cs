using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Serialization;

namespace WPFAppCreateImg
{
    public class GameDataPopulate
    {
        /// <remarks />
        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", IsNullable = false)]
        public class next_lottery_data
        {
            public next_lottery_dataDraw[] DrawField;

            /// <remarks />
            [XmlElement("draw")]
            public next_lottery_dataDraw[] draw{
                get { return DrawField; }
                set { DrawField = value; }
            }
        }

        /// <remarks />
        [XmlType(AnonymousType = true)]
        public class next_lottery_dataDraw
        {
            /// <remarks />
            public string name { get; set; }

            /// <remarks />
            [XmlAttribute]
            public string id { get; set; }
        }
    }

    public class DataObject
    {
        public DataObject(){
            ListOfItems = new List<string>();


            try{
                var url = ConfigurationManager.AppSettings["serviceUrl"];
                var requestUrl = string.Format("{0}/GameValuesToPopulate", url);

                var request = WebRequest.Create(requestUrl);
                // Get response  
                using (var resp = request.GetResponse() as HttpWebResponse){
                    var reader = new StreamReader(resp.GetResponseStream());
                    var responce = reader.ReadToEnd();
                    var xml = responce.Substring(1).Substring(0, responce.Length - 2).Replace("\\", "") + "";
                    ListOfItems = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x.name);
                    ListOfItemsData = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x);
                }
            } catch (Exception ex){
                MessageBox.Show(
                    "Error access file on server to populate data, contact the administrator:  " + ex.Message, "Server",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        public IEnumerable<string> ListOfItems { get; set; }
        public IEnumerable<GameDataPopulate.next_lottery_dataDraw> ListOfItemsData { get; set; }
    }
}