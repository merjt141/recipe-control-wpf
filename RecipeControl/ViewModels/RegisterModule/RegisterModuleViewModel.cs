using RecipeControl.Commands;
using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Credential;
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
        private readonly IRecetaVersionDetalleRepository _recetaVersiondDetalleRepository;

        // Weighing service to interact with the scale hardware
        private readonly IWeighingService _weighingService;

        // Credential service to manage user authentication
        private readonly ICredentialService _credentialService;

        public RegisterModuleViewModel(
            IDatabaseService databaseService, 
            ITipoInsumoRepository tipoInsumoRepository,
            IRegistroPesoRepository registroPesoRepository,
            IUsuarioRepository usuarioRepository,
            IInsumoRepository insumoRepository,
            IRecetaVersionRepository recetaVersionRepository,
            IWeighingService weighingService,
            ICredentialService credentialService,
            IRecetaVersionDetalleRepository recetaVersionDetalleRepository)
        {
            _databaseService = databaseService;
            _tipoInsumoRepository = tipoInsumoRepository;
            _registroPesoRepository = registroPesoRepository;
            _usuarioRepository = usuarioRepository;
            _insumoRepository = insumoRepository;
            _recetaVersionRepository = recetaVersionRepository;
            _weighingService = weighingService;
            _credentialService = credentialService;
            _recetaVersiondDetalleRepository = recetaVersionDetalleRepository;

            // Initialize commands
            StopScalesCaptureCommand = new AsyncRelayCommand(async _ => await StopScalesCapture());
            RegisterWeightCommand = new AsyncRelayCommand(async _ => await RegisterWeight());

            // Subscribe to events

            // Load initial data
            _ = LoadOnStartUp();
        }

        /// <summary>
        /// Initialize all connections and lists on loading the application
        /// </summary>
        /// <returns></returns>
        private async Task LoadOnStartUp()
        {
            // Log operator user into the system
            await _credentialService.LoginAsync("Operador", "d3d9446802a44259755d38e6d163e820");
            OnPropertyChanged(nameof(UsuarioBalanza));

            // Prepare tasks for parallel execution
            List<Task> startupTasks = new List<Task>();

            // Start the weighing service
            startupTasks.Add(Task.Run(async () =>
            {
                await StartScalesCapture();
            }));

            // Load initial data lists
            startupTasks.Add(Task.Run(async () =>
            {
                await LoadRecetaVersionList();
                await LoadTipoInsumoList();
                await LoadInsumoList();
            }));

            // Load existing weight registers
            startupTasks.Add(Task.Run(async () =>
            {
                await LoadWeightRegisters();
            }));

            // Await all startup tasks to complete
            await Task.WhenAll(startupTasks);
        }

        /// <summary>
        /// Load the list of active recipe versions
        /// </summary>
        /// <returns></returns>
        private async Task LoadRecetaVersionList()
        {
            var result = await _recetaVersionRepository.GetAllActiveAsync();
            RecetaVersionList = new ObservableCollection<RecetaVersionDTO>(result);
            _selectedRecetaVersionId = RecetaVersionList.FirstOrDefault()?.RecetaVersionId ?? 1001;
            OnPropertyChanged(nameof(RecetaVersionList));
            OnPropertyChanged(nameof(SelectedRecetaVersionId));
        }

        /// <summary>
        /// Load the list of input types
        /// </summary>
        /// <returns></returns>
        private async Task LoadTipoInsumoList()
        {
            var result = await _tipoInsumoRepository.GetAllAsync();
            TipoInsumoList = new ObservableCollection<TipoInsumo>(result);
            _selectedTipoInsumoId = TipoInsumoList.FirstOrDefault()?.TipoInsumoId ?? 1001;
            OnPropertyChanged(nameof(TipoInsumoList));
            OnPropertyChanged(nameof(SelectedTipoInsumoId));
        }

        /// <summary>
        /// Load the list of inputs based on selected recipe version and input type
        /// </summary>
        /// <returns></returns>
        private async Task LoadInsumoList()
        {
            var result = await _insumoRepository.GetInsumoByRecetaAndTipoAsync(_selectedRecetaVersionId, _selectedTipoInsumoId);
            InsumoList = new ObservableCollection<Insumo>(result);
            SelectedInsumoId = InsumoList.FirstOrDefault()?.InsumoId ?? 1001;
            OnPropertyChanged(nameof(InsumoList));
            OnPropertyChanged(nameof(SelectedInsumoId));
        }

        private async Task LoadRecipeVersionDetail()
        {
            var result = await _recetaVersiondDetalleRepository.GetRecetaVersionDetalleByRecetaAndInsumoAsync(_selectedRecetaVersionId, _selectedInsumoId);
            RecetaVersionDetalleBalanza = result;
            OnPropertyChanged(nameof(RecetaVersionDetalleBalanza));
        }

        /// <summary>
        /// Load existing weight registers
        /// </summary>
        /// <returns></returns>
        private async Task LoadWeightRegisters()
        {
            var result = await _registroPesoRepository.GetAllDataGridDTOAsync();
            RegistroPesoList = new ObservableCollection<RegistroPesoDTO>(result);
            OnPropertyChanged(nameof(RegistroPesoList));
        }

        #region Private Properties
        private int _selectedTipoInsumoId;
        private int _selectedInsumoId;
        private int _selectedRecetaVersionId;

        private CancellationTokenSource _cancellationCaptureWeight;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user associated with the balance system.
        /// </summary>
        public Usuario UsuarioBalanza
        {
            get => _credentialService.GetCurrentUser();
        }

        /// <summary>
        /// Selected recipe version detail for the balance operation.
        /// </summary>
        public RecetaVersionDetalle RecetaVersionDetalleBalanza { get; set; } = new RecetaVersionDetalle();

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
        public ObservableCollection<RegistroPesoDTO> RegistroPesoList { get; set; } = new ObservableCollection<RegistroPesoDTO>();

        #endregion

        #region Commands
        public ICommand StopScalesCaptureCommand { get; }
        public ICommand RegisterWeightCommand { get; }
        #endregion

        #region Methods

        /// <summary>
        /// Start cyclic weight capture from the scales
        /// </summary>
        /// <returns></returns>
        private async Task StartScalesCapture()
        {
            _cancellationCaptureWeight = new CancellationTokenSource();
            await _weighingService.StartService();

            // Async task that runs cyclically
            _ = CaptureWeightCyclicallyAsync(_cancellationCaptureWeight);
        }

        /// <summary>
        /// Stop cyclic weight caputure from the scales
        /// </summary>
        /// <returns></returns>
        private async Task StopScalesCapture()
        {
            _cancellationCaptureWeight.Cancel();
            await _weighingService.StopService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task CaptureWeightCyclicallyAsync(CancellationTokenSource token)
        {
            // Scale index --- (just for test) ---
            int scaleIndex = 0;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Print scale info --- (just for test) ---
                    string ip = _weighingService.GetScaleInfo(scaleIndex);
                    Debug.WriteLine($"Scale Info: {ip}");

                    // Retrieve weight value from scale
                    RegistroPesoBalanza.CantidadPesada = await _weighingService.GetScaleWeightAsync(scaleIndex);
                    OnPropertyChanged(nameof(RegistroPesoBalanza));

                    // Print scale value --- (just for test) ---
                    Debug.WriteLine($"Capturing weight: {RegistroPesoBalanza.CantidadPesada} kg at {DateTime.Now}");

                    await Task.Delay(500);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Weight capture operation was canceled.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error capturing weight: {ex.Message}");
            }
        }

        private async Task RegisterWeight()
        {
            RegistroPesoBalanza.RecetaVersionId = _selectedRecetaVersionId;
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
