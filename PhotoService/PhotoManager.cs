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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

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

            returnImage.Save(@"C:\Users\Public\TestFolder\" + fileName + ".jpg");

            // Save the photo on database.
            //using (DataAcess data = new DataAcess())
            //{
            //    var photo = new Photo() { Name = fileName, Description = fileName, Data = ms.ToArray(), DateTime = DateTime.UtcNow };
            //    data.InsertPhoto(photo);
            //}

            ms.Close();
            Console.WriteLine("Uploaded file {0} with {1} bytes", fileName, totalBytesRead);
        }


        [WebInvoke(UriTemplate = "CreateFiles/{name}/{code}/{plusText}/{amountCheckBox}/{fontFamily}/{textSize}/{fontStyle}/{sFontColor}/{width}/{marginRight}/{fontColor}/{height}/{marginLeft}/{jpSelected}", Method = "GET")]
        public void CreateFiles(string name, string code, string plusText, string amountCheckBox, string fontFamily, string textSize, string fontStyle, string sFontColor,
            string width, string marginRight, string fontColor, string height, string marginLeft, string jpSelected)
        {
            string csTemplate = File.ReadAllText(@"C:\Users\Public\TestFolder\CSTemplate.txt");
            string aspxTemplate = File.ReadAllText(@"C:\Users\Public\TestFolder\ASPXTemplate.txt");

            var details = new Dictionary<string, string>();
            details["name"] = name;
            details["code"] = code;
            details["plusText"] = plusText;
            details["amount"] = amountCheckBox;
            details["fontFamily"] = fontFamily;
            details["textSize"] = textSize;
            details["fontStyle"] = fontStyle;
            details["sFontColor"] = sFontColor;
            details["width"] = width;
            details["marginRight"] = marginRight;
            details["fontColor"] = fontColor;
            details["height"] = height;
            details["marginLeft"] = marginLeft;

            csTemplate = details.Aggregate(csTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));
            aspxTemplate = details.Aggregate(aspxTemplate, (current, detail) => current.Replace("@" + detail.Key, detail.Value));

            File.WriteAllText(@"C:\Users\Public\TestFolder\" + name + ".cs", csTemplate);
            File.WriteAllText(@"C:\Users\Public\TestFolder\" + name + ".aspx", aspxTemplate);

            Console.WriteLine("Create file {0}", name);

        }
    }
}
