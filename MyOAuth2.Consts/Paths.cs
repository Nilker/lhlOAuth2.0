/************************************************************************
 * 项目名称 :  MyOAuth.Consts  
 * 项目描述 :     
 * 类 名 称 :  Paths
 * 版 本 号 :  v1.0.0.0 
 * 说    明 :     
 * 作    者 :  lhl
 * 创建时间 :  2015/12/8 9:45:12
 * 更新时间 :  2015/12/8 9:45:12
************************************************************************
 * Copyright @ YST 2015. All rights reserved.
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOAuth2.Consts
{
    public static class Paths
    {
        public const string AppId = "123456";

        public const string AppSecret = "abcdef";

        public const string AuthorizeCodeCallBackPath = "http://lhl.client.com/Account/ExternalLoginCallback";

        public const string AuthorizationServerBaseAddress = "http://lhl.service.com";

        public const string ResourceValuesApiPath = "http://lhl.Resource.com/api/Home/GetValues";

        public const string LoginPath = "/Account/Login";
        public const string LogoutPath = "/Account/Logout";

        public const string AuthorizePath = "/Authorize";
        public const string TokenPath = "/Token";

        //public const string AppId = "123456";

        //public const string AppSecret = "abcdef";

        //public const string AuthorizeCodeCallBackPath = "http://localhost:33333/Account/ExternalLoginCallback";

        //public const string AuthorizationServerBaseAddress = "http://localhost:55555";

        //public const string ResourceValuesApiPath = "http://localhost:44444/api/Home/GetValues";

        //public const string LoginPath = "/Account/Login";
        //public const string LogoutPath = "/Account/Logout";

        //public const string AuthorizePath = "/Authorize";
        //public const string TokenPath = "/Token";
    }
}
