
using Newegg.API.ServiceHost;
using ServiceStack.Text;

namespace Newegg.MIS.API.EggRolls
{
    /// <summary>
    /// 模块初始化
    /// Module initialization
    /// </summary>
    public class AppInit : AppHostBase
    {
        public override void Init()
        {
            JsConfig.ThrowOnDeserializationError = true;
            RegisterValidators(typeof(AppInit).Assembly);
        }
    }
}
