using System;
using System.IO;
using System.Collections.Generic;

namespace ccsviewer
{
    public class DataFile
    {
        public string FileName;
        public DataFile (string fname)
        {
            if (!File.Exists (fname)) {
                throw new FileNotFoundException ("File: " + fname + " could not be found.");
            }
            FileName = fname;
            
        }
    }
}

