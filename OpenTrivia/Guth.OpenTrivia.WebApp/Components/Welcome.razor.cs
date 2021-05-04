using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public ISnackbar Snackbar { get; set; }

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

        private async Task ConnectToGame(string connectionCode)
        {
            _gameLoading = true;
            StateHasChanged();
            ConnectionCode code = await RealtimeDB.GetConnectionCode(connectionCode);
            if (code == null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Invalid connection code", Severity.Error);
            }
            else
            {
                _game = await RealtimeDB.GetGame(code.GameId);
                await RealtimeDB.AddPlayerToGame(_game.Id, _player.Id);
                await DialogService.ShowMessageBox("Connected to game!", $"Game ID: {_game.Id}");
            }
            _gameLoading = false;
            StateHasChanged();
            
        }

        private async Task CreateGame(QuestionOptions questionOptions)
        {
            _gameLoading = true;
            StateHasChanged();
            var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            ConnectionCode connection = await RealtimeDB.GenerateConnectionCode(cancellationSource.Token);
            _game = await RealtimeDB.CreateGame(connection.Code, questionOptions);
            await RealtimeDB.AddPlayerToGame(_game.Id, _player.Id);
            _gameLoading = false;
            StateHasChanged();
            await DialogService.ShowMessageBox("Game Created!", $"Connection code: {_game.ConnectionCode}");
        }
    }
}
