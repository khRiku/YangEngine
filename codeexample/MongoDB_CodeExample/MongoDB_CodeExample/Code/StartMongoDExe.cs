using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 用代码中启动MongoDB的服务器
/// </summary>
public class StartMongoDExe
{
    //我家里电脑的路径
    ////MongoDB 服务器执行文件的位置
    //private static string mMongodPath = @"E:\WorkProgramFiles\MongoDB\MongoDB4.0.10\bin\mongod.exe";   

    ////用于cmd 的指令， 用于通过cmd 开启执行文件
    //private static string mMongodStartCommand = @"E:\WorkProgramFiles\MongoDB\MongoDB4.0.10\bin\mongod.exe --dbpath ""E:\MongoDBCache\data\db""";

    ////设置数据库的位置的指令
    //private static string mDbpathPar = @"--dbpath ""E:\MongoDBCache\data\db""";


    //公司电脑的路径
    //MongoDB 服务器执行文件的位置
    private static string mMongodPath = @"D:\program\MongoDB\Server\4.0.10\bin\mongod.exe";

    //用于cmd 的指令， 用于通过cmd 开启执行文件
    private static string mMongodStartCommand = @"D:\program\MongoDB\Server\4.0.10\bin\mongod.exe --dbpath ""C:\MongoDB\data\db""";

    //设置数据库的位置的指令
    private static string mDbpathPar = @"--dbpath ""C:\MongoDB\data\db""";



    //cmd 的位置
    private static string mCmdPath = @"C:\Windows\System32\cmd.exe";


    /// <summary>
    /// 是否已有Mongod 服务器在运行了
    /// </summary>
    /// <returns></returns>
    public  static bool HasStartMongod()
    {
        foreach (var tProcess in Process.GetProcesses())
        {
            Console.WriteLine(tProcess.ProcessName);

            if (tProcess.ProcessName == "mongod")
            {
                Console.WriteLine("已有Mongod 服务器在运行了");
                return true;
            }

        }

        return false;
    }

    /// <summary>
    /// 通过cmd 来启动 MongoDB 服务器， 这样如果有什么错误导致，MongDB服务器启动不起来
    /// 可以通过cmd 面板查看信息
    /// </summary>
    public static void StartCmdToRunMongod()
    {
        if (HasStartMongod())
            return;

        ProcessStartInfo tProcessStartInfo = new ProcessStartInfo();

        tProcessStartInfo.FileName = mCmdPath;

        //得这样设置， 才能像进程写入命令
        tProcessStartInfo.UseShellExecute = false;
        tProcessStartInfo.RedirectStandardInput = true;


        //  tProcessStartInfo.CreateNoWindow = false;              //是否用一个新窗口来执行该进程      
        //  tProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;   //隐藏窗口，相当于后台执行

        Process tProcess = Process.Start(tProcessStartInfo);

        //向cmd 输入指令
        tProcess.StandardInput.WriteLine(mMongodStartCommand);
        tProcess.StandardInput.Flush();

        Console.ReadLine();
    }



    /// <summary>
    /// 直接启动MongoDB 服务器， 但如果有错误会导致启动不了， 又没报错信息，
    ///  可切换到StartCmdToRunMogod函数， 这样会出现报错信息，方便处理
    /// </summary>
    public static void StartMongoDB()
    {
        if (HasStartMongod())
            return;

        ProcessStartInfo tProcessStartInfo = new ProcessStartInfo();
        tProcessStartInfo.FileName = mMongodPath;
        tProcessStartInfo.Arguments = mDbpathPar;

        //  tProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;   //隐藏窗口， 相当于后台运行

        Process.Start(tProcessStartInfo);

        Console.ReadLine();
    }
}

