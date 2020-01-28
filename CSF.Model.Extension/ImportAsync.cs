using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Model.Extension
{
    public static class ImportAsync
    {
        public static Task AddAsync(this Type[] types, Label label)
        {
            return new Task(() =>
            {
                var type = new Type(label);
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == type.Name)
                    {
                        types[i].Add(label);
                        return;
                    }
                }
                types.Concat(new Type[] { type });
            });
        }
        public static async Task FromFileAsync(this Type[] type, Core.IFile File)
        {
            foreach (var label in File.Labels)
            {
                await type.AddAsync(new Label(label));
            }
        }
        public static async Task FromCSFAsync(this Type[] type, string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            await type.FromFileAsync(Core.Helper.FileHelper.CreateFile(bytes));
        }
    }
}
