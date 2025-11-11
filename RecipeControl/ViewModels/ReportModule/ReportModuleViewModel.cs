using RecipeControl.Commands;
using RecipeControl.Models.DTOs;
using RecipeControl.Models.Entities;
using RecipeControl.Repositories;
using RecipeControl.Repositories.Interfaces;
using RecipeControl.Services.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecipeControl.ViewModels.ReportModule
{
    public class ReportModuleViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;
        private readonly IRegistroBatchWarehouseRepository _registroBatchWarehouseRepository;
        private readonly IInsumoRepository _insumoRepository;
        private readonly IRecetaVersionRepository _recetaVersionRepository;

        public ReportModuleViewModel(
            IDatabaseService databaseService,
            IRegistroBatchWarehouseRepository registroBatchWarehouseRepository,
            IInsumoRepository insumoRepository,
            IRecetaVersionRepository recetaVersionRepository)
        {
            _databaseService = databaseService;
            _registroBatchWarehouseRepository = registroBatchWarehouseRepository;
            _insumoRepository = insumoRepository;
            _recetaVersionRepository = recetaVersionRepository;

            // Initialize commands
            ReadBatchsCommand = new AsyncRelayCommand(async _ => await ReadBatchs());
            GenerateReportCommand = new AsyncRelayCommand(async _ => await GenerateReport());

            // Load initial data
            _ = LoadOnStartUp();
        }

        private async Task LoadOnStartUp()
        {
            await LoadRecetaList();
            await LoadInsumoList();
        }

        private async Task LoadInsumoList()
        {
            var result = await _insumoRepository.GetAllAsync();

            var insumoList = new List<Insumo>
            {
                new Insumo
                {
                    InsumoId = 1000,
                    Codigo = "- TODOS -",
                    Descripcion = "Todos los insumos",
                }
            };

            insumoList.AddRange(result);

            InsumoList = new ObservableCollection<Insumo>(insumoList);
            SelectedInsumoId = InsumoList.FirstOrDefault()?.InsumoId ?? 1000;
            OnPropertyChanged(nameof(InsumoList));
            OnPropertyChanged(nameof(SelectedInsumoId));
        }

        private async Task LoadRecetaList()
        {
            var result = await _recetaVersionRepository.GetAllActiveAsync();

            var recetaList = new List<RecetaVersionDTO>
            {
                new RecetaVersionDTO
                {
                    RecetaVersionId = 1000,
                    RecetaId = 1000,
                    RecetaCodigo = " - TODOS -",
                }
            };

            recetaList.AddRange(result);

            RecetaVersionList = new ObservableCollection<RecetaVersionDTO>(result);
            _selectedRecetaVersionId = RecetaVersionList.FirstOrDefault()?.RecetaVersionId ?? 1000;
            OnPropertyChanged(nameof(RecetaVersionList));
            OnPropertyChanged(nameof(SelectedRecetaVersionId));
        }

        #region Private Properties
        private int _selectedRecetaVersionId;
        private int _selectedInsumoId;
        private DateTime _fechaInicial = DateTime.Now.AddDays(-7);
        private DateTime _fechaFinal = DateTime.Now;
        #endregion

        #region Properties
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
        public DateTime FechaInicial
        {
            get => _fechaInicial;
            set
            {
                if (_fechaInicial != value)
                {
                    _fechaInicial = value;
                    Debug.WriteLine(value.ToString("yyyy-MM-dd HH:mm:ss"));
                    OnPropertyChanged(nameof(FechaInicial));
                }
            }
        }
        public DateTime FechaFinal
        {
            get => _fechaFinal;
            set
            {
                if (_fechaFinal != value)
                {
                    _fechaFinal = value;
                    Debug.WriteLine(value.ToString("yyyy-MM-dd HH:mm:ss"));
                    OnPropertyChanged(nameof(FechaFinal));
                }
            }
        }
        public ObservableCollection<RegistroBatchDTO> RegistroBatchList { get; set; } = new ObservableCollection<RegistroBatchDTO>();
        #endregion

        #region Commands
        public ICommand ReadBatchsCommand { get; }
        public ICommand GenerateReportCommand { get; }
        #endregion

        #region Methods

        private async Task ReadBatchs()
        {
            // Implementation for reading batchs from the database
            var result = await _registroBatchWarehouseRepository.GetAllDTOsAsync();

            // Process batchs as needed
            RegistroBatchList = new ObservableCollection<RegistroBatchDTO>(result);
            OnPropertyChanged(nameof(RegistroBatchList));
        }

        private async Task GenerateReport()
        {
            // Implementation for generating report
            await Task.Delay(500); // Simulate report generation delay
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
