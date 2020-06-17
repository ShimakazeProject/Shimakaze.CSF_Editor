namespace RA2CsfEditor.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    public class CSF
    {
        #region 文件属性
        /// <summary>
        /// 文件头
        /// </summary>
        public Header Header { get; private set; }
        /// <summary>
        /// 标签列表
        /// </summary>
        public Label[] Label { get; private set; }
        #endregion
        public CSF()
        {
            Label = new Label[0];
            Header = new Header();
        }
        public CSF(int ver, int lang, IEnumerable<Label> l)
        {
            Label = l.ToArray();
            MakeHeader(ver,l.Count(),(from label in l select label.StringNum).Sum(), lang);
        }
        #region 用户方法
        /// <summary>
        /// 修改文件头
        /// </summary>
        /// <param name="header">文件头</param>
        public void ChangeHeader(Header header)
        {
            Header = header;
        }
        /// <summary>
        /// 修改一个标签
        /// </summary>
        /// <param name="label">文件标签</param>
        /// <param name="index">索引</param>
        public void ChangeLabel(Label label, int index)
        {
            Label[index] = label;
        }
        /// <summary>
        /// 增加一个标签
        /// </summary>
        /// <param name="label">标签</param>
        public void AddLabel(Label label)
        {
            for (int i = 0; i < Label.Length; i++)
            {
                if (Label[i].LBLequal(label))
                {
                    Header.DropString(Label[i].StringNum);
                    Label[i] = label;
                    Header.AddString(label.StringNum);
                    return;
                }
            }
            Label = Label.Concat(new Label[] { label }).ToArray();
            Header.AddLabel(1);
            Header.AddString(label.StringNum);
        }
        public void SetLabel(IEnumerable<Label> labels)
        {
            Label = labels.ToArray();
        }
        /// <summary>
        /// 删除一个标签
        /// </summary>
        /// <param name="label"></param>
        public void DropLabel(Label label)
        {
            var tmp = Label.ToList();
            tmp.Remove(label);
            Label = tmp.ToArray();
            Header.DropLabel(label.StringNum);
        }
        /// <summary>
        /// ! 清空文件标签列表
        /// </summary>
        public void CleanLabels()
        {
            Label = new Label[0];
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="Path">文件路径</param>
        public async Task LoadFromFile(string Path)
        {
            var Label = new List<Label>();// 初始化
            using (FileStream fs = new FileStream(Path, FileMode.Open))
            {
                // =================== 文件头 =================
                byte[] FlagBytes = new byte[4];// 文件头
                byte[] VersionBytes = new byte[4];// 版本
                byte[] NumLabelsBytes = new byte[4];// 标签数
                byte[] NumStrings = new byte[4];// 字符串数
                byte[] NullBytes = new byte[4];// 无用内容
                byte[] LanguageBytes = new byte[4];// 语言
                                                   // ========= 异步读取字节数组 ===========
                await fs.ReadAsync(FlagBytes, 0, 4);
                await fs.ReadAsync(VersionBytes, 0, 4);
                await fs.ReadAsync(NumLabelsBytes, 0, 4);
                await fs.ReadAsync(NumStrings, 0, 4);
                await fs.ReadAsync(NullBytes, 0, 4);
                await fs.ReadAsync(LanguageBytes, 0, 4);
                // ========= 数据处理 =========
                string Flag = Encoding.ASCII.GetString(FlagBytes);
                int headVersion = BitConverter.ToInt32(VersionBytes, 0);
                int headLabelNum = BitConverter.ToInt32(NumLabelsBytes, 0);
                int headStringNum = BitConverter.ToInt32(NumStrings, 0);
                int headLanguage = BitConverter.ToInt32(LanguageBytes, 0);
                // 文件头信息写入
                Header = new Header(Flag, headVersion, headLabelNum, headStringNum, NullBytes, headLanguage);
                // ===============================
                int stringnum = 0;
                for (int labelno = 0; labelno < headLabelNum; labelno++)
                {
                    // =========== 先声明一堆看起来很厉害的字段
                    byte[] lblTag = new byte[4];// 标签标记
                    byte[] lblNum = new byte[4];// 标签字符串数
                    byte[] lblLen = new byte[4];// 标签长
                    byte[] lblVal = new byte[4];// 标签字符串
                    byte[] vleTag = new byte[4];// 值标记
                    byte[] vleLen = new byte[4];// 值字符串长度
                    byte[] vleStr = new byte[4];// 值字符串
                    byte[] vleEle = new byte[4];// 额外值长度
                    byte[] vleEst = new byte[4];// 额外值字符串
                                                // ========= 标签
                    await fs.ReadAsync(lblTag, 0, 4);// 异步读文件字节 标签头
                    await fs.ReadAsync(lblNum, 0, 4);// 异步读文件字节 字符串对数
                    await fs.ReadAsync(lblLen, 0, 4);// 异步读文件字节 标签长
                    string ltag = Encoding.ASCII.GetString(lblTag);// 字节数组到字符串
                    int lnum = BitConverter.ToInt32(lblNum, 0);// 字节数组到整数
                    int llen = BitConverter.ToInt32(lblLen, 0);// byte[] => int
                    lblVal = new byte[llen];// 重实例化标签内容 修正长度
                    await fs.ReadAsync(lblVal, 0, llen);// 异步读
                    string lstr = Encoding.ASCII.GetString(lblVal);// byte[]=>string
                                                                   // ========= 值
                    await fs.ReadAsync(vleTag, 0, 4);
                    await fs.ReadAsync(vleLen, 0, 4);
                    string vtag = Encoding.ASCII.GetString(vleTag);
                    List<Value> values = new List<Value>();
                    for (int i = 0; i < lnum; i++)
                    {
                        int vlen = BitConverter.ToInt32(vleLen, 0);
                        vleStr = new byte[2 * vlen];
                        await fs.ReadAsync(vleStr, 0, 2 * vlen);
                        string vstr = Encoding.Unicode.GetString(Decoding(vlen, vleStr));
                        values.Add(new Value(vlen, vstr));
                        stringnum++;
                    }
                    if (vtag == "WRTS")
                    {
                        await fs.ReadAsync(vleEle, 0, 4);
                        int vele = BitConverter.ToInt32(vleEle, 0);
                        vleEst = new byte[vele];
                        await fs.ReadAsync(vleEst, 0, vele);
                        string vest = Encoding.ASCII.GetString(vleEst);

                        // ==================== 将结果写入属性 =========================
                        Label.Add(new Label(ltag, lnum, llen, lstr, vtag, values.ToArray(), vele, vest));
                    }
                    else Label.Add(new Label(ltag, lnum, llen, lstr, vtag, values.ToArray()));
                    //// 调试输出
                    //System.Diagnostics.Debug.Write("\n" + labelno.ToString() + "\t");
                    //System.Diagnostics.Debug.Write(vstrs[0]);
                }
                this.Label = Label.ToArray();
                // ====== 可能出现的异常 ======
                if (Flag != " FSC") System.Diagnostics.Debug.Write("不是有效的csf文件"); // 不是有效的csf文件 尚未定义异常
                if (stringnum != headStringNum) System.Diagnostics.Debug.Write("喵喵喵???");// 字符串数对不上 尚未定义异常
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="Path">文件路径</param>
        public async Task SaveToFile(string Path)
        {
            using (FileStream fs = new FileStream(Path, FileMode.Create))
            {
                await fs.WriteAsync(
                    // Flag
                    new byte[0].Concat(Encoding.ASCII.GetBytes(Header.Flag))
                    // Version
                    .Concat(BitConverter.GetBytes(Header.Version))
                    // Labels Count
                    .Concat(BitConverter.GetBytes(Header.NumLabel))
                    // Strings Count
                    .Concat(BitConverter.GetBytes(Header.NumString))
                    // Unknow Bytes
                    .Concat(Header.Message)
                    // Languang
                    .Concat(BitConverter.GetBytes(Header.Language)).ToArray(),
                0, 24);
                foreach (var (label, VLVSs) in
                // ====== 文件头写完了
                from label in Label
                let VLVSs = new byte[0]
                select (label, VLVSs))
                {
                    foreach (var item in from value in label.Values
                                         select BitConverter.GetBytes(value.ValueLength)
                                         .Concat(Decoding(value.ValueLength, Encoding.Unicode.GetBytes(value.ValueString))))
                        VLVSs.Concat(item);
                    var tmplist = new byte[0]
                        // Label Tag
                        .Concat(Encoding.ASCII.GetBytes(label.LabelTag))
                        // Strings Count
                        .Concat(BitConverter.GetBytes(label.StringNum))
                        // Label Tag Length
                        .Concat(BitConverter.GetBytes(label.LabelLength))
                        // Label Tag String
                        .Concat(Encoding.ASCII.GetBytes(label.LabelString))
                        // Value Tag
                        .Concat(Encoding.ASCII.GetBytes(label.ValueTag))
                        // Value Strings
                        .Concat(VLVSs)
                        // Extra Value Length
                        .Concat(label.ExtraValueLength != null ? BitConverter.GetBytes((int)label.ExtraValueLength) : (new byte[0]))
                        // Extra Value
                        .Concat(label.ExtraValueLength != null ? Encoding.ASCII.GetBytes(label.ExtraValue) : (new byte[0])).ToArray();
                    await fs.WriteAsync(tmplist, 0, tmplist.Length);
                }
                await fs.FlushAsync();
            }
        }
        /// <summary>
        /// 导出txt
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public async Task SaveAsText(string Path)
        {
            FileStream fs = new FileStream(Path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            await sw.WriteLineAsync("=============== Header ==============");
            await sw.WriteLineAsync("Flag :" + Header.Flag);
            await sw.WriteLineAsync("Version:" + Header.Version.ToString());
            await sw.WriteLineAsync("LabelNumber:" + Header.NumLabel.ToString());
            await sw.WriteLineAsync("StringNumber:" + Header.NumString.ToString());
            string result = string.Empty;
            foreach (var b in Header.Message)//逐字节变为16进制字符
            {
                result += Convert.ToString(b, 16) + " ";
            }
            await sw.WriteLineAsync("Message:" + result);
            await sw.WriteLineAsync("Language:" + Header.Language);
            await sw.WriteLineAsync("=============== Header ==============");
            // ====== 文件头写完了
            foreach (var label in Label)
            {
                await sw.WriteAsync(label.LabelString + " | ");
                foreach (var values in label.Values) await sw.WriteAsync(values.ValueString + "|");

                await sw.WriteLineAsync();
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        /// <summary>
        /// 搜索字符串标签
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public int[] SearchLabel(string label)
        {
            List<int> returni1 = new List<int>();
            List<int> returni2 = new List<int>();
            for (int i = 0; i < Label.Length; i++)
            {
                if (Label[i].LabelString.ToUpper() == label.ToUpper())// 全字匹配
                {
                    returni1.Add(i);
                    continue;
                }
                else if (Label[i].LabelString.ToUpper().Contains(label.ToUpper()))// 关键字匹配
                {
                    returni2.Add(i);
                    continue;
                }
            }
            returni1.AddRange(returni2);
            return returni1.ToArray();
        }
        /// <summary>
        /// 搜索字符串内容
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public int[] SearchString(string @string)
        {
            List<int> returni1 = new List<int>();
            List<int> returni2 = new List<int>();
            for (int i = 0; i < Label.Length; i++)
            {
                foreach (var values in Label[i].Values)
                {
                    var str = values.ValueString;
                    if (str.ToUpper() == @string.ToUpper())// 全字匹配
                    {
                        returni1.Add(i);
                        break;
                    }
                    else if (str.ToUpper().Contains(@string.ToUpper()))// 关键字匹配
                    {
                        returni2.Add(i);
                        break;
                    }
                }
            }
            returni1.AddRange(returni2);
            return returni1.ToArray();
        }
        /// <summary>
        /// 搜索功能
        /// </summary>
        /// <param name="str">要搜索的东西</param>
        /// <param name="mode">搜索模式 null为全部 true 仅标签 false 仅字符串值</param>
        /// <param name="fullOrRegex">true 全字匹配 false 正则表达式</param>
        /// <param name="regex"></param>
        public void Search(string str, bool? mode, bool? fullOrRegex = null)
        {
            for (int i = 0; i < Label.Length; i++)
            {
                Label[i].Visibility = false;
                if (string.IsNullOrWhiteSpace(str))// 全部显示
                {
                    Label[i].Visibility = true;
                    continue;
                }
                if (mode ?? true)// 搜索标签
                {
                    if (fullOrRegex == true && Label[i].LabelString.ToUpper() == str.ToUpper())// 全字匹配
                    {
                        Label[i].Visibility = true;
                        continue;
                    }
                    else if (fullOrRegex == false)// 正则匹配
                    {
                        try
                        {
                            if (Regex.IsMatch(Label[i].LabelString, str, RegexOptions.IgnoreCase))
                            {
                                Label[i].Visibility = true;
                                continue;
                            }
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                    else if (fullOrRegex == null && Label[i].LabelString.ToUpper().Contains(str.ToUpper()))// 关键字匹配
                    {
                        Label[i].Visibility = true;
                        continue;
                    }
                }
                if (!(mode ?? false))// 搜索值内容
                {
                    foreach (var values in Label[i].Values)
                    {
                        var vstr = values.ValueString;
                        if (fullOrRegex == true && vstr.ToUpper() == str.ToUpper())// 全字匹配
                        {
                            Label[i].Visibility = true;
                            continue;
                        }
                        else if (fullOrRegex == false)// 正则匹配
                        {
                            try
                            {
                                if (Regex.IsMatch(vstr, str, RegexOptions.IgnoreCase))
                                {
                                    Label[i].Visibility = true;
                                    continue;
                                }
                            }
                            catch (ArgumentException)
                            {
                                continue;
                            }
                        }
                        else if (fullOrRegex == null && vstr.ToUpper().Contains(str.ToUpper()))// 关键字匹配
                        {
                            Label[i].Visibility = true;
                            continue;
                        }
                    }
                }
            }
        }

        public void MakeHeader(int ver, int lang)
        {
            Header = new Header(" FSC", ver, Header.NumLabel, Header.NumString, new byte[4], lang);
        }
        public void MakeHeader(int ver, int NumLabel, int NumString, int lang)
        {
            Header = new Header(" FSC", ver, NumLabel, NumString, new byte[4], lang);
        }
        #endregion

        /// <summary>
        /// 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        private static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
            {
                ValueData[i] = (byte)~ValueData[i];
            }
            return ValueData;
        }
    }
}
