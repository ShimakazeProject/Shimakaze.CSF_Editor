using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public class TypeSet:File
    {
        public TypeSet()
        {
            Types = Array.Empty<Type>();
        }

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
        public new Type this[int index]
        {
            get => Types[index];
            set => Types[index] = value;
        }
        public new Type this[string name]
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
