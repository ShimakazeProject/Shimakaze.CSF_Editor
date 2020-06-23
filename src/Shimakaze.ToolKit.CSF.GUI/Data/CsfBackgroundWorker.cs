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
    public class CsfBackgroundWorker<T> : INotifyPropertyChanged
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusString)));
            }
        }
        public TaskStatus TaskStatus
        {
            get => taskStatus; private set
            {
                taskStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskStatus)));
            }
        }
        public int MaxProgress
        {
            get => maxProgress; private set
            {
                maxProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxProgress)));
            }
        }
        public int Progress
        {
            get => progress; private set
            {
                progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }
        public bool UnknowProgress
        {
            get => unknowProgress; private set
            {
                unknowProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnknowProgress)));
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
        public delegate void CsfBackgroundWorkerFinishedHander(CsfBackgroundWorker<T> sender, T result);
        public event CsfBackgroundWorkerFinishedHander Finished;
        public event PropertyChangedEventHandler PropertyChanged;
        public async void BeginParse(Stream stream)
        {
            if (TaskStatus == TaskStatus.Running) return;// 重复运行
            else TaskStatus = TaskStatus.Running;
            const string progressStr = "正在读取第{0}/{1}个标签";

            // 变量声明区
            T file = new T();// 创建实例
            int h_labelCount, l_count, l_nameLength,
                this_label_number, this_value_number;
            string  l_flag, l_name;
            CsfLabelStruct tmp_label;
            // ====================================================
            if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
            try
            {
                // 获取文件头

                StatusString = "正在读取文件头";
                file.Head = await CsfHeadStruct.ParseAsync(stream);
                h_labelCount = file.Head.LabelCount;
                MaxProgress = file.Head.StringCount;

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
                    if (!l_flag.Equals(CsfLabelStruct.CSF_LABEL_FLAG))
                        throw new FormatException(string.Format("Unknown File Format: Label[{0}] Flag is NOT True", this_label_number));

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

                        tmp_label.Add(await CsfValueStruct.ParseAsync(stream));
                        Progress++;
                    }
                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    file.AddNoChangeHead(tmp_label);
                }
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
            finally
            {
                await stream.DisposeAsync();
            }
            Error = null;
            TaskStatus = TaskStatus.RanToCompletion;
            Finished?.Invoke(this, result);
        }
        public void Cancel()
        {
            TaskStatus = TaskStatus.Canceled;
        }

        public async void BeginDeparse(Stream stream, T file)
        {
            if (file is null)
            {
                Error = new NullReferenceException();
                TaskStatus = TaskStatus.Faulted;
                return;
            }
            if (TaskStatus == TaskStatus.Running) return;// 重复运行
            else TaskStatus = TaskStatus.Running;
            const string progressStr = "正在写入第{0}/{1}个标签";

            // 变量声明区
            int h_labelCount, l_count,
                this_label_number, this_value_number;
            CsfLabelStruct tmp_label;
            // ====================================================
            if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
            try
            {
                StatusString = "正在写入文件头";
                await file.Head.DeparseAsync(stream);
                h_labelCount = file.Head.LabelCount;
                MaxProgress = file.Head.StringCount;
                UnknowProgress = false;
                Progress = 0;
                if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                for (this_label_number = 0; this_label_number < h_labelCount; this_label_number++)
                {
                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    StatusString = string.Format(progressStr, this_label_number, h_labelCount);

                    tmp_label = file[this_label_number];

                    // 标签头
                    await stream.WriteAsync(Encoding.ASCII.GetBytes(CsfLabelStruct.CSF_LABEL_FLAG));
                    // 字符串数量 
                    await stream.WriteAsync(BitConverter.GetBytes(tmp_label.Count));
                    // 标签名长度
                    await stream.WriteAsync(BitConverter.GetBytes(tmp_label.Name.Length));
                    // 标签名
                    await stream.WriteAsync(Encoding.ASCII.GetBytes(tmp_label.Name));

                    l_count = tmp_label.Count;

                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    for (this_value_number = 0; this_value_number < l_count; this_value_number++)
                    {
                        await tmp_label[this_value_number].DeparseAsync(stream);

                        Progress++;
                        if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                    }
                    if (TaskStatus == TaskStatus.Canceled) throw new TaskCanceledException();
                }
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
            finally
            {
                await stream.DisposeAsync();
            }
            Error = null;
            TaskStatus = TaskStatus.RanToCompletion;
            Finished?.Invoke(this, result);
        }
    }
}
