using RecipeControl.Commands;
using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using RecipeControl.Services.Scales;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RecipeControl.ViewModels.RegisterModule
{
    public class RegisterModuleViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;
        private readonly ITipoInsumoRepository _tipoInsumoRepository;
        private readonly IRegistroPesoRepository _registroPesoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IInsumoRepository _insumoRepository;
        private readonly IRecetaVersionRepository _recetaVersionRepository;
        private readonly IWeighingService _weighingService;

        public RegisterModuleViewModel(
            IDatabaseService databaseService, 
            ITipoInsumoRepository tipoInsumoRepository,
            IRegistroPesoRepository registroPesoRepository,
            IUsuarioRepository usuarioRepository,
            IInsumoRepository insumoRepository,
            IRecetaVersionRepository recetaVersionRepository,
            IWeighingService weighingService)
        {
            _databaseService = databaseService;
            _tipoInsumoRepository = tipoInsumoRepository;
            _registroPesoRepository = registroPesoRepository;
            _usuarioRepository = usuarioRepository;
            _insumoRepository = insumoRepository;
            _recetaVersionRepository = recetaVersionRepository;
            _weighingService = weighingService;

            // Initialize commands
            CaptureWeightCommand = new AsyncRelayCommand(async _ => await CaptureWeight());
            RegisterWeightCommand = new AsyncRelayCommand(async _ => await RegisterWeight());

            // Subscribe to events

            // Load initial data
            _ = LoadOnStartUp();
        }
        private async Task LoadOnStartUp()
        {
            await LoadUserAsync();
            await LoadRecetaVersionList();
            await LoadTipoInsumoList();
            await LoadInsumoList();
            await LoadWeightRegisters();
        }

        private async Task LoadUserAsync()
        {
            var user = await _usuarioRepository.GetByIdAsync(1002);
            if (user != null)
            {
                UsuarioBalanza = user;
                OnPropertyChanged(nameof(UsuarioBalanza));
            }
        }

        private async Task LoadRecetaVersionList()
        {
            var result = await _recetaVersionRepository.GetAllActiveAsync();
            RecetaVersionList = new ObservableCollection<RecetaVersionDTO>(result);
            _selectedRecetaVersionId = RecetaVersionList.FirstOrDefault()?.RecetaVersionId ?? 1001;
            OnPropertyChanged(nameof(RecetaVersionList));
            OnPropertyChanged(nameof(SelectedRecetaVersionId));
        }

        private async Task LoadTipoInsumoList()
        {
            var result = await _tipoInsumoRepository.GetAllAsync();
            TipoInsumoList = new ObservableCollection<TipoInsumo>(result);
            _selectedTipoInsumoId = TipoInsumoList.FirstOrDefault()?.TipoInsumoId ?? 1001;
            OnPropertyChanged(nameof(TipoInsumoList));
            OnPropertyChanged(nameof(SelectedTipoInsumoId));
        }

        private async Task LoadInsumoList()
        {
            var result = await _insumoRepository.GetInsumoByRecipeAndTypeAsync(_selectedRecetaVersionId, _selectedTipoInsumoId);
            InsumoList = new ObservableCollection<Insumo>(result);
            SelectedInsumoId = InsumoList.FirstOrDefault()?.InsumoId ?? 1001;
            OnPropertyChanged(nameof(InsumoList));
            OnPropertyChanged(nameof(SelectedInsumoId));
        }

        private async Task LoadWeightRegisters()
        {
            var result = await _registroPesoRepository.GetAllDataGridDTOAsync();
            RegistroPesoList = new ObservableCollection<RegisterWeightDataGridDTO>(result);
            OnPropertyChanged(nameof(RegistroPesoList));
        }

        #region Private Properties
        private int _selectedTipoInsumoId;
        private int _selectedInsumoId;
        private int _selectedRecetaVersionId;
        #endregion

        #region Properties

        public Usuario UsuarioBalanza { get; set; } = new Usuario();
        public RegistroPeso RegistroPesoBalanza { get; set; } = new RegistroPeso();
        public ObservableCollection<TipoInsumo> TipoInsumoList { get; set; } = new ObservableCollection<TipoInsumo>();
        public int SelectedTipoInsumoId 
        { 
            get => _selectedTipoInsumoId; 
            set
            {
                if (_selectedTipoInsumoId != value)
                {
                    _selectedTipoInsumoId = value;
                    OnPropertyChanged(nameof(SelectedTipoInsumoId));
                    _ = LoadInsumoList();
                }
            } 
        }
        public ObservableCollection<Insumo> InsumoList { get; set; } = new ObservableCollection<Insumo>();
        public int SelectedInsumoId
        { 
            get => _selectedInsumoId; 
            set
            {
                if (_selectedInsumoId != value)
                {
                    _selectedInsumoId = value;
                    OnPropertyChanged(nameof(SelectedInsumoId));
                }
            } 
        }
        public ObservableCollection<RecetaVersionDTO> RecetaVersionList { get; set; } = new ObservableCollection<RecetaVersionDTO>();
        public int SelectedRecetaVersionId
        {
            get => _selectedRecetaVersionId;
            set
            {
                if (_selectedRecetaVersionId != value)
                {
                    _selectedRecetaVersionId = value;
                    OnPropertyChanged(nameof(SelectedRecetaVersionId));
                    _ = LoadInsumoList();
                }
            }
        }
        public bool IsInRange { get; set; }
        public ObservableCollection<RegisterWeightDataGridDTO> RegistroPesoList { get; set; } = new ObservableCollection<RegisterWeightDataGridDTO>();

        #endregion

        #region Commands
        public ICommand CaptureWeightCommand { get; }
        public ICommand RegisterWeightCommand { get; }
        #endregion

        #region Methods

        private async Task CaptureWeight()
        {
            await Task.Delay(500);
            Debug.WriteLine($"Capturing weight: {RegistroPesoBalanza.Valor} kg");
        }

        private async Task RegisterWeight()
        {
            RegistroPesoBalanza.Descripcion = "";
            RegistroPesoBalanza.RecetaVersionId = 1002;
            RegistroPesoBalanza.InsumoId = SelectedInsumoId;
            RegistroPesoBalanza.BalanzaId = 1001;
            RegistroPesoBalanza.UsuarioId = UsuarioBalanza.UsuarioId;
            RegistroPesoBalanza.FechaPesado = DateTime.Now;

            // Update the RegistroPesoBalanza in the database and get the inserted record with ID
            RegistroPesoBalanza = await _registroPesoRepository.InsertAsync(RegistroPesoBalanza);
            OnPropertyChanged(nameof(RegistroPesoBalanza));
            await LoadWeightRegisters();
        }
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
