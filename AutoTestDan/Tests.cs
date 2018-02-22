using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace AutoTestDan
{
    [TestFixture]
    public class Tests
    {
        private IWebDriver wa = null;
        private string localhosturl = "http://localhost:4447/dashboard/";
        public static string LocalDir = AppDomain.CurrentDomain.BaseDirectory; //to get the test directory
        public static List<Config> setupcases = null;
        //public static string LocalDir = TestContext.CurrentContext.TestDirectory + @"\"; //to get the test directory
        private string SeleniumBatchPath = @LocalDir + @"Files";
        string targetDir = "";
        Process proc2 = null;


        [SetUp]
        public void Start()
        {
            Console.WriteLine("Start...");

            try
            {
                KillBrowserDrivers();
                KillSeleniumHub(); //kill hub and nodes

            }
            catch
            {

            }

        }

        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("Teardown...");

            if(wa!=null)
            {
                wa.Quit();
            }

            try
            {
                KillBrowserDrivers();
                KillSeleniumHub(); //kill hub and nodes

            }
            catch
            {

            }
        }

        [Test]
        public void Test1()
        {
            /*
             1.)Open a browser (will use direct web driver)
             2.)Maximize window and Go to a url
             3.)Find one element in the home page
             4.)Get link elements top bar and click each then go back to home page
             4.)Close the browser
             */
            Console.WriteLine("Test1...");
            wa = new ChromeDriver();
            //wa = new FirefoxDriver();
            //wa = new InternetExplorerDriver();
            //wa = new EdgeDriver();

            wa.Manage().Window.Maximize();
            wa.Navigate().GoToUrl(localhosturl);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            bool find = false;
            while (stopwatch.ElapsedMilliseconds < 5 * 1000)
            {
                try
                {
                    if (wa.FindElement(By.XPath("//h1/a[contains(.,'Apache')]")).Displayed)
                    {
                        find = true;
                    }
                }
                catch
                {

                }
                
                if(find) { break; }     

            }
            stopwatch.Stop();
            if (!find)
            { Assert.Fail("Can't find Apache element"); }


            ICollection<IWebElement> topbarlinks = wa.FindElements(By.XPath("//section[@class='top-bar-section']/ul/li/a"));

            String mainWindow = wa.CurrentWindowHandle;
            for (int i=0;i<topbarlinks.Count;i++)
            {
                
                foreach (String handle in wa.WindowHandles)
                {
                    if (handle != wa.WindowHandles.ToArray()[0])
                    {
                        wa.SwitchTo().Window(handle);
                        wa.Close();
                        wa.SwitchTo().Window(mainWindow);

                    }

                    
                }
                

                topbarlinks = wa.FindElements(By.XPath("//section[@class='top-bar-section']/ul/li/a"));
                
               
                
                try
                {
                    topbarlinks.ElementAt(i).Click();
                    wa.Navigate().Back();
                   
                }
                catch
                {
             
                }
                
            }
       


            try
            {
                wa.Quit();
                wa = null;
            }
            catch
            {

            }
            
           


        }

        [Test]
        public void Test2_thread()
        {
            List<Thread> threads;
            List<IWebDriver> drivers;
            Stopwatch stopwatch = new Stopwatch();
            threads = new List<Thread>();
            drivers = new List<IWebDriver>();
            int ThreadError = 0;

            stopwatch.Restart();
            for(int i=0;i<2;i++)
            {
                threads.Add(new Thread(delegate ()
                {
                    try { 
                    drivers.Add(new ChromeDriver());
                    }
                    catch
                    {
                        ThreadError++;
                    }
                }));
            }
            foreach(Thread thread in threads)
            {
                thread.Start();

            }
            foreach (Thread thread in threads)
            {
                thread.Join();

            }
            stopwatch.Stop();



            //b
            threads = new List<Thread>();
            foreach (IWebDriver wa in drivers)
            {
                threads.Add(new Thread(delegate ()
                {
                    try
                    {
                        wa.Manage().Window.Maximize();
                        wa.Navigate().GoToUrl(localhosturl);

                        Stopwatch stopwatch1 = new Stopwatch();
                        stopwatch1.Restart();
                        bool find = false;
                        while (stopwatch1.ElapsedMilliseconds < 5 * 1000)
                        {
                            try
                            {
                                if (wa.FindElement(By.XPath("//h1/a[contains(.,'Apache')]")).Displayed)
                                {
                                    find = true;
                                }
                            }
                            catch
                            {

                            }

                            if (find) { break; }

                        }
                        stopwatch1.Stop();
                        if (!find)
                        { Assert.Fail("Can't find Apache element"); }


                        ICollection<IWebElement> topbarlinks = wa.FindElements(By.XPath("//section[@class='top-bar-section']/ul/li/a"));

                        for (int i = 0; i < topbarlinks.Count; i++)
                        {
                            topbarlinks = wa.FindElements(By.XPath("//section[@class='top-bar-section']/ul/li/a"));



                            try
                            {
                                topbarlinks.ElementAt(i).Click();
                                wa.Navigate().Back();

                            }
                            catch
                            {

                            }

                        }



                        try
                        {
                            wa.Quit();
                            //wa = null;
                        }
                        catch
                        {

                        }

                    }
                    catch
                    {
                        ThreadError++;
                    }
                }
                ));
            }
            foreach (Thread thread in threads)
            {
                thread.Start();

            }
            foreach (Thread thread in threads)
            {
                thread.Join();

            }

        }

        [Test,TestCase("chrome","http://www.google.com"), TestCase("firefox", "http://www.google.com")]
        public void Test3_testcasedata(string browser, string url)
        {
            switch(browser)
            {
                case "chrome" :
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = url;
            wa.Quit();
        }

        [Test,TestCaseSource("getData")]
        public void Test4_testcasesource_testcasedata(string browser)
        {
            switch (browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = localhosturl;
            wa.Quit();

        }

        [Test, TestCaseSource("getData2")]
        public void Test5_testcasesource_testcasedata2(string browser, string browser2)
        {
            switch (browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;

                case "ie":
                    wa = new InternetExplorerDriver();
                    break;

                case "edge":
                    wa = new EdgeDriver();
                    break;
            }

            wa.Url = localhosturl;
            wa.Quit();

            switch (browser2)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;

                case "ie":
                    wa = new InternetExplorerDriver();
                    break;

                case "edge":
                    wa = new EdgeDriver();
                    break;
            }

            wa.Url = localhosturl;
            wa.Quit();

        }


        [Test, TestCaseSource("getDataconfig")]
        public void Test6_testcasesource_testcasedata3_config(string browser)
        {
            switch (browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = localhosturl;
            wa.Quit();

        }


        [Test, TestCaseSource("getDataconfig2")]
        public void Test7_testcasesource_testcasedata3_config(string browser)
        {
            switch (browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = localhosturl;
            wa.Quit();

        }

        [Test, TestCaseSource("getDataconfig_config")]
        public void Test8_testcasesource_testcasedata3_config(Config setup)
        {
            switch (setup.browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = setup.url;
            wa.Quit();

        }

        [Test, TestCaseSource("getDataconfig_config2")]
        public void Test9_testcasesource_testcasedata3_config(Config setup)
        {
            switch (setup.browser)
            {
                case "chrome":
                    wa = new ChromeDriver();
                    break;

                case "firefox":
                    wa = new FirefoxDriver();
                    break;
            }

            wa.Url = setup.url;
            wa.Quit();

        }


        [Test, TestCaseSource("getDataconfig_config2")]
        public void Test9b_threadSetup(Config setup)
        {
            List<Thread> threads;
            List<IWebDriver> drivers;
            Stopwatch stopwatch = new Stopwatch();
            threads = new List<Thread>();
            drivers = new List<IWebDriver>();
            int ThreadError = 0;

            //switch (setup.browser)
            //{
            //    case "chrome":
            //        wa = new ChromeDriver();
            //        break;

            //    case "firefox":
            //        wa = new FirefoxDriver();
            //        break;

            //}

            stopwatch.Restart();
            for (int i = 0; i < 2; i++)
            {
                threads.Add(new Thread(delegate ()
                {
                    try
                    {
                        switch (setup.browser)
                        {
                            case "chrome":
                                drivers.Add(new ChromeDriver());
                                break;

                            case "firefox":
                                drivers.Add(new FirefoxDriver());
                                break;

                        }
                        //drivers.Add(new ChromeDriver());

                    }
                    catch
                    {
                        ThreadError++;
                    }
                }));
            }
            foreach (Thread thread in threads)
            {
                thread.Start();

            }
            foreach (Thread thread in threads)
            {
                thread.Join();

            }
            stopwatch.Stop();



            //b
            threads = new List<Thread>();
            foreach (IWebDriver wa in drivers)
            {
                threads.Add(new Thread(delegate ()
                {
                    try
                    {
                        var wa_loc = wa;
                        var setup_loc = setup;
                        wa_loc.Manage().Window.Maximize();
                        wa_loc.Navigate().GoToUrl(setup_loc.url);

                        try
                        {
                            wa_loc.Quit();
                            //wa = null;
                        }
                        catch
                        {

                        }

                    }
                    catch
                    {
                        ThreadError++;
                    }
                }
                ));
            }
            foreach (Thread thread in threads)
            {
                thread.Start();

            }
            foreach (Thread thread in threads)
            {
                thread.Join();

            }

        }

        [Test]
        public void Test9c_LogandScreenShot()
        {
            Add("I'm here!");
            wa = new ChromeDriver();
            wa.Url = localhosturl;
            wa.Manage().Window.Maximize();
            CreateScreenShot("hi");
            wa.Quit();

        }

        [Test]
        public void Test9d_OpenAllBrowsers_RemoteWebdriver_AutomaticRunBatchFiles()
        {
            //KillBrowserDrivers();

            Console.WriteLine("Test...");
            Console.WriteLine("Test_1_OpenBrowser_DirectWebdriver...");

            string url = localhosturl;
            var uri = new Uri("http://localhost:4444/wd/hub");

            StartSeleniumHub();

            StartBrowserNode("chrome");
            ChromeOptions t1 = new ChromeOptions();
            wa = new RemoteWebDriver(uri, t1.ToCapabilities());
            wa.Url = url;
            wa.Manage().Window.Maximize();
            wa.Quit();


            KillBrowserDrivers();
            KillSeleniumHub();
        }


        [Test]
        public void Test9e_GetPerson_MySQL()
        {
            List<string[]> accounts = null;
            accounts = GetPersonsList();

            if (accounts == null)
            {
                Assert.Fail("No accounts Found on database");
            }
            else
            {
                Console.WriteLine("FullName: " + accounts.ToArray()[0][1]);
                Add("FullName: " + accounts.ToArray()[0][1]);
            }
        }

        [Test]
        public void Test9e_2_UpdatePersonList()
        {
            Random random = new Random();
            string sql = "update persons set FullName='Roldan Villaber_" + random.Next(97, 123) + "' where person_id=1 ";

            try
            {
                RunSQL(sql);
            }
            catch (System.Exception ex) { Assert.Fail(ex.Message); }

        }

        [Test]
        public void Test9e_3_AddPerson()
        {
            Random random = new Random();
            string sql = "INSERT INTO persons VALUES('','Jamie Riett_" + random.Next(97, 123) + "')";

            try
            {
                RunSQL(sql);
            }
            catch (System.Exception ex) { Assert.Fail(ex.Message); }

        }

        [Test]
        public void Test9e_4_AddandDeletePerson()
        {
            Random random = new Random();
            string name = "Jamie Riett_" + random.Next(97, 123);
            string sql = "INSERT INTO persons VALUES('','" + name + "')";

            try
            {
                RunSQL(sql);

                sql = "DELETE FROM persons WHERE FullName ='" + name + "'";
                RunSQL(sql);
            }

            catch (System.Exception ex) { Assert.Fail(ex.Message); }

        }




        public void Add(string tolog = "")
        {
            string dateFormat = "MM/dd/yyyy HH:mm:ss";
            string LogDir = TestContext.CurrentContext.TestDirectory + @"\" + @"Logs" + @"\" + TestContext.CurrentContext.Test.FullName;
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
            tolog = DateTime.Now.ToString(dateFormat) + ":->" + tolog;
            File.AppendAllText(LogDir + @"\" + "Log.txt", tolog + "\r\n");
        }

        public void CreateScreenShot(string tolog = "")
        {
            Screenshot Screen;
            string curr_time = DateTime.Now.ToString("MM_dd_yyyy(HH-mm-ss)");
            string LogDir = TestContext.CurrentContext.TestDirectory + @"\" + @"Logs" + @"\" + TestContext.CurrentContext.Test.FullName;
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
            try
            {
                Screen = ((ITakesScreenshot)wa).GetScreenshot();
                Screen.SaveAsFile(LogDir + @"\" + tolog + "_" + curr_time + ".jpg", ScreenshotImageFormat.Jpeg);


            }
            catch (Exception ex)
            {
                Assert.Fail("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }

        public void StartSeleniumHub()
        {
            if (Directory.Exists(SeleniumBatchPath))
            {
                try
                {
                    targetDir = string.Format(SeleniumBatchPath);   //this is where mybatch.bat lies
                    proc2 = new Process();
                    proc2.StartInfo.WorkingDirectory = targetDir;
                    proc2.StartInfo.FileName = "start_selenium_server.bat";
                    proc2.StartInfo.Arguments = string.Format("10");  //this is argument
                    proc2.StartInfo.CreateNoWindow = false;
                    proc2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  //this is for hiding the cmd window...so execution will happen in back ground.

                    try
                    {
                        proc2.Start();
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                    }
                    Thread.Sleep(4000);

                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                }
            }
            else
            {
                Assert.Fail("Can't find SeleniumBatchPath!");
            }
        }

        public void StartBrowserNode(string browser = "chrome")
        {
            string browserbatchfilename = "";
            switch (browser)
            {
                case "chrome":
                    browserbatchfilename = "node_chrome.bat";
                    break;

                case "firefox":
                    browserbatchfilename = "node_firefox.bat";
                    break;

                case "edge":
                    browserbatchfilename = "node_edge.bat";
                    break;

                case "ie":
                    browserbatchfilename = "node_ie.bat";
                    break;

            }

            if (Directory.Exists(SeleniumBatchPath))
            {
                try
                {
                    targetDir = string.Format(SeleniumBatchPath);   //this is where mybatch.bat lies
                    proc2 = new Process();
                    proc2.StartInfo.WorkingDirectory = targetDir;
                    proc2.StartInfo.FileName = browserbatchfilename;
                    proc2.StartInfo.Arguments = string.Format("10");  //this is argument
                    proc2.StartInfo.CreateNoWindow = false;
                    proc2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  //this is for hiding the cmd window...so execution will happen in back ground.

                    try
                    {
                        proc2.Start();
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                    }
                    Thread.Sleep(4000);

                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                }
            }
            else
            {
                Assert.Fail("Can't find SeleniumBatchPath!");
            }

        }

        public void KillSeleniumHub()
        {
            Process[] process = Process.GetProcesses();
            foreach (Process proc in process)
            {
                if (proc.ProcessName.ToLower() == "java")
                {
                    Console.WriteLine(proc.ProcessName.ToLower());
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }

            }
        }

        public void KillBrowserDrivers()
        {
            Process[] process2 = Process.GetProcesses();
            foreach (Process proc in process2)
            {
                if (proc.ProcessName.ToLower() == "iedriverserver")
                {
                    Console.WriteLine(proc.ProcessName.ToLower());
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }

            }

            Process[] process3 = Process.GetProcesses();
            foreach (Process proc in process3)
            {
                if (proc.ProcessName.ToLower() == "microsoftwebdriver")
                {
                    Console.WriteLine(proc.ProcessName.ToLower());
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }

            }

            Process[] process4 = Process.GetProcesses();
            foreach (Process proc in process4)
            {
                if (proc.ProcessName.ToLower() == "chromedriver")
                {
                    Console.WriteLine(proc.ProcessName.ToLower());
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }
            }

            Process[] process5 = Process.GetProcesses();
            foreach (Process proc in process5)
            {
                if (proc.ProcessName.ToLower() == "geckodriver")
                {
                    Console.WriteLine(proc.ProcessName.ToLower());
                    proc.Kill();
                    proc.Close();
                    proc.Dispose();
                }
            }
        }

        #region testcasesource
        public static List<string> getData()
        {
            List<string> datas = new List<string>() { "chrome", "firefox" };
 
            return datas;


        }

        public static IEnumerable<TestCaseData> getData2()
        {

            yield return new TestCaseData("chrome", "firefox");
            yield return new TestCaseData("edge", "ie");


        }

        public static IEnumerable<TestCaseData> getDataconfig()
        {
            string path = @LocalDir + @"Files" + @"\" + Path.GetFileName("browsers.txt");

            if(!File.Exists(path))
            {
                Assert.Fail("Config file can't find! (" + "browsers.txt" + ")");
            }
            
            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                yield return new TestCaseData(line);
            }


        }

        public static List<string> getDataconfig2()
        {
            List<string> datas = new List<string>();

            string path = @LocalDir + @"Files" + @"\" + Path.GetFileName("browsers.txt");

            if (!File.Exists(path))
            {
                Assert.Fail("Config file can't find! (" + "browsers.txt" + ")");
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                datas.Add(line);
            }

            return datas;

        }

        public static List<Config> getDataconfig_config()
        {
            setupcases = new List<Config> { new Config(), new Config(), new Config() };
            return setupcases;
        }

        public static List<Config> getDataconfig_config2()
        {
            setupcases = LoadFile.LoadFile2<Config>("basic.txt");
            if (setupcases == null || setupcases.Count == 0)
            {
                setupcases = new List<Config> { new Config() };
            }

            return setupcases;

        }
        #endregion

        #region db
        public static volatile bool LogStatements = true;
        public static string LastDBError = "";
        private static int CommandTimeoutSeconds = 120;
        //MYSQL
        public static DataTable GetSQL(string sqlCommand, CommandType commandType = CommandType.Text,
            Action<MySqlParameterCollection> addParameters = null, string ConnectionString = "")
        {
            //if (LogStatements) Add("DB GetSQL:=>" + sqlCommand);
            ConnectionString = ConfigurationManager.ConnectionStrings["qa_DB"].ConnectionString;

            DataTable dt = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sqlCommand, connection))
                {
                    command.CommandTimeout = CommandTimeoutSeconds;
                    command.CommandType = commandType;
                    if (addParameters != null) addParameters(command.Parameters);
                    try
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            //dt = new DataTable();
                            //dt.BeginLoadData();
                            adapter.Fill(dt);
                            //dt.EndLoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        //LastDBError = "GetSQL failed: " + ex.Message; //throw new Exception(msg, ex);
                        //Log.Add("GetSQL failed: " + ex.Message, Log.Level.Error);
                    }
                }
            }
            return dt;
        }


        //MSSQL
        public static DataTable GetSQL_MSQL(string sqlCommand, CommandType commandType = CommandType.Text,
        Action<SqlParameterCollection> addParameters = null, string ConnectionString = "")
        {
            //if (LogStatements) Log.Add("DB GetSQL:=>" + sqlCommand);
            ConnectionString = ConfigurationManager.ConnectionStrings["qa_DB"].ConnectionString;

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sqlCommand, connection))
                {
                    command.CommandTimeout = CommandTimeoutSeconds;
                    command.CommandType = commandType;
                    if (addParameters != null) addParameters(command.Parameters);
                    try
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            //dt = new DataTable();
                            //dt.BeginLoadData();
                            adapter.Fill(dt);
                            //dt.EndLoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        //LastDBError = "GetSQL failed: " + ex.Message; //throw new Exception(msg, ex);
                        //Log.Add("GetSQL failed: " + ex.Message, Log.Level.Error);
                    }
                }
            }
            return dt;
        }


        public static bool RunSQL(string sqlCommand, CommandType commandType = CommandType.Text,
        Action<MySqlParameterCollection> addParameters = null, string ConnectionString = "")
        {
            //if (LogStatements) Log.Add("DB RunSQL:=>" + sqlCommand);
            ConnectionString = ConfigurationManager.ConnectionStrings["qa_DB"].ConnectionString;
            bool ret = false;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sqlCommand, connection))
                {
                    command.CommandTimeout = CommandTimeoutSeconds;
                    command.CommandType = commandType;
                    if (addParameters != null) addParameters(command.Parameters);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        ret = true;
                    }
                    catch (Exception ex)
                    {
                        //LastDBError = "RunSQL failed: " + ex.Message; //throw new Exception(msg, ex);
                        //if (LogError) Console.WriteLine("DB ERRROR: " + ex.Message);
                        //Log.Add("RunSQL failed: " + ex.Message, Log.Level.Error);
                        throw new Exception("RunSQL failed: " + ex.Message, ex);
                    }
                }
            }
            return ret;
        }



        public static System.Collections.Generic.List<string[]> GetPersonsList()
        {
            string sql = "";
            sql = "SELECT * FROM `persons` WHERE 1";

            DataTable dt = GetSQL(sql);
            if (dt.Rows.Count > 0)
            {
                System.Collections.Generic.List<string[]> mydatas = new System.Collections.Generic.List<string[]>();
                foreach (System.Data.DataRow r in dt.Rows)
                {
                    System.Collections.Generic.List<string> myval = new System.Collections.Generic.List<string>();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        myval.Add(r[i].ToString());
                    }

                    mydatas.Add(myval.ToArray());
                }
                return mydatas;
            }
            else
            {
                return null;
            }
        }


        #endregion


    }
}
