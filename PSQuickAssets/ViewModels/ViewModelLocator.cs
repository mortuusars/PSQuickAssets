﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace PSQuickAssets.ViewModels;

internal class ViewModelLocator
{
    public TaskBarViewModel TaskBarViewModel { get => DIKernel.ServiceProvider.GetRequiredService<TaskBarViewModel>(); }
    public AssetsWindowViewModel AssetsWindowViewModel { get => DIKernel.ServiceProvider.GetRequiredService<AssetsWindowViewModel>(); }
    public MainViewModel MainViewModel { get => DIKernel.ServiceProvider.GetRequiredService<MainViewModel>(); }
    public SettingsViewModel SettingsViewModel { get => DIKernel.ServiceProvider.GetRequiredService<SettingsViewModel>(); }
}