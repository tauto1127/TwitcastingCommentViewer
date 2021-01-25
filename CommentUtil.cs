﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace TwitcastingCommentViewer
{
    public class TwicasComment
    {
        public TwicasComment(string comment, string id, int number, int time)
        {
            this.Comment = comment;
            this.Id = id;
            this.Number = number;
            this.Time = time;
        }

        public TwicasComment()
        {
            
        }
        public int Number { get; set; }
        public string Id { get; set; }
        public int Time { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{Number}   時間：{Time}　コメント：{Comment}　ID；{Id}";
        }
    }
    public class CommentUtil
    {

        /// <summary>
        /// コメントそのまま
        /// </summary>
        public static List<string> OldCommentList = new List<string>();
        /// <summary>
        /// 分割したコメント
        /// </summary>
        public ObservableCollection<TwicasComment> TwicasComments { get; set; } = new ObservableCollection<TwicasComment>();

        public CommentUtil(string path)
        {
            OldCommentList.Clear();
            
            CommentLoader(path); 
            //CommentLoader(path);
            CommentSplit();
        }

        /// <summary>
        /// テスト用
        /// </summary>
        /// <param name="testMode"></param>
        /// <param name="path"></param>

        private void CommentSplit()
        {//NotSupportedException
            Regex regex = new Regex(@"\[....\/..\/(?<date>..).(?<oclock>.+)\:(?<minute>..)\:(?<secound>..)\].(?<comment>.+)（(?<id>.+)）");
            var i = 0;
            Match match;



            int Day = 0;

            try
            {
                match = regex.Match(OldCommentList[0]);
                Day = Int32.Parse(match.Groups["date"].ToString());
            }
            catch (NotSupportedException)
            {
                MessageBox.Show("対応してません");
            }

            int time;
            foreach (string variable in OldCommentList)
            {
                
                if (regex.IsMatch(variable))
                {
                    match = regex.Match(variable);
                    time = Int32.Parse(match.Groups["oclock"].ToString()) * 360 + Int32.Parse(match.Groups["minute"].ToString()) * 60 +
                           Int32.Parse(match.Groups["secound"].ToString());
                    if (Day != Int32.Parse(match.Groups["date"].ToString()))
                    {
                        time += 24 * 60 * 60;
                    }
                    TwicasComments.Add(new TwicasComment(match.Groups["comment"].ToString(), match.Groups["id"].ToString(), i, time));
                    i++;
                }
            }
        }

        public static void CommentLoader(string commentFilePath)
        {
            Regex regex = new Regex(@"\[....\/..\/.+\:..\:..\].+（.+）");
            try
            {
                using (StreamReader sr = new StreamReader(commentFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (regex.IsMatch(line))
                        {
                            OldCommentList.Add(line);
                            line = null;
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException();
            }
            if(OldCommentList.Count == 0){throw new NoCommentException();}

        }



    }

    [Serializable()]
    public class NoCommentException : Exception
    {
        public NoCommentException()
        {
            
        }
    }

    
}