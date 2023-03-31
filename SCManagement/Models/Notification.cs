namespace SCManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public bool IsEnabled { get; set; }
        public NotificationType Type { get; set; }
        
        public enum NotificationType : int
        {
            Event_Created = 1,
            Event_Edited = 2,
            Event_Canceled = 3,
            Club_Edited = 4,
            Team_Created = 5,
            Team_Deleted = 6,
            Team_Edited = 7,
            Added_To_Team = 8,
            Removed_From_Team = 9,
            TrainingPlan_Assigned = 10,
            TrainingPlan_Deleted = 11,
            TrainingPlan_Edited = 12,
            MealPlan_Assigned = 13,
            MealPlan_Deleted = 14,
            MealPlan_Edited = 15,
            Goal_Assigned = 16,
            Goal_Deleted = 17,
            Goal_Edited = 18,
            Goal_Completed = 19,
            Athletes_Number_Almost_Full = 20,
            Payment_Late = 21,
            Payment_Received = 22,
            Subscription_Canceled = 23,
            Subscription_Expired = 24,
            Subscription_Extended = 25,
            Subscription_Renewed = 26,
            Subscription_Started = 27,
            Club_Quota_Update = 28,
            Plan_Descontinued = 29,
        }
    }
}
