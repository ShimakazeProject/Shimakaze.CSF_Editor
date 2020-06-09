using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Shimakaze.ToolKit.CSF.Kernel;
using Shimakaze.ToolKit.CSF.Kernel.Extension;

namespace Shimakaze.ToolKit.CSF.GUI.Data
{
    public class ParseBackgroundWorker<T> : INotifyPropertyChanged
        where T : CsfFileStruct, new()
    {
        private string statusString;
        private TaskStatus taskStatus;
        private int maxProgress = 0;
        private int progress = 0;
        private bool unknowProgress = true;
        private Exception error;
        private T result;
        public string StatusString
        {
            get => statusString; private set
            {
                statusString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public TaskStatus TaskStatus
        {
            get => taskStatus; private set
            {
                taskStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public int MaxProgress
        {
            get => maxProgress; private set
            {
                maxProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public int Progress
        {
            get => progress; private set
            {
                progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public bool UnknowProgress
        {
            get => unknowProgress; private set
            {
                unknowProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public Exception Error
        {
            get => error; private set
            {
                error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
            }
        }
        public delegate void ParseBackgroundWorkerFinishedHander(ParseBackgroundWorker<T> sender, T result);
        public event ParseBackgroundWorkerFinishedHander Finished;
        public event PropertyChangedEventHandler PropertyChanged;
        public async void BeginParse(Stream stream)
        {
            if (TaskStatus == TaskStatus.Running) return;// 重复运行
            else TaskStatus = TaskStatus.Running;
            const string progressStr = "正在读取第{0}/{1}个标签";

            // 变量声明区
            T file = new T();// 创建实例
            int h_version, h_labelCount, h_stringCount, h_unknown, h_language,
                l_count, l_nameLength,
                v_length, v_extraLength,
                this_label_number, this_value_number;
            string h_flag, l_flag, l_name, v_flag, v_content, v_extra;
            CsfLabelStruct tmp_label;
            CsfValueStruct tmp_value;
            // ====================================================
            if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
            try
            {
                StatusString = "正在读取文件头";
                // 获取文件头
                h_flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
                if (!h_flag.Equals(CsfHeadStruct.CSF_FLAG)) throw new FormatException("Unknown File Format: Flag is NOT True");

                // 文件版本
                h_version = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                // 文件标签数
                h_labelCount = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                // 文件字符串数   满进程
                h_stringCount = MaxProgress = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                // 不明内容
                h_unknown = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                // 语言
                h_language = BitConverter.ToInt32(await stream.ReadAsync(4), 0);

                UnknowProgress = false;
                Progress = 0;
                if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                for (this_label_number = 0; this_label_number < h_labelCount; this_label_number++)
                {
                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    StatusString = string.Format(progressStr, this_label_number, h_labelCount);

                    // 标签头
                    l_flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
                    // 标签头对不上
                    if (!l_flag.Equals(CsfLabelStruct.CSF_LABEL_FLAG)) throw new FormatException("Unknown File Format");

                    // 字符串数量 
                    l_count = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                    // 标签名长度
                    l_nameLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                    // 标签名
                    l_name = Encoding.ASCII.GetString(await stream.ReadAsync(l_nameLength));

                    tmp_label = new CsfLabelStruct(l_name);

                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    for (this_value_number = 0; this_value_number < l_count; this_value_number++)
                    {
                        if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                        // 字符串标记
                        v_flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
                        // 字符串主要内容长度
                        v_length = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                        // 字符串主要内容
                        v_content = Encoding.Unicode.GetString(
                            CsfValueStruct.Decoding(
                                await stream.ReadAsync(v_length << 1)));

                        tmp_value = new CsfValueStruct(v_content);
                        // 判断是否包含额外内容
                        if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                        if (v_flag.Equals(CsfValueStruct.CSF_VALUE_WSTR))// 存在额外内容
                        {
                            tmp_value.IsWstr = true;
                            // 额外内容长度
                            v_extraLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                            // 额外内容
                            tmp_value.Extra = v_extra = Encoding.ASCII.GetString(await stream.ReadAsync(v_extraLength));
                        }
                        Progress++;
                        if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                        tmp_label.Add(tmp_value);
                    }
                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    file.Add(tmp_label);
                }
                file.Head = new CsfHeadStruct(h_version, h_labelCount, h_stringCount, h_unknown, h_language);
                result = file;
            }
            catch (TaskCanceledException)
            {
                TaskStatus = TaskStatus.Canceled;
                return;
            }
            catch (Exception e)
            {
                Error = e;
                TaskStatus = TaskStatus.Faulted;
                Finished?.Invoke(this, result);
                return;
            }
            Error = null;
            TaskStatus = TaskStatus.RanToCompletion;
            Finished?.Invoke(this, result);
        }
        public void Cancel()
        {
            TaskStatus = TaskStatus.Canceled;
        }
    }
}
