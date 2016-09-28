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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using DLNA_TestResultReader.ResultFileUtil;

namespace DLNA_TestResultReader.FileFormats.UCTT
{
    [Serializable]
    public class TestCase : ITestCase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("level")]
        public string Level { get; set; }
        [XmlAttribute("scriptVersion")]
        public string ScriptVersion { get; set; }
        [XmlAttribute("stamp")]
        public DateTime Timestamp { get; set; }
        [XmlElement("profileAllowedResults")]
        public List<string> AllowedResults { get; set; }
        [XmlArray("Logs")]
        [XmlArrayItem("Log")]
        public ObservableCollection<LogObject> LogCollection { get; set; }

        [XmlElement("Result")]
        public string ResultString { get; set; }

        [XmlIgnore]
        public EResult Result
        {
            get
            {
                switch(this.ResultString.ToLower()[0])
                {
                    case 'p':
                        return EResult.Passed;
                    case 'f':
                        return EResult.Failed;
                    case 'n':
                        return EResult.NotApplicable;
                    case 'w':
                        return EResult.Warning;
                    default:
                        return EResult.NotRun;
                }
            }
        }

        [XmlIgnore]
        public string Log
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach(var log in this.LogCollection)
                {
                    builder.Append(log.Level);
                    builder.Append(": ");
                    builder.AppendLine(log.Content);
                }
                return builder.ToString();
            }
        }

        [XmlIgnore]
        public string ResultNote { get { return this.LogCollection.Last().Content; } }

        [XmlIgnore]
        public string ID { get { return this.Name.Substring(0, this.Name.IndexOf(' ')); } }

        public ESeverityLevel Severity
        {
            get
            {
                if (this.AllowedResults.Contains("Failed"))
                    return ESeverityLevel.Optional;
                else if (this.AllowedResults.Contains("NA"))
                    return ESeverityLevel.Recommended;
                else if (this.AllowedResults.Contains("WARNING"))
                    return ESeverityLevel.Mandatory;
                else if (this.AllowedResults.Contains("PASSED"))
                    return ESeverityLevel.Mandatory;
                else return ESeverityLevel.NotAvailable;
            }
        }
    }
}
