
using System;

namespace BetfairBirzhaBot.Base
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

}
