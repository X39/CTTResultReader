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
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using DLNA_TestResultReader.ResultFileUtil;

namespace DLNA_TestResultReader.FileFormats.UCTT
{
    [Serializable]
    public class TestRun : ITestRun
    {
        [XmlElement("DUT")]
        public DeviceProfile DeviceProfileValue { get; set; }
        [XmlAttribute("startTime")]
        public DateTime Timestamp { get; set; }
        [XmlElement("TestCase")]
        public ObservableCollection<TestCase> TestCaseCollection { get; set; }




        [XmlIgnore]
        public int TotalPassed { get { return this.TestCaseCollection.Count(obj => obj.Result == EResult.Passed); } }
        [XmlIgnore]
        public int TotalFailed { get { return this.TestCaseCollection.Count(obj => obj.Result == EResult.Failed); } }
        [XmlIgnore]
        public int TotalWarning { get { return this.TestCaseCollection.Count(obj => obj.Result == EResult.Warning); } }
        [XmlIgnore]
        public int TotalNotApplicable { get { return this.TestCaseCollection.Count(obj => obj.Result == EResult.NotApplicable); } }
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
                if (this.DeviceProfileValue.DeviceType.ToLower().Contains("mediaserver"))
                    return EDeviceClass.DMS;
                else if (this.DeviceProfileValue.DeviceType.ToLower().Contains("mediarenderer"))
                    return EDeviceClass.DMS;
                else return EDeviceClass.DMC;
            }
        }

        [XmlIgnore]
        public string TestTool { get { return "UCTT"; } }
    }
}
