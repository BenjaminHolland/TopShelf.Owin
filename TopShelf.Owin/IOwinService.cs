namespace Topshelf.Owin
{
    /// <summary>
    /// Simplification of the Topshelf.ServiceControl interface
    /// </summary>
    public interface IOwinService
    {
        bool Stop();
        bool Start();
    }
}