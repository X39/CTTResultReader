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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Atlassian.Jira;
using DLNA_TestResultReader.ResultFileUtil;
using System.ComponentModel;


namespace DLNA_TestResultReader.JIRA
{
    public class JiraContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public ICommand cmd_jira_login { get { return this._cmd_jira_login; } set { this._cmd_jira_login = value; this.RaisePropertyChanged(); } }
        private ICommand _cmd_jira_login { get; set; }
        public ICommand cmd_jira_commit { get { return this._cmd_jira_commit; } set { this._cmd_jira_commit = value; this.RaisePropertyChanged(); } }
        private ICommand _cmd_jira_commit { get; set; }

        public string Username { get { return this._Username; } set { this._Username = value; this.RaisePropertyChanged(); } }
        private string _Username { get; set; }

        public string ReportTemplate { get { return this._ReportTemplate; } set { this._ReportTemplate = value; this.RaisePropertyChanged(); } }
        private string _ReportTemplate { get; set; }

        public Jira jira { get; set; }

        public List<ITestCase> TestCases
        {
            get
            {
                var list = new List<ITestCase>();
                foreach (var tc in TestRun.TestCases)
                {
                    if (tc.Result != EResult.Passed && tc.Result != EResult.NotRun)
                        list.Add(tc);
                }
                return list;
            }
        }
        public ITestRun TestRun { get; set; }

        public JiraContext(ITestRun run)
        {
            this.TestRun = run;
            this.cmd_jira_login = new RelayCommandAsync(cmd_jira_login_implementation);
            this.cmd_jira_commit = new RelayCommandAsync(cmd_jira_commit_implementation);
            this.ReportTemplate = App.Current.FindResource("JiraDefaultDescriptionTemplate") as string;
        }

        private async Task cmd_jira_login_implementation(object param)
        {
            if (!(param is JIRA.Login.Dialog))
                return;
            var w = param as Login.Dialog;
            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(w.pbPassword.Password))
            {
                MessageBox.Show("Please fill out all fields.", "Invalid Credentials", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                this.jira = Jira.CreateRestClient(App.Current.FindResource("ServerAdress") as string, this.Username, w.pbPassword.Password);
                JiraUser user = await this.jira.Users.GetUserAsync(this.Username);
                if (user == null)
                {
                    MessageBox.Show("Given credentials are not valid.", "Invalid Credentials", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                w.DialogResult = true;
                w.Close();
            }
            catch
            {
                MessageBox.Show("Given credentials are not valid.", "Invalid Credentials", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async Task cmd_jira_commit_implementation(object param)
        {
            if (!(param is JIRA.CommitPrepare.Dialog))
                return;
            var w = param as CommitPrepare.Dialog;


            StringBuilder builder = new StringBuilder();
            builder.Append("project = ");
            builder.Append(App.Current.FindResource("JiraProjectId") as string);
            builder.Append(" AND status NOT IN");
            builder.Append(App.Current.FindResource("JiraIgnoreStatuses") as string);
            builder.Append(" AND (");
            bool isFirst = true;
            List<ITestCase> testCaseList = new List<ITestCase>();
            foreach (var it in w.lb.SelectedItems)
            {
                if (!(it is ITestCase))
                    continue;
                var tc = it as ITestCase;
                if (isFirst)
                {
                    isFirst = false;
                    builder.Append("summary ~ ");
                }
                else
                {
                    builder.Append(" || summary ~ ");
                }
                builder.Append('"');
                builder.Append(tc.ID);
                builder.Append('"');
                testCaseList.Add(tc);
            }
            builder.Append(')');
            if (isFirst == true)
            {
                MessageBox.Show("There are no issues matching your selection", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string jql = builder.ToString();
            var issues = await this.jira.Issues.GetIsssuesFromJqlAsync(jql);
            List<ITestCase> toReport = new List<ITestCase>();
            foreach (var tc in testCaseList)
            {
                bool contained = false;
                foreach (var issue in issues)
                {
                    if (issue.Summary.Contains(tc.ID) && issue.Summary.Contains(Enum.GetName(typeof(EDeviceClass), this.TestRun.DeviceClass)))
                    {
#if DEBUG
                        if(toReport.Count == 0 && testCaseList[testCaseList.Count - 1] == tc)
                            toReport.Add(tc);
#endif
                        contained = true;
                        break;
                    }
                }
                if (!contained)
                {
                    toReport.Add(tc);
                }
            }
            if (toReport.Count == 0)
            {
                MessageBox.Show("All issues matching your selection are already reported.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            List<string> IssuesCreated = new List<string>();
            foreach (var tc in toReport)
            {
                var newIssue = new Atlassian.Jira.Issue(this.jira, "TSD");
                var issueTypes = await jira.IssueTypes.GetIssueTypesAsync();
                foreach(var type in issueTypes)
                {
                    if(type.Name == "Bug")
                    {

                        newIssue.Type = type;
                        break;
                    }
                }
                string log = tc.Log;
                newIssue.Summary = string.Format(App.Current.FindResource("JiraSummaryTemplate") as string, Enum.GetName(typeof(EDeviceClass), this.TestRun.DeviceClass), TestRun.TestTool, Enum.GetName(typeof(EResult), tc.Result), tc.ID);
                newIssue.Description = this.ReportTemplate.Replace("<TCID>", tc.ID)
                                                          .Replace("<TCRESULT>", Enum.GetName(typeof(EResult), tc.Result))
                                                          .Replace("<LOG>", log.Length > 75000 ? string.Concat(log.Substring(0, 75000), "\n\nfurther log needed to get cut due to ticket length limitations") : log);
                try
                {
                    IssuesCreated.Add(await this.jira.Issues.CreateIssueAsync(newIssue));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            if (IssuesCreated.Count > 0)
            {
                StringBuilder createdIssuesBuilder = new StringBuilder();
                createdIssuesBuilder.AppendFormat("Created {0} issues\n\n", IssuesCreated.Count);
                foreach (var it in IssuesCreated)
                {
                    createdIssuesBuilder.AppendFormat("{0};", it);
                }
                createdIssuesBuilder.Append("\n\nYou can aquire a copy of this MessageBox by pressing CTRL+C :)");
                MessageBox.Show(createdIssuesBuilder.ToString(), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(string.Concat("All testcases selected are already reported :)\nYou can find them using following filter:\n", jql), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
