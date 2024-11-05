using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCollabMaui.Service
{
    public class PageDialogService : IPageDialogService
    {
        private readonly Page _currentPage;

        public PageDialogService(Page currentPage)
        {
            _currentPage = currentPage;
        }

        public Task ShowAlertAsync(string title, string message, string cancel)
        {
            return _currentPage.DisplayAlert(title, message, cancel);
        }

        public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            return _currentPage.DisplayAlert(title, message, accept, cancel);
        }
        public async Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short, double fontSize = 16)
        {
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show();
        }
    }
}
