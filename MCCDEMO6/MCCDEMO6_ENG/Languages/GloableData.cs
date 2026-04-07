using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace MccCustomLanguage
{
    public class GlobalData
    {
        /// <summary>
        /// 系统语言（Chinese（中文），English（英文）。。。）
        /// </summary>
        /// //System.Configuration.ConfigurationManager.AppSettings
        public static string SystemLanguage = System.Configuration.ConfigurationManager.AppSettings["Language"];
        /*
         * 上面这句话就是告诉我们需要引用System.Configuration.dll就会出现ConfigurationManager类了，就是这么简单。
         * 严重性代码说明项目文件行禁止显示状态
         * 警告	CS0618	“ConfigurationSettings.AppSettings”已过时:“This method is obsolete, it has been replaced by System.Configuration!
         * System.Configuration.ConfigurationManager.AppSettings”	MCCDEMO	F:\mcc6svn\MCCDEMO\Languages\GloableData.cs	16	不适用
         * 
         * 解决：依次单击“引用”->“程序集”->“System.Configuration”，即可
         */

        private static Language globalLanguage;
        public static Language GlobalLanguage
        {
            get
            {
                if (globalLanguage == null)
                {
                    globalLanguage = new Language();
                    return globalLanguage;
                }
                return globalLanguage;
            }
        }
    }
}
