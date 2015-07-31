/* Copyright 2012 Marco Minerva, marco.minerva@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace PhotoService
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PhotoManager
    {

        [WebInvoke(UriTemplate = "UploadPhoto/{fileName}", Method = "POST")]
        public void UploadPhoto(string fileName, Stream fileContents)
        {
            byte[] buffer = new byte[32768];
            MemoryStream ms = new MemoryStream();
            int bytesRead, totalBytesRead = 0;
            do
            {
                bytesRead = fileContents.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;

                ms.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            Image returnImage = Image.FromStream(ms);

            returnImage.Save(@"C:\www\dynamic.bflimg.com\images\" + fileName + ".jpg");

            ms.Close();
            Console.WriteLine("Uploaded file {0} with {1} bytes", fileName, totalBytesRead);
        }



        [WebInvoke(UriTemplate = "SendParametersToServer", Method = "POST")]
        public void SendParametersToServer(Stream fileContents)
        {
            byte[] buffer = new byte[32768];
            MemoryStream ms = new MemoryStream();
            int bytesRead, totalBytesRead = 0;
            do
            {
                bytesRead = fileContents.Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;

                ms.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            string text = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace("\0","");
            JavaScriptSerializer j = new JavaScriptSerializer();
            object a = j.Deserialize(text, typeof(object));
            Dictionary<string, string> details = ConvertToDictionary(a);

            string csTemplate = File.ReadAllText(@"C:\www\dynamic.bflimg.com\CreateDynFiles\templates\CSTemplate.txt");
            string aspxTemplate = File.ReadAllText(@"C:\www\dynamic.bflimg.com\CreateDynFiles\templates\ASPXTemplate.txt");

            if (details["jpSelected"] == "True")
            {
                details["jackpot"] = details["amount"] != "True" ? "jackpot" : "amount";
                details["valueAmt"] = "string drawDate = \"" + details["plusText"] + "\" + GetDrawDate(\"" + details["code"] + "\").ToUpper() + \"" + details["plusTextAfter"] + "\"";
            } else {
                details["jackpot"] = "drawdate";

                if (details["languageComboBox"] == "English")
                {
                    details["valueAmt"] = "string drawDate = \"" + details["addTextBefore"] + "\" + GetDrawDate(\"" + details["code"] + "\").ToUpper() + \"" + details["addTextAfter"] + "\"";
                }else{
                    details["valueAmt"] = "DateTime nextDrawPL = Convert.ToDateTime(GetDrawDate(\"" + details["code"] + "\")); " +
                        "string drawDate = \"" + details["addTextBefore"] + "\" + nextDrawPL.ToString(\"D\", new System.Globalization.CultureInfo(\"" + details["languageComboBox"].Substring(0, 2).ToLower() + "-" + details["languageComboBox"].Substring(0, 2).ToUpper() + "\")).ToUpper() + \"" + details["addTextAfter"] + "\"";
                }
            }


            csTemplate = details.Aggregate(csTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));
            aspxTemplate = details.Aggregate(aspxTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));

            File.WriteAllText(@"C:\www\dynamic.bflimg.com\DImg\" + details["name"] + ".aspx.cs", csTemplate);
            File.WriteAllText(@"C:\www\dynamic.bflimg.com\DImg\" + details["name"] + ".aspx", aspxTemplate);

            Console.WriteLine("Create file {0}", details["name"]);

            ms.Close();
            Console.WriteLine("Uploaded file {0} with bytes",  totalBytesRead);
        }
        

        [WebInvoke(UriTemplate = "GameValuesToPopulate", Method = "GET")]
        public string GameValuesToPopulate()
        {
            return File.ReadAllText(@"C:\www\dynamic.bflimg.com\CreateDynFiles\templates\GameDataPopulate.xml");
        }

        public static Dictionary<string,string> ConvertToDictionary(object obj)
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>();

            if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
            {
                IDictionary idict = (IDictionary)obj;

                foreach (object key in idict.Keys)
                {
                   newDict.Add(key.ToString(), idict[key].ToString());
                }
            }
            
            return newDict;
        }
    }
}
