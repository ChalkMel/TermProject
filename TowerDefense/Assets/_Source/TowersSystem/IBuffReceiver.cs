using _Source.TowersSystem;
using _Source.TowersSystem.Runtimes;
namespace _Source
{
  public interface IBuffReceiver
  {
    void RegisterBuffer(BufferTowerRuntime buffer);
    void UnregisterBuffer(BufferTowerRuntime buffer);
    void OnBufferValueChanged();
  }
}