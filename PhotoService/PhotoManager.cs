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
            try
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
                returnImage.Save(@"C:\xxx\xxxxxx.xxx\xxxx\" + fileName + ".jpg");
                ms.Close();

                try{
                    using (NetworkShareAccesser.Access("xxxxxxxxx", "xxxxxxxxx", "xxxxx", "xxxxxxxxx"))
                    {
                        File.Copy(@"C:\xxx\xxxxxx.xxx\xxxx\" + fileName + ".jpg", @"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\" + fileName + ".jpg");
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt", true))
                    {
                        logFile.WriteLine("Image wasn't copied " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                        logFile.WriteLine("Image name: {0} , error: {1} ", fileName, ex.Message);
                        logFile.WriteLine("------------------------------------------------");
                    }
                }
                

                using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt", true)){
                    logFile.WriteLine("Starting proccess Image Upload at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    logFile.WriteLine("Create Image  {0} with {1} bytes", fileName, totalBytesRead);
                    logFile.WriteLine("------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt", true)){
                    logFile.WriteLine("Starting proccess Image Upload at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    logFile.WriteLine("Error during Image upload: " + ex.Message);
                    logFile.WriteLine("------------------------------------------------");
                }
            }
        }

        [WebInvoke(UriTemplate = "SendParametersToServer", Method = "POST")]
        public void SendParametersToServer(Stream fileContents)
        {

            try
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

                string text = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace("\0", "");
                JavaScriptSerializer j = new JavaScriptSerializer();
                object a = j.Deserialize(text, typeof(object));
                Dictionary<string, string> details = ConvertToDictionary(a);

                string csTemplate = File.ReadAllText(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\templates\CSTemplate.txt");
                string aspxTemplate = File.ReadAllText(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\templates\ASPXTemplate.txt");

                if (details["jpSelected"] == "True")
                {
                    details["jackpot"] = details["amount"] != "True" ? "jackpot" : "amount";
                    details["valueAmt"] = "string drawDate = \"" + details["plusText"] + "\" + GetDrawDate(\"" + details["code"] + "\").ToUpper() + \"" + details["plusTextAfter"] + "\"";
                }
                else
                {
                    details["jackpot"] = "drawdate";

                    if (details["languageComboBox"] == "English")
                    {
                        details["valueAmt"] = "string drawDate = \"" + details["addTextBefore"] + "\" + GetDrawDate(\"" + details["code"] + "\").ToUpper() + \"" + details["addTextAfter"] + "\"";
                    }
                    else
                    {
                        details["languageComboBox"] = details["languageComboBox"] == "Polish" ? details["languageComboBox"] = "Pl" : details["languageComboBox"];
                        details["valueAmt"] = "DateTime nextDrawPL = Convert.ToDateTime(GetDrawDate(\"" + details["code"] + "\")); " +
                            "string drawDate = \"" + details["addTextBefore"] + "\" + nextDrawPL.ToString(\"D\", new System.Globalization.CultureInfo(\"" + details["languageComboBox"].Substring(0, 2).ToLower() + "-" + details["languageComboBox"].Substring(0, 2).ToUpper() + "\")).ToUpper() + \"" + details["addTextAfter"] + "\"";
                    }
                }


                csTemplate = details.Aggregate(csTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));
                aspxTemplate = details.Aggregate(aspxTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));

                File.WriteAllText(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx.cs", csTemplate, Encoding.UTF8);
                File.WriteAllText(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx", aspxTemplate, Encoding.UTF8);

                try
                {
                    using (NetworkShareAccesser.Access("xxxxxxxxx", "xxxxxxxxx", "xxxxx", "xxxxxxxxx"))
                    {
                        File.Copy(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx.cs", @"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx.cs");
                        File.Copy(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx", @"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx");
                        File.Copy(@"C:\xxx\xxxxxx.xxx\xxxx\" + details["name"] + ".jpg", @"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\" + details["name"] + ".jpg");
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt",true)){
                        logFile.WriteLine("Files on remote server were edited " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                        logFile.WriteLine("File: " + ex.Message);
                        logFile.WriteLine("------------------------------------------------");
                    }
                   
                    File.WriteAllText(@"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx.cs", csTemplate, Encoding.UTF8);
                    File.WriteAllText(@"\\xxxxxxxxx\C$\xxx\xxxxxx.xxx\xxxx\xxxxx\" + details["name"] + ".aspx", aspxTemplate, Encoding.UTF8);
                }

                ms.Close();
                using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt", true)){
                    logFile.WriteLine("Starting proccess at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    logFile.WriteLine("Create file {0}", details["name"]);
                    logFile.WriteLine("------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter logFile = new StreamWriter(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\Log.txt", true))
                {
                    logFile.WriteLine("Starting proccess at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    logFile.WriteLine("Error during file upload: " + ex.Message);
                    logFile.WriteLine("------------------------------------------------");
                }
            }
        }



        [WebInvoke(UriTemplate = "GameValuesToPopulate", Method = "GET")]
        public string GameValuesToPopulate()
        {
            return File.ReadAllText(@"C:\xxx\xxxxxx.xxx\xxxx\xxxxx\templates\GameDataPopulate.xml");
        }

        public static Dictionary<string, string> ConvertToDictionary(object obj)
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>();

            if (obj is IDictionary)
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
