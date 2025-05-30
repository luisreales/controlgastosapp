namespace ControlGastosApp.Web.Services
{
    public interface IJsonDataService
    {
        /// <summary>
        /// Carga datos de una propiedad específica del archivo JSON
        /// </summary>
        /// <typeparam name="T">Tipo de datos a cargar</typeparam>
        /// <param name="property">Nombre de la propiedad en el JSON</param>
        /// <returns>Lista de objetos del tipo especificado</returns>
        List<T> LoadData<T>(string property) where T : class;

        /// <summary>
        /// Guarda datos en una propiedad específica del archivo JSON
        /// </summary>
        /// <typeparam name="T">Tipo de datos a guardar</typeparam>
        /// <param name="data">Lista de objetos a guardar</param>
        /// <param name="property">Nombre de la propiedad en el JSON</param>
        void SaveData<T>(List<T> data, string property) where T : class;
    }
} 