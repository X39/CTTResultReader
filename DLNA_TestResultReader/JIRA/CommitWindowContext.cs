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
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace DLNA_TestResultReader.JIRA
{
    public class CommitWindowContext : INotifyPropertyChanged
    {
        public ObservableCollection<DLNA_TestResultReader.FileFormats.TRF.TestCase> Issues { get; set; }
        public ObservableCollection<DLNA_TestResultReader.FileFormats.TRF.TestCase> IssuesSelected { get; set; }
        private string _IssueTemplate;
        public string IssueTemplate
        {
            get
            {
                return this._IssueTemplate;
            }
            set
            {
                this._IssueTemplate = value;
                if(PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CloseOK { get; set; }
        public CommitWindowContext()
        {
            CloseOK = new RelayCommand( (o) =>
            {
                (o as Window).DialogResult = true;
                (o as Window).Close();
            });
        }
    }
}
