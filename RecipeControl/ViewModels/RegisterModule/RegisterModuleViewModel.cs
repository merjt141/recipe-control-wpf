using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public RegisterModuleViewModel(IDatabaseService databaseService, ITipoInsumoRepository tipoInsumoRepository)
        {
            _databaseService = databaseService;
            _tipoInsumoRepository = tipoInsumoRepository;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
