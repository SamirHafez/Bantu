using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;

namespace Bantu.Helpers
{
    public class DataSaver<MyDataType>
    {
        private const string TargetFolderName = "Settings";
        private DataContractSerializer _mySerializer;
        private IsolatedStorageFile _isoFile;
        IsolatedStorageFile IsoFile
        {
            get
            {
                if (_isoFile == null)
                    _isoFile = System.IO.IsolatedStorage.
                                IsolatedStorageFile.GetUserStoreForApplication();
                return _isoFile;
            }
        }

        public DataSaver()
        {
            _mySerializer = new DataContractSerializer(typeof(MyDataType));
        }

        public void SaveMyData(MyDataType sourceData, String targetFileName)
        {
            string TargetFileName = String.Format("{0}/{1}.dat",
                                           TargetFolderName, targetFileName);
            if (!IsoFile.DirectoryExists(TargetFolderName))
                IsoFile.CreateDirectory(TargetFolderName);
            try
            {
                using (var targetFile = IsoFile.CreateFile(TargetFileName))
                {
                    _mySerializer.WriteObject(targetFile, sourceData);
                }
            }
            catch (Exception e)
            {
                IsoFile.DeleteFile(TargetFileName);
            }


        }

        public MyDataType LoadMyData(string sourceName)
        {
            MyDataType retVal = default(MyDataType);
            string TargetFileName = String.Format("{0}/{1}.dat",
                                                  TargetFolderName, sourceName);
            if (IsoFile.FileExists(TargetFileName))
                using (var sourceStream =
                        IsoFile.OpenFile(TargetFileName, FileMode.Open))
                {
                    retVal = (MyDataType)_mySerializer.ReadObject(sourceStream);
                }
            return retVal;
        }
    }
}
