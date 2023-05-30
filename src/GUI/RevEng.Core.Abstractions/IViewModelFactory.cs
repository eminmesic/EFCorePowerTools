using RevEng.Core.Abstractions.Metadata;

namespace RevEng.Core.Abstractions.Model
{
    public interface IViewModelFactory
    {
        RoutineModel Create(string connectionString, ModuleModelFactoryOptions options);
    }
}
