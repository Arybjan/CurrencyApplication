using CurrencyApplication.Models;
using CurrencyApplication.Services;
using System;
using System.Windows;

namespace CurrencyApplication
{
    public partial class App : Application
    {
        protected override async void OnExit(ExitEventArgs e)
        {
            var settingsService = new SettingsService();

            await settingsService.SaveAsync(new AppSettings
            {
                LastSessionTime = DateTime.Now
            });

            base.OnExit(e);
        }
    }
}