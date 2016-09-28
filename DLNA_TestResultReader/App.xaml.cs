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
using System.Windows;
using System.Runtime.InteropServices;

namespace DLNA_TestResultReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
        //private struct ReportFilesStruct
        //{
        //    public string path;
        //    public string buildname;
        //    public bool fails;
        //    public bool warns;
        //    public bool nas;
        //};
        protected override void OnStartup(StartupEventArgs e)
        {

            string stringsxaml = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Strings.xaml");
            stringsxaml = stringsxaml.Substring("file:\\\\".Length - 1);
            using (var stream = System.IO.File.OpenRead(stringsxaml))
            {
                ResourceDictionary rd = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                Application.Current.Resources.MergedDictionaries.Add(rd);
            }
            bool close = true;
            var argEnumerator = e.Args.GetEnumerator();
            AttachConsole(-1);
            for (string arg; argEnumerator.MoveNext();)
            {
                arg = argEnumerator.Current as string;
                if (arg[0] == '/' || arg[0] == '-')
                {
                    var cmd = arg.Substring(1).ToLower();
                    string username = string.Empty;
                    string password = string.Empty;
                    //var filesToReport = new List<ReportFilesStruct>();
                    switch (cmd)
                    {
                        case "export":
                            {
                                if (!argEnumerator.MoveNext())
                                    break;
                                arg = argEnumerator.Current as string;
                                if (!argEnumerator.MoveNext())
                                    break;
                                string pathFrom = argEnumerator.Current as string;
                                if (!argEnumerator.MoveNext())
                                    break;
                                string pathTo = System.IO.Path.GetFullPath(argEnumerator.Current as string);

                                if (!System.IO.File.Exists(pathFrom))
                                {
                                    Console.Error.WriteLine(string.Format("The path '{0}' cannot be found.", pathFrom));
                                    break;
                                }
                                switch (arg.ToLower())
                                {
                                    case "csv":
                                        try
                                        {
                                            FileFormats.TRF.TestResultFile trf = FileFormats.TRF.TestResultFile.Deserialize(pathFrom);
                                            foreach (var tr in trf.TestRuns)
                                            {
                                                ResultViewer.Exporter.ToType(tr, System.IO.Path.Combine(pathTo, string.Concat(tr.Timestamp.ToString("yyyyMMdd_HHmmss"), ".csv")));
                                            }
                                        }
                                        catch (Exception er)
                                        {
                                            Console.Error.WriteLine(er.Message);
                                        }
                                        break;
                                }
                            }
                            break;
                        case "credentials":
                            {
                                if (!argEnumerator.MoveNext())
                                    break;
                                username = argEnumerator.Current as string;
                                if (!argEnumerator.MoveNext())
                                    break;
                                password = argEnumerator.Current as string;
                            }
                            break;
                        //case "report":
                        //    {
                        //        var rfs = new ReportFilesStruct();
                        //        if (!argEnumerator.MoveNext())
                        //            break;
                        //        rfs.path = argEnumerator.Current as string;
                        //        if (!argEnumerator.MoveNext())
                        //            break;
                        //        rfs.buildname = argEnumerator.Current as string;
                        //        if (!argEnumerator.MoveNext())
                        //            break;
                        //        var filter = argEnumerator.Current as string;
                        //        rfs.fails = filter.Contains("f");
                        //        rfs.warns = filter.Contains("w");
                        //        rfs.nas = filter.Contains("n");
                        //        filesToReport.Add(rfs);
                        //    }
                        //    break;
                        case "help":
                        case "?":
                            {
                                Console.WriteLine("help");
                                Console.WriteLine("?");
                                Console.WriteLine("export <type: { csv }> <sourcefile> <targetfolder>");
                                Console.WriteLine("credentials <username> <password>");
                                //Console.WriteLine("report <sourcefile> <buildname> <filter: { f, w, n }; f => failed; w => warnings; n => NA>");
                            }
                            break;
                    }
                    //foreach(var rfs in filesToReport)
                    //{
                    //    try
                    //    {
                    //        var jc = new JIRA.JiraContext();
                    //        jc.BuildName = rfs.buildname;
                    //        jc.ReportFailed = rfs.fails;
                    //        jc.ReportNotApplicable = rfs.nas;
                    //        jc.ReportWarnings = rfs.warns;
                    //        jc.jira = Atlassian.Jira.Jira.CreateRestClient(App.Current.FindResource("ServerAdress") as string, username, password);
                    //        FileFormats.TRF.TestResultFile trf = FileFormats.TRF.TestResultFile.Deserialize(arg);
                    //        trf.JiraContext = jc;
                    //        trf.JiraCommitCommand.Execute(null);
                    //    }
                    //    catch (Exception er)
                    //    {
                    //        Console.Error.WriteLine(er.Message);
                    //    }
                    //}
                }
                else
                {
                    try
                    {
                        ResultViewer.Dialog dlg = new ResultViewer.Dialog(new ResultViewer.DialogContext(arg));
                        dlg.Show();
                        close = false;
                    }
                    catch (Exception er)
                    {
                        Console.Error.WriteLine(er.Message);
                        MessageBox.Show(er.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            base.OnStartup(e);
            if (e.Args.Count() == 0)
            {
                try
                {
                    ResultViewer.Dialog dlg = new ResultViewer.Dialog(new ResultViewer.DialogContext(""));
                    dlg.Show();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (close)
                App.Current.Shutdown();
        }
    }
}
