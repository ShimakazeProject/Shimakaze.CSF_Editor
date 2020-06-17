using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RA2CsfEditor.Model
{
    /// <summary>
    /// 标签列表显示的结构
    /// </summary>
    public class LabelStruct : INotifyPropertyChanged
    {
        // 标签类型
        public string Type { get; private set; }
        public Label[] Labels { get; private set; }
        public bool Visibility { get; set; }
        public LabelStruct(Label label)
        {
            Visibility = true;
            Labels = new Label[0];
            var tag = label.LabelString.Split(new char[] { ':', '_' });
            Type = tag.Length != 1 ? tag[0].ToUpper() : "(default)";
            Add(label);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 多字符串时使用
        /// </summary>
        /// <param name="label"></param>
        public void Add(Label label)
        {
            label.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Visibility")
                {
                    Visibility = false;
                    foreach (var lbl in Labels)
                    {
                        if (lbl.Visibility)
                        {
                            Visibility = true;
                        }
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
                }
            };
            Labels = Labels.Concat(new Label[] { label }).ToArray();
        }
    }
}
