using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wechat.Api.Beans.Friends;
using Wechat.Protocol;
using WeChat.Api.Errors;
using static MMPro.MM;

namespace WeChat.Api.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly WechatHelper _wechat;

        public FriendsController(WechatHelper wechat)
        {
          this._wechat = wechat;
        }
        /// <summary>
        ///  初始化好友列表
        /// </summary>
        /// <param name="wxid">已登录的wxid</param>
        [HttpPost("init_contract_list/{wxid}")]
        public Error<InitContactResp> InitContract([FromRoute,Required]string wxid,[FromBody] InitContactReq req)
        {
            var result = _wechat.InitUser(wxid, req.SyncKey, req.Buffer);
            if (result != null){
                return Error<InitContactResp>.New()
                .WithCode(ErrorCode.OK).
                WithData(new InitContactResp()
                {
                    InitResponse = result.Item1,
                    Buffer = result.Item2,
                    SyncKey = (int)result.Item3
                });
            }
            return Error<InitContactResp>.New().WithCode(ErrorCode.ErrInterServcerErr).WithMessage("初始化好友列表失败");
        }
        /// <summary>
        ///  获取联系人列表
        /// </summary>
        /// <param name="wxid">已登录的wxid</param>
        [HttpPost("contract_list/{wxid}")]
        public Error<ListContactResp> ListContract([FromRoute, Required]string wxid, [FromBody] ListContactReq req)
        {
            var result = _wechat.InitContact(wxid, req.CurrentWxcontactSeq, req.CurrentChatRoomContactSeq);
            if (result != null && result.baseResponse.ret == (int)MMPro.MM.RetConst.MM_OK)
            {
                return Error<ListContactResp>.New()
                      .WithCode(ErrorCode.OK).
                      WithData(new ListContactResp()
                      {
                          Contacts = result.contactUsernameList,
                          CurrentWxcontactSeq = result.currentWxcontactSeq,
                          CurrentChatRoomContactSeq = result.currentChatRoomContactSeq
                      });
            }
            return Error<ListContactResp>.New().WithCode(ErrorCode.ErrInterServcerErr).WithMessage("获取好友列表失败");
        }
        /// <summary>
        /// 获取自己简介信息
        /// </summary>
        /// <param name="wxid">已登录的wxid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("self_profiles/{wxid}")]
        public Error<SelfProfileResp> GetSelfProfile([FromRoute,Required]string wxid)
        {
            var result = _wechat.GetContractProfile(wxid, wxid);
            if (result!=null && result.baseResponse.ret == (int)RetConst.MM_OK)
            {
                return Error<SelfProfileResp>.New()
                    .WithCode(ErrorCode.OK)
                    .WithData(new SelfProfileResp() {
                        UserInfo=result.userInfo,
                        UserInfoExt=result.userInfoExt,
                    });
            }
            return Error<SelfProfileResp>.New()
                .WithCode(ErrorCode.ErrInterServcerErr)
                .WithMessage("获取自己简介信息失败");
        }
        /// <summary>
        /// 批量获取好友简介
        /// </summary>
        /// <param name="wxid">已登录的wxid</param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("contact_profiles/{wxid}")]
        public Error<FetchContactProfilesResp> BatchGetContractProfiles([FromRoute, Required]string wxid, [FromBody]FetchContactProfilesReq req)
        {
            var result = _wechat.BatchGetContractProfile(wxid, req.ContactWxIds, 0);
            if (result != null)
            {
                //var profiles = new List<ContactProfile>();
                //foreach(var item in result)
                //{
                //    profiles.Add(new ContactProfile()
                //    {
                //        WxId=item.Alias,
                //        Alias= item.Alias,
                //        NickName =item.NickName,
                //    });
                //}
                return Error<FetchContactProfilesResp>.New()
                   .WithCode(ErrorCode.OK).
                   WithData(new FetchContactProfilesResp()
                   {
                       Profiles= result,
                   });
            }
            return Error<FetchContactProfilesResp>.New()
                .WithCode(ErrorCode.ErrInterServcerErr)
                .WithMessage("获取批量获取好友简介失败");
        }
        ///// <summary>
        /////  搜索微信用户
        ///// </summary>
        ///// <param name="wxid">已登录的wxid</param>
        ///// <param name="searchKey">手机号，微信号，QQ号</param>
        //[HttpPost("search")]
        //public void Search(string wxid,string searchKey)
        //{


        //}
        ///// <summary>
        ///// 发送添加好友请求
        ///// </summary>
        //[HttpPost("send_request")]
        //public void SendRequest()
        //{


        //}
    }
}