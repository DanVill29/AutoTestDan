using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoTestDan
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public static class LoadFile
    {
        public static string FilePAth;
        private static string LocalDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string Separator = "[===]";
        public static string StartConfig = "QA";


        public static List<T> LoadFile2<T>(string XLS_Path)
        {
            List<T> SetupList = new List<T>();
            T SetupObject = (T)Activator.CreateInstance(typeof(T));
            FieldInfo fieldInfo;
            string[] keyValue;
            string _Data;
            bool inSection = false;
            int listcount = 0;

            XLS_Path = @LocalDir + @"Files" + @"\" + Path.GetFileName(XLS_Path);

            if (!File.Exists(XLS_Path))
            {
                Assert.Fail("Config file can't find! (" + XLS_Path + ")");
            }
            try
            {
                string[] lines = File.ReadAllLines(XLS_Path);


                for (int i = 0; i < lines.Length; i++)
                {
                    _Data = (lines[i] ?? "").ToString();

                    if (_Data.NotEmpty() && _Data.IndexOf(Separator) > -1)
                    {
                        keyValue = _Data.Split(new string[] { Separator }, StringSplitOptions.None);
                        if (keyValue[0] == "environment")
                        {
                            StartConfig = keyValue[1];
                        }

                        if (inSection)
                        {
                            fieldInfo = SetupObject.GetType().GetField(keyValue[0]);
                            fieldInfo.SetValue(SetupObject, keyValue[1]);
                        }

                    }

                    if (_Data == StartConfig)
                    {
                        //Create new Instance
                        SetupObject = (T)Activator.CreateInstance(typeof(T));
                        fieldInfo = SetupObject.GetType().GetField("environment");
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(SetupObject, StartConfig);
                        }
                        inSection = true;
                        listcount++;

                    }


                    if (inSection && _Data.Trim().Empty())
                    {

                        SetupList.Add(SetupObject);
                        inSection = false;


                    }

                }




            }
            catch
            { }
            if (listcount != SetupList.Count)
            {
                SetupList.Add(SetupObject);
            }
            return SetupList;
        }



        public static bool Empty(this string s)
        {
            return System.String.IsNullOrEmpty(s);
        }

        public static bool NotEmpty(this string s)
        {
            return !System.String.IsNullOrEmpty(s);
        }
    }
}
