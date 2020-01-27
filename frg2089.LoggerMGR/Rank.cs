using System;
using System.Collections.Generic;
using System.Text;

namespace frg2089.LoggerMGR
{
    public enum Rank
    {
        ALL,// 全部开启

        TRACE,// 该级别日志，默认情况下，既不打印到终端也不输出到文件。
        DEBUG,// 该级别日志，默认情况下会打印到终端输出，但是不会归档到日志文件。
        INFO,// 一般这种信息都是一过性的，不会大量反复输出。
        WARN,// 程序处理中遇到非法数据或者某种可能的错误。
        ERROR,// 该错误发生后程序仍然可以运行，但是极有可能运行在某种非正常的状态下，导致无法完成全部既定的功能。
        FATAL,// 表明程序遇到了致命的错误，必须马上终止运行。

        OFF,// 全部关闭
    }
}
