
namespace Phoenix.AutoDvdBackup.RippingServices
{
    public interface IRippingService
    {
        bool CanRip(string diskIdentifier);

        void Rip(string diskIdentifier, string outputLocation);
    }
}
