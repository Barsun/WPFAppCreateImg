using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace fileCreateApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class FileCreate : IFileCreateApi
    {
       
        public string CreateFiles(Dictionary<string, string> dictionary)
        {
            string text = "A class is the most powerful data type in C#. Like a structure, " +
                           "a class defines the data and behavior of the data type. ";

            File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText.txt", text);

            return "true";
        }

        public string CreateTestFiles(Dictionary<string, string> dictionary)
        {



            return "true";
        }


        public string ImageUpload(Stream fileStream)
        {
            FileStream fileToupload = new FileStream(@"C:\Users\Public\TestFolder\" + "dsasdf", FileMode.Create);

            byte[] bytearray = new byte[10000];
            int bytesRead, totalBytesRead = 0;
            do
            {
                bytesRead = fileStream.Read(bytearray, 0, bytearray.Length);
                totalBytesRead += bytesRead;
            } while (bytesRead > 0);

            fileToupload.Write(bytearray, 0, bytearray.Length);
            fileToupload.Close();
            fileToupload.Dispose();

            return "true";
        }

        public void UploadPhoto(Stream fileContents)
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

            string text = "A class is the most powerful data type in C#. Like a structure, " +
                           "a class defines the data and behavior of the data type. ";

            File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText1.txt", ms.ToString());

            ms.Close();
            Console.WriteLine("Uploaded file {0} with {1} bytes", "ssdfsdf", totalBytesRead);
        }

    }
}
