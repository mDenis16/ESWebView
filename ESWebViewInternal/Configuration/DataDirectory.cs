using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESWebViewInternal.Configuration
{
    public class DataDirectory
    {
        public string Path { get; set; }


        public DataDirectory(string _path)
        {
            Path = _path;
        }


        public Tuple<bool, string> CheckIfDirectoryExists()
        {
            try
            {

                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                else
                    return new Tuple<bool, string>(true, "Directory already exists.");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            return new Tuple<bool, string>(true, "Directory succesfully created.");
        }
    }
}