/*
MIT License

Copyright (c) 2016 Marco Silipo (X39)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace DLNA_TestResultReader.FileFormats
{
    public static class Importer
    {
        public static ObservableCollection<ResultFileUtil.ITestRun> ImportFromExtension(string path = "")
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                List<string> allowedFileTypes = new List<string>();
                allowedFileTypes.Add("UCTT Test Result File (*.uctt)|*.uctt");
                allowedFileTypes.Add("DLNA Test Result File (*.trf)|*.trf");

                StringBuilder endFilterBuilder = new StringBuilder();
                endFilterBuilder.Append("Allowed File Types (");
                bool isFirst = true;
                foreach (var type in allowedFileTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        endFilterBuilder.Append(',');
                    endFilterBuilder.Append(type.Substring(type.IndexOf('|') + 1));
                }
                endFilterBuilder.Append(')');
                endFilterBuilder.Append('|');
                isFirst = true;
                foreach (var type in allowedFileTypes)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        endFilterBuilder.Append(';');
                    endFilterBuilder.Append(type.Substring(type.IndexOf('|') + 1));
                }
                foreach (var type in allowedFileTypes)
                {
                    endFilterBuilder.Append('|');
                    endFilterBuilder.Append(type);
                }
                endFilterBuilder.Append("|All Files|*.*");
                ofd.Filter = endFilterBuilder.ToString();
                var dlgRes = ofd.ShowDialog();
                if (dlgRes.HasValue && dlgRes.Value)
                {
                    path = ofd.FileName;
                }
                else
                {
                    return new ObservableCollection<ResultFileUtil.ITestRun>();
                }
            }

            string ext = System.IO.Path.GetExtension(path);
            switch(ext.ToLower())
            {
                case ".trf":
                    return new ObservableCollection<ResultFileUtil.ITestRun>(TRF.TestResultFile.Deserialize(path).TestRuns);
                case ".uctt":
                    return new ObservableCollection<ResultFileUtil.ITestRun>(UCTT.TestResultFile.Deserialize(path).TestRuns);
                default:
                    throw new ArgumentException(string.Format("Unknown file extension {0}", ext), "path");
            }
        }
    }
}
