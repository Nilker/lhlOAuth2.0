using System.IO;
using System.Net.Http.Headers;
using MyOAuth2.Consts;
using Newtonsoft.Json;

namespace Owin.Security.Lhl
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Microsoft.Owin;
    using Microsoft.Owin.Infrastructure;
    using Microsoft.Owin.Logging;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Infrastructure;

    internal class LHLAuthenticationHandler : AuthenticationHandler<LHLAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public LHLAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            this._httpClient = httpClient;
            this._logger = logger;
        }


        /// <summary>
        /// �ڳ�ʼ��֮���ɹ����������һ�Ρ���������֤�м��ֱ����Ӧ��֪��ר��·�����������д������·����������·��������֪·�����бȽϣ��ṩ�ʵ�����Ӧ��Ϣ����ֹͣ��һ������
        /// </summary>
        /// <returns>
        /// ������ false���򹫹����뽫��˳�������һ���м���������� true���򹫹����뽫��ʼ�첽��ɹ��̣����������м���ܵ������ಿ�֡�
        /// </returns>
        public override async Task<bool> InvokeAsync()
        {
            bool flag;
            //if (!string.IsNullOrEmpty(this.Options.CallbackPath) && this.Options.CallbackPath == this.Request.Path.ToString())
            //    flag = await this.InvokeReturnPathAsync();
            if (!string.IsNullOrEmpty(this.Options.CallbackPath) && this.Options.CallbackPath == this.Request.Scheme + "://" + Request.Uri.Host + Request.Uri.LocalPath)
            {
                flag = await this.InvokeReturnPathAsync();
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        private async Task<bool> InvokeReturnPathAsync()
        {
            AuthenticationTicket model = await this.AuthenticateAsync();
            bool flag;
            if (model == null)
            {
                this.Response.StatusCode = 500;
                flag = true;
            }
            else
            {
                LHLReturnEndpointContext context = new LHLReturnEndpointContext(this.Context, model);
                context.SignInAsAuthenticationType = this.Options.SignInAsAuthenticationType;
                context.RedirectUri = model.Properties.RedirectUri;
                model.Properties.RedirectUri = null;

                await this.Options.Provider.ReturnEndpoint(context);
                if (context.SignInAsAuthenticationType != null && context.Identity != null)
                {
                    ClaimsIdentity claimsIdentity = context.Identity;
                    if (!string.Equals(claimsIdentity.AuthenticationType, context.SignInAsAuthenticationType,StringComparison.Ordinal))
                    {
                        claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, context.SignInAsAuthenticationType, claimsIdentity.NameClaimType, claimsIdentity.RoleClaimType);
                    }
                    this.Context.Authentication.SignIn(context.Properties, new ClaimsIdentity[1]
                                                                               {
                                                                                   claimsIdentity
                                                                               });
                }
                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    if (context.Identity == null)
                    {
                        context.RedirectUri = WebUtilities.AddQueryString(context.RedirectUri, "error", "access_denied");
                    }
                    this.Response.Redirect(context.RedirectUri);
                    context.RequestCompleted();
                }
                flag = context.IsRequestCompleted;
            }


            return flag;
        }



        private static string GetStateParameter(IReadableStringCollection query)
        {
            IList<string> values = query.GetValues("state");
            if (values != null && values.Count == 1)
                return values[0];
            else
                return null;
        }
        private AuthenticationProperties UnpackStateParameter(IReadableStringCollection query)
        {
            string stateParameter = GetStateParameter(query);
            if (stateParameter != null)
                return this.Options.StateDataFormat.Unprotect(stateParameter);
            else
                return null;
        }

        /// <summary>
        /// �����ɴ�������ṩ�ĺ��������֤�߼������ÿ������������һ�Ρ���Ҫֱ�ӵ��ã���Ӧ���ð�װ Authenticate ������
        /// </summary>
        /// 
        /// <returns>
        /// �������֤�߼��ṩ��Ʊ֤����
        /// </returns>
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;
            AuthenticationTicket authenticationTicket;

            IReadableStringCollection query = this.Request.Query;
            properties = this.UnpackStateParameter(query);
            string code = string.Empty;
            IList<string> values = query.GetValues("code");
            if (values != null && values.Count == 1)
                code = values[0];
            if (string.IsNullOrEmpty(code))
            {
                authenticationTicket = new AuthenticationTicket(null, properties);
                return authenticationTicket;
            }

            if (properties == null)
            {
                authenticationTicket = null;
            }
            else if (!this.ValidateCorrelationId(properties, this._logger))
            {
                authenticationTicket = new AuthenticationTicket(null, properties);
            }
            else
            {
                //Get���������� �޷���֤��ȡappId..*************************************
                string tokenEndpoint = "http://localhost:55555/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}";
                var url = string.Format(
                    tokenEndpoint,
                    Uri.EscapeDataString(this.Options.AppID),
                    Uri.EscapeDataString(this.Options.AppKey),
                    Uri.EscapeDataString(code), Uri.EscapeDataString("http://" + this.Request.Host));
                //HttpResponseMessage tokenResponse = await this._httpClient.GetAsync(url, this.Request.CallCancelled);
                //Get���������� �޷���֤��ȡappId..


                var _serviceClient = new HttpClient();
                _serviceClient.BaseAddress = new Uri(Paths.AuthorizationServerBaseAddress);

                var parameters = new Dictionary<string, string>();
                parameters.Add("client_id", this.Options.AppID);
                parameters.Add("client_secret", this.Options.AppKey);
                parameters.Add("grant_type", "authorization_code");
                parameters.Add("code", (code));
                parameters.Add("redirect_uri", Paths.AuthorizeCodeCallBackPath);

                HttpResponseMessage tokenResponsePost = await _serviceClient.PostAsync(Paths.AuthorizationServerBaseAddress + Paths.TokenPath, new FormUrlEncodedContent(parameters));


                tokenResponsePost.EnsureSuccessStatusCode();
                string access_tokenReturnValue = await tokenResponsePost.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject(access_tokenReturnValue);
                var accessToken = ((dynamic)obj).access_token.ToString();

                #region ������ʽ
                //var pa = @"access_token=(.+?)\&";
                //var pa = "\"access_token\":\"(.+?)\"";
                //string accesstoken = Regex.Match(access_tokenReturnValue, pa).Groups[1].Value;

                //var bbbb = accesstoken == accessToken; 
                #endregion

                var resourceClient = new HttpClient();

                resourceClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                resourceClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                HttpResponseMessage resourceResponse = await resourceClient.GetAsync(Paths.ResourceValuesApiPath, this.Request.CallCancelled);


                var resourceData = resourceResponse.Content.ReadAsStringAsync().Result;
                var resObj = JsonConvert.DeserializeObject(resourceData);
                var userName = ((dynamic)resObj).userName.ToString();
                var userId = ((dynamic)resObj).userId.ToString();


                //var meurltemp = "http://localhost:55555/me?access_token={0}";
                //var meurl = string.Format(meurltemp, Uri.EscapeDataString(accesstoken));
                //var meResponse = await this._httpClient.GetAsync(meurl, this.Request.CallCancelled);
                //meResponse.EnsureSuccessStatusCode();
                //var me = await meResponse.Content.ReadAsStringAsync();

                ////�����ȡ��open id����ͨ�� open id��ȡ���ǳ�
                //var paopenid = "\"openid\":\"(.+?)\"";
                //var openid = Regex.Match(me, paopenid).Groups[1].Value;
                //var nameurltemp = "http://localhost:55555/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}";
                //var nameurl = string.Format(
                //    nameurltemp,
                //    Uri.EscapeDataString(accesstoken),
                //    Uri.EscapeDataString(this.Options.AppID),
                //    Uri.EscapeDataString(openid));
                //var nameResponse = await this._httpClient.GetAsync(nameurl, this.Request.CallCancelled);
                //nameResponse.EnsureSuccessStatusCode();
                //var nametxt = await nameResponse.Content.ReadAsStringAsync();
                //var paname = "\"nickname\": \"(.+?)\"";
                //var name = Regex.Match(nametxt, paname).Groups[1].Value;

                var context = new LHLAuthenticatedContext(this.Context, accessToken, userId, userName);

                var identity = new ClaimsIdentity(this.Options.AuthenticationType);

                if (!string.IsNullOrEmpty(context.OpenId))
                {
                    identity.AddClaim(
                        new Claim(
                            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                            context.OpenId,
                            "http://www.w3.org/2001/XMLSchema#string",
                            this.Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Name))
                {
                    identity.AddClaim(
                        new Claim(
                            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                            context.Name,
                            "http://www.w3.org/2001/XMLSchema#string",
                            this.Options.AuthenticationType));
                }
                await this.Options.Provider.Authenticated(context);
                authenticationTicket = new AuthenticationTicket(identity, properties);
            }

            return authenticationTicket;
        }

        /// <summary>
        /// �����������֤�����������֤������Ϊ����������һ���ִ�������д�˷����Դ��� 401 ��ѯ���⡣�����������Ӧ��ͷ���� 401 �������Ϊ��¼ҳ�� 302 ���ⲿ��¼λ�á���
        /// </summary>
        protected override Task ApplyResponseChallengeAsync()
        {
            if (this.Response.StatusCode != 401)
            {
                return Task.FromResult((object)null);
            }
            AuthenticationResponseChallenge responseChallenge = this.Helper.LookupChallenge(this.Options.AuthenticationType, this.Options.AuthenticationMode);
            //�����ض������֤�м������Ӧ��ѯ��ϸ��Ϣ
            if (responseChallenge != null)
            {
                string stringToEscape = this.Request.Scheme + Uri.SchemeDelimiter + this.Request.Host;
                AuthenticationProperties properties = responseChallenge.Properties;
                //redirectUri
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri =
                        string.Concat(
                            new object[]
                                {
                                    stringToEscape, this.Request.PathBase, this.Request.Path,this.Request.QueryString
                                });
                }
                //�������Id
                this.GenerateCorrelationId(properties);

                var loginRedirectUrlFormat = Paths.AuthorizationServerBaseAddress + Paths.AuthorizePath + "?client_id={0}&response_type=code&redirect_uri={1}";
                //�������ݣ�ʹ�䲻�ܱ���ʽ����
                var protector = this.Options.StateDataFormat.Protect(properties);

                var url = string.Format(loginRedirectUrlFormat,
                    Uri.EscapeDataString(this.Options.AppID),
                    (BuildReturnTo(protector)));

                this.Response.StatusCode = 302;
                this.Response.Headers.Set("Location", url);
            }
            return base.ApplyResponseChallengeAsync();
        }

        private string BuildReturnTo(string state)
        {
            return Paths.AuthorizeCodeCallBackPath + "&state=" + Uri.EscapeDataString(state);
        }
    }
}