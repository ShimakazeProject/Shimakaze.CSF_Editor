using CSF.Model.Extension;
using CSF.WPF.Framework.CommandBase;
using CSF.WPF.Framework.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.WPF.Framework.ViewModel
{
    public class DocumentVM : INotifyPropertyChanged
    {
        private Controls.Document document;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties Fields
        private Model.Type[] types;
        private Model.Type selectedType;
        private Model.Label selectedLabel;
        private int typesCount;
        private string selectedTypeLabelsNum;
        private string selectedTypeStringsNum;
        private string selectedLabelStringsNum;
        private int stringCount;
        private int labelCount;
        #endregion

        public DocumentVM(Controls.Document document)
        {
            this.document = document;
            Types = Array.Empty<Model.Type>();

            PropertyChanged += DocumentVM_PropertyChanged;

        }

        private void DocumentVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            switch (e.PropertyName)
            {
                case nameof(Types):
                    TypesCount = types.Length;// 类型总数
                    break;
                case nameof(SelectedType):
                    SelectedTypeLabelsNum = selectedType?.Labels.Length.ToString();
                    SelectedTypeStringsNum = selectedType != null ? (from label in selectedType.Labels select label.LabelValues.Length)?.Sum().ToString() : null;
                    break;
                case nameof(SelectedLabel):
                    SelectedLabelStringNum = selectedLabel?.LabelStrCount.ToString();
                    break;
                default:
                    break;
            }
            //stopwatch.Stop();
            //Console.WriteLine("用时:{0}秒", ((double)stopwatch.ElapsedMilliseconds / 1000).ToString("0.000"));
        }

        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        public string Flag { get; private set; }
        /// <summary>
        /// 文件版本
        /// </summary>
        public int Version { get; private set; }



        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public byte[] Unknow { get; private set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public CsfLanguage Language { get; private set; }

        public void MakeType(Model.File file)
        {
            Flag = file.Flag;
            Version = file.Version;
            LabelCount = file.LabelCount;
            StringCount = file.StringCount;
            Unknow = file.Unknow;
            Language = (CsfLanguage)file.Language;
            var labels = file.Labels;
            var types = new List<Model.Type>();
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                var split = label.LabelName.Split(':', '_');
                string typeName = split.Length > 1 ? split[0] : "(Default)";

                bool finished = false;
                for (int j = 0; j < types.Count; j++)
                {
                    if (types[j].Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    {
                        types[j] += label;
                        finished = true;
                        break;
                    }
                }
                if (!finished)
                {
                    types.Add(new Model.Type(typeName.ToUpper(), label));
                }
            }
            Types = types.ToArray();
        }
        #region Binding Propertys
        public Model.Type[] Types
        {
            get => types; set
            {
                types = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            }
        }
        public Model.Type SelectedType
        {
            get => selectedType; set
            {
                selectedType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            }
        }
        public Model.Label SelectedLabel
        {
            get => selectedLabel; set
            {
                selectedLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabel)));
            }
        }


        /// <summary>
        /// 类型数
        /// </summary>
        public int TypesCount
        {
            get => typesCount; set
            {
                typesCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypesCount)));
            }
        }
        /// <summary>
        /// 标签数
        /// </summary>
        public int LabelCount
        {
            get => labelCount; private set
            {
                labelCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelCount)));
            }
        }
        /// <summary>
        /// 字符串数
        /// </summary>
        public int StringCount
        {
            get => stringCount; private set
            {
                stringCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StringCount)));
            }
        }

        public string SelectedTypeLabelsNum
        {
            get => selectedTypeLabelsNum; set
            {
                selectedTypeLabelsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeLabelsNum)));
            }
        }
        public string SelectedTypeStringsNum
        {
            get => selectedTypeStringsNum; set
            {
                selectedTypeStringsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeStringsNum)));
            }
        }
        public string SelectedLabelStringNum
        {
            get => selectedLabelStringsNum; set
            {
                selectedLabelStringsNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabelStringNum)));
            }
        }
        #endregion
        public RelayCommand<object> EditLabelCommand => new RelayCommand<object>(EditLabelMethod);
        public RelayCommand Refresh => new RelayCommand(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabel)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypesCount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeLabelsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTypeStringsNum)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLabelStringNum)));
        });

       public enum CsfLanguage : byte
        {
            US, ZERO1, GERMAN, FRENCH, ZERO2, ZERO3,
            ZERO4, ZERO5, KOREAN, CHINESE
        }

        public void EditLabelMethod(object obj)
        {
            if (obj.GetType() != typeof(Model.Label)) return;
            var newlabel = Dialog.EditDialog.ShowDialog(obj as Model.Label);
            (obj as Model.Label).Changed(newlabel);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedType)));
        }
        public void OpenCsf(string path)
        {
            var file = new Model.File();
            file.LoadFromFile(path);
            MakeType(file);
            //var task = Import.FromCSF(path);
            //Types = (await task).ToArray();
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Types)));
        }

    }
}
