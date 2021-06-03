using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Abstractions.Models;
using Guth.OpenTrivia.FirebaseDB;
using Guth.OpenTrivia.WebApp.Api;
using Guth.OpenTrivia.WebApp.Api.Games;

namespace Guth.OpenTrivia.WebApp.Components
{
    public partial class Welcome
    {
        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

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
        private HttpClient _httpClient;

        protected override void OnParametersSet()
        {
            _httpClient = HttpClientFactory.CreateClient("BaseApiClient");
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        }
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
                await InvokeAsync(StateHasChanged);
                _player = await RealtimeDB.CreatePlayer(_playerName);
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ConnectToGame(string connectionCode)
        {
            _gameLoading = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                ConnectionCode code = await _httpClient.GetFromJsonAsync<ConnectionCode>($"/api/v1/connectioncodes/{connectionCode}");
                _game = await _httpClient.PostReturnJsonAsync<Game>($"/api/v1/games/{code.Code}/join?playerId={_player.Id}&connectionCode={code.Code}");
                NavManager.NavigateTo($"/games/{_game.Id}/{_player.Id}");
            }
            catch(HttpRequestException)
            {
                await DisplayInvalidConnectionCodeAlert();
            }
        }

        private async Task DisplayInvalidConnectionCodeAlert()
        {
            Snackbar.Clear();
            Snackbar.Add("Invalid connection code", Severity.Error);
            _gameLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task CreateGame(QuestionOptions questionOptions)
        {
            try
            {
                _gameLoading = true;
                await InvokeAsync(StateHasChanged);
                var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                _game = await _httpClient.PostWithJsonAsync<CreateGameCommand, Game>(
                    $"/api/v1/games?playerId={_player.Id}",
                    new CreateGameCommand
                    {
                        Options = questionOptions
                    });
                NavManager.NavigateTo($"/games/{_game.Id}/{_player.Id}");
            }
            catch (HttpRequestException)
            {
                _gameLoading = false;
                await InvokeAsync(StateHasChanged);
                Snackbar.Add("Something went wrong. Please refresh the page and try again.", Severity.Error);
            }
        }
    }
}
