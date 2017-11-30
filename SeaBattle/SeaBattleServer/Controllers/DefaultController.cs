using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using SeaBattleFramework;
using SeaBattleServer.Models;

// Здесь происходит отладка SeaBattle.dll

namespace SeaBattleServer.Controllers
{
    [RoutePrefix("Default")]
    public class DefaultController : ApiController
    {
        [Route("NewGame")]
        [HttpPost]
        public string NewGame(string serializedMsg)
        {
            /*try
            {
                var ctx = GameCtxProvider.Instance;

                return ctx.CreateNewSession().ToString();
            }
            catch (GameException exc)
            {
                return "Error text: " + exc.ErrorText + " Error code: " + (int)exc.ErrorCode;
            }*/

            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.Username, "Test"},
                {(byte) ClientParameterCode.Password, "test"},
                {(byte) ClientParameterCode.SubOperationCode, 0}
            };

            Message msg = new Message
            {
                Parameters = para
            };

            return JsonConvert.SerializeObject(msg);

            //"{\"Code\":0,\"Parameters\":{},\"SubCode\":5,\"DebugMessage\":\"No ERRORS\",\"ReturnCode\":3}"
            /*Message msg = new Message
            {
                Code = 0,
                DebugMessage = "No ERRORS",
                Parameters = new Dictionary<byte, object>(),
                ReturnCode = 3,
                SubCode = 5
            };*/
        }
    }
}
