using MergeOpenApi.Configuration.Ui.Model.Enums;

namespace MergeOpenApi.Configuration.Ui.Model
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ServiceStatus Status { get; set; }
        public int Retry { get; set; }
        public bool Enabled { get; set; }
    }
}
