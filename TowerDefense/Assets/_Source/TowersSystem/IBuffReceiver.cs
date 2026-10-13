using TowersSystem.Runtimes;

namespace TowersSystem
{
  public interface IBuffReceiver
  {
    void RegisterBuffer(BufferTowerRuntime buffer);
    void UnregisterBuffer(BufferTowerRuntime buffer);
    void OnBufferValueChanged();
  }
}