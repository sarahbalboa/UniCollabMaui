using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCollabMaui.Service
{
    public interface IPageDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel);
        Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
        Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short, double fontSize = 16);

    }
}
