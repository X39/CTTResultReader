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
using System.Threading.Tasks;
using System.Windows.Input;
using DLNA_TestResultReader.ResultFileUtil;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DLNA_TestResultReader.ResultViewer
{
    public class DialogContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string caller = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public string WindowTitle { get { return this.FilePath; } }
        public string FilePath { get { return this._FilePath; } set { this._FilePath = value; this.RaisePropertyChanged(); this.RaisePropertyChanged("WindowTitle"); } }
        private string _FilePath { get; set; }

        public ICommand cmd_jira_commit { get { return this._cmd_jira_commit; } set { this._cmd_jira_commit = value; this.RaisePropertyChanged(); } }
        private ICommand _cmd_jira_commit { get; set; }

        public ICommand cmd_export { get { return this._cmd_export; } set { this._cmd_export = value; this.RaisePropertyChanged(); } }
        private ICommand _cmd_export { get; set; }

        public ObservableCollection<ITestRun> AllTestRuns { get { return this._AllTestRuns; } set { this._AllTestRuns = value; this.RaisePropertyChanged(); } }
        private ObservableCollection<ITestRun> _AllTestRuns { get; set; }

        public ITestRun SelectedTestRun { get { return this._SelectedTestRun; } set { this._SelectedTestRun = value; if (value != null) { this.AllTestCases = value.TestCases; } else { this.AllTestCases = null; } this.RaisePropertyChanged(); } }
        private ITestRun _SelectedTestRun { get; set; }

        public ObservableCollection<ITestCase> AllTestCases { get { return this._AllTestCases; } set { this._AllTestCases = value; this.RaisePropertyChanged(); } }
        private ObservableCollection<ITestCase> _AllTestCases { get; set; }

        public ITestCase SelectedTestCase { get { return this._SelectedTestCase; } set { this._SelectedTestCase = value; this.RaisePropertyChanged(); } }
        private ITestCase _SelectedTestCase { get; set; }


        public DialogContext(string FilePath)
        {
            this.cmd_jira_commit = new RelayCommandAsync(cmd_jira_commit_implementation);
            this.cmd_export = new RelayCommandAsync(cmd_export_implementation);
            this.FilePath = string.IsNullOrWhiteSpace(FilePath) ? "CTT Result Reader" : FilePath;
            this.AllTestRuns = new ObservableCollection<ITestRun>(FileFormats.Importer.ImportFromExtension(FilePath));
        }
        private Task cmd_jira_commit_implementation(object param)
        {
            JIRA.JiraContext jc = new JIRA.JiraContext(SelectedTestRun);
            JIRA.Login.Dialog loginDlg = new JIRA.Login.Dialog();
            loginDlg.DataContext = jc;
            var loginDlgResult = loginDlg.ShowDialog();
            if(!loginDlgResult.HasValue || !loginDlgResult.Value)
            {
                return Task.CompletedTask;
            }

            JIRA.CommitPrepare.Dialog commitDlg = new JIRA.CommitPrepare.Dialog();
            commitDlg.DataContext = jc;
            commitDlg.ShowDialog();
            return Task.CompletedTask;
        }
        private Task cmd_export_implementation(object param)
        {
            Exporter.ToType(SelectedTestRun);
            return Task.CompletedTask;
        }
    }
}
