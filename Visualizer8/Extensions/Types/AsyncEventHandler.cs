namespace Visualizer8.Extensions.Types;

public delegate Task AsyncEventHandler<TArgs>(object? sender, TArgs e) where TArgs : EventArgs;