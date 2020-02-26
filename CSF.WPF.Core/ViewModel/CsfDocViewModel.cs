﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CSF.WPF.Core.ViewModel
{
    public class CsfDocViewModel : INotifyPropertyChanged
    {
        private string filePath;
        private Model.Label data;
        private Model.Type dataList;
        private Model.TypeSet typeList;

        public CsfDocViewModel()
        {
            TypeList = new Model.TypeSet();
            EditViewModel = new EditViewModel();
            EditViewModel.BaseVM = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Binding Property
        public Model.Label Data
        {
            get => data; set
            {
                data = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            }
        }
        public Model.Type DataList
        {
            get => dataList; set
            {
                dataList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataList)));
            }
        }
        public Model.TypeSet TypeList
        {
            get => typeList; set
            {
                typeList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypeList)));
            }
        }
        public string FilePath
        {
            get => filePath; set
            {
                filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
            }
        }
        public EditViewModel EditViewModel { get; set; }
        #endregion

        public void Open(string path)
        {
            FilePath = path;
            var typelist = new Model.TypeSet();
            typelist.LoadFromFile(path);
            TypeList = typelist;
        }
        public void Merge(string path)
        {
            var list = new List<Model.Label>(TypeList.Labels);
            var typelist = new Model.TypeSet();
            typelist.LoadFromFile(path);
            list.AddRange(typelist.Labels);
            TypeList.Labels = list.ToArray();
            TypeList.MakeType();
        }
        public void Save() => TypeList.SaveToFile(FilePath);
        public void SaveAs(string path)
        {
            FilePath = path;
            Save();
        }
        public void ReCount()
        {
            TypeList.LabelCount = TypeList.Labels.Length;
            TypeList.StringCount = (from Model.Label label in TypeList.Labels select label.LabelStrCount).Sum();
            Update(1);
        }

        public void AddLabel()
        {
            var label = new Model.Label("new label", new Model.Value[] { new Model.Value("new value") });
            TypeList.Add(label);
            EditViewModel.SetLabel(label, true);
        }
        public void EditLabel()
        {
            EditViewModel.SetLabel(data);
        }
        public void DropLabel()
        {
            TypeList.Remove(data);
            ReCount();
            TypeList.MakeType();
            Update(1);
        }
        public void Update(int level = 0)
        {
            if (level <= 0) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypeList)));
            if (level <= 1) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataList)));
            if (level <= 2) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));

        }

        public void Search(string s, Data.SearchMode mode)
        {
            bool sregex = (mode & Core.Data.SearchMode.Regex) == Core.Data.SearchMode.Regex;
            bool sfull = (mode & Core.Data.SearchMode.Full) == Core.Data.SearchMode.Full;
            bool ignore = (mode & Core.Data.SearchMode.IgnoreCase) == Core.Data.SearchMode.IgnoreCase;
            for (int i = 0; i < TypeList.Labels.Length; i++)
            {
                var label = TypeList.Labels[i];
                label.Visibility = false;
                if (string.IsNullOrWhiteSpace(s))// 全部显示
                {
                    TypeList.Labels[i].Visibility = true;
                    continue;
                }
                try
                {
                    if ((mode & Core.Data.SearchMode.Label) == Core.Data.SearchMode.Label)// 搜索标签                    
                        Search(label, label.LabelName, s, sregex, sfull, ignore);
                    if ((mode & Core.Data.SearchMode.Value) == Core.Data.SearchMode.Value || (mode & Core.Data.SearchMode.Extra) == Core.Data.SearchMode.Extra)// 搜索值内容                    
                        foreach (var value in label.LabelValues)
                        {
                            if ((mode & Core.Data.SearchMode.Value) == Core.Data.SearchMode.Value)// 搜索值内容                            
                                Search(label, value.ValueString, s, sregex, sfull, ignore);
                            if ((mode & Core.Data.SearchMode.Extra) == Core.Data.SearchMode.Extra)// 搜索额外值                            
                                Search(label, value.ExtraString, s, sregex, sfull, ignore);
                        }
                    Update();
                }
                catch (System.ArgumentException)// 正则有误
                {
                    label.Visibility = true;
                    return;
                }
            }

        }

        private static void Search(Model.Label label, string vstr, string s, bool sregex, bool sfull, bool ignoreCase)
        {
            System.StringComparison stringComparison = System.StringComparison.Ordinal;
            System.Text.RegularExpressions.RegexOptions regexOptions = System.Text.RegularExpressions.RegexOptions.None;
            if (ignoreCase)
            {
                regexOptions |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                stringComparison = System.StringComparison.OrdinalIgnoreCase;
            }
            if (sfull && vstr.Equals(s, stringComparison))// 全字匹配
            {
                label.Visibility = true;
            }
            else if (sregex && System.Text.RegularExpressions.Regex.IsMatch(vstr, s, regexOptions))// 正则匹配
            {
                label.Visibility = true;
            }
            else if (vstr.Contains(s, stringComparison))// 关键字匹配
            {
                label.Visibility = true;
            }
        }
    }
}
