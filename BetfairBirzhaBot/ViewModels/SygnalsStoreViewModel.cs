using BetfairBirzhaBot.Base;
using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Core.Managers;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Services.Interfaces;
using BetfairBirzhaBot.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetfairBirzhaBot.ViewModels
{
    public class SygnalsStoreViewModel : BaseViewModel
    {
        private SygnalUploadManager _uploadManager;
        private ExcelSygnalUploadManager _excelUploadManager;
        public IAsyncCommand UploadSygnalsCommand { get; set; }
        public IAsyncCommand InitializeViewModelCommand { get; set; }
        public string Title { get; set; }

        private ISettingsService _service;
        private SessionSettings _settings;
        private string _statusMessage { get; set; }
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public int SygnalsInStoreCount { get => _settings.SygnalStore.Count; }
        public SygnalsStoreViewModel(ISettingsService service)
        {
            Title = "Отображение всех сигналов";
            UploadSygnalsCommand = new AsyncCommand(async () => await Upload());
            InitializeViewModelCommand = new AsyncCommand(async () => await UpdateInfo());
            _uploadManager = new SygnalUploadManager();
            _excelUploadManager = new ExcelSygnalUploadManager();
            _service = service;
            _settings = service.Get();
        }

        private async Task UpdateInfo()
        {

        }

        private async Task Upload()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                StatusMessage = "Началась выгрузка в Excel.";
                await Task.Delay(3 * 1000);
                var excelWriterTask = Task.Factory.StartNew(async () => await _excelUploadManager.Upload(new List<StrategySygnalResult>(_settings.SygnalStore), path));
                
                await excelWriterTask;
                _settings.SygnalStore.Clear();
                _service.Save();


                StatusMessage = "Выгрузка в Excel закончилась.";
                OnPropertyChanged(nameof(SygnalsInStoreCount));
            }
        }
    }
}
