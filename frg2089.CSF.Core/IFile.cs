using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core
{
    public interface IFile
    {
        /// <summary>
        /// 文件头
        /// </summary>
        IHeader Header { get; }
        /// <summary>
        /// 标签列表
        /// </summary>
        IEnumerable<ILabel> Labels { get; }

    }
}
