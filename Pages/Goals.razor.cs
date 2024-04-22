
using ApiClient;
using GoalQuest.DTOs;
using GoalQuest.Shared.Components;
using GoalQuest.Shared.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GoalQuest.Pages
{
    public partial class Goals
    {
        private List<ApiClient.GoalDto> goals = new();
        private IEnumerable<ActivityDto> pagedData;
        private MudTable<ActivityDto> table = new();
        private int selectedGoalId;
        private int totalItems;
        private string searchString = null;
        private bool selectedGoal = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadGoals();
        }
        private async Task<TableData<ActivityDto>> ServerReload(TableState state)
        {
            state.Page += 1;
            IEnumerable<ActivityDto> data = new List<ActivityDto>();
            var resultData = await WebApiClient.PaginatedAsync(selectedGoalId, state.Page < 1 ? 1 : state.Page, state.PageSize);
            data = resultData.Activities.ToList();
            data = data.Where(task =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (task.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (task.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{task.CreatedDate}".Contains(searchString))
                    return true;
                return false;
            }).ToArray();
            totalItems = resultData.TotalRecords;
            switch (state.SortLabel)
            {
                case "name_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Name);
                    break;
                case "date_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.CreatedDate);
                    break;
                case "status_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Status);
                    break;
            }

            pagedData = data.ToArray();
            return new TableData<ActivityDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        private async Task CreateGoal()
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };

            var dialog = DialogService.Show<CreateUpdateGoalDialog>("Create Goal", options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                SnackBar.Add("Goal Created", Severity.Success);
                await LoadGoals();
                StateHasChanged();

            }

        }
        private async Task UpdateGoal(GoalDto goalDto)
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };
            
            var parameters = new DialogParameters { ["Goal"] = goalDto };
            var dialog = DialogService.Show<CreateUpdateGoalDialog>("Update Goal", parameters, options);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                SnackBar.Add("Goal Updated", Severity.Success);
                await LoadGoals();
                StateHasChanged();

            }
        }

        private async Task CreateActivity()
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };

            var parameters = new DialogParameters { ["GoalId"] = selectedGoalId };
            var dialog = DialogService.Show<CreateUpdateActivityDialog>("Create Activity", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                SnackBar.Add("Activity Created", Severity.Success);
                await LoadGoals();
                await table.ReloadServerData();
                StateHasChanged();

            }
        }
        private async Task CompleteActivity()
        {
            var updateActivityStatusRequest = new ApiClient.UpdateActivityStatusRequest();
            foreach (var activity in pagedData)
            {
                if (activity.Selected )
                {
                    updateActivityStatusRequest.Id = activity.Id;
                    await WebApiClient.UpdateActivityStatusAsync(activity.Id, updateActivityStatusRequest);
                    await LoadGoals();
                    await table.ReloadServerData();
                    StateHasChanged();
                }
            }
        }
        private async Task EditActivity(ActivityDto activity)
        {
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters { ["GoalId"] = selectedGoalId, ["Activity"] = activity };

            var dialog = DialogService.Show<CreateUpdateActivityDialog>("Update Activity", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                SnackBar.Add("Activity Updated", Severity.Success);
                await table.ReloadServerData();
                StateHasChanged();
            }

        }
        private async Task DeleteActivities()
        {
            if (pagedData.Count() < 1)
            {
                SnackBar.Add("There is not items in the List");
                return;
            }
            var parameters = new DialogParameters { ["ToDeleteElement"] = "Activities"};

            var dialog = DialogService.Show<DeleteConfirmationDialog>("Delete Confirmation", parameters);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                List<int> activitiesIds = new List<int>();
                foreach (var activity in pagedData)
                {
                    if (activity.Selected)
                    {
                        activitiesIds.Add(activity.Id);
                    }
                }
                await WebApiClient.DeleteActivitiesAsync(activitiesIds);
                await table.ReloadServerData();
                await LoadGoals();
                SnackBar.Add("Activities Deleted", Severity.Success);
                StateHasChanged();
            }

        }

        private async Task OnViewGoalDeatils(int goalId)
        {
            selectedGoal = true;
            selectedGoalId = goalId;
            await table.ReloadServerData();
            StateHasChanged();
        }
        private async Task OnDeleteGoal(GoalDto goalDto)
        {
            var parameters = new DialogParameters { ["Goal"] = goalDto, ["ToDeleteElement"] = goalDto.Name };
            var dialog = DialogService.Show<DeleteConfirmationDialog>("Delete Confirmation", parameters);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var goalsDto = await WebApiClient.GetAllGoalsAsync();
                goals = goalsDto.ToList();

                SnackBar.Add("Goal Deleted", Severity.Success);
                StateHasChanged();

            }

        }
        private async Task LoadGoals()
        {
            var goalsDto = await WebApiClient.GetAllGoalsAsync();
            goals = goalsDto.ToList();
        }
        async Task HandleCheckBoxChange(bool isChecked, int id)
        {
            var updateImportantStatusRequest = new ApiClient.UpdateActivityImportantStatusRequest() { Id = id, ImportantStatus = isChecked};
            await WebApiClient.UpdateActivityImportantStatusAsync(id, updateImportantStatusRequest);
            table.ReloadServerData();
            StateHasChanged();
        }
    }
}
