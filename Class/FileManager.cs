using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Oxcel.Class
{
    public class FileManager
    {
        public static void SaveTableToJson(ObservableCollection<ObservableCollection<CellItem>> cells, string fileName)
        {
            var serializedData = JsonConvert.SerializeObject(cells, Formatting.Indented);
            File.WriteAllText(fileName, serializedData);
        }

        public static ObservableCollection<ObservableCollection<CellItem>> LoadTableFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Прочитати шлях
                string json = File.ReadAllText(filePath);

                // Десеріалізація в ObservableCollection<ObservableCollection<CellItem>>
                return JsonConvert.DeserializeObject<ObservableCollection<ObservableCollection<CellItem>>>(json);
            }

            return null;
        }
    }
}
