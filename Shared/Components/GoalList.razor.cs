using ApiClient;
using Microsoft.AspNetCore.Components;

namespace GoalQuest.Shared.Components
{
    public partial class GoalList
    {
        [Parameter]
        public List<ApiClient.GoalDto> Goals { get; set; } = new();

        [Parameter]
        public EventCallback<int> OnViewGoalDeatils { get; set; }
        [Parameter]
        public EventCallback<GoalDto> OnEditGoal { get; set; }
        [Parameter]
        public EventCallback<GoalDto> OnDeleteGoal { get; set; }
        private async Task ViewGoalDetails(int goalId)
        {
            await OnViewGoalDeatils.InvokeAsync(goalId);
        }

        private async Task EditGoal(GoalDto goalDto)
        {
            await OnEditGoal.InvokeAsync(goalDto);
        }
        private async Task DeleteGoal(GoalDto goalDto)
        {
            await OnDeleteGoal.InvokeAsync(goalDto);
        }

    }
}
