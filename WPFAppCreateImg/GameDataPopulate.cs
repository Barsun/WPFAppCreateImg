using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
            string xml = File.ReadAllText(@"GameDataPopulate.xml");
            ListOfItems = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x.name);
            ListOfItemsData = xml.ParseXML<GameDataPopulate.next_lottery_data>().DrawField.Select(x => x);
        }

        public IEnumerable<string> ListOfItems { get; set; }
        public IEnumerable<GameDataPopulate.next_lottery_dataDraw> ListOfItemsData { get; set; }
        
    }
}
