using Newtonsoft.Json.Linq;
using System;
using System.Collections;

namespace CxjText.utils
{
    public class Config
    {

        
        //软件使用人  admin为管理员   不用登录
        //包含admin的可以使用别的网站但是要登录
        //不包含admin  只能使用配置网址
        public static String softUserStr = "";
        public static long softTime = -1; //软件使用时间记录
        public static String urls = "www.mkbet6.com" + "\t" + "www.328365.com" + "\t" + "www.3122tt.com" + "\t" + "www.3122ss.com" + "\t" + "www.20266ll.com" + "\t" + "www.7tgj.com" + "\t" + "www.vn80.com" + "\t" + "www.7893657.com" + "\t" + "www.88ac.net" + "\t" + "www.9889p.com" + "\t" + "www.5554j.com" + "\t" + "www.k96688.com" + "\t" + "www.xpj86778.com" + "\t" + "www.js89699.com" + "\t" + "www.bet7088.com" + "\t" + "www.js889z.com" + "\t" + "www.91789l.com" + "\t" + "www.vs8866.com" + "\t" + "www.4661f.com" + "\t" + "www.4600aa.com" + "\t" + "www.9v555.com" + "\t" + "www.98229999.com" + "\t" + "www.v1050.com" + "\t" + "www.71277i.com" + "\t" + "www.r79077.com" + "\t" + "www.1366f.com" + "\t" + "www.4212c.com" + "\t" + "www.108818.com" + "\t" + "www.jhg10.com" + "\t" + "www.x222.am" + "\t" + "www.hg8828.la" + "\t" + "www.wh006.com" + "\t" + "www.ra8988.com" + "\t" + "www.c92776.com" + "\t" + "www.6547799.com" + "\t" + "www.029722.com" + "\t" + "www.0037444.com" + "\t" + "www.yf700.com" + "\t" + "www.219365.com" + "\t" + "www.6686sky.tv" + "\t" + "www.xpjvip8.cc" + "\t" + "www.xjs775.com" + "\t" + "www.js1158.com" + "\t" + "www.733555p.com" + "\t" + "www.3569y.com" + "\t" + "www.888365t.com" + "\t" + "www.2767v.com" + "\t" + "www.32456v.com" + "\t" + "www.36138vip.com" + "\t" + "www.38080v.com" + "\t" + "www.50633v.com" + "\t" + "www.5087v.com" + "\t" + "www.43335v.com" + "\t" + "www.63777v.com" + "\t" + "www.78118vip.com" + "\t" + "www.9870v.com" + "\t" + "www.bet9360.com" + "\t" + "www.hg67809.com" + "\t" + "www.hg7578.com" + "\t" + "www.hg7688.com" + "\t" + "www.hg9688.com" + "\t" + "www.hg9689.com" + "\t" + "www.pj83336.com" + "\t" + "www.pj88863.com" + "\t" + "www.v15388.com" + "\t" + "www.v55998.com" + "\t" + "www.v90567.com" + "\t" + "www.yl5918.com" + "\t" + "www.6686sky.net" + "\t" + "www.6686sky.com" + "\t" + "www.6690bet.com" + "\t" + "www.hg56188.com" + "\t" + "www.hg87723.com" + "\t" + "www.vn947.com" + "\t" + "www.vn5488.com" + "\t" + "www.vn488.com" + "\t" + "www.vn135.com" + "\t" + "www.vip8323.com" + "\t" + "www.18018v.com" + "\t" + "www.2767v.com" + "\t" + "www.30688v.com" + "\t" + "www.30929v.com" + "\t" + "www.3122v.com" + "\t" + "www.32456v.com" + "\t" + "www.36138vip.com" + "\t" + "www.38080v.com" + "\t" + "www.43335v.com" + "\t" + "www.50633v.com" + "\t" + "www.5087v.com" + "\t" + "www.63777v.com" + "\t" + "www.26yh.com" + "\t" + "www.6hf.com" + "\t" + "www.78118vip.com" + "\t" + "www.8520v.com" + "\t" + "www.9870v.com" + "\t" + "www.98995v.com" + "\t" + "www.bet9360.com" + "\t" + "www.hg67809.com" + "\t" + "www.hg700.com" + "\t" + "www.hg7578.com" + "\t" + "www.hg7688.com" + "\t" + "www.hg8982.com" + "\t" + "www.hg9688.com" + "\t" + "www.hg9689.com" + "\t" + "www.js19669.com" + "\t" + "www.pj83336.com" + "\t" + "www.pj88863.com" + "\t" + "www.v1464.com" + "\t" + "www.v15388.com" + "\t" + "www.v55998.com" + "\t" + "www.v73370.com" + "\t" + "www.v77790.com" + "\t" + "www.v8791.com" + "\t" + "www.v90567.com" + "\t" + "www.v992.com" + "\t" + "www.v9922.com" + "\t" + "www.vip63599.com" + "\t" + "www.wns38138.com" + "\t" + "www.wns42167.com" + "\t" + "www.wns809.com" + "\t" + "www.xpj20188.com" + "\t" + "www.xpj25810.com" + "\t" + "www.xpj77774.com" + "\t" + "www.xpj95679.com" + "\t" + "www.yl5918.com" + "\t" + "www.yl87888.com" + "\t" + "www.yl5544.org" + "\t" + "www.js766766.com" + "\t" + "www.pj88771.com" + "\t" + "www.xpj9601.com" + "\t" + "www.xpj18899.com " + "\t" + "www.v36864.com " + "\t" + "www.hg5183.com" + "\t" + "www.hg22775.com" + "\t" + "www.xpj15678.com" + "\t" + "www.112233xpj.com" + "\t" + "www.hg8336.com" + "\t" + "www.hg60088.com" + "\t" + "www.hg5883.com" + "\t" + "www.hg55575.com" + "\t" + "www.hg45788.com" + "\t" + "www.hg330.com" + "\t" + "www.hg3122.com" + "\t" + "www.hg2578.com" + "\t" + "www.hg23809.com" + "\t" + "www.hg234.com" + "\t" + "www.hg22799.com" + "\t" + "www.hg2233.com" + "\t" + "www.hg11814.com" + "\t" + "www.hga5616.com" + "\t" + "www.hg10678.com" + "\t" + "www.2hg4080.com" + "\t" + "www.a6995.com" + "\t" + "www.392988y.com" + "\t" + "www.32333.com" + "\t" + "www.js2058.com" + "\t" + "www.206558.com" + "\t" + "www.xpj8182.cc" + "\t" + "www.hg00003.vip" + "\t" + "www.bwin17.com" + "\t" + "www.5035b.com" + "\t" + "www.hggj003.com" + "\t" + "www.wynn.gg" + "\t" + "www.1360444.com" + "\t" + "www.xsj8999.com" + "\t" + "www.350833.com" + "\t" + "www.hg480.com" + "\t" + "www.vip99567.com" + "\t" + "www.fh005.com" + "\t" + "www.707679.com" + "\t" + "www.fspfsa.com" + "\t" + "www.68068.vip" + "\t" + "www.vns660.com" + "\t" + "www.ws5555.com" + "\t" + "www.lj558.com" + "\t" + "www.548701.com" + "\t" + "www.96996mm.com" + "\t" + "www.cr208.com" + "\t" + "www.31998a.com" + "\t" + "www.98506.com" + "\t" + "www.8866958.com" + "\t" + "www.yl9609.com" + "\t" + "www.660050.com" + "\t" + "www.z998k.us" + "\t" + "www.js67744.com" + "\t" + "www.js379s.com" + "\t" + "www.77527e.com" + "\t" + "www.4288ss.com" + "\t" + "www.yl2333.com" + "\t" + "www.yl8356.com" + "\t" + "www.yl3888.com" + "\t" + "www.36068.com" + "\t" + "www.76345.com" + "\t" + "www.mgm388w.com" + "\t" + "www.58588lv.com" + "\t" + "www.45678908.com" + "\t" + "www.m5158.com" + "\t" + "www.pj580.com" + "\t" + "www.7933311.com" + "\t" + "www.js67744.com" + "\t" + "www.0302444.com" + "\t" + "www.lv66588.com" + "\t" + "www.wh006.com" + "\t" + "www.88510.com" + "\t" + "www.665945.com" + "\t" + "www.79333c.com" + "\t" + "www.w34348.com" + "\t" + "www.60879.com" + "\t" + "www.889900333.com" + "\t" + "www.js9686.com" + "\t" + "www.70626.com" + "\t" + "www.34224.com" + "\t" + "www.gg006.com" + "\t" + "www.22566.com" + "\t" + "www.6613kk.com" + "\t" + "www.23026888.com" + "\t" + "www.wns9838.com" + "\t" + "www.70882.com" + "\t" + "www.yf157.com" + "\t" + "www.mgm8409.com" + "\t" + "www.1122xpj.com" + "\t" + "www.8996876.com" + "\t" + "www.778501.com" + "\t" + "www.92776.com" + "\t" + "www.yl5789.com" + "\t" + "www.js19993.com" + "\t" + "www.8lldlrirn22h10.545030.com" + "\t" + "www.xingcaipingtai.cc" + "\t" + "www.vn537.com" + "\t" + "www.22566.com" + "\t" + "www.v8803.com" + "\t" + "www.mgm8626.com" + "\t" + "www.kk2233.com" + "\t" + "www.j58b.com" + "\t" + "www.d93345.com" + "\t" + "www.735078.com" + "\t" + "www.589264.com" + "\t" + "www.0078011.com" + "\t" + "www.bc871.com" + "\t" + "www.9844558.com" + "\t" + "www.93955a.com" + "\t" + "www.899819.com" + "\t" + "www.818669.com" + "\t" + "www.341626.com" + "\t" + "www.24248g.com" + "\t" + "www.172.me" + "\t" + "www.0888xpj.com" + "\t" + "www.js9686.com" + "\t" + "www.lv66588.com" + "\t" + "www.665945.com" + "\t" + "www.79333c.com" + "\t" + "www.js18808.com" + "\t" + "www.707306.com" + "\t" + "www.76345.com" + "\t" + "www.189458.com" + "\t" + "www.w1.cc" + "\t" + "www.665995.com" + "\t" + "www.js85858.com" + "\t" + "www.093198.com" + "\t" + "www.059723.com" + "\t" + "www.v4421.com" + "\t" + "www.53710.cc" + "\t" + "www.mgm8022.com" + "\t" + "www.x106.com" + "\t" + "www.mgm26688.com" + "\t" + "www.hao6767.com" + "\t" + "www.189458.com" + "\t" + "www.323hu.com" + "\t" + "www.dafa7898.com" + "\t" + "www.986365.com" + "\t" + "www.1146.cc" + "\t" + "www.fg456.com" + "\t" + "www.ra7666.com" + "\t" + "www.x721.com" + "\t" + "www.ys111.cc" + "\t" + "www.98995v.com" + "\t" + "www.kk2233.com" + "\t" + "www.037932.com" + "\t" + "www.986365.com" + "\t" + "www.026126.com" + "\t" + "www.yf165.com" + "\t" + "";
        public static bool canOrder = true; //是否可以下单  正式版本是true

        public static String webSocketUrl = "ws://47.88.168.99:8600/";
        public static String netUrl = "http://47.88.168.99:8500";

        //public static String webSocketUrl = "ws://172.28.2.61:8600/";
        //public static String netUrl = "http://172.28.2.61:8500";

        //打码平台的账户和密码
        public static String codeUserStr = "";
        public static String codePwdStr = "";
        public static String codeMoneyStr = "";


        //打码平台的开发者配置
        public static int codeAppId = 4129; //appId
        public static String codeSerect = "d04c0a85b2b739491d2fd2d95ebeae26";
        public static String userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3269.3 Safari/537.36";
        //用户配置登录数据结构
        public static int pramsNum = 4; //参数个数
        public static ArrayList userList = null; //用户登陆信息记录
        public static String vString = "V2.0";


        public static bool isDeug = false;  //控制打印
                  
        public static void console(String str) {
            if(isDeug) Console.WriteLine(str);
        }



        public static JObject speakJObject = new JObject();   

    }
}
