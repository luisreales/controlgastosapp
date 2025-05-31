using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using ControlGastosApp.Web.Models;

namespace ControlGastosApp.Web.Services
{
    public class JsonDataService
    {
        private readonly string _jsonFilePath;
        private JsonData _data;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService(IWebHostEnvironment environment)
        {
            _jsonFilePath = Path.Combine(environment.ContentRootPath, "Data", "data.json");
            _data = new JsonData();
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            LoadData();
        }

        private void CargarPropiedadesNavegacion()
        {
            // Cargar propiedades de navegación para los gastos
            foreach (var gasto in _data.Gastos)
            {
                // Cargar Fondo
                gasto.Fondo = _data.Fondos.FirstOrDefault(f => f.Id == gasto.FondoId);

                // Cargar TipoGasto para cada detalle
                foreach (var detalle in gasto.Detalles)
                {
                    detalle.TipoGasto = _data.TiposGasto.FirstOrDefault(t => t.Id == detalle.TipoGastoId);
                }
            }

            // Cargar propiedades de navegación para los depósitos
            foreach (var deposito in _data.Depositos)
            {
                deposito.FondoMonetario = _data.Fondos.FirstOrDefault(f => f.Id == deposito.FondoId);
            }
        }

        private void LoadData()
        {
            if (File.Exists(_jsonFilePath))
            {
                var jsonString = File.ReadAllText(_jsonFilePath);
                var loadedData = JsonSerializer.Deserialize<JsonData>(jsonString, _jsonOptions);
                if (loadedData != null)
                {
                    _data = loadedData;
                    CargarPropiedadesNavegacion();
                }
            }
            else
            {
                SaveData();
            }
        }

        private void SaveData()
        {
            var jsonString = JsonSerializer.Serialize(_data, _jsonOptions);
            File.WriteAllText(_jsonFilePath, jsonString);
        }

        // Métodos para acceder y modificar los datos
        public List<TipoGasto> GetTiposGasto() => _data.TiposGasto;
        public List<FondoMonetario> GetFondos() => _data.Fondos;
        public List<RegistroGasto> GetGastos() => _data.Gastos;
        public List<Presupuesto> GetPresupuestos() => _data.Presupuestos;
        public List<Deposito> GetDepositos() => _data.Depositos;

        public void SaveTiposGasto(List<TipoGasto> tiposGasto)
        {
            _data.TiposGasto = tiposGasto;
            SaveData();
        }

        public void SaveFondos(List<FondoMonetario> fondos)
        {
            _data.Fondos = fondos;
            SaveData();
        }

        public void SaveGastos(List<RegistroGasto> gastos)
        {
            _data.Gastos = gastos;
            SaveData();
        }

        public void SavePresupuestos(List<Presupuesto> presupuestos)
        {
            _data.Presupuestos = presupuestos;
            SaveData();
        }

        public void SaveDepositos(List<Deposito> depositos)
        {
            _data.Depositos = depositos;
            SaveData();
        }

        public int GetNextGastoId()
        {
            return _data.Gastos.Count == 0 ? 1 : _data.Gastos.Max(g => g.Id) + 1;
        }

        public int GetNextDepositoId()
        {
            return _data.Depositos.Count == 0 ? 1 : _data.Depositos.Max(d => d.Id) + 1;
        }

        public void AddGasto(RegistroGasto gasto)
        {
            _data.Gastos.Add(gasto);
            SaveData();
        }

        public void AddDeposito(Deposito deposito)
        {
            _data.Depositos.Add(deposito);
            SaveData();
        }

        public RegistroGasto? GetGasto(int id)
        {
            var gasto = _data.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto != null)
            {
                // Cargar propiedades de navegación para el gasto específico
                gasto.Fondo = _data.Fondos.FirstOrDefault(f => f.Id == gasto.FondoId);
                foreach (var detalle in gasto.Detalles)
                {
                    detalle.TipoGasto = _data.TiposGasto.FirstOrDefault(t => t.Id == detalle.TipoGastoId);
                }
            }
            return gasto;
        }

        public Deposito? GetDeposito(int id)
        {
            var deposito = _data.Depositos.FirstOrDefault(d => d.Id == id);
            if (deposito != null)
            {
                deposito.FondoMonetario = _data.Fondos.FirstOrDefault(f => f.Id == deposito.FondoId);
            }
            return deposito;
        }

        public void UpdateGasto(RegistroGasto gasto)
        {
            var index = _data.Gastos.FindIndex(g => g.Id == gasto.Id);
            if (index != -1)
            {
                _data.Gastos[index] = gasto;
                SaveData();
            }
        }

        public void UpdateDeposito(Deposito deposito)
        {
            var index = _data.Depositos.FindIndex(d => d.Id == deposito.Id);
            if (index != -1)
            {
                _data.Depositos[index] = deposito;
                SaveData();
            }
        }

        public void DeleteGasto(int id)
        {
            var gasto = _data.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto != null)
            {
                _data.Gastos.Remove(gasto);
                SaveData();
            }
        }

        public void DeleteDeposito(int id)
        {
            var deposito = _data.Depositos.FirstOrDefault(d => d.Id == id);
            if (deposito != null)
            {
                _data.Depositos.Remove(deposito);
                SaveData();
            }
        }
    }
} 