using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSF.Core
{
    public abstract class CsfHeader
    {
        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        public abstract string Flag { get; }
        /// <summary>
        /// 文件版本
        /// </summary>
        public abstract int Version { get; }
        /// <summary>
        /// 标签数
        /// </summary>
        public abstract int LabelCount { get; }
        /// <summary>
        /// 字符串数
        /// </summary>
        public abstract int StringCount { get; }
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public abstract byte[] Unknow { get; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public abstract CsvLanguage Language { get; }
        public static CsfHeader GetHeader(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return GetHeader(ms);
        }
        public static CsfHeader GetHeader(Stream stream)
        {
            using (var br = new ByteReader(stream))
                return GetHeader(br.Read(4), br.Read(4), br.Read(4), br.Read(4), br.Read(4), br.Read(4));
        }
        public static CsfHeader GetHeader(string flag, int version, int labelCount, int stringCount, byte[] unknow, int language) 
            => GetHeader(flag, version, labelCount, stringCount, unknow, (byte)language);

        public static CsfHeader GetHeader(int version, int labelCount, int stringCount, int language)
            => GetHeader(" FSC", version, labelCount, stringCount, new byte[4], (byte)language);
        public static CsfHeader GetHeader(byte[] flag, byte[] version, byte[] labelCount, byte[] stringCount, byte[] nuknow, byte[] language)
            => GetHeader(Encoding.ASCII.GetString(flag), BitConverter.ToInt32(version, 0), BitConverter.ToInt32(labelCount, 0),
                BitConverter.ToInt32(stringCount, 0), nuknow, BitConverter.ToInt32(language, 0));

        /// <summary>
        /// 最重要的GetHeader方法
        /// </summary>
        public static CsfHeader GetHeader(string flag, int version, int labelCount, int stringCount, byte[] unknow, byte language)
            => new HeaderMaker(flag, version, labelCount, stringCount, unknow, (CsvLanguage)language);
    }
    public enum CsvLanguage : byte
    {
        US, ZERO1, GERMAN, FRENCH, ZERO2, ZERO3,
        ZERO4, ZERO5, KOREAN, CHINESE
    }
    public class HeaderMaker : CsfHeader
    {
        private readonly string flag;
        private readonly int version;
        private readonly int labelCount;
        private readonly int stringCount;
        private readonly byte[] unknow;
        private readonly CsvLanguage language;

        public override string Flag => flag;
        public override int Version => version;
        public override int LabelCount => labelCount;
        public override int StringCount => stringCount;
        public override byte[] Unknow => unknow;
        public override CsvLanguage Language => language;
        public HeaderMaker(string flag, int version, int labelCount, int stringCount,
            byte[] unknow, CsvLanguage language)
        {
            this.flag = flag;
            this.version = version;
            this.labelCount = labelCount;
            this.stringCount = stringCount;
            this.unknow = unknow;
            this.language = language;
        }
    }
}
