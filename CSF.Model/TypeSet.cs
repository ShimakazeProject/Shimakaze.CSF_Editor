using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public class TypeSet
    {
        public TypeSet()
        {
            Types = Array.Empty<Type>();
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
        /// 标签数
        /// </summary>
        public int LabelCount { get; private set; }
        /// <summary>
        /// 字符串数
        /// </summary>
        public int StringCount { get; private set; }
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public byte[] Unknow { get; private set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public int Language { get; private set; }

        public Type[] Types { get; set; }

        public void MakeType(File file)
        {
            Flag = file.Flag;
            Version = file.Version;
            LabelCount = file.LabelCount;
            StringCount = file.StringCount;
            Unknow = file.Unknow;
            Language = file.Language;
            var labels = file.Labels;
            var types = new List<Type>();
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
                    types.Add(new Type(typeName.ToUpper(), label));
                }
            }
            Types = types.ToArray();
        }


        #region Indexer
        public Type this[int index]
        {
            get => Types[index];
            set => Types[index] = value;
        }
        public Type this[string name]
        {
            get
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    if (Types[i].Name.Equals(name))
                    {
                        return Types[i];
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    if (Types[i].Name.Equals(name))
                    {
                        Types[i] = value;
                    }
                }
            }
        }
        #endregion
    }
}
