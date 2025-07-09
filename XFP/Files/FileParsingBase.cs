using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.Files
{
    internal class FileParsingBase
    {
        /// <summary>
        /// Returns the element name from the specified line.
        /// </summary>
        internal static string ItemName(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                var splitChar = input.Contains("(") ? "(" : input.Contains(":") ? ":" : "=";
                var split = input.Split(splitChar);
                if (split.Length > 0)
                    return split[0].Trim().Trim([ '"', '/' ]);
            }
            return "";
        }


        internal static int LineNumber = 1;


        /// <summary>
        /// Reads the next line from inputStream: returns null if it matches endTag.<br/>
        /// Default behaviour is to remove comment prefix unless removeCommentPrefix is false.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="endTag"></param>
        /// <param name="removeCommentPrefix"></param>
        /// <returns>The next line from inputStream; null if endTag was reached</returns>
        protected static string readNext(StreamReader inputStream, string endTag, bool removeCommentPrefix = true)
        {
            LineNumber++;
            var currentLine = removeDelimiters(inputStream.ReadLine(), removeCommentPrefix);
            return (currentLine != null && !isEndTag(currentLine, endTag)) ? currentLine : null;
        }


        protected static bool isEndTag(string input, string tag) => input == tag;


        /// <summary>
        /// Returns a copy of input shorn of leading/trailing white space.<br/>
        /// Default behaviour is to remove comment prefix unless removeCommentPrefix is false.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="removeCommentPrefix"></param>
        /// <returns></returns>
        protected static string removeDelimiters(string input, bool removeCommentPrefix = true)
        {
            if (input == null) return null;
            var trimmed = input.Trim();
            return removeCommentPrefix ? FileParsingBase.removeCommentPrefix(trimmed) : trimmed;
        }


        protected static string removeCommentPrefix(string input)
            => input?.Substring(0, input.Contains(Tags.Comment) ? input.IndexOf(Tags.Comment) : input.Length);


        internal static string ParseString(string input)
        {
            //divider in legacy file is '=', for the case of a json file check for ':'
            var legacyFile = input.Contains("=");
            var split = input.Split(legacyFile ? "=" : ":");
            if (split.Length > 1)
            {
                var tmp = new StringBuilder();
                foreach (var c in from c in split[1]
                                  where c < 0xff
                                  select c)
                    tmp.Append(c);

                return legacyFile ? tmp.ToString().Trim() : tmp.ToString().Trim().Trim(['\"', ',']);
            }
            return "";
        }


        protected static bool parseBool(string input)
        {
            var split = input.Split("=");
            if (split.Length > 1)
                if (bool.TryParse(split[1].Trim(), out var ret))
                    return ret;
            return false;
        }


        protected static int parseInt(string input)
        {
            int ret = 0;
            //divider in legacy file is '=', for the case of a json file check for ':'
            var split = input.Split(input.Contains("=") ? "=" : ":");
            if (split.Length > 1)
                int.TryParse(split[1].Trim(), out ret);
            return ret;
        }


        protected static char parseChar(string input)
        {
            string tmp = null;
            var split = input.Split("=");
            if (split.Length > 1)
                tmp = split[1].Trim();
            return string.IsNullOrEmpty(tmp) ? ' ' : tmp[0];
        }


        protected static DateTime parseDate(string input)
        {
            var split = input.Split("=");
            if (split.Length > 1)
                if (DateTime.TryParse(split[1].Trim(), out DateTime date))
                    return date;
            return new();
        }


        protected static TimeSpan parseTime(string input)
        {
            var split = input.Split("=");
            if (split.Length > 1)
            {
                if (TimeSpan.TryParse(split[1].Trim(), out TimeSpan time))
                {
                    //return zero if greater than 1 day
                    if (time.Subtract(new(1, 0, 0, 0)) > TimeSpan.Zero)
                        return TimeSpan.Zero;
                    return time;
                }
            }
            return new();
        }


        protected static TimeSpan parseIntTime(string input, int? zeroValue = null)
        {
            var split = input.Split("=");
            if (split.Length > 1)
                if (int.TryParse(split[1].Trim(), out int time))
                {
                    if (zeroValue is not null)
                    {
                        if (time == zeroValue.Value)
                            return TimeOfDay.Midnight;
                    }
                    return new(time / 3600, time / 60 % 60, time % 60);
                }
            return new();
        }


        protected static int parseItemIndex(string input)
        {
            int ret = 0;
            int par1 = input.IndexOf("(");
            int par2 = input.IndexOf(")");
            if (par1 >= 0 && par2 > 1)
                int.TryParse(input.Substring(par1 + 1, par2 - par1 - 1).Trim(), out ret);
            return ret;
        }


        protected static string parseComments(StreamReader inputStream)
        {
            var comm = new StringBuilder();
            foreach (var m in parseMemoText(inputStream))
                comm.Append(m);
            return comm.ToString();
        }


        protected static List<string> parseMemoText(StreamReader inputStream)
        {
            var ret = new List<string>();
            string currentLine;
            //NB: Memo text is prefixed with "//" which we don't want readNext() to treat as a file comment
            while ((currentLine = readNext(inputStream, Tags.EndMemo, false)) != null)
                ret.Add(currentLine.StartsWith(Tags.Comment) ? currentLine[Tags.Comment.Length..].TrimStart() : currentLine);
            return ret;
        }

        protected static CTecDevices.ObjectTypes parseProtocol(string input)
        {
            return ParseString(input) switch
            {
                "1" or Tags.ApolloProtocol => CTecDevices.ObjectTypes.XfpApollo,
                "2" or Tags.CastProtocol   => CTecDevices.ObjectTypes.XfpCast,
                _                          => CTecDevices.ObjectTypes.NotSet,
            };
        }        
    }
}
