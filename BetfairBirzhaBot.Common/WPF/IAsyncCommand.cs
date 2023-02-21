using System.Threading.Tasks;
using System.Windows.Input;

namespace BetfairBirzhaBot.Base
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

}
