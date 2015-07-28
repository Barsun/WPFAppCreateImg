using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace fileCreateApi
{
    [ServiceContract]
    public interface IFileCreateApi
    {

        [OperationContract]
        string CreateFiles(Dictionary<string, string> dictionary);

        [OperationContract]
        string ImageUpload(Stream fileStream);

        [OperationContract]
        void UploadPhoto(Stream fileContents);
    }

}
