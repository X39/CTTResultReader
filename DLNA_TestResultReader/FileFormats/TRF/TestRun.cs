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
using DLNA_TestResultReader.ResultFileUtil;

namespace DLNA_TestResultReader.FileFormats.TRF
{
    [Serializable]
    public class TestRun : ITestRun
    {
        [XmlElement("DeviceProfile")]
        public DeviceProfile DeviceProfileValue { get; set; }
        [XmlAttribute("stamp")]
        public DateTime Timestamp { get; set; }
        [XmlElement("testCase")]
        public ObservableCollection<TestCase> TestCaseCollection { get; set; }
        [XmlAttribute("cttBuild")]
        public string cttBuild { get; set; }
        [XmlAttribute("mcvtBuild")]
        public string mcvtBuild { get; set; }




        [XmlIgnore]
        public int TotalPassed
        {
            get
            {
                var count = 0;
                foreach (var tc in this.TestCaseCollection)
                {
                    if (tc.ResultObject.Test.Equals("Pass"))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        [XmlIgnore]
        public int TotalFailed
        {
            get
            {
                var count = 0;
                foreach (var tc in this.TestCaseCollection)
                {
                    if (tc.ResultObject.Test.Equals("Fail"))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        [XmlIgnore]
        public int TotalWarning
        {
            get
            {
                var count = 0;
                foreach (var tc in this.TestCaseCollection)
                {
                    if (tc.ResultObject.Test.Equals("Warning"))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        [XmlIgnore]
        public int TotalNotApplicable
        {
            get
            {
                var count = 0;
                foreach (var tc in this.TestCaseCollection)
                {
                    if (tc.ResultObject.Test.Equals("NA"))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        [XmlIgnore]
        public ObservableCollection<ITestCase> TestCases
        {
            get
            {
                return new ObservableCollection<ITestCase>(this.TestCaseCollection);
            }
        }

        public EDeviceClass DeviceClass
        {
            get
            {
                switch(this.DeviceProfileValue.DeviceClass.ToLower())
                {
                    case "dmc":
                        return EDeviceClass.DMC;
                    case "dmr":
                        return EDeviceClass.DMR;
                    case "dmp":
                        return EDeviceClass.DMP;
                    case "dms":
                        return EDeviceClass.DMS;
                    default:
                        throw new Exception();
                }
            }
        }

        public string TestTool { get { return string.IsNullOrWhiteSpace(this.cttBuild) ? "MCVT" : "CTT"; } }
    }
}
