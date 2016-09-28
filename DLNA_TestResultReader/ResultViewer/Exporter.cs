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
using Microsoft.Win32;
using System.IO;
using DLNA_TestResultReader.ResultFileUtil;

namespace DLNA_TestResultReader.ResultViewer
{
    public static class Exporter
    {
        public static void ToType(ITestRun testrun, string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Comma-Separated Values (*.csv)|*.csv|All files|*.*";
                var dlgRes = sfd.ShowDialog();
                if (dlgRes.HasValue && dlgRes.Value)
                {
                    path = sfd.FileName;
                }
                else
                {
                    return;
                }
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            switch (Path.GetExtension(path).ToLower())
            {
                case ".csv":
                    ToCsv(testrun, path);
                    break;
                default:
                    throw new ArgumentException(string.Format("Export type {0} is unknown", Path.GetExtension(path)), "type");
            }
        }

        public static void ToCsv(ITestRun testrun, string path)
        {
            using (var sWriter = new StreamWriter(File.Create(path)))
            {
                sWriter.WriteLine("Testcase;Result;Note");
                foreach (var tc in testrun.TestCases)
                {
                    sWriter.Write('"');
                    sWriter.Write(tc.Name.Replace("\"", "\"\""));
                    sWriter.Write('"');
                    sWriter.Write(';');
                    sWriter.Write(Enum.GetName(typeof(EResult), tc.Result));
                    sWriter.Write(';');
                    sWriter.WriteLine(tc.ResultNote);
                }
            }
        }
    }
}
