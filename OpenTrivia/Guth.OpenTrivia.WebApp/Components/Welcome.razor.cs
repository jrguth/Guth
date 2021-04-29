using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.FirebaseDB;

namespace Guth.OpenTrivia.WebApp.Components
{
    public partial class Welcome
    {
        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public TriviaRealtimeDB RealtimeDB { get; set; }

        

        private Player _player;
        private Game _game;
        private string _playerName;
        private bool _gameLoading = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IDialogReference dialog = DialogService.Show<PlayerNameDialog>("Enter a name", new DialogOptions
                {
                    DisableBackdropClick = true,
                    Position = DialogPosition.Center
                });
                DialogResult result = await dialog.Result;
                _playerName = result.Data as string;
                StateHasChanged();
                _player = await RealtimeDB.CreatePlayer(_playerName);
                StateHasChanged();
            }
        }

        private async Task CreateGame(QuestionOptions questionOptions)
        {
            _gameLoading = true;
            StateHasChanged();
            _game = await RealtimeDB.CreateGame(questionOptions);
            _gameLoading = false;
            StateHasChanged();
            await DialogService.ShowMessageBox("Game Created!", $"Game ID: {_game.Id}");
        }
    }
}
