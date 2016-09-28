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
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Windows.Input;

namespace DLNA_TestResultReader.FileFormats.UCTT
{
    [Serializable]
    [XmlRoot("UPnPCTT2.0")]
    public class TestResultFile
    {
        [XmlIgnore]
        public string FilePath { get; set; }
        [XmlElement("TestRun")]
        public ObservableCollection<TestRun> TestRuns { get; set; }


        internal static TestResultFile Deserialize(string arg)
        {
            using (var stream = System.IO.File.OpenRead(arg))
            {
                try
                {
                    XmlSerializer s = new XmlSerializer(typeof(TestResultFile));
                    var trf = s.Deserialize(stream) as TestResultFile;
                    trf.FilePath = arg;
                    return trf;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        [XmlIgnore]
        public ICommand JiraCommitCommand { get; set; }
        [XmlIgnore]
        public JIRA.JiraContext JiraContext = null;
    }
}
