﻿namespace Ares.Core.Execution;

internal class PauseTokenSource : IDisposable
{
  private readonly ManualResetEventSlim _mre = new(true);
  private bool _disposed;
  public bool IsPaused => !_mre.IsSet;

  public PauseToken Token
  {
    get
    {
      ThrowIfDisposed();
      return new PauseToken(this);
    }
  }

  public void Dispose()
  {
    _mre.Dispose();
    _disposed = true;
  }

  public void Pause()
  {
    ThrowIfDisposed();
    _mre.Reset();
  }

  public void Resume()
  {
    ThrowIfDisposed();
    _mre.Set();
  }

  public void Wait(CancellationToken token)
  {
    ThrowIfDisposed();
    _mre.Wait(token);
  }

  private void ThrowIfDisposed()
  {
    if (_disposed)
      ThrowObjectDisposedException(GetType().Name);
  }

  private static void ThrowObjectDisposedException(string name)
  {
    throw new ObjectDisposedException(name);
  }
}
