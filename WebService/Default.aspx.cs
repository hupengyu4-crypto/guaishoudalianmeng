using BattleSystem;
using Google.Protobuf;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebService
{
    public partial class _Default : System.Web.UI.Page
    {
        private Stopwatch mStopWatcher;

        private static volatile uint _uuidCurrent = uint.MinValue + 1;

        public static uint GenerateUUID()
        {
            uint id = ++_uuidCurrent;
            if (_uuidCurrent == uint.MaxValue)
            {
                _uuidCurrent = uint.MinValue;
            }

            return id;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mStopWatcher = new Stopwatch();
            mStopWatcher.Start();
            uint uuid = GenerateUUID();
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log/";
            string logErrorPath = AppDomain.CurrentDomain.BaseDirectory + "LogError/";
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);

                }
                if (!Directory.Exists(logErrorPath))
                {
                    Directory.CreateDirectory(logErrorPath);

                }
                BattleWeb.ConfigInitCheck(this);
                string action = Request.QueryString["action"];
                if (!string.IsNullOrEmpty(action))
                {
                    try
                    {
                        switch (action)
                        {
                            case "md5":
                                CheckMd5();
                                break;
                            case "check":
                                CheckCfgAndScriptsMd5();
                                break;
                            case "ReloadConfig":
                                BattleWeb.ReloadConfig(this);
                                break;
                        }

                    }
                    catch (Exception error)
                    {
                        Response.Write(error);
                    }
                    return;
                }
                byte[] battleBytes = Request.BinaryRead(Request.ContentLength);
                if (battleBytes.Length == 0)
                {
                    Response.Write("这里是Web版本信息");
                    return;
                }

                try
                {
                    BattleWeb battleWeb = new BattleWeb(battleBytes);
                    battleWeb.BattleBegin();
                    var data = battleWeb.BattleWebData();
                    data.WebMillisecond = Convert.ToUInt32(mStopWatcher.ElapsedMilliseconds);
                    Response.BinaryWrite(data.ToByteArray());
                    if (BattleDef.DebugLog)
                    {
                        string time = battleWeb.Battle.Uid + ".txt"; //DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.fff") + ".txt";
                        if (File.Exists(logPath + time))
                        {
                            File.Delete(logPath + time);
                        }
                        battleWeb.Battle.SaveInfo(logPath + time);
                    }
                    battleWeb.Clear();
                }
                catch (Exception ex)
                {
                    string timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.fff");
                    string logFile = "Log_" + timeStr + "_" + uuid + ".txt";
                    mStopWatcher.Stop();
                    string logRequest = battleBytes.ToHexString();
                    string time = "\n\nWebMillisecond: " + Convert.ToInt32(mStopWatcher.ElapsedMilliseconds);
                    string pid = "\nWebProcessPID: " + Process.GetCurrentProcess().Id;
                    string unixTime = "\nSystemTime" + (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds * 1000;
                    string str = "ErrorLog:\n\n" + logRequest + "\n\n" + ex + time + pid + unixTime;
                    web_reply web_data = new web_reply();
                    web_data.ErrInfo = "web_fight_error:" + str;
                    Response.BinaryWrite(web_data.ToByteArray());
                    File.AppendAllText(logErrorPath + logFile, str, Encoding.UTF8);
                }
            }
            catch (Exception logError)
            {
                Response.Write(logError);
                string timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.fff");
                string logFile = "Log_" + timeStr + "_" + uuid + ".txt";
                string pid = "\nWebProcessPID: " + Process.GetCurrentProcess().Id;
                string unixTime = "\nSystemTime" + (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds * 1000;
                string str = "ErrorLog:\n\n" + logError + unixTime;
                File.AppendAllText(logErrorPath + logFile, str, Encoding.UTF8);
            }
        }

        private void CheckMd5()
        {
            string[] paths = new string[]
            {
                AppDomain.CurrentDomain.BaseDirectory + "/bin",
                AppDomain.CurrentDomain.BaseDirectory + "/ConfigByteNew",
                AppDomain.CurrentDomain.BaseDirectory + "/Scripts"
            };
            var md5 = BattleUtils.CheckMd5(paths);
            Response.Write(md5);
        }

        /// <summary>
        /// 检查代码和配置MD5
        /// </summary>
        private void CheckCfgAndScriptsMd5()
        {
            string[] paths = new string[]
            {
                AppDomain.CurrentDomain.BaseDirectory + "/ConfigByteNew/Battle/System",
                AppDomain.CurrentDomain.BaseDirectory + "/Scripts"
            };
            string[] ignoreKeys = new string[]
            {
                "Scripts/Core/",
                "Scripts/Log/",
                "Scripts/Register/",
                "BattleWeb.cs",
            };
            var md5 = BattleUtils.CheckMd5(paths, ignoreKeys);
            Response.Write(md5);
        }
    }
}
