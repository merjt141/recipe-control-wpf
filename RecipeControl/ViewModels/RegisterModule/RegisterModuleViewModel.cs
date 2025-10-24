using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipeControl.ViewModels.RegisterModule
{
    public class RegisterModuleViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;
        private readonly ITipoInsumoRepository _tipoInsumoRepository;

        public RegisterModuleViewModel(
            IDatabaseService databaseService, 
            ITipoInsumoRepository tipoInsumoRepository)
        {
            _databaseService = databaseService;
            _tipoInsumoRepository = tipoInsumoRepository;

            // Initialize commands

            // Subscribe to events

            // Load initial data
            _ = LoadTipoInsumosAsync();
        }

        private async Task LoadTipoInsumosAsync()
        {
            var result = await _tipoInsumoRepository.GetAllAsync();

            TipoInsumoList = new ObservableCollection<TipoInsumo>(result);
            SelectedTipoInsumoId = TipoInsumoList.FirstOrDefault()?.TipoInsumoId ?? 1001;

            OnPropertyChanged(nameof(TipoInsumoList));
            OnPropertyChanged(nameof(SelectedTipoInsumoId));
        }

        #region Properties

        public string UserName { get; set; } = "DefaultUser";
        public ObservableCollection<TipoInsumo> TipoInsumoList { get; set; } = new ObservableCollection<TipoInsumo>();
        public int SelectedTipoInsumoId { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
