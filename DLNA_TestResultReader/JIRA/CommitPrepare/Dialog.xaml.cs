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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DLNA_TestResultReader.JIRA.CommitPrepare
{
    /// <summary>
    /// Interaction logic for JiraCommitWindow.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var it in e.RemovedItems)
            {
                if (it is ResultFileUtil.ITestCase)
                {
                    var tc = it as ResultFileUtil.ITestCase;
                    switch (tc.Result)
                    {
                        case ResultFileUtil.EResult.Failed:
                            if (cb_SelectFailed.IsChecked.HasValue && cb_SelectFailed.IsChecked.Value)
                                cb_SelectFailed.IsChecked = null;
                            break;
                        case ResultFileUtil.EResult.Warning:
                            if (cb_SelectWarning.IsChecked.HasValue && cb_SelectWarning.IsChecked.Value)
                                cb_SelectWarning.IsChecked = null;
                            break;
                        case ResultFileUtil.EResult.NotApplicable:
                            if (cb_SelectNotApplicable.IsChecked.HasValue && cb_SelectNotApplicable.IsChecked.Value)
                                cb_SelectNotApplicable.IsChecked = null;
                            break;
                    }
                }
            }
            foreach (var it in e.AddedItems)
            {
                if (it is ResultFileUtil.ITestCase)
                {
                    var tc = it as ResultFileUtil.ITestCase;
                    switch (tc.Result)
                    {
                        case ResultFileUtil.EResult.Failed:
                            if(cb_SelectFailed.IsChecked.HasValue && !cb_SelectFailed.IsChecked.Value)
                                cb_SelectFailed.IsChecked = null;
                            break;
                        case ResultFileUtil.EResult.Warning:
                            if (cb_SelectWarning.IsChecked.HasValue && !cb_SelectWarning.IsChecked.Value)
                                cb_SelectWarning.IsChecked = null;
                            break;
                        case ResultFileUtil.EResult.NotApplicable:
                            if (cb_SelectNotApplicable.IsChecked.HasValue && !cb_SelectNotApplicable.IsChecked.Value)
                                cb_SelectNotApplicable.IsChecked = null;
                            break;
                    }
                }
            }
        }

        private List<object> getItemsOfResult(ResultFileUtil.EResult res)
        {
            List<object> lbiList = new List<object>();
            foreach (var it in lb.Items)
            {
                if (it is ResultFileUtil.ITestCase)
                {
                    var tc = it as ResultFileUtil.ITestCase;
                    if (tc.Result == res)
                    {
                        lbiList.Add(it);
                    }
                }
            }
            return lbiList;
        }

        private void cb_SelectFailed_Checked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.Failed);
            foreach(var it in lbiList)
                lb.SelectedItems.Add(it);
        }

        private void cb_SelectFailed_Unchecked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.Failed);
            foreach (var it in lbiList)
                lb.SelectedItems.Remove(it);
        }

        private void cb_SelectWarning_Checked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.Warning);
            foreach (var it in lbiList)
                lb.SelectedItems.Add(it);
        }

        private void cb_SelectWarning_Unchecked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.Warning);
            foreach (var it in lbiList)
                lb.SelectedItems.Remove(it);
        }

        private void cb_SelectNotApplicable_Checked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.NotApplicable);
            foreach (var it in lbiList)
                lb.SelectedItems.Add(it);
        }

        private void cb_SelectNotApplicable_Unchecked(object sender, RoutedEventArgs e)
        {
            List<object> lbiList = getItemsOfResult(ResultFileUtil.EResult.NotApplicable);
            foreach (var it in lbiList)
                lb.SelectedItems.Remove(it);
        }
    }
}
