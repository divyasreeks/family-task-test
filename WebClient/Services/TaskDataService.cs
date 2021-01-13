using System;
using System.Collections.Generic;
using System.Linq;
using WebClient.Abstractions;
using WebClient.Shared.Models;
using Core.Extensions.ModelConversion;
using System.Threading.Tasks;
using Domain.Commands;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Domain.ViewModel;
using Domain.Queries;
using System.Text.Json;

namespace WebClient.Services
{
    public class TaskDataService : ITaskDataService
    {
        private readonly HttpClient httpClient;
        public TaskVm SelectedTask { get; private set; }
        public IEnumerable<TaskVm> tasks;
        public IEnumerable<TaskVm> Tasks => tasks;

        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            tasks = new List<TaskVm>();
            LoadTasks();
        }
        /// <summary>
        /// method to load all tasks
        /// </summary>
        private async void LoadTasks()
        {
            tasks = (await GetAllTasks()).Payload;
            TasksUpdated?.Invoke(this, null);
        }

        /// <summary>
        /// Method to set the selected task details when user selects a particular task from UI
        /// </summary>
        /// <param name="id"></param>
        public void SelectTask(Guid id)
        {
            if (tasks.All(taskVm => taskVm.Id != id)) return;
            {
                SelectedTask = tasks.SingleOrDefault(taskVm => taskVm.Id == id);
                TasksUpdated?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Toggle task completion status on check box click
        /// </summary>
        /// <param name="model"></param>

        public void ToggleTask(TaskVm model)
        {
            foreach (var taskModel in Tasks)
            {
                if (taskModel.Id == model.Id)
                {
                    model.IsComplete = !model.IsComplete;
                    UpdateTask(model);
                    taskModel.IsComplete = !taskModel.IsComplete;
                }
            }

            TasksUpdated?.Invoke(this, null);
        }

        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            try
            {
                return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        /// <summary>
        /// Method to fetch all tasks
        /// </summary>
        /// <returns></returns>
        private async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/{command.Id}", command);
        }


       /// <summary>
       /// New Task creation
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>

        public async Task AddTask(TaskVm model)
        {

            var result = await Create(model.ToCreateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }

        }


        private async Task<GetAllTasksQueryResult> GetAllMembers()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }


        public void AddTask(TaskModel model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public async Task UpdateTask(TaskVm model)
        {
            var result = await Update(model.ToUpdateTaskCommand());
            Console.WriteLine(JsonSerializer.Serialize(result));

            if (result != null)
            {
                var updatedList = (await GetAllMembers()).Payload;

                if (updatedList != null)
                {
                    tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
            }
        }

        public async void LoadAllTasks()
        {
            tasks = (await GetAllTasks()).Payload;
            TasksUpdated?.Invoke(this, null);
        }
    }
}