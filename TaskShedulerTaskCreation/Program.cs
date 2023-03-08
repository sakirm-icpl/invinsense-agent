using Microsoft.Win32.TaskScheduler;

namespace TaskSchedulerTaskCreation
{
    class Program
    {
        public static void Main(string[] args) 
        {
            try
            {
                //TaskService taskService = new TaskService();
                TaskDefinition taskDefinition = TaskService.Instance.NewTask();
                //taskDefinition.Triggers.Add(new DailyTrigger { DaysInterval = 1 });
                taskDefinition.Actions.Add(new ExecAction(@"C:\Program Files\Notepad++\notepad++.exe"));
                taskDefinition.RegistrationInfo.Author = "Infopercept";
                taskDefinition.RegistrationInfo.Description = "Testing the Task Sheduler";
                if (TaskService.Instance.RootFolder.SubFolders["Infopercept"].Name=="Infopercept")
                {
                    TaskService.Instance.RootFolder.SubFolders["Infopercept"].RegisterTaskDefinition("Infopercept",taskDefinition).Run();
                    //Console.WriteLine(TaskService.Instance.RootFolder.SubFolders["Infopercept"].Name.ToString());
                }
                else
                {
                    var folder = TaskService.Instance.RootFolder.CreateFolder("Infopercept");
                    folder.RegisterTaskDefinition("Infopercept", taskDefinition).Run();
                }
               // taskService.RootFolder.RegisterTaskDefinition("Infopercept", taskDefinition, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount).Run();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

